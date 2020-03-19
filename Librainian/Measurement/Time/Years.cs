// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Years.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Years.cs" was last formatted by Protiguous on 2020/03/18 at 10:25 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Years : IComparable<Years>, IQuantityOfTime {

        /// <summary>One <see cref="Years" /> .</summary>
        public static Years One { get; } = new Years( 1 );

        /// <summary></summary>
        public static Years Ten { get; } = new Years( 10 );

        /// <summary></summary>
        public static Years Thousand { get; } = new Years( 1000 );

        /// <summary>Zero <see cref="Years" /></summary>
        public static Years Zero { get; } = new Years( 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Years( Decimal value ) => this.Value = ( Rational ) value;

        public Years( Rational value ) => this.Value = value;

        public Years( Int64 value ) => this.Value = value;

        public Years( BigInteger value ) => this.Value = value;

        public static Years Combine( Years left, Years right ) => Combine( left, right.Value );

        public static Years Combine( Years left, Decimal years ) => new Years( left.Value + ( Rational ) years );

        public static Years Combine( Years left, Rational years ) => new Years( left.Value + years );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Years left, Years right ) => left.Value == right.Value;

        public static implicit operator Months( Years years ) => years.ToMonths();

        [NotNull]
        public static implicit operator SpanOfTime( Years years ) => new SpanOfTime( years: years );

        public static Years operator -( Years years ) => new Years( years.Value * -1 );

        public static Years operator -( Years left, Years right ) => Combine( left, -right );

        public static Years operator -( Years left, Decimal years ) => Combine( left, -years );

        public static Boolean operator !=( Years left, Years right ) => !Equals( left, right );

        public static Years operator +( Years left, Years right ) => Combine( left, right );

        public static Years operator +( Years left, Decimal years ) => Combine( left, years );

        public static Years operator +( Years left, BigInteger years ) => Combine( left, years );

        public static Boolean operator <( Years left, Years right ) => left.Value < right.Value;

        public static Boolean operator ==( Years left, Years right ) => Equals( left, right );

        public static Boolean operator >( Years left, Years right ) => left.Value > right.Value;

        public Int32 CompareTo( Years other ) {
            if ( other == null ) {
                throw new ArgumentNullException( nameof( other ) );
            }

            return this.Value.CompareTo( other.Value );
        }

        public Boolean Equals( Years other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Years years && this.Equals( years );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Days ToDays() => new Days( this.Value * Days.InOneCommonYear );

        public Months ToMonths() => new Months( this.Value * Months.InOneCommonYear );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * ( Rational ) PlanckTimes.InOneYear );

        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneCommonYear );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "year" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( "year" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        public Weeks ToWeeks() => new Weeks( this.Value * ( Rational ) Weeks.InOneCommonYear );

    }

}