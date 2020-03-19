// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "SpanOfTime.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "SpanOfTime.cs" was last formatted by Protiguous on 2020/03/16 at 3:08 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;
    using Collections.Extensions;
    using Extensions;
    using JetBrains.Annotations;
    using JM.LinqFaster.SIMD;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;
    using Rationals;

    /// <summary>
    ///     <para><see cref="SpanOfTime" /> represents the smallest <see cref="PlanckTimes" /> to an absurd huge(!) duration of time.</para>
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Units_of_time" />
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject( memberSerialization: MemberSerialization.Fields )]
    [Immutable]
    public struct SpanOfTime : IEquatable<SpanOfTime>, IComparable<SpanOfTime>, IComparable<TimeSpan> {

        /// <summary></summary>
        /// <summary>
        ///     <para>1 of each measure of time</para>
        /// </summary>
        public static SpanOfTime Identity = new SpanOfTime( planckTimes: 1, yoctoseconds: 1, zeptoseconds: 1, attoseconds: 1, femtoseconds: 1, picoseconds: 1, nanoseconds: 1,
            microseconds: 1, milliseconds: 1, seconds: 1, minutes: 1, hours: 1, days: 1, weeks: 1, months: 1, years: 1 );

        /// <summary></summary>
        public static SpanOfTime Zero = new SpanOfTime( planckTimes: 0 );

        /// <summary></summary>
        [JsonProperty]
        public Attoseconds Attoseconds { get; }

        /// <summary>How many <see cref="Days" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Days Days { get; }

        /// <summary></summary>
        [JsonProperty]
        public Femtoseconds Femtoseconds { get; }

        /// <summary>How many <see cref="Hours" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Hours Hours { get; }

        /// <summary>
        ///     <para>A microsecond is an SI unit of time equal to one millionth (10−6 or 1/1,000,000) of a second.</para>
        ///     <para>Its symbol is μs.</para>
        /// </summary>
        /// <trivia>One microsecond is to one second as one second is to 11.574 days.</trivia>
        [JsonProperty]
        public Microseconds Microseconds { get; }

        /// <summary>How many <see cref="Milliseconds" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Milliseconds Milliseconds { get; }

        /// <summary>How many <see cref="Minutes" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Minutes Minutes { get; }

        /// <summary>How many <see cref="Months" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Months Months { get; }

        /// <summary></summary>
        [JsonProperty]
        public Nanoseconds Nanoseconds { get; }

        /// <summary>A picosecond is an SI unit of time equal to 10E−12 of a second.</summary>
        /// <see cref="http://wikipedia.org/wiki/Picosecond" />
        [JsonProperty]
        public Picoseconds Picoseconds { get; }

        /// <summary></summary>
        [JsonProperty]
        public PlanckTimes PlanckTimes { get; }

        /// <summary>How many <see cref="Seconds" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Seconds Seconds { get; }

        /// <summary>How many <see cref="Weeks" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Weeks Weeks { get; }

        /// <summary>How many <see cref="Years" /> does this <see cref="SpanOfTime" /> span?</summary>
        [JsonProperty]
        public Years Years { get; }

        /// <summary></summary>
        [JsonProperty]
        public Yoctoseconds Yoctoseconds { get; }

        /// <summary></summary>
        [JsonProperty]
        public Zeptoseconds Zeptoseconds { get; }

        public SpanOfTime( BigInteger planckTimes ) {

            this.Years = new Years( value: PlanckTimes.InOneYear.PullPlancks( planckTimes: ref planckTimes ) );

            this.Months = new Months( value: PlanckTimes.InOneMonth.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Months.Value.Should().BeInRange( 0, Months.InOneCommonYear );

            this.Weeks = new Weeks( value: PlanckTimes.InOneWeek.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Weeks.Value.Should().BeInRange( 0, Weeks.InOneCommonYear );

            this.Days = new Days( value: PlanckTimes.InOneDay.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Days.Value.Should().BeInRange( 0, Days.InOneCommonYear + 1 ); //leap year

            this.Hours = new Hours( value: PlanckTimes.InOneHour.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Hours.Value.Should().BeInRange( 0, Hours.InOneDay );

            this.Minutes = new Minutes( value: PlanckTimes.InOneMinute.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Minutes.Value.Should().BeInRange( 0, Minutes.InOneHour );

            this.Seconds = new Seconds( value: PlanckTimes.InOneSecond.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Seconds.Value.Should().BeInRange( 0, Seconds.InOneMinute );

            this.Milliseconds = new Milliseconds( value: PlanckTimes.InOneMillisecond.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Milliseconds.Value.Should().BeInRange( 0, Milliseconds.InOneSecond );

            this.Microseconds = new Microseconds( value: PlanckTimes.InOneMicrosecond.PullPlancks( planckTimes: ref planckTimes ) );

            //this.Microseconds.Value.Should().BeInRange( 0, Microseconds.InOneMillisecond );

            this.Nanoseconds = new Nanoseconds( value: PlanckTimes.InOneNanosecond.PullPlancks( planckTimes: ref planckTimes ) );

            this.Picoseconds = new Picoseconds( value: PlanckTimes.InOnePicosecond.PullPlancks( planckTimes: ref planckTimes ) );

            this.Femtoseconds = new Femtoseconds( value: PlanckTimes.InOneFemtosecond.PullPlancks( planckTimes: ref planckTimes ) );

            this.Attoseconds = new Attoseconds( value: PlanckTimes.InOneAttosecond.PullPlancks( planckTimes: ref planckTimes ) );

            this.Zeptoseconds = new Zeptoseconds( value: PlanckTimes.InOneZeptosecond.PullPlancks( planckTimes: ref planckTimes ) );

            this.Yoctoseconds = new Yoctoseconds( value: PlanckTimes.InOneYoctosecond.PullPlancks( planckTimes: ref planckTimes ) );

            this.PlanckTimes = new PlanckTimes( value: planckTimes );
        }

        /// <summary>
        ///     <para>Negative parameters passed to this constructor will interpret as zero instead of throwing an <see cref="ArgumentOutOfRangeException" />.</para>
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="normalize"></param>
        public SpanOfTime( TimeSpan timeSpan, Boolean normalize = true ) : this( seconds: new Milliseconds( value: timeSpan.Ticks / TimeSpan.TicksPerMillisecond ) ) { }

        /// <summary>
        ///     <para>Negative parameters passed to this constructor will interpret as zero instead of throwing an <see cref="ArgumentOutOfRangeException" />.</para>
        /// </summary>
        /// <param name="femtoseconds"></param>
        /// <param name="picoseconds"></param>
        /// <param name="attoseconds"></param>
        /// <param name="nanoseconds"></param>
        /// <param name="microseconds"></param>
        /// <param name="milliseconds"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="days"></param>
        /// <param name="weeks"></param>
        /// <param name="months"></param>
        /// <param name="years"></param>
        /// <param name="planckTimes"></param>
        /// <param name="yoctoseconds"></param>
        /// <param name="zeptoseconds"></param>
        public SpanOfTime( PlanckTimes? planckTimes = default, Yoctoseconds? yoctoseconds = default, Zeptoseconds? zeptoseconds = default, Attoseconds? attoseconds = default,
            Femtoseconds? femtoseconds = default, Picoseconds? picoseconds = default, Nanoseconds? nanoseconds = default, Microseconds? microseconds = default,
            Milliseconds? milliseconds = default, Seconds? seconds = default, Minutes? minutes = default, Hours? hours = default, Days? days = default, Weeks? weeks = default,
            Months? months = default, Years? years = default ) : this( planckTimes: planckTimes?.Value, yoctoseconds: yoctoseconds?.Value, zeptoseconds: zeptoseconds?.Value,
            attoseconds: attoseconds?.Value, femtoseconds: femtoseconds?.Value, picoseconds: picoseconds?.Value, nanoseconds: nanoseconds?.Value,
            microseconds: microseconds?.Value, milliseconds: milliseconds?.Value, seconds: seconds?.Value, minutes: minutes?.Value, hours: hours?.Value, days: days?.Value,
            weeks: weeks?.Value, months: months?.Value, years: years?.Value ) { }

        /// <summary></summary>
        /// <param name="planckTimes"></param>
        /// <param name="yoctoseconds"></param>
        /// <param name="zeptoseconds"></param>
        /// <param name="attoseconds"></param>
        /// <param name="femtoseconds"></param>
        /// <param name="picoseconds"></param>
        /// <param name="nanoseconds"></param>
        /// <param name="microseconds"></param>
        /// <param name="milliseconds"></param>
        /// <param name="seconds"></param>
        /// <param name="minutes"></param>
        /// <param name="hours"></param>
        /// <param name="days"></param>
        /// <param name="weeks"></param>
        /// <param name="months"></param>
        /// <param name="years"></param>
        public SpanOfTime( BigInteger? planckTimes, Rational? yoctoseconds = null, Rational? zeptoseconds = null, Rational? attoseconds = null, Rational? femtoseconds = null,
            Rational? picoseconds = null, Rational? nanoseconds = null, Rational? microseconds = null, Rational? milliseconds = null, Rational? seconds = null,
            Rational? minutes = null, Rational? hours = null, Rational? days = null, Rational? weeks = null, Rational? months = null, Rational? years = null ) {

            //TODO Unit testing needed to verify the math.

            this.PlanckTimes = PlanckTimes.Zero;

            if ( planckTimes.HasValue ) {
                this.PlanckTimes += new PlanckTimes( value: planckTimes.Value );
            }

            this.PlanckTimes += new Yoctoseconds( value: yoctoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Zeptoseconds( value: zeptoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Attoseconds( value: attoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Femtoseconds( value: femtoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Picoseconds( value: picoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Nanoseconds( value: nanoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Microseconds( value: microseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Milliseconds( value: milliseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Seconds( value: seconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Minutes( value: minutes.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Hours( value: hours.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Days( value: days.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Weeks( weeks: weeks.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Months( value: months.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Years( value: years.IfLessThanZeroThenZero() ).ToPlanckTimes();

            var spanOfTime = new SpanOfTime( planckTimes: this.PlanckTimes.Value ); //cheat?

            this.PlanckTimes = spanOfTime.PlanckTimes;
            this.Yoctoseconds = spanOfTime.Yoctoseconds;
            this.Zeptoseconds = spanOfTime.Zeptoseconds;
            this.Attoseconds = spanOfTime.Attoseconds;
            this.Femtoseconds = spanOfTime.Femtoseconds;
            this.Picoseconds = spanOfTime.Picoseconds;
            this.Nanoseconds = spanOfTime.Nanoseconds;
            this.Microseconds = spanOfTime.Microseconds;
            this.Milliseconds = spanOfTime.Milliseconds;
            this.Seconds = spanOfTime.Seconds;
            this.Minutes = spanOfTime.Minutes;
            this.Hours = spanOfTime.Hours;
            this.Days = spanOfTime.Days;
            this.Weeks = spanOfTime.Weeks;
            this.Months = spanOfTime.Months;
            this.Years = spanOfTime.Years;

            //this.TotalPlanckTimes = this.CalcTotalPlanckTimes();
        }

        /// <summary>
        ///     <para>Negative parameters passed to this constructor will interpret as zero instead of throwing an <see cref="ArgumentOutOfRangeException" />.</para>
        /// </summary>
        /// <summary>TODO untested</summary>
        /// <param name="seconds"></param>
        public SpanOfTime( Rational seconds ) {
            var span = new SpanOfTime( planckTimes: new Seconds( value: seconds ).ToPlanckTimes().Value );

            this.PlanckTimes = span.PlanckTimes;
            this.Attoseconds = span.Attoseconds;
            this.Days = span.Days;
            this.Femtoseconds = span.Femtoseconds;
            this.Hours = span.Hours;
            this.Microseconds = span.Microseconds;
            this.Milliseconds = span.Milliseconds;
            this.Minutes = span.Minutes;
            this.Months = span.Months;
            this.Nanoseconds = span.Nanoseconds;
            this.Picoseconds = span.Picoseconds;
            this.Seconds = span.Seconds;

            this.Weeks = span.Weeks;
            this.Years = span.Years;
            this.Yoctoseconds = span.Yoctoseconds;
            this.Zeptoseconds = span.Zeptoseconds;
        }

        /// <summary>
        ///     <para>Given the <paramref name="left" /><see cref="SpanOfTime" />,</para>
        ///     <para>add (+) the <paramref name="right" /><see cref="SpanOfTime" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SpanOfTime Combine( SpanOfTime left, SpanOfTime right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            //TODO do some overflow handling with BigInteger math

            //var planckTimes = left.PlanckTimes + right.PlanckTimes;
            var yoctoseconds = left.Yoctoseconds + right.Yoctoseconds;
            var zeptoseconds = left.Zeptoseconds + right.Zeptoseconds;
            var attoseconds = left.Attoseconds + right.Attoseconds;
            var femtoseconds = left.Femtoseconds + right.Femtoseconds;
            var picoseconds = left.Picoseconds + right.Picoseconds;
            var nanoseconds = left.Nanoseconds + right.Nanoseconds;
            var microseconds = left.Microseconds + right.Microseconds;
            var milliseconds = left.Milliseconds + right.Milliseconds;
            var seconds = left.Seconds + right.Seconds;
            var minutes = left.Minutes + right.Minutes;
            var hours = left.Hours + right.Hours;
            var days = left.Days + right.Days;
            var months = left.Months + right.Months;
            var years = left.Years + right.Years;

            return new SpanOfTime( yoctoseconds: yoctoseconds, zeptoseconds: zeptoseconds, attoseconds: attoseconds, femtoseconds: femtoseconds, picoseconds: picoseconds,
                nanoseconds: nanoseconds, microseconds: microseconds, milliseconds: milliseconds, seconds: seconds, minutes: minutes, hours: hours, days: days, months: months,
                years: years );
        }

        ///// <summary>
        ///// </summary>
        //public BigInteger TotalPlanckTimes => this.LazyTotal.Value.Value;
        /// <summary>
        ///     <para>Compares two <see cref="SpanOfTime" /> values, returning an <see cref="Int32" /> that indicates their relationship.</para>
        ///     <para>Returns 1 if <paramref name="left" /> is larger.</para>
        ///     <para>Returns -1 if <paramref name="right" /> is larger.</para>
        ///     <para>Returns 0 if <paramref name="left" /> and <paramref name="right" /> are equal.</para>
        ///     <para>Static comparison function</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Int32 CompareTo( SpanOfTime left, SpanOfTime right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            var leftPlancks = left.CalcTotalPlanckTimes();
            var rightPlancks = right.CalcTotalPlanckTimes();

            return leftPlancks.CompareTo( other: rightPlancks );
        }

        /// <summary>
        ///     <para>Static comparison of two <see cref="SpanOfTime" /> values.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( SpanOfTime left, SpanOfTime right ) => left.CalcTotalPlanckTimes() == right.CalcTotalPlanckTimes();

        /// <summary>
        ///     <para>Allow an explicit cast from a <see cref="TimeSpan" /> into a <see cref="SpanOfTime" />.</para>
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static explicit operator SpanOfTime( TimeSpan span ) => new SpanOfTime( timeSpan: span );

        /// <summary>Allow an automatic cast to <see cref="TimeSpan" />.</summary>
        /// <param name="spanOfTime"></param>
        /// <returns></returns>
        public static implicit operator TimeSpan( SpanOfTime spanOfTime ) =>
            new TimeSpan( days: ( Int32 ) spanOfTime.Days.Value, hours: ( Int32 ) spanOfTime.Hours.Value, minutes: ( Int32 ) spanOfTime.Minutes.Value,
                seconds: ( Int32 ) spanOfTime.Seconds.Value, milliseconds: ( Int32 ) spanOfTime.Milliseconds.Value );

        /// <summary>
        ///     <para>Given the <paramref name="left" /><see cref="SpanOfTime" />,</para>
        ///     <para>subtract (-) the <paramref name="right" /><see cref="SpanOfTime" />.</para>
        ///     <para>And return the resulting span (which will not go below 0).</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SpanOfTime operator -( SpanOfTime left, SpanOfTime right ) {
            var leftPlancks = left.CalcTotalPlanckTimes();
            var rightPlancks = right.CalcTotalPlanckTimes();

            var result = leftPlancks.Value - rightPlancks.Value;

            return result <= BigInteger.Zero ? Zero : new SpanOfTime( planckTimes: result );
        }

        public static Boolean operator !=( SpanOfTime t1, SpanOfTime t2 ) => !Equals( left: t1, right: t2 );

        /// <summary>
        ///     <para>Given the <paramref name="left" /><see cref="SpanOfTime" />,</para>
        ///     <para>add (+) the <paramref name="right" /><see cref="SpanOfTime" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static SpanOfTime operator +( SpanOfTime left, SpanOfTime right ) {
            if ( left == null ) {
                throw new ArgumentNullException( paramName: nameof( left ) );
            }

            if ( right == null ) {
                throw new ArgumentNullException( paramName: nameof( right ) );
            }

            return Combine( left: left, right: right );
        }

        public static Boolean operator <( SpanOfTime left, SpanOfTime right ) => left.CalcTotalPlanckTimes() < right.CalcTotalPlanckTimes();

        public static Boolean operator <=( SpanOfTime left, SpanOfTime right ) => left.CalcTotalPlanckTimes() <= right.CalcTotalPlanckTimes();

        public static Boolean operator ==( SpanOfTime left, SpanOfTime right ) => Equals( left: left, right: right );

        public static Boolean operator >( SpanOfTime left, SpanOfTime right ) => left.CalcTotalPlanckTimes() > right.CalcTotalPlanckTimes();

        public static Boolean operator >=( SpanOfTime left, SpanOfTime right ) => left.CalcTotalPlanckTimes() >= right.CalcTotalPlanckTimes();

        /// <summary>assume seconds given</summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static SpanOfTime TryParse( [CanBeNull] String text ) {
            try {
                if ( null == text ) {
                    return Zero;
                }

                text = text.Trim();

                if ( text.IsNullOrWhiteSpace() ) {
                    return Zero;
                }

                if ( TimeSpan.TryParse( s: text, result: out var result ) ) {
                    return new SpanOfTime( timeSpan: result ); //cheat and use the existing TimeSpan parsing code...
                }

                if ( text.TryGetDecimal( result: out var units ) ) {
                    return new SpanOfTime( seconds: ( Rational ) units ); //assume seconds given
                }

                if ( text.EndsWith( value: "milliseconds", comparisonType: StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( splitter: "milliseconds" );

                    if ( text.TryGetDecimal( result: out units ) ) {
                        return new SpanOfTime( seconds: new Milliseconds( value: units ) );
                    }
                }

                if ( text.EndsWith( value: "millisecond", comparisonType: StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( splitter: "millisecond" );

                    if ( text.TryGetDecimal( result: out units ) ) {
                        return new SpanOfTime( seconds: new Milliseconds( value: units ) );
                    }
                }

                if ( text.EndsWith( value: "seconds", comparisonType: StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( splitter: "seconds" );

                    if ( text.TryGetDecimal( result: out units ) ) {
                        return new SpanOfTime( seconds: ( Rational ) units );
                    }
                }

                if ( text.EndsWith( value: "second", comparisonType: StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( splitter: "second" );

                    if ( text.TryGetDecimal( result: out units ) ) {
                        return new SpanOfTime( seconds: ( Rational ) units );
                    }
                }

                //TODO parse for more, even multiple  2 days, 3 hours, and 4 minutes etc...
            }
            catch ( ArgumentNullException ) { }
            catch ( ArgumentException ) { }
            catch ( OverflowException ) { }

            return Zero;
        }

        public PlanckTimes CalcTotalPlanckTimes() {
            var sum = new[] {
                this.PlanckTimes.ToPlanckTimes(), this.Yoctoseconds.ToPlanckTimes(), this.Zeptoseconds.ToPlanckTimes(), this.Attoseconds.ToPlanckTimes(),
                this.Femtoseconds.ToPlanckTimes(), this.Picoseconds.ToPlanckTimes(), this.Nanoseconds.ToPlanckTimes(), this.Microseconds.ToPlanckTimes(),
                this.Milliseconds.ToPlanckTimes(), this.Seconds.ToPlanckTimes(), this.Minutes.ToPlanckTimes(), this.Hours.ToPlanckTimes(), this.Days.ToPlanckTimes(),
                this.Weeks.ToPlanckTimes(), this.Months.ToPlanckTimes(), this.Years.ToPlanckTimes()
            };

            return sum.SumS();

            //return sum.Aggregate( PlanckTimes.Zero, ( current, timese ) => current + timese );
        }

        public Int32 CompareTo( SpanOfTime other ) => CompareTo( left: this, right: other );

        public Int32 CompareTo( TimeSpan other ) => CompareTo( left: this, right: new SpanOfTime( timeSpan: other ) );

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
        /// <param name="obj">Another object to compare to.</param>
        /// <filterpriority>2</filterpriority>
        public override Boolean Equals( [CanBeNull] Object? obj ) {
            if ( obj is null ) {
                return default;
            }

            return obj is SpanOfTime span && Equals( left: this, right: span );
        }

        public Boolean Equals( SpanOfTime obj ) => Equals( left: this, right: obj );

        /// <summary>
        ///     <para>Return a <see cref="TimeSpan" />'s worth of <see cref="Milliseconds" />.</para>
        ///     <para>
        ///     <see cref="Days" />+ <see cref="Hours" />+ <see cref="Minutes" />+ <see cref="Seconds" />+ <see cref="Milliseconds" />+ <see cref="Microseconds" />+
        ///     <see cref="Nanoseconds" />
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public Double GetApproximateMilliseconds() {
            var mill = Milliseconds.Zero;
            mill += this.Nanoseconds.ToMicroseconds().ToMilliseconds();
            mill += this.Microseconds.ToMilliseconds();
            mill += this.Milliseconds;
            mill += this.Seconds.ToMilliseconds();
            mill += this.Minutes.ToSeconds().ToMilliseconds();
            mill += this.Hours.ToMinutes().ToSeconds().ToMilliseconds();
            mill += this.Days.ToHours().ToMinutes().ToSeconds().ToMilliseconds();

            return ( Double ) mill.Value;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <filterpriority>2</filterpriority>
        public override Int32 GetHashCode() =>
            ( this.Yoctoseconds, this.Zeptoseconds, this.Attoseconds, this.Femtoseconds, this.Picoseconds, this.Nanoseconds, this.Microseconds, this.Milliseconds,
                this.Seconds, this.Minutes, this.Hours, this.Days, this.Weeks, this.Months, this.Years ).GetHashCode();

        /// <summary>
        ///     <para>Returns a <see cref="BigInteger" /> of all the whole (integer) years in this <see cref="SpanOfTime" />.</para>
        /// </summary>
        public BigInteger GetWholeYears() {
            var span = new SpanOfTime( planckTimes: this.CalcTotalPlanckTimes() );

            return span.Years.Value.WholePart;
        }

        [NotNull]
        public override String ToString() {
            var bob = new Queue<String>( capacity: 20 );

            if ( this.Years.Value != 0 ) {
                bob.Enqueue( item: this.Years.ToString() );
            }

            if ( this.Months.Value != 0 ) {
                bob.Enqueue( item: this.Months.ToString() );
            }

            if ( this.Weeks.Value != 0 ) {
                bob.Enqueue( item: this.Weeks.ToString() );
            }

            if ( this.Days.Value != 0 ) {
                bob.Enqueue( item: this.Days.ToString() );
            }

            if ( this.Hours.Value != 0 ) {
                bob.Enqueue( item: this.Hours.ToString() );
            }

            if ( this.Minutes.Value != 0 ) {
                bob.Enqueue( item: this.Minutes.ToString() );
            }

            if ( this.Seconds.Value != 0 ) {
                bob.Enqueue( item: this.Seconds.ToString() );
            }

            if ( this.Milliseconds.Value != 0 ) {
                bob.Enqueue( item: this.Milliseconds.ToString() );
            }

            if ( this.Microseconds.Value != 0 ) {
                bob.Enqueue( item: this.Microseconds.ToString() );
            }

            if ( this.Nanoseconds.Value != 0 ) {
                bob.Enqueue( item: this.Nanoseconds.ToString() );
            }

            if ( this.Picoseconds.Value != 0 ) {
                bob.Enqueue( item: this.Picoseconds.ToString() );
            }

            if ( this.Femtoseconds.Value != 0 ) {
                bob.Enqueue( item: this.Femtoseconds.ToString() );
            }

            if ( this.Attoseconds.Value != 0 ) {
                bob.Enqueue( item: this.Attoseconds.ToString() );
            }

            if ( this.Zeptoseconds.Value != 0 ) {
                bob.Enqueue( item: this.Zeptoseconds.ToString() );
            }

            if ( this.Yoctoseconds.Value != 0 ) {
                bob.Enqueue( item: this.Yoctoseconds.ToString() );
            }

            if ( this.PlanckTimes.Value != BigInteger.Zero ) {
                bob.Enqueue( item: this.PlanckTimes.ToString() );
            }

            return bob.ToStrings( separator: ", ", atTheEnd: ", and " );
        }

        Boolean IEquatable<SpanOfTime>.Equals( SpanOfTime other ) => this.Equals( obj: other );

        /*
                public String ApproximatelySeconds() {
                    BigDecimal bigSeconds = this.Seconds.Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Milliseconds.ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Microseconds.ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Nanoseconds.ToMicroseconds().ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Picoseconds.ToNanoseconds().ToMicroseconds().ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Femtoseconds.ToPicoseconds().ToNanoseconds().ToMicroseconds().ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Attoseconds.ToFemtoseconds().ToPicoseconds().ToNanoseconds().ToMicroseconds().ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Zeptoseconds.ToAttoseconds().ToFemtoseconds().ToPicoseconds().ToNanoseconds().ToMicroseconds().ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Yoctoseconds.ToZeptoseconds().ToAttoseconds().ToFemtoseconds().ToPicoseconds().ToNanoseconds().ToMicroseconds().ToMilliseconds().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Minutes.ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Hours.ToMinutes().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Days.ToHours().ToMinutes().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    bigSeconds += this.Weeks.ToDays().ToHours().ToMinutes().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                        goto display;
                    }

                    //bigSeconds += this.Months.ToYears().ToDays().ToHours().ToMinutes().ToSeconds().Value;
                    //if ( bigSeconds >= MaximumUsefulDecimal ) {
                    //    goto display;
                    //}

                    bigSeconds += this.Years.ToDays().ToHours().ToMinutes().ToSeconds().Value;
                    if ( bigSeconds >= MaximumUsefulDecimal ) {
                    }

                display:
                    if ( bigSeconds >= MaximumUsefulDecimal ) {

                        // ||||
                        bigSeconds = MaximumUsefulDecimal; // HACK VVVV
                    }
                    var asSeconds = new Seconds( ( Decimal )bigSeconds );
                    return String.Format( "{0} seconds", asSeconds );
                }
        */

    }

}