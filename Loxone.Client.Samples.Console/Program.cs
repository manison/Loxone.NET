// ----------------------------------------------------------------------
// <copyright file="Program.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Samples.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        //Login doesn't work anymore!
        //Due to Loxone this is locked!
        private static string _miniserverAddress = "http://testminiserver.loxone.com:7778/";

        static async Task MainAsync(string[] args)
        {
            List<string> searchAdresses = await LoxoneMiniserverFinder.SearchForDevices();
            if (searchAdresses != null && searchAdresses.Count > 0)
            {
                _miniserverAddress = searchAdresses[0];
                Console.WriteLine("Discovered Miniserver at: " + _miniserverAddress);
            }

            var connection = new MiniserverConnection(new Uri(_miniserverAddress));

            // Specify Miniserver username and password.
            connection.Credentials = new NetworkCredential("admin", "admin");

            Console.WriteLine($"Opening connection to miniserver at {connection.Address}...");
            await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);

            // Load cached structure file or download a fresh one if the local file does not exist or is outdated.
            StructureFile structureFile = null;
            structureFile = await StructureFile.LoadAsync("LoxAPP3.json", CancellationToken.None).ConfigureAwait(false);
            var lastModified = await connection.GetStructureFileLastModifiedDateAsync(CancellationToken.None).ConfigureAwait(false);
            if (lastModified > structureFile.LastModified)
            {
                // Structure file cached locally is outdated, throw it away.
                structureFile = null;
            }

            if (structureFile == null)
            {
                // The structure file either did not exist on disk or was outdated. Download a fresh copy from
                // miniserver right now.
                Console.WriteLine("Downloading structure file...");
                structureFile = await connection.DownloadStructureFileAsync(CancellationToken.None).ConfigureAwait(false);

                // Save it locally on disk.
                await structureFile.SaveAsync("LoxAPP3.json", CancellationToken.None).ConfigureAwait(false);
            }

            Console.WriteLine($"Structure file loaded, culture {structureFile.Localization.Culture}.");


            Console.WriteLine("Enabling status updates...");
            await connection.EnableStatusUpdatesAsync(CancellationToken.None).ConfigureAwait(false);
            Thread.Sleep(500);
            await connection.EnableStatusUpdatesAsync(CancellationToken.None).ConfigureAwait(false);

            Thread.Sleep(Timeout.Infinite);
        }

        private static void StructureFileRoundTrip()
        {
            var structureFile = StructureFile.LoadAsync("LoxAPP3.json", CancellationToken.None).GetAwaiter().GetResult();
            structureFile.SaveAsync("LoxAPP3.roundtripped.json", CancellationToken.None).GetAwaiter().GetResult();
        }

        static void Main(string[] args)
        {
            ////StructureFileRoundTrip();

            try
            {
                MainAsync(args).GetAwaiter().GetResult();
            }
            catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
