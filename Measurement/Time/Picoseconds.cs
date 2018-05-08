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
// "Librainian/Picoseconds.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

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
    public struct Picoseconds : IComparable<Picoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneNanosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Picoseconds" /> s.
        /// </summary>
        public static readonly Picoseconds Fifteen = new Picoseconds( 15 );

        /// <summary>
        ///     Five <see cref="Picoseconds" /> s.
        /// </summary>
        public static readonly Picoseconds Five = new Picoseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Picoseconds" /> s.
        /// </summary>
        public static readonly Picoseconds FiveHundred = new Picoseconds( 500 );

        /// <summary>
        ///     One <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds One = new Picoseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Picoseconds" /> (Prime).
        /// </summary>
        public static readonly Picoseconds OneThousandNine = new Picoseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds Sixteen = new Picoseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Picoseconds" /> s.
        /// </summary>
        public static readonly Picoseconds Ten = new Picoseconds( 10 );

        /// <summary>
        ///     Three <see cref="Picoseconds" /> s.
        /// </summary>
        public static readonly Picoseconds Three = new Picoseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds ThreeHundredThirtyThree = new Picoseconds( 333 );

        /// <summary>
        ///     Two <see cref="Picoseconds" /> s.
        /// </summary>
        public static readonly Picoseconds Two = new Picoseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds TwoHundred = new Picoseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Picoseconds" /> (Prime).
        /// </summary>
        public static readonly Picoseconds TwoHundredEleven = new Picoseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Picoseconds" /> (Prime).
        /// </summary>
        public static readonly Picoseconds TwoThousandThree = new Picoseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Picoseconds" />.
        /// </summary>
        public static readonly Picoseconds Zero = new Picoseconds( 0 );

        public Picoseconds( Decimal value ) => this.Value = value;

	    public Picoseconds( BigRational value ) => this.Value = value;

	    public Picoseconds( Int64 value ) => this.Value = value;

	    public Picoseconds( BigInteger value ) => this.Value = value;

	    [JsonProperty]
        public BigRational Value {
            get;
        }

        public static Picoseconds Combine( Picoseconds left, Picoseconds right ) => Combine( left, right.Value );

        public static Picoseconds Combine( Picoseconds left, BigRational picoseconds ) => new Picoseconds( left.Value + picoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Picoseconds left, Picoseconds right ) => left.Value == right.Value;

        public static implicit operator Femtoseconds( Picoseconds picoseconds ) => picoseconds.ToFemtoseconds();

        public static implicit operator Nanoseconds( Picoseconds picoseconds ) => picoseconds.ToNanoseconds();

        public static implicit operator Span( Picoseconds picoseconds ) => new Span( picoseconds: picoseconds );

        public static Picoseconds operator -( Picoseconds nanoseconds ) => new Picoseconds( nanoseconds.Value * -1 );

        public static Picoseconds operator -( Picoseconds left, Picoseconds right ) => Combine( left, -right );

        public static Picoseconds operator -( Picoseconds left, Decimal nanoseconds ) => Combine( left, -nanoseconds );

        public static Boolean operator !=( Picoseconds left, Picoseconds right ) => !Equals( left, right );

        public static Picoseconds operator +( Picoseconds left, Picoseconds right ) => Combine( left, right );

        public static Picoseconds operator +( Picoseconds left, Decimal nanoseconds ) => Combine( left, nanoseconds );

        public static Boolean operator <( Picoseconds left, Picoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Picoseconds left, Picoseconds right ) => Equals( left, right );

        public static Boolean operator >( Picoseconds left, Picoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Picoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Picoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }
            return obj is Picoseconds picoseconds && this.Equals( picoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Femtoseconds ToFemtoseconds() => new Femtoseconds( this.Value * Femtoseconds.InOnePicosecond );

        public Nanoseconds ToNanoseconds() => new Nanoseconds( this.Value / InOneNanosecond );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOnePicosecond * this.Value );

        [Pure]
        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();
                return $"{whole} {whole.PluralOf( "ps" )}";
            }
            var dec = ( Decimal )this.Value;
            return $"{dec} {dec.PluralOf( "ps" )}";
        }
    }
}