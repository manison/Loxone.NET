// ----------------------------------------------------------------------
// <copyright file="JsonWithinStringConverter.cs">
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

    internal sealed class JsonWithinStringConverter<T> : JsonConverter<T>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string s = reader.GetString();
            s = s.Replace('\'', '"');
            return JsonSerializer.Deserialize<T>(s, SerializationHelper.DefaultOptions);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => new NotSupportedException();
    }
}
