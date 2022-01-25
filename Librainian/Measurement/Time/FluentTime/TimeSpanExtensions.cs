// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "TimeSpanExtensions.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Time.FluentTime;

using System;
using System.Linq;
using Maths;

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
		if ( !etaCalculator.CanWeHaveAnEta() ) {
			return TimeSpan.MaxValue;
		}

		var datapoints = etaCalculator.GetDataPoints().ToList();

		var intercept = datapoints.Intercept();

		var estimateTimeRemaing = TimeSpan.FromMilliseconds( intercept );

		return estimateTimeRemaing;
	}

	public static DateTimeOffset FromNow( this TimeSpan span ) => After( span, DateTimeOffset.Now );
}