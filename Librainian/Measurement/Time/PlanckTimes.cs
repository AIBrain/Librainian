// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "PlanckTimes.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "PlanckTimes.cs" was last formatted by Protiguous on 2020/03/18 at 10:25 AM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Rationals;

    /// <summary>
    ///     <para>In physics, the Planck time (tP) is the unit of time in the system of natural units known as Planck units.</para>
    ///     <para>It is the time required for light to travel, in a vacuum, a distance of 1 Planck length.</para>
    ///     <para>The Planck time is defined as:</para>
    ///     <para>t_P \equiv \sqrt{\frac{\hbar G}{c^5}} ≈ 5.39106(32) × 10−44 s</para>
    ///     <para>
    ///     where: \hbar = h / 2 \pi is the reduced Planck constant (sometimes h is used instead of \hbar in the definition[1]) G = gravitational constant c = speed of light in a
    ///     vacuum s is the SI unit of time, the second. The two digits between parentheses denote the standard error of the estimated value.
    ///     </para>
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Planck_time" />
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public struct PlanckTimes : IQuantityOfTime {

        public const Double InOneAttosecond = InOneFemtosecond / Attoseconds.InOneFemtosecond;

        public const Double InOneDay = InOneSecond * Seconds.InOneDay;

        public const Double InOneFemtosecond = InOnePicosecond / Femtoseconds.InOnePicosecond;

        public const Double InOneHour = InOneSecond * Seconds.InOneHour;

        public const Double InOneMicrosecond = InOneMillisecond / Microseconds.InOneMillisecond;

        public const Double InOneMillisecond = InOneSecond / Milliseconds.InOneSecond;

        public const Double InOneMinute = InOneSecond * Seconds.InOneMinute;

        public const Double InOneMonth = InOneSecond * Seconds.InOneMonth;

        public const Double InOneNanosecond = InOneMicrosecond / Nanoseconds.InOneMicrosecond;

        public const Double InOnePicosecond = InOneNanosecond / Picoseconds.InOneNanosecond;

        /// <summary>
        ///     <para>18550948324478400000 (where did I get this number??? It's so.. specific?)</para>
        /// </summary>
        public const Double InOneSecond = 18550948324478E30;

        public const Double InOneWeek = InOneSecond * Seconds.InOneWeek;

        public const Double InOneYear = InOneSecond * Seconds.InOneCommonYear;

        public const Double InOneYoctosecond = InOneZeptosecond / Yoctoseconds.InOneZeptosecond;

        public const Double InOneZeptosecond = InOneAttosecond / Zeptoseconds.InOneAttosecond;

        /// <summary>One <see cref="PlanckTimes" />.</summary>
        public static PlanckTimes One { get; } = new PlanckTimes( value: 1 );

        /// <summary>Two <see cref="PlanckTimes" />.</summary>
        public static PlanckTimes Two { get; } = new PlanckTimes( value: 2 );

        /// <summary>Zero <see cref="PlanckTimes" />.</summary>
        public static PlanckTimes Zero { get; } = new PlanckTimes( value: 0 );

        [JsonProperty]
        public BigInteger Value { get; }

        public PlanckTimes( Int64 value ) : this( value: ( BigInteger ) value ) { }

        public PlanckTimes( Rational value ) : this( value: value.WholePart ) { }

        public PlanckTimes( BigInteger value ) => this.Value = value;

        public PlanckTimes( Seconds seconds ) : this( value: seconds.ToPlanckTimes().Value ) { }

        public PlanckTimes( Years years ) : this( value: years.ToPlanckTimes().Value ) { }

        public static PlanckTimes Combine( PlanckTimes left, PlanckTimes right ) => new PlanckTimes( value: left.Value + right.Value );

        public static PlanckTimes Combine( PlanckTimes left, BigInteger planckTimes ) => new PlanckTimes( value: left.Value + planckTimes );

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( PlanckTimes left, PlanckTimes right ) => left.Value == right.Value;

        [NotNull]
        public static implicit operator SpanOfTime( PlanckTimes planckTimes ) => new SpanOfTime( planckTimes: planckTimes );

        /// <summary>Implicitly convert the number of <paramref name="planckTimes" /> to <see cref="Yoctoseconds" />.</summary>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds( PlanckTimes planckTimes ) => ToYoctoseconds( planckTimes: planckTimes );

        public static PlanckTimes operator -( PlanckTimes left, BigInteger planckTimes ) => Combine( left: left, planckTimes: -planckTimes );

        public static Boolean operator !=( PlanckTimes left, PlanckTimes right ) => !Equals( left: left, right: right );

        public static PlanckTimes operator +( PlanckTimes left, PlanckTimes right ) => Combine( left: left, right: right );

        public static PlanckTimes operator +( PlanckTimes left, BigInteger planckTimes ) => Combine( left: left, planckTimes: planckTimes );

        public static Boolean operator <( PlanckTimes left, PlanckTimes right ) => left.Value < right.Value;

        public static Boolean operator <=( PlanckTimes left, PlanckTimes right ) => left.Value <= right.Value;

        public static Boolean operator ==( PlanckTimes left, PlanckTimes right ) => Equals( left: left, right: right );

        public static Boolean operator >( PlanckTimes left, PlanckTimes right ) => left.Value > right.Value;

        public static Boolean operator >=( PlanckTimes left, PlanckTimes right ) => left.Value >= right.Value;

        /// <summary>
        ///     <para>Convert to a larger unit.</para>
        /// </summary>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static Yoctoseconds ToYoctoseconds( PlanckTimes planckTimes ) => new Yoctoseconds( value: planckTimes.Value / ( Rational ) InOneYoctosecond );

        public Int32 CompareTo( PlanckTimes other ) => this.Value.CompareTo( other: other.Value );

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

        public Boolean Equals( PlanckTimes other ) => Equals( left: this, right: other );

        public override Boolean Equals( Object obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is PlanckTimes times && this.Equals( other: times );
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => this;

        public Seconds ToSeconds() => new Seconds( value: this.Value * ( Rational ) InOneSecond );

        public override String ToString() => $"{this.Value} tP";

        public TimeSpan ToTimeSpan() => this.ToSeconds();

    }

}