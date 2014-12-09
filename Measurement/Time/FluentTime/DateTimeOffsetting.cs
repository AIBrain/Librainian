

namespace Librainian.Measurement.Time.FluentTime {
    using System;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public static class DateTimeOffsetting {

        public static DateTimeOffset Offset( this DateTime d, TimeSpan offset ) => new DateTimeOffset( d, offset );

        public static DateTimeOffset Offset( this DateTime d, int hours ) => d.Offset( TimeSpan.FromHours( hours ) );

        public static DateTimeOffset OffsetFor( this DateTime d, TimeZoneInfo zone ) => d.Offset( zone.GetUtcOffset( d ) );

        public static DateTimeOffset OffsetFor( this DateTime d, string timeZoneId ) => d.OffsetFor( TimeZoneInfo.FindSystemTimeZoneById( timeZoneId ) );

        public static DateTimeOffset OffsetForLocal( this DateTime d ) => d.OffsetFor( TimeZoneInfo.Local );
    }
}