// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Seconds.cs",
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
// "Librainian/Librainian/Seconds.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
    ///     <para>
    ///         Under the International System of Units, since 1967 the second has been defined as the duration of 9192631770
    ///         periods of the radiation corresponding to the transition between the two hyperfine levels of the ground
    ///         state of the caesium 133 atom.
    ///     </para>
    /// </summary>
    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct Seconds : IComparable<Seconds>, IQuantityOfTime {

        /// <summary>
        ///     31536000
        /// </summary>
        public const UInt32 InOneCommonYear = 31536000;

        /// <summary>
        ///     86400
        /// </summary>
        public const UInt32 InOneDay = 86400;

        /// <summary>
        ///     3600
        /// </summary>
        public const UInt16 InOneHour = 3600;

        /// <summary>
        ///     60
        /// </summary>
        public const Byte InOneMinute = 60;

        /// <summary>
        ///     2635200 (30.5 days)
        /// </summary>
        public const UInt32 InOneMonth = 2635200;

        /// <summary>
        ///     604800
        /// </summary>
        public const UInt32 InOneWeek = 604800;

        /// <summary>
        ///     <see cref="Five" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Five = new Seconds( 5 );

        /// <summary>
        ///     <see cref="One" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds One = new Seconds( 1 );

        /// <summary>
        ///     <see cref="OnePointFive" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds OnePointFive = new Seconds( 1.5 );

        /// <summary>
        ///     <see cref="Seven" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Seven = new Seconds( 7 );

        /// <summary>
        ///     <see cref="Ten" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Ten = new Seconds( 10 );

        /// <summary>
        ///     <see cref="Thirteen" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Thirteen = new Seconds( 13 );

        /// <summary>
        ///     <see cref="Thirty" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Thirty = new Seconds( 30 );

        /// <summary>
        ///     <see cref="Three" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Three = new Seconds( 3 );

        /// <summary>
        ///     <see cref="Twenty" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Twenty = new Seconds( 20 );

        /// <summary>
        ///     <see cref="Two" /><see cref="Seconds" />.
        /// </summary>
        public static readonly Seconds Two = new Seconds( 2 );

        /// <summary>
        /// </summary>
        public static readonly Seconds Zero = new Seconds( 0 );

        [JsonProperty]
        public BigRational Value { get; }

        public Seconds( Decimal value ) => this.Value = value;

        public Seconds( BigRational value ) => this.Value = value;

        public Seconds( Int64 value ) => this.Value = value;

        public Seconds( BigInteger value ) => this.Value = value;

        public static Seconds Combine( Seconds left, Seconds right ) => Combine( left, right.Value );

        public static Seconds Combine( Seconds left, BigRational seconds ) => new Seconds( left.Value + seconds );

        public static Seconds Combine( Seconds left, BigInteger seconds ) => new Seconds( ( BigInteger )left.Value + seconds );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Seconds left, Seconds right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="seconds" /> to <see cref="Milliseconds" />.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static implicit operator Milliseconds( Seconds seconds ) => seconds.ToMilliseconds();

        /// <summary>
        ///     Implicitly convert the number of <paramref name="seconds" /> to <see cref="Minutes" />.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static implicit operator Minutes( Seconds seconds ) => seconds.ToMinutes();

        public static implicit operator Span( Seconds seconds ) => new Span( seconds: seconds );

        public static implicit operator TimeSpan( Seconds seconds ) => TimeSpan.FromSeconds( ( Double )seconds.Value );

        public static Seconds operator -( Seconds seconds ) => new Seconds( seconds.Value * -1 );

        public static Seconds operator -( Seconds left, Seconds right ) => Combine( left: left, right: -right );

        public static Seconds operator -( Seconds left, Decimal seconds ) => Combine( left, -seconds );

        public static Boolean operator !=( Seconds left, Seconds right ) => !Equals( left, right );

        public static Seconds operator +( Seconds left, Seconds right ) => Combine( left, right );

        public static Seconds operator +( Seconds left, Decimal seconds ) => Combine( left, seconds );

        public static Seconds operator +( Seconds left, BigInteger seconds ) => Combine( left, seconds );

        public static Boolean operator <( Seconds left, Seconds right ) => left.Value < right.Value;

        public static Boolean operator <( Seconds left, Milliseconds right ) => left < ( Seconds )right;

        public static Boolean operator <( Seconds left, Minutes right ) => ( Minutes )left < right;

        public static Boolean operator ==( Seconds left, Seconds right ) => Equals( left, right );

        public static Boolean operator >( Seconds left, Minutes right ) => ( Minutes )left > right;

        public static Boolean operator >( Seconds left, Seconds right ) => left.Value > right.Value;

        public static Boolean operator >( Seconds left, Milliseconds right ) => left > ( Seconds )right;

        public Int32 CompareTo( Seconds other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Seconds other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Seconds seconds && this.Equals( seconds );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        /*
                [Pure]
                public Minutes ToMonths() => new Minutes(this.Value / InOneMonth );
        */

        [Pure]
        public Milliseconds ToMilliseconds() => new Milliseconds( this.Value * Milliseconds.InOneSecond );

        [Pure]
        public Minutes ToMinutes() => new Minutes( this.Value / InOneMinute );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneSecond * this.Value );

        [Pure]
        public Seconds ToSeconds() => this;

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "second" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "second" )}";
        }

        [Pure]
        public Weeks ToWeeks() => new Weeks( this.Value / InOneWeek );

        [Pure]
        public Years ToYears() => new Years( this.Value / InOneCommonYear );
    }
}