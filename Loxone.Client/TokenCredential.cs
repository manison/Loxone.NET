// ----------------------------------------------------------------------
// <copyright file="TokenCredential.cs">
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
    using System.Net;

    public class TokenCredential : NetworkCredential
    {
        /// <summary>
        /// Specifies the permission a token needs to be granted. This impacts the
        /// token lifespan.
        /// </summary>
        public TokenPermission Permission { get; set; }

        /// <summary>
        /// Identifies the client who is requesting the token on the Miniserver. It
        /// allows to look up all tokens a client has been granted.This is why the UUID
        /// should either be derived from your devices identity information or generated
        /// automatically and stored within the app.
        /// </summary>
        public Uuid ClientID { get; set; }

        private string _clientName;

        /// <summary>
        /// Text describing the client.
        /// </summary>
        public string ClientName
        {
            get => _clientName;
            set => _clientName = value ?? String.Empty;
        }

        public TokenCredential(string userName, string password, TokenPermission permission, Uuid clientID, string clientName) : base(userName, password)
        {
            this.Permission = permission;
            this.ClientID = clientID;
            this.ClientName = clientName;
        }
    }
}
