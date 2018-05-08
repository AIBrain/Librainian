// Copyright 2016 Protiguous.
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
// "Librainian/BenchmarkTests.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Measurement {

    using System;
    using System.Threading;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public static class BenchmarkTests {

        [Test]
        public static void TestBenchmarking() {
            Action a = () => {
                Thread.Sleep( 11 );
            };
            Action b = () => {
                Thread.Sleep( 12 );
            };
            var result = a.WhichIsFaster( b );
            result.Should().Be( Benchmark.AorB.MethodA );

            a = () => {
                Thread.Sleep( 8 );
            };
            b = () => {
                Thread.Sleep( 8 );
            };
            result = a.WhichIsFaster( b );
            result.Should().Be( Benchmark.AorB.Same );

            a = () => {
                Thread.Sleep( 2 );
            };
            b = () => {
                Thread.Sleep( 1 );
            };
            result = a.WhichIsFaster( b );
            result.Should().Be( Benchmark.AorB.MethodB );

            a = () => {
                Thread.Sleep( 11 );
            };
            b = () => {
                Thread.Sleep( 12 );
            };
            result = a.WhichIsFaster( b );
            result.Should().Be( Benchmark.AorB.MethodA );
        }
    }
}