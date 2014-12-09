

namespace Librainian.Measurement.Time.FluentTime {
    using System;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public static class DateTimeCreation {

        public static DateTime AM( this DateTime d ) {
            Reject24HourTime( d );
            return d.Hour < 12 ? d : d.AddHours( -12 );
        }

        public static DateTime April( this int day, int year ) => new DateTime( year, 4, day );

        public static DateTime At( this DateTime d, int hour, int minute = 0, int second = 0 ) => new DateTime( d.Year, d.Month, d.Day, hour, minute, second );

        public static DateTime August( this int day, int year ) => new DateTime( year, 8, day );

        public static DateTime December( this int day, int year ) => new DateTime( year, 12, day );

        public static DateTime February( this int day, int year ) => new DateTime( year, 2, day );

        public static DateTime January( this int day, int year ) => new DateTime( year, 1, day );

        public static DateTime July( this int day, int year ) => new DateTime( year, 7, day );

        public static DateTime June( this int day, int year ) => new DateTime( year, 6, day );

        public static DateTime Local( this DateTime d ) => DateTime.SpecifyKind( d, DateTimeKind.Local );

        public static DateTime March( this int day, int year ) => new DateTime( year, 3, day );

        public static DateTime May( this int day, int year ) => new DateTime( year, 5, day );

        public static DateTime November( this int day, int year ) => new DateTime( year, 11, day );

        public static DateTime October( this int day, int year ) => new DateTime( year, 10, day );

        public static DateTime PM( this DateTime d ) {
            Reject24HourTime( d );
            return d.Hour == 12 ? d : d.AddHours( 12 );
        }

        public static DateTime September( this int day, int year ) => new DateTime( year, 9, day );

        public static DateTime Utc( this DateTime d ) => DateTime.SpecifyKind( d, DateTimeKind.Utc );

        private static void Reject24HourTime( DateTime d ) {
            if ( d.Hour > 12 )
                throw new ArgumentOutOfRangeException( "d", string.Format( "Not a 12-hour time. Hour is {0}.", d.Hour ) );
        }
    }
}