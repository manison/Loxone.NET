// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class StructureFile
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        [JsonProperty("lastModified")]
        public DateTime LastModified;

        [JsonProperty("msInfo")]
        public MiniserverInfo MiniserverInfo;

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData;

        #pragma warning restore CS0649
    }
}
