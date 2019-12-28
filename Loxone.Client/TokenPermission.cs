// ----------------------------------------------------------------------
// <copyright file="TokenPermission.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    public enum TokenPermission
    {
        /// <value>
        /// Token will have a short lifespan.
        /// </value>
        Web = 2,

        /// <summary>
        /// Token will last for a longer period.
        /// </summary>
        App = 4,
    }
}
