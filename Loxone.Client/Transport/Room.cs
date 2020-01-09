// ----------------------------------------------------------------------
// <copyright file="Room.cs">
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

    internal sealed class Room
    {
        public Uuid Uuid { get; set; }

        public string Name { get; set; }

        public bool IsFavorite { get; set; }

        public int DefaultRating { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> ExtensionData { get; set; }
    }
}
