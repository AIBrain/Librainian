// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Diagnostical.cs" was last cleaned by Rick on 2014/09/02 at 3:44 AM

namespace Librainian {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Collections;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Maths;
    using Measurement.Time;
    using Measurement.Time.Clocks;
    using NUnit.Framework;
    using Parsing;
    using Threading;

    [TestFixture]
    public static class Diagnostical {

        [Test]
        public static void PassProbabilityTest() {
            var lower = new List< Boolean >();
            var probability = -0.33f;
            for ( var i = 0; i < 1048576 * 10; i++ ) {
                lower.Add( probability.Probability() );
            }

            var higher = new List< Boolean >();
            probability = 0.123f;
            for ( var i = 0; i < 1048576 * 10; i++ ) {
                higher.Add( probability.Probability() );
            }

            lower.RemoveAll( b => !b );
            higher.RemoveAll( b => !b );

            higher.Count.Should().BeGreaterThan( lower.Count );
        }

        [OneTimeSetUp]
        public static void Setup() {
        }

        [OneTimeTearDown]
        public static void TearDown() {
        }

        [Test]
        public static void TestHour() {
            Hour.Minimum.Value.Should().BeLessThan( Hour.Maximum.Value );
            Hour.Maximum.Value.Should().BeGreaterThan( Hour.Minimum.Value );
        }

        [Test]
        public static void TestMillisecond() {
            Millisecond.Minimum.Value.Should().BeLessThan( Millisecond.Maximum.Value );
            Millisecond.Maximum.Value.Should().BeGreaterThan( Millisecond.Minimum.Value );
        }

        [Test]
        public static void TestMinute() {
            Minute.Minimum.Value.Should().BeLessThan( Minute.Maximum.Value );
            Minute.Maximum.Value.Should().BeGreaterThan( Minute.Minimum.Value );
        }

        //[Test]
        //public static void TestNumberConversions() {

        //    Console.WriteLine( "Calculating..." );

        //    var test = BigRational..Parse( "0" );
        //    test.Should().Be( BigRational.Zero );

        //    test += BigRational.Parse( "1" );
        //    test.Should().Be( BigRational.One );

        //    test += BigRational.Parse( "0.000123" );
        //    ( ( Decimal )test ).Should().Be( 1.000123m );

        //    test += BigRational.Parse( "-10.000123" );
        //    ( ( Decimal )test ).Should().Be( -9M );

        //    test += BigRational.Parse( "11.1234567890" );
        //    ( ( Decimal )test ).Should().Be( 2.123456789M );

        //    test += BigRational.Parse( "-0.12342345" );
        //    ( ( Decimal )test ).Should().Be( 2.000033339M );

        //    test += BigRational.Parse( "1.1234567890" );
        //    ( ( Decimal )test ).Should().Be( 3.123490128M );

        //    test += BigRational.Parse( "-001.1234567890" );
        //    ( ( Decimal )test ).Should().Be( 2.000033339M );

        //    var test2 = BigRational.Parse( "111111.111111" );
        //    ( ( Decimal )test2 ).Should().Be( 111111.111111M );

        //    test2 = test2 * test2;
        //    ( ( Decimal )test2 ).Should().Be( 12345679012.320987654321M );

        //    test2 -= BigRational.Parse( "12345678970.320987654321" );

        //    var answer = test2.ToString();
        //    Console.WriteLine( "The Answer is {0}.", answer );

        //    Console.WriteLine( "Now, please input the question." );
        //}

        [Test]
        public static void TestPlanckTimes() {
            PlanckTimes.InOneYear.Should().BeGreaterThan( PlanckTimes.InOneMonth );
            PlanckTimes.InOneMonth.Should().BeGreaterThan( PlanckTimes.InOneWeek );
            PlanckTimes.InOneWeek.Should().BeGreaterThan( PlanckTimes.InOneDay );
            PlanckTimes.InOneDay.Should().BeGreaterThan( PlanckTimes.InOneHour );
            PlanckTimes.InOneHour.Should().BeGreaterThan( PlanckTimes.InOneMinute );
            PlanckTimes.InOneMinute.Should().BeGreaterThan( PlanckTimes.InOneSecond );
            PlanckTimes.InOneSecond.Should().BeGreaterThan( PlanckTimes.InOneMillisecond );
            PlanckTimes.InOneMillisecond.Should().BeGreaterThan( PlanckTimes.InOneMicrosecond );
            PlanckTimes.InOneMicrosecond.Should().BeGreaterThan( PlanckTimes.InOneNanosecond );
            PlanckTimes.InOneNanosecond.Should().BeGreaterThan( PlanckTimes.InOnePicosecond );
            PlanckTimes.InOnePicosecond.Should().BeGreaterThan( PlanckTimes.InOneFemtosecond );
            PlanckTimes.InOneFemtosecond.Should().BeGreaterThan( PlanckTimes.InOneAttosecond );
            PlanckTimes.InOneAttosecond.Should().BeGreaterThan( PlanckTimes.InOneZeptosecond );
            PlanckTimes.InOneZeptosecond.Should().BeGreaterThan( PlanckTimes.InOneYoctosecond );
        }

        [Test]
        public static void TestRandems() {
            var ints = new ConcurrentBag< Int32 >();
            var processorCount = Environment.ProcessorCount;
            Parallel.ForEach( source: 1.To( processorCount ), parallelOptions: ThreadingExtensions.ParallelOptions, body: i => ints.AddToList() );
            if ( !ints.Duplicates().Any() ) {
                return;
            }
            ints.RemoveAll();
            Parallel.ForEach( 1.To( processorCount ), ThreadingExtensions.ParallelOptions, i => ints.AddToList() );
            if ( !ints.Duplicates().Any() ) {
                return;
            }
            String.Format( "WARNING: Duplicate Randem.Next() found in static test!" ).WriteLine();
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        [Test]
        public static void TestRoman() {
            const Int16 a = 0;
            const Int16 b = 1;
            const Int16 c = 1234;
            const Int16 d = 3999;
            const Int16 e = 4000;

            $"{a} {a.ToRoman().Should()}".WriteLine();
            $"{b} {b.ToRoman()}".WriteLine();
            $"{c} {c.ToRoman()}".WriteLine();
            $"{d} {d.ToRoman()}".WriteLine();
            $"{e} {e.ToRoman()}".WriteLine();
        }

        [Test]
        public static void TestSecond() {
            Second.Minimum.Value.Should().BeLessThan( Second.Maximum.Value );
            Second.Maximum.Value.Should().BeGreaterThan( Second.Minimum.Value );
        }

        [Test]
        public static void TestSimilarities() {
            var reasons = new ConcurrentQueue< String >();
            var test1 = "hi".Similarity( "hello", reasons );
            $"test1 was {test1}".WriteLine();
        }

        [Test]
        public static void TestWordVsGuid(WordToGuidAndGuidToWord wordToGuidAndGuidToWord) {
            var g = new Guid( @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            wordToGuidAndGuidToWord[ "AIBrain" ] = g;
            wordToGuidAndGuidToWord[ g ].Same( "AIBrain" ).BreakIfFalse();
        }

        internal static void AddToList( [NotNull] this ConcurrentBag<Int32> list ) {
            if ( list == null ) {
                throw new ArgumentNullException( nameof( list ) );
            }
            Parallel.ForEach( 1.To( 128 ), ThreadingExtensions.ParallelOptions, i => list.Add( Int32.MinValue.Next( maxValue: Int32.MaxValue ) ) );
        }

    }
}