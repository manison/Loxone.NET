// ----------------------------------------------------------------------
// <copyright file="MessageInfoFlags.cs">
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

    [Flags]
    internal enum MessageInfoFlags
    {
        None = 0,
        EstimatedLength = 0x80,
    }
}
