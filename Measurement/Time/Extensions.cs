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
// "Librainian/Extensions.cs" was last cleaned by Rick on 2014/09/02 at 5:11 AM

#endregion License & Information

namespace Librainian.Measurement.Time {
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Numerics;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Maths;
    using Parsing;
    using Threading;

    public static class Extensions {

        static Extensions() {
            try {

                // Time.Milliseconds.One.Should().BeLessThan(  Time.Seconds.One );

                //Assert.That( Time.Milliseconds.One < Time.Seconds.One );
                //Assert.That( Time.Milliseconds.One < Time.Minutes.One );

                //Assert.That( Time.Seconds.One > Time.Milliseconds.One );
                //Assert.That( Time.Minutes.One > Time.Milliseconds.One );

                //Assert.That( Time.Seconds.One < Time.Minutes.One );
                //Assert.That( Time.Minutes.One > Time.Seconds.One );
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        /// <summary>
        ///     Returns the Start of the given <paramref name="date" />.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime BeginningOfDay( this DateTime date ) => new DateTime( year: date.Year, month: date.Month, day: date.Day, hour: 0, minute: 0, second: 0, millisecond: 0, kind: date.Kind );

        [Pure]
        public static BigInteger CalcTotalPlanckTimes( this Span span ) {
            var counter = BigInteger.Zero;

            // These if are a super minor optimization. The result should still calc the same.
            if ( span.PlanckTimes.Value > 0 ) {
                counter += span.PlanckTimes.ToPlanckTimes();
            }
            if ( span.Yoctoseconds.Value > 0 ) {
                counter += span.Yoctoseconds.ToPlanckTimes();
            }
            if ( span.Zeptoseconds.Value > 0 ) {
                counter += span.Zeptoseconds.ToPlanckTimes();
            }
            if ( span.Attoseconds.Value > 0 ) {
                counter += span.Attoseconds.ToPlanckTimes();
            }
            if ( span.Femtoseconds.Value > 0 ) {
                counter += span.Femtoseconds.ToPlanckTimes();
            }
            if ( span.Picoseconds.Value > 0 ) {
                counter += span.Picoseconds.ToPlanckTimes();
            }
            if ( span.Nanoseconds.Value > 0 ) {
                counter += span.Nanoseconds.ToPlanckTimes();
            }
            if ( span.Microseconds.Value > 0 ) {
                counter += span.Microseconds.ToPlanckTimes();
            }
            if ( span.Milliseconds.Value > 0 ) {
                counter += span.Milliseconds.ToPlanckTimes();
            }
            if ( span.Seconds.Value > 0 ) {
                counter += span.Seconds.ToPlanckTimes();
            }
            if ( span.Minutes.Value > 0 ) {
                counter += span.Minutes.ToPlanckTimes();
            }
            if ( span.Hours.Value > 0 ) {
                counter += span.Hours.ToPlanckTimes();
            }
            if ( span.Days.Value > 0 ) {
                counter += span.Days.ToPlanckTimes();
            }

            //if ( span.Weeks.Value > 0 ) {
            //    counter += span.Weeks.ToPlanckTimes();
            //}
            if ( span.Months.Value > 0 ) {
                counter += span.Months.ToPlanckTimes();
            }
            if ( span.Years.Value > 0 ) {
                counter += span.Years.ToPlanckTimes();
            }
            return counter;
        }

        /// <summary>
        ///     Example: Console.WriteLine( 3.Days().FromNow() );
        /// </summary>
        /// <param name="days"> </param>
        /// <returns> </returns>
        public static TimeSpan Days( this Double days ) => TimeSpan.FromDays( days );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Days().FromNow() );
        /// </summary>
        /// <param name="days"> </param>
        /// <returns> </returns>
        public static TimeSpan Days( this int days ) => TimeSpan.FromDays( days );

        /// <summary>
        ///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static TimeSpan Divided( this TimeSpan timeSpan, Double scalar ) => TimeSpan.FromTicks( ( long )( timeSpan.Ticks / scalar ) );

        /// <summary>
        ///     <para>Returns the last millisecond of the given <paramref name="date" />.</para>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay( this DateTime date ) => new DateTime( year: date.Year, month: date.Month, day: date.Day, hour: 23, minute: 59, second: 59, millisecond: 999, kind: date.Kind );

        //public static int Comparison( this Minutes minutes, Milliseconds milliseconds ) {
        //    var lhs = minutes.Value;
        //    var rhs = new Minutes( milliseconds: milliseconds ).Value;
        //    return lhs.CompareTo( rhs );
        //}
        /// <summary>
        ///     Return a quick estimation of the time remaining [on a download for example].
        /// </summary>
        /// <param name="timeElapsed">Time elapsed so far.</param>
        /// <param name="progress">Progress done so far from 0.0 to 1.0</param>
        /// <returns></returns>
        public static TimeSpan EstimateTimeRemaining( TimeSpan timeElapsed, Double progress ) {
            if ( progress <= Double.Epsilon ) {
                progress = Double.Epsilon;
            }
            else if ( progress >= 1.0 ) {
                progress = 1.0;
            }

            var milliseconds = timeElapsed.TotalMilliseconds; // example: 5 seconds elapsed so far
            var remainingTime = ( milliseconds / progress ) - milliseconds; // should be 15 seconds ( 20 - 5)
            return TimeSpan.FromMilliseconds( value: remainingTime );
        }

        public static DateTime FirstDayOfTheMonth( this DateTime date ) => new DateTime( year: date.Year, month: date.Month, day: 1 );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Days().FromNow() );
        /// </summary>
        /// <param name="timeSpan"> </param>
        /// <returns> </returns>
        public static DateTime FromNow( this TimeSpan timeSpan ) => DateTime.UtcNow.Add( timeSpan );

        /// <summary>
        ///     returns seconds since 1970-01-01 as a <see cref="DateTime" />.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromUNIXTimestamp( this UInt64 timestamp ) => Epochs.UNIX.AddSeconds( timestamp );

        /// <summary>
        ///     returns seconds since 1970-01-01 as a <see cref="DateTime" />.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromUNIXTimestamp( this Int32 timestamp ) => Epochs.UNIX.AddSeconds( timestamp );

        /// <summary>
        ///     returns seconds since 1970-01-01 as a <see cref="DateTime" />.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromUNIXTimestamp( this long timestamp ) => Epochs.UNIX.AddSeconds( timestamp );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Hours().FromNow() );
        /// </summary>
        /// <param name="hours"> </param>
        /// <returns> </returns>
        public static TimeSpan Hours( this Double hours ) => TimeSpan.FromHours( hours );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Hours().FromNow() );
        /// </summary>
        /// <param name="hours"> </param>
        /// <returns> </returns>
        public static TimeSpan Hours( this int hours ) => TimeSpan.FromHours( hours );

        /// <summary>
        ///     <para>Determines if the specified year is a leap year.</para>
        /// </summary>
        /// <returns></returns>
        /// <param name="year">Year to test.</param>
        /// <copyright>Tommy Dugger & Jared Chavez</copyright>
        public static Boolean IsLeapYear( this long year ) {

            // not divisible by 4? not a leap year
            if ( year % 4 != 0 ) {
                return false;
            }

            // divisible by 4 and not divisible by 100? always a leap year
            if ( year % 100 != 0 ) {
                return true;
            }

            // divisible by 4 and 100? Only a leap year if also divisible by 400
            return year % 400 == 0;
        }

        public static DateTime LastDayOfTheMonth( this DateTime date ) => new DateTime( year: date.Year, month: date.Month, day: DateTime.DaysInMonth( year: date.Year, month: date.Month ) );

        /// <summary>
        ///     untested.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek( this DateTime date ) {
            var month = date.Month;
            while ( month == date.Month && date.DayOfWeek != DayOfWeek.Saturday ) {
                date = date.AddDays( 1 );
            }
            if ( date.Month != month ) {
                date = date.AddDays( -1 );
            }

            return date;
        }

        /// <summary>
        ///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
        /// </summary>
        /// <param name="milliseconds"> </param>
        /// <returns> </returns>
        public static TimeSpan Milliseconds( this Int64 milliseconds ) => TimeSpan.FromMilliseconds( milliseconds );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
        /// </summary>
        /// <param name="milliseconds"> </param>
        /// <returns> </returns>
        public static TimeSpan Milliseconds( this Double milliseconds ) => TimeSpan.FromMilliseconds( milliseconds );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
        /// </summary>
        /// <param name="milliseconds"> </param>
        /// <returns> </returns>
        public static TimeSpan Milliseconds( this Milliseconds milliseconds ) => milliseconds;

        /// <summary>
        ///     Example: Console.WriteLine( 3.Minutes().FromNow() );
        /// </summary>
        /// <param name="minutes"> </param>
        /// <returns> </returns>
        public static TimeSpan Minutes( this int minutes ) => TimeSpan.FromMinutes( minutes );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Minutes().FromNow() );
        /// </summary>
        /// <param name="minutes"> </param>
        /// <returns> </returns>
        public static TimeSpan Minutes( this Double minutes ) => TimeSpan.FromMinutes( minutes );

        /// <summary>
        ///     Given the <paramref name="constant" />, reduce <paramref name="planckTimes" />, and return the amount(integer)
        ///     reduced.
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static BigInteger PullPlancks( this BigInteger constant, ref BigInteger planckTimes ) {
            //if ( planckTimes < constant ) {
            //    return BigInteger.Zero;
            //}
            var integer = BigInteger.Divide( planckTimes, constant );
            planckTimes -= BigInteger.Multiply( integer, constant );
            return integer;
        }

        /// <summary>
        ///     Example: Console.WriteLine( 3.Seconds().FromNow() );
        /// </summary>
        /// <param name="seconds"> </param>
        /// <returns> </returns>
        public static TimeSpan Seconds( this int seconds ) => TimeSpan.FromSeconds( seconds );

        /// <summary>
        ///     Example: Console.WriteLine( 3.Seconds().FromNow() );
        /// </summary>
        /// <param name="seconds"> </param>
        /// <returns> </returns>
        public static TimeSpan Seconds( this Double seconds ) => TimeSpan.FromSeconds( seconds );

        /// <summary>
        ///     Display a <see cref="TimeSpan" /> in simpler terms. ie "2 hours 4 minutes 33 seconds".
        /// </summary>
        /// <param name="ts"> </param>
        public static String Simpler( this TimeSpan ts ) {

            var span = ( Span ) ts;
            return span.ToString();

            //var sb = new StringBuilder();

            //if ( ts.Days > ( 365 * 2 ) ) {
            //    sb.AppendFormat( " {0:n0} years", ts.Days / 365 );
            //}
            //else if ( ts.Days > 365 ) {
            //    sb.AppendFormat( " {0} year", ts.Days / 365 );
            //}
            //else if ( ts.Days > 14 ) {
            //    sb.AppendFormat( " {0:n0} weeks", ts.Days / 7 );
            //}
            //else if ( ts.Days > 7 ) {
            //    sb.AppendFormat( " {0} week", ts.Days / 7 );
            //}
            //else if ( ts.Days > 1 ) {
            //    sb.AppendFormat( " {0:n0} days", ts.Days );
            //}
            //else if ( ts.Days == 1 ) {
            //    sb.AppendFormat( " {0} day", ts.Days );
            //}

            //if ( ts.Hours > 1 ) {
            //    sb.AppendFormat( " {0:n0} hours", ts.Hours );
            //}
            //else if ( ts.Hours == 1 ) {
            //    sb.AppendFormat( " {0} hour", ts.Hours );
            //}

            //if ( ts.Minutes > 1 ) {
            //    sb.AppendFormat( " {0:n0} minutes", ts.Minutes );
            //}
            //else if ( ts.Minutes == 1 ) {
            //    sb.AppendFormat( " {0} minute", ts.Minutes );
            //}

            //if ( ts.Seconds > 1 ) {
            //    sb.AppendFormat( " {0:n0} seconds", ts.Seconds );
            //}
            //else if ( ts.Seconds == 1 ) {
            //    sb.AppendFormat( " {0} second", ts.Seconds );
            //}

            //if ( ts.Milliseconds > 1 ) {
            //    sb.AppendFormat( " {0:n0} milliseconds", ts.Milliseconds );
            //}
            //else if ( ts.Milliseconds == 1 ) {
            //    sb.AppendFormat( " {0} millisecond", ts.Milliseconds );
            //}

            //if ( String.IsNullOrEmpty( sb.ToString().Trim() ) ) {
            //    sb.Append( " 0 milliseconds " );
            //}

            //return sb.ToString().Trim();
        }

        /// <summary>
        ///     <para>
        ///         Throws an <see cref="OverflowException" /> if the <paramref name="value" /> is lower than
        ///         <see cref="Decimal.MinValue" /> or higher than <see cref="Decimal.MaxValue" />.
        ///     </para>
        /// </summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public static void ThrowIfOutOfDecimalRange( this BigInteger value ) {
            value.Should().BeInRange( Constants.MinimumUsefulDecimal, Constants.MaximumUsefulDecimal );

            if ( value < Constants.MinimumUsefulDecimal ) {
                throw new OverflowException( Constants.ValueIsTooLow );
            }

            if ( value > Constants.MaximumUsefulDecimal ) {
                throw new OverflowException( Constants.ValueIsTooHigh );
            }
        }

        /// <summary>
        ///     <para>
        ///         Throws an <see cref="OverflowException" /> if the <paramref name="value" /> is lower than
        ///         <see cref="Decimal.MinValue" /> or higher than <see cref="Decimal.MaxValue" />.
        ///     </para>
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfOutOfDecimalRange( this Double value ) {
            value.Should().BeInRange( ( Double )Decimal.MinValue, ( Double )Decimal.MaxValue );

            if ( value < ( Double )Decimal.MinValue ) {
                throw new OverflowException( Constants.ValueIsTooLow );
            }

            if ( value > ( Double )Decimal.MaxValue ) {
                throw new OverflowException( Constants.ValueIsTooHigh );
            }
        }

        /// <summary>
        ///     Increase a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static TimeSpan Times( this TimeSpan timeSpan, Double scalar ) => TimeSpan.FromTicks( value: ( Int64 )( timeSpan.Ticks * scalar ) );

        public static Span TimeStatement( [CanBeNull] this Action action ) {
            var one = Stopwatch.StartNew();
            try {
                if ( null != action ) {
                    action();
                }
            }
            catch ( Exception exception ) {
                exception.More();
            }
            return new Span( one.Elapsed );
        }

        public static DateTime Ago( this DateTime dateTime, TimeSpan timeSpan ) => dateTime - timeSpan;

        public static String ToPath( this DateTime dateTime ) {
            var sb = new StringBuilder( String.Empty, 24 );
            sb.AppendFormat( "{0:D}/", dateTime.Year );
            sb.AppendFormat( "{0:D}/", dateTime.Month );
            sb.AppendFormat( "{0:D}/", dateTime.Day );
            sb.AppendFormat( "{0:D}/", dateTime.Hour );
            sb.AppendFormat( "{0:D}/", dateTime.Minute );
            sb.AppendFormat( "{0:D}/", dateTime.Second );
            sb.AppendFormat( "{0:D}/", dateTime.Millisecond );
            return sb.ToString();
        }

        public static Span ToSpan( this Date date ) {
            var span = Span.Zero;
            span += new Years( date.Year );
            span += new Months( ( Decimal )date.Month.Value );
            span += new Days( date.Day.Value );
            return span;
        }

        /// <summary>
        ///     Seconds since 1970-01-01
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ulong ToUNIXTimestamp( this DateTime date ) {
            var diff = date - Epochs.UNIX;
            return ( ulong )diff.TotalSeconds;
        }

        public static Boolean TryConvertToDateTime( this Date date, out DateTime? dateTime ) {
            try {
                if ( date.Year.Value.Between( DateTime.MinValue.Year, DateTime.MaxValue.Year ) ) {
                    dateTime = new DateTime( year: ( int )date.Year.Value, month: date.Month.Value, day: date.Day.Value );
                    return true;
                }
            }
            catch ( ArgumentOutOfRangeException ) {
            }
            dateTime = null;
            return false;
        }

        #region untested code pulled from https://github.com/FluentDateTime/FluentDateTime/blob/master/FluentDateTime/DateTime/DateTimeExtensions.cs

        /// <summary>
        ///     Adds the given number of business days to the <see cref="DateTime" />.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns>A <see cref="DateTime" /> increased by a given number of business days.</returns>
        public static DateTime AddBusinessDays( this DateTime current, int days ) {
            var sign = Math.Sign( days );
            var unsignedDays = Math.Abs( days );
            for ( var i = 0 ; i < unsignedDays ; i++ ) {
                do {
                    current = current.AddDays( sign );
                } while ( current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday );
            }
            return current;
        }

        /// <summary>
        ///     Returns the given <see cref="DateTime" /> with hour and minutes set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <returns><see cref="DateTime" /> with hour and minute set to given values.</returns>
        public static DateTime At( this DateTime current, int hour, int minute ) => current.SetTime( hour, minute );

        /// <summary>
        ///     Returns the given <see cref="DateTime" /> with hour and minutes and seconds set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <returns><see cref="DateTime" /> with hour and minutes and seconds set to given values.</returns>
        public static DateTime At( this DateTime current, int hour, int minute, int second ) => current.SetTime( hour, minute, second );

        /// <summary>
        ///     Returns the given <see cref="DateTime" /> with hour and minutes and seconds and milliseconds set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <param name="milliseconds">The milliseconds to set time to.</param>
        /// <returns><see cref="DateTime" /> with hour and minutes and seconds set to given values.</returns>
        public static DateTime At( this DateTime current, int hour, int minute, int second, int milliseconds ) => current.SetTime( hour, minute, second, milliseconds );

        /// <summary>
        ///     Decreases the <see cref="DateTime" /> object with given <see cref="TimeSpan" /> value.
        /// </summary>
        public static DateTime DecreaseTime( this DateTime startDate, TimeSpan toSubtract ) => startDate - toSubtract;

        /// <summary>
        ///     Sets the day of the <see cref="DateTime" /> to the first day in that month.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
        /// <returns>given <see cref="DateTime" /> with the day part set to the first day in that month.</returns>
        public static DateTime FirstDayOfMonth( this DateTime current ) => current.SetDay( 1 );

        /// <summary>
        ///     Returns a DateTime adjusted to the beginning of the week.
        /// </summary>
        /// <param name="dateTime">The DateTime to adjust</param>
        /// <returns>A DateTime instance adjusted to the beginning of the current week</returns>
        /// <remarks>the beginning of the week is controlled by the current Culture</remarks>
        public static DateTime FirstDayOfWeek( this DateTime dateTime ) {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
            var offset = dateTime.DayOfWeek - firstDayOfWeek < 0 ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - firstDayOfWeek;

            return dateTime.AddDays( -numberOfDaysSinceBeginningOfTheWeek );
        }

        /// <summary>
        ///     Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 =>
        ///     2011-01-01T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime FirstDayOfYear( this DateTime current ) => current.SetDate( current.Year, 1, 1 );

        /// <summary>
        ///     Increases the <see cref="DateTime" /> object with given <see cref="TimeSpan" /> value.
        /// </summary>
        public static DateTime IncreaseTime( this DateTime startDate, TimeSpan toAdd ) => startDate + toAdd;

        /// <summary>
        ///     Determines whether the specified <see cref="DateTime" /> value is After then current value.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="toCompareWith">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified current is after; otherwise, <c>false</c>.
        /// </returns>
        public static Boolean IsAfter( this DateTime current, DateTime toCompareWith ) => current > toCompareWith;

        /// <summary>
        ///     Determines whether the specified <see cref="DateTime" /> is before then current value.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="toCompareWith">Value to compare with.</param>
        /// <returns>
        ///     <c>true</c> if the specified current is before; otherwise, <c>false</c>.
        /// </returns>
        public static Boolean IsBefore( this DateTime current, DateTime toCompareWith ) => current < toCompareWith;

        /// <summary>
        ///     Determine if a <see cref="DateTime" /> is in the future.
        /// </summary>
        /// <param name="dateTime">The date to be checked.</param>
        /// <returns><c>true</c> if <paramref name="dateTime" /> is in the future; otherwise <c>false</c>.</returns>
        public static Boolean IsInFuture( this DateTime dateTime ) => dateTime > DateTime.Now;

        /// <summary>
        ///     Determine if a <see cref="DateTime" /> is in the past.
        /// </summary>
        /// <param name="dateTime">The date to be checked.</param>
        /// <returns><c>true</c> if <paramref name="dateTime" /> is in the past; otherwise <c>false</c>.</returns>
        public static Boolean IsInPast( this DateTime dateTime ) => dateTime < DateTime.Now;

        /// <summary>
        ///     Sets the day of the <see cref="DateTime" /> to the last day in that month.
        /// </summary>
        /// <param name="current">The current DateTime to be changed.</param>
        /// <returns>given <see cref="DateTime" /> with the day part set to the last day in that month.</returns>
        public static DateTime LastDayOfMonth( this DateTime current ) => current.SetDay( DateTime.DaysInMonth( current.Year, current.Month ) );

        /// <summary>
        ///     Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 =>
        ///     2011-12-25T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeeek( this DateTime current ) => current.FirstDayOfWeek().AddDays( 6 );

        /// <summary>
        ///     Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 =>
        ///     2011-12-31T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime LastDayOfYear( this DateTime current ) => current.SetDate( current.Year, 12, 31 );

        /// <summary>
        ///     Returns original <see cref="DateTime" /> value with time part set to midnight (alias for
        ///     <see cref="BeginningOfDay" /> method).
        /// </summary>
        public static DateTime Midnight( this DateTime value ) => value.BeginningOfDay();

        /// <summary>
        ///     Returns first next occurrence of specified <see cref="DayOfWeek" />.
        /// </summary>
        public static DateTime Next( this DateTime start, DayOfWeek day ) {
            do {
                start = start.NextDay();
            } while ( start.DayOfWeek != day );

            return start;
        }

        /// <summary>
        ///     Returns <see cref="DateTime" /> increased by 24 hours ie Next Day.
        /// </summary>
        public static DateTime NextDay( this DateTime start ) => start + 1.Days();

        /// <summary>
        ///     Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005 => 2013-01-05T06:40:20.005
        ///     If the next month doesn't have that many days the last day of the next month is used. Eg, 2013-01-31T06:40:20.005
        ///     => 2013-02-28T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime NextMonth( this DateTime current ) {
            var year = current.Month == 12 ? current.Year + 1 : current.Year;

            var month = current.Month == 12 ? 1 : current.Month + 1;

            var firstDayOfNextMonth = current.SetDate( year, month, 1 );

            var lastDayOfPreviousMonth = firstDayOfNextMonth.LastDayOfMonth().Day;

            var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

            return firstDayOfNextMonth.SetDay( day );
        }

        /// <summary>
        ///     Returns the same date (same Day, Month, Hour, Minute, Second etc) in the next calendar year.
        ///     If that day does not exist in next year in same month, number of missing days is added to the last day in same
        ///     month next year.
        /// </summary>
        public static DateTime NextYear( this DateTime start ) {
            var nextYear = start.Year + 1;
            var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth( nextYear, start.Month );

            if ( numberOfDaysInSameMonthNextYear < start.Day ) {
                var differenceInDays = start.Day - numberOfDaysInSameMonthNextYear;
                var dateTime = new DateTime( nextYear, start.Month, numberOfDaysInSameMonthNextYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );
                return dateTime + differenceInDays.Days();
            }
            return new DateTime( nextYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );
        }

        /// <summary>
        ///     Returns original <see cref="DateTime" /> value with time part set to Noon (12:00:00h).
        /// </summary>
        /// <param name="value">The <see cref="DateTime" /> find Noon for.</param>
        /// <returns>A <see cref="DateTime" /> value with time part set to Noon (12:00:00h).</returns>
        public static DateTime Noon( this DateTime value ) => value.SetTime( 12, 0, 0, 0 );

        /// <summary>
        ///     Returns first next occurrence of specified <see cref="DayOfWeek" />.
        /// </summary>
        public static DateTime Previous( this DateTime start, DayOfWeek day ) {
            do {
                start = start.PreviousDay();
            } while ( start.DayOfWeek != day );

            return start;
        }

        /// <summary>
        ///     Returns <see cref="DateTime" /> decreased by 24h period ie Previous Day.
        /// </summary>
        public static DateTime PreviousDay( this DateTime start ) => start - 1.Days();

        /// <summary>
        ///     Returns the previous month keeping the time component intact. Eg, 2010-01-20T06:40:20.005 =>
        ///     2009-12-20T06:40:20.005
        ///     If the previous month doesn't have that many days the last day of the previous month is used. Eg,
        ///     2009-03-31T06:40:20.005 => 2009-02-28T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime PreviousMonth( this DateTime current ) {
            var year = current.Month == 1 ? current.Year - 1 : current.Year;

            var month = current.Month == 1 ? 12 : current.Month - 1;

            var firstDayOfPreviousMonth = current.SetDate( year, month, 1 );

            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.LastDayOfMonth().Day;

            var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

            return firstDayOfPreviousMonth.SetDay( day );
        }

        /// <summary>
        ///     Returns the same date (same Day, Month, Hour, Minute, Second etc) in the previous calendar year.
        ///     If that day does not exist in previous year in same month, number of missing days is added to the last day in same
        ///     month previous year.
        /// </summary>
        public static DateTime PreviousYear( this DateTime start ) {
            var previousYear = start.Year - 1;
            var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth( previousYear, start.Month );

            if ( numberOfDaysInSameMonthPreviousYear < start.Day ) {
                var differenceInDays = start.Day - numberOfDaysInSameMonthPreviousYear;
                var dateTime = new DateTime( previousYear, start.Month, numberOfDaysInSameMonthPreviousYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );
                return dateTime + differenceInDays.Days();
            }
            return new DateTime( previousYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );
        }

        public static DateTime Round( this DateTime dateTime, RoundTo rt ) {
            DateTime rounded;

            switch ( rt ) {
                case RoundTo.Second: {
                        rounded = new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind );
                        if ( dateTime.Millisecond >= 500 ) {
                            rounded = rounded.AddSeconds( 1 );
                        }
                        break;
                    }
                case RoundTo.Minute: {
                        rounded = new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind );
                        if ( dateTime.Second >= 30 ) {
                            rounded = rounded.AddMinutes( 1 );
                        }
                        break;
                    }
                case RoundTo.Hour: {
                        rounded = new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind );
                        if ( dateTime.Minute >= 30 ) {
                            rounded = rounded.AddHours( 1 );
                        }
                        break;
                    }
                case RoundTo.Day: {
                        rounded = new DateTime( dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind );
                        if ( dateTime.Hour >= 12 ) {
                            rounded = rounded.AddDays( 1 );
                        }
                        break;
                    }
                default: {
                        throw new ArgumentOutOfRangeException( "rt" );
                    }
            }

            return rounded;
        }

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Year part.
        /// </summary>
        public static DateTime SetDate( this DateTime value, int year ) => new DateTime( year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Year and Month part.
        /// </summary>
        public static DateTime SetDate( this DateTime value, int year, int month ) => new DateTime( year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Year, Month and Day part.
        /// </summary>
        public static DateTime SetDate( this DateTime value, int year, int month, int day ) => new DateTime( year, month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Day part.
        /// </summary>
        public static DateTime SetDay( this DateTime value, int day ) => new DateTime( value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Hour part.
        /// </summary>
        public static DateTime SetHour( this DateTime originalDate, int hour ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Millisecond part.
        /// </summary>
        public static DateTime SetMillisecond( this DateTime originalDate, int millisecond ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, originalDate.Second, millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Minute part.
        /// </summary>
        public static DateTime SetMinute( this DateTime originalDate, int minute ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Month part.
        /// </summary>
        public static DateTime SetMonth( this DateTime value, int month ) => new DateTime( value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Second part.
        /// </summary>
        public static DateTime SetSecond( this DateTime originalDate, int second ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, second, originalDate.Millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns the original <see cref="DateTime" /> with Hour part changed to supplied hour parameter.
        /// </summary>
        public static DateTime SetTime( this DateTime originalDate, int hour ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns the original <see cref="DateTime" /> with Hour and Minute parts changed to supplied hour and minute
        ///     parameters.
        /// </summary>
        public static DateTime SetTime( this DateTime originalDate, int hour, int minute ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns the original <see cref="DateTime" /> with Hour, Minute and Second parts changed to supplied hour, minute
        ///     and second parameters.
        /// </summary>
        public static DateTime SetTime( this DateTime originalDate, int hour, int minute, int second ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, originalDate.Millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns the original <see cref="DateTime" /> with Hour, Minute, Second and Millisecond parts changed to supplied
        ///     hour, minute, second and millisecond parameters.
        /// </summary>
        public static DateTime SetTime( this DateTime originalDate, int hour, int minute, int second, int millisecond ) => new DateTime( originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, millisecond, originalDate.Kind );

        /// <summary>
        ///     Returns <see cref="DateTime" /> with changed Year part.
        /// </summary>
        public static DateTime SetYear( this DateTime value, int year ) => new DateTime( year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind );

        /// <summary>
        ///     Obsolete. This method has been renamed to FirstDayOfWeek to be more consistent with existing conventions.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [Obsolete( "This method has been renamed to FirstDayOfWeek to be more consistent with existing conventions." )]
        public static DateTime StartOfWeek( this DateTime dateTime ) => FirstDayOfWeek( dateTime );

        /// <summary>
        ///     Subtracts the given number of business days to the <see cref="DateTime" />.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateTime" /> increased by a given number of business days.</returns>
        public static DateTime SubtractBusinessDays( this DateTime current, int days ) => AddBusinessDays( current, -days );

        /*

                /// <summary>
                ///     Increases supplied <see cref="DateTime" /> for 7 days ie returns the Next Week.
                /// </summary>
                public static DateTime WeekAfter( this DateTime start ) {
                    return start + 1.Weeks();
                }
        */

        /*

                /// <summary>
                ///     Decreases supplied <see cref="DateTime" /> for 7 days ie returns the Previous Week.
                /// </summary>
                public static DateTime WeekEarlier( this DateTime start ) {
                    return start - 1.Weeks();
                }
        */

        #endregion untested code pulled from https://github.com/FluentDateTime/FluentDateTime/blob/master/FluentDateTime/DateTime/DateTimeExtensions.cs

        /// <summary>
        /// Parses most common JSON date formats
        /// </summary>
        /// <param name="input">JSON value to parse</param>
        /// <param name="culture"></param>
        /// <returns>DateTime</returns>
        public static DateTime ParseJsonDate( this String input, CultureInfo culture ) {
            input = input.Replace( "\n", "" );
            input = input.Replace( "\r", "" );

            input = input.RemoveSurroundingQuotes();

            long unix;
            if ( Int64.TryParse( input, out unix ) ) {
                var epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
                return epoch.AddSeconds( unix );
            }

            if ( input.Contains( "/Date(" ) ) {
                return ExtractDate( input, @"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/", culture );
            }

            if ( !input.Contains( "new Date(" ) ) {
                return ParseFormattedDate( input, culture );
            }

            input = input.Replace( " ", "" );
            // because all whitespace is removed, match against newDate( instead of new Date(
            return ExtractDate( input, @"newDate\((-?\d+)*\)", culture );
        }

        private static DateTime ParseFormattedDate( String input, CultureInfo culture ) {
            var formats = new[] {
				"u", 
				"s", 
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", 
				"yyyy-MM-ddTHH:mm:ssZ", 
				"yyyy-MM-dd HH:mm:ssZ", 
				"yyyy-MM-ddTHH:mm:ss", 
				"yyyy-MM-ddTHH:mm:sszzzzzz",
				"M/d/yyyy h:mm:ss tt" // default format for invariant culture
			};

            DateTime date;
            if ( DateTime.TryParseExact( input, formats, culture, DateTimeStyles.None, out date ) ) {
                return date;
            }
            if ( DateTime.TryParse( input, culture, DateTimeStyles.None, out date ) ) {
                return date;
            }

            return default( DateTime );
        }

        private static DateTime ExtractDate( String input, String pattern, IFormatProvider culture ) {
            var dt = DateTime.MinValue;
            var regex = new Regex( pattern );
            if ( !regex.IsMatch( input ) ) {
                return dt;
            }
            var matches = regex.Matches( input );
            var match = matches[ 0 ];
            var ms = Convert.ToInt64( match.Groups[ 1 ].Value );
            var epoch = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            dt = epoch.AddMilliseconds( ms );

            // adjust if time zone modifier present
            if ( match.Groups.Count <= 2 || String.IsNullOrEmpty( match.Groups[ 3 ].Value ) ) {
                return dt;
            }

            var mod = DateTime.ParseExact( match.Groups[ 3 ].Value, "HHmm", culture );
            dt = match.Groups[ 2 ].Value == "+" ? dt.Add( mod.TimeOfDay ) : dt.Subtract( mod.TimeOfDay );
            return dt;
        }
    }
}