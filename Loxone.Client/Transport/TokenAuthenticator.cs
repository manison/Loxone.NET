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
    using System.Net;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Transport.Serialization.Responses;

    internal sealed class TokenAuthenticator : Authenticator
    {
        public TokenAuthenticator(Session session, NetworkCredential credentials) : base(session, credentials)
        {
        }

        public override async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            var sessionKey = await Session.GetSessionKeyAsync(cancellationToken).ConfigureAwait(false);
            var keyExchangeResponse = await Client.RequestCommandAsync<string>($"jdev/sys/keyexchange/{sessionKey}", CommandEncryption.None, cancellationToken).ConfigureAwait(false);
            var userKeyResponse = await Client.RequestCommandAsync<GetKey2>($"jdev/sys/getkey2/{Credentials.UserName}", CommandEncryption.None, cancellationToken).ConfigureAwait(false);

            string pwHash;
            using (var hashAlgorithm = HashAlgorithm.Create(userKeyResponse.Value.HashAlgorithm))
            {
                pwHash = HexConverter.FromByteArray(hashAlgorithm.ComputeHash(LXClient.Encoding.GetBytes($"{Credentials.Password}:{userKeyResponse.Value.Salt}")));
            }

            string hash;
            using (var hmac = HMAC.Create($"HMAC{userKeyResponse.Value.HashAlgorithm}"))
            {
                hmac.Key = HexConverter.FromString(userKeyResponse.Value.Key);
                hash = HexConverter.FromByteArray(hmac.ComputeHash(LXClient.Encoding.GetBytes($"{Credentials.UserName}:{pwHash}")));
            }

            string command = BuildAcquireTokenCommand(hash);
            var response = await Client.RequestCommandAsync<GetToken>(command, CommandEncryption.RequestAndResponse, cancellationToken).ConfigureAwait(false);
        }

        private string BuildAcquireTokenCommand(string hash)
        {
            int permission;
            Uuid uuid;
            string info;
            var tokenCredentials = Credentials as TokenCredential;
            if (tokenCredentials != null)
            {
                permission = (int)tokenCredentials.Permission;
                uuid = tokenCredentials.ClientID;
            }
            else
            {
                permission = (int)TokenPermission.Web;
            }

            if (uuid == default)
            {
                uuid = MachineNameUuid.Value;
            }

            info = tokenCredentials?.ClientName ?? "Loxone.NET";
            info = Uri.EscapeUriString(info);
            return $"jdev/sys/gettoken/{hash}/{Credentials.UserName}/{permission}/{uuid}/{info}";
        }
    }
}
