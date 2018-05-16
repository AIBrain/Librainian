// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DeserializeReportStats.cs",
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
// "Librainian/Librainian/DeserializeReportStats.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Persistence {

    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Magic;
    using Measurement.Time;
    using Threading;

    public sealed class DeserializeReportStats : ABetterClassDispose {

        private readonly ThreadLocal<Int64> _gains = new ThreadLocal<Int64>( trackAllValues: true );

        private readonly ThreadLocal<Int64> _losses = new ThreadLocal<Int64>( trackAllValues: true );

        public readonly TimeSpan Timing;

        public DeserializeReportStats( Action<DeserializeReportStats> handler, TimeSpan? timing = null ) {
            this._gains.Values.Clear();
            this._gains.Value = 0;

            this._losses.Values.Clear();
            this._losses.Value = 0;

            this.Total = 0;
            this.Handler = handler;
            this.Timing = timing ?? Milliseconds.ThreeHundredThirtyThree;
        }

        private Action<DeserializeReportStats> Handler { get; }

        public Boolean Enabled { get; set; }

        public Int64 Total { get; set; }

        /// <summary>
        ///     Perform a Report.
        /// </summary>
        private async Task Report() {
            if ( !this.Enabled ) { return; }

            var handler = this.Handler;

            if ( handler is null ) { return; }

            handler( this );

            if ( this.Enabled ) {
                await this.Timing.Then( async () => await this.Report() ); //TODO is this correct?
            }
        }

        public void AddFailed( Int64 amount = 1 ) => this._losses.Value += amount;

        public void AddSuccess( Int64 amount = 1 ) => this._gains.Value += amount;

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            this._gains.Dispose();
            this._losses.Dispose();
        }

        public Int64 GetGains() => this._gains.Values.Sum( arg => arg );

        public Int64 GetLoss() => this._losses.Values.Sum( arg => arg );

        public async Task StartReporting() {
            this.Enabled = true;
            await this.Timing.Then( async () => await this.Report() );
        }

        public void StopReporting() => this.Enabled = false;
    }
}