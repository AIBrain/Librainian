// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/TimeTests.cs" was last cleaned by Rick on 2015/10/07 at 9:46 AM

namespace Librainian.Measurement.Time {

    using System;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public static class TimeTests {

        [OneTimeSetUp]
        public static void Setup() {
        }

        [OneTimeTearDown]
        public static void TearDown() {
        }

        [Test]
        public static void TestIdentity() {
            try {
                //var sizeInBytes = Span.Identity.CalcSizeInBytes();

                Span.Identity.Years.Value.Should()
                    .Be( 1 );
                Span.Identity.Months.Value.Should()
                    .Be( 1 );
                Span.Identity.Weeks.Value.Should()
                    .Be( 1 );
                Span.Identity.Days.Value.Should()
                    .Be( 1 );
                Span.Identity.Hours.Value.Should()
                    .Be( 1 );
                Span.Identity.Minutes.Value.Should()
                    .Be( 1 );
                Span.Identity.Seconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Milliseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Microseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Nanoseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Picoseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Attoseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Femtoseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Zeptoseconds.Value.Should()
                    .Be( 1 );
                Span.Identity.Yoctoseconds.Value.Should()
                    .Be( 1 );

                Span.Bitey.TotalPlanckTimes.Should()
                    .BeGreaterThan( Span.Identity.TotalPlanckTimes );
            }
            catch ( Exception exception ) {
                exception.More();
            }
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
        public static void TestTimes() {
            UniversalDateTime.Now.Should()
                             .BeGreaterThan( UniversalDateTime.Unix );

            
        }

    }

    [TestFixture]
    public class StarDateTests {
        [Test]
        public void ShouldReturnCorrectStarDate_WhenKnownEarthDatePassedIn() {
            var equvalentStarDate = new DateTime( 2318, 7, 5, 12, 0, 0 ).ToStarDate();
            Assert.AreEqual( 0m, equvalentStarDate );
        }

        [Test]
        public void ShouldReturnCorrectStarDate_WhenSendingToday_April_24_2015() {
            var tngKnownDate = new DateTime( 2015, 4, 24, 12, 0, 0 );
            Assert.AreEqual( -278404.30m, tngKnownDate.ToStarDate() );
        }

        [Test]
        public void ShouldReturnCorrectHomesteadDate_WhenSending_April_6_2378() {
            var homesteadDate = new DateTime( 2378, 4, 6, 12, 0, 0 );
            Assert.AreEqual( 54868.82m, homesteadDate.ToStarDate() );
        }
    }

}
