// ----------------------------------------------------------------------
// <copyright file="ContextControlsTests.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Tests
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContextControlsTests
    {
        private StructureFile _structureFile;

        [TestInitialize]
        public async Task Setup()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Loxone.Client.Tests.LoxAPP3.json"))
            {
                _structureFile = await StructureFile.LoadAsync(stream, CancellationToken.None);
            }
        }

        [TestMethod]
        public void CreateControls()
        {
            using (var context = new MiniserverContext(_structureFile))
            {

            }
        }
    }
}
