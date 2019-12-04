// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Years.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Years.cs" was last formatted by Protiguous on 2019/08/08 at 9:11 AM.

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
    public class Years : IComparable<Years>, IQuantityOfTime {

        /// <summary>
        ///     One <see cref="Years" /> .
        /// </summary>
        public static Years One { get; } = new Years( 1 );

        /// <summary>
        /// </summary>
        public static Years Ten { get; } = new Years( 10 );

        /// <summary>
        /// </summary>
        public static Years Thousand { get; } = new Years( 1000 );

        /// <summary>
        ///     Zero <see cref="Years" />
        /// </summary>
        public static Years Zero { get; } = new Years( 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Years( Decimal value ) => this.Value = ( Rational )value;

        public Years( Rational value ) => this.Value = value;

        public Years( Int64 value ) => this.Value = value;

        public Years( BigInteger value ) => this.Value = value;

        [NotNull]
        public static Years Combine( [NotNull] Years left, [NotNull] Years right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left, right.Value );
        }

        [NotNull]
        public static Years Combine( [NotNull] Years left, Decimal years ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return new Years( left.Value + ( Rational ) years );
        }

        [NotNull]
        public static Years Combine( [NotNull] Years left, Rational years ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return new Years( left.Value + years );
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] Years left, [CanBeNull] Years right ) {
            if ( ReferenceEquals(left,right) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            return left.Value == right.Value;
        }

        public static implicit operator Months( [NotNull] Years years ) {
            if ( years == null ) {
                throw new ArgumentNullException( paramName: nameof( years ) );
            }

            return years.ToMonths();
        }

        [NotNull]
        public static implicit operator SpanOfTime( [NotNull] Years years ) {
            if ( years == null ) {
                throw new ArgumentNullException( paramName: nameof( years ) );
            }

            return new SpanOfTime( years: years );
        }

        [NotNull]
        public static Years operator -( [NotNull] Years years ) {
            if ( years == null ) {
                throw new ArgumentNullException( paramName: nameof( years ) );
            }

            return new Years( years.Value * -1 );
        }

        [NotNull]
        public static Years operator -( [NotNull] Years left, [NotNull] Years right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left: left, right: -right );
        }

        [NotNull]
        public static Years operator -( [NotNull] Years left, Decimal years ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            return Combine( left, -years );
        }

        public static Boolean operator !=( [NotNull] Years left, [NotNull] Years right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return !Equals( left, right );
        }

        [NotNull]
        public static Years operator +( Years left, Years right ) => Combine( left, right );

        [NotNull]
        public static Years operator +( Years left, Decimal years ) => Combine( left, years );

        [NotNull]
        public static Years operator +( Years left, BigInteger years ) => Combine( left, years );

        public static Boolean operator <( Years left, Years right ) => left.Value < right.Value;

        public static Boolean operator ==( [CanBeNull] Years left, [CanBeNull] Years right ) => Equals( left, right );

        public static Boolean operator >( Years left, Years right ) => left.Value > right.Value;

        public Int32 CompareTo( Years other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( [CanBeNull] Years other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            return obj is Years years && this.Equals( years );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Days ToDays() => new Days( this.Value * Days.InOneCommonYear );

        public Months ToMonths() => new Months( this.Value * Months.InOneCommonYear );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * ( Rational )PlanckTimes.InOneYear );

        [NotNull]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneCommonYear );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "year" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "year" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        public Weeks ToWeeks() => new Weeks( this.Value * ( Rational )Weeks.InOneCommonYear );
    }
}