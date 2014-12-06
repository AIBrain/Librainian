#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Diagnostical.cs" was last cleaned by Rick on 2014/09/02 at 3:44 AM
#endregion

namespace Librainian {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Annotations;
    using Collections;
    using FluentAssertions;
    using Maths;
    using Measurement.Time;
    using Measurement.Time.Clocks;
    using NUnit.Framework;
    using Parsing;
    using Threading;

    [TestFixture]
    public static class Diagnostical {
        [TestFixtureSetUp]
        public static void Setup() {
        }

        [TestFixtureTearDown]
        public static void TearDown() {
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

        [DebuggerStepThrough]
        public static void BreakIfFalse( this Boolean condition, String message = "" ) {
            if ( condition ) {
                return;
            }
            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( message );
            }
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        [DebuggerStepThrough]
        public static void BreakIfTrue( this Boolean condition, String message = "" ) {
            if ( !condition ) {
                return;
            }
            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( message );
            }
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        /// <summary>
        ///     Gets the number of frames in the <see cref="StackTrace" />
        /// </summary>
        /// <param name="obj"> </param>
        /// <returns> </returns>
        public static int FrameCount( this Object obj ) {
            return ( new StackTrace( false ) ).FrameCount;
        }

        /// <summary>
        ///     Force a memory garbage collection on generation0 and generation1 objects.
        /// </summary>
        public static void Garbage() {
            var before = GC.GetTotalMemory( forceFullCollection: false );
            ThreadingExtensions.Wrap( () => GC.Collect( generation: 1, mode: GCCollectionMode.Optimized, blocking: false ) );
            var after = GC.GetTotalMemory( forceFullCollection: false );

            if ( after < before ) {
                Report.Info( String.Format( "{0} bytes freed by the GC.", before - after ));
            }
        }

        /// <summary>
        ///     TODO replace this with a proper IoC container.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        [DebuggerStepThrough]
        public static void Error( [CanBeNull] this Exception exception, [CanBeNull] String message = "", [CanBeNull] [CallerMemberName] String memberName = "", [CanBeNull] [CallerFilePath] String sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0 ) {
#if DEBUG
            if ( !String.IsNullOrEmpty( message ) ) {
                Debug.WriteLine( "[{0}]", message );
            }
            Debug.Indent();
            Debug.WriteLine( "[Method: {0}]", memberName );
            if ( exception != null ) {
                Debug.WriteLine( "[Exception: {0}]", exception.Message );
                Debug.WriteLine( "[In: {0}]", exception.Source );
                Debug.WriteLine( "[Msg: {0}]", exception.Message );
                Debug.WriteLine( "[Source: {0}]", sourceFilePath );
                Debug.WriteLine( "[Line: {0}]", sourceLineNumber );
            }
            Debug.Unindent();
#else

            if ( !String.IsNullOrEmpty( message ) ) {
                Trace.WriteLine( "[{0}]", message );
            }
            Trace.Indent();
            Trace.WriteLine( "[Method: {0}]", memberName );
            if ( exception != null ) {
                Trace.WriteLine( "[Exception: {0}]", exception.Message );
                Trace.WriteLine( "[In: {0}]", exception.Source );
                Trace.WriteLine( "[Msg: {0}]", exception.Message );
                Trace.WriteLine( "[Source: {0}]", sourceFilePath );
                Trace.WriteLine( "[Line: {0}]", sourceLineNumber );
            }
            Trace.Unindent();
#endif

            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        [Test]
        public static void TestWordVsGuid( WordToGuidAndGuidToWord wordToGuidAndGuidToWord ) {
            var g = new Guid( @"bddc4fac-20b9-4365-97bf-c98e84697012" );
            wordToGuidAndGuidToWord[ "AIBrain" ] = g;
            wordToGuidAndGuidToWord[ g ].Same( "AIBrain" ).BreakIfFalse();
        }

        [Test]
        public static void TestSimilarities() {
            var reasons = new ConcurrentQueue< String >();
            var test1 = "hi".Similarity( "hello", ref reasons );
            String.Format( "test1 was {0}", test1 ).WriteLine();
        }

        [Test]
        public static void TestRandems() {
            var ints = new ConcurrentBag< int >();
            var processorCount = Environment.ProcessorCount;
            Parallel.ForEach( source: 1.To( processorCount ), parallelOptions: ThreadingExtensions.Parallelism, body: i => Randem.AddToList( ints ) );
            if ( !ints.Duplicates().Any() ) {
                return;
            }
            ints.RemoveAll();
            Parallel.ForEach( 1.To( processorCount ), ThreadingExtensions.Parallelism, i => Randem.AddToList( ints ) );
            if ( !ints.Duplicates().Any() ) {
                return;
            }
            String.Format( "WARNING: Duplicate Randem.Next() found in static test!" ).WriteLine();
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        [Test]
        public static Boolean TestRoman() {
            short a = 0;
            short b = 1;
            short c = 1234;
            short d = 3999;
            short e = 4000;

            String.Format( "{0} {1}", a, a.ToRoman().Should() ).WriteLine();
            String.Format( "{0} {1}", b, b.ToRoman() ).WriteLine();
            String.Format( "{0} {1}", c, c.ToRoman() ).WriteLine();
            String.Format( "{0} {1}", d, d.ToRoman() ).WriteLine();
            String.Format( "{0} {1}", e, e.ToRoman() ).WriteLine();

            return true;
        }

        [Test]
        public static Boolean PassProbabilityTest() {
            var lower = new List< bool >();
            var probability = -0.33f;
            for ( var i = 0 ; i < 1048576 * 10 ; i++ ) {
                lower.Add( probability.Probability() );
            }

            var higher = new List< bool >();
            probability = 0.123f;
            for ( var i = 0 ; i < 1048576 * 10 ; i++ ) {
                higher.Add( probability.Probability() );
            }

            lower.RemoveAll( b => !b );
            higher.RemoveAll( b => !b );

            return higher.Count > lower.Count;
        }

        [Test]
        public static void TestNumberConversions() {

            Console.WriteLine( "Calculating..." );

            var test = BigDecimal.Parse( "0" );
            test.Should().Be( BigDecimal.Zero );

            test += BigDecimal.Parse( "1" );
            test.Should().Be( BigDecimal.One );

            test += BigDecimal.Parse( "0.000123" );
            ( ( Decimal )test ).Should().Be( 1.000123m );

            test += BigDecimal.Parse( "-10.000123" );
            ( ( Decimal )test ).Should().Be( -9M );

            test += BigDecimal.Parse( "11.1234567890" );
            ( ( Decimal )test ).Should().Be( 2.123456789M );

            test += BigDecimal.Parse( "-0.12342345" );
            ( ( Decimal )test ).Should().Be( 2.000033339M );

            test += BigDecimal.Parse( "1.1234567890" );
            ( ( Decimal )test ).Should().Be( 3.123490128M );

            test += BigDecimal.Parse( "-001.1234567890" );
            ( ( Decimal )test ).Should().Be( 2.000033339M );

            var test2 = BigDecimal.Parse( "111111.111111" );
            ( ( Decimal )test2 ).Should().Be( 111111.111111M );

            test2 = test2 * test2;
            ( ( Decimal )test2 ).Should().Be( 12345679012.320987654321M );

            test2 -= BigDecimal.Parse( "12345678970.320987654321" );

            var answer = test2.ToString();
            Console.WriteLine( "The Answer is {0}.", answer );

            Console.WriteLine( "Now, please input the question." );
        }

        [Test]
        public static void TestHour() {
            Hour.Minimum.Value.Should().BeLessThan( Hour.Maximum.Value );
            Hour.Maximum.Value.Should().BeGreaterThan( Hour.Minimum.Value );
        }

        [Test]
        public static void TestMinute() {
            Minute.Minimum.Value.Should().BeLessThan( Minute.Maximum.Value );
            Minute.Maximum.Value.Should().BeGreaterThan( Minute.Minimum.Value );
        }

        [Test]
        public static void TestSecond() {
            Second.Minimum.Value.Should().BeLessThan( Second.Maximum.Value );
            Second.Maximum.Value.Should().BeGreaterThan( Second.Minimum.Value );
        }

        [Test]
        public static void TestMillisecond() {
            Millisecond.Minimum.Value.Should().BeLessThan( Millisecond.Maximum.Value );
            Millisecond.Maximum.Value.Should().BeGreaterThan( Millisecond.Minimum.Value );
        }

        public static Boolean HasConsoleBeenAllocated { get; set; }
    }
}
