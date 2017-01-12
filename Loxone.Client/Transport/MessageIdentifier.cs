// ----------------------------------------------------------------------
// <copyright file="MessageIdentifier.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    internal enum MessageIdentifier
    {
        Text = 0,
        Binary = 1,
        ValueStates = 2,
        TextStates = 3,
        DayTimerStates = 4,
        OutOfService = 5,
        KeepAlive = 6
    }
}
