// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Minutes.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Minutes.cs" was last formatted by Protiguous on 2019/08/08 at 9:06 AM.

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
    public struct Minutes : IComparable<Minutes>, IQuantityOfTime {

        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneHour = 60;

        /// <summary>
        ///     15
        /// </summary>
        public static readonly Minutes Fifteen = new Minutes( 15 );

        /// <summary>
        ///     One <see cref="Minutes" /> .
        /// </summary>
        public static readonly Minutes One = new Minutes( 1 );

        /// <summary>
        ///     10
        /// </summary>
        public static readonly Minutes Ten = new Minutes( 10 );

        /// <summary>
        ///     30
        /// </summary>
        public static readonly Minutes Thirty = new Minutes( 30 );

        /// <summary>
        /// </summary>
        public static readonly Minutes Thousand = new Minutes( 1000 );

        /// <summary>
        ///     Zero <see cref="Minutes" />
        /// </summary>
        public static readonly Minutes Zero = new Minutes( 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Minutes( Decimal value ) => this.Value = ( Rational ) value;

        public Minutes( Rational value ) => this.Value = value;

        public Minutes( Int64 value ) => this.Value = value;

        public Minutes( BigInteger value ) => this.Value = value;

        public static Minutes Combine( Minutes left, Minutes right ) => Combine( left, right.Value );

        public static Minutes Combine( Minutes left, Rational minutes ) => new Minutes( left.Value + minutes );

        public static Minutes Combine( Minutes left, BigInteger minutes ) => new Minutes( left.Value + minutes );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Minutes left, Minutes right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Hours" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator Hours( Minutes minutes ) => minutes.ToHours();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to <see cref="Seconds" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        [NotNull]
        public static implicit operator Seconds( Minutes minutes ) => minutes.ToSeconds();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="minutes" /> to a <see cref="SpanOfTime" />.
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static implicit operator SpanOfTime( Minutes minutes ) => new SpanOfTime( minutes );

        public static implicit operator TimeSpan( Minutes minutes ) => TimeSpan.FromMinutes( ( Double ) minutes.Value );

        public static Minutes operator -( Minutes minutes ) => new Minutes( minutes.Value * -1 );

        public static Minutes operator -( Minutes left, Minutes right ) => Combine( left: left, right: -right );

        public static Minutes operator -( Minutes left, Decimal minutes ) => Combine( left, ( Rational ) ( -minutes ) );

        public static Boolean operator !=( Minutes left, Minutes right ) => !Equals( left, right );

        public static Minutes operator +( Minutes left, Minutes right ) => Combine( left, right );

        public static Minutes operator +( Minutes left, Decimal minutes ) => Combine( left, ( Rational ) minutes );

        public static Minutes operator +( Minutes left, BigInteger minutes ) => Combine( left, minutes );

        public static Boolean operator <( Minutes left, Minutes right ) => left.Value < right.Value;

        public static Boolean operator <( Minutes left, Hours right ) => ( Hours ) left < right;

        public static Boolean operator <( Minutes left, Seconds right ) => left < ( Minutes ) right;

        public static Boolean operator ==( Minutes left, Minutes right ) => Equals( left, right );

        public static Boolean operator >( Minutes left, Hours right ) => ( Hours ) left > right;

        public static Boolean operator >( Minutes left, Minutes right ) => left.Value > right.Value;

        public static Boolean operator >( Minutes left, Seconds right ) => left > ( Minutes ) right;

        public Int32 CompareTo( Minutes other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Minutes other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj == null ) {
                return false;
            }

            return obj is Minutes minutes && this.Equals( minutes );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Hours ToHours() => new Hours( this.Value / InOneHour );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational ) PlanckTimes.InOneMinute * this.Value );

        [NotNull]
        [Pure]
        public Seconds ToSeconds() => new Seconds( this.Value * Seconds.InOneMinute );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "minute" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( "minute" )}";
        }

        public TimeSpan ToTimeSpan() => throw new NotImplementedException();
    }
}