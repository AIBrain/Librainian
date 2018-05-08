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
// "Librainian/HighResolutionDateTime.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using OperatingSystem;

    /// <summary>
    ///     From https://manski.net/2014/07/high-resolution-clock-in-csharp/
    /// </summary>
    public static class HighResolutionDateTime {

        static HighResolutionDateTime() {
            try {
				NativeMethods.GetSystemTimePreciseAsFileTime( out var filetime );
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
                    throw new InvalidOperationException( "High resolution clock is not available." );
                }
				NativeMethods.GetSystemTimePreciseAsFileTime( out var filetime );
				return DateTime.FromFileTimeUtc( filetime );
            }
        }

    }
}