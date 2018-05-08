// Copyright 2016 Protiguous.
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
// "Librainian/Quota.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading.RandomOrg {

    using System;
    using System.Net;
    using Internet;

    public static class Quota {
        public const Int64 Error = Int64.MinValue;
        private const String Unexpected = "Error: unexpected data.";

        public static Int64 Check() => Int64.TryParse( "http://www.random.org/quota/?format=plain".GetWebPage(), out var result ) ? result : Error;

	    public static Int64 Check( IPAddress ip ) => Int64.TryParse( $"http://www.random.org/quota/?ip={ip}&format=plain".GetWebPage(), out var result ) ? result : Error;

    }
}