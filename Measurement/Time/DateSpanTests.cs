// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DateSpanTests.cs",
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
// "Librainian/Librainian/DateSpanTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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