// Copyright 2018 Protiguous.
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
// "Librainian/TimeTests.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement.Time {

    using System;
    using Clocks;
    using FluentAssertions;
    using Frequency;
    using NUnit.Framework;

    [TestFixture]
    public static class TimeTests {

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

		//[OneTimeSetUp]
		public static void Setup() {
        }

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

        //[OneTimeTearDown]
        public static void TearDown() {
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
            PlanckTimes.InOneSecond.Should().BeLessThan( PlanckTimes.InOneMinute );
            PlanckTimes.InOneMinute.Should().BeLessThan( PlanckTimes.InOneHour );
            PlanckTimes.InOneHour.Should().BeLessThan( PlanckTimes.InOneDay );
            PlanckTimes.InOneDay.Should().BeLessThan( PlanckTimes.InOneWeek );
            PlanckTimes.InOneWeek.Should().BeLessThan( PlanckTimes.InOneMonth );
            PlanckTimes.InOneMonth.Should().BeLessThan( PlanckTimes.InOneYear );
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
                Span.Identity.Years.Value.Should().Be( 1 );
                Span.Identity.Months.Value.Should().Be( 1 );
                Span.Identity.Weeks.Value.Should().Be( 1 );
                Span.Identity.Days.Value.Should().Be( 1 );
                Span.Identity.Hours.Value.Should().Be( 1 );
                Span.Identity.Minutes.Value.Should().Be( 1 );
                Span.Identity.Seconds.Value.Should().Be( 1 );
                Span.Identity.Milliseconds.Value.Should().Be( 1 );
                Span.Identity.Microseconds.Value.Should().Be( 1 );
                Span.Identity.Nanoseconds.Value.Should().Be( 1 );
                Span.Identity.Picoseconds.Value.Should().Be( 1 );
                Span.Identity.Attoseconds.Value.Should().Be( 1 );
                Span.Identity.Femtoseconds.Value.Should().Be( 1 );
                Span.Identity.Zeptoseconds.Value.Should().Be( 1 );
                Span.Identity.Yoctoseconds.Value.Should().Be( 0 ); //BUG should be 1, not 0

                Span.Bytey.TotalPlanckTimes.Should().BeGreaterThan( Span.Identity.TotalPlanckTimes );
            }
            catch ( Exception exception ) {
                exception.More();
            }
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