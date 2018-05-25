// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Femtoseconds.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Femtoseconds.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    /// <seealso cref="http://wikipedia.org/wiki/Femtosecond" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    [Immutable]
    public struct Femtoseconds : IComparable<Femtoseconds>, IQuantityOfTime {

        /// <summary>
        ///     1000
        /// </summary>
        public const UInt16 InOnePicosecond = 1000;

        /// <summary>
        ///     Ten <see cref="Femtoseconds" /> s.
        /// </summary>
        public static readonly Femtoseconds Fifteen = new Femtoseconds( 15 );

        /// <summary>
        ///     Five <see cref="Femtoseconds" /> s.
        /// </summary>
        public static readonly Femtoseconds Five = new Femtoseconds( 5 );

        /// <summary>
        ///     Five Hundred <see cref="Femtoseconds" /> s.
        /// </summary>
        public static readonly Femtoseconds FiveHundred = new Femtoseconds( 500 );

        /// <summary>
        ///     One <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds One = new Femtoseconds( 1 );

        /// <summary>
        ///     One Thousand Nine <see cref="Femtoseconds" /> (Prime).
        /// </summary>
        public static readonly Femtoseconds OneThousandNine = new Femtoseconds( 1009 );

        /// <summary>
        ///     Sixteen <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds Sixteen = new Femtoseconds( 16 );

        /// <summary>
        ///     Ten <see cref="Femtoseconds" /> s.
        /// </summary>
        public static readonly Femtoseconds Ten = new Femtoseconds( 10 );

        /// <summary>
        ///     Three <see cref="Femtoseconds" /> s.
        /// </summary>
        public static readonly Femtoseconds Three = new Femtoseconds( 3 );

        /// <summary>
        ///     Three Three Three <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds ThreeHundredThirtyThree = new Femtoseconds( 333 );

        /// <summary>
        ///     Two <see cref="Femtoseconds" /> s.
        /// </summary>
        public static readonly Femtoseconds Two = new Femtoseconds( 2 );

        /// <summary>
        ///     Two Hundred <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds TwoHundred = new Femtoseconds( 200 );

        /// <summary>
        ///     Two Hundred Eleven <see cref="Femtoseconds" /> (Prime).
        /// </summary>
        public static readonly Femtoseconds TwoHundredEleven = new Femtoseconds( 211 );

        /// <summary>
        ///     Two Thousand Three <see cref="Femtoseconds" /> (Prime).
        /// </summary>
        public static readonly Femtoseconds TwoThousandThree = new Femtoseconds( 2003 );

        /// <summary>
        ///     Zero <see cref="Femtoseconds" />.
        /// </summary>
        public static readonly Femtoseconds Zero = new Femtoseconds( 0 );

        [JsonProperty]
        public BigRational Value { get; }

        public Femtoseconds( Decimal value ) => this.Value = value;

        public Femtoseconds( BigRational value ) => this.Value = value;

        public Femtoseconds( Int64 value ) => this.Value = value;

        public Femtoseconds( BigInteger value ) => this.Value = value;

        public static Femtoseconds Combine( Femtoseconds left, Femtoseconds right ) => Combine( left, right.Value );

        public static Femtoseconds Combine( Femtoseconds left, BigRational femtoseconds ) => new Femtoseconds( left.Value + femtoseconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Femtoseconds left, Femtoseconds right ) => left.Value == right.Value;

        public static implicit operator Attoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToAttoseconds();

        public static implicit operator Picoseconds( Femtoseconds femtoseconds ) => femtoseconds.ToPicoseconds();

        public static implicit operator Span( Femtoseconds femtoseconds ) => new Span( femtoseconds: femtoseconds );

        public static Femtoseconds operator -( Femtoseconds femtoseconds ) => new Femtoseconds( femtoseconds.Value * -1 );

        public static Femtoseconds operator -( Femtoseconds left, Femtoseconds right ) => Combine( left, -right );

        public static Femtoseconds operator -( Femtoseconds left, Decimal femtoseconds ) => Combine( left, -femtoseconds );

        public static Boolean operator !=( Femtoseconds left, Femtoseconds right ) => !Equals( left, right );

        public static Femtoseconds operator +( Femtoseconds left, Femtoseconds right ) => Combine( left, right );

        public static Femtoseconds operator +( Femtoseconds left, Decimal femtoseconds ) => Combine( left, femtoseconds );

        public static Boolean operator <( Femtoseconds left, Femtoseconds right ) => left.Value < right.Value;

        public static Boolean operator ==( Femtoseconds left, Femtoseconds right ) => Equals( left, right );

        public static Boolean operator >( Femtoseconds left, Femtoseconds right ) => left.Value > right.Value;

        public Int32 CompareTo( Femtoseconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Femtoseconds other ) => Equals( this, other );

        public override Boolean Equals( [CanBeNull] Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Femtoseconds femtoseconds && this.Equals( femtoseconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /// <summary>
        ///     Convert to a smaller unit.
        /// </summary>
        /// <returns></returns>
        public Attoseconds ToAttoseconds() => new Attoseconds( this.Value * Attoseconds.InOneFemtosecond );

        /// <summary>
        ///     Convert to a larger unit.
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Picoseconds ToPicoseconds() => new Picoseconds( this.Value / InOnePicosecond );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( this.Value * PlanckTimes.InOneFemtosecond );

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "fs" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "fs" )}";
        }
    }
}