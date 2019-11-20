// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DiagnosticTests.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
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
// Project: "LibrainianTests", "DiagnosticTests.cs" was last formatted by Protiguous on 2019/03/17 at 11:06 AM.

namespace LibrainianTests {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Librainian.Logging;
    using Librainian.Maths;
    using Librainian.Measurement.Time;
    using Librainian.Measurement.Time.Clocks;
    using Librainian.Parsing;
    using Xunit;

    public static class DiagnosticTests {

        private static void AddToList( [NotNull] this ConcurrentBag<Int32> list ) {
            if ( list is null ) {
                throw new ArgumentNullException( nameof( list ) );
            }

            Parallel.ForEach( 1.To( 128 ), i => list.Add( Int32.MinValue.Next( maxValue: Int32.MaxValue ) ) );
        }

        [Fact]
        public static void PassProbabilityTest() {
            var lower = new List<Boolean>();
            var probability = -0.33f;

            for ( var i = 0; i < 1048576; i++ ) {
                lower.Add( probability.Probability() );
            }

            var higher = new List<Boolean>();
            probability = 0.123f;

            for ( var i = 0; i < 1048576; i++ ) {
                higher.Add( probability.Probability() );
            }

            lower.RemoveAll( b => !b );
            higher.RemoveAll( b => !b );

            higher.Count.Should().BeGreaterThan( lower.Count );
        }

        //[OneTimeSetUp]
        public static void Setup() { }

        //[OneTimeTearDown]
        public static void TearDown() { }

        [Fact]
        public static void TestHour() {
            Hour.Minimum.Value.Should().BeLessThan( Hour.Maximum.Value );
            Hour.Maximum.Value.Should().BeGreaterThan( Hour.Minimum.Value );
        }

        [Fact]
        public static void TestMillisecond() {
            Millisecond.Minimum.Value.Should().BeLessThan( Millisecond.Maximum.Value );
            Millisecond.Maximum.Value.Should().BeGreaterThan( Millisecond.Minimum.Value );
        }

        [Fact]
        public static void TestMinute() {
            Minute.Minimum.Value.Should().BeLessThan( Minute.Maximum.Value );
            Minute.Maximum.Value.Should().BeGreaterThan( Minute.Minimum.Value );
        }

        [Fact]
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

        [Fact]
        public static void TestRoman() {
            const UInt32 a = 0;
            const UInt32 b = 1;
            const UInt32 c = 1234;
            const UInt32 d = 3999;
            const UInt32 e = 4000;
            const UInt32 f = 5000;

            $"{a} {a.ToRoman().Should().Be( String.Empty )}".Log();
            $"{b} {b.ToRoman().Should().Be( "I" )}".Log();
            $"{c} {c.ToRoman()}".Log();
            $"{d} {d.ToRoman()}".Log();
            $"{e} {e.ToRoman()}".Log();
            $"{f} {f.ToRoman()}".Log();
        }

        [Fact]
        public static void TestSecond() {
            Second.Minimum.Value.Should().BeLessThan( Second.Maximum.Value );
            Second.Maximum.Value.Should().BeGreaterThan( Second.Minimum.Value );
        }

        [Fact]
        public static void TestSimilarities() {
            var reasons = new ConcurrentQueue<String>();
            var test1 = "hi".Similarity( "hello", reasons );
            $"test1 was {test1}".Log();
        }

        //[Fact]
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