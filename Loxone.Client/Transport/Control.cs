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
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal sealed class Control
    {
        [JsonPropertyName("uuidAction")]
        public Uuid Uuid { get; set; }

        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string ControlType { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsSecured { get; set; }

        public int DefaultRating { get; set; }

        public Uuid? Room { get; set; }

        [JsonPropertyName("cat")]
        public Uuid? Category { get; set; }

        public IDictionary<string, Uuid> States { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> ExtensionData { get; set; }
    }
}
