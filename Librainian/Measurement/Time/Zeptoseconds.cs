// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Zeptoseconds.cs" belongs to Protiguous@Protiguous.com
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
// Project: "Librainian", "Zeptoseconds.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

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

    /// <summary></summary>
    /// <see cref="http://wikipedia.org/wiki/Zeptosecond" />
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Zeptoseconds : IComparable<Zeptoseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneAttosecond = 1000;

        /// <summary><see cref="Five" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Five = new Zeptoseconds( 5 );

        /// <summary><see cref="One" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds One = new Zeptoseconds( 1 );

        /// <summary><see cref="Seven" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Seven = new Zeptoseconds( 7 );

        /// <summary><see cref="Ten" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Ten = new Zeptoseconds( 10 );

        /// <summary><see cref="Thirteen" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Thirteen = new Zeptoseconds( 13 );

        /// <summary><see cref="Thirty" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Thirty = new Zeptoseconds( 30 );

        /// <summary><see cref="Three" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Three = new Zeptoseconds( 3 );

        /// <summary><see cref="Two" /><see cref="Zeptoseconds" />.</summary>
        public static readonly Zeptoseconds Two = new Zeptoseconds( 2 );

        /// <summary></summary>
        public static readonly Zeptoseconds Zero = new Zeptoseconds( 0 );

        public static Rational InOneSecond { get; } = new BigInteger( 10E21 );

        [JsonProperty]
        public Rational Value { get; }

        public Zeptoseconds( Decimal value ) => this.Value = ( Rational )value;

        public Zeptoseconds( Rational value ) => this.Value = value;

        public Zeptoseconds( Int64 value ) => this.Value = value;

        public Zeptoseconds( BigInteger value ) => this.Value = value;

        public static Zeptoseconds Combine( Zeptoseconds left, Zeptoseconds right ) => Combine( left, right.Value );

        public static Zeptoseconds Combine( Zeptoseconds left, Rational zeptoseconds ) => new Zeptoseconds( left.Value + zeptoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Zeptoseconds left, Zeptoseconds right ) => left.Value == right.Value;

        /// <summary>Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Milliseconds" />.</summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Attoseconds( Zeptoseconds zeptoseconds ) => zeptoseconds.ToAttoseconds();

        [NotNull]
        public static implicit operator SpanOfTime( Zeptoseconds zeptoseconds ) => new SpanOfTime( zeptoseconds );

        public static implicit operator TimeSpan( Zeptoseconds zeptoseconds ) => TimeSpan.FromSeconds( ( Double )zeptoseconds.Value );

        /// <summary>Implicitly convert the number of <paramref name="zeptoseconds" /> to <see cref="Yoctoseconds" />.</summary>
        /// <param name="zeptoseconds"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds( Zeptoseconds zeptoseconds ) => zeptoseconds.ToYoctoseconds();

        public static Zeptoseconds operator -( Zeptoseconds zeptoseconds ) => new Zeptoseconds( zeptoseconds.Value * -1 );

        public static Zeptoseconds operator -( Zeptoseconds left, Zeptoseconds right ) => Combine( left, -right );

        public static Zeptoseconds operator -( Zeptoseconds left, Decimal zeptoseconds ) => Combine( left, ( Rational )( -zeptoseconds ) );

        public static Boolean operator !=( Zeptoseconds left, Zeptoseconds right ) => !Equals( left, right );

        public static Zeptoseconds operator +( Zeptoseconds left, Zeptoseconds right ) => Combine( left, right );

        public static Zeptoseconds operator +( Zeptoseconds left, Decimal zeptoseconds ) => Combine( left, ( Rational )zeptoseconds );

        public static Boolean operator <( Zeptoseconds left, Zeptoseconds right ) => left.Value < right.Value;

        public static Boolean operator <( Zeptoseconds left, Yoctoseconds right ) => left < ( Zeptoseconds )right;

        public static Boolean operator ==( Zeptoseconds left, Zeptoseconds right ) => Equals( left, right );

        public static Boolean operator >( Zeptoseconds left, Yoctoseconds right ) => left > ( Zeptoseconds )right;

        public static Boolean operator >( Zeptoseconds left, Zeptoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Zeptoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Zeptoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Zeptoseconds zeptoseconds && this.Equals( zeptoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        ///     <para>Convert to a larger unit.</para>
        /// </summary>
        /// <returns></returns>
        public Attoseconds ToAttoseconds() => new Attoseconds( this.Value / InOneAttosecond );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * ( Rational )PlanckTimes.InOneZeptosecond );

        public Seconds ToSeconds() => new Seconds( this.Value * InOneSecond );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "zs" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "zs" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        /// <summary>
        ///     <para>Convert to a smaller unit.</para>
        /// </summary>
        /// <returns></returns>
        public Yoctoseconds ToYoctoseconds() => new Yoctoseconds( this.Value * Yoctoseconds.InOneZeptosecond );
    }
}