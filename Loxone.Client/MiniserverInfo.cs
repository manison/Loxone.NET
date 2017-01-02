// ----------------------------------------------------------------------
// <copyright file="MiniserverInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Diagnostics.Contracts;

    public sealed class MiniserverInfo
    {
        private Transport.MiniserverInfo _msInfo;

        public string MiniserverName => _msInfo.MiniserverName;

        public SerialNumber SerialNumber => _msInfo.SerialNumber;

        public MiniserverType MiniserverType => (MiniserverType)_msInfo.MiniserverType;

        public string LocalAddress => _msInfo.LocalUrl;

        public string RemoteAddress => _msInfo.RemoteUrl;

        internal MiniserverInfo(Transport.MiniserverInfo msInfo)
        {
            Contract.Requires(msInfo != null);
            this._msInfo = msInfo;
        }
    }
}
