// ----------------------------------------------------------------------
// <copyright file="RoomCollection.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public sealed class RoomCollection : IReadOnlyCollection<Room>
    {
        private readonly IDictionary<string, Transport.Room> _innerRooms;
        private readonly Dictionary<Uuid, Room> _rooms;

        internal RoomCollection(IDictionary<string, Transport.Room> innerRooms)
        {
            Contract.Requires(innerRooms != null);
            this._innerRooms = innerRooms;
            _rooms = new Dictionary<Uuid, Room>(_innerRooms.Count);
        }

        public int Count => _innerRooms.Count;

        public IEnumerator<Room> GetEnumerator()
        {
            foreach (var pair in _innerRooms)
            {
                yield return this[Uuid.Parse(pair.Key)];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal Room this[Uuid uuid]
        {
            get
            {
                if (!_rooms.TryGetValue(uuid, out var room))
                {
                    var innerRoom = _innerRooms[uuid.ToString()];
                    room = new Room(innerRoom);
                    _rooms[uuid] = room;
                }
                return room;
            }
        }
    }
}
