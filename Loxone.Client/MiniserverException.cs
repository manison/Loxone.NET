// ----------------------------------------------------------------------
// <copyright file="MiniserverException.cs">
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
    using System.Runtime.Serialization;

    [Serializable]
    public class MiniserverException : Exception
    {
        public MiniserverException()
        {
        }

        public MiniserverException(string message)
            : base(message)
        {
        }

        public MiniserverException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MiniserverException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
