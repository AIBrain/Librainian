#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Span.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Collections;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Librainian.Extensions;
    using Maths;
    using Parsing;

    /// <summary>
    ///     <para><see cref="Span" /> represents the smallest planckTimes to (absurd)large(!) duration of time.</para>
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Units_of_time" />
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Serializable]
    [Immutable]
    public struct Span : IEquatable<Span>, IComparable<Span>, IComparable<TimeSpan> {

        /// <summary>
        /// <para>1 of each measure of time</para>
        /// </summary>
        public static readonly Span Identity = new Span( 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 );

        /// <summary>
        /// 
        /// </summary>
        public static readonly Span Forever = new Span( yoctoseconds: Decimal.MaxValue, zeptoseconds: Decimal.MaxValue, attoseconds: Decimal.MaxValue, femtoseconds: Decimal.MaxValue, picoseconds: Decimal.MaxValue, nanoseconds: Decimal.MaxValue, microseconds: Decimal.MaxValue, milliseconds: Decimal.MaxValue, seconds: Decimal.MaxValue, minutes: Decimal.MaxValue, hours: Decimal.MaxValue, days: Decimal.MaxValue, weeks: Decimal.MaxValue, months: Decimal.MaxValue, years: Decimal.MaxValue.Half() );

        public static readonly Span Infinity = Forever;

        /// <summary>
        /// </summary>
        public static readonly Span Zero = new Span( planckTimes: 0 );

        /// <summary>
        /// </summary>
        public readonly Attoseconds Attoseconds;

        /// <summary>
        ///     How many <seealso cref="Days" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Days Days;

        /// <summary>
        /// </summary>
        public readonly Femtoseconds Femtoseconds;

        /// <summary>
        ///     How many <seealso cref="Hours" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Hours Hours;

        /// <summary>
        ///     <para>A microsecond is an SI unit of time equal to one millionth (10−6 or 1/1,000,000) of a second.</para>
        ///     <para>Its symbol is μs.</para>
        /// </summary>
        /// <trivia>One microsecond is to one second as one second is to 11.574 days.</trivia>
        public readonly Microseconds Microseconds;

        /// <summary>
        ///     How many <seealso cref="Milliseconds" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Milliseconds Milliseconds;

        /// <summary>
        ///     How many <seealso cref="Minutes" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Minutes Minutes;

        /// <summary>
        ///     How many <seealso cref="Months" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Months Months;

        /// <summary>
        /// </summary>
        public readonly Nanoseconds Nanoseconds;

        /// <summary>
        ///     A picosecond is an SI unit of time equal to 10E−12 of a second.
        /// </summary>
        /// <see cref="http://wikipedia.org/wiki/Picosecond" />
        public readonly Picoseconds Picoseconds;

        /// <summary>
        /// </summary>
        public readonly PlanckTimes PlanckTimes;

        /// <summary>
        ///     How many <seealso cref="Seconds" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Seconds Seconds;

        /// <summary>
        ///     How many <seealso cref="Weeks" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Weeks Weeks;

        /// <summary>
        ///     How many <seealso cref="Years" /> does this <seealso cref="Span" /> span?
        /// </summary>
        public readonly Years Years;

        /// <summary>
        /// </summary>
        public readonly Yoctoseconds Yoctoseconds;

        /// <summary>
        /// </summary>
        public readonly Zeptoseconds Zeptoseconds;

        //not the largest Span possible, but anything larger.. wow. just wow.
        //not the largest Span possible, but anything larger.. wow. just wow.
        private static readonly BigDecimal MaximumUsefulDecimal = new BigDecimal( Decimal.MaxValue );

        /// <summary>
        ///     <para>This value is not calculated until needed.</para>
        /// </summary>
        private readonly Lazy<BigInteger> _lazyTotal;

        static Span() {
            Identity.Years.Value.Should().Be( 1 );
            Identity.Months.Value.Should().Be( 1 );
            Identity.Weeks.Value.Should().Be( 1 );
            Identity.Days.Value.Should().Be( 1 );
            Identity.Hours.Value.Should().Be( 1 );
            Identity.Minutes.Value.Should().Be( 1 );
            Identity.Seconds.Value.Should().Be( 1 );
            Identity.Milliseconds.Value.Should().Be( 1 );
            Identity.Microseconds.Value.Should().Be( 1 );
            Identity.Nanoseconds.Value.Should().Be( 1 );
            Identity.Picoseconds.Value.Should().Be( 1 );
            Identity.Attoseconds.Value.Should().Be( 1 );
            Identity.Femtoseconds.Value.Should().Be( 1 );
            Identity.Zeptoseconds.Value.Should().Be( 1 );
            Identity.Yoctoseconds.Value.Should().Be( 1 );
        }

        public Span( BigInteger planckTimes )
            : this() {
            planckTimes.Should().BeGreaterOrEqualTo( BigInteger.Zero );

            if ( planckTimes < BigInteger.Zero ) {
                throw new ArgumentOutOfRangeException( "planckTimes", "Must be greater than or equal to 0" );
            }

            //NOTE the order here is mostly important. I think? maybe not. oh well.
            this.Years = new Years( PlanckTimes.InOneYear.PullPlancks( ref planckTimes ) );

            this.Months = new Months( PlanckTimes.InOneMonth.PullPlancks( ref planckTimes ) );
            this.Months.Value.Should().BeInRange( 0, Months.InOneCommonYear );

            this.Weeks = new Weeks( PlanckTimes.InOneWeek.PullPlancks( ref planckTimes ) );
            this.Weeks.Value.Should().BeInRange( 0.0m, Weeks.InOneCommonYear );

            var days = PlanckTimes.InOneDay.PullPlancks( ref planckTimes );
            this.Days = new Days( days );
            this.Days.Value.Should().BeInRange( 0, Days.InOneCommonYear + 1 ); //leap year

            var hours = PlanckTimes.InOneHour.PullPlancks( ref planckTimes );
            this.Hours = new Hours( hours );
            this.Hours.Value.Should().BeInRange( 0, Hours.InOneDay );

            this.Minutes = new Minutes( PlanckTimes.InOneMinute.PullPlancks( ref planckTimes ) );
            this.Minutes.Value.Should().BeInRange( 0, Minutes.InOneHour );

            this.Seconds = new Seconds( PlanckTimes.InOneSecond.PullPlancks( ref planckTimes ) );
            this.Seconds.Value.Should().BeInRange( 0, Seconds.InOneMinute );

            this.Milliseconds = new Milliseconds( PlanckTimes.InOneMillisecond.PullPlancks( ref planckTimes ) );
            this.Milliseconds.Value.Should().BeInRange( 0, Milliseconds.InOneSecond );

            this.Microseconds = new Microseconds( PlanckTimes.InOneMicrosecond.PullPlancks( ref planckTimes ) );
            this.Microseconds.Value.Should().BeInRange( 0, Microseconds.InOneMillisecond );

            this.Nanoseconds = new Nanoseconds( PlanckTimes.InOneNanosecond.PullPlancks( ref planckTimes ) );
            this.Nanoseconds.Value.Should().BeInRange( 0, Nanoseconds.InOneMicrosecond );

            this.Picoseconds = new Picoseconds( PlanckTimes.InOnePicosecond.PullPlancks( ref planckTimes ) );
            this.Picoseconds.Value.Should().BeInRange( 0, Picoseconds.InOneNanosecond );

            this.Femtoseconds = new Femtoseconds( PlanckTimes.InOneFemtosecond.PullPlancks( ref planckTimes ) );
            this.Femtoseconds.Value.Should().BeInRange( 0, Femtoseconds.InOnePicosecond );

            this.Attoseconds = new Attoseconds( PlanckTimes.InOneAttosecond.PullPlancks( ref planckTimes ) );
            this.Attoseconds.Value.Should().BeInRange( 0, Attoseconds.InOneFemtosecond );

            this.Zeptoseconds = new Zeptoseconds( PlanckTimes.InOneZeptosecond.PullPlancks( ref planckTimes ) );
            this.Zeptoseconds.Value.Should().BeInRange( 0, Zeptoseconds.InOneAttosecond );

            this.Yoctoseconds = new Yoctoseconds( PlanckTimes.InOneYoctosecond.PullPlancks( ref planckTimes ) );
            this.Yoctoseconds.Value.Should().BeInRange( 0, Yoctoseconds.InOneZeptosecond );

            planckTimes.ThrowIfOutOfDecimalRange();
            this.PlanckTimes += planckTimes;

            var tmpThis = this;
            this._lazyTotal = new Lazy<BigInteger>( valueFactory: () => tmpThis.CalcTotalPlanckTimes(), isThreadSafe: true );
        }

        /// <summary>
        ///     <para>
        ///         Negative parameters passed to this constructor will interpret as zero instead of throwing an
        ///         <see cref="ArgumentOutOfRangeException" />.
        ///     </para>
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="normalize"></param>
        public Span( TimeSpan timeSpan, Boolean normalize = true )
            : this( milliseconds: timeSpan.Milliseconds, seconds: timeSpan.Seconds, minutes: timeSpan.Minutes, hours: timeSpan.Hours, days: timeSpan.Days ) {
        }

        /// <summary>
        ///     <para>
        ///         Negative parameters passed to this constructor will interpret as zero instead of throwing an
        ///         <see cref="ArgumentOutOfRangeException" />.
        ///     </para>
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
        /// <param name="yoctoseconds"></param>
        /// <param name="zeptoseconds"></param>
        public Span( Yoctoseconds yoctoseconds = default( Yoctoseconds ), Zeptoseconds zeptoseconds = default( Zeptoseconds ), Attoseconds attoseconds = default( Attoseconds ), Femtoseconds femtoseconds = default( Femtoseconds ), Picoseconds picoseconds = default( Picoseconds ), Nanoseconds nanoseconds = default( Nanoseconds ), Microseconds microseconds = default( Microseconds ), Milliseconds milliseconds = default( Milliseconds ), Seconds seconds = default( Seconds ), Minutes minutes = default( Minutes ), Hours hours = default( Hours ), Days days = default( Days ), Weeks weeks = default( Weeks ), Months months = default( Months ), Years years = default( Years ) )
            : this( yoctoseconds: yoctoseconds.Value, zeptoseconds: zeptoseconds.Value, attoseconds: attoseconds.Value, femtoseconds: femtoseconds.Value, picoseconds: picoseconds.Value, nanoseconds: nanoseconds.Value, microseconds: microseconds.Value, milliseconds: milliseconds.Value, seconds: seconds.Value, minutes: minutes.Value, hours: hours.Value, days: days.Value, weeks: weeks.Value, months: months.Value, years: years.Value ) {
        }

        /// <summary>
        ///     <para>
        ///         Negative parameters passed to this constructor will interpret as zero instead of throwing an
        ///         <see cref="ArgumentOutOfRangeException" />.
        ///     </para>
        /// </summary>
        /// <param name="yoctoseconds"></param>
        /// <param name="zeptoseconds"></param>
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
        public Span( Decimal yoctoseconds = 0, Decimal zeptoseconds = 0, Decimal attoseconds = 0, Decimal femtoseconds = 0, Decimal picoseconds = 0, Decimal nanoseconds = 0, Decimal microseconds = 0, Decimal milliseconds = 0, Decimal seconds = 0, Decimal minutes = 0, Decimal hours = 0, Decimal days = 0, Decimal weeks = 0, Decimal months = 0, Decimal years = 0 )
            : this() {

            //TODO Unit testing needed to verify the math.

            //this.PlanckTimes = new PlanckTimes( planckTimes.IfLessThanZeroThenZero() );
            this.PlanckTimes = PlanckTimes.Zero;
            this.PlanckTimes += new Yoctoseconds( yoctoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Zeptoseconds( zeptoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Attoseconds( attoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Femtoseconds( femtoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Picoseconds( picoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Nanoseconds( nanoseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Microseconds( microseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Milliseconds( milliseconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Seconds( seconds.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Minutes( minutes.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Hours( hours.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Days( days.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Weeks( weeks.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Months( months.IfLessThanZeroThenZero() ).ToPlanckTimes();
            this.PlanckTimes += new Years( years.IfLessThanZeroThenZero() ).ToPlanckTimes();

            var span = new Span( planckTimes: this.PlanckTimes.Value ); //cheat

            this.PlanckTimes = span.PlanckTimes;
            this.Yoctoseconds = span.Yoctoseconds;
            this.Zeptoseconds = span.Zeptoseconds;
            this.Attoseconds = span.Attoseconds;
            this.Femtoseconds = span.Femtoseconds;
            this.Picoseconds = span.Picoseconds;
            this.Nanoseconds = span.Nanoseconds;
            this.Microseconds = span.Microseconds;
            this.Milliseconds = span.Milliseconds;
            this.Seconds = span.Seconds;
            this.Minutes = span.Minutes;
            this.Hours = span.Hours;
            this.Days = span.Days;
            this.Weeks = span.Weeks;
            this.Months = span.Months;
            this.Years = span.Years;

            //if ( normalize ) {
            //    //NOTE the order here is important.

            //    //BUG the "while"s used below can probably be changed to "if"s because of the math, which should work the first time.

            //    while ( this.PlanckTimes.Value > PlanckTimes.InOneYoctosecond ) {
            //        var truncate = this.PlanckTimes.Value / PlanckTimes.InOneYoctosecond;
            //        this.PlanckTimes -= truncate * PlanckTimes.InOneYoctosecond;
            //        truncate.ThrowIfOutOfDecimalRange();
            //        this.Yoctoseconds += (  System.Decimal )truncate;
            //    }

            //    while ( this.Yoctoseconds.Value > Yoctoseconds.InOneZeptosecond ) {
            //        var truncate = Math.Truncate( this.Yoctoseconds.Value / Yoctoseconds.InOneZeptosecond );
            //        this.Yoctoseconds -= truncate * Yoctoseconds.InOneZeptosecond;
            //        this.Zeptoseconds += truncate;
            //    }

            //    while ( this.Zeptoseconds.Value > Zeptoseconds.InOneAttosecond ) {
            //        var truncate = Math.Truncate( this.Zeptoseconds.Value / Zeptoseconds.InOneAttosecond );
            //        this.Zeptoseconds -= truncate * Yoctoseconds.InOneZeptosecond;
            //        this.Attoseconds += truncate;
            //    }

            //    while ( this.Attoseconds.Value > Attoseconds.InOneFemtosecond ) {
            //        var truncate = Math.Truncate( this.Attoseconds.Value / Attoseconds.InOneFemtosecond );
            //        this.Attoseconds -= truncate * Attoseconds.InOneFemtosecond;
            //        this.Femtoseconds += truncate;
            //    }

            //    while ( this.Femtoseconds.Value > Femtoseconds.InOnePicosecond ) {
            //        var truncate = Math.Truncate( this.Femtoseconds.Value / Femtoseconds.InOnePicosecond );
            //        this.Femtoseconds -= truncate * Femtoseconds.InOnePicosecond;
            //        this.Picoseconds += truncate;
            //    }

            //    while ( this.Picoseconds.Value > Picoseconds.InOneNanosecond ) {
            //        var truncate = Math.Truncate( this.Picoseconds.Value / Picoseconds.InOneNanosecond );
            //        this.Picoseconds -= truncate * Picoseconds.InOneNanosecond;
            //        this.Nanoseconds += truncate;
            //    }

            //    while ( this.Nanoseconds.Value > Nanoseconds.InOneMicrosecond ) {
            //        var truncate = Math.Truncate( this.Nanoseconds.Value / Nanoseconds.InOneMicrosecond );
            //        this.Nanoseconds -= truncate * Nanoseconds.InOneMicrosecond;
            //        this.Microseconds += truncate;
            //    }

            //    while ( this.Microseconds.Value > Microseconds.InOneMillisecond ) {
            //        var truncate = Math.Truncate( this.Microseconds.Value / Microseconds.InOneMillisecond );
            //        this.Microseconds -= truncate * Microseconds.InOneMillisecond;
            //        this.Milliseconds += truncate;
            //    }

            //    while ( this.Milliseconds > Milliseconds.InOneSecond ) {
            //        var truncate = Math.Truncate( this.Milliseconds.Value / Milliseconds.InOneSecond );
            //        this.Milliseconds -= truncate * Milliseconds.InOneSecond;
            //        this.Seconds += truncate;
            //    }

            //    while ( this.Seconds.Value > Seconds.InOneMinute ) {
            //        var truncate = Math.Truncate( this.Seconds.Value / Seconds.InOneMinute );
            //        this.Seconds -= truncate * Seconds.InOneMinute;
            //        this.Minutes += truncate;
            //    }

            //    while ( this.Minutes.Value > Minutes.InOneHour ) {
            //        var truncate = Math.Truncate( this.Minutes.Value / Minutes.InOneHour );
            //        this.Minutes -= truncate * Minutes.InOneHour;
            //        this.Hours += truncate;
            //    }

            //    while ( this.Hours.Value > Hours.InOneDay ) {
            //        var truncate = Math.Truncate( this.Hours.Value / Hours.InOneDay );
            //        this.Hours -= truncate * Hours.InOneDay;
            //        this.Days += truncate;
            //    }

            //    while ( this.Days.Value > Days.InOneWeek ) {
            //        var truncate = Math.Truncate( this.Days.Value / Days.InOneWeek );
            //        this.Days -= truncate * Days.InOneWeek;
            //        this.Weeks += truncate;
            //    }

            //    while ( this.Months.Value > Months.InOneYear ) {
            //        var truncate = Math.Truncate( this.Months.Value / Months.InOneYear );
            //        this.Months -= truncate * Months.InOneYear;
            //        this.Years += truncate;
            //    }

            //    if ( this.Months.Value < Months.InOneYear ) { break; }
            //    if ( normalize ) {
            //    }

            //    this.IsNormalized = true;
            //} else {
            //    this.IsNormalized = false;
            //}

            var tmpThis = this;
            this._lazyTotal = new Lazy<BigInteger>( valueFactory: () => tmpThis.CalcTotalPlanckTimes(), isThreadSafe: true );
        }

        /// <summary>
        ///     TODO untested
        /// </summary>
        /// <param name="seconds"></param>
        public Span( Decimal seconds )
            : this() {
            var span = new Span( new Seconds( seconds ).ToPlanckTimes() );

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

            //this.Weeks = span.Weeks;
            this.Years = span.Years;
            this.Yoctoseconds = span.Yoctoseconds;
            this.Zeptoseconds = span.Zeptoseconds;

            var tmpThis = this;
            this._lazyTotal = new Lazy<BigInteger>( valueFactory: () => tmpThis.CalcTotalPlanckTimes(), isThreadSafe: true );
        }

        /// <summary>
        /// </summary>
        public BigInteger TotalPlanckTimes => this._lazyTotal.Value;

        [UsedImplicitly]
        private String DebuggerDisplay => this.ToString();

        /// <summary>
        ///     <para>Given the <paramref name="left" /> <see cref="Span" />,</para>
        ///     <para>add (+) the <paramref name="right" /> <see cref="Span" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Span Combine( Span left, Span right ) {

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

            return new Span( yoctoseconds: yoctoseconds, zeptoseconds: zeptoseconds, attoseconds: attoseconds, femtoseconds: femtoseconds, picoseconds: picoseconds, nanoseconds: nanoseconds, microseconds: microseconds, milliseconds: milliseconds, seconds: seconds, minutes: minutes, hours: hours, days: days, months: months, years: years );
        }

        /// <summary>
        ///     <para>Compares two <see cref="Span" /> values, returning an <see cref="int" /> that indicates their relationship.</para>
        ///     <para>Returns 1 if <paramref name="left" /> is larger.</para>
        ///     <para>Returns -1 if <paramref name="right" /> is larger.</para>
        ///     <para>Returns 0 if <paramref name="left" /> and <paramref name="right" /> are equal.</para>
        ///     <para>Static comparison function</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int CompareTo( Span left, Span right ) {
            var leftPlancks = left.TotalPlanckTimes;
            var rightPlancks = right.TotalPlanckTimes;

            return leftPlancks.CompareTo( rightPlancks );
        }

        /// <summary>
        ///     <para>Static comparison of two <see cref="Span" /> values.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Span left, Span right ) {
            var leftPlancks = left.TotalPlanckTimes;
            var rightPlancks = right.TotalPlanckTimes;

            return leftPlancks == rightPlancks;
        }

        /// <summary>
        ///     <para>Allow an explicit cast from a <see cref="TimeSpan" /> into a <see cref="Span" />.</para>
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static explicit operator Span( TimeSpan span ) => new Span( span );

        /// <summary>
        ///     Allow an automatic cast to <see cref="TimeSpan" />.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static implicit operator TimeSpan( Span span ) => new TimeSpan( ( int )span.Days.Value, ( int )span.Hours.Value, ( int )span.Minutes.Value, ( int )span.Seconds.Value, ( int )span.Milliseconds.Value );

        /// <summary>
        ///     <para>Given the <paramref name="left" /> <see cref="Span" />,</para>
        ///     <para>subtract (-) the <paramref name="right" /> <see cref="Span" />.</para>
        ///     <para>And return the resulting span (which will not go below 0).</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Span operator -( Span left, Span right ) {
            var leftPlancks = left.TotalPlanckTimes;
            var rightPlancks = right.TotalPlanckTimes;

            var result = leftPlancks - rightPlancks;
            return result <= BigInteger.Zero ? Zero : new Span( result );
        }

        //public Span( When min, When max ) {
        //    var difference = max - min; // difference.Value now has the total number of planckTimes since the big bang (difference.Value is a BigInteger).
        //    var bo = 5.850227064E+53;
        public static Boolean operator !=( Span t1, Span t2 ) => !Equals( t1, t2 );

        //    //BigInteger.DivRem
        //    //var  = difference % Attoseconds.One.Value;
        //}
        /// <summary>
        ///     <para>Given the <paramref name="left" /> <see cref="Span" />,</para>
        ///     <para>add (+) the <paramref name="right" /> <see cref="Span" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Span operator +( Span left, Span right ) => Combine( left, right );

        public static Boolean operator <( Span left, Span right ) => left.TotalPlanckTimes < right.TotalPlanckTimes;

        public static Boolean operator <=( Span left, Span right ) => left.TotalPlanckTimes <= right.TotalPlanckTimes;

        public static Boolean operator ==( Span left, Span right ) => Equals( left, right );

        public static Boolean operator >( Span left, Span right ) => left.TotalPlanckTimes > right.TotalPlanckTimes;

        public static Boolean operator >=( Span left, Span right ) => left.TotalPlanckTimes >= right.TotalPlanckTimes;

        ///// <summary>
        ///// Allow a known cast to <see cref="TimeSpan"/>.
        ///// </summary>
        ///// <param name="span"></param>
        ///// <returns></returns>
        //public static explicit operator TimeSpan( Span span ) {
        //    return new TimeSpan( ( int )span.Days.Value, ( int )span.Hours.Value, ( int )span.Minutes.Value, ( int )span.Seconds.Value, ( int )span.Milliseconds.Value );
        //}
        /// <summary>
        ///     assume seconds given
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Span TryParse( [CanBeNull] String text ) {
            try {
                if ( null == text ) {
                    return Zero;
                }
                text = text.Trim();
                if ( text.IsNullOrWhiteSpace() ) {
                    return Zero;
                }

                TimeSpan result;
                if ( TimeSpan.TryParse( text, out result ) ) {
                    return new Span( result ); //cheat and use the existing TimeSpan parsing code...
                }

                Decimal units;
                if ( text.IsJustNumbers( out units ) ) {
                    return new Span( seconds: units ); //assume seconds given
                }

                if ( text.EndsWith( "milliseconds", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "milliseconds" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( milliseconds: units );
                    }
                }

                if ( text.EndsWith( "millisecond", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "millisecond" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( milliseconds: units );
                    }
                }

                if ( text.EndsWith( "seconds", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "seconds" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( seconds: units );
                    }
                }

                if ( text.EndsWith( "second", StringComparison.InvariantCultureIgnoreCase ) ) {
                    text = text.Before( "second" );
                    if ( text.IsJustNumbers( out units ) ) {
                        return new Span( seconds: units );
                    }
                }

                //TODO parse for more, even multiple  2 days, 3 hours, and 4 minutes etc...
            }
            catch ( ArgumentNullException ) {
            }
            catch ( ArgumentException ) {
            }
            catch ( OverflowException ) {
            }
            return Zero;
        }

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

            bigSeconds += this.Months.ToYears().ToDays().ToHours().ToMinutes().ToSeconds().Value;
            if ( bigSeconds >= MaximumUsefulDecimal ) {
                goto display;
            }

            bigSeconds += this.Years.ToDays().ToHours().ToMinutes().ToSeconds().Value;
            if ( bigSeconds >= MaximumUsefulDecimal ) {
            }

        display:
            if ( bigSeconds >= MaximumUsefulDecimal ) {

                //  ||||
                bigSeconds = MaximumUsefulDecimal; // HACK VVVV
            }
            var asSeconds = new Seconds( ( Decimal )bigSeconds );
            return String.Format( "{0} seconds", asSeconds );
        }

        public int CompareTo( Span other ) => CompareTo( this, other );

        public int CompareTo( TimeSpan other ) => CompareTo( this, new Span( other ) );

        public Boolean Equals( Span obj ) => Equals( this, obj );

        /// <summary>
        ///     Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        /// <filterpriority>2</filterpriority>
        public override Boolean Equals( [CanBeNull] object obj ) {
            if ( ReferenceEquals( null, obj ) ) {
                return false;
            }
            return obj is Span && Equals( this, ( Span )obj );
        }

        /// <summary>
        ///     <para>Return a <see cref="TimeSpan" />'s worth of <see cref="Milliseconds" />.</para>
        ///     <para>
        ///         <see cref="Days" />+<see cref="Hours" />+<see cref="Minutes" />+<see cref="Seconds" />+
        ///         <see cref="Milliseconds" />+<see cref="Microseconds" />+<see cref="Nanoseconds" />
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
            return ( Double )mill.Value;
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => this.PlanckTimes.GetHashMerge( this.Yoctoseconds.GetHashMerge( this.Zeptoseconds.GetHashMerge( this.Attoseconds.GetHashMerge( this.Femtoseconds.GetHashMerge( this.Picoseconds.GetHashMerge( this.Nanoseconds.GetHashMerge( this.Microseconds.GetHashMerge( this.Milliseconds.GetHashMerge( this.Seconds.GetHashMerge( this.Minutes.GetHashMerge( this.Hours.GetHashMerge( this.Days.GetHashMerge( this.Weeks.GetHashMerge( this.Months.GetHashMerge( this.Years ) ) ) ) ) ) ) ) ) ) ) ) ) ) );

        /// <summary>
        ///     <para>Returns a <see cref="BigInteger" /> of all the whole (integer) years in this <see cref="Span" />.</para>
        /// </summary>
        public BigInteger GetWholeYears() {
            var span = new Span( this.TotalPlanckTimes );
            return ( BigInteger ) span.Years.Value;
        }

        Boolean IEquatable<Span>.Equals( Span other ) => this.Equals( obj: other );

        [Pure]
        public override String ToString() {
            var bob = new Queue<String>( 20 );

            if ( this.Years.Value != Decimal.Zero ) {
                bob.Enqueue( this.Years.ToString() );
            }
            if ( this.Months.Value != Decimal.Zero ) {
                bob.Enqueue( this.Months.ToString() );
            }
            if ( this.Weeks.Value != Decimal.Zero ) {
                bob.Enqueue( this.Weeks.ToString() );
            }
            if ( this.Days.Value != Decimal.Zero ) {
                bob.Enqueue( this.Days.ToString() );
            }
            if ( this.Hours.Value != Decimal.Zero ) {
                bob.Enqueue( this.Hours.ToString() );
            }
            if ( this.Minutes.Value != Decimal.Zero ) {
                bob.Enqueue( this.Minutes.ToString() );
            }
            if ( this.Seconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Seconds.ToString() );
            }
            if ( this.Milliseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Milliseconds.ToString() );
            }
            if ( this.Microseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Microseconds.ToString() );
            }
            if ( this.Nanoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Nanoseconds.ToString() );
            }
            if ( this.Picoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Picoseconds.ToString() );
            }
            if ( this.Femtoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Femtoseconds.ToString() );
            }
            if ( this.Attoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Attoseconds.ToString() );
            }
            if ( this.Zeptoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Zeptoseconds.ToString() );
            }
            if ( this.Yoctoseconds.Value != Decimal.Zero ) {
                bob.Enqueue( this.Yoctoseconds.ToString() );
            }
            if ( this.PlanckTimes.Value != BigInteger.Zero ) {
                bob.Enqueue( this.PlanckTimes.ToString() );
            }

            return bob.ToStrings( ", ", ", and " );
        }
    }
}