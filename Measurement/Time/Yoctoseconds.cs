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
// "Librainian/Yoctoseconds.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

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

    /// <summary>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Yoctosecond" />
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Yoctoseconds : IComparable<Yoctoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOneZeptosecond = 1000;

        /// <summary>
        ///     <see cref="Five" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Five = new Yoctoseconds( 5 );

        /// <summary>
        ///     <see cref="One" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds One = new Yoctoseconds( 1 );

        /// <summary>
        ///     <see cref="Seven" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Seven = new Yoctoseconds( 7 );

        /// <summary>
        ///     <see cref="Ten" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Ten = new Yoctoseconds( 10 );

        /// <summary>
        ///     <see cref="Thirteen" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Thirteen = new Yoctoseconds( 13 );

        /// <summary>
        ///     <see cref="Thirty" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Thirty = new Yoctoseconds( 30 );

        /// <summary>
        ///     <see cref="Three" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Three = new Yoctoseconds( 3 );

        /// <summary>
        ///     <see cref="Two" /><see cref="Yoctoseconds" />.
        /// </summary>
        public static readonly Yoctoseconds Two = new Yoctoseconds( 2 );

        /// <summary>
        /// </summary>
        public static readonly Yoctoseconds Zero = new Yoctoseconds( 0 );

        public Yoctoseconds( Decimal value ) => this.Value = value;

	    public Yoctoseconds( BigRational value ) => this.Value = value;

	    public Yoctoseconds( Int64 value ) => this.Value = value;

	    public Yoctoseconds( BigInteger value ) => this.Value = value;

	    [JsonProperty]
        public BigRational Value {
            get;
        }

        public static Yoctoseconds Combine( Yoctoseconds left, Yoctoseconds right ) => Combine( left, right.Value );

        public static Yoctoseconds Combine( Yoctoseconds left, BigRational yoctoseconds ) => new Yoctoseconds( left.Value + yoctoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Yoctoseconds left, Yoctoseconds right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="PlanckTimes" />.
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator PlanckTimes( Yoctoseconds yoctoseconds ) => ToPlanckTimes( yoctoseconds );

        public static implicit operator Span( Yoctoseconds yoctoseconds ) => new Span( yoctoseconds: yoctoseconds );

        /// <summary>
        ///     Implicitly convert the number of <paramref name="yoctoseconds" /> to <see cref="Zeptoseconds" />.
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <returns></returns>
        public static implicit operator Zeptoseconds( Yoctoseconds yoctoseconds ) => yoctoseconds.ToZeptoseconds();

        public static Yoctoseconds operator -( Yoctoseconds yoctoseconds ) => new Yoctoseconds( yoctoseconds.Value * -1 );

        public static Yoctoseconds operator -( Yoctoseconds left, Yoctoseconds right ) => Combine( left: left, right: -right );

        public static Yoctoseconds operator -( Yoctoseconds left, Decimal seconds ) => Combine( left, -seconds );

        public static Boolean operator !=( Yoctoseconds left, Yoctoseconds right ) => !Equals( left, right );

        public static Yoctoseconds operator +( Yoctoseconds left, Yoctoseconds right ) => Combine( left, right );

        public static Yoctoseconds operator +( Yoctoseconds left, Decimal yoctoseconds ) => Combine( left, yoctoseconds );

        public static Boolean operator <( Yoctoseconds left, Yoctoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Yoctoseconds left, Yoctoseconds right ) => Equals( left, right );

        public static Boolean operator >( Yoctoseconds left, Yoctoseconds right ) => left.Value > right.Value;

        public static PlanckTimes ToPlanckTimes( Yoctoseconds yoctoseconds ) => new PlanckTimes( PlanckTimes.InOneYoctosecond * yoctoseconds.Value );

        public Int32 CompareTo( Yoctoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Yoctoseconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }
            return obj is Yoctoseconds yoctoseconds && this.Equals( yoctoseconds );
        }

        [Pure]
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneYoctosecond * this.Value );

        //TODO
        //[Pure]public Seconds ToSeconds() => new Seconds( this.Value / Seconds. );

        [Pure]
        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();
                return $"{whole} {whole.PluralOf( "ys" )}";
            }
            var dec = ( Decimal )this.Value;
            return $"{dec} {dec.PluralOf( "ys" )}";
        }

        [Pure]
        public Zeptoseconds ToZeptoseconds() => new Zeptoseconds( this.Value / InOneZeptosecond );
    }
}