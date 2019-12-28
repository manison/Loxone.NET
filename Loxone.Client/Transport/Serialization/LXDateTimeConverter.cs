// ----------------------------------------------------------------------
// <copyright file="LXDateTimeConverter.cs">
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

    internal sealed class LXDateTimeConverter : JsonConverter
    {
        private static readonly DateTime _epochStart = new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public override bool CanConvert(Type objectType) => objectType == typeof(DateTime);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.Value)
            {
                case long l:
                    return FromInt64(l);
                case double d:
                    return FromDouble(d);
                default:
                    throw new JsonSerializationException(Strings.LXDateTimeConverter_InvalidFormat);
            }
        }

        private static DateTime FromInt64(long l) => FromDouble(l);

        private static DateTime FromDouble(double d) => _epochStart.AddSeconds(d);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = (DateTime)value;

            // local Miniserver time
            date = date.ToLocalTime();

            double d = date.Subtract(_epochStart).TotalSeconds;
            writer.WriteValue(d);
        }
    }
}
