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
// "Librainian/Statistically.cs" was last cleaned by Rick on 2016/06/18 at 10:52 PM

namespace Librainian.Maths.Numbers {

    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>Ups: Probability between 0.0 and 100.0%</para>
    ///     <para>Downs: Probability between 0.0 and 100.0%</para>
    /// </summary>
    /// <remarks>Not thread safe.</remarks>
    [JsonObject]
    public class Statistically {
        public static readonly Statistically Zero = new Statistically( ups: 0, downs: 0 );

        //public static Double Combine( Double value1, Double value2 ) { return ( value1 + value2 ) / 2D; }

        public static Statistically Undecided = new Statistically( 0.5, 0.5 );

        public Statistically( Double ups = 0d, Double downs = 0d ) {
            Reset( statistically: this, newUps: ups, newDowns: downs );
        }

        [JsonProperty]
        public Double Downs {
            get; private set;
        }

        public Boolean IsDowner => this.Downs > this.Ups;

        public Boolean IsProtiguous => this.IsUpper && !this.Downs.Near( 0 ) && !this.Ups.Near( 0 );

        public Boolean IsUpper => this.Ups > this.Downs;

        [JsonProperty]
        public Double Total {
            get; private set;
        }

        [JsonProperty]
        public Double Ups {
            get; private set;
        }

        public static Statistically Combine( Statistically value1, Statistically value2 ) => new Statistically( ups: value1.Ups + value2.Ups, downs: value1.Downs + value2.Downs );

        public static void Reset( Statistically statistically, Double newUps = 0.0, Double newDowns = 0.0 ) {
            statistically.Ups = 0d;
            statistically.Downs = 0d;
            statistically.Total = 0d;
            statistically.IncrementUps( newUps );
            statistically.IncrementDowns( newDowns );
        }

        public void Add( Statistically other ) {
            this.IncrementUps( other.Ups );
            this.IncrementDowns( other.Downs );
        }

        public Statistically Clone() => new Statistically( ups: this.Ups, downs: this.Downs );

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
                exception.More();
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
                exception.More();
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