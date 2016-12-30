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
    using Loxone.Client.Transport.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class MiniserverInfo
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        [JsonProperty("serialNr")]
        public SerialNumber SerialNumber;

        [JsonProperty("msName")]
        public string MiniserverName;

        [JsonProperty("projectName")]
        public string ProjectName;

        [JsonProperty("location")]
        public string Location;

        [JsonProperty("heatPeriodStart")]
        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime HeatPeriodStart;

        [JsonProperty("heatPeriodEnd")]
        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime HeatPeriodEnd;

        [JsonProperty("coolPeriodStart")]
        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime CoolPeriodStart;

        [JsonProperty("coolPeriodEnd")]
        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime CoolPeriodEnd;

        [JsonProperty("catTitle")]
        public string CategoryTitle;

        [JsonProperty("roomTitle")]
        public string RoomTitle;

        [JsonProperty("miniserverType")]
        public int MiniserverType;

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData;

        #pragma warning restore CS0649
    }
}
