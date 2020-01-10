// ----------------------------------------------------------------------
// <copyright file="Control.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Control
    {
        private static readonly Dictionary<string, Func<Control>> _factories = new Dictionary<string, Func<Control>>(1)
        {
            {  "Switch", () => new Switch() },
        };

        private Transport.Control _innerControl;

        internal Transport.Control InnerControl
        {
            get => _innerControl;
            private set
            {
                _innerControl = value;
                Initialize();
            }
        }

        public Uuid Uuid => InnerControl.Uuid;

        public string Name => InnerControl.Name;

        public Room Room { get; internal set; }

        public Category Category { get; internal set; }

        protected Control()
        {
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void UpdateValueState(ValueState state)
        {
        }

        internal void OnValueStateUpdate(ValueState state) => UpdateValueState(state);

        protected string GetStateNameByUuid(Uuid uuid) => InnerControl.States
            .Where(pair => pair.Value == uuid)
            .Select(pair => pair.Key)
            .First();

        protected Uuid GetStateUuidByName(string name) => InnerControl.States[name];

        internal static Control CreateControl(Transport.Control innerControl)
        {
            if (!_factories.TryGetValue(innerControl.ControlType, out var factory))
            {
                factory = () => new Control();
            }

            var control = factory();
            control.InnerControl = innerControl;
            return control;
        }
    }
}
