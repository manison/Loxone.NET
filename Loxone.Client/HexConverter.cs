// ----------------------------------------------------------------------
// <copyright file="HexConverter.cs">
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
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Converts string of hexadecimal digits into the byte array and vice versa.
    /// </summary>
    internal static class HexConverter
    {
        public static string FromByteArray(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            var s = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                s.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return s.ToString();
        }

        public static byte[] FromString(string s, params char[] allowedSeparators)
        {
            Contract.Requires(s != null);

            byte[] bytes;
            bool hasSeparators = (allowedSeparators != null && s.IndexOfAny(allowedSeparators) >= 0);
            if (hasSeparators)
            {
                bytes = new byte[(s.Length + 1) / 3];
            }
            else
            {
                if (s.Length % 2 > 0)
                {
                    // Should be even.
                    throw new FormatException(Strings.HexConverter_BadFormat);
                }

                bytes = new byte[s.Length / 2];
            }

            int j = 0;
            int validCount = 0;

            for (int i = 0; i < s.Length; i++)
            {
                int value = s[i];

                if (value >= 0x30 && value <= 0x39)
                {
                    value -= 0x30;
                }
                else if (value >= 0x41 && value <= 0x46)
                {
                    value -= 0x37;
                }
                else if (allowedSeparators != null && allowedSeparators.Contains(s[i]))
                {
                    if (validCount == 2)
                    {
                        validCount = 0;
                        continue;
                    }
                    else
                    {
                        throw new FormatException(Strings.HexConverter_BadFormat);
                    }
                }
                else
                {
                    throw new FormatException(Strings.HexConverter_BadFormat);
                }

                if (validCount % 2 == 0)
                {
                    bytes[j] = (byte)(value << 4);
                }
                else
                {
                    bytes[j++] |= (byte)value;
                }

                validCount++;
            }

            return bytes;
        }
    }
}
