// ----------------------------------------------------------------------
// <copyright file="Uuid.cs">
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
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Unique 128b identifier to identify controls, inputs and outputs.
    /// </summary>
    /// <devdoc>
    /// Loxone's UUID is very much like <see cref="System.Guid"/> in .NET
    /// albeit with different string representation (missing dash in last
    /// segment, <see cref="ToString"/> method). Therefore this UUID
    /// implementation is just a thin wrapper around the Guid.
    /// </devdoc>
    public struct Uuid : IComparable, IComparable<Uuid>, IEquatable<Uuid>
    {
        private readonly Guid _uuid;

        public Uuid(ArraySegment<byte> bytes)
        {
            Contract.Requires(bytes.Count == 16);

            var b = (IList<byte>)bytes;

            _uuid = new Guid(
                ((int)b[3] << 24) | ((int)b[2] << 16) | ((int)b[1] << 8) | b[0],
                (short)(((int)b[5] << 8) | b[4]),
                (short)(((int)b[7] << 8) | b[6]),
                b[8],
                b[9],
                b[10],
                b[11],
                b[12],
                b[13],
                b[14],
                b[15]);
        }

        public Uuid(string s)
        {
            _uuid = ParseInternal(s);
        }

        private static Guid ParseInternal(string s)
        {
            s = s.Replace("-", String.Empty);
            return Guid.ParseExact(s, "N");
        }

        public static Uuid Parse(string s)
        {
            return new Uuid(s);
        }

        public override string ToString()
        {
            // xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
            // ->
            // xxxxxxxx-xxxx-xxxx-xxxxxxxxxxxxxxxx
            return _uuid.ToString("D").Remove(23, 1);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is Uuid other))
            {
                throw new ArgumentException(Strings.Uuid_ArgMustBeUuid, nameof(obj));
            }

            return _uuid.CompareTo(other._uuid);
        }

        public int CompareTo(Uuid other)
        {
            return _uuid.CompareTo(other._uuid);
        }

        public bool Equals(Uuid other)
        {
            return _uuid.Equals(other._uuid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Uuid other))
            {
                return false;
            }

            return _uuid.Equals(other._uuid);
        }

        public override int GetHashCode()
        {
            return _uuid.GetHashCode();
        }

        public static bool operator ==(Uuid a, Uuid b)
        {
            return a._uuid == b._uuid;
        }

        public static bool operator !=(Uuid a, Uuid b)
        {
            return !(a == b);
        }
    }
}
