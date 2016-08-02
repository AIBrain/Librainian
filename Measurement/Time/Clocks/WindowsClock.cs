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
// "Librainian/WindowsClock.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Pulled from BenchmarkDotNet.Horology
    /// </summary>
    public class WindowsClock {
        private static readonly Int64 frequency;
        private static readonly Boolean isAvailable;

        static WindowsClock() {
            try {
                Int64 counter;
                isAvailable = QueryPerformanceFrequency( out frequency ) && QueryPerformanceCounter( out counter );
            }
            catch ( Exception ) {
                isAvailable = false;
            }
        }

        public Int64 Frequency => frequency;

        public Boolean IsAvailable => isAvailable;

        public Int64 GetTimestamp() {
            Int64 value;
            QueryPerformanceCounter( out value );
            return value;
        }

        [DllImport( "kernel32.dll" )]
        private static extern Boolean QueryPerformanceCounter( out Int64 value );

        [DllImport( "kernel32.dll" )]
        private static extern Boolean QueryPerformanceFrequency( out Int64 value );
    }
}