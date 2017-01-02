// ----------------------------------------------------------------------
// <copyright file="CultureHelper.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Collections.Generic;
    using System.Globalization;

    internal static class CultureHelper
    {
        private static IDictionary<string, string> _threeLetterWindowsNameMappings = new Dictionary<string, string>()
        {
            ["CSY"] = "cs-CZ",
            ["DEU"] = "de",
            ["ENG"] = "en-UK",
            ["ENU"] = "en-US",
        };

        public static CultureInfo GetCultureByThreeLetterWindowsLanguageName(string languageName)
        {
            CultureInfo culture = null;
            string name;

            if (_threeLetterWindowsNameMappings.TryGetValue(languageName, out name))
            {
                culture = new CultureInfo(name);
            }

            return culture;
        }
    }
}
