namespace Librainian.Measurement.Time.FluentTime {

    using System;
    using System.Linq;
    using Maths;

    /// <summary>
    /// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.
    /// </summary>
    public static class TimeSpanExtensions {

        public static DateTime After( this TimeSpan span, DateTime dateTime ) => dateTime + span;

        public static DateTimeOffset After( this TimeSpan span, DateTimeOffset dateTime ) => dateTime + span;

        public static DateTimeOffset Ago( this TimeSpan span ) => Before( span, DateTimeOffset.Now );

        public static DateTime Before( this TimeSpan span, DateTime dateTime ) => dateTime - span;

        public static DateTimeOffset Before( this TimeSpan span, DateTimeOffset dateTime ) => dateTime - span;

        public static DateTimeOffset FromNow( this TimeSpan span ) => After( span, DateTimeOffset.Now );

        /// <summary>
        /// <para>Calculates the Estimated Time Remaining</para>
        /// </summary>
        /// <param name="etaCalculator"></param>
        public static TimeSpan ETR( this ETACalculator etaCalculator ) {
            if ( !etaCalculator.DoWeHaveAnETA( ) ) {
                return TimeSpan.MaxValue;
            }

            var estimateTimeRemaing = TimeSpan.MaxValue; //assume forever

            //var datapoints = this.GetDataPoints().OrderBy( pair => pair.Key ).ToList();
            //var datapointCount = datapoints.Count;

            //var timeActuallyTakenSoFar = TimeSpan.Zero;

            //foreach ( var dataPoint in datapoints ) {
            //    var timePassed = dataPoint.Key;
            //    var progress = dataPoint.Value;
            //}

            var datapoints = etaCalculator.GetDataPoints( ).ToList( );

            var intercept = datapoints.Intercept( );

            estimateTimeRemaing += TimeSpan.FromMilliseconds( intercept );

            return estimateTimeRemaing;
        }
    }
}