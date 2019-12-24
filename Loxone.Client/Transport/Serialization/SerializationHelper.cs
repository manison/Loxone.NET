// ----------------------------------------------------------------------
// <copyright file="SerializationHelper.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization
{
    using System.IO;
    using Newtonsoft.Json;

    internal static class SerializationHelper
    {
        private static JsonConverter[] _converters = new JsonConverter[]
        {
            new JsonWithinStringConverter(typeof(Api)),
            new SerialNumberConverter(),
            new UuidConverter(),
            new ColorConverter(),
        };

        public static JsonSerializer CreateSerializer()
        {
            var serializer = new JsonSerializer();
            serializer.DateFormatString = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            
            for (int i = 0; i < _converters.Length; i++)
            {
                serializer.Converters.Add(_converters[i]);
            }

            return serializer;
        }

        public static T Deserialize<T>(TextReader reader)
        {
            var serializer = CreateSerializer();
            using (var jsonReader = new JsonTextReader(reader) { CloseInput = false })
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public static T Deserialize<T>(string s)
        {
            using (var reader = new StringReader(s))
            {
                return Deserialize<T>(reader);
            }
        }
    }
}
