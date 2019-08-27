// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DateSpan.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "DateSpan.cs" was last formatted by Protiguous on 2019/08/08 at 8:59 AM.

namespace Librainian.Measurement.Time {

    using System;
    using Extensions;
    using Newtonsoft.Json;

    /// <summary>
    ///     A struct similar to <see cref="TimeSpan" /> that stores the elapsed time between two dates,
    ///     but does so in a way that respects the number of actual days in the elapsed years and months.
    /// </summary>
    /// <remarks>
    ///     Adapted from <see cref="http://github.com/danielcrenna/vault/blob/master/dates/src/Dates/DateSpan.cs" />
    /// </remarks>
    [Immutable]
    [JsonObject( MemberSerialization.Fields )]
    [Serializable]
    public struct DateSpan {

        /// <summary>
        ///     The number of discrete days occurring in this span
        /// </summary>
        public Int32 Days { get; }

        /// <summary>
        ///     The number of discrete hours occurring in this span
        /// </summary>
        public Int32 Hours { get; }

        /// <summary>
        ///     The number of discrete minutes occurring in this span
        /// </summary>
        public Int32 Minutes { get; }

        /// <summary>
        ///     The number of discrete months occurring in this span
        /// </summary>
        public Int32 Months { get; }

        /// <summary>
        ///     The number of discrete seconds occurring in this span
        /// </summary>
        public Int32 Seconds { get; }

        /// <summary>
        ///     The number of discrete weeks occurring in this span
        /// </summary>
        public Int32 Weeks { get; }

        /// <summary>
        ///     The number of discrete years occurring in this span
        /// </summary>
        public Int32 Years { get; }

        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <param name="excludeEndDate">If true, the span is exclusive of the end date</param>
        public DateSpan( DateTime start, DateTime end, Boolean excludeEndDate = true ) : this() {
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();

            if ( start > end ) {
                var temp = start;
                start = end;
                end = temp;
            }

            {
                this.Years = end.Year - start.Year;

                if ( this.Years <= 0 ) { }
                else if ( end.Month < start.Month ) {
                    this.Years--;
                }
                else if ( end.Month == start.Month ) {
                    if ( end.Day >= start.Day ) {
                        if ( end.Day != start.Day ) { }
                        else if ( end.Hour < start.Hour ) {
                            this.Years--;
                        }
                        else if ( end.Hour == start.Hour ) {
                            if ( end.Minute >= start.Minute ) {
                                if ( end.Minute != start.Minute || end.Second >= start.Second ) { }
                                else {
                                    this.Years--;
                                }
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

            {
                this.Months = end.Month - start.Month;

                if ( end.Month < start.Month || (end.Month <= start.Month && this.Years > 1) ) {
                    this.Months = 12 - start.Month + end.Month;
                }

                if ( this.Months <= 0 ) { }
                else if ( end.Day < start.Day ) {
                    this.Months--;
                }
                else if ( end.Day == start.Day ) {
                    if ( end.Hour < start.Hour ) {
                        this.Months--;
                    }
                    else if ( end.Hour == start.Hour ) {
                        if ( end.Minute >= start.Minute ) {
                            if ( end.Minute != start.Minute || end.Second >= start.Second ) { }
                            else {
                                this.Months--;
                            }
                        }
                        else {
                            this.Months--;
                        }
                    }
                }
            }

            {
                this.Days = end.Day - start.Day;

                if ( end.Day < start.Day ) {
                    this.Days = DateTime.DaysInMonth( start.Year, start.Month ) - start.Day + end.Day;
                }

                if ( this.Days > 0 ) {
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
            }

            {
                this.Hours = end.Hour - start.Hour;

                if ( end.Hour < start.Hour ) {
                    this.Hours = 24 - start.Hour + end.Hour;
                }

                if ( this.Hours <= 0 ) { }
                else if ( end.Minute >= start.Minute ) {
                    if ( end.Minute != start.Minute || end.Second >= start.Second ) { }
                    else {
                        this.Hours--;
                    }
                }
                else {
                    this.Hours--;
                }
            }

            {
                this.Minutes = end.Minute - start.Minute;

                if ( end.Minute < start.Minute ) {
                    this.Minutes = 60 - start.Minute + end.Minute;
                }

                if ( this.Minutes <= 0 || end.Second >= start.Second ) { }
                else {
                    this.Minutes--;
                }
            }

            {
                this.Seconds = end.Second - start.Second;

                if ( end.Second < start.Second ) {
                    this.Seconds = 60 - start.Second + end.Second;
                }
            }
        }

        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <param name="excludeEndDate">If true, the span is exclusive of the end date</param>
        public DateSpan( DateTimeOffset start, DateTimeOffset end, Boolean excludeEndDate = true ) : this( start.DateTime, end.DateTime, excludeEndDate ) { }

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

                default: throw new ArgumentOutOfRangeException( nameof( interval ) );
            }

            return sum;
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

        public static Int64 GetDifferenceInDays( DateTime start, DateSpan span, Boolean excludeEndDate = true ) {
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
    }
}