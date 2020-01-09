// ----------------------------------------------------------------------
// <copyright file="TimePeriodConverter.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization
{
    internal sealed class TimePeriodConverter : FormattedDateTimeConverter
    {
        public TimePeriodConverter() : base("MM'-'dd")
        {
        }
    }
}
