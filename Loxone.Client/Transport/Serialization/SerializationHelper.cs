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
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Transport.Serialization.Responses;

    internal static class SerializationHelper
    {
        private static readonly JsonConverter[] _defaultConvertes = new JsonConverter[]
        {
            new SerialNumberConverter(),
            new UuidConverter(),
            new ColorConverter(),
            new VersionConverter(),
            new QuotedInt32Converter(),
            new FormattedDateTimeConverter("yyyy'-'MM'-'dd' 'HH':'mm':'ss"),

        };

        private static readonly JsonSerializerOptions _options = InitializeOptions();

        internal static readonly JsonSerializerOptions DefaultOptions = CreateDefaultOptions();

        private static JsonSerializerOptions CreateDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            for (int i = 0; i < _defaultConvertes.Length; i++)
            {
                options.Converters.Add(_defaultConvertes[i]);
            }

            return options;
        }

        private static JsonSerializerOptions InitializeOptions()
        {
            var options = CreateDefaultOptions();
            options.Converters.Add(new JsonWithinStringConverter<Api>());
            return options;
        }

        public static T Deserialize<T>(string s) => JsonSerializer.Deserialize<T>(s, _options);

        public static ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
            => JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken);

        public static Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken)
            => JsonSerializer.SerializeAsync<T>(stream, value, _options, cancellationToken);
    }
}
