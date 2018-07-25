﻿/* 
 * Copyright (c) 2014, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using System;
using Hl7.Fhir.Specification;

namespace Hl7.Fhir.Serialization
{
    [Obsolete("Replace useage of class with the FhirJsonWriter from the Hl7.Fhir.Serialization assembly, and call a Write(), which accepts IElementNavigator")]
    public interface IFhirWriter : IDisposable
    {
        void WriteStartRootObject(string name, bool contained);
        void WriteEndRootObject(bool contained);

        void WriteStartProperty(string name);
        void WriteEndProperty();

        void WriteStartComplexContent();
        void WriteEndComplexContent();

        void WritePrimitiveContents(object value,XmlRepresentation xmlFormatHint);

        void WriteStartArray();
        //void WriteStartArrayElement(string name);
        //void WriteEndArrayElement();
        void WriteEndArray();

        bool HasValueElementSupport { get; }
    }
}
