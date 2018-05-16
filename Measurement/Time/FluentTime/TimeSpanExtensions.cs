// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "TimeSpanExtensions.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/TimeSpanExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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
            if ( !etaCalculator.DoWeHaveAnEta() ) { return TimeSpan.MaxValue; }

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