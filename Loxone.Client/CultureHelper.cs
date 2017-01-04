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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal static class CultureHelper
    {
#if NETFX
        public static CultureInfo GetCultureByThreeLetterWindowsLanguageName(string languageName)
        {
            var all = CultureInfo.GetCultures(CultureTypes.AllCultures);

            // Try specific cultures first.
            var culture = all
                .Where(c => !c.IsNeutralCulture &&
                       string.Equals(c.ThreeLetterWindowsLanguageName, languageName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (culture == null)
            {
                // Try neutral culture if a specific was not found.
                culture = all
                .Where(c => c.IsNeutralCulture &&
                       string.Equals(c.ThreeLetterWindowsLanguageName, languageName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            }

            return culture;
        }
#else
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
#endif
    }
}
