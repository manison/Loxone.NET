// ----------------------------------------------------------------------
// <copyright file="CryptographyUtils.cs">
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
    using System.Linq;
    using System.Security.Cryptography;
    using DerConverter;
    using DerConverter.Asn;
    using DerConverter.Asn.KnownTypes;

    internal static class CryptographyUtils
    {
        public static RSA GetRsaPublicKey(byte[] der)
        {
            var parameters = GetRsaPublicKeyParameters(der);
            var rsa = RSA.Create();

            try
            {
                rsa.ImportParameters(parameters);
            }
            catch
            {
                rsa.Dispose();
                throw;
            }

            return rsa;
        }

        // For now we use 3rd party package to create RSA public key from the Miniserver provided data.
        // Once .NET provides its own ways to do this we will migrate.
        // https://github.com/dotnet/corefx/issues/20414
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa.importrsapublickey
        // The following code is courtesy PemUtils NuGet package licensed under Apache License 2.0.
        // https://github.com/huysentruitw/pem-utils

        public static RSAParameters GetRsaPublicKeyParameters(byte[] der)
        {
            var asn = DerConvert.Decode(der);
            return ReadPublicKey(asn);
        }

        private static readonly int[] RsaIdentifier = new[] { 1, 2, 840, 113549, 1, 1, 1 };

        private static RSAParameters ReadPublicKey(DerAsnType der)
        {
            if (der == null) throw new ArgumentNullException(nameof(der));
            var outerSequence = der as DerAsnSequence;
            if (outerSequence == null) throw new ArgumentException($"{nameof(der)} is not a sequence");
            if (outerSequence.Value.Length != 2) throw new InvalidOperationException("Outer sequence must contain 2 parts");

            var headerSequence = outerSequence.Value[0] as DerAsnSequence;
            if (headerSequence == null) throw new InvalidOperationException("First part of outer sequence must be another sequence (the header sequence)");
            if (headerSequence.Value.Length != 2) throw new InvalidOperationException("The header sequence must contain 2 parts");
            var objectIdentifier = headerSequence.Value[0] as DerAsnObjectIdentifier;
            if (objectIdentifier == null) throw new InvalidOperationException("First part of header sequence must be an object-identifier");
            if (!Enumerable.SequenceEqual(objectIdentifier.Value, RsaIdentifier)) throw new InvalidOperationException($"RSA object-identifier expected 1.2.840.113549.1.1.1, got: {string.Join(".", objectIdentifier.Value.Select(x => x.ToString()))}");
            if (!(headerSequence.Value[1] is DerAsnNull)) throw new InvalidOperationException("Second part of header sequence must be a null");

            var innerSequenceBitString = outerSequence.Value[1] as DerAsnBitString;
            if (innerSequenceBitString == null) throw new InvalidOperationException("Second part of outer sequence must be a bit-string");

            var innerSequenceData = innerSequenceBitString.Encode(null).Skip(1).ToArray();
            var innerSequence = DerConvert.Decode(innerSequenceData) as DerAsnSequence;
            if (innerSequence == null) throw new InvalidOperationException("Could not decode the bit-string as a sequence");
            if (innerSequence.Value.Length < 2) throw new InvalidOperationException("Inner sequence must at least contain 2 parts (modulus and exponent)");

            return new RSAParameters
            {
                Modulus = GetIntegerData(innerSequence.Value[0]),
                Exponent = GetIntegerData(innerSequence.Value[1])
            };
        }

        private static byte[] GetIntegerData(DerAsnType der)
        {
            var data = (der as DerAsnInteger)?.Encode(null);
            if (data == null) throw new InvalidOperationException("Part does not contain integer data");
            if (data[0] == 0x00) data = data.Skip(1).ToArray();
            return data;
        }
    }
}
