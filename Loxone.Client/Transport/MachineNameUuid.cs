// ----------------------------------------------------------------------
// <copyright file="MachineNameUuid.cs">
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

    public static class MachineNameUuid
    {
        // {BFC65CBD-CD9F-4E5C-8930-2CCEE45CB40C}
        private static readonly byte[] _namespaceUuid = new byte[] { 0xbd, 0x5c, 0xc6, 0xbf, 0x9f, 0xcd, 0x5c, 0x4e, 0x89, 0x30, 0x2c, 0xce, 0xe4, 0x5c, 0xb4, 0x0c };

        public static readonly Uuid Value;

        static MachineNameUuid()
        {
            string machineName = Environment.MachineName;
            int machineNameByteCount = LXClient.Encoding.GetByteCount(machineName);
            byte[] bytes = new byte[machineNameByteCount + _namespaceUuid.Length];
            Array.Copy(_namespaceUuid, bytes, _namespaceUuid.Length);
            LXClient.Encoding.GetBytes(machineName, 0, machineName.Length, bytes, _namespaceUuid.Length);
            byte[] hash;
            using (var sha1 = SHA1.Create())
            {
                hash = sha1.ComputeHash(bytes);
            }
            // version 5 UUID
            hash[7] &= 0x0F;
            hash[7] |= 0x50;
            // variant 1 UUID
            hash[8] &= 0x3F;
            hash[8] |= 0x80;
            Value = new Uuid(new ArraySegment<byte>(hash, 0, 16));
        }
    }
}
