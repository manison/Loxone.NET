// ----------------------------------------------------------------------
// <copyright file="MiniserverInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Transport.Serialization;

    internal sealed class MiniserverInfo
    {
        [JsonPropertyName("serialNr")]
        public SerialNumber SerialNumber { get; set; }

        [JsonPropertyName("msName")]
        public string MiniserverName { get; set; }

        public string ProjectName { get; set; }

        public string Location { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime HeatPeriodStart { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime HeatPeriodEnd { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime CoolPeriodStart { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime CoolPeriodEnd { get; set; }

        [JsonPropertyName("catTitle")]
        public string CategoryTitle { get; set; }

        public string RoomTitle { get; set; }

        public int MiniserverType { get; set; }

        public string LocalUrl { get; set; }

        public string RemoteUrl { get; set; }

        public string LanguageCode { get; set; }

        public string Currency { get; set; }

        [JsonPropertyName("tempUnit")]
        public int TemperatureUnit { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> ExtensionData { get; set; }
    }
}
