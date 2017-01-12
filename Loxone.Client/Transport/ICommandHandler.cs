// ----------------------------------------------------------------------
// <copyright file="ICommandHandler.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface ICommandHandler
    {
        bool CanHandleMessage(MessageIdentifier identifier);

        Task HandleMessageAsync(MessageHeader header, LXWebSocket socket, CancellationToken cancellationToken);
    }
}
