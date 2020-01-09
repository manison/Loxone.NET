// ----------------------------------------------------------------------
// <copyright file="DeserializationTests.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Tests
{
    using System;
    using Loxone.Client.Transport;
    using Loxone.Client.Transport.Serialization.Responses;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void DeserializeResponseNumberCodeLowercase()
        {
            string s = @"{""LL"":{""control"":""jdev/sys/getkey2/user"",""code"":200,""value"":{""key"":""0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"",""salt"":""0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF012345""}}}";
            var r = LXResponse<GetKey2>.Deserialize(s);
            Assert.AreEqual(200, r.Code);
            Assert.AreEqual("jdev/sys/getkey2/user", r.Control);
            Assert.IsNotNull(r.Value);
        }

        [TestMethod]
        public void DeserializeResponseStringCodeUppercase()
        {
            string s = @"{""LL"": { ""control"": ""dev/cfg/api"", ""value"": ""{'snr': 'AA:BB:CC:DD:EE:FF', 'version':'10.3.4.10'}"", ""Code"": ""200""}}";
            var r = LXResponse<Api>.Deserialize(s);
            Assert.AreEqual(200, r.Code);
            Assert.AreEqual("dev/cfg/api", r.Control);
            Assert.IsNotNull(r.Value);
            Assert.AreEqual(SerialNumber.Parse("AA:BB:CC:DD:EE:FF"), r.Value.SerialNumber);
            Assert.AreEqual(new Version(10, 3, 4, 10), r.Value.Version);
        }
    }
}
