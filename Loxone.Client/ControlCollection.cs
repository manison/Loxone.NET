// ----------------------------------------------------------------------
// <copyright file="ControlCollection.cs">
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
    using Loxone.Client.Controls;

    public sealed class ControlCollection : IReadOnlyList<Control>
    {
        private List<Control> _toplevelControls;
        private Dictionary<Uuid, Control> _allControlsByUuid;

        public Control this[int index] => _toplevelControls[index];

        public Control this[Uuid uuid] => _allControlsByUuid[uuid];

        public int Count => _toplevelControls.Count;

        public IEnumerator<Control> GetEnumerator() => _toplevelControls.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal ControlCollection()
        {
            this._toplevelControls = new List<Control>();
            this._allControlsByUuid = new Dictionary<Uuid, Control>();
        }

        internal void Clear(int? capacity)
        {
            _toplevelControls.Clear();
            _toplevelControls.Capacity = capacity.GetValueOrDefault();
            _allControlsByUuid.Clear();
        }

        internal void Add(Control control)
        {
            _toplevelControls.Add(control);
            _allControlsByUuid.Add(control.Uuid, control);
        }
    }
}
