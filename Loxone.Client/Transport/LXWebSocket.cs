// ----------------------------------------------------------------------
// <copyright file="LXWebSocket.cs">
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
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Net.WebSockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class LXWebSocket : IDisposable
    {
        private Uri _baseUri;

        private ClientWebSocket _webSocket;

        internal static readonly Encoding Encoding = Encoding.UTF8;

        private HMACSHA1 _hmac;

        private NetworkCredential _credentials;

        private CancellationTokenSource _receiveLoopCancellation;

        private ICommandHandler _pendingCommand;

        public LXWebSocket(Uri baseUri, NetworkCredential credentials)
        {
            Contract.Requires(baseUri != null);
            Contract.Requires(HttpUtils.IsWebSocketUri(baseUri));
            Contract.Requires(string.IsNullOrEmpty(baseUri.PathAndQuery));
            Contract.Requires(credentials != null);

            this._baseUri = baseUri;
            this._credentials = credentials;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            Contract.Requires(_webSocket == null);

            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new UriBuilder(_baseUri) { Path = "ws/rfc6455" }.Uri, cancellationToken).ConfigureAwait(false);

            StartReceiveLoop();

            await ObtainKeyAsync(cancellationToken).ConfigureAwait(false);
            await AuthenticateAsync(cancellationToken).ConfigureAwait(false);
        }

        public void StartReceiveLoop()
        {
            Contract.Requires(_receiveLoopCancellation == null, "Receive loop is already running");

            _receiveLoopCancellation = new CancellationTokenSource();

            // Fire and forget, no await here.
            ReceiveLoopAsync();
        }

        private async void ReceiveLoopAsync()
        {
            Contract.Requires(_receiveLoopCancellation != null, "Receive loop is not running");

            bool quit = false;

            while (!quit)
            {
                try
                {
                    var header = await ReceiveHeaderAsync(_receiveLoopCancellation.Token).ConfigureAwait(false);
                    Contract.Assert(!header.IsLengthEstimated);
                    await DispatchMessageAsync(header, _receiveLoopCancellation.Token).ConfigureAwait(false);
                }
                catch
                {
                    //quit = true;
                }
            }
        }

        private async Task DispatchMessageAsync(MessageHeader header, CancellationToken cancellationToken)
        {
            bool processed = false;

            var command = _pendingCommand;
            if (command != null && command.CanHandleMessage(header.Identifier))
            {
                Interlocked.CompareExchange(ref _pendingCommand, null, command);
                await command.HandleMessageAsync(header, this, cancellationToken).ConfigureAwait(false);
                processed = true;
                (command as IDisposable)?.Dispose();
            }

            if (!processed)
            {
                processed = await HandleUnsolicitedMessageAsync(header, cancellationToken).ConfigureAwait(false);
                if (!processed)
                {
                    await SkipBytesAsync(header.Length, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> HandleUnsolicitedMessageAsync(MessageHeader header, CancellationToken cancellationToken)
        {
            switch (header.Identifier)
            {
                case MessageIdentifier.ValueStates:
                    await HandleValueStatesAsync(header.Length, cancellationToken).ConfigureAwait(false);
                    return true;

                case MessageIdentifier.TextStates:
                    await HandleTextStatesAsync(header.Length, cancellationToken).ConfigureAwait(false);
                    return true;

                case MessageIdentifier.DayTimerStates:
                    break;

                case MessageIdentifier.OutOfService:
                    break;

                case MessageIdentifier.KeepAlive:
                    break;

                case MessageIdentifier.WeatherStates:
                    break;
            }

            return false;
        }

        private async Task HandleValueStatesAsync(int length, CancellationToken cancellationToken)
        {
            if (length >= 24)
            {
                var states = new List<ValueState>(length / 24);
                var buffer = new ArraySegment<byte>(new byte[24]);

                while (length >= 24)
                {
                    await ReceiveAtomicAsync(buffer, false, cancellationToken).ConfigureAwait(false);
                    var uuid = new Uuid(new ArraySegment<byte>(buffer.Array, 0, 16));
                    double value = BitConverter.ToDouble(buffer.Array, 16);
                    ValueState state = new ValueState(uuid, value);
                    states.Add(state);
                    length -= 24;
                    Console.WriteLine("ValueState: " + state.ToString());
                }
            }

            if (length > 0)
            {
                // Extra data beyond the last value state?
                await SkipBytesAsync(length, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task HandleTextStatesAsync(int length, CancellationToken cancellationToken)
        {
            int offset = 0;

            var buffer = new ArraySegment<byte>(new byte[length]);
            await ReceiveAtomicAsync(buffer, false, cancellationToken).ConfigureAwait(false);
            var states = new List<TextState>();

            while (length > 0)
            {

                var uuid = new Uuid(new ArraySegment<byte>(buffer.Array, offset, offset + 16));
                offset += 16;

                var uuidIcon = new Uuid(new ArraySegment<byte>(buffer.Array, offset, offset + 16));
                offset += 16;

                int textLength = (int)BitConverter.ToUInt32(buffer.Array, offset);
                offset += 4;

                string text = Encoding.UTF8.GetString(buffer.Array, offset, textLength);


                int newOffset = 36 + (textLength % 4 > 0 ? textLength + 4 - (textLength % 4) : textLength);
                offset += newOffset;

                length -= offset;

                TextState state = new TextState(uuid, uuidIcon, textLength, text);
                states.Add(state);
                Console.WriteLine("TextState: " + state.ToString());
            }
        }


        private async Task SkipBytesAsync(int numberOfBytesToReadOut, CancellationToken cancellationToken)
        {
            int bufferSize = Math.Min(numberOfBytesToReadOut, 4096);
            var buffer = new ArraySegment<byte>(new byte[bufferSize]);

            while (numberOfBytesToReadOut > 0)
            {
                var result = await _webSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                numberOfBytesToReadOut -= result.Count;

                if (result.CloseStatus != null)
                {
                    break;
                }
            }
        }

        private Task SendStringAsync(string s, CancellationToken cancellationToken)
        {
            var encoded = Encoding.GetBytes(s);
            var segment = new ArraySegment<byte>(encoded);
            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }

        internal async Task ReceiveAtomicAsync(ArraySegment<byte> segment, bool throwIfNotEom, CancellationToken cancellationToken)
        {
            int received = 0;
            bool eom = false;

            while (received < segment.Count)
            {
                var slice = new ArraySegment<byte>(segment.Array, segment.Offset + received, segment.Count - received);
                var result = await _webSocket.ReceiveAsync(slice, cancellationToken).ConfigureAwait(false);

                received += result.Count;

                if (result.CloseStatus != null)
                {
                    break;
                }

                eom = result.EndOfMessage;
            }

            if (received < segment.Count)
            {
                throw new MiniserverTransportException();
            }
            else if (received == segment.Count && !eom && throwIfNotEom)
            {
                // More data available but unexpected.
                throw new MiniserverTransportException();
            }
        }

        private async Task<MessageHeader> ReceiveHeaderAsync(CancellationToken cancellationToken)
        {
            var headerSegment = new ArraySegment<byte>(new byte[8]);
            await ReceiveAtomicAsync(headerSegment, false, cancellationToken).ConfigureAwait(false);
            var header = new MessageHeader(headerSegment);
            if (header.IsLengthEstimated)
            {
                await ReceiveAtomicAsync(headerSegment, false, cancellationToken).ConfigureAwait(false);
                header = new MessageHeader(headerSegment);
            }

            return header;
        }

        public async Task<string> RequestStringAsync(string command, CancellationToken cancellationToken)
        {
            var handler = new StringCommandHandler();
            await EnqueueCommandAsync(command, handler, cancellationToken).ConfigureAwait(false);
            var s = await handler.Task.ConfigureAwait(false);
            return s;
        }

        [Flags]
        public enum RequestCommandValidation
        {
            None = 0,
            Status = 1,
            Command = 2,
            All = Status | Command
        }

        private static bool AreCommandsEqual(string requestCommand, string responseCommand)
        {
            bool equals = String.Equals(requestCommand, responseCommand, StringComparison.OrdinalIgnoreCase);

            // Response to 'jdev' may actually be 'dev' instead.
            if (!equals &&
                requestCommand.StartsWith("jdev/", StringComparison.OrdinalIgnoreCase) &&
                responseCommand.StartsWith("dev/", StringComparison.OrdinalIgnoreCase))
            {
                equals = String.Equals(requestCommand.Substring(5), responseCommand.Substring(4), StringComparison.OrdinalIgnoreCase);
            }

            return equals;
        }

        public async Task<LXResponse<T>> RequestCommandAsync<T>(string command, RequestCommandValidation validation, CancellationToken cancellationToken)
        {
            var handler = new LXResponseCommandHandler<T>();
            await EnqueueCommandAsync(command, handler, cancellationToken).ConfigureAwait(false);
            var response = await handler.Task.ConfigureAwait(false);

            if ((validation & RequestCommandValidation.Command) != 0 && !AreCommandsEqual(command, response.Control))
            {
                throw new MiniserverTransportException();
            }

            if ((validation & RequestCommandValidation.Status) != 0 && !LXStatusCode.IsSuccess(response.Code))
            {
                throw new MiniserverCommandException(response.Code);
            }

            return response;
        }

        public Task<LXResponse<T>> RequestCommandAsync<T>(string command, CancellationToken cancellationToken)
        {
            return RequestCommandAsync<T>(command, RequestCommandValidation.All, cancellationToken);
        }

        private async Task ObtainKeyAsync(CancellationToken cancellationToken)
        {
            var response = await RequestCommandAsync<string>("jdev/sys/getkey", cancellationToken).ConfigureAwait(false);
            var key = HexConverter.FromString(response.Value);
            _hmac = new HMACSHA1(key);
        }

        private async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            string credentials = string.Concat(_credentials.UserName, ":", _credentials.Password);
            var hash = _hmac.ComputeHash(Encoding.GetBytes(credentials));
            string request = "authenticate/" + HexConverter.FromByteArray(hash);
            var response = await RequestCommandAsync<string>(request, cancellationToken).ConfigureAwait(false);
        }

        private async Task EnqueueCommandAsync(string command, ICommandHandler handler, CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _pendingCommand, handler, null) != null)
            {
                // Command already pending.
                throw new InvalidOperationException();
            }

            await SendStringAsync(command, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_receiveLoopCancellation != null)
            {
                _receiveLoopCancellation.Cancel();
                _receiveLoopCancellation.Dispose();
            }

            _webSocket?.Dispose();
            _hmac?.Dispose();
        }
    }
}
