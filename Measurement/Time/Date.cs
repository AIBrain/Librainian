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
// "Librainian/Date.cs" was last cleaned by Rick on 2014/08/09 at 2:15 PM
#endregion

namespace Librainian.Measurement.Time {
    using System;
    using System.Numerics;
    using Annotations;
    using Librainian.Extensions;

    /// <summary>
    /// </summary>
    [Immutable]
    public struct Date {
        /// <summary>
        ///     <para>The day of the month. (valid range is 1 to 31)</para>
        /// </summary>
        public readonly Day Day;

        /// <summary>
        ///     <para>The number of the month. (valid range is 1-12)</para>
        ///     <para>12 months makes 1 year.</para>
        /// </summary>
        public readonly Month Month;

        /// <summary>
        ///     <para><see cref="Year" /> can be a positive or negative <see cref="BigInteger" />.</para>
        /// </summary>
        public readonly Year Year;

        public Date( BigInteger year, Byte month, Byte day ) {
            while ( day > Day.Maximum ) {
                day -= Day.Maximum;
                month++;
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

        public Date( DateTime dateTime ) : this( year: dateTime.Year, month: ( Byte ) dateTime.Month, day: ( Byte ) dateTime.Day ) { }

        //public Date( long year, long month, long day )
        //    : this( year: new Year( year ), month: new Month( month ), day: new Day( day ) ) {
        //}

        //public Date( BigInteger year, BigInteger month, BigInteger day )
        //    : this( year: new Year( year ), month: new Month( month ), day: new Day( day ) ) {
        //}

        //public Date( Years years, Months months, Days days )
        //    : this( year: ( BigInteger )years.Value, month: ( BigInteger )months.Value, day: ( BigInteger )days.Value ) {
        //}

        public Date( Span span ) {
            this.Year = new Year( span.GetWholeYears() );
            this.Month = new Month( ( byte ) span.Months.Value );
            this.Day = new Day( ( byte ) span.Days.Value );
        }

        public static Date Now { get { return new Date( DateTime.Now ); } }

        public static Date UtcNow { get { return new Date( DateTime.UtcNow ); } }

        public static implicit operator DateTime?( Date date ) {
            DateTime? dateTime;
            return Extensions.TryConvertToDateTime( date, out dateTime ) ? dateTime : default( DateTime );
        }

        public static Boolean operator <( Date left, Date right ) {
            return left.ToSpan().TotalPlanckTimes < right.ToSpan().TotalPlanckTimes;
        }

        public static Boolean operator >( Date left, Date right ) {
            return left.ToSpan().TotalPlanckTimes > right.ToSpan().TotalPlanckTimes;
        }

        public static Boolean operator <=( Date left, Date right ) {
            return left.ToSpan().TotalPlanckTimes <= right.ToSpan().TotalPlanckTimes;
        }

        public static Boolean operator >=( Date left, Date right ) {
            return left.ToSpan().TotalPlanckTimes >= right.ToSpan().TotalPlanckTimes;
        }

        //public static Date operator +( Date left, Date right ) {
        //    //what does it mean to add two dates ?
        //    var days = new Span( days: new Decimal( left.Day.Value + right.Day.Value ), months: new Decimal( left.Month.Value + right.Month.Value ), years: new Decimal(( double ) ( left.Year.Value + right.Year.Value )) );
        //    var months = new Span(  );
        //    return days;
        //}

        [Pure]
        public Boolean TryConvertToDateTime( out DateTime? dateTime ) {
            return Extensions.TryConvertToDateTime( this, out dateTime );
        }
    }
}
