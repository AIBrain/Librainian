// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Attoseconds.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Attoseconds.cs" was last formatted by Protiguous on 2020/03/16 at 2:57 PM.

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

    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Attoseconds : IQuantityOfTime {

        /// <summary>1000</summary>
        /// <see cref="Femtoseconds" />
        public const UInt16 InOneFemtosecond = 1000;

        /// <summary>Ten <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Fifteen { get; } = new Attoseconds( value: 15 );

        /// <summary>Five <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Five { get; } = new Attoseconds( value: 5 );

        /// <summary>Five Hundred <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds FiveHundred { get; } = new Attoseconds( value: 500 );

        /// <summary>111. 1 Hertz <see cref="Attoseconds" />.</summary>
        public static Attoseconds Hertz111 { get; } = new Attoseconds( value: 9 );

        /// <summary>One <see cref="Attoseconds" />.</summary>
        /// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
        public static Attoseconds One { get; } = new Attoseconds( value: 1 );

        /// <summary><see cref="OneHundred" /><see cref="Attoseconds" />.</summary>
        /// <remarks>fastest ever view of molecular motion</remarks>
        public static Attoseconds OneHundred { get; } = new Attoseconds( value: 100 );

        /// <summary>One Thousand Nine <see cref="Attoseconds" /> (Prime).</summary>
        public static Attoseconds OneThousandNine { get; } = new Attoseconds( value: 1009 );

        /// <summary>Sixteen <see cref="Attoseconds" />.</summary>
        public static Attoseconds Sixteen { get; } = new Attoseconds( value: 16 );

        /// <summary><see cref="SixtySeven" /><see cref="Attoseconds" />.</summary>
        /// <remarks>the shortest pulses of laser light yet created</remarks>
        public static Attoseconds SixtySeven { get; } = new Attoseconds( value: 67 );

        /// <summary>Ten <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Ten { get; } = new Attoseconds( value: 10 );

        /// <summary>Three <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Three { get; } = new Attoseconds( value: 3 );

        /// <summary>Three Three Three <see cref="Attoseconds" />.</summary>
        public static Attoseconds ThreeHundredThirtyThree { get; } = new Attoseconds( value: 333 );

        /// <summary><see cref="ThreeHundredTwenty" /><see cref="Attoseconds" />.</summary>
        /// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
        public static Attoseconds ThreeHundredTwenty { get; } = new Attoseconds( value: 320 );

        /// <summary><see cref="Twelve" /><see cref="Attoseconds" />.</summary>
        /// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
        public static Attoseconds Twelve { get; } = new Attoseconds( value: 12 );

        /// <summary><see cref="TwentyFour" /><see cref="Attoseconds" />.</summary>
        /// <remarks>the atomic unit of time</remarks>
        public static Attoseconds TwentyFour { get; } = new Attoseconds( value: 24 );

        /// <summary>Two <see cref="Attoseconds" /> s.</summary>
        public static Attoseconds Two { get; } = new Attoseconds( value: 2 );

        /// <summary><see cref="TwoHundred" /><see cref="Attoseconds" />.</summary>
        /// <remarks>(approximately) – half-life of beryllium-8, maximum time available for the triple-alpha process for the synthesis of carbon and heavier elements in stars</remarks>
        public static Attoseconds TwoHundred { get; } = new Attoseconds( value: 200 );

        /// <summary>Two Hundred Eleven <see cref="Attoseconds" /> (Prime).</summary>
        public static Attoseconds TwoHundredEleven { get; } = new Attoseconds( value: 211 );

        /// <summary>Two Thousand Three <see cref="Attoseconds" /> (Prime).</summary>
        public static Attoseconds TwoThousandThree { get; } = new Attoseconds( value: 2003 );

        /// <summary>Zero <see cref="Attoseconds" />.</summary>
        public static Attoseconds Zero { get; } = new Attoseconds( value: 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Attoseconds( Decimal value ) => this.Value = ( Rational )value;

        public Attoseconds( Rational value ) => this.Value = value;

        public Attoseconds( Int64 value ) => this.Value = value;

        public Attoseconds( BigInteger value ) => this.Value = value;

        public static Attoseconds Combine( Attoseconds left, Attoseconds right ) => new Attoseconds( value: left.Value + right.Value );

        public static Attoseconds Combine( Attoseconds left, Decimal attoseconds ) => new Attoseconds( value: left.Value + ( Rational )attoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Attoseconds left, Attoseconds right ) => left.Value == right.Value;

        public static implicit operator Femtoseconds( Attoseconds attoseconds ) => attoseconds.ToFemtoseconds();

        [NotNull]
        public static implicit operator SpanOfTime( Attoseconds attoseconds ) => new SpanOfTime( planckTimes: attoseconds.ToPlanckTimes().Value );

        public static implicit operator Zeptoseconds( Attoseconds attoseconds ) => attoseconds.ToZeptoseconds();

        public static Attoseconds operator -( Attoseconds left, Decimal attoseconds ) => Combine( left: left, attoseconds: -attoseconds );

        public static Boolean operator !=( Attoseconds left, Attoseconds right ) => !Equals( left: left, right: right );

        public static Attoseconds operator +( Attoseconds left, Attoseconds right ) => Combine( left: left, right: right );

        public static Attoseconds operator +( Attoseconds left, Decimal attoseconds ) => Combine( left: left, attoseconds: attoseconds );

        public static Boolean operator <( Attoseconds left, Attoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Attoseconds left, Attoseconds right ) => Equals( left: left, right: right );

        public static Boolean operator >( Attoseconds left, Attoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Attoseconds other ) => this.Value.CompareTo( other: other.Value );

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the
        /// same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance. </param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes
        /// <paramref name="other" /> in the sort order.  Zero This instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance
        /// follows <paramref name="other" /> in the sort order.
        /// </returns>
        public Int32 CompareTo( [NotNull] IQuantityOfTime other ) {
            if ( other is null ) {
                throw new ArgumentNullException( paramName: nameof( other ) );
            }

            return this.ToPlanckTimes().Value.CompareTo( other: other.ToPlanckTimes().Value );
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the
        /// same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes
        /// <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows
        /// <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj" /> is not the same type as this instance.</exception>
        public Int32 CompareTo( [CanBeNull] Object obj ) {
            if ( obj is null ) {
                return 1;
            }

            return obj is Attoseconds other ? this.CompareTo( other: other ) : throw new ArgumentException( message: $"Object must be of type {nameof( Attoseconds )}" );
        }

        public override Boolean Equals( [CanBeNull] Object obj ) => obj is Attoseconds attoseconds && Equals( left: this, right: attoseconds );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /// <summary>Convert to a larger unit.</summary>
        /// <returns></returns>
        public Femtoseconds ToFemtoseconds() => new Femtoseconds( value: this.Value / InOneFemtosecond );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: ( Rational )PlanckTimes.InOneAttosecond * this.Value );

        public Seconds ToSeconds() => this.ToZeptoseconds().ToSeconds();

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "as" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( singular: "as" )}";
        }

        public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds( value: ( Double )this.ToSeconds().Value );

        /// <summary>Convert to a smaller unit.</summary>
        /// <returns></returns>
        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( value: this.Value * Zeptoseconds.InOneAttosecond );
    }
}