// ----------------------------------------------------------------------
// <copyright file="Category.cs">
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
    using System.Drawing;

    public sealed class Category : IEquatable<Category>
    {
        private Transport.Category _innerCategory;

        public Uuid Uuid => _innerCategory.Uuid;

        public string Name => _innerCategory.Name;

        // There should be support for Color type in .NET Standard 1.7
        public Color Color => _innerCategory.Color;

        internal Category(Transport.Category category)
        {
            Contract.Requires(category != null);
            this._innerCategory = category;
        }

        public bool Equals(Category other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Uuid == other.Uuid;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Category);
        }

        public override int GetHashCode()
        {
            return Uuid.GetHashCode();
        }

        public override string ToString()
        {
            return Name ?? Uuid.ToString();
        }
    }
}
