// ----------------------------------------------------------------------
// <copyright file="TextStateEventArgs.cs">
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

    public sealed class TextStateEventArgs : EventArgs
    {
        private IReadOnlyList<TextState> _textStates;

        public IReadOnlyList<TextState> TextStates => _textStates;

        public TextStateEventArgs(IReadOnlyList<TextState> textStates)
        {
            Contract.Requires(textStates != null);
            this._textStates = textStates;
        }
    }
}
