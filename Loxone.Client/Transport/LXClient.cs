// ----------------------------------------------------------------------
// <copyright file="LXClient.cs">
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
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Transport.Serialization.Responses;

    internal abstract class LXClient : IDisposable
    {
        protected readonly Uri BaseUri;

        internal static readonly Encoding Encoding = Encoding.UTF8;

        protected internal abstract LXClient HttpClient { get; }

        protected LXClient(Uri baseUri)
        {
            Contract.Requires(baseUri != null);
            Contract.Requires(string.IsNullOrEmpty(baseUri.PathAndQuery));
            this.BaseUri = baseUri;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            await OpenInternalAsync(cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task OpenInternalAsync(CancellationToken cancellationToken);

        [Flags]
        public enum RequestCommandValidation
        {
            None = 0,
            Status = 1,
            Command = 2,
            All = Status | Command
        }

        protected abstract Task<LXResponse<T>> RequestCommandInternalAsync<T>(string command, CommandEncryption encryption, CancellationToken cancellationToken);

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

        public async Task<LXResponse<T>> RequestCommandAsync<T>(string command, CommandEncryption encryption, RequestCommandValidation validation, CancellationToken cancellationToken)
        {
            var response = await RequestCommandInternalAsync<T>(command, encryption, cancellationToken).ConfigureAwait(false);

            string control = Uri.EscapeUriString(response.Control);
            if ((validation & RequestCommandValidation.Command) != 0 && !AreCommandsEqual(command, control))
            {
                throw new MiniserverTransportException();
            }

            if ((validation & RequestCommandValidation.Status) != 0 && !LXStatusCode.IsSuccess(response.Code))
            {
                throw new MiniserverCommandException(response.Code);
            }

            return response;
        }

        public Task<LXResponse<T>> RequestCommandAsync<T>(string command, CommandEncryption encryption, CancellationToken cancellationToken)
            => RequestCommandAsync<T>(command, encryption, RequestCommandValidation.All, cancellationToken);

        public async Task<Api> CheckMiniserverReachableAsync(CancellationToken cancellationToken)
        {
            var response = await HttpClient.RequestCommandAsync<Api>("jdev/cfg/api", CommandEncryption.None, RequestCommandValidation.All, cancellationToken).ConfigureAwait(false);
            return response.Value;
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
