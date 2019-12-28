// ----------------------------------------------------------------------
// <copyright file="MiniserverLimitedInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System;

    public class MiniserverLimitedInfo
    {
        private SerialNumber _serialNumber;

        public SerialNumber SerialNumber => _serialNumber;

        private Version _firmwareVersion;

        public Version FirmwareVersion => _firmwareVersion;

        internal void Update(Transport.Serialization.Responses.Api api)
        {
            _serialNumber = api.SerialNumber;
            _firmwareVersion = api.Version;
        }

        internal MiniserverLimitedInfo()
        {
        }
    }
}
