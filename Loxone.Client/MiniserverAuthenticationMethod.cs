// ----------------------------------------------------------------------
// <copyright file="MiniserverAuthenticationMethod.cs">
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
    /// Specifies method used to authenticate to Miniserver.
    /// </summary>
    public enum MiniserverAuthenticationMethod
    {
        /// <summary>
        /// Automatically decide authentication method based on Miniserver version.
        /// </summary>
        Default,

        /// <summary>
        /// Use password based authentication.
        /// </summary>
        /// <remarks>
        /// This is the default for versions 8 and below.
        /// </remarks>
        Password,

        /// <summary>
        /// Use token based authentication.
        /// </summary>
        /// <remarks>
        /// This is the default for versions 9 and above.
        /// </remarks>
        Token
    }
}
