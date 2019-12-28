// ----------------------------------------------------------------------
// <copyright file="TaskCompletionCommandHandler.cs">
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

    internal abstract class TaskCompletionCommandHandler<T> : ICommandHandler
    {
        private TaskCompletionSource<T> _completionSource;

        public Task<T> Task => _completionSource.Task;

        public IRequestEncoder Encoder { get; set; }

        public IResponseDecoder Decoder { get; set; }

        protected TaskCompletionCommandHandler()
        {
            this._completionSource = new TaskCompletionSource<T>();
        }

        public abstract bool CanHandleMessage(MessageIdentifier identifier);

        public abstract Task HandleMessageAsync(MessageHeader header, LXWebSocket socket, CancellationToken cancellationToken);

        protected bool TrySetResult(T result)
        {
            return _completionSource.TrySetResult(result);
        }
    }
}
