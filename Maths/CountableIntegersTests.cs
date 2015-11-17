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
// "Librainian/CountableIntegersTests.cs" was last cleaned by Rick on 2015/10/07 at 4:28 AM

namespace Librainian.Maths {

    using System;
    using System.Threading.Tasks;
    using Measurement.Time;
    using NUnit.Framework;
    using Threading;

    [TestFixture]
    public static class CountableIntegersTests {

        public static CountableBigIntegers<String> CountableBigIntegers {
            get; private set;
        }

        [OneTimeSetUp]
        public static void Setup() {
            CountableBigIntegers = new CountableBigIntegers<String>( readTimeout: Seconds.One, writeTimeout: Seconds.One );
        }

        [OneTimeTearDown]
        public static void TearDown() {
            using ( CountableBigIntegers ) {
            }
        }

        [Test]
        public static void TestAdding() {
            var bob = new Action( () => {
                Parallel.Invoke(
                    () => {
                        Parallel.For( 0, 1024 * 1024, ThreadingExtensions.ParallelOptions, l => {
                            CountableBigIntegers[ Randem.NextString( 3 ) ] += Randem.NextBigInteger( Randem.NextByte( 1, 255 ) );
                        } );
                    },
                    () => {
                        Parallel.For( 0, 1024 * 1024, ThreadingExtensions.ParallelOptions, l => {
                            CountableBigIntegers[ Randem.NextString( 3 ) ] -= Randem.NextBigInteger( Randem.NextByte( 1, 255 ) );
                        } );
                    } );


                CountableBigIntegers.Complete();

                Parallel.For( 0, 1024 * 1024, ThreadingExtensions.ParallelOptions, l => {
                    CountableBigIntegers[ Randem.NextString( 3 ) ] = Randem.NextBigInteger( Randem.NextByte( 1, 255 ) );
                } );
            } );
            TimeSpan timeTaken = bob.TimeStatement();
            Console.WriteLine( timeTaken.Simpler() );
        }

    }

}
