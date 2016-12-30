// ----------------------------------------------------------------------
// <copyright file="SerialNumberConverter.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization
{
    using System;
    using Newtonsoft.Json;

    internal sealed class SerialNumberConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SerialNumber);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string s = reader.Value as string;
            return SerialNumber.Parse(s);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Just use SerialNumber.ToString method.
            writer.WriteValue(value.ToString());
        }
    }
}
