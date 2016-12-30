// ----------------------------------------------------------------------
// <copyright file="MiniserverConnection.cs">
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
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Encapsulates connection to the Loxone Miniserver.
    /// </summary>
    public class MiniserverConnection : IDisposable
    {
        private enum State
        {
            Constructed,
            Opening,
            Open,
            Disposing,
            Disposed
        }

        private volatile int _state;

        private MiniserverLimitedInfo _miniserverInfo;

        public MiniserverLimitedInfo MiniserverInfo
        {
            get
            {
                return _miniserverInfo;
            }
        }

        private Uri _baseUri;

        public Uri Address
        {
            get
            {
                return _baseUri;
            }
            set
            {
                Contract.Requires(value == null || HttpUtils.IsHttpUri(value));
                Contract.Requires(value == null || String.IsNullOrEmpty(value.PathAndQuery));

                CheckDisposed();
                CheckState(State.Constructed);

                if (value != null)
                {
                    InitWithUri(value);
                }
                else
                {
                    _baseUri = null;
                }
            }
        }

        private Transport.LXWebSocket _webSocket;

        private ICredentials _credentials;

        public ICredentials Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                CheckDisposed();
                CheckState(State.Constructed);

                _credentials = value;
            }
        }

        public MiniserverConnection()
        {
            _miniserverInfo = new MiniserverLimitedInfo();
            _state = (int)State.Constructed;
        }

        public MiniserverConnection(Uri address)
            : this()
        {
            Contract.Requires(address != null);
            Contract.Requires(HttpUtils.IsHttpUri(address));
            Contract.Requires(String.IsNullOrEmpty(address.PathAndQuery));

            InitWithUri(address);
        }

        public MiniserverConnection(IPEndPoint endpoint)
            : this()
        {
            Contract.Requires(endpoint != null);

            InitWithEndpoint(endpoint);
        }

        public MiniserverConnection(IPAddress address, int port)
            : this()
        {
            Contract.Requires(address != null);
            Contract.Requires(port >= IPEndPoint.MinPort && port <= IPEndPoint.MaxPort);

            InitWithEndpoint(new IPEndPoint(address, port));
        }

        public MiniserverConnection(IPAddress address)
            : this(address, HttpUtils.DefaultHttpPort)
        {
        }

        private void InitWithEndpoint(IPEndPoint endpoint)
        {
            var builder = new UriBuilder(HttpUtils.HttpScheme, endpoint.Address.ToString());
            if (endpoint.Port != HttpUtils.DefaultHttpPort)
            {
                builder.Port = endpoint.Port;
            }

            InitWithUri(builder.Uri);
        }

        private void InitWithUri(Uri address)
        {
            Contract.Requires(address != null);
            Contract.Requires(HttpUtils.IsHttpUri(address));
            Contract.Requires(String.IsNullOrEmpty(address.PathAndQuery));

            _baseUri = address;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            CheckBeforeOpen();

            ChangeState(State.Opening, State.Constructed);

            try
            {
                await CheckMiniserverReachableAsync(cancellationToken).ConfigureAwait(false);
                await OpenWebSocketAsync(cancellationToken).ConfigureAwait(false);
                ChangeState(State.Open);
            }
            catch
            {
                ChangeState(State.Constructed);
                throw;
            }
        }

        private void CheckBeforeOperation()
        {
            CheckDisposed();
            CheckState(State.Open);
        }

        public async Task<StructureFile> DownloadStructureFileAsync(CancellationToken cancellationToken)
        {
            CheckBeforeOperation();
            string s = await _webSocket.RequestStringAsync("data/LoxAPP3.json", cancellationToken).ConfigureAwait(false);
            return StructureFile.Parse(s);
        }

        public async Task<DateTime> GetStructureFileLastModifiedDateAsync(CancellationToken cancellationToken)
        {
            CheckBeforeOperation();
            var message = await _webSocket.RequestCommandAsync<DateTime>("jdev/sps/LoxAPPversion3", cancellationToken).ConfigureAwait(false);
            return DateTime.SpecifyKind(message.Response.Value, DateTimeKind.Local);
        }

        public async Task EnableStatusUpdatesAsync(CancellationToken cancellationToken)
        {
            CheckBeforeOperation();
            var message = await _webSocket.RequestCommandAsync<string>("jdev/sps/enablebinstatusupdate", cancellationToken).ConfigureAwait(false);
        }

        private void CheckBeforeOpen()
        {
            if (_baseUri == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.MiniserverConnection_MustBeSetBeforeOpenFmt, nameof(Address)));
            }

            if (_credentials == null)
            {
                // Here we only check whether ICredentials is null. If ICredentials.GetCredential
                // returns null then the exception is thrown from OpenWebSocketAsync.
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.MiniserverConnection_MustBeSetBeforeOpenFmt, nameof(Credentials)));
            }
        }

        private async Task CheckMiniserverReachableAsync(CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = _baseUri;
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpUtils.JsonMediaType));
                using (var response = await httpClient.GetAsync("jdev/cfg/api", HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode && HttpUtils.IsJsonMediaType(response.Content.Headers.ContentType))
                    {
                        string contentStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var lxResponse = Transport.LXResponse<Transport.Api>.Deserialize(contentStr);
                        if (Transport.LXStatusCode.IsSuccess(lxResponse.Code))
                        {
                            _miniserverInfo.Update(lxResponse.Value);
                        }
                    }
                }
            }
        }

        private Task OpenWebSocketAsync(CancellationToken cancellationToken)
        {
            var uri = new UriBuilder(_baseUri);
            uri.Scheme = HttpUtils.WebSocketScheme;

            var credentials = _credentials.GetCredential(uri.Uri, HttpUtils.BasicAuthenticationScheme);
            if (credentials == null)
            {
                throw new InvalidOperationException();
            }

            _webSocket = new Transport.LXWebSocket(uri.Uri, credentials);
            return _webSocket.OpenAsync(cancellationToken);
        }

        private void CheckState(State requiredState)
        {
            if (_state != (int)requiredState)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Atomically checks the state of the connection and changes it to a new state.
        /// </summary>
        /// <param name="newState">New state.</param>
        /// <param name="requiredState">Required current state of the connection.</param>
        /// <exception cref="InvalidOperationException">
        /// The current connection state does not match <paramref name="requiredState"/>.
        /// </exception>
        private void ChangeState(State newState, State requiredState)
        {
            if (Interlocked.CompareExchange(ref _state, (int)newState, (int)requiredState) != (int)requiredState)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Atomically changes state of the connection.
        /// </summary>
        /// <param name="newState">New state.</param>
        /// <returns>Previous state.</returns>
        private State ChangeState(State newState)
        {
            return (State)Interlocked.Exchange(ref _state, (int)newState);
        }

        #region IDisposable Implementation

        protected void CheckDisposed()
        {
            if (_state >= (int)State.Disposing)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_state != (int)State.Disposed)
            {
                // May be already disposing on another thread.
                if (Interlocked.Exchange(ref _state, (int)State.Disposing) != (int)State.Disposing)
                {
                    if (disposing)
                    {
                        _webSocket.Dispose();
                    }

                    _state = (int)State.Disposed;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
