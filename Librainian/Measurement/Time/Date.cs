// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Date.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Date.cs" was last formatted by Protiguous on 2018/07/13 at 1:27 AM.

namespace Librainian.Measurement.Time {

    using Clocks;
    using Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Numerics;

    /// <summary><see cref="Year" />, <see cref="Month" />, and <see cref="Day" />.</summary>
    [Immutable]
    [JsonObject]
    public struct Date {

        public static readonly Date Zero = new Date(Year.Zero, Month.Minimum, Day.Minimum);

        /// <summary>
        ///     <para>The day of the month. (valid range is 1 to 31)</para>
        /// </summary>
        [JsonProperty]
        public Day Day { get; }

        /// <summary>
        ///     <para>The number of the month. (valid range is 1-12)</para>
        ///     <para>12 months makes 1 year.</para>
        /// </summary>
        [JsonProperty]
        public Month Month { get; }

        /// <summary>
        ///     <para><see cref="Year" /> can be a positive or negative <see cref="BigInteger" />.</para>
        /// </summary>
        [JsonProperty]
        public Year Year { get; }

        //public Date( Years years, Months months, Days days )
        //    : this( year: ( BigInteger )years.Value, month: ( BigInteger )months.Value, day: ( BigInteger )days.Value ) {
        //}
        public static Date Now => new Date(DateTime.Now);

        //public Date( BigInteger year, BigInteger month, BigInteger day )
        //    : this( year: new Year( year ), month: new Month( month ), day: new Day( day ) ) {
        //}
        public static Date UtcNow => new Date(DateTime.UtcNow);

        public Date(BigInteger year, Byte month, Byte day) {
            while (day > Day.Maximum) {
                day -= Day.Maximum;
                month++;

                while (month > Month.Maximum) {
                    month -= Month.Maximum;
                    year++;
                }
            }

            this.Day = new Day(day);

            while (month > Month.Maximum) {
                month -= Month.Maximum;
                year++;
            }

            this.Month = new Month(month);

            this.Year = new Year(year);
        }

        public Date(Year year, Month month, Day day) {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        public Date(DateTime dateTime) : this(year: dateTime.Year, month: (Byte)dateTime.Month, day: (Byte)dateTime.Day) { }

        public Date(SpanOfTime spanOfTime) {
            this.Year = new Year(spanOfTime.GetWholeYears());

            this.Month = spanOfTime.Months.Value < Month.Minimum.Value ? new Month(Month.Minimum) : new Month((Byte)spanOfTime.Months.Value);

            this.Day = spanOfTime.Days.Value < Day.Minimum.Value ? new Day(Day.Minimum) : new Day((Byte)spanOfTime.Days.Value);
        }

        //public Date( long year, long month, long day )
        //    : this( year: new Year( year ), month: new Month( month ), day: new Day( day ) ) {
        //}
        public static implicit operator DateTime? (Date date) => TimeExtensions.TryConvertToDateTime(date, out var dateTime) ? dateTime : default;

        public static Boolean operator <(Date left, Date right) => left.ToSpan().CalcTotalPlanckTimes() < right.ToSpan().CalcTotalPlanckTimes();

        public static Boolean operator <=(Date left, Date right) => left.ToSpan().CalcTotalPlanckTimes() <= right.ToSpan().CalcTotalPlanckTimes();

        public static Boolean operator >(Date left, Date right) => left.ToSpan().CalcTotalPlanckTimes() > right.ToSpan().CalcTotalPlanckTimes();

        public static Boolean operator >=(Date left, Date right) => left.ToSpan().CalcTotalPlanckTimes() >= right.ToSpan().CalcTotalPlanckTimes();

        //public static Date operator +( Date left, Date right ) {
        //    //what does it mean to add two dates ?
        //    var days = new Span( days: new   System.Decimal( left.Day.Value + right.Day.Value ), months: new   System.Decimal( left.Month.Value + right.Month.Value ), years: new   System.Decimal(( Double ) ( left.Year.Value + right.Year.Value )) );
        //    var months = new Span(  );
        //    return days;
        //}

        public Boolean TryConvertToDateTime(out DateTime? dateTime) => TimeExtensions.TryConvertToDateTime(this, out dateTime);
    }
}