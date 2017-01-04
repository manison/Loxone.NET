// ----------------------------------------------------------------------
// <copyright file="MiniserverCommandException.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    [Serializable]
    public class MiniserverCommandException : MiniserverTransportException
    {
        private int _statusCode;

        public int StatusCode => _statusCode;

        public MiniserverCommandException(int statusCode)
            : base(string.Format(CultureInfo.CurrentCulture, Strings.MiniserverCommandException_MessageFmt, statusCode))
        {
            this._statusCode = statusCode;
        }

        public MiniserverCommandException(int statusCode, string message)
            : base(message)
        {
            this._statusCode = statusCode;
        }

        public MiniserverCommandException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this._statusCode = statusCode;
        }

#if NETFX
        protected MiniserverCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
