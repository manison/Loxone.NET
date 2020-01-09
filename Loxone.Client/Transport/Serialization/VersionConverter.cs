// ----------------------------------------------------------------------
// <copyright file="VersionConverter.cs">
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

    internal sealed class VersionConverter : JsonConverter<Version>
    {
        public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Version.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
