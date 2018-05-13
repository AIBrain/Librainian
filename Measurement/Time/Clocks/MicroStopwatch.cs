// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MicroStopwatch.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.Clocks {

    using System;
    using System.Diagnostics;

    /// <summary>
    /// MicroStopwatch class
    /// </summary>
    public class MicroStopwatch : Stopwatch {

        /// <summary>
        /// </summary>
        public readonly Double MicrosecondsPerTick = 1000000d / Frequency;

        public MicroStopwatch() {
            if ( !IsHighResolution ) {
                throw new InvalidOperationException( "The high-resolution performance counter is not available" );
            }
        }

        public UInt64 ElapsedMicroseconds => ( UInt64 )( ElapsedTicks * this.MicrosecondsPerTick );
    }
}