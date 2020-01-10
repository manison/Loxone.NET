// ----------------------------------------------------------------------
// <copyright file="Switch.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Controls
{
    public class Switch : Control
    {
        private bool? _activeState;

        public bool? Active
        {
            get => _activeState;
            set
            {

            }
        }

        protected override void UpdateValueState(ValueState state)
        {
            if (GetStateNameByUuid(state.Control) == "active")
            {
                _activeState = state.Value != 0;
            }
        }
    }
}
