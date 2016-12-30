// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public sealed class StructureFile
    {
        private Transport.StructureFile _innerFile;

        public DateTime LastModified
        {
            get
            {
                // Structure file uses local time (miniserver based).
                return DateTime.SpecifyKind(_innerFile.LastModified, DateTimeKind.Local);
            }
        }

        private StructureFile(Transport.StructureFile innerFile)
        {
            Contract.Requires(innerFile != null);
            this._innerFile = innerFile;
        }

        public static StructureFile Parse(string s)
        {
            var transportFile = Transport.Serialization.SerializationHelper.Deserialize<Transport.StructureFile>(s);
            return new StructureFile(transportFile);
        }

        public static async Task<StructureFile> LoadAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return await LoadAsync(file, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<StructureFile> LoadAsync(Stream stream, CancellationToken cancellationToken)
        {
            using (var reader = new StreamReader(stream))
            {
                return await LoadAsync(reader, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<StructureFile> LoadAsync(TextReader reader, CancellationToken cancellationToken)
        {
            string s = await reader.ReadToEndAsync().ConfigureAwait(false);
            return Parse(s);
        }

        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await SaveAsync(file, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task SaveAsync(Stream stream, CancellationToken cancellationToken)
        {
            using (var writer = new StreamWriter(stream))
            {
                await SaveAsync(writer, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task SaveAsync(TextWriter writer, CancellationToken cancellationToken)
        {
            var serializer = Transport.Serialization.SerializationHelper.CreateSerializer();
            serializer.TraceWriter = new Newtonsoft.Json.Serialization.MemoryTraceWriter() { LevelFilter = TraceLevel.Verbose };
            serializer.Serialize(writer, _innerFile);
            return Task.FromResult(0);
        }
    }
}
