// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Extensions.cs" was last cleaned by Rick on 2015/06/12 at 2:56 PM

namespace Librainian.Internet.Servers {

    using System;

    public static class Extensions {

        /// <summary>
        /// Returns the date and time formatted for insertion as the expiration date in a
        /// "Set-Cookie" header.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToCookieTime(this DateTime time) => time.ToString( "dd MMM yyyy hh:mm:ss GMT" );
    }
}