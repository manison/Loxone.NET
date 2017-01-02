// ----------------------------------------------------------------------
// <copyright file="Room.cs">
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

    public sealed class Room : IEquatable<Room>
    {
        private Transport.Room _innerRoom;

        public Uuid Uuid => _innerRoom.Uuid;

        public string Name => _innerRoom.Name;

        internal Room(Transport.Room room)
        {
            Contract.Requires(room != null);
            this._innerRoom = room;
        }

        public bool Equals(Room other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Uuid == other.Uuid;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Room);
        }

        public override int GetHashCode()
        {
            return Uuid.GetHashCode();
        }

        public override string ToString()
        {
            return Name ?? Uuid.ToString();
        }
    }
}
