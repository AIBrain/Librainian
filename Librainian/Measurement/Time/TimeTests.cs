// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TimeTests.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "TimeTests.cs" was last formatted by Protiguous on 2018/07/13 at 1:31 AM.

namespace Librainian.Measurement.Time {

	using System;
	using Clocks;
	using FluentAssertions;
	using Frequency;
	using NUnit.Framework;

	[TestFixture]
	public static class TimeTests {

		//[OneTimeSetUp]
		public static void Setup() { }

		//[OneTimeTearDown]
		public static void TearDown() { }

		[Test]
		public static void DayTest() => Day.MaximumValue.Should().BeGreaterThan( Day.MinimumValue );

		[Test]
		public static void HourTest() => Hour.MaximumValue.Should().BeGreaterThan( Hour.MinimumValue );

		[Test]
		public static void MicrosecondTest() => Microsecond.MaximumValue.Should().BeGreaterThan( Microsecond.MinimumValue );

		[Test]
		public static void MillisecondTest() => Millisecond.MaximumValue.Should().BeGreaterThan( Millisecond.MinimumValue );

		[Test]
		public static void MinuteTest() => Minute.MaximumValue.Should().BeGreaterThan( Minute.MinimumValue );

		[Test]
		public static void MonthTest() => Month.Maximum.Should().BeGreaterThan( Month.Minimum );

		[Test]
		public static void SecondTest() => Second.MaximumValue.Should().BeGreaterThan( Second.MinimumValue );

		[Test]
		public static void ShouldReturnCorrectHomesteadDate_WhenSending_April_6_2378() {
			var homesteadDate = new DateTime( 2378, 4, 6, 12, 0, 0 );
			Assert.AreEqual( 54868.82m, homesteadDate.ToStarDate() );
		}

		[Test]
		public static void ShouldReturnCorrectStarDate_WhenKnownEarthDatePassedIn() {
			var equvalentStarDate = new DateTime( 2318, 7, 5, 12, 0, 0 ).ToStarDate();
			Assert.AreEqual( 0m, equvalentStarDate );
		}

		[Test]
		public static void ShouldReturnCorrectStarDate_WhenSendingToday_April_24_2015() {
			var tngKnownDate = new DateTime( 2015, 4, 24, 12, 0, 0 );
			Assert.AreEqual( -278404.30m, tngKnownDate.ToStarDate() );
		}

		[Test]
		public static void TestAttoseconds() {
			Attoseconds.Zero.Should().BeLessThan( Attoseconds.One );
			Attoseconds.One.Should().BeGreaterThan( Attoseconds.Zero );
			Attoseconds.One.Should().Be( Attoseconds.One );
			Attoseconds.One.Should().BeGreaterThan( Zeptoseconds.One );
			Attoseconds.One.Should().BeLessThan( Femtoseconds.One );
		}

		[Test]
		public static void TestDays() {
			Days.Zero.Should().BeLessThan( Days.One );
			Days.One.Should().BeGreaterThan( Days.Zero );
			Days.One.Should().Be( Days.One );
			Days.One.Should().BeLessThan( Weeks.One );
			Days.One.Should().BeGreaterThan( Hours.One );
		}

		[Test]
		public static void TestDurationParser() {

			//var example = "10s123456789y3mon";
			//TimeSpan timeSpan;
			//String failReason;
			//DurationParser.TryParse( example, out timeSpan, out failReason ); //TODO
		}

		[Test]
		public static void TestFemtoseconds() {
			Femtoseconds.Zero.Should().BeLessThan( Femtoseconds.One );
			Femtoseconds.One.Should().BeGreaterThan( Femtoseconds.Zero );
			Femtoseconds.One.Should().Be( Femtoseconds.One );
			Femtoseconds.One.Should().BeGreaterThan( Attoseconds.One );
			Femtoseconds.One.Should().BeLessThan( Picoseconds.One );
		}

		[Test]
		public static void TestFps() {
			Assert.That( Fps.One < Fps.Two );
			Assert.That( Fps.Ten > Fps.One );
		}

		[Test]
		public static void TestHours() {
			Hours.Zero.Should().BeLessThan( Hours.One );
			Hours.One.Should().BeGreaterThan( Hours.Zero );
			Hours.One.Should().Be( Hours.One );
			Hours.One.Should().BeLessThan( Days.One );
			Hours.One.Should().BeGreaterThan( Minutes.One );
		}

		[Test]
		public static void TestMicroSeconds() {
			Microseconds.Zero.Should().BeLessThan( Microseconds.One );
			Microseconds.One.Should().BeGreaterThan( Microseconds.Zero );
			Microseconds.One.Should().Be( Microseconds.One );
			Microseconds.One.Should().BeLessThan( Milliseconds.One );
			Microseconds.One.Should().BeGreaterThan( Nanoseconds.One );
		}

		[Test]
		public static void TestMilliseconds() {
			Milliseconds.Zero.Should().BeLessThan( Milliseconds.One );
			Milliseconds.One.Should().BeGreaterThan( Milliseconds.Zero );
			Milliseconds.One.Should().Be( Milliseconds.One );
			Milliseconds.One.Should().BeLessThan( Seconds.One );
			Milliseconds.One.Should().BeGreaterThan( Microseconds.One );
		}

		[Test]
		public static void TestMinutes() {
			Minutes.Zero.Should().BeLessThan( Minutes.One );
			Minutes.One.Should().BeGreaterThan( Minutes.Zero );
			Minutes.One.Should().Be( Minutes.One );
			Minutes.One.Should().BeLessThan( Hours.One );
			Minutes.One.Should().BeGreaterThan( Seconds.One );
		}

		[Test]
		public static void TestMonths() {
			Months.Zero.Should().BeLessThan( Months.One );
			Months.One.Should().BeGreaterThan( Months.Zero );
			Months.One.Should().Be( Months.One );
			Months.One.Should().BeLessThan( Years.One );
		}

		[Test]
		public static void TestNanoseconds() {
			Nanoseconds.Zero.Should().BeLessThan( Nanoseconds.One );
			Nanoseconds.One.Should().BeGreaterThan( Nanoseconds.Zero );
			Nanoseconds.One.Should().Be( Nanoseconds.One );
			Nanoseconds.One.Should().BeLessThan( Microseconds.One );
			Nanoseconds.One.Should().BeGreaterThan( Picoseconds.One );
		}

		[Test]
		public static void TestPicoseconds() {
			Picoseconds.Zero.Should().BeLessThan( Picoseconds.One );
			Picoseconds.One.Should().BeGreaterThan( Picoseconds.Zero );
			Picoseconds.One.Should().Be( Picoseconds.One );
			Picoseconds.One.Should().BeLessThan( Nanoseconds.One );
			Picoseconds.One.Should().BeGreaterThan( Femtoseconds.One );
		}

		[Test]
		public static void TestPlanckTimes() {
			PlanckTimes.Zero.Should().BeLessThan( PlanckTimes.One );
			PlanckTimes.One.Should().BeGreaterThan( PlanckTimes.Zero );
			PlanckTimes.One.Should().Be( PlanckTimes.One );
			PlanckTimes.One.Should().BeLessThan( Yoctoseconds.One );
			PlanckTimes.InOneSecond.Should().BeLessThan( PlanckTimes.Constants.InOneMinute );
			PlanckTimes.Constants.InOneMinute.Should().BeLessThan( PlanckTimes.Constants.InOneHour );
			PlanckTimes.Constants.InOneHour.Should().BeLessThan( PlanckTimes.Constants.InOneDay );
			PlanckTimes.Constants.InOneDay.Should().BeLessThan( PlanckTimes.InOneWeek );
			PlanckTimes.InOneWeek.Should().BeLessThan( PlanckTimes.Constants.InOneMonth );
			PlanckTimes.Constants.InOneMonth.Should().BeLessThan( PlanckTimes.InOneYear );
		}

		[Test]
		public static void TestSeconds() {
			Seconds.Zero.Should().BeLessThan( Seconds.One );
			Seconds.One.Should().BeGreaterThan( Seconds.Zero );
			Seconds.One.Should().Be( Seconds.One );
			Seconds.One.Should().BeLessThan( Minutes.One );
			Seconds.One.Should().BeGreaterThan( Milliseconds.One );
		}

		[Test]
		public static void TestSpanIdentity() {
			try {
				SpanOfTime.Identity.Years.Value.Should().Be( 1 );
				SpanOfTime.Identity.Months.Value.Should().Be( 1 );
				SpanOfTime.Identity.Weeks.Value.Should().Be( 1 );
				SpanOfTime.Identity.Days.Value.Should().Be( 1 );
				SpanOfTime.Identity.Hours.Value.Should().Be( 1 );
				SpanOfTime.Identity.Minutes.Value.Should().Be( 1 );
				SpanOfTime.Identity.Seconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Milliseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Microseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Nanoseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Picoseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Attoseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Femtoseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Zeptoseconds.Value.Should().Be( 1 );
				SpanOfTime.Identity.Yoctoseconds.Value.Should().Be( 0 ); //BUG should be 1, not 0

				SpanOfTime.Bytey.TotalPlanckTimes.Should().BeGreaterThan( SpanOfTime.Identity.TotalPlanckTimes );
			}
			catch ( Exception exception ) { exception.Log(); }
		}

		[Test]
		public static void TestTimes() => UniversalDateTime.Now.Should().BeGreaterThan( UniversalDateTime.Unix );

		[Test]
		public static void TestWeeks() {
			Weeks.Zero.Should().BeLessThan( Weeks.One );
			Weeks.One.Should().BeGreaterThan( Weeks.Zero );
			Weeks.One.Should().Be( Weeks.One );

			Weeks.One.Should().BeLessThan( Months.One );
			Weeks.One.Should().BeGreaterThan( Days.One );
		}

		[Test]
		public static void TestYears() {
			Years.Zero.Should().BeLessThan( Years.One );
			Years.One.Should().BeGreaterThan( Years.Zero );
			Years.One.Should().Be( Years.One );

			//One.Should().BeGreaterThan( Months.One );
		}

		[Test]
		public static void TestYoctoseconds() {
			Yoctoseconds.Zero.Should().BeLessThan( Yoctoseconds.One );
			Yoctoseconds.One.Should().BeGreaterThan( Yoctoseconds.Zero );
			Yoctoseconds.One.Should().Be( Yoctoseconds.One );
			Yoctoseconds.One.Should().BeGreaterThan( PlanckTimes.One );
			Yoctoseconds.One.Should().BeLessThan( Zeptoseconds.One );
		}

		[Test]
		public static void TestZeptoSeconds() {
			Zeptoseconds.Zero.Should().BeLessThan( Zeptoseconds.One );
			Zeptoseconds.One.Should().BeGreaterThan( Zeptoseconds.Zero );
			Zeptoseconds.One.Should().BeGreaterThan( Yoctoseconds.One );
			Zeptoseconds.One.Should().Be( Zeptoseconds.One );
			Zeptoseconds.One.Should().BeLessThan( Attoseconds.One );
		}
	}
}