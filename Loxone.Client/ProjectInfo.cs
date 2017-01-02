// ----------------------------------------------------------------------
// <copyright file="ProjectInfo.cs">
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

    public sealed class ProjectInfo
    {
        private Transport.MiniserverInfo _msInfo;

        public string Name => _msInfo.ProjectName;

        public string Location => _msInfo.Location;

        public TimePeriod HeatingPeriod => new TimePeriod(_msInfo.HeatPeriodStart, _msInfo.HeatPeriodEnd);

        public TimePeriod CoolingPeriod => new TimePeriod(_msInfo.CoolPeriodStart, _msInfo.CoolPeriodEnd);

        internal ProjectInfo(Transport.MiniserverInfo msInfo)
        {
            Contract.Requires(msInfo != null);
            this._msInfo = msInfo;
        }
    }
}
