// ----------------------------------------------------------------------
// <copyright file="MiniserverType.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    /// <summary>
    /// Identifies Miniserver type.
    /// </summary>
    public enum MiniserverType
    {
        /// <summary>
        /// Miniserver Gen 1.
        /// </summary>
        MiniserverGen1,

        /// <summary>
        /// Miniserver Go.
        /// </summary>
        MiniserverGo,

        /// <summary>
        /// Miniserver Gen 2.
        /// </summary>
        /// <devdoc>Need to verify this value, Loxone documentation to structure file has not been updated yet.</devdoc>
        MiniserverGen2,
    }
}
