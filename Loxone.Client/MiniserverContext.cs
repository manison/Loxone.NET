// ----------------------------------------------------------------------
// <copyright file="MiniserverContext.cs">
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
    using Loxone.Client.Controls;

    public class MiniserverContext : IDisposable
    {
        private bool _disposed;

        private bool _ownsConnection;

        private MiniserverConnection _connection;

        public MiniserverConnection Connection
        {
            get
            {
                CheckDisposed();
                return _connection;
            }

            set => SetConnection(value, ownsConnection: false, nameof(value), throwOnNull: false);
        }

        private StructureFile _structureFile;

        public StructureFile StructureFile
        {
            get
            {
                CheckDisposed();
                return _structureFile;
            }

            set => SetStructureFile(value, nameof(value), throwOnNull: false);
        }

        private ControlCollection _controls = new ControlCollection();

        public ControlCollection Controls => _controls;

        private Dictionary<Uuid, Control> _stateToControl = new Dictionary<Uuid, Control>();

        public MiniserverContext()
        {
        }

        public MiniserverContext(StructureFile structureFile)
        {
            SetStructureFile(structureFile, nameof(structureFile), throwOnNull: true);
        }

        public MiniserverContext(MiniserverConnection connection) : this(connection, true)
        {
        }

        public MiniserverContext(MiniserverConnection connection, bool ownsConnection)
        {
            SetConnection(connection, ownsConnection, nameof(connection), throwOnNull: true);
        }

        public MiniserverContext(StructureFile structureFile, MiniserverConnection connection) : this(structureFile, connection, true)
        {
        }

        public MiniserverContext(StructureFile structureFile, MiniserverConnection connection, bool ownsConnection)
        {
            SetStructureFile(structureFile, nameof(structureFile), throwOnNull: true);
            SetConnection(connection, ownsConnection, nameof(connection), throwOnNull: true);
        }

        private void SetConnection(MiniserverConnection connection, bool ownsConnection, string parameterName, bool throwOnNull)
        {
            if (connection == null && throwOnNull)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (connection.IsDisposed)
            {
                throw new ArgumentException(Strings.MiniserverContext_ConnectionDisposed, parameterName);
            }

            DisposeConnection();

            _connection = connection;
            _ownsConnection = ownsConnection;

            WireEventHandlers();
        }

        private void SetStructureFile(StructureFile structureFile, string parameterName, bool throwOnNull)
        {
            if (structureFile == null && throwOnNull)
            {
                throw new ArgumentNullException(parameterName);
            }

            _structureFile = structureFile;
            RebuildControls();
        }

        private void WireEventHandlers()
        {
            if (_connection != null)
            {
                _connection.ValueStateChanged += Connection_ValueStateChanged;
                _connection.TextStateChanged += Connection_TextStateChanged;
            }
        }

        private void UnwireEventHandlers()
        {
            if (_connection != null)
            {
                _connection.ValueStateChanged -= Connection_ValueStateChanged;
                _connection.TextStateChanged -= Connection_TextStateChanged;
            }
        }

        private void Connection_ValueStateChanged(object sender, ValueStateEventArgs e)
        {
            foreach (var state in e.ValueStates)
            {
                if (_stateToControl.TryGetValue(state.Control, out var control))
                {
                    control.OnValueStateUpdate(state);
                }
            }
        }

        private void Connection_TextStateChanged(object sender, TextStateEventArgs e)
        {
        }

        private void RebuildControls()
        {
            _controls.Clear(_structureFile?.InnerFile?.Controls?.Count);
            _stateToControl.Clear();
            if (_structureFile != null)
            {
                foreach (var controlPair in _structureFile.InnerFile.Controls)
                {
                    var innerControl = controlPair.Value;
                    var control = Control.CreateControl(innerControl);
                    control.Room = _structureFile.Rooms[innerControl.Room.Value];
                    control.Category = _structureFile.Categories[innerControl.Category.Value];
                    _controls.Add(control);
                    if (innerControl.States != null)
                    {
                        foreach (var statePair in innerControl.States)
                        {
                            _stateToControl.Add(statePair.Value, control);
                        }
                    }
                }
            }
        }

        private void DisposeConnection()
        {
            UnwireEventHandlers();
            if (_connection != null && _ownsConnection)
            {
                _connection.Dispose();
                _ownsConnection = false;
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeConnection();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
