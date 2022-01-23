// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any
// binaries, libraries, repositories, or source code (directly or derived) from our binaries,
// libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original
// license has been overwritten by formatting. (We try to avoid it from happening, but it does
// accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original
// license and our thanks goes to those Authors. If you find your code unattributed in this source
// code, please let us know so we can properly attribute you and include the proper license and/or
// copyright(s). If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied,
// or given. We are NOT responsible for Anything You Do With Our Code. We are NOT responsible for
// Anything You Do With Our Executables. We are NOT responsible for Anything You Do With Your
// Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our
// code in your project(s). For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "TimeTests.cs" last touched on 2022-01-22 at 1:17 AM by Protiguous.

// ReSharper disable EqualExpressionComparison

namespace TestBigDecimal.Maths;

using System.Diagnostics;
using ExtendedNumerics;
using FluentAssertions;
using Librainian.Measurement.Frequency;
using Librainian.Measurement.Time;
using NUnit.Framework;

[TestFixture]
public static class TimeTests {

	[Test]
	public static void ShouldReturnCorrectHomesteadDate_WhenSending_April_6_2378() {
		var homesteadDate = new DateTime( 2378, 4, 6, 12, 0, 0 );
		Assert.That( homesteadDate.ToStarDate(), Is.EqualTo( 54868.82m ) );
	}

	[Test]
	public static void ShouldReturnCorrectStarDate_WhenKnownEarthDatePassedIn() {
		var equvalentStarDate = new DateTime( 2318, 7, 5, 12, 0, 0 ).ToStarDate();
		Assert.That( equvalentStarDate, Is.EqualTo( 0m ) );
	}

	[Test]
	public static void ShouldReturnCorrectStarDate_WhenSendingToday_April_24_2015() {
		var tngKnownDate = new DateTime( 2015, 4, 24, 12, 0, 0 );
		Assert.That( tngKnownDate.ToStarDate(), Is.EqualTo( -278404.30m ) );
	}

	//[OneTimeTearDown]
	public static void TearDown() { }

	[Test]
	public static void TestAttoseconds() {

		//Attoseconds.Zero.Value.Should().BeLessThan(Attoseconds.One.Value);
		//Attoseconds.One.Should().BeGreaterThan(Attoseconds.Zero);
		//Attoseconds.One.Should().Be(Attoseconds.One);
		//Attoseconds.One.Should().BeGreaterThan(Zeptoseconds.One);
		//Attoseconds.One.Should().BeLessThan(Femtoseconds.One);
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
	public static void TestEstimateTimeRemaining() {
		var timetakenSoFar = ( TimeSpan )Minutes.Fifteen;
		const Decimal progress = 0.25m;

		var estimateTimeRemaining = timetakenSoFar.EstimateTimeRemaining( progress );
		TestContext.WriteLine( estimateTimeRemaining.Simpler() );

		var expected = new Minutes( 45 ).AsTimeSpan();

		Assert.That( expected, Is.EqualTo( estimateTimeRemaining ) );
	}

	[Test]
	public static void TestFemtoseconds() {
		Femtoseconds.Zero.Should()?.BeLessThan( Femtoseconds.One );
		Femtoseconds.One.Should()?.BeGreaterThan( Femtoseconds.Zero );
		Femtoseconds.One.Should()?.Be( Femtoseconds.One );
		Femtoseconds.One.Should()?.BeGreaterThan( Attoseconds.One );
		Femtoseconds.One.Should()?.BeLessThan( Picoseconds.One );
	}

	[Test]
	public static void TestFps() {
		Assert.That( Fps.One < Fps.Two, Is.True );
		Assert.That( Fps.Ten > Fps.One, Is.True );
	}

	[Test]
	public static void TestHours() {
		Hours.Zero.Should()?.BeLessThan( Hours.One );
		Hours.One.Should()?.BeGreaterThan( Hours.Zero );
		Hours.One.Should()?.Be( Hours.One );
		Hours.One.Should()?.BeLessThan( Days.One );
		Hours.One.Should()?.BeGreaterThan( Minutes.One );
	}

	[Test]
	public static void TestMicroSeconds() {
		Microseconds.Zero.Should()?.BeLessThan( Microseconds.One );
		Microseconds.One.Should()?.BeGreaterThan( Microseconds.Zero );
		Microseconds.One.Should()?.Be( Microseconds.One );
		Microseconds.One.Should()?.BeLessThan( Milliseconds.One );
		Microseconds.One.Should()?.BeGreaterThan( Nanoseconds.One );
	}

	[Test]
	public static void TestMilliseconds() {
		Milliseconds.Zero.Should()?.BeLessThan( Milliseconds.One );
		Milliseconds.One.Should()?.BeGreaterThan( Milliseconds.Zero );
		Milliseconds.One.Should()?.Be( Milliseconds.One );
		Milliseconds.One.Should()?.BeLessThan( Seconds.One );
		Milliseconds.One.Should()?.BeGreaterThan( Microseconds.One );
	}

	[Test]
	public static void TestMinutes() {
		Minutes.Zero.Should()?.BeLessThan( Minutes.One );
		Minutes.One.Should()?.BeGreaterThan( Minutes.Zero );
		Minutes.One.Should()?.Be( Minutes.One );
		Minutes.One.Should()?.BeLessThan( Hours.One );
		Minutes.One.Should()?.BeGreaterThan( Seconds.One );
	}

	[Test]
	public static void TestMonths() {
		Months.Zero.Should()?.BeLessThan( Months.One );
		Months.One.Should()?.BeGreaterThan( Months.Zero );
		Months.One.Should()?.Be( Months.One );
		Months.One.Should()?.BeLessThan( Years.One );
	}

	[Test]
	public static void TestNanoseconds() {
		Nanoseconds.Zero.Should()?.BeLessThan( Nanoseconds.One );
		Nanoseconds.One.Should()?.BeGreaterThan( Nanoseconds.Zero );
		Nanoseconds.One.Should()?.Be( Nanoseconds.One );
		Nanoseconds.One.Should()?.BeLessThan( Microseconds.One );
		Nanoseconds.One.Should()?.BeGreaterThan( Picoseconds.One );
	}

	[Test]
	public static void TestPicoseconds() {
		Picoseconds.Zero.Should()?.BeLessThan( Picoseconds.One );
		Picoseconds.One.Should()?.BeGreaterThan( Picoseconds.Zero );
		Picoseconds.One.Should()?.Be( Picoseconds.One );
		Picoseconds.One.Should()?.BeLessThan( Nanoseconds.One );
		Picoseconds.One.Should()?.BeGreaterThan( Femtoseconds.One );
	}

	[Test]
	public static void TestPlanckTimes() {
		PlanckTimes.One.Should().Be( PlanckTimes.One );
		PlanckTimes.Zero.Should().BeLessThan( PlanckTimes.One );
		PlanckTimes.One.Should().BeGreaterThan( PlanckTimes.Zero );
		PlanckTimes.One.Should().BeLessThan( Yoctoseconds.One );

		Debug.Assert( PlanckTimes.InOneSecond < PlanckTimes.InOneMinute );
		Assert.That( PlanckTimes.InOneMinute < PlanckTimes.InOneHour, Is.True );
		Assert.That( PlanckTimes.InOneHour < PlanckTimes.InOneDay, Is.True );
		Assert.That( PlanckTimes.InOneDay < PlanckTimes.InOneWeek, Is.True );
		Assert.That( PlanckTimes.InOneWeek < PlanckTimes.InOneMonth, Is.True );
		Assert.That( PlanckTimes.InOneMonth < PlanckTimes.InOneYear, Is.True );
	}

	[Test]
	public static void TestSeconds() {
		Assert.Less( Seconds.Zero.ToTimeSpan(), Seconds.One.ToTimeSpan() );
		Assert.Greater( Seconds.One.ToTimeSpan(), Seconds.Zero.ToTimeSpan() );
		Assert.That( Seconds.One.ToTimeSpan(), Is.EqualTo( Seconds.One.ToTimeSpan() ) );
		Assert.Less( Seconds.One.ToTimeSpan(), Minutes.One.ToTimeSpan() );
		Assert.Greater( Seconds.One.ToTimeSpan(), Milliseconds.One.ToTimeSpan() );
	}

	[Test]
	public static void TestSpanIdentityAttoseconds() => SpanOfTime.Identity.Attoseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityDays() => SpanOfTime.Identity.Days.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityFemtoseconds() => SpanOfTime.Identity.Femtoseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityHours() => SpanOfTime.Identity.Hours.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityMicroseconds() => SpanOfTime.Identity.Microseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityMilliseconds() => SpanOfTime.Identity.Milliseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityMinutes() => SpanOfTime.Identity.Minutes.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityMonths() => SpanOfTime.Identity.Months.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityNanoseconds() => SpanOfTime.Identity.Nanoseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityPicoseconds() => SpanOfTime.Identity.Picoseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentitySeconds() => SpanOfTime.Identity.Seconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityWeeks() => SpanOfTime.Identity.Weeks.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityYears() => SpanOfTime.Identity.Years.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityYoctoseconds() => SpanOfTime.Identity.Yoctoseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestSpanIdentityZeptoseconds() => SpanOfTime.Identity.Zeptoseconds.Value.Should()?.Be( BigDecimal.One );

	[Test]
	public static void TestTimes() {
		var now = UniversalDateTime.Now();
		var unix = UniversalDateTime.Unix;

		now.Should()!.BeGreaterThan( unix );
	}

	[Test]
	public static void TestWeeks() {
		Assert.That( Weeks.Zero < Weeks.One, Is.True );
		Assert.That( Weeks.One == Weeks.One, Is.True );
		Assert.That( Weeks.One < Months.One, Is.True );
		Assert.That( Weeks.One > Days.One, Is.True );
		Assert.That( Weeks.One < Years.One, Is.True );
	}

	[Test]
	public static void TestYears() => Assert.That( Years.Zero < Years.One, Is.True );

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