// ----------------------------------------------------------------------
// <copyright file="TimePeriod.cs">
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
    using System.Globalization;

    public struct TimePeriod
    {
        private readonly DateTime _start;

        public DateTime Start => _start;

        private readonly DateTime _end;

        public DateTime End => _end;

        public TimePeriod(DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:m} - {1:m}", _start, _end);
        }
    }
}
