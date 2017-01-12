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

        private static readonly Encoding _encoding = Encoding.UTF8;

        private HMACSHA1 _hmac;

        private NetworkCredential _credentials;

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

            await ObtainKeyAsync(cancellationToken).ConfigureAwait(false);
            await AuthenticateAsync(cancellationToken).ConfigureAwait(false);
        }

        private Task SendStringAsync(string s, CancellationToken cancellationToken)
        {
            var encoded = _encoding.GetBytes(s);
            var segment = new ArraySegment<byte>(encoded);
            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task ReceiveAtomicAsync(ArraySegment<byte> segment, bool throwIfNotEom, CancellationToken cancellationToken)
        {
            int received = 0;
            bool eom = false;

            while (received < segment.Count)
            {
                var slice = new ArraySegment<byte>(segment.Array, segment.Offset + received, segment.Count - received);
                var result = await _webSocket.ReceiveAsync(slice, cancellationToken);

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

        delegate TResult ReceiveWorkerHandler<TResult>(ref MessageHeader header, byte[] content);

        private static string StringHandler(ref MessageHeader header, byte[] content)
        {
            return _encoding.GetString(content);
        }

        private async Task<TResult> ReceiveWorkerAsync<TResult>(ReceiveWorkerHandler<TResult> handler, CancellationToken cancellationToken)
        {
            var header = await ReceiveHeaderAsync(cancellationToken).ConfigureAwait(false);
            var content = new byte[header.Length];
            await ReceiveAtomicAsync(new ArraySegment<byte>(content), true, cancellationToken).ConfigureAwait(false);
            return handler(ref header, content);
        }

        private Task<string> ReceiveStringAsync(CancellationToken cancellationToken)
        {
            return ReceiveWorkerAsync<string>(StringHandler, cancellationToken);
        }

        private Task<Message<TValue>> ReceiveMessageAsync<TValue>(CancellationToken cancellationToken)
        {
            return ReceiveWorkerAsync<Message<TValue>>(
                (ref MessageHeader header, byte[] content) =>
                {
                    string s = StringHandler(ref header, content);
                    var response = LXResponse<TValue>.Deserialize(s);
                    return new Message<TValue>(ref header, response);
                }, cancellationToken);
        }

        public async Task<string> RequestStringAsync(string cmd, CancellationToken cancellationToken)
        {
            await SendStringAsync(cmd, cancellationToken).ConfigureAwait(false);
            return await ReceiveStringAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Message<TValue>> RequestCommandAsync<TValue>(string cmd, bool checkStatus, CancellationToken cancellationToken)
        {
            await SendStringAsync(cmd, cancellationToken).ConfigureAwait(false);
            var message = await ReceiveMessageAsync<TValue>(cancellationToken).ConfigureAwait(false);
            if (checkStatus && !LXStatusCode.IsSuccess(message.Response.Code))
            {
                throw new MiniserverCommandException(message.Response.Code);
            }

            return message;
        }

        public Task<Message<TValue>> RequestCommandAsync<TValue>(string cmd, CancellationToken cancellationToken)
        {
            return RequestCommandAsync<TValue>(cmd, true, cancellationToken);
        }

        private async Task ObtainKeyAsync(CancellationToken cancellationToken)
        {
            var message = await RequestCommandAsync<string>("jdev/sys/getkey", cancellationToken).ConfigureAwait(false);
            var key = HexConverter.FromString(message.Response.Value);
            _hmac = new HMACSHA1(key);
        }

        private async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            string credentials = string.Concat(_credentials.UserName, ":", _credentials.Password);
            var hash = _hmac.ComputeHash(_encoding.GetBytes(credentials));
            string request = "authenticate/" + HexConverter.FromByteArray(hash);
            var message = await RequestCommandAsync<string>(request, false, cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _webSocket?.Dispose();
            _hmac?.Dispose();
        }
    }
}
