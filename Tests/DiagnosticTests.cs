// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DiagnosticTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/LibrainianTests/DiagnosticTests.cs" was last formatted by Protiguous on 2018/05/24 at 7:36 PM.

namespace LibrainianTests {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Librainian;
    using Librainian.Collections;
    using Librainian.Maths;
    using Librainian.Measurement.Time;
    using Librainian.Measurement.Time.Clocks;
    using Librainian.Parsing;
    using Librainian.Threading;
    using NUnit.Framework;

    [TestFixture]
    public static class DiagnosticTests {

        private static void AddToList( [NotNull] this ConcurrentBag<Int32> list ) {
            if ( list == null ) { throw new ArgumentNullException( nameof( list ) ); }

            Parallel.ForEach( 1.To( 128 ), ThreadingExtensions.CPUIntensive, i => list.Add( Int32.MinValue.Next( maxValue: Int32.MaxValue ) ) );
        }

        [Test]
        public static void PassProbabilityTest() {
            var lower = new List<Boolean>();
            var probability = -0.33f;

            for ( var i = 0; i < 1048576; i++ ) { lower.Add( probability.Probability() ); }

            var higher = new List<Boolean>();
            probability = 0.123f;

            for ( var i = 0; i < 1048576; i++ ) { higher.Add( probability.Probability() ); }

            lower.RemoveAll( b => !b );
            higher.RemoveAll( b => !b );

            higher.Count.Should().BeGreaterThan( lower.Count );
        }

        //[OneTimeSetUp]
        public static void Setup() { }

        //[OneTimeTearDown]
        public static void TearDown() { }

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
            var reasons = new ConcurrentQueue<String>();
            var test1 = "hi".Similarity( "hello", reasons );
            $"test1 was {test1}".WriteLine();
        }

        [Test]
        public static void TestWordVsGuid() {
            var wordToGuidAndGuidToWord = new WordToGuidAndGuidToWord( "test", "$$$" );
            var g = new Guid( @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            wordToGuidAndGuidToWord["AIBrain"] = g;
            wordToGuidAndGuidToWord[g].Is( "AIBrain" ).BreakIfFalse();
        }

        //[Test]
        //public static void TestNumberConversions() {
        //    Console.WriteLine( "Calculating..." );

        // var test = BigRational..Parse( "0" ); test.Should().Be( BigRational.Zero );

        // test += BigRational.Parse( "1" ); test.Should().Be( BigRational.One );

        // test += BigRational.Parse( "0.000123" ); ( ( Decimal )test ).Should().Be( 1.000123m );

        // test += BigRational.Parse( "-10.000123" ); ( ( Decimal )test ).Should().Be( -9M );

        // test += BigRational.Parse( "11.1234567890" ); ( ( Decimal )test ).Should().Be( 2.123456789M );

        // test += BigRational.Parse( "-0.12342345" ); ( ( Decimal )test ).Should().Be( 2.000033339M );

        // test += BigRational.Parse( "1.1234567890" ); ( ( Decimal )test ).Should().Be( 3.123490128M );

        // test += BigRational.Parse( "-001.1234567890" ); ( ( Decimal )test ).Should().Be( 2.000033339M );

        // var test2 = BigRational.Parse( "111111.111111" ); ( ( Decimal )test2 ).Should().Be( 111111.111111M );

        // test2 = test2 * test2; ( ( Decimal )test2 ).Should().Be( 12345679012.320987654321M );
    }
}