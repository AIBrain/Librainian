// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "CountDownWatch.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "CountDownWatch.cs" was last formatted by Protiguous on 2020/03/18 at 10:25 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Timers;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>The 'reverse' of the Stopwatch class. //TODO needs unit testing.</summary>
    [JsonObject( MemberSerialization.Fields )]
    public class CountDownWatch {

        [NotNull]
        private Action Liftoff { get; }

        private Timer Timer { get; set; }

        public TimeSpan Countdown { get; }

        public Boolean HasLaunched { get; private set; }

        public Boolean IsRunning { get; private set; }

        public DateTime TargetTime { get; private set; }

        public DateTime WhenStarted { get; private set; }

        public DateTime WhenStopped { get; private set; }

        /// <summary></summary>
        /// <param name="countdown"></param>
        /// <param name="liftoff">Action to invoke when countdown reaches zero.</param>
        public CountDownWatch( TimeSpan countdown, [CanBeNull] Action liftoff = null ) {
            if ( countdown < TimeSpan.Zero ) {
                throw new ArgumentOutOfRangeException( nameof( countdown ), "Must be a positive value." );
            }

            this.Countdown = countdown;
            this.TargetTime = DateTime.UtcNow.Add( this.Countdown );

            this.Liftoff = () => {
                try {
                    this.HasLaunched = true;
                    liftoff?.Invoke();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            };
        }

        public TimeSpan Remaining() {
            if ( this.IsRunning ) {
                return this.Countdown.Subtract( DateTime.UtcNow - this.WhenStarted );
            }

            if ( this.HasLaunched ) {
                return this.Countdown.Subtract( this.WhenStopped - this.WhenStarted );
            }

            throw new InvalidOperationException( "???" );
        }

        public void Start() {
            this.WhenStarted = DateTime.UtcNow;
            this.IsRunning = true;

            this.Timer = this.Countdown.Create( () => {
                this.Stop();
                this.Liftoff();
            } ).Once().Begin();
        }

        public void Stop() {
            this.IsRunning = false;
            this.WhenStopped = DateTime.UtcNow;
            this.Timer.Stop();
        }

    }

}