namespace Librainian.Measurement.Time.FluentTime {

    using System;

    public static class Next {

        public static DateTime Friday( ) => GetNextOfDay( DayOfWeek.Friday );

        public static DateTime Monday( ) => GetNextOfDay( DayOfWeek.Monday );

        public static DateTime Saturday( ) => GetNextOfDay( DayOfWeek.Saturday );

        public static DateTime Sunday( ) => GetNextOfDay( DayOfWeek.Sunday );

        public static DateTime Thursday( ) => GetNextOfDay( DayOfWeek.Thursday );

        public static DateTime Tuesday( ) => GetNextOfDay( DayOfWeek.Tuesday );

        public static DateTime Wednesday( ) => GetNextOfDay( DayOfWeek.Wednesday );

        private static DateTime GetNextOfDay( DayOfWeek dayOfWeek ) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta <= 0 ? delta + 7 : delta );
            return result;
        }
    }
}