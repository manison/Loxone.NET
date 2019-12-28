// ----------------------------------------------------------------------
// <copyright file="ValueStateEventArgs.cs">
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
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public sealed class ValueStateEventArgs : EventArgs
    {
        private IReadOnlyList<ValueState> _valueStates;

        public IReadOnlyList<ValueState> ValueStates => _valueStates;

        public ValueStateEventArgs(IReadOnlyList<ValueState> valueStates)
        {
            Contract.Requires(valueStates != null);
            this._valueStates = valueStates;
        }
    }
}
