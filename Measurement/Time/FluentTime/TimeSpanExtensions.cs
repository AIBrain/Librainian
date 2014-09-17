

namespace Librainian.Measurement.Time.FluentTime {
    using System;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public static class TimeSpanExtensions {

        public static DateTime After( this TimeSpan span, DateTime dateTime ) {
            return dateTime + span;
        }

        public static DateTimeOffset After( this TimeSpan span, DateTimeOffset dateTime ) {
            return dateTime + span;
        }

        public static DateTimeOffset Ago( this TimeSpan span ) {
            return Before( span, DateTimeOffset.Now );
        }

        public static DateTime Before( this TimeSpan span, DateTime dateTime ) {
            return dateTime - span;
        }

        public static DateTimeOffset Before( this TimeSpan span, DateTimeOffset dateTime ) {
            return dateTime - span;
        }

        public static DateTimeOffset FromNow( this TimeSpan span ) {
            return After( span, DateTimeOffset.Now );
        }
    }
}