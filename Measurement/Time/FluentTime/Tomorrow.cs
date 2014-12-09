namespace Librainian.Measurement.Time.FluentTime {
    using System;

    public static class Tomorrow {

        public static DateTime At( int hour, int minute = 0, int second = 0 ) => AdjustableCurrentTime.Today.AddDays( 1 ).At( hour, minute, second );
    }
}