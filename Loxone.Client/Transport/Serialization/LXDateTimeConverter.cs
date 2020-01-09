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
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal sealed class LXDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime _epochStart = new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => FromDouble(reader.GetDouble());

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var date = (DateTime)value;

            // local Miniserver time
            date = date.ToLocalTime();

            double d = date.Subtract(_epochStart).TotalSeconds;
            writer.WriteNumberValue(d);
        }

        private static DateTime FromDouble(double d) => _epochStart.AddSeconds(d);
    }
}
