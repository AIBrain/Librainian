// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Picoseconds.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Picoseconds.cs" was last formatted by Protiguous on 2020/03/18 at 10:25 AM.

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

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Picoseconds : IComparable<Picoseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOneNanosecond = 1000;

        /// <summary>Ten <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Fifteen = new Picoseconds( 15 );

        /// <summary>Five <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Five = new Picoseconds( 5 );

        /// <summary>Five Hundred <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds FiveHundred = new Picoseconds( 500 );

        /// <summary>One <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds One = new Picoseconds( 1 );

        /// <summary>One Thousand Nine <see cref="Picoseconds" /> (Prime).</summary>
        public static readonly Picoseconds OneThousandNine = new Picoseconds( 1009 );

        /// <summary>Sixteen <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds Sixteen = new Picoseconds( 16 );

        /// <summary>Ten <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Ten = new Picoseconds( 10 );

        /// <summary>Three <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Three = new Picoseconds( 3 );

        /// <summary>Three Three Three <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds ThreeHundredThirtyThree = new Picoseconds( 333 );

        /// <summary>Two <see cref="Picoseconds" /> s.</summary>
        public static readonly Picoseconds Two = new Picoseconds( 2 );

        /// <summary>Two Hundred <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds TwoHundred = new Picoseconds( 200 );

        /// <summary>Two Hundred Eleven <see cref="Picoseconds" /> (Prime).</summary>
        public static readonly Picoseconds TwoHundredEleven = new Picoseconds( 211 );

        /// <summary>Two Thousand Three <see cref="Picoseconds" /> (Prime).</summary>
        public static readonly Picoseconds TwoThousandThree = new Picoseconds( 2003 );

        /// <summary>Zero <see cref="Picoseconds" />.</summary>
        public static readonly Picoseconds Zero = new Picoseconds( 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Picoseconds( Decimal value ) => this.Value = ( Rational ) value;

        public Picoseconds( Rational value ) => this.Value = value;

        public Picoseconds( Int64 value ) => this.Value = value;

        public Picoseconds( BigInteger value ) => this.Value = value;

        public static Picoseconds Combine( Picoseconds left, Picoseconds right ) => Combine( left, right.Value );

        public static Picoseconds Combine( Picoseconds left, Rational picoseconds ) => new Picoseconds( left.Value + picoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Picoseconds left, Picoseconds right ) => left.Value == right.Value;

        public static implicit operator Femtoseconds( Picoseconds picoseconds ) => picoseconds.ToFemtoseconds();

        public static implicit operator Nanoseconds( Picoseconds picoseconds ) => picoseconds.ToNanoseconds();

        [NotNull]
        public static implicit operator SpanOfTime( Picoseconds picoseconds ) => new SpanOfTime( picoseconds );

        public static Picoseconds operator -( Picoseconds nanoseconds ) {
            if ( nanoseconds == null ) {
                throw new ArgumentNullException( nameof( nanoseconds ) );
            }

            return new Picoseconds( nanoseconds.Value * -1 );
        }

        public static Picoseconds operator -( Picoseconds left, Picoseconds right ) => Combine( left, -right );

        public static Picoseconds operator -( Picoseconds left, Decimal nanoseconds ) => Combine( left, ( Rational ) ( -nanoseconds ) );

        public static Boolean operator !=( Picoseconds left, Picoseconds right ) => !Equals( left, right );

        public static Picoseconds operator +( Picoseconds left, Picoseconds right ) => Combine( left, right );

        public static Picoseconds operator +( Picoseconds left, Decimal nanoseconds ) => Combine( left, ( Rational ) nanoseconds );

        public static Boolean operator <( Picoseconds left, Picoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Picoseconds left, Picoseconds right ) => Equals( left, right );

        public static Boolean operator >( Picoseconds left, Picoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Picoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Picoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Picoseconds picoseconds && this.Equals( picoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value * Femtoseconds.InOnePicosecond );

        public Nanoseconds ToNanoseconds() => new Nanoseconds( this.Value / InOneNanosecond );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational ) PlanckTimes.InOnePicosecond * this.Value );

        public Seconds ToSeconds() => this.ToNanoseconds().ToSeconds();

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "ps" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( "ps" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

    }

}