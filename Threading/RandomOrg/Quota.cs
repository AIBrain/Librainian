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
// "Librainian/Quota.cs" was last cleaned by Rick on 2015/06/12 at 3:14 PM

namespace Librainian.Threading.RandomOrg {

    using System;
    using System.Net;
    using Internet;

    public static class Quota {
        public const Int64 Error = Int64.MinValue;
        private const String Unexpected = "Error: unexpected data.";

        public static Int64 Check() {
            Int64 result;
            return Int64.TryParse( "http://www.random.org/quota/?format=plain".GetWebPage(), out result ) ? result : Error;
        }

        public static Int64 Check(IPAddress ip) {
            Int64 result;
            return Int64.TryParse( $"http://www.random.org/quota/?ip={ip}&format=plain".GetWebPage(), out result ) ? result : Error;
        }
    }
}