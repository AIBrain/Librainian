// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "DateSpanTests.cs" belongs to Protiguous@Protiguous.com
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
// File "DateSpanTests.cs" was last formatted by Protiguous on 2018/06/26 at 1:28 AM.

namespace Librainian.Measurement.Time {

	using System;
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
}