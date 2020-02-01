// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Versus.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Versus.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;
    using Rationals;

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

        public Int64 Failures {
            get => Thread.VolatileRead( ref this._failures );

            private set => Thread.VolatileWrite( ref this._failures, value );
        }

        public Int64 Successes {
            get => Thread.VolatileRead( ref this._successes );

            private set => Thread.VolatileWrite( ref this._successes, value );
        }

        public Int64 Total => this.Successes + this.Failures;

        public Versus( Int64 successes = 0, Int64 failures = 0 ) {
            this.Successes = successes;
            this.Failures = failures;
        }

        [NotNull]
        public Versus Clone() => new Versus( successes: this.Successes, failures: this.Failures );

        /// <summary>
        ///     <para>Increments the <see cref="Failures" /> count by <paramref name="amount" />.</para>
        /// </summary>
        public void Failure( Int64 amount = 1 ) => this.Failures += amount;

        public Single FailurePercentage() {
            try {
                var total = this.Total;

                if ( !total.Near( 0 ) ) {
                    var result = new Rational( this.Failures, total );

                    return ( Single )result;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();
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

                var chance = new Rational( this.Successes, total );

                return ( Single )chance;
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();

                return 0;
            }
        }

        public override String ToString() => $"{this.SuccessPercentage():P1} successes vs {this.FailurePercentage():p1} failures out of {this.Total} total.";
    }
}