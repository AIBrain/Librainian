namespace Librainian.Measurement.Time.FluentTime {

    using System;

    public static class Today {

        public static DateTime At( int hour, int minute = 0, int second = 0 ) => AdjustableCurrentTime.Today.At( hour, minute, second );
    }
}