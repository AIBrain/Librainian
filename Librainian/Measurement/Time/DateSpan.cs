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
// File "DateSpan.cs" last formatted on 2022-12-22 at 3:48 AM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using Extensions;

/// <summary>
///     A struct similar to <see cref="TimeSpan" /> that stores the elapsed time between two dates, but does so in a way
///     that
///     respects the number of actual days in the elapsed years and months.
/// </summary>
/// <remarks>
///     Adapted from <see cref="http://github.com/danielcrenna/vault/blob/master/dates/src/Dates/DateSpan.cs" />
/// </remarks>
[Immutable]
[Serializable]
public struct DateSpan {

	/// <summary>
	/// </summary>
	/// <param name="start">The start date</param>
	/// <param name="end">The end date</param>
	/// <param name="excludeEndDate">If true, the span is exclusive of the end date</param>
	public DateSpan( DateTime start, DateTime end, Boolean excludeEndDate = true ) : this() {
		this.Initialize( start, end, excludeEndDate );
	}

	/// <summary>
	/// </summary>
	/// <param name="start">The start date</param>
	/// <param name="end">The end date</param>
	/// <param name="excludeEndDate">If true, the span is exclusive of the end date</param>
	public DateSpan( DateTimeOffset start, DateTimeOffset end, Boolean excludeEndDate = true ) : this() {
		this.Initialize( start.DateTime, end.DateTime, excludeEndDate );
	}

	/// <summary>
	///     The number of discrete days occurring in this span
	/// </summary>
	public Int32 Days { get; private set; }

	/// <summary>
	///     The number of discrete hours occurring in this span
	/// </summary>
	public Int32 Hours { get; private set; }

	/// <summary>
	///     The number of discrete minutes occurring in this span
	/// </summary>
	public Int32 Minutes { get; private set; }

	/// <summary>
	///     The number of discrete months occurring in this span
	/// </summary>
	public Int32 Months { get; private set; }

	/// <summary>
	///     The number of discrete seconds occurring in this span
	/// </summary>
	public Int32 Seconds { get; private set; }

	/// <summary>
	///     The number of discrete weeks occurring in this span
	/// </summary>
	public Int32 Weeks { get; private set; }

	/// <summary>
	///     The number of discrete years occurring in this span
	/// </summary>
	public Int32 Years { get; private set; }

	private static Int64 CalculateDifference( DateInterval interval, DateTime start, DateTime end, Boolean excludeEndDate ) {
		Int64 sum = 0;
		var span = new DateSpan( start, end );

		var differenceInDays = GetDifferenceInDays( start, span, excludeEndDate );

		switch ( interval ) {
			case DateInterval.Years:
				sum += span.Years;
				break;

			case DateInterval.Months:
				if ( span.Years > 0 ) {
					sum += span.Years * 12;
				}

				sum += span.Months;
				sum += span.Weeks / 4; // Helps resolve lower resolution
				break;

			case DateInterval.Weeks:
				sum = differenceInDays / 7;
				break;

			case DateInterval.Days:
				sum = differenceInDays;
				break;

			case DateInterval.Hours:
				sum = differenceInDays * 24;
				sum += span.Hours;
				break;

			case DateInterval.Minutes:
				sum = differenceInDays * 24 * 60;
				sum += span.Hours * 60;
				sum += span.Minutes;
				break;

			case DateInterval.Seconds:
				sum = differenceInDays * 24 * 60 * 60;
				sum += span.Hours * 60 * 60;
				sum += span.Minutes * 60;
				sum += span.Seconds;
				break;

			default:
				throw new ArgumentOutOfRangeException( nameof( interval ) );
		}

		return sum;
	}

	private static Int64 GetDifferenceInDays( DateTime start, DateSpan span, Boolean excludeEndDate = true ) {
		var sum = 0;

		if ( span.Years > 0 ) {
			for ( var i = 0; i < span.Years; i++ ) {
				var year = start.Year + i;
				sum += DateTime.IsLeapYear( year ) ? 366 : 365;
			}
		}

		if ( span.Months > 0 ) {
			for ( var i = 1; i <= span.Months; i++ ) {
				var month = start.Month + i;
				if ( month >= 13 ) {
					month -= 12;
				}

				sum += DateTime.DaysInMonth( start.Year + span.Years, month );
			}
		}

		sum += span.Weeks * 7;

		sum += span.Days;

		if ( excludeEndDate ) {
			sum--;
		}

		return sum;
	}

	private void CalculateDays( DateTime start, DateTime end, Boolean excludeEndDate = false ) {
		this.Days = end.Day - start.Day;

		if ( end.Day < start.Day ) {
			this.Days = DateTime.DaysInMonth( start.Year, start.Month ) - start.Day + end.Day;
		}

		if ( this.Days <= 0 ) {
			return;
		}

		if ( end.Hour < start.Hour ) {
			this.Days--;
		}
		else if ( end.Hour == start.Hour ) {
			if ( end.Minute >= start.Minute ) {
				if ( end.Minute == start.Minute && end.Second < start.Second ) {
					this.Days--;
				}
			}
			else {
				this.Days--;
			}
		}

		this.Weeks = this.Days / 7;

		this.Days = this.Days % 7;

		if ( !excludeEndDate ) {
			this.Days++;
		}
	}

	private void CalculateHours( DateTime start, DateTime end ) {
		this.Hours = end.Hour - start.Hour;

		if ( end.Hour < start.Hour ) {
			this.Hours = 24 - start.Hour + end.Hour;
		}

		if ( this.Hours <= 0 ) {
			return;
		}

		if ( end.Minute >= start.Minute ) {
			if ( end.Minute != start.Minute || end.Second >= start.Second ) {
				return;
			}

			this.Hours--;
		}
		else {
			this.Hours--;
		}
	}

	private void CalculateMinutes( DateTime start, DateTime end ) {
		this.Minutes = end.Minute - start.Minute;

		if ( end.Minute < start.Minute ) {
			this.Minutes = 60 - start.Minute + end.Minute;
		}

		if ( this.Minutes <= 0 ) {
			return;
		}

		if ( end.Second < start.Second ) {
			this.Minutes--;
		}
	}

	private void CalculateMonths( DateTime start, DateTime end ) {
		this.Months = end.Month - start.Month;

		if ( end.Month < start.Month || end.Month <= start.Month && this.Years > 1 ) {
			this.Months = 12 - start.Month + end.Month;
		}

		if ( this.Months <= 0 ) {
			return;
		}

		if ( end.Day < start.Day ) {
			this.Months--;
		}
		else if ( end.Day == start.Day ) {
			if ( end.Hour < start.Hour ) {
				this.Months--;
			}
			else if ( end.Hour == start.Hour ) {
				if ( end.Minute >= start.Minute ) {
					if ( end.Minute != start.Minute || end.Second >= start.Second ) {
						return;
					}

					this.Months--;
				}
				else {
					this.Months--;
				}
			}
		}
	}

	private void CalculateSeconds( DateTime start, DateTime end ) {
		this.Seconds = end.Second - start.Second;

		if ( end.Second < start.Second ) {
			this.Seconds = 60 - start.Second + end.Second;
		}
	}

	private void CalculateYears( DateTime start, DateTime end ) {
		this.Years = end.Year - start.Year;

		if ( this.Years <= 0 ) {
			return;
		}

		if ( end.Month < start.Month ) {
			this.Years--;
		}
		else if ( end.Month == start.Month ) {
			if ( end.Day >= start.Day ) {
				if ( end.Day != start.Day ) {
					return;
				}

				if ( end.Hour < start.Hour ) {
					this.Years--;
				}
				else if ( end.Hour == start.Hour ) {
					if ( end.Minute >= start.Minute ) {
						if ( end.Minute != start.Minute ) {
							return;
						}

						if ( end.Second >= start.Second ) {
							return;
						}

						this.Years--;
					}
					else {
						this.Years--;
					}
				}
			}
			else {
				this.Years--;
			}
		}
	}

	private void Initialize( DateTime start, DateTime end, Boolean excludeEndDate ) {
		start = start.ToUniversalTime();
		end = end.ToUniversalTime();

		if ( start > end ) {
			var temp = start;
			start = end;
			end = temp;
		}

		this.CalculateYears( start, end );
		this.CalculateMonths( start, end );
		this.CalculateDays( start, end, excludeEndDate );
		this.CalculateHours( start, end );
		this.CalculateMinutes( start, end );
		this.CalculateSeconds( start, end );
	}

	/// <summary>
	///     Gets the scalar difference between two dates given a <see cref="DateInterval" /> value.
	/// </summary>
	/// <param name="interval">The interval to calculate</param>
	/// <param name="start">The start date</param>
	/// <param name="end">The end date</param>
	/// <param name="excludeEndDate">If true, the difference is exclusive of the end date</param>
	/// <returns></returns>
	public static Int64 GetDifference( DateInterval interval, DateTime start, DateTime end, Boolean excludeEndDate = false ) =>
		CalculateDifference( interval, start, end, excludeEndDate );

	public static Int64 GetDifference( DateInterval interval, DateTimeOffset start, DateTimeOffset end, Boolean excludeEndDate = false ) =>
		CalculateDifference( interval, start.DateTime, end.DateTime, excludeEndDate );
}