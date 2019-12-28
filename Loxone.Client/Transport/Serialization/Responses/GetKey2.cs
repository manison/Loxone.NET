// ----------------------------------------------------------------------
// <copyright file="GetKey2.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization.Responses
{
    using Newtonsoft.Json;

    internal sealed class GetKey2
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        [JsonProperty("key")]
        public string Key;

        [JsonProperty("salt")]
        public string Salt;

        [JsonProperty("hashAlg")]
        public string HashAlgorithm;

        #pragma warning restore CS0649
    }
}
