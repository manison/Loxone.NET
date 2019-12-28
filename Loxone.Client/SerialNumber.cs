// ----------------------------------------------------------------------
// <copyright file="SerialNumber.cs">
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
    using System.Text;

    public struct SerialNumber : IEquatable<SerialNumber>
    {
        private byte[] _bytes;

        private SerialNumber(byte[] bytes)
        {
            this._bytes = bytes;
        }

        public static SerialNumber Parse(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return default;
            }

            // Colon, dash and space can be all used as MAC address separators
            // (miniserver serial number), dot is used to separate digits in
            // 1-wire device numbers.
            var bytes = Transport.HexConverter.FromString(s, ':', '-', ' ', '.');
            return new SerialNumber(bytes);
        }

        public override string ToString()
        {
            if (_bytes == null)
            {
                return string.Empty;
            }

            var s = new StringBuilder(_bytes.Length * 2);
            for (int i = 0; i < _bytes.Length; i++)
            {
                s.Append(_bytes[i].ToString("X2", CultureInfo.InvariantCulture));
            }

            return s.ToString();
        }

        public override int GetHashCode()
        {
            uint hash = 0;

            if (_bytes != null)
            {
                for (int i = 0; i < _bytes.Length; i++)
                {
                    hash <<= 5;
                    hash |= _bytes[i];
                }
            }

            return (int)hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is SerialNumber)
            {
                return Equals((SerialNumber)obj);
            }

            return false;
        }

        public bool Equals(SerialNumber other)
        {
            if (this._bytes == other._bytes)
            {
                return true;
            }

            if (this._bytes.Length == other._bytes.Length)
            {
                return other._bytes.SequenceEqual(this._bytes);
            }

            return false;
        }
    }
}
