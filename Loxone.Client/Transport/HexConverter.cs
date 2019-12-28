// ----------------------------------------------------------------------
// <copyright file="HexConverter.cs">
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
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Converts string of hexadecimal digits into the byte array and vice versa.
    /// </summary>
    internal static class HexConverter
    {
        private static char GetHexValue(int i)
        {
            if (i < 10)
            {
                return (char)(i + '0');
            }

            return (char)(i - 10 + 'A');
        }

        public static void FromByteArray(char[] dest, int offset, byte[] source)
        {
            Contract.Requires(source != null);
            Contract.Requires(dest != null);
            Contract.Requires(offset >= 0 && offset < dest.Length);

            for (int i = 0; i < source.Length; i++)
            {
                byte b = source[i];
                dest[i * 2 + offset + 0] = GetHexValue(b / 16);
                dest[i * 2 + offset + 1] = GetHexValue(b % 16);
            }
        }

        public static string FromByteArray(byte[] bytes)
        {
            Contract.Requires(bytes != null);

            char[] chars = new char[bytes.Length * 2];
            FromByteArray(chars, 0, bytes);

            return new string(chars);
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
