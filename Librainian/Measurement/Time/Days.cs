// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Days.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Days.cs" was last formatted by Protiguous on 2020/03/16 at 2:57 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    [JsonObject]
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    public struct Days : IComparable<Days>, IQuantityOfTime, IEquatable<Days> {

        /// <summary>365</summary>
        public const UInt16 InOneCommonYear = 365;

        /// <summary>7</summary>
        public const UInt16 InOneWeek = 7;

        /// <summary>One <see cref="Days" /> .</summary>
        public static readonly Days One = new Days( value: 1 );

        /// <summary>Seven <see cref="Days" /> .</summary>
        public static readonly Days Seven = new Days( value: 7 );

        /// <summary>Ten <see cref="Days" /> .</summary>
        public static readonly Days Ten = new Days( value: 10 );

        /// <summary></summary>
        public static readonly Days Thousand = new Days( value: 1000 );

        /// <summary>Zero <see cref="Days" /></summary>
        public static readonly Days Zero = new Days( value: 0 );

        [JsonProperty]
        public Rational Value { get; }

        public Days( Decimal value ) => this.Value = ( Rational )value;

        public Days( Rational value ) => this.Value = value;

        public Days( Int64 value ) => this.Value = value;

        public Days( BigInteger value ) => this.Value = value;

        public static Days Combine( Days left, Days right ) => Combine( left: left, days: right.Value );

        public static Days Combine( Days left, Rational days ) => new Days( value: left.Value + days );

        public static Days Combine( Days left, BigInteger days ) => new Days( value: left.Value + days );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Days left, Days right ) => left.Value == right.Value;

        /// <summary>Implicitly convert the number of <paramref name="days" /> to <see cref="Hours" />.</summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static implicit operator Hours( Days days ) => days.ToHours();

        [NotNull]
        public static implicit operator SpanOfTime( Days days ) => new SpanOfTime( timeSpan: days );

        public static implicit operator TimeSpan( Days days ) => TimeSpan.FromDays( value: ( Double )days.Value );

        /// <summary>Implicitly convert the number of <paramref name="days" /> to <see cref="Weeks" />.</summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public static implicit operator Weeks( Days days ) => days.ToWeeks();

        public static Days operator -( Days days ) => new Days( value: days.Value * -1 );

        public static Days operator -( Days left, Days right ) => Combine( left: left, right: -right );

        public static Days operator -( Days left, Decimal days ) => Combine( left: left, days: ( Rational )( -days ) );

        public static Boolean operator !=( Days left, Days right ) => !Equals( left: left, right: right );

        public static Days operator +( Days left, Days right ) => Combine( left: left, right: right );

        public static Days operator +( Days left, Decimal days ) => Combine( left: left, days: ( Rational )days );

        public static Days operator +( Days left, BigInteger days ) => Combine( left: left, days: days );

        public static Boolean operator <( Days left, Days right ) => left.Value < right.Value;

        public static Boolean operator <( Days left, Hours right ) => left < ( Days )right;

        public static Boolean operator ==( Days left, Days right ) => Equals( left: left, right: right );

        public static Boolean operator >( Days left, Hours right ) => left > ( Days )right;

        public static Boolean operator >( Days left, Days right ) => left.Value > right.Value;

        public Int32 CompareTo( Days other ) => this.Value.CompareTo( other: other.Value );

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
            switch ( obj ) {
                case null: return 1;
                case Days other: return this.CompareTo( other: other );
                default: throw new ArgumentException( message: $"Object must be of type {nameof( Days )}" );
            }
        }

        public Boolean Equals( Days other ) => Equals( left: this, right: other );

        public override Boolean Equals( Object obj ) => Equals( left: this, right: obj is Days days ? days : default );

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public Hours ToHours() => new Hours( value: this.Value * Hours.InOneDay );

        public PlanckTimes ToPlanckTimes() => new PlanckTimes( value: ( Rational )PlanckTimes.InOneDay * this.Value );

        public Seconds ToSeconds() => new Seconds( value: ( Rational )TimeSpan.FromDays( value: ( Double )this.Value ).TotalSeconds );

        public override String ToString() {
            if ( this.Value > MathConstants.DecimalMaxValueAsBigRational ) {
                var whole = this.Value.WholePart;

                return $"{whole} {whole.PluralOf( singular: "day" )}";
            }

            var dec = ( Decimal )this.Value;

            return $"{dec} {dec.PluralOf( singular: "day" )}";
        }

        public TimeSpan ToTimeSpan() => this.ToSeconds();

        public Weeks ToWeeks() => new Weeks( weeks: this.Value / InOneWeek );
    }
}