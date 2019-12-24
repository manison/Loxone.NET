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
    using Newtonsoft.Json;

    // There should be support for Color type in .NET Standard 1.7
    internal sealed class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Color);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string s = reader.Value as string;
            if (s == null || s.Length != 7 || s[0] != '#')
            {
                throw new JsonSerializationException(Strings.ColorConverter_InvalidFormat);
            }

            var rgb = int.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            return Color.FromArgb(rgb | unchecked((int)0xFF000000));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rgb = ((Color)value).ToArgb();
            writer.WriteValue(
                string.Concat(
                    "#",
                    ((rgb >> 16) & 0xFF).ToString("X2", CultureInfo.InvariantCulture),
                    ((rgb >>  8) & 0xFF).ToString("X2", CultureInfo.InvariantCulture),
                    ( rgb        & 0xFF).ToString("X2", CultureInfo.InvariantCulture)
                ));
        }
    }
}
