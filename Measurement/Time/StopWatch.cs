// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "StopWatch.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/StopWatch.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

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
    ///     Pulled from Microsoft's Stopwatch() source. Wanted to see how it works.
    ///     Made my changes to it. Needs some unit tests.
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class StopWatch : IComparable<StopWatch>, IComparable<TimeSpan> {

        [JsonProperty]
        private Int64 _endTimeStamp;

        [JsonProperty]
        private volatile Boolean _isRunning;

        [JsonProperty]
        private Int64 _startTimeStamp;

        public const Int64 TicksPerMicrosecond = 10;

        public const Int64 TicksPerMillisecond = 10000;

        public StopWatch() => this.Reset();

        public TimeSpan Elapsed => new TimeSpan( ticks: this.GetElapsedTicks() );

        public Int64 ElapsedMicroseconds => this.GetElapsedTicks() / TicksPerMicrosecond;

        public Int64 ElapsedMilliseconds => this.GetElapsedTicks() / TicksPerMillisecond;

        public Int64 ElapsedTicks => this.GetElapsedTicks();

        public Int64 EndTimeStamp {
            get => Interlocked.Read( ref this._endTimeStamp );

            private set => Interlocked.Exchange( ref this._endTimeStamp, value );
        }

        public Boolean IsRunning {
            get => this._isRunning;

            private set => this._isRunning = value;
        }

        public Int64 StartTimeStamp {
            get => Interlocked.Read( ref this._startTimeStamp );

            private set => Interlocked.Exchange( ref this._startTimeStamp, value );
        }

        [Pure]
        private Int64 GetElapsedTicks() {
            if ( this.IsRunning ) { return DateTime.UtcNow.Ticks - this.StartTimeStamp; }

            return this.EndTimeStamp - this.StartTimeStamp;
        }

        public static implicit operator TimeSpan( StopWatch stopWatch ) => TimeSpan.FromMilliseconds( stopWatch.ElapsedMilliseconds );

        public static StopWatch StartNew() {
            var stopWatch = new StopWatch();
            stopWatch.Start();

            return stopWatch;
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        ///     Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance
        ///     occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows
        ///     <paramref name="other" /> in the sort order.
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param>
        public Int32 CompareTo( StopWatch other ) => this.GetElapsedTicks().CompareTo( other.GetElapsedTicks() );

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
        ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
        ///     Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance
        ///     occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows
        ///     <paramref name="other" /> in the sort order.
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param>
        public Int32 CompareTo( TimeSpan other ) => this.Elapsed.CompareTo( other );

        public void Pause() => throw new NotImplementedException();

        /// <summary>
        ///     Stops the stopwatch and resets all the values to default.
        /// </summary>
        public void Reset() {
            this.IsRunning = false;
            this.StartTimeStamp = 0;
            this.EndTimeStamp = 0;
        }

        public void Restart() {
            this.StartTimeStamp = DateTime.UtcNow.Ticks;
            this.IsRunning = true;
        }

        public void Resume() {
            this.Start();

            throw new NotImplementedException();
        }

        public void Start() {
            if ( this.IsRunning ) { return; }

            this.StartTimeStamp = DateTime.UtcNow.Ticks; //BUG possible bug here?
            this.IsRunning = true;
        }

        public void Stop() {
            if ( !this.IsRunning ) { return; }

            this.EndTimeStamp = DateTime.UtcNow.Ticks;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override String ToString() => this.Elapsed.ToString( "g" );
    }
}