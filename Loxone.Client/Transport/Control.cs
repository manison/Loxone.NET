// ----------------------------------------------------------------------
// <copyright file="Control.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class Control
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        [JsonProperty("uuidAction")]
        public Uuid Uuid;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("type")]
        public string ControlType;

        [JsonProperty("isFavorite")]
        public bool IsFavorite;

        [JsonProperty("isSecured")]
        public bool IsSecured;

        [JsonProperty("defaultRating")]
        public int DefaultRating;

        [JsonProperty("room")]
        public Uuid? Room;

        [JsonProperty("cat")]
        public Uuid? Category;

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData;

        #pragma warning restore CS0649
    }
}
