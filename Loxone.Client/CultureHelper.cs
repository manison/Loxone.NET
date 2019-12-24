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
    using System.Globalization;
    using System.Linq;

    internal static class CultureHelper
    {
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
    }
}
