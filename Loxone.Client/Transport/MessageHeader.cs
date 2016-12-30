// ----------------------------------------------------------------------
// <copyright file="MessageHeader.cs">
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
    using System.Collections.Generic;

    internal struct MessageHeader
    {
        private readonly MessageIdentifier _identifier;

        public MessageIdentifier Identifier => _identifier;

        private readonly int _length;

        public int Length => _length;

        private readonly MessageInfoFlags _flags;

        public bool IsLengthEstimated => (_flags & MessageInfoFlags.EstimatedLength) != 0;

        public MessageHeader(ArraySegment<byte> header)
        {
            var h = (IList<byte>)header;
            if (h[0] != 3)
            {
                throw new MiniserverTransportException();
            }

            if (!Enum.IsDefined(typeof(MessageIdentifier), (int)h[1]))
            {
                throw new MiniserverTransportException();
            }

            _identifier = (MessageIdentifier)h[1];
            _flags = (MessageInfoFlags)h[2];
            _length = BitConverter.ToInt32(header.Array, header.Offset + 4);
        }
    }
}
