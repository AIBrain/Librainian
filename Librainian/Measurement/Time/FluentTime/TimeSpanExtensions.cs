// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeSpanExtensions.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "TimeSpanExtensions.cs" was last formatted by Protiguous on 2018/06/26 at 1:29 AM.

namespace Librainian.Measurement.Time.FluentTime {

	using System;
	using System.Linq;
	using JetBrains.Annotations;
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
		public static TimeSpan Etr( [NotNull] this EtaCalculator etaCalculator ) {
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