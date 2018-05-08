// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Attoseconds.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Numerics;
    using Parsing;

    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Attoseconds : IComparable<Attoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        /// <seealso cref="Femtoseconds" />
        public const UInt16 InOneFemtosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Attoseconds" /> s.
        /// </summary>
        public static readonly Attoseconds Fifteen = new Attoseconds( 15 );

        /// <summary>
        ///     Five <see cref="Attoseconds" /> s.
        /// </summary>
        public static readonly Attoseconds Five = new Attoseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Attoseconds" /> s.
        /// </summary>
        public static readonly Attoseconds FiveHundred = new Attoseconds( 500 );

        /// <summary>
        ///     111. 1 Hertz <see cref="Attoseconds" />.
        /// </summary>
        public static readonly Attoseconds Hertz111 = new Attoseconds( 9 );

        /// <summary>
        ///     One <see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>the time it takes for light to travel the length of three hydrogen atoms</remarks>
        public static readonly Attoseconds One = new Attoseconds( 1 );

        /// <summary>
        ///     <see cref="OneHundred" /><see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>fastest ever view of molecular motion</remarks>
        public static readonly Attoseconds OneHundred = new Attoseconds( 100 );

        /// <summary>
        ///     One Thousand Nine <see cref="Attoseconds" /> (Prime).
        /// </summary>
        public static readonly Attoseconds OneThousandNine = new Attoseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Attoseconds" />.
        /// </summary>
        public static readonly Attoseconds Sixteen = new Attoseconds( 16 );

        /// <summary>
        ///     <see cref="SixtySeven" /><see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>the shortest pulses of laser light yet created</remarks>
        public static readonly Attoseconds SixtySeven = new Attoseconds( 67 );

        /// <summary>
        ///     Ten <see cref="Attoseconds" /> s.
        /// </summary>
        public static readonly Attoseconds Ten = new Attoseconds( 10 );

        /// <summary>
        ///     Three <see cref="Attoseconds" /> s.
        /// </summary>
        public static readonly Attoseconds Three = new Attoseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Attoseconds" />.
        /// </summary>
        public static readonly Attoseconds ThreeHundredThirtyThree = new Attoseconds( 333 );

        /// <summary>
        ///     <see cref="ThreeHundredTwenty" /><see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>estimated time it takes electrons to transfer between atoms</remarks>
        public static readonly Attoseconds ThreeHundredTwenty = new Attoseconds( 320 );

        /// <summary>
        ///     <see cref="Twelve" /><see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>record for shortest time interval measured as of 12 May 2010</remarks>
        public static readonly Attoseconds Twelve = new Attoseconds( 12 );

        /// <summary>
        ///     <see cref="TwentyFour" /><see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>the atomic unit of time</remarks>
        public static readonly Attoseconds TwentyFour = new Attoseconds( 24 );

        /// <summary>
        ///     Two <see cref="Attoseconds" /> s.
        /// </summary>
        public static readonly Attoseconds Two = new Attoseconds( 2 );

        /// <summary>
        ///     <see cref="TwoHundred" /><see cref="Attoseconds" />.
        /// </summary>
        /// <remarks>
        ///     (approximately) – half-life of beryllium-8, maximum time available for the triple-alpha
        ///     process for the synthesis of carbon and heavier elements in stars
        /// </remarks>
        public static readonly Attoseconds TwoHundred = new Attoseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Attoseconds" /> (Prime).
        /// </summary>
        public static readonly Attoseconds TwoHundredEleven = new Attoseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Attoseconds" /> (Prime).
        /// </summary>
        public static readonly Attoseconds TwoThousandThree = new Attoseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Attoseconds" />.
        /// </summary>
        public static readonly Attoseconds Zero = new Attoseconds( 0 );

        public Attoseconds( Decimal value ) => this.Value = value;

	    public Attoseconds( BigRational value ) => this.Value = value;

	    public Attoseconds( Int64 value ) => this.Value = value;

	    public Attoseconds( BigInteger value ) => this.Value = value;

	    [JsonProperty]
        public BigRational Value {
            get;
        }

        public static Attoseconds Combine( Attoseconds left, Attoseconds right ) => new Attoseconds( left.Value + right.Value );

        public static Attoseconds Combine( Attoseconds left, Decimal attoseconds ) => new Attoseconds( left.Value + attoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Attoseconds left, Attoseconds right ) => left.Value == right.Value;

        public static implicit operator Femtoseconds( Attoseconds attoseconds ) => attoseconds.ToFemtoseconds();

        public static implicit operator Span( Attoseconds attoseconds ) {
            var plancks = attoseconds.ToPlanckTimes();
            return new Span( plancks );
        }

        public static implicit operator Zeptoseconds( Attoseconds attoseconds ) => attoseconds.ToZeptoseconds();

        public static Attoseconds operator -( Attoseconds left, Decimal attoseconds ) => Combine( left, -attoseconds );

        public static Boolean operator !=( Attoseconds left, Attoseconds right ) => !Equals( left, right );

        public static Attoseconds operator +( Attoseconds left, Attoseconds right ) => Combine( left, right );

        public static Attoseconds operator +( Attoseconds left, Decimal attoseconds ) => Combine( left, attoseconds );

        public static Boolean operator <( Attoseconds left, Attoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Attoseconds left, Attoseconds right ) => Equals( left, right );

        public static Boolean operator >( Attoseconds left, Attoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Attoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Attoseconds other ) => Equals( this, other );

        public override Boolean Equals( [CanBeNull] Object obj ) {
            if ( obj is null ) {
                return false;
            }
            return obj is Attoseconds attoseconds && this.Equals( attoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        ///     Convert to a larger unit.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value / InOneFemtosecond );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneAttosecond * this.Value );

        [Pure]
        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();
                return $"{whole} {whole.PluralOf( "as" )}";
            }
            var dec = ( Decimal )this.Value;
            return $"{dec} {dec.PluralOf( "as" )}";
        }

        /// <summary>
        ///     Convert to a smaller unit.
        /// </summary>
        /// <returns></returns>
        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value * Zeptoseconds.InOneAttosecond );
    }
}