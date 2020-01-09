// ----------------------------------------------------------------------
// <copyright file="ColorConverter.cs">
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
    using System.Drawing;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal sealed class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string s = reader.GetString();
            if (s == null || s.Length != 7 || s[0] != '#')
            {
                throw new FormatException(Strings.ColorConverter_InvalidFormat);
            }

            int rgb = Int32.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            return Color.FromArgb(rgb | unchecked((int)0xFF000000));
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            int rgb = value.ToArgb();
            writer.WriteStringValue(
                String.Concat(
                    "#",
                    ((rgb >> 16) & 0xFF).ToString("X2", CultureInfo.InvariantCulture),
                    ((rgb >>  8) & 0xFF).ToString("X2", CultureInfo.InvariantCulture),
                    ( rgb        & 0xFF).ToString("X2", CultureInfo.InvariantCulture)
                ));
        }
    }
}
