// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Date.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Date.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time {

    using System;
    using System.Numerics;
    using Clocks;
    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary><see cref="Year" />, <see cref="Month" />, and <see cref="Day" />.</summary>
    [Immutable]
    [JsonObject]
    public struct Date {

        public static readonly Date Zero = new Date( Year.Zero, Month.Minimum, Day.Minimum );

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
        public static Date Now => new Date( DateTime.Now );

        //public Date( BigInteger year, BigInteger month, BigInteger day )
        //    : this( year: new Year( year ), month: new Month( month ), day: new Day( day ) ) {
        //}
        public static Date UtcNow => new Date( DateTime.UtcNow );

        public Date( BigInteger year, Byte month, Byte day ) {
            while ( day > Day.MaximumValue ) {
                day -= Day.MaximumValue;
                month++;

                while ( month > Month.Maximum ) {
                    month -= Month.Maximum;
                    year++;
                }
            }

            this.Day = new Day( day );

            while ( month > Month.Maximum ) {
                month -= Month.Maximum;
                year++;
            }

            this.Month = new Month( month );

            this.Year = new Year( year );
        }

        public Date( Year year, Month month, Day day ) {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }

        public Date( DateTime dateTime ) : this( year: dateTime.Year, month: ( Byte )dateTime.Month, day: ( Byte )dateTime.Day ) { }

        public Date( Span span ) {
            this.Year = new Year( span.GetWholeYears() );

            this.Month = span.Months.Value < Month.Minimum ? new Month( Month.Minimum ) : new Month( ( Byte )span.Months.Value );

            this.Day = span.Days.Value < Day.MinimumValue ? new Day( Day.MinimumValue ) : new Day( ( Byte )span.Days.Value );
        }

        //public Date( long year, long month, long day )
        //    : this( year: new Year( year ), month: new Month( month ), day: new Day( day ) ) {
        //}
        public static implicit operator DateTime? ( Date date ) => TimeExtensions.TryConvertToDateTime( date, out var dateTime ) ? dateTime : default;

        public static Boolean operator <( Date left, Date right ) => left.ToSpan().TotalPlanckTimes < right.ToSpan().TotalPlanckTimes;

        public static Boolean operator <=( Date left, Date right ) => left.ToSpan().TotalPlanckTimes.Value <= right.ToSpan().TotalPlanckTimes.Value;

        public static Boolean operator >( Date left, Date right ) => left.ToSpan().TotalPlanckTimes > right.ToSpan().TotalPlanckTimes;

        public static Boolean operator >=( Date left, Date right ) => left.ToSpan().TotalPlanckTimes.Value >= right.ToSpan().TotalPlanckTimes.Value;

        //public static Date operator +( Date left, Date right ) {
        //    //what does it mean to add two dates ?
        //    var days = new Span( days: new   System.Decimal( left.Day.Value + right.Day.Value ), months: new   System.Decimal( left.Month.Value + right.Month.Value ), years: new   System.Decimal(( Double ) ( left.Year.Value + right.Year.Value )) );
        //    var months = new Span(  );
        //    return days;
        //}

        [Pure]
        public Boolean TryConvertToDateTime( out DateTime? dateTime ) => TimeExtensions.TryConvertToDateTime( this, out dateTime );
    }
}