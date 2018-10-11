// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "PlanckTimes.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "PlanckTimes.cs" was last formatted by Protiguous on 2018/08/15 at 7:09 PM.

namespace Librainian.Measurement.Time {

    using Newtonsoft.Json;
    using Numerics;
    using System;
    using System.Diagnostics;
    using System.Numerics;

    /// <summary>
    ///     <para>
    ///         In physics, the Planck time (tP) is the unit of time in the system of natural units known as Planck units.
    ///     </para>
    ///     <para>
    ///         It is the time required for light to travel, in a vacuum, a distance of 1 Planck length.
    ///     </para>
    ///     <para>The Planck time is defined as:</para>
    ///     <para>t_P \equiv \sqrt{\frac{\hbar G}{c^5}} ≈ 5.39106(32) × 10−44 s</para>
    ///     <para>
    ///         where: \hbar = h / 2 \pi is the reduced Planck constant (sometimes h is used instead of
    ///         \hbar in the definition[1]) G = gravitational constant c = speed of light in a vacuum
    ///         s is the SI unit of time, the second. The two digits between parentheses denote the
    ///         standard error of the estimated value.
    ///     </para>
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Planck_time" />
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    [JsonObject]
    public struct PlanckTimes : IComparable<PlanckTimes>, IQuantityOfTime {

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

        /// <summary>
        ///     One <see cref="PlanckTimes" />.
        /// </summary>
        public static PlanckTimes One { get; } = new PlanckTimes(1);

        /// <summary>
        ///     Two <see cref="PlanckTimes" />.
        /// </summary>
        public static PlanckTimes Two { get; } = new PlanckTimes(2);

        /// <summary>
        ///     Zero <see cref="PlanckTimes" />.
        /// </summary>
        public static PlanckTimes Zero { get; } = new PlanckTimes(0);

        [JsonProperty]
        public BigInteger Value { get; }

        public PlanckTimes(Int64 value) : this((BigInteger)value) { }

        public PlanckTimes(BigRational value) : this(value.GetWholePart()) { }

        public PlanckTimes(BigInteger value) => this.Value = value <= BigInteger.Zero ? BigInteger.Zero : value;

        public PlanckTimes(Seconds seconds) : this(seconds.ToPlanckTimes().Value) { }

        public PlanckTimes(Years years) : this(years.ToPlanckTimes().Value) { }

        public static PlanckTimes Combine(PlanckTimes left, PlanckTimes right) {
            if (left == null) { throw new ArgumentNullException(paramName: nameof(left)); }

            if (right == null) { throw new ArgumentNullException(paramName: nameof(right)); }

            return new PlanckTimes(left.Value + right.Value);
        }

        public static PlanckTimes Combine(PlanckTimes left, BigInteger planckTimes) {
            if (left == null) { throw new ArgumentNullException(paramName: nameof(left)); }

            return new PlanckTimes(left.Value + planckTimes);
        }

        /// <summary>
        ///     <para>static equality test</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals(PlanckTimes left, PlanckTimes right) => left.Value == right.Value;

        public static implicit operator SpanOfTime(PlanckTimes planckTimes) => new SpanOfTime(planckTimes);

        /// <summary>
        ///     Implicitly convert the number of <paramref name="planckTimes" /> to <see cref="Yoctoseconds" />.
        /// </summary>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static implicit operator Yoctoseconds(PlanckTimes planckTimes) => ToYoctoseconds(planckTimes);

        public static PlanckTimes operator -(PlanckTimes left, BigInteger planckTimes) => Combine(left, -planckTimes);

        public static Boolean operator !=(PlanckTimes left, PlanckTimes right) => !Equals(left, right);

        public static PlanckTimes operator +(PlanckTimes left, PlanckTimes right) => Combine(left, right);

        public static PlanckTimes operator +(PlanckTimes left, BigInteger planckTimes) => Combine(left, planckTimes);

        public static Boolean operator <(PlanckTimes left, PlanckTimes right) => left.Value < right.Value;

        public static Boolean operator <=(PlanckTimes left, PlanckTimes right) => left.Value <= right.Value;

        public static Boolean operator ==(PlanckTimes left, PlanckTimes right) => Equals(left, right);

        public static Boolean operator >(PlanckTimes left, PlanckTimes right) => left.Value > right.Value;

        public static Boolean operator >=(PlanckTimes left, PlanckTimes right) => left.Value >= right.Value;

        /// <summary>
        ///     <para>Convert to a larger unit.</para>
        /// </summary>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static Yoctoseconds ToYoctoseconds(PlanckTimes planckTimes) => new Yoctoseconds(planckTimes.Value / (BigInteger)InOneYoctosecond);

        public Int32 CompareTo(PlanckTimes other) => this.Value.CompareTo(other.Value);

        public Boolean Equals(PlanckTimes other) => Equals(this, other);

        public override Boolean Equals(Object obj) {
            if (obj == null) { return false; }

            return obj is PlanckTimes times && this.Equals(times);
        }

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public PlanckTimes ToPlanckTimes() => this;

        public Seconds ToSeconds() => new Seconds(this.Value * (BigInteger)InOneSecond);

        public override String ToString() => $"{this.Value} tP";

        public TimeSpan ToTimeSpan() => throw new NotImplementedException();
    }
}