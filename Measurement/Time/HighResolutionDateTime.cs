// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/HighResolutionDateTime.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     From https://manski.net/2014/07/high-resolution-clock-in-csharp/
    /// </summary>
    public static class HighResolutionDateTime {

        static HighResolutionDateTime() {
            try {
                Int64 filetime;
                GetSystemTimePreciseAsFileTime( out filetime );
                IsAvailable = true;
            }
            catch ( EntryPointNotFoundException ) {

                // Not running Windows 8 or higher.
                IsAvailable = false;
            }
        }

        public static Boolean IsAvailable {
            get;
        }

        public static DateTime UtcNow {
            get {
                if ( !IsAvailable ) {
                    throw new InvalidOperationException( "High resolution clock isn't available." );
                }
                Int64 filetime;
                GetSystemTimePreciseAsFileTime( out filetime );
                return DateTime.FromFileTimeUtc( filetime );
            }
        }

        [DllImport( "Kernel32.dll", CallingConvention = CallingConvention.Winapi )]
        private static extern void GetSystemTimePreciseAsFileTime( out Int64 filetime );
    }
}