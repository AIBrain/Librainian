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
// "Librainian/StopWatch.cs" was last cleaned by Rick on 2015/10/26 at 7:47 AM

namespace Librainian.Measurement.Time {

    using System;

    // ==++==
    // 
    // Copyright (c) Microsoft Corporation. All rights reserved.
    // 
    // ==--==
    /*============================================================
    **
    ** Class:  Stopwatch
    **
    ** Purpose: Implementation for Stopwatch class.
    **
    ** Date:  Nov 27, 2002
    **
    ===========================================================*/

    /// <summary>
    ///     Pulled from Microsoft's source.
    /// </summary>
    public class StopWatch {

        private Int64 _elapsed;

        private Int64 _startTimeStamp;

        public const Int64 TicksPerMillisecond = 10000;

        public StopWatch() {
            this.Reset();
        }

        public TimeSpan Elapsed => new TimeSpan( this.GetElapsedDateTimeTicks() );

        public Int64 ElapsedMilliseconds => this.GetElapsedDateTimeTicks() / TicksPerMillisecond;

        public Int64 ElapsedTicks => this.GetRawElapsedTicks();

        public Boolean IsRunning { get; private set; }

        public static Int64 GetTimestamp() {
            return DateTime.UtcNow.Ticks;
        }

        public static StopWatch StartNew() {
            var s = new StopWatch();
            s.Start();
            return s;
        }

        public Int64 GetElapsedDateTimeTicks() {
            return this.GetRawElapsedTicks();
        }

        public void Reset() {
            this._elapsed = 0;
            this.IsRunning = false;
            this._startTimeStamp = 0;
        }

        public void Restart() {
            this._elapsed = 0;
            this._startTimeStamp = GetTimestamp();
            this.IsRunning = true;
        }

        public void Start() {
            if ( this.IsRunning ) {
                return;
            }
            this._startTimeStamp = GetTimestamp();
            this.IsRunning = true;
        }

        public void Stop() {
            if ( !this.IsRunning ) {
                return;
            }
            var endTimeStamp = GetTimestamp();
            var elapsedThisPeriod = endTimeStamp - this._startTimeStamp;
            this._elapsed += elapsedThisPeriod;
            this.IsRunning = false;

            if ( this._elapsed < 0 ) {
                // When measuring small time periods the StopWatch.Elapsed* properties can return
                // negative values. This is due to bugs in the basic input/output system (BIOS) or
                // the hardware abstraction layer (HAL) on machines with variable-speed CPUs (e.g.
                // Intel SpeedStep).

                this._elapsed = 0;
            }
        }

        // Get the _elapsed ticks.
        private Int64 GetRawElapsedTicks() {
            var timeElapsed = this._elapsed;

            if ( !this.IsRunning ) {
                return timeElapsed;
            }

            // If the StopWatch is running, add _elapsed time since the Stopwatch is started last time.
            var currentTimeStamp = GetTimestamp();
            var elapsedUntilNow = currentTimeStamp - this._startTimeStamp;
            timeElapsed += elapsedUntilNow;
            return timeElapsed;
        }

    }
}
