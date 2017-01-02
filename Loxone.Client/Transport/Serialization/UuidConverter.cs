// ----------------------------------------------------------------------
// <copyright file="UuidConverter.cs">
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

    internal sealed class UuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Uuid) || objectType == typeof(Uuid?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool nullable = objectType == typeof(Uuid?);

            if (reader.TokenType == JsonToken.Null && nullable)
            {
                return null;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return Uuid.Parse(reader.Value as string);
            }

            throw new JsonSerializationException(Strings.UuidConverter_UnexpectedValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
