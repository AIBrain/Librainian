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
// "Librainian/DeserializeReportStats.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Persistence {

    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Magic;
    using Measurement.Time;
    using Threading;

    public sealed class DeserializeReportStats : ABetterClassDispose {
        public readonly TimeSpan Timing;
        private readonly ThreadLocal<Int64> _gains = new ThreadLocal<Int64>( trackAllValues: true );

        private readonly ThreadLocal<Int64> _losses = new ThreadLocal<Int64>( trackAllValues: true );

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            this._gains.Dispose();
            this._losses.Dispose();
        }

        public DeserializeReportStats( Action<DeserializeReportStats> handler, TimeSpan? timing = null ) {
            this._gains.Values.Clear();
            this._gains.Value = 0;

            this._losses.Values.Clear();
            this._losses.Value = 0;

            this.Total = 0;
            this.Handler = handler;
            this.Timing = timing ?? Milliseconds.ThreeHundredThirtyThree;
        }

        public Boolean Enabled {
            get; set;
        }

        public Int64 Total {
            get; set;
        }

        private Action<DeserializeReportStats> Handler {
            get;
        }

        public void AddFailed( Int64 amount = 1 ) => this._losses.Value += amount;

        public void AddSuccess( Int64 amount = 1 ) => this._gains.Value += amount;

        public Int64 GetGains() => this._gains.Values.Sum( arg => arg );

        public Int64 GetLoss() => this._losses.Values.Sum( arg => arg );

        public async Task StartReporting() {
            this.Enabled = true;
            await this.Timing.Then( job: this.Report );
        }

        public void StopReporting() => this.Enabled = false;

        /// <summary>Perform a Report.</summary>
        private async void Report() {
            if ( !this.Enabled ) {
                return;
            }
            var handler = this.Handler;
            if ( handler is null ) {
                return;
            }

            handler( this );

            if ( this.Enabled ) {
                await this.Timing.Then( job: this.Report ); //TODO is this correct?
            }
        }


    }
}