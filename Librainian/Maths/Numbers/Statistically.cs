// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Statistically.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Statistically.cs" was last formatted by Protiguous on 2020/03/16 at 2:56 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using JetBrains.Annotations;
    using Logging;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Ups: Probability between 0.0 and 100.0%</para>
    ///     <para>Downs: Probability between 0.0 and 100.0%</para>
    /// </summary>
    /// <remarks>Not thread safe.</remarks>
    [JsonObject]
    public class Statistically {

        public static readonly Statistically Zero = new Statistically( 0, 0 );

        public static Statistically Undecided = new Statistically( 0.5, 0.5 );

        [JsonProperty]
        public Double Downs { get; private set; }

        public Boolean IsDowner => this.Downs > this.Ups;

        public Boolean IsProtiguous => this.IsUpper && !this.Downs.Near( 0 ) && !this.Ups.Near( 0 );

        public Boolean IsUpper => this.Ups > this.Downs;

        [JsonProperty]
        public Double Total { get; private set; }

        [JsonProperty]
        public Double Ups { get; private set; }

        //public static Double Combine( Double value1, Double value2 ) { return ( value1 + value2 ) / 2D; }
        public Statistically( Double ups = 0d, Double downs = 0d ) => Reset( this, ups, downs );

        [NotNull]
        public static Statistically Combine( [NotNull] Statistically value1, [NotNull] Statistically value2 ) =>
            new Statistically( value1.Ups + value2.Ups, value1.Downs + value2.Downs );

        public static void Reset( [NotNull] Statistically statistically, Double newUps = 0.0, Double newDowns = 0.0 ) {
            statistically.Ups = 0d;
            statistically.Downs = 0d;
            statistically.Total = 0d;
            statistically.IncrementUps( newUps );
            statistically.IncrementDowns( newDowns );
        }

        public void Add( [NotNull] Statistically other ) {
            this.IncrementUps( other.Ups );
            this.IncrementDowns( other.Downs );
        }

        [NotNull]
        public Statistically Clone() => new Statistically( this.Ups, this.Downs );

        public void DecrementDowns( Double byAmount = 1d ) {
            this.Downs -= byAmount;
            this.Total -= byAmount;
        }

        public void DecrementDownsIfAny( Double byAmount = 1d ) {
            if ( this.Downs < byAmount ) {
                return;
            }

            this.Downs -= byAmount;
            this.Total -= byAmount;
        }

        public void DecrementUps( Double byAmount = 1d ) {
            this.Ups -= byAmount;
            this.Total -= byAmount;
        }

        public void DecrementUpsIfAny( Double byAmount = 1d ) {
            if ( this.Ups < byAmount ) {
                return;
            }

            this.Ups -= byAmount;
            this.Total -= byAmount;
        }

        public Double GetDownProbability() {
            try {
                var total = this.Total;

                if ( !total.Near( 0 ) ) {
                    return this.Downs / total;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();
            }

            return 0;
        }

        public Double GetUpProbability() {
            try {
                var total = this.Total;

                if ( !total.Near( 0 ) ) {
                    return this.Ups / total;
                }
            }
            catch ( DivideByZeroException exception ) {
                exception.Log();
            }

            return 0;
        }

        /// <summary>Increments <see cref="Downs" /> and <see cref="Total" /> by <paramref name="byAmount" />.</summary>
        /// <param name="byAmount"></param>
        public void IncrementDowns( Double byAmount = 1 ) {
            this.Downs += byAmount;
            this.Total += byAmount;
        }

        /// <summary>Increments <see cref="Ups" /> and <see cref="Total" /> by <paramref name="byAmount" />.</summary>
        /// <param name="byAmount"></param>
        public void IncrementUps( Double byAmount = 1 ) {
            this.Ups += byAmount;
            this.Total += byAmount;
        }

        public override String ToString() => $"U:{this.Ups:f1} vs D:{this.Downs:f1} out of {this.Total:f1}";
    }
}