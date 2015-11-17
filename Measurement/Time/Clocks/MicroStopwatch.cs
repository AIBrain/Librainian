// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/MicroStopwatch.cs" was last cleaned by Rick on 2015/10/26 at 6:47 AM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Diagnostics;

    /// <summary>MicroStopwatch class</summary>
    public class MicroStopwatch : Stopwatch {

        private readonly Double _microsecondsPerTick = 1000000d / Frequency;

        public MicroStopwatch() {
            if ( !IsHighResolution ) {
                throw new InvalidOperationException( "The high-resolution performance counter is not available" );
            }
        }

        public UInt64 ElapsedMicroseconds => ( UInt64 ) ( ElapsedTicks * _microsecondsPerTick );

    }

}
