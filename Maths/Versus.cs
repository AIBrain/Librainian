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
// "Librainian/Versus.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using Newtonsoft.Json;
    using Numerics;

    /// <summary>
    ///     <para>Keep count of Success or Failure counts (threadsafe).</para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{ToString(),nq}" )]
    public class Versus {

        /// <summary>One failure.</summary>
        public static readonly Versus Failured = new Versus( successes: 0, failures: 1 );

        /// <summary>One success.</summary>
        public static readonly Versus Successed = new Versus( successes: 1, failures: 0 );

        /// <summary>None for either.</summary>
        public static readonly Versus Zero = new Versus( successes: 0, failures: 0 );

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Int64 _failures;

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Int64 _successes;

        public Versus( Int64 successes = 0, Int64 failures = 0 ) {
            this.Successes = successes;
            this.Failures = failures;
        }

        public Int64 Failures {
            get {
                return Thread.VolatileRead( ref this._failures );
            }

            private set {
                Thread.VolatileWrite( ref this._failures, value );
            }
        }

        public Int64 Successes {
            get {
                return Thread.VolatileRead( ref this._successes );
            }

            private set {
                Thread.VolatileWrite( ref this._successes, value );
            }
        }

        public Int64 Total => this.Successes + this.Failures;

        public Versus Clone() => new Versus( successes: this.Successes, failures: this.Failures );

        /// <summary>
        ///     <para>Increments the <see cref="Failures" /> count by <paramref name="amount" />.</para>
        /// </summary>
        public void Failure( Int64 amount = 1 ) => this.Failures += amount;

        public Single FailurePercentage() {
            try {
                var total = this.Total;
                if ( !total.Near( 0 ) ) {
                    var result = new BigRational( this.Failures, total );
                    return ( Single )result;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.More();
            }
            return 0;
        }

        /// <summary>
        ///     <para>Increments the <see cref="Successes" /> count by <paramref name="amount" />.</para>
        /// </summary>
        public void Success( Int64 amount = 1 ) => this.Successes += amount;

        public Single SuccessPercentage() {
            try {
                var total = this.Total;
                if ( total.Near( 0 ) ) {
                    return 0;
                }
                var chance = new BigRational( this.Successes, total );
                return ( Single )chance;
            }
            catch ( DivideByZeroException exception ) {
                exception.More();
                return 0;
            }
        }

        public override String ToString() => $"{this.SuccessPercentage():P1} successes vs {this.FailurePercentage():p1} failures out of {this.Total} total.";
    }
}