// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Femtoseconds.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Femtoseconds.cs" was last formatted by Protiguous on 2020/03/16 at 3:08 PM.

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
    /// <see cref="http://wikipedia.org/wiki/Femtosecond" />
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Femtoseconds : IComparable<Femtoseconds>, IQuantityOfTime {

        /// <summary>1000</summary>
        public const UInt16 InOnePicosecond = 1000;

        /// <summary>Ten <see cref="Femtoseconds" /> s.</summary>
        public static readonly Femtoseconds Fifteen = new Femtoseconds( value: 15 );

        /// <summary>Five <see cref="Femtoseconds" /> s.</summary>
        public static readonly Femtoseconds Five = new Femtoseconds( value: 5 );

        /// <summary>Five Hundred <see cref="Femtoseconds" /> s.</summary>
        public static readonly Femtoseconds FiveHundred = new Femtoseconds( value: 500 );

        /// <summary>One <see cref="Femtoseconds" />.</summary>
        public static readonly Femtoseconds One = new Femtoseconds( value: 1 );

        /// <summary>One Thousand Nine <see cref="Femtoseconds" /> (Prime).</summary>
        public static readonly Femtoseconds OneThousandNine = new Femtoseconds( value: 1009 );

        /// <summary>Sixteen <see cref="Femtoseconds" />.</summary>
        public static readonly Femtoseconds Sixteen = new Femtoseconds( value: 16 );

        /// <summary>Ten <see cref="Femtoseconds" /> s.</summary>
        public static readonly Femtoseconds Ten = new Femtoseconds( value: 10 );

        /// <summary>Three <see cref="Femtoseconds" /> s.</summary>
        public static readonly Femtoseconds Three = new Femtoseconds( value: 3 );

        /// <summary>Three Three Three <see cref="Femtoseconds" />.</summary>
        public static readonly Femtoseconds ThreeHundredThirtyThree = new Femtoseconds( value: 333 );

        /// <summary>Two <see cref="Femtoseconds" /> s.</summary>
        public static readonly Femtoseconds Two = new Femtoseconds( value: 2 );

        /// <summary>Two Hundred <see cref="Femtoseconds" />.</summary>
        public static readonly Femtoseconds TwoHundred = new Femtoseconds( value: 200 );

        /// <summary>Two Hundred Eleven <see cref="Femtoseconds" /> (Prime).</summary>
        public static readonly Femtoseconds TwoHundredEleven = new Femtoseconds( value: 211 );

        /// <summary>Two Thousand Three <see cref="Femtoseconds" /> (Prime).</summary>
        public static readonly Femtoseconds TwoThousandThree = new Femtoseconds( value: 2003 );

        /// <summary>Zero <see cref="Femtoseconds" />.</summary>
        public static readonly Femtoseconds Zero = new Femtoseconds( value: 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Femtoseconds( Decimal value ) => this.Value = ( Rational ) value;

        public Femtoseconds( Rational value ) => this.Value = value;

        public Femtoseconds( Int64 value ) => this.Value = value;

        public Femtoseconds( BigInteger value ) => this.Value = value;

        public static Femtoseconds Combine( Femtoseconds left, Femtoseconds right ) => Combine( left: left, femtoseconds: right.Value );

        public static Femtoseconds Combine( Femtoseconds left, Rational femtoseconds ) => new Femtoseconds( value: left.Value + femtoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Femtoseconds left, Femtoseconds right ) => left.Value == right.Value;

        public static implicit operator Attoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToAttoseconds();

        public static implicit operator Picoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToPicoseconds();

        public static implicit operator SpanOfTime( Femtoseconds femtoseconds ) => new SpanOfTime( femtoseconds: femtoseconds );

        public static Femtoseconds operator -( Femtoseconds femtoseconds ) => new Femtoseconds( value: femtoseconds.Value * -1 );

        public static Femtoseconds operator -( Femtoseconds left, Femtoseconds right ) => Combine( left: left, right: -right );

        public static Femtoseconds operator -( Femtoseconds left, Decimal femtoseconds ) => Combine( left: left, femtoseconds: ( Rational ) ( -femtoseconds ) );

        public static Boolean operator !=( Femtoseconds left, Femtoseconds right ) => !Equals( left: left, right: right );

        public static Femtoseconds operator +( Femtoseconds left, Femtoseconds right ) => Combine( left: left, right: right );

        public static Femtoseconds operator +( Femtoseconds left, Decimal femtoseconds ) => Combine( left: left, femtoseconds: ( Rational ) femtoseconds );

        public static Boolean operator <( Femtoseconds left, Femtoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Femtoseconds left, Femtoseconds right ) => Equals( left: left, right: right );

        public static Boolean operator >( Femtoseconds left, Femtoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Femtoseconds other ) => this.Value.CompareTo( other: other.Value );

        public Boolean Equals( Femtoseconds other ) => Equals( left: this, right: other );

        public override Boolean Equals( [CanBeNull] Object? obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is Femtoseconds femtoseconds && this.Equals( other: femtoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /// <summary>Convert to a smaller unit.</summary>
        /// <returns></returns>
        public Attoseconds ToAttoseconds() => new Attoseconds( value: this.Value * Attoseconds.InOneFemtosecond );

        /// <summary>Convert to a larger unit.</summary>
        /// <returns></returns>
        public Picoseconds ToPicoseconds() => new Picoseconds( value: this.Value / InOnePicosecond );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: this.Value * ( Rational ) PlanckTimes.InOneFemtosecond );

        public Seconds ToSeconds() => throw new NotImplementedException();

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "fs" )}";
            }

            var dec = ( Decimal ) this.Value;

            return $"{dec} {dec.PluralOf( singular: "fs" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

    }

}