// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "TimeExtensions.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Time;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using ExtendedNumerics;
using Logging;
using Maths;
using Parsing;
using PooledAwait;
using Utilities;

public static class TimeExtensions {

	/// <summary>
	///     The ISO 8601 format string.
	///     <remarks>
	///         <span>Doesn't make a good filename because of the :</span>
	///     </remarks>
	/// </summary>
	public const String Iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

	private static TimeSpan? AverageTimePrecision;

	public static DateTime StarDateOrigin { get; } = new(2318, 7, 5, 12, 0, 0, DateTimeKind.Utc);

	private static DateTime ParseFormattedDate( String? input, IFormatProvider? culture ) {
		var formats = new[] {
			"u", "s", "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-dd HH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:sszzzzzz",
			"M/d/yyyy h:mm:ss tt" // default format for invariant culture
		};

		if ( DateTime.TryParseExact( input, formats, culture, DateTimeStyles.None, out var date ) ) {
			return date;
		}

		return DateTime.TryParse( input, culture, DateTimeStyles.None, out date ) ? date : default( DateTime );
	}

	public static DateTime Ago( this DateTime dateTime, TimeSpan timeSpan ) => dateTime - timeSpan;

	/// <summary>
	///     Returns the given <see cref="DateTime" /> with hour and minutes set At given values.
	/// </summary>
	/// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
	/// <param name="hour">   The hour to set time to.</param>
	/// <param name="minute"> The minute to set time to.</param>
	/// <returns><see cref="DateTime" /> with hour and minute set to given values.</returns>
	public static DateTime At( this DateTime current, Int32 hour, Int32 minute ) => current.SetTime( hour, minute );

	/// <summary>
	///     Returns the given <see cref="DateTime" /> with hour and minutes and seconds set At given values.
	/// </summary>
	/// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
	/// <param name="hour">   The hour to set time to.</param>
	/// <param name="minute"> The minute to set time to.</param>
	/// <param name="second"> The second to set time to.</param>
	/// <returns><see cref="DateTime" /> with hour and minutes and seconds set to given values.</returns>
	public static DateTime At( this DateTime current, Int32 hour, Int32 minute, Int32 second ) => current.SetTime( hour, minute, second );

	/// <summary>
	///     Returns the given <see cref="DateTime" /> with hour and minutes and seconds and milliseconds set At given values.
	/// </summary>
	/// <param name="current">     The current <see cref="DateTime" /> to be changed.</param>
	/// <param name="hour">        The hour to set time to.</param>
	/// <param name="minute">      The minute to set time to.</param>
	/// <param name="second">      The second to set time to.</param>
	/// <param name="milliseconds">The milliseconds to set time to.</param>
	/// <returns><see cref="DateTime" /> with hour and minutes and seconds set to given values.</returns>
	public static DateTime At( this DateTime current, Int32 hour, Int32 minute, Int32 second, Int32 milliseconds ) => current.SetTime( hour, minute, second, milliseconds );

	public static DateTime Average( this IEnumerable<DateTime> dates ) {
		if ( dates is null ) {
			throw new ArgumentEmptyException( nameof( dates ) );
		}

		var ticks = dates.Select( time => time.Ticks ).Average();

		return new DateTime( ( Int64 ) ticks );
	}

	/// <summary>
	///     The fastest time a task context switch should take?
	/// </summary>
	public static async PooledTask<TimeSpan> AwaitContextSwitch() {
		var stopwatch = Stopwatch.StartNew();

		await Task.Run( async () => await Task.Delay( 1 ).ConfigureAwait( false ) ).ConfigureAwait( false );

		return stopwatch.Elapsed;
	}

	/// <summary>
	///     Returns the Start of the given <paramref name="date" />.
	/// </summary>
	/// <param name="date"></param>
	public static DateTime BeginningOfDay( this DateTime date ) => new(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);

	public static Boolean Between( this DateTime dt, DateTime rangeBeg, DateTime rangeEnd ) => rangeBeg <= dt && dt <= rangeEnd;

	public static TimeSpan DateTimePrecision() {
		Int64 now;
		var then = DateTime.UtcNow.Ticks;

		do {
			now = DateTime.UtcNow.Ticks;
		} while ( then == now );

		return TimeSpan.FromTicks( now - then );
	}

	/// <summary>
	///     Example: Console.WriteLine( 3.Days().FromNow() );
	/// </summary>
	/// <param name="days"></param>
	public static TimeSpan Days( this Double days ) => TimeSpan.FromDays( days );

	/// <summary>
	///     Example: Console.WriteLine( 3.Days().FromNow() );
	/// </summary>
	/// <param name="days"></param>
	public static TimeSpan Days( this Int32 days ) => TimeSpan.FromDays( days );

	/// <summary>
	///     Decreases the <see cref="DateTime" /> by given <see cref="TimeSpan" /> value.
	/// </summary>
	public static DateTime DecreaseTime( this DateTime startDate, TimeSpan toSubtract ) => startDate - toSubtract;

	/// <summary>
	///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
	/// </summary>
	/// <param name="timeSpan"></param>
	/// <param name="scalar">  </param>
	public static TimeSpan Divide( this TimeSpan timeSpan, Double scalar ) => TimeSpan.FromTicks( ( Int64 ) ( timeSpan.Ticks / scalar ) );

	/// <summary>
	///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
	/// </summary>
	/// <param name="timeSpan"></param>
	/// <param name="scalar">  </param>
	public static TimeSpan Divide( this TimeSpan timeSpan, Int64 scalar ) => TimeSpan.FromTicks( timeSpan.Ticks / scalar );

	/// <summary>
	///     Reduce a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
	/// </summary>
	/// <param name="timeSpan"></param>
	/// <param name="scalar">  </param>
	public static TimeSpan Divide( this TimeSpan timeSpan, Decimal scalar ) => TimeSpan.FromTicks( ( Int64 ) ( timeSpan.Ticks / scalar ) );

	/// <summary>
	///     <para>Returns the last millisecond of the given <paramref name="date" />.</para>
	/// </summary>
	/// <param name="date"></param>
	public static DateTime EndOfDay( this DateTime date ) => new(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);

	/// <summary>
	///     Return a quick estimation of the time remaining [on a download for example].
	/// </summary>
	/// <param name="timeElapsed">Time elapsed so far.</param>
	/// <param name="progress">   Progress done so far from 0.0 to 1.0</param>
	public static TimeSpan EstimateTimeRemaining( this TimeSpan timeElapsed, Decimal progress ) {
		const Decimal max = 1m;

		if ( progress < max.Epsilon() ) {
			progress = max.Epsilon();
		}
		else if ( progress > max ) {
			progress = max;
		}

		var milliseconds = ( Decimal ) timeElapsed.TotalMilliseconds; // example: 5 seconds elapsed so far
		var remainingTime = milliseconds / progress - milliseconds; // should be 15 seconds ( 20 - 5)

		return TimeSpan.FromMilliseconds( ( Double ) remainingTime );
	}

	public static DateTime ExtractDate( this String input, String pattern, IFormatProvider? culture ) {
		var dt = DateTime.MinValue;
		var regex = new Regex( pattern );

		if ( !regex.IsMatch( input ) ) {
			return dt;
		}

		var matches = regex.Matches( input );
		var match = matches[ 0 ];
		var ms = Convert.ToInt64( match.Groups[ 1 ].Value );

		dt = DateTime.UnixEpoch.AddMilliseconds( ms );

		// adjust if time zone modifier present
		if ( match.Groups.Count <= 2 || String.IsNullOrEmpty( match.Groups[ 3 ].Value ) ) {
			return dt;
		}

		var mod = DateTime.ParseExact( match.Groups[ 3 ].Value, "HHmm", culture );
		dt = match.Groups[ 2 ].Value == "+" ? dt.Add( mod.TimeOfDay ) : dt.Subtract( mod.TimeOfDay );

		return dt;
	}

	//public static int Comparison( this Minutes minutes, Milliseconds milliseconds ) {
	//    var left = minutes.Value;
	//    var right = new Minutes( milliseconds: milliseconds ).Value;
	//    return left.CompareTo( right );
	//}
	/// <summary>
	///     Sets the day of the <see cref="DateTime" /> to the first day in that month.
	/// </summary>
	/// <param name="current">The current <see cref="DateTime" /> to be changed.</param>
	/// <returns>given <see cref="DateTime" /> with the day part set to the first day in that month.</returns>
	public static DateTime FirstDayOfMonth( this DateTime current ) => current.SetDay( 1 );

	public static DateTime FirstDayOfTheMonth( this DateTime date ) => new(date.Year, date.Month, 1);

	/// <summary>
	///     Returns a DateTime adjusted to the beginning of the week.
	/// </summary>
	/// <param name="dateTime">The DateTime to adjust</param>
	/// <returns>A DateTime instance adjusted to the beginning of the current week</returns>
	/// <remarks>the beginning of the week is controlled by the current Culture</remarks>
	public static DateTime FirstDayOfWeek( this DateTime dateTime ) {
		var currentCulture = CultureInfo.CurrentCulture;
		var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
		var offset = dateTime.DayOfWeek - firstDayOfWeek < 0 ? 7 : 0;
		var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - firstDayOfWeek;

		return dateTime.AddDays( -numberOfDaysSinceBeginningOfTheWeek );
	}

	/// <summary>
	///     Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 =&gt;
	///     2011-01-01T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTime to adjust</param>
	public static DateTime FirstDayOfYear( this DateTime current ) => current.SetDate( current.Year, 1, 1 );

	public static DateTime From( this DateTime dateTime, TimeSpan timeSpan ) => dateTime + timeSpan;

	/// <summary>
	///     Example: Console.WriteLine( 3.Days().FromNow() );
	/// </summary>
	/// <param name="timeSpan"></param>
	public static DateTime FromNow( this TimeSpan timeSpan ) => DateTime.UtcNow.Add( timeSpan );

	/// <summary>
	///     returns seconds since 1970-01-01 as a <see cref="DateTime" />.
	/// </summary>
	/// <param name="timestamp"></param>
	public static DateTime FromUNIXTimestamp( this UInt64 timestamp ) => DateTime.UnixEpoch.AddSeconds( timestamp );

	/// <summary>
	///     returns seconds since 1970-01-01 as a <see cref="DateTime" />.
	/// </summary>
	/// <param name="timestamp"></param>
	public static DateTime FromUNIXTimestamp( this Int32 timestamp ) => DateTime.UnixEpoch.AddSeconds( timestamp );

	/// <summary>
	///     returns seconds since 1970-01-01 as a <see cref="DateTime" />.
	/// </summary>
	/// <param name="timestamp"></param>
	public static DateTime FromUNIXTimestamp( this Int64 timestamp ) => DateTime.UnixEpoch.AddSeconds( timestamp );

	/// <summary>
	///     Return how many years old the person is in <see cref="Years" />.
	/// </summary>
	/// <param name="dateOfBirth"></param>
	public static Years GetAge( this DateTime dateOfBirth ) {
		//this seems to work for 99% of cases, but it still feels hacky.
		//what about leap-year birthdays?
		//what about other calendars?
		var today = DateTime.Today;

		var a = ( today.Year * 100 + today.Month ) * 100 + today.Day;
		var b = ( dateOfBirth.Year * 100 + dateOfBirth.Month ) * 100 + dateOfBirth.Day;

		return new Years( ( a - b ) / 10000 );
	}

	public static Int32 GetQuarter( this DateTime date ) => ( date.Month - 1 ) / 3 + 1;

	public static TimeSpan GetTimePrecision() {
		if ( AverageTimePrecision.HasValue ) {
			return AverageTimePrecision.Value;
		}

		$"Performing {Environment.ProcessorCount} timeslice calibrations.".Verbose();
		AverageTimePrecision = new Milliseconds( 0.To( Environment.ProcessorCount ).Select( i => DateTimePrecision() ).Average( span => span.TotalMilliseconds ) );
		$"Average datetime precision is {AverageTimePrecision.GetValueOrDefault( Time.Milliseconds.One ).Simpler()}.".Verbose();

		return AverageTimePrecision.GetValueOrDefault( Time.Milliseconds.One );
	}

	/// <summary>
	///     Accurate to within how many nanoseconds?
	/// </summary>
	public static Int64 GetTimerAccuracy() => 1000000000L / Stopwatch.Frequency;

	/// <summary>
	///     Example: Console.WriteLine( 3.Hours().FromNow() );
	/// </summary>
	/// <param name="hours"></param>
	public static TimeSpan Hours( this Double hours ) => TimeSpan.FromHours( hours );

	/// <summary>
	///     Example: Console.WriteLine( 3.Hours().FromNow() );
	/// </summary>
	/// <param name="hours"></param>
	public static TimeSpan Hours( this Int32 hours ) => TimeSpan.FromHours( hours );

	/// <summary>
	///     Increases the <see cref="DateTime" /> object with given <see cref="TimeSpan" /> value.
	/// </summary>
	public static DateTime IncreaseTime( this DateTime startDate, TimeSpan toAdd ) => startDate + toAdd;

	/// <summary>
	///     Determines whether the specified <see cref="DateTime" /> value is After then current value.
	/// </summary>
	/// <param name="current">      The current value.</param>
	/// <param name="toCompareWith">Value to compare with.</param>
	/// <returns><c>true</c> if the specified current is after; otherwise, <c>false</c>.</returns>
	public static Boolean IsAfter( this DateTime current, DateTime toCompareWith ) => current > toCompareWith;

	/// <summary>
	///     Determines whether the specified <see cref="DateTime" /> is before then current value.
	/// </summary>
	/// <param name="current">      The current value.</param>
	/// <param name="toCompareWith">Value to compare with.</param>
	/// <returns><c>true</c> if the specified current is before; otherwise, <c>false</c>.</returns>
	public static Boolean IsBefore( this DateTime current, DateTime toCompareWith ) => current < toCompareWith;

	/// <summary>
	///     Determine if a <see cref="DateTime" /> is in the future.
	/// </summary>
	/// <param name="dateTime">The date to be checked.</param>
	/// <returns><c>true</c> if <paramref name="dateTime" /> is in the future; otherwise <c>false</c>.</returns>
	public static Boolean IsInFuture( this DateTime dateTime ) => dateTime.ToUniversalTime() > DateTime.UtcNow;

	/// <summary>
	///     Determine if a <see cref="DateTime" /> is in the past.
	/// </summary>
	/// <param name="dateTime">The date to be checked.</param>
	/// <returns><c>true</c> if <paramref name="dateTime" /> is in the past; otherwise <c>false</c>.</returns>
	public static Boolean IsInPast( this DateTime dateTime ) => dateTime.ToUniversalTime() < DateTime.UtcNow;

	/// <summary>
	///     <para>Determines if the specified year is a leap year.</para>
	/// </summary>
	/// <param name="year">Year to test.</param>
	/// <copyright>
	///     Tommy
	///     Dugger & Jared Chavez
	/// </copyright>
	public static Boolean IsLeapYear( this Int64 year ) {
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

	public static Boolean IsWeekend( this DateTime date ) => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

	public static Boolean IsWorkingDay( this DateTime date ) => date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;

	/// <summary>
	///     Sets the day of the <see cref="DateTime" /> to the last day in that month.
	/// </summary>
	/// <param name="current">The current DateTime to be changed.</param>
	/// <returns>given <see cref="DateTime" /> with the day part set to the last day in that month.</returns>
	public static DateTime LastDayOfMonth( this DateTime current ) => current.SetDay( DateTime.DaysInMonth( current.Year, current.Month ) );

	public static DateTime LastDayOfTheMonth( this DateTime date ) => new(date.Year, date.Month, DateTime.DaysInMonth( date.Year, date.Month ));

	/// <summary>
	///     Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 =&gt;
	///     2011-12-25T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTime to adjust</param>
	public static DateTime LastDayOfWeeek( this DateTime current ) => current.FirstDayOfWeek().AddDays( 6 );

	/// <summary>
	///     untested.
	/// </summary>
	/// <param name="date"></param>
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
	///     Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 =&gt;
	///     2011-12-31T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTime to adjust</param>
	public static DateTime LastDayOfYear( this DateTime current ) => current.SetDate( current.Year, 12, 31 );

	/// <summary>
	///     Adds the given number of business days to the <see cref="DateTime" />.
	/// </summary>
	/// <param name="current">The date to be changed.</param>
	/// <returns>A <see cref="DateTime" /> increased by a given number of business days.</returns>
	public static void MakeNextBusinessDay( ref this DateTime current ) {
		while ( current.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ) {
			current += TimeSpan.FromDays( 1 );
		}
	}

	/// <summary>
	///     Returns original <see cref="DateTime" /> value with time part set to midnight (alias for
	///     <see cref="BeginningOfDay" /> method).
	/// </summary>
	public static DateTime Midnight( this DateTime value ) => value.BeginningOfDay();

	/// <summary>
	///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
	/// </summary>
	/// <param name="milliseconds"></param>
	public static TimeSpan Milliseconds( this Int64 milliseconds ) => TimeSpan.FromMilliseconds( milliseconds );

	/// <summary>
	///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
	/// </summary>
	/// <param name="milliseconds"></param>
	public static TimeSpan Milliseconds( this Double milliseconds ) => TimeSpan.FromMilliseconds( milliseconds );

	/// <summary>
	///     Example: Console.WriteLine( 3.Milliseconds().FromNow() );
	/// </summary>
	/// <param name="milliseconds"></param>
	public static TimeSpan Milliseconds( this Milliseconds milliseconds ) => milliseconds;

	/// <summary>
	///     Example: Console.WriteLine( 3.Minutes().FromNow() );
	/// </summary>
	/// <param name="minutes"></param>
	public static TimeSpan Minutes( this Int32 minutes ) => TimeSpan.FromMinutes( minutes );

	/// <summary>
	///     Example: Console.WriteLine( 3.Minutes().FromNow() );
	/// </summary>
	/// <param name="minutes"></param>
	public static TimeSpan Minutes( this Double minutes ) => TimeSpan.FromMinutes( minutes );

	/// <summary>
	///     Multiplies a timespan by an integer value
	/// </summary>
	public static TimeSpan Multiply( this TimeSpan multiplicand, Int64 multiplier ) => TimeSpan.FromTicks( multiplicand.Ticks * multiplier );

	/// <summary>
	///     Multiplies a timespan by a double value
	/// </summary>
	public static TimeSpan Multiply( this TimeSpan multiplicand, Double multiplier ) => TimeSpan.FromTicks( ( Int64 ) ( multiplicand.Ticks * multiplier ) );

	/// <summary>
	///     Multiplies a timespan by a decimal value
	/// </summary>
	public static TimeSpan Multiply( this TimeSpan multiplicand, Decimal multiplier ) => TimeSpan.FromTicks( ( Int64 ) ( multiplicand.Ticks * multiplier ) );

	/// <summary>
	///     Multiplies a timespan by an integer value
	/// </summary>
	public static TimeSpan Multiply( this TimeSpan multiplicand, Int32 multiplier ) => TimeSpan.FromTicks( multiplicand.Ticks * multiplier );

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
	///     Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005 = &gt;
	///     2013-01-05T06:40:20.005 If the next month doesn't have that many days
	///     the last day of the next month is used. Eg, 2013-01-31T06:40:20.005 = &gt; 2013-02-28T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTime to adjust</param>
	public static DateTime NextMonth( this DateTime current ) {
		var year = current.Month == 12 ? current.Year + 1 : current.Year;

		var month = current.Month == 12 ? 1 : current.Month + 1;

		var firstDayOfNextMonth = current.SetDate( year, month, 1 );

		var lastDayOfPreviousMonth = firstDayOfNextMonth.LastDayOfMonth().Day;

		var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

		return firstDayOfNextMonth.SetDay( day );
	}

	public static DateTime NextWorkday( this DateTime date ) {
		var nextDay = date.AddDays( 1 );
		while ( !nextDay.IsWorkingDay() ) {
			nextDay = nextDay.AddDays( 1 );
		}

		return nextDay;
	}

	/// <summary>
	///     Returns the same date (same Day, Month, Hour, Minute, Second etc) in the next calendar year. If that day does not
	///     exist in next year in same month, number of
	///     missing days is added to the last day in same month next year.
	/// </summary>
	public static DateTime NextYear( this DateTime start ) {
		var nextYear = start.Year + 1;
		var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth( nextYear, start.Month );

		if ( numberOfDaysInSameMonthNextYear >= start.Day ) {
			return new DateTime( nextYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );
		}

		var differenceInDays = start.Day - numberOfDaysInSameMonthNextYear;
		var dateTime = new DateTime( nextYear, start.Month, numberOfDaysInSameMonthNextYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );

		return dateTime + differenceInDays.Days();
	}

	/// <summary>
	///     Returns original <see cref="DateTime" /> value with time part set to Noon (12:00:00h).
	/// </summary>
	/// <param name="value">The <see cref="DateTime" /> find Noon for.</param>
	/// <returns>A <see cref="DateTime" /> value with time part set to Noon (12:00:00h).</returns>
	public static DateTime Noon( this DateTime value ) => value.SetTime( 12, 0, 0, 0 );

	/// <summary>
	///     Converts the specified ISO 8601 representation of a date and time to its DateTime equivalent.
	/// </summary>
	/// <param name="value">The ISO 8601 string representation to parse.</param>
	/// <returns>The DateTime equivalent.</returns>
	public static DateTime ParseIso8601( this String value ) =>
		DateTime.ParseExact( value, Iso8601Format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal );

	/// <summary>
	///     Parses most common JSON date formats
	/// </summary>
	/// <param name="input">  JSON value to parse</param>
	/// <param name="culture"></param>
	/// <returns>DateTime</returns>
	public static DateTime ParseJsonDate( this String input, CultureInfo? culture ) {
		if ( String.IsNullOrWhiteSpace( input ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( input ) );
		}

		input = input.Replace( "\n", "" );
		input = input.Replace( "\r", "" );

		input = input.RemoveSurroundingQuotes();

		if ( Int64.TryParse( input, out var unix ) ) {
			return DateTime.UnixEpoch.AddSeconds( unix );
		}

		if ( input.Contains( "/Date(" ) ) {
			return input.ExtractDate( @"\\?/Date\((-?\d+)(-|\+)?([0-9]{4})?\)\\?/", culture );
		}

		if ( !input.Contains( "new Date(" ) ) {
			return ParseFormattedDate( input, culture );
		}

		input = input.Replace( " ", "" );

		// because all whitespace is removed, match against newDate( instead of new Date(
		return input.ExtractDate( @"newDate\((-?\d+)*\)", culture );
	}

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
	///     Returns the previous month keeping the time component intact. Eg, 2010-01-20T06:40:20.005 =&gt;
	///     2009-12-20T06:40:20.005 If the previous month doesn't have that
	///     many days the last day of the previous month is used. Eg, 2009-03-31T06:40:20.005 = &gt; 2009-02-28T06:40:20.005
	/// </summary>
	/// <param name="current">The DateTime to adjust</param>
	public static DateTime PreviousMonth( this DateTime current ) {
		var year = current.Month == 1 ? current.Year - 1 : current.Year;

		var month = current.Month == 1 ? 12 : current.Month - 1;

		var firstDayOfPreviousMonth = current.SetDate( year, month, 1 );

		var lastDayOfPreviousMonth = firstDayOfPreviousMonth.LastDayOfMonth().Day;

		var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

		return firstDayOfPreviousMonth.SetDay( day );
	}

	/// <summary>
	///     Returns the same date (same Day, Month, Hour, Minute, Second etc) in the previous calendar year. If that day does
	///     not exist in previous year in same month, number
	///     of missing days is added to the last day in same month previous year.
	/// </summary>
	public static DateTime PreviousYear( this DateTime start ) {
		var previousYear = start.Year - 1;
		var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth( previousYear, start.Month );

		if ( numberOfDaysInSameMonthPreviousYear >= start.Day ) {
			return new DateTime( previousYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );
		}

		var differenceInDays = start.Day - numberOfDaysInSameMonthPreviousYear;

		var dateTime = new DateTime( previousYear, start.Month, numberOfDaysInSameMonthPreviousYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind );

		return dateTime + differenceInDays.Days();
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
				throw new ArgumentOutOfRangeException( nameof( rt ) );
			}
		}

		return rounded;
	}

	/// <summary>
	///     Example: Console.WriteLine( 3.Seconds().FromNow() );
	/// </summary>
	/// <param name="seconds"></param>
	public static TimeSpan Seconds( this Int32 seconds ) => TimeSpan.FromSeconds( seconds );

	/// <summary>
	///     Example: Console.WriteLine( 3.Seconds().FromNow() );
	/// </summary>
	/// <param name="seconds"></param>
	public static TimeSpan Seconds( this Double seconds ) => TimeSpan.FromSeconds( seconds );

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Year part.
	/// </summary>
	public static DateTime SetDate( this DateTime value, Int32 year ) =>
		new(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Year and Month part.
	/// </summary>
	public static DateTime SetDate( this DateTime value, Int32 year, Int32 month ) =>
		new(year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Year, Month and Day part.
	/// </summary>
	public static DateTime SetDate( this DateTime value, Int32 year, Int32 month, Int32 day ) =>
		new(year, month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Day part.
	/// </summary>
	public static DateTime SetDay( this DateTime value, Int32 day ) =>
		new(value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Hour part.
	/// </summary>
	public static DateTime SetHour( this DateTime originalDate, Int32 hour ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Millisecond part.
	/// </summary>
	public static DateTime SetMillisecond( this DateTime originalDate, Int32 millisecond ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, originalDate.Second, millisecond, originalDate.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Minute part.
	/// </summary>
	public static DateTime SetMinute( this DateTime originalDate, Int32 minute ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Month part.
	/// </summary>
	public static DateTime SetMonth( this DateTime value, Int32 month ) =>
		new(value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Second part.
	/// </summary>
	public static DateTime SetSecond( this DateTime originalDate, Int32 second ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, second, originalDate.Millisecond, originalDate.Kind);

	/// <summary>
	///     Returns the original <see cref="DateTime" /> with Hour part changed to supplied hour parameter.
	/// </summary>
	public static DateTime SetTime( this DateTime originalDate, Int32 hour ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

	/// <summary>
	///     Returns the original <see cref="DateTime" /> with Hour and Minute parts changed to supplied hour and minute
	///     parameters.
	/// </summary>
	public static DateTime SetTime( this DateTime originalDate, Int32 hour, Int32 minute ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

	/// <summary>
	///     Returns the original <see cref="DateTime" /> with Hour, Minute and Second parts changed to supplied hour, minute
	///     and second parameters.
	/// </summary>
	public static DateTime SetTime( this DateTime originalDate, Int32 hour, Int32 minute, Int32 second ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, originalDate.Millisecond, originalDate.Kind);

	/// <summary>
	///     Returns the original <see cref="DateTime" /> with Hour, Minute, Second and Millisecond parts changed to supplied
	///     hour, minute, second and millisecond parameters.
	/// </summary>
	public static DateTime SetTime( this DateTime originalDate, Int32 hour, Int32 minute, Int32 second, Int32 millisecond ) =>
		new(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, millisecond, originalDate.Kind);

	/// <summary>
	///     Returns <see cref="DateTime" /> with changed Year part.
	/// </summary>
	public static DateTime SetYear( this DateTime value, Int32 year ) =>
		new(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

	/// <summary>
	///     Display a <see cref="TimeSpan" /> in simpler terms. ie "2 hours 4 minutes 33 seconds".
	/// </summary>
	/// <param name="timeSpan"></param>
	public static String Simpler( this TimeSpan timeSpan ) {
		var length = 0;

		if ( timeSpan.Days.Any() ) {
			length += 2 * ( timeSpan.Days.ToString().Length + " years ".Length );
		}

		if ( timeSpan.Hours.Any() ) {
			length += timeSpan.Hours.ToString().Length + " hours ".Length;
		}

		if ( timeSpan.Minutes.Any() ) {
			length += timeSpan.Minutes.ToString().Length + " minutes ".Length;
		}

		if ( timeSpan.Seconds.Any() ) {
			length += timeSpan.Seconds.ToString().Length + " seconds ".Length;
		}

		if ( timeSpan.Milliseconds.Any() ) {
			length += timeSpan.Milliseconds.ToString().Length + " milliseconds ".Length;
		}

		var sb = new StringBuilder( length, length );

		if ( timeSpan.Days > 365 * 2 ) {
			sb.AppendFormat( " {0:n0} years", timeSpan.Days / 365 );
		}
		else if ( timeSpan.Days.Between( 365, 366 ) ) {
			sb.Append( " 1 year" );
		}

		switch ( timeSpan.Days ) {
			case > 1: {
				sb.Append( $" {timeSpan.Days:R} days" );
				break;
			}
			case 1: {
				sb.Append( $" {timeSpan.Days:R} day" );
				break;
			}
		}

		switch ( timeSpan.Hours ) {
			case > 1: {
				sb.Append( $" {timeSpan.Hours:n0} hours" );
				break;
			}

			case 1: {
				sb.Append( $" {timeSpan.Hours} hour" );
				break;
			}
		}

		switch ( timeSpan.Minutes ) {
			case > 1: {
				sb.Append( $" {timeSpan.Minutes:n0} minutes" );
				break;
			}

			case 1: {
				sb.Append( $" {timeSpan.Minutes} minute" );
				break;
			}
		}

		switch ( timeSpan.Seconds ) {
			case > 1: {
				sb.Append( $" {timeSpan.Seconds:n0} seconds" );
				break;
			}

			case 1: {
				sb.Append( $" {timeSpan.Seconds} second" );
				break;
			}
		}

		switch ( timeSpan.Milliseconds ) {
			case > 1: {
				sb.Append( $" {timeSpan.Milliseconds:n0} milliseconds" );
				break;
			}
			case 1: {
				sb.Append( $" {timeSpan.Milliseconds} millisecond" );
				break;
			}
		}

		if ( String.IsNullOrEmpty( sb.ToString().Trim() ) ) {
			sb.Append( " ~0 milliseconds " );
		}

		return sb.ToString().Trim();
	}

	/// <summary>
	///     Display a <see cref="Duration" /> in simpler terms. ie "2 hours 4 minutes 33 seconds".
	/// </summary>
	/// <param name="duration"></param>
	public static String Simpler( this Duration duration ) {
		var sb = new StringBuilder();

		if ( BigDecimal.Abs( duration.Years() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Years() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} year" : " {0:R} years", duration.Years() );
		}

		if ( BigDecimal.Abs( duration.Weeks() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Weeks() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} week" : " {0:R} weeks", duration.Weeks() );
		}

		if ( BigDecimal.Abs( duration.Days() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Days() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} day" : " {0:R} days", duration.Days() );
		}

		if ( BigDecimal.Abs( duration.Hours() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Hours() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} hour" : " {0:R} hours", duration.Hours() );
		}

		if ( BigDecimal.Abs( duration.Minutes() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Minutes() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} minute" : " {0:R} minutes", duration.Minutes() );
		}

		if ( BigDecimal.Abs( duration.Seconds() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Seconds() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} second" : " {0:R} seconds", duration.Seconds() );
		}

		if ( BigDecimal.Abs( duration.Milliseconds() ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Milliseconds() - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} millisecond" : " {0:R} milliseconds",
				duration.Milliseconds() );
		}

		if ( BigDecimal.Abs( duration.Microseconds ) >= BigDecimal.One.BigEpsilon() ) {
			sb.AppendFormat( BigDecimal.Abs( duration.Microseconds - 1 ) <= BigDecimal.One.BigEpsilon() ? " {0:R} microsecond" : " {0:R} microseconds",
				duration.Microseconds );
		}

		if ( String.IsNullOrEmpty( sb.ToString().Trim() ) ) {
			sb.Append( " 0 microseconds " );
		}

		return sb.ToString().Trim();
	}

	/// <summary>
	///     <para>
	///         Throws an <see cref="OverflowException" /> if the <paramref name="value" /> is lower than
	///         <see cref="Decimal.MinValue" /> or higher than
	///         <see
	///             cref="Decimal.MaxValue" />
	///         .
	///     </para>
	/// </summary>
	/// <param name="value"></param>
	public static void ThrowIfOutOfDecimalRange( this Double value ) {
		if ( value < ( Double ) Decimal.MinValue ) {
			throw new OverflowException( Constants.ValueIsTooLow );
		}

		if ( value > ( Double ) Decimal.MaxValue ) {
			throw new OverflowException( Constants.ValueIsTooHigh );
		}
	}

	/// <summary>
	///     The fastest time a task context switch should take?
	/// </summary>
	public static TimeSpan TimeAThreadSwitch() {
		var stopwatch = Stopwatch.StartNew();
		Thread.Sleep( 1 );

		return stopwatch.Elapsed;
	}

	/// <summary>
	///     Increase a <see cref="TimeSpan" /> by a <paramref name="scalar" /> amount.
	/// </summary>
	/// <param name="timeSpan"></param>
	/// <param name="scalar">  </param>
	public static TimeSpan Times( this TimeSpan timeSpan, Double scalar ) => TimeSpan.FromTicks( ( Int64 ) ( timeSpan.Ticks * scalar ) );

	// if ( value < Constants.MinimumUsefulDecimal ) { throw new OverflowException( Constants.ValueIsTooLow ); }
	public static SpanOfTime TimeStatement( this Action? action ) {
		var one = Stopwatch.StartNew();

		try {
			action?.Invoke();
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return new SpanOfTime( one.Elapsed );
	}

	/// <summary>
	///     Formats the date in the standard ISO 8601 format.
	/// </summary>
	/// <param name="value">The date to format.</param>
	/// <returns>The formatted date.</returns>
	public static String ToIso8601( this DateTime value ) => value.ToUniversalTime().ToString( Iso8601Format, CultureInfo.CurrentCulture );

	public static String ToPath( this DateTime dateTime ) {
		var sb = new StringBuilder( String.Empty, 20 );
		sb.Append( $"{dateTime.Year:D}/" );
		sb.Append( $"{dateTime.Month:D}/" );
		sb.Append( $"{dateTime.Day:D}/" );
		sb.Append( $"{dateTime.Hour:D}/" );
		sb.Append( $"{dateTime.Minute:D}/" );
		sb.Append( $"{dateTime.Second:D}/" );
		sb.Append( $"{dateTime.Millisecond:D}/" );

		return sb.ToString();
	}

	public static SpanOfTime ToSpanOfTime( this Date date ) {
		( var year, var month, var day ) = date;

		return new(years: new Years( year.Value ), months: new Months( month.Value ), days: new Days( day.Value ));
	}

	public static Decimal ToStarDate( this DateTime earthDateTime ) {
		var earthToStarDateDiff = earthDateTime - StarDateOrigin;
		var millisecondConversion = ( Decimal ) earthToStarDateDiff.TotalMilliseconds / 34367056.4m;
		var starDate = Math.Floor( millisecondConversion * 100 ) / 100;

		return Math.Round( starDate, 2, MidpointRounding.AwayFromZero );
	}

	/// <summary>
	///     Seconds since 1970-01-01
	/// </summary>
	/// <param name="date"></param>
	public static Int64 ToUnixTimestamp( this DateTime date ) {
		var diff = date - DateTime.UnixEpoch;

		return ( Int64 ) diff.TotalSeconds;
	}

	public static Boolean TryConvertToDateTime( this Date date, out DateTime? dateTime ) {
		try {
			( var year, var month, var day ) = date;

			if ( year.Value.Between( DateTime.MinValue.Year, DateTime.MaxValue.Year ) ) {
				dateTime = new DateTime( ( Int32 ) year.Value, ( Int32 ) month.Value, ( Int32 ) day.Value );

				return true;
			}
		}
		catch ( ArgumentOutOfRangeException ) { }

		dateTime = null;

		return false;
	}

	/// <summary>
	///     Return how many years old the person is in <see cref="Years" />.
	/// </summary>
	/// <param name="dateOfBirth"></param>
	public static Years YearsFrom( this DateTime dateOfBirth ) => new Seconds( ( DateTime.UtcNow - dateOfBirth ).TotalSeconds ).ToYears();

}