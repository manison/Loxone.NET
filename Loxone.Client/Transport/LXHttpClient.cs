// ----------------------------------------------------------------------
// <copyright file="TokenAuthenticator.cs">
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
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class LXHttpClient : LXClient
    {
        private HttpClient _httpClient;

        protected internal override LXClient HttpClient => this;

        public LXHttpClient(Uri baseUri) : base(baseUri)
        {
            Contract.Requires(HttpUtils.IsHttpUri(baseUri));
        }

        private void EnsureHttpClient()
        {
            if (_httpClient == null)
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = BaseUri;
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HttpUtils.JsonMediaType));
                if (Interlocked.CompareExchange(ref _httpClient, httpClient, null) != null)
                {
                    httpClient.Dispose();
                }
            }
        }

        protected override Task OpenInternalAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected override async Task<LXResponse<T>> RequestCommandInternalAsync<T>(string command, CommandEncryption encryption, CancellationToken cancellationToken)
        {
            EnsureHttpClient();

            using (var response = await _httpClient.GetAsync(command, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode && HttpUtils.IsJsonMediaType(response.Content.Headers.ContentType))
                {
                    string contentStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var lxResponse = Transport.LXResponse<T>.Deserialize(contentStr);
                    return lxResponse;
                }
                else
                {
                    throw new MiniserverTransportException();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }
    }
}
