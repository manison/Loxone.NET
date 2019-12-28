// ----------------------------------------------------------------------
// <copyright file="Authenticator.cs">
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
    using System.Diagnostics.Contracts;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    internal abstract class Authenticator : IDisposable
    {
        protected readonly NetworkCredential Credentials;

        protected readonly Session Session;

        protected LXClient Client => Session.Client;

        protected Authenticator(Session session, NetworkCredential credentials)
        {
            Contract.Requires(session != null);
            Contract.Requires(credentials != null);
            this.Session = session;
            this.Credentials = credentials;
        }

        public abstract Task AuthenticateAsync(CancellationToken cancellationToken);

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
