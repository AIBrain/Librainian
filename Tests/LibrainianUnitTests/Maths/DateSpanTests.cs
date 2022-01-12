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
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "DateSpanTests.cs" last touched on 2022-01-01 at 6:50 AM by Protiguous.

namespace LibrainianUnitTests.Maths;

using System;
using Librainian.Measurement.Time;
using NUnit.Framework;

/// <summary>
///     From <see cref="http://github.com/danielcrenna/vault/blob/master/dates/src/Dates.Tests/DateSpanTests.cs" />
/// </summary>
[TestFixture]
public class DateSpanTests {

	[Test]
	public void Can_get_date_difference_in_days() {
		var start = DateTime.Now;
		var end = DateTime.Now.AddDays( 5 );
		var diff = DateSpan.GetDifference( DateInterval.Days, start, end );

		Assert.AreEqual( 5, diff );
	}

	[Test]
	public void Can_get_date_difference_in_days_spanning_one_month() {
		var start = new DateTime( 2009, 09, 30 );
		var end = new DateTime( 2009, 10, 01 );

		var days = DateSpan.GetDifference( DateInterval.Days, start, end );
		Assert.AreEqual( 1, days );
	}

	[Test]
	public void Can_get_date_difference_in_days_spanning_one_week() {
		var start = new DateTime( 2009, 09, 30 );
		var end = start.AddDays( 10 );

		var days = DateSpan.GetDifference( DateInterval.Days, start, end );
		var weeks = DateSpan.GetDifference( DateInterval.Weeks, start, end );

		Assert.AreEqual( 10, days );
		Assert.AreEqual( 1, weeks );
	}

	[Test]
	public void Can_get_date_difference_in_days_spanning_two_months() {
		var start = new DateTime( 2009, 09, 30 );
		var end = new DateTime( 2009, 11, 04 ); // 4 days in November, 31 in October

		var days = DateSpan.GetDifference( DateInterval.Days, start, end );
		Assert.AreEqual( 35, days );
	}

	[Test]
	public void Can_get_date_difference_in_seconds() {
		var start = DateTime.Now;
		var end = DateTime.Now.AddDays( 5 );
		var diff = DateSpan.GetDifference( DateInterval.Seconds, start, end );

		Assert.AreEqual( 432000, diff );
	}

	[Test]
	public void Can_handle_composite_spans() {
		var start = new DateTime( 2009, 9, 30 );
		var end = new DateTime( 2009, 10, 31 );
		var span = new DateSpan( start, end );

		Assert.AreEqual( 1, span.Months );
		Assert.AreEqual( 1, span.Days );

		Console.WriteLine( span.Months );
		Console.WriteLine( span.Days );

		var difference = DateSpan.GetDifference( DateInterval.Days, start, end );
		Console.WriteLine( difference );
	}

}