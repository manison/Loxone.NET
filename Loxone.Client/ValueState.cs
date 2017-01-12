// ----------------------------------------------------------------------
// <copyright file="ValueState.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    public struct ValueState
    {
        private Uuid _uuid;

        public Uuid Uuid => _uuid;

        private double _value;

        public double Value => _value;

        public ValueState(Uuid uuid, double value)
        {
            this._uuid = uuid;
            this._value = value;
        }

        public override string ToString()
        {
            return string.Concat(_uuid.ToString(), ": ", _value.ToString());
        }
    }
}
