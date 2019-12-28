// ----------------------------------------------------------------------
// <copyright file="Session.cs">
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
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class Session : IDisposable
    {
        private object _lock = new object();

        private readonly LXClient _client;

        public LXClient Client => _client;

        private Task<RSA> _publicKeyTask;
        private bool _publicKeyTaskInitialized;

        private Aes _aes;
        private bool _aesInitialized;

        private Task<string> _sessionKeyTask;
        private bool _sessionKeyTaskInitialized;

        private const int SaltByteLength = 35;
        private string _salt;

        public string Salt => LazyInitializer.EnsureInitialized(ref _salt, GenerateSalt);

        public Session(LXClient client)
        {
            this._client = client;
        }

        public Task<RSA> GetMiniserverPublicKeyAsync(CancellationToken cancellationToken) =>
            LazyInitializer.EnsureInitialized(ref _publicKeyTask, ref _publicKeyTaskInitialized, ref _lock, () => GetMiniserverPublicKeyInternalAsync(cancellationToken));

        private async Task<RSA> GetMiniserverPublicKeyInternalAsync(CancellationToken cancellationToken)
        {
            var response = await _client.HttpClient.RequestCommandAsync<string>("jdev/sys/getPublicKey", CommandEncryption.None, cancellationToken).ConfigureAwait(false);
            var pem = response.Value;
            pem = pem.Replace("-----BEGIN CERTIFICATE-----", String.Empty);
            pem = pem.Replace("-----END CERTIFICATE-----", String.Empty);
            pem = pem.Trim();
            byte[] der = Convert.FromBase64String(pem);
            return CryptographyUtils.GetRsaPublicKey(der);
        }

        private void EnsureAes() => LazyInitializer.EnsureInitialized(ref _aes, ref _aesInitialized, ref _lock, () =>
        {
            var aes = Aes.Create();
            try
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.Padding = PaddingMode.Zeros;
                aes.GenerateKey();
                aes.GenerateIV();
            }
            catch
            {
                aes.Dispose();
                throw;
            }
            return aes;
        });

        public Task<string> GetSessionKeyAsync(CancellationToken cancellationToken) =>
            LazyInitializer.EnsureInitialized(ref _sessionKeyTask, ref _sessionKeyTaskInitialized, ref _lock, () => GetSessionKeyInternalAsync(cancellationToken));

        private async Task<string> GetSessionKeyInternalAsync(CancellationToken cancellationToken)
        {
            EnsureAes();
            string cipher = FormatAesCipher();
            var rsa = await GetMiniserverPublicKeyAsync(cancellationToken);
            var encrypted = rsa.Encrypt(LXClient.Encoding.GetBytes(cipher), RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encrypted);
        }

        private string FormatAesCipher()
        {
            byte[] key = _aes.Key;
            byte[] iv = _aes.IV;
            char[] chars = new char[(key.Length + iv.Length) * 2 + 1];
            HexConverter.FromByteArray(chars, 0, key);
            chars[key.Length * 2] = ':';
            HexConverter.FromByteArray(chars, key.Length * 2 + 1, iv);
            return new string(chars);
        }

        public ICryptoTransform CreateEncryptor()
        {
            EnsureAes();
            return _aes.CreateEncryptor();
        }

        public ICryptoTransform CreateDecryptor()
        {
            EnsureAes();
            return _aes.CreateDecryptor();
        }

        private string GenerateSalt()
        {
            byte[] salt;

            using (var random = RandomNumberGenerator.Create())
            {
                salt = new byte[SaltByteLength];
                random.GetBytes(salt);
            }

            return HexConverter.FromByteArray(salt);
        }

        public void Dispose()
        {
            if (_publicKeyTaskInitialized && _publicKeyTask.IsCompleted)
            {
                _publicKeyTask.Result.Dispose();
            }

            _aes?.Dispose();
        }
    }
}
