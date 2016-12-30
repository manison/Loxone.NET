// ----------------------------------------------------------------------
// <copyright file="Message.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    internal struct Message<TValue>
    {
        private MessageHeader _header;

        public MessageHeader Header => _header;

        private readonly LXResponse<TValue> _response;

        public LXResponse<TValue> Response => _response;

        public Message(ref MessageHeader header, LXResponse<TValue> response)
        {
            this._header = header;
            this._response = response;
        }
    }
}
