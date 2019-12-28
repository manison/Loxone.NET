// ----------------------------------------------------------------------
// <copyright file="HttpUtils.cs">
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
    using System.Diagnostics.Contracts;
    using System.Net.Http.Headers;

    internal static class HttpUtils
    {
        public const int DefaultHttpPort = 80;

        public const string HttpScheme = "http";

        public const string WebSocketScheme = "ws";

        public const string JsonMediaType = "application/json";

        public const string BasicAuthenticationScheme = "Basic";

        public static bool IsHttpUri(Uri uri)
        {
            Contract.Requires(uri != null);

            return string.Equals(HttpScheme, uri.Scheme, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsWebSocketUri(Uri uri)
        {
            Contract.Requires(uri != null);

            return string.Equals(WebSocketScheme, uri.Scheme, StringComparison.OrdinalIgnoreCase);
        }

        private static Uri ChangeScheme(Uri uri, string scheme)
        {
            Contract.Requires(uri != null);

            if (!String.Equals(uri.Scheme, scheme, StringComparison.OrdinalIgnoreCase))
            {
                uri = new UriBuilder(uri) { Scheme = scheme }.Uri;
            }

            return uri;
        }

        public static Uri MakeWebSocketUri(Uri uri) => ChangeScheme(uri, WebSocketScheme);

        public static Uri MakeHttpUri(Uri uri) => ChangeScheme(uri, HttpScheme);

        public static bool IsJsonMediaType(MediaTypeHeaderValue mediaType)
        {
            return mediaType != null && string.Equals(mediaType.MediaType, JsonMediaType, StringComparison.OrdinalIgnoreCase);
        }
    }
}
