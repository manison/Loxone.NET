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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Newtonsoft.Json;

    internal sealed class JsonWithinStringConverter : JsonConverter
    {
        private HashSet<Type> _targetTypes;

        public ICollection<Type> TargetTypes
        {
            get { return _targetTypes; }
        }

        public JsonWithinStringConverter()
        {
            _targetTypes = new HashSet<Type>();
        }

        public JsonWithinStringConverter(Type targetType)
            : this()
        {
            Contract.Requires(targetType != null);

            _targetTypes.Add(targetType);
        }

        public JsonWithinStringConverter(params Type[] targetTypes)
        {
            Contract.Requires(targetTypes != null);

            foreach (var type in targetTypes)
            {
                _targetTypes.Add(type);
            }
        }

        public override bool CanConvert(Type objectType) => _targetTypes.Contains(objectType);

        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string s = reader.Value as string;
            return JsonConvert.DeserializeObject(
                s,
                objectType,
                serializer.Converters
                    .Where(c => c.GetType() != typeof(JsonWithinStringConverter))
                    .ToArray());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
