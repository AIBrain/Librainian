// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Extensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet.Servers {

    using System;

    public static class Extensions {

        /// <summary>
        ///     Returns the date and time formatted for insertion as the expiration date in a
        ///     "Set-Cookie" header.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String ToCookieTime( this DateTime time ) => time.ToString( "dd MMM yyyy hh:mm:ss GMT" );
    }
}