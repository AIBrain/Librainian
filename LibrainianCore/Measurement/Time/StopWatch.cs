// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "StopWatch.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "StopWatch.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

namespace LibrainianCore.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>Pulled from Microsoft's Stopwatch() source code.
    /// <para>Wanted to see how it works.</para>
    /// <para>Made my changes to it. Needs some unit tests.</para>
    /// </summary>
    /// <copyright>Copyright (c) Microsoft Corporation. All rights reserved.</copyright>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Obsolete( "Not really obsolete, but BUGS MIGHT HAVE BEEN INTRODUCED." )]
    public class StopWatch : IComparable<StopWatch>, IComparable<TimeSpan> {

        [JsonProperty]
        private Int64 _endTimeStamp;

        [JsonProperty]
        private volatile Boolean _isRunning;

        [JsonProperty]
        private Int64 _startTimeStamp;

        public const Int64 TicksPerMicrosecond = 10;

        public const Int64 TicksPerMillisecond = 10000;

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

        public StopWatch() => this.Reset();

        private Int64 GetElapsedTicks() {
            if ( this.IsRunning ) {
                return DateTime.UtcNow.Ticks - this.StartTimeStamp;
            }

            return this.EndTimeStamp - this.StartTimeStamp;
        }

        public static implicit operator TimeSpan( [NotNull] StopWatch stopWatch ) => TimeSpan.FromMilliseconds( stopWatch.ElapsedMilliseconds );

        [NotNull]
        public static StopWatch StartNew() {
            var stopWatch = new StopWatch();
            stopWatch.Start();

            return stopWatch;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the
        /// same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes
        /// <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance
        /// follows <paramref name="other" /> in the sort order.
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param>
        public Int32 CompareTo( [NotNull] StopWatch other ) => this.GetElapsedTicks().CompareTo( other.GetElapsedTicks() );

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the
        /// same position in the sort order as the other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes
        /// <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance
        /// follows <paramref name="other" /> in the sort order.
        /// </returns>
        /// <param name="other">An object to compare with this instance. </param>
        public Int32 CompareTo( TimeSpan other ) => this.Elapsed.CompareTo( other );

        public void Pause() => throw new NotImplementedException();

        /// <summary>Stops the stopwatch and resets all the values to default.</summary>
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
            if ( this.IsRunning ) {
                return;
            }

            this.StartTimeStamp = DateTime.UtcNow.Ticks; //BUG possible bug here?
            this.IsRunning = true;
        }

        public void Stop() {
            if ( !this.IsRunning ) {
                return;
            }

            this.EndTimeStamp = DateTime.UtcNow.Ticks;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString() => this.Elapsed.ToString( "g" );
    }
}