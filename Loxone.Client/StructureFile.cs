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

    public sealed class StructureFile
    {
        private Transport.StructureFile _innerFile;

        internal Transport.StructureFile InnerFile => _innerFile;

        public DateTime LastModified
        {
            get
            {
                // Structure file uses local time (miniserver based).
                return DateTime.SpecifyKind(_innerFile.LastModified, DateTimeKind.Local);
            }
        }

        private MiniserverInfo _miniserverInfo;

        public MiniserverInfo MiniserverInfo => _miniserverInfo;

        private ProjectInfo _project;

        public ProjectInfo Project => _project;

        private LocalizationInfo _localization;

        public LocalizationInfo Localization => _localization;

        private RoomCollection _rooms;

        public RoomCollection Rooms
        {
            get
            {
                if (_rooms == null)
                {
                    _rooms = new RoomCollection(_innerFile.Rooms);
                }

                return _rooms;
            }
        }

        private CategoryCollection _categories;

        public CategoryCollection Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new CategoryCollection(_innerFile.Categories);
                }

                return _categories;
            }
        }

        private StructureFile(Transport.StructureFile innerFile)
        {
            Contract.Requires(innerFile != null);
            this._innerFile = innerFile;
            this._miniserverInfo = new MiniserverInfo(_innerFile.MiniserverInfo);
            this._project = new ProjectInfo(_innerFile.MiniserverInfo);
            this._localization = new LocalizationInfo(_innerFile.MiniserverInfo);
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
            var transportFile = await Transport.Serialization.SerializationHelper.DeserializeAsync<Transport.StructureFile>(
                stream, cancellationToken).ConfigureAwait(false);
            return new StructureFile(transportFile);
        }

        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await SaveAsync(file, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task SaveAsync(Stream stream, CancellationToken cancellationToken)
            => Transport.Serialization.SerializationHelper.SerializeAsync(stream, _innerFile, cancellationToken);
    }
}
