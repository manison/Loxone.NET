// ----------------------------------------------------------------------
// <copyright file="CategoryCollection.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public sealed class CategoryCollection : IReadOnlyCollection<Category>
    {
        private readonly IDictionary<string, Transport.Category> _innerCategories;
        private readonly Dictionary<Uuid, Category> _categories;

        internal CategoryCollection(IDictionary<string, Transport.Category> innerCategorys)
        {
            Contract.Requires(innerCategorys != null);
            this._innerCategories = innerCategorys;
            _categories = new Dictionary<Uuid, Category>(_innerCategories.Count);
        }

        public int Count => _innerCategories.Count;

        public IEnumerator<Category> GetEnumerator()
        {
            foreach (var pair in _innerCategories)
            {
                yield return this[Uuid.Parse(pair.Key)];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal Category this[Uuid uuid]
        {
            get
            {
                if (!_categories.TryGetValue(uuid, out var category))
                {
                    var innerCategory = _innerCategories[uuid.ToString()];
                    category = new Category(innerCategory);
                    _categories[uuid] = category;
                }
                return category;
            }
        }
    }
}
