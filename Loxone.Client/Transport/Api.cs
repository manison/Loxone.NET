// ----------------------------------------------------------------------
// <copyright file="Api.cs">
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
    using Newtonsoft.Json;

    internal sealed class Api
    {
        // Suppress 'Field is never assigned to, and will always have its
        // default value' warning.
        // Justification: Fields are set during deserialization.
        #pragma warning disable CS0649

        [JsonProperty("snr")]
        public SerialNumber SerialNumber;

        [JsonProperty("version")]
        public Version Version;

        #pragma warning restore CS0649
    }
}
