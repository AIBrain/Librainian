// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TimeSpanExtensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.FluentTime {

    using System;
    using System.Linq;
    using Maths;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class TimeSpanExtensions {

        public static DateTime After( this TimeSpan span, DateTime dateTime ) => dateTime + span;

        public static DateTimeOffset After( this TimeSpan span, DateTimeOffset dateTime ) => dateTime + span;

        public static DateTimeOffset Ago( this TimeSpan span ) => Before( span, DateTimeOffset.Now );

        public static DateTime Before( this TimeSpan span, DateTime dateTime ) => dateTime - span;

        public static DateTimeOffset Before( this TimeSpan span, DateTimeOffset dateTime ) => dateTime - span;

        /// <summary>
        ///     <para>Calculates the Estimated Time Remaining</para>
        /// </summary>
        /// <param name="etaCalculator"></param>
        public static TimeSpan Etr( this EtaCalculator etaCalculator ) {
            if ( !etaCalculator.DoWeHaveAnEta() ) {
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

            var datapoints = etaCalculator.GetDataPoints().ToList();

            var intercept = datapoints.Intercept();

            estimateTimeRemaing += TimeSpan.FromMilliseconds( intercept );

            return estimateTimeRemaing;
        }

        public static DateTimeOffset FromNow( this TimeSpan span ) => After( span, DateTimeOffset.Now );
    }
}