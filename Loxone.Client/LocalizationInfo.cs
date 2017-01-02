// ----------------------------------------------------------------------
// <copyright file="LocalizationInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public sealed class LocalizationInfo
    {
        private Transport.MiniserverInfo _msInfo;

        private CultureInfo _culture;

        public CultureInfo Culture
        {
            get
            {
                if (_culture == null)
                {
                    _culture = CultureHelper.GetCultureByThreeLetterWindowsLanguageName(_msInfo.LanguageCode);
                }

                return _culture;
            }
        }

        public string CurrencySymbol => _msInfo.Currency;

        public TemperatureUnit TemperatureUnit => (TemperatureUnit)_msInfo.TemperatureUnit;

        public string CategoryTitle => _msInfo.CategoryTitle;

        public string RoomTitle => _msInfo.RoomTitle;

        internal LocalizationInfo(Transport.MiniserverInfo msInfo)
        {
            Contract.Requires(msInfo != null);
            this._msInfo = msInfo;
        }
    }
}
