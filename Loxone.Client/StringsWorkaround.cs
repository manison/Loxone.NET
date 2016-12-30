// ----------------------------------------------------------------------
// <copyright file="StringsWorkaround.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    // https://github.com/dotnet/roslyn-project-system/issues/747

    internal static class Strings
    {
        public const string HexConverter_BadFormat = "Hex string format is invalid.";
        public const string MiniserverCommandException_MessageFmt = "Miniserver command failed with status code {0}.";
        public const string MiniserverConnection_MustBeSetBeforeOpenFmt = "Property '{0}' must be set before opening the connection.";
        public const string MiniserverTransportException_Message = "Message received from Miniserver has invalid format.";
    }
}
