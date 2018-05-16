// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Days.cs",
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
// "Librainian/Librainian/Days.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Numerics;
    using Parsing;

    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public struct Days : IComparable<Days>, IQuantityOfTime {

        /// <summary>
        ///     365
        /// </summary>
        public const UInt16 InOneCommonYear = 365;

        /// <summary>
        ///     7
        /// </summary>
        public const UInt16 InOneWeek = 7;

        /// <summary>
        ///     One <see cref="Days" /> .
        /// </summary>
        public static readonly Days One = new Days( 1 );

        /// <summary>
        ///     Seven <see cref="Days" /> .
        /// </summary>
        public static readonly Days Seven = new Days( 7 );

        /// <summary>
        ///     Ten <see cref="Days" /> .
        /// </summary>
        public static readonly Days Ten = new Days( 10 );

        /// <summary>
        /// </summary>
        public static readonly Days Thousand = new Days( 1000 );

        /// <summary>
        ///     Zero <see cref="Days" />
        /// </summary>
        public static readonly Days Zero = new Days( 0 );

        public Days( Decimal value ) => this.Value = value;

        public Days( BigRational value ) => this.Value = value;

        public Days( Int64 value ) => this.Value = value;

        public Days( BigInteger value ) => this.Value = value;

        [JsonProperty]
        public BigRational Value { get; }

        public static Days Combine( Days left, Days right ) => Combine( left, right.Value );

        //public const Byte InOneMonth = 31;
        public static Days Combine( Days left, BigRational days ) => new Days( left.Value + days );

        public static Days Combine( Days left, BigInteger days ) => new Days( ( BigInteger )left.Value + days );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Days left, Days right ) => left.Value == right.Value;

        /// <summary>
        ///     Implicitly convert the number of <paramref name="days" /> to <see cref="Hours" />.
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static implicit operator Hours( Days days ) => days.ToHours();

        public static implicit operator Span( Days days ) => new Span( days: days.Value );

        public static implicit operator TimeSpan( Days days ) => TimeSpan.FromDays( ( Double )days.Value );

        /// <summary>
        ///     Implicitly convert the number of <paramref name="days" /> to <see cref="Weeks" />.
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static implicit operator Weeks( Days days ) => days.ToWeeks();

        public static Days operator -( Days days ) => new Days( days.Value * -1 );

        public static Days operator -( Days left, Days right ) => Combine( left: left, right: -right );

        public static Days operator -( Days left, Decimal days ) => Combine( left, -days );

        public static Boolean operator !=( Days left, Days right ) => !Equals( left, right );

        public static Days operator +( Days left, Days right ) => Combine( left, right );

        public static Days operator +( Days left, Decimal days ) => Combine( left, days );

        public static Days operator +( Days left, BigInteger days ) => Combine( left, days );

        public static Boolean operator <( Days left, Days right ) => left.Value < right.Value;

        public static Boolean operator <( Days left, Hours right ) => left < ( Days )right;

        public static Boolean operator ==( Days left, Days right ) => Equals( left, right );

        public static Boolean operator >( Days left, Hours right ) => left > ( Days )right;

        public static Boolean operator >( Days left, Days right ) => left.Value > right.Value;

        public Int32 CompareTo( Days other ) => this.Value.CompareTo( other.Value );

        public Boolean Equals( Days other ) => Equals( this.Value, other.Value );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) { return false; }

            return obj is Days days && this.Equals( days );
        }

        [Pure]
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [Pure]
        public Hours ToHours() => new Hours( this.Value * Hours.InOneDay );

        [Pure]
        public PlanckTimes ToPlanckTimes() => new PlanckTimes( PlanckTimes.InOneDay * this.Value );

        [Pure]
        public override String ToString() {
            if ( this.Value > Constants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.GetWholePart();

                return $"{whole} {whole.PluralOf( "day" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "day" )}";
        }

        [Pure]
        public Weeks ToWeeks() => new Weeks( this.Value / InOneWeek );
    }
}