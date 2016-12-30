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
    using Newtonsoft.Json;

    internal sealed class LXResponse<TValue>
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        private struct Root
        {
            [JsonProperty("LL")]
            public LXResponse<TValue> Value;
        }

        [JsonProperty("control")]
        public string Control;

        [JsonProperty("Code")]
        public int Code;

        [JsonProperty("value")]
        public TValue Value;

        #pragma warning restore CS0649

        public static LXResponse<TValue> Deserialize(string s)
        {
            var root = Serialization.SerializationHelper.Deserialize<Root>(s);
            return root.Value;
        }
    }
}
