// ----------------------------------------------------------------------
// <copyright file="LXResponse.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System.Text.Json.Serialization;

    internal sealed class LXResponse<TValue>
    {
        private struct Root
        {
            [JsonPropertyName("LL")]
            public LXResponse<TValue> Value { get; set; }
        }

        public string Control { get; set; }

        public int Code { get; set; }

        public TValue Value { get; set; }

        public static LXResponse<TValue> Deserialize(string s)
        {
            var root = Serialization.SerializationHelper.Deserialize<Root>(s);
            return root.Value;
        }
    }
}
