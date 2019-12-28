// ----------------------------------------------------------------------
// <copyright file="PasswordAuthenticator.cs">
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
    using System.Net;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class PasswordAuthenticator : Authenticator
    {
        private HMACSHA1 _hmac;

        public PasswordAuthenticator(Session session, NetworkCredential credentials) : base(session, credentials)
        {
        }

        public override async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            await ObtainKeyAsync(cancellationToken).ConfigureAwait(false);
            await DoAuthenticateAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task ObtainKeyAsync(CancellationToken cancellationToken)
        {
            var response = await Client.RequestCommandAsync<string>("jdev/sys/getkey", CommandEncryption.None, cancellationToken).ConfigureAwait(false);
            var key = HexConverter.FromString(response.Value);
            _hmac = new HMACSHA1(key);
        }

        private async Task DoAuthenticateAsync(CancellationToken cancellationToken)
        {
            string credentials = String.Concat(Credentials.UserName, ":", Credentials.Password);
            var hash = _hmac.ComputeHash(LXWebSocket.Encoding.GetBytes(credentials));
            string request = "authenticate/" + HexConverter.FromByteArray(hash);
            var response = await Client.RequestCommandAsync<string>(request, CommandEncryption.None, cancellationToken).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hmac?.Dispose();
            }
        }
    }
}
