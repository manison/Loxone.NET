// ----------------------------------------------------------------------
// <copyright file="GetToken.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization.Responses
{
    using System;
    using Newtonsoft.Json;

    internal sealed class GetToken
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        [JsonProperty("token")]
        public string Token;

        [JsonProperty("key")]
        public string Key;

        [JsonProperty("validUntil")]
        [JsonConverter(typeof(LXDateTimeConverter))]
        public DateTime ValidUntil;

        [JsonProperty("tokenRights")]
        public int TokenRights;

        [JsonProperty("unsecurePass")]
        public bool UnsecurePassword;

        #pragma warning restore CS0649
    }
}
