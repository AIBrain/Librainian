// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Days.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Days.cs" was last formatted by Protiguous on 2019/08/08 at 9:00 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    // return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );

    [JsonObject]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class Days : IComparable<Days>, IQuantityOfTime, IEquatable<Days> {

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

        [JsonProperty]
        public Rational Value { get; }

        public Days( Decimal value ) => this.Value = ( Rational )value;

        public Days( Rational value ) => this.Value = value;

        public Days( Int64 value ) => this.Value = value;

        public Days( BigInteger value ) => this.Value = value;

        [CanBeNull]
        public static Days Combine( [CanBeNull] Days left, Days right ) => Combine( left, right.Value );

        //public const Byte InOneMonth = 31;
        [NotNull]
        public static Days Combine( Days left, Rational days ) => new Days( left.Value + days );

        [NotNull]
        public static Days Combine( Days left, BigInteger days ) => new Days( left.Value + days );

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
        [CanBeNull]
        public static implicit operator Hours( Days days ) => days.ToHours();

        [NotNull]
        public static implicit operator SpanOfTime( [CanBeNull] Days days ) => new SpanOfTime( days: days );

        public static implicit operator TimeSpan( Days days ) => TimeSpan.FromDays( ( Double )days.Value );

        /// <summary>
        ///     Implicitly convert the number of <paramref name="days" /> to <see cref="Weeks" />.
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        [CanBeNull]
        public static implicit operator Weeks( Days days ) => days.ToWeeks();

        [NotNull]
        public static Days operator -( Days days ) => new Days( days.Value * -1 );

        [CanBeNull]
        public static Days operator -( [CanBeNull] Days left, [CanBeNull] Days right ) => Combine( left: left, right: -right );

        [NotNull]
        public static Days operator -( [CanBeNull] Days left, Decimal days ) => Combine( left, ( Rational )( -days ) );

        public static Boolean operator !=( [CanBeNull] Days left, [CanBeNull] Days right ) => !Equals( left, right );

        [CanBeNull]
        public static Days operator +( [CanBeNull] Days left, [CanBeNull] Days right ) => Combine( left, right );

        [NotNull]
        public static Days operator +( [CanBeNull] Days left, Decimal days ) => Combine( left, ( Rational )days );

        [NotNull]
        public static Days operator +( [CanBeNull] Days left, BigInteger days ) => Combine( left, days );

        public static Boolean operator <( Days left, Days right ) => left.Value < right.Value;

        public static Boolean operator <( [CanBeNull] Days left, [CanBeNull] Hours right ) => left < ( Days )right;

        public static Boolean operator ==( [CanBeNull] Days left, [CanBeNull] Days right ) => Equals( left, right );

        public static Boolean operator >( [CanBeNull] Days left, [CanBeNull] Hours right ) => left > ( Days )right;

        public static Boolean operator >( Days left, Days right ) => left.Value > right.Value;

        public Int32 CompareTo( Days other ) => this.Value.CompareTo( other.Value );

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates
        ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
        ///     object.
        /// </summary>
        /// <param name="other">An object to compare with this instance. </param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This
        ///     instance occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This
        ///     instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public Int32 CompareTo( [NotNull] IQuantityOfTime other ) {
            if ( other is null ) {
                throw new ArgumentNullException(  nameof( other ) );
            }

            return this.ToPlanckTimes().Value.CompareTo( other.ToPlanckTimes().Value );
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type and returns an integer that indicates
        ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
        ///     object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance. </param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has these meanings:
        ///     Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance
        ///     occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows
        ///     <paramref name="obj" /> in the sort order.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="obj" /> is not the same type as this instance.
        /// </exception>
        public Int32 CompareTo( [CanBeNull] Object obj ) {
            if ( obj is null ) {
                return 1;
            }

            if ( obj is Days other ) {
                return this.CompareTo( other );
            }

            throw new ArgumentException( $"Object must be of type {nameof( Days )}" );
        }

        public Boolean Equals( Days other ) => Equals( this.Value, other.Value );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return false;
            }

            return obj is Days days && this.Equals( days );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [NotNull]
        public Hours ToHours() => new Hours( this.Value * Hours.InOneDay );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( ( Rational )PlanckTimes.InOneDay * this.Value );

        [NotNull]
        public Seconds ToSeconds() => new Seconds( ( Rational )TimeSpan.FromDays( ( Double )this.Value ).TotalSeconds );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( "day" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( "day" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        [NotNull]
        public Weeks ToWeeks() => new Weeks( this.Value / InOneWeek );
    }
}