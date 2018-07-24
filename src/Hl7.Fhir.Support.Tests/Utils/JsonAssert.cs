﻿/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hl7.Fhir.Utility;
using Newtonsoft.Json.Linq;
using System;

namespace Hl7.Fhir.Tests
{
    public class JsonAssert
    {
        public static void AreSame(string expected, string actual)
        {
            var exp = SerializationUtil.JObjectFromJsonText(expected);
            var act = SerializationUtil.JObjectFromJsonText(actual);

            AreSame(exp, act);
        }

        public static void AreSame(JToken expected, JToken actual)
        {
            if (expected.Type != actual.Type)
                throw new AssertFailedException($"Token types are not the same at {actual.Path} (actual: {actual.Type}, expected: {expected.Type})");

            switch (expected)
            {
                case JValue exV:
                    {
                        JValue acV = (JValue)actual;
                        compareValues(exV.Value, acV.Value, expected.Path);
                        return;
                    }
                case JProperty exP:
                    {
                        JProperty acP = (JProperty)actual;
                        if (exP.Name != acP.Name)
                            throw new AssertFailedException($"Expected element '{exP.Name}', actual '{acP.Name}' at '{exP.Path}'");
                        AreSame(exP.Value, acP.Value);
                        return;
                    }             
                case JContainer exC:
                    {
                        JContainer acC = (JContainer)actual;
                        areSame(exC, acC);
                        return;
                    }
            }
        }

        private static void areSame(JContainer expected, JContainer actual)
        {
            if (expected.Count != actual.Count)
                throw new AssertFailedException($"Number of elements are not the same in container {expected.Path}");

            bool isRelevant(JToken t)
            {
                if (t is JProperty p)
                    return p.Name != "fhir_comments";
                else
                    return true;
            }

            var expectedList = expected.Children().Select(c=>isRelevant(c)).ToList();
            var actualList = actual.Children().Select(c=>isRelevant(c)).ToList();

            for (int elemNr = 0; elemNr < expectedList.Count(); elemNr++)
            {
                var ex = expectedList[elemNr];
                var ac = actualList[elemNr];

                AreSame(ex, ac);
            }
        }

        public static void compareValues(object exp, object act, string path)
        {
            if (exp == null && act == null) return;
            else if (exp != null && act != null)
            {
                if(exp.GetType() != act.GetType())
                    throw new AssertFailedException($"The types of the values are not the same at '{path}'");

                object expected = exp;
                object actual = act;

                if (exp is string expS)
                {
                    var actS = (string)act;
                    // Hack for timestamps, binaries and narrative html
                    if (expS.EndsWith("+00:00")) expS = expS.Replace("+00:00", "Z");
                    if (actS.EndsWith("+00:00")) actS = actS.Replace("+00:00", "Z");
                    if (expS.Contains(".000+")) expS = expS.Replace(".000+", "+");
                    if (actS.Contains(".000+")) actS = actS.Replace(".000+", "+");
                    if (expS.Contains(".000Z")) expS = expS.Replace(".000Z", "Z");
                    if (actS.Contains(".000Z")) actS = actS.Replace(".000Z", "Z");
                    actS = actS.Replace("\n", "");
                    actS = actS.Replace("\r", "");
                    actS = actS.Replace(" ", "");
                    expS = expS.Replace("\n", "");
                    expS = expS.Replace("\r", "");
                    expS = expS.Replace(" ", "");

                    expected = expS.Trim();
                    actual = actS.Trim();
                }

                if (!Object.Equals(expected,actual))
                {
                    throw new AssertFailedException($"Values are not equal at '{path}', expected '{expected}', actual '{actual}'");
                }
            }
            else
            {
                throw new AssertFailedException($"One of the values (but not both) are null at '{path}'");
            }
        }
    }
}