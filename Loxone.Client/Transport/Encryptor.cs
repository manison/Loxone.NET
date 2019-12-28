// ----------------------------------------------------------------------
// <copyright file="Encryptor.cs">
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
    using System.IO;
    using System.Security.Cryptography;

    internal sealed class Encryptor : IRequestEncoder, IResponseDecoder
    {
        private readonly Session _session;
        private readonly CommandEncryption _mode;

        public Encryptor(Session session, CommandEncryption mode)
        {
            Contract.Requires(session != null);
            this._session = session;
            this._mode = mode;
        }

        public string EncodeCommand(string command)
        {
            if (_mode == CommandEncryption.None)
            {
                return command;
            }

            string encryptedCommand = Convert.ToBase64String(AesEncrypt($"salt/{_session.Salt}/{command}"));
            encryptedCommand = Uri.EscapeDataString(encryptedCommand);
            string endpoint = (_mode == CommandEncryption.RequestAndResponse) ? "fenc" : "enc";
            return $"jdev/sys/{endpoint}/{encryptedCommand}";
        }

        public string DecodeCommand(string command)
        {
            if (_mode == CommandEncryption.None)
            {
                return command;
            }

            byte[] encryptedResponse = Convert.FromBase64String(command);
            string decrypted = AesDecrypt(encryptedResponse);
            return decrypted;
        }

        private byte[] AesEncrypt(string command)
        {
            byte[] input = LXClient.Encoding.GetBytes(command);
            using (var encryptor = _session.CreateEncryptor())
            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(input, 0, input.Length);
                cryptoStream.FlushFinalBlock();
                return memoryStream.ToArray();
            }
        }

        private string AesDecrypt(byte[] input)
        {
            using (var decryptor = _session.CreateDecryptor())
            using (var memoryStream = new MemoryStream(input, false))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream, LXClient.Encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
