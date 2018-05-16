// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Versus.cs",
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
// "Librainian/Librainian/Versus.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

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
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Versus {

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Int64 _failures;

        /// <summary>ONLY used in the getter and setter.</summary>
        [JsonProperty]
        private Int64 _successes;

        /// <summary>One failure.</summary>
        public static readonly Versus Failured = new Versus( successes: 0, failures: 1 );

        /// <summary>One success.</summary>
        public static readonly Versus Successed = new Versus( successes: 1, failures: 0 );

        /// <summary>None for either.</summary>
        public static readonly Versus Zero = new Versus( successes: 0, failures: 0 );

        public Versus( Int64 successes = 0, Int64 failures = 0 ) {
            this.Successes = successes;
            this.Failures = failures;
        }

        public Int64 Failures {
            get => Thread.VolatileRead( ref this._failures );

            private set => Thread.VolatileWrite( ref this._failures, value );
        }

        public Int64 Successes {
            get => Thread.VolatileRead( ref this._successes );

            private set => Thread.VolatileWrite( ref this._successes, value );
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
            catch ( DivideByZeroException exception ) { exception.More(); }

            return 0;
        }

        /// <summary>
        ///     <para>Increments the <see cref="Successes" /> count by <paramref name="amount" />.</para>
        /// </summary>
        public void Success( Int64 amount = 1 ) => this.Successes += amount;

        public Single SuccessPercentage() {
            try {
                var total = this.Total;

                if ( total.Near( 0 ) ) { return 0; }

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