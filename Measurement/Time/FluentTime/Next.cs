namespace Librainian.Measurement.Time.FluentTime {
    using System;

    public static class Next {

        public static DateTime Friday() {
            return GetNextOfDay( DayOfWeek.Friday );
        }

        public static DateTime Monday() {
            return GetNextOfDay( DayOfWeek.Monday );
        }

        public static DateTime Saturday() {
            return GetNextOfDay( DayOfWeek.Saturday );
        }

        public static DateTime Sunday() {
            return GetNextOfDay( DayOfWeek.Sunday );
        }

        public static DateTime Thursday() {
            return GetNextOfDay( DayOfWeek.Thursday );
        }

        public static DateTime Tuesday() {
            return GetNextOfDay( DayOfWeek.Tuesday );
        }

        public static DateTime Wednesday() {
            return GetNextOfDay( DayOfWeek.Wednesday );
        }

        private static DateTime GetNextOfDay( DayOfWeek dayOfWeek ) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta <= 0 ? delta + 7 : delta );
            return result;
        }
    }
}