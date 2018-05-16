// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "CountableIntegersTests.cs",
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
// "Librainian/Librainian/CountableIntegersTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths {

    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Measurement.Time;
    using Numbers;
    using NUnit.Framework;
    using Threading;

    [TestFixture]
    public static class CountableIntegersTests {

        public static Countable<String> Countable { get; } = new Countable<String>( readTimeout: Seconds.One, writeTimeout: Seconds.One );

        [OneTimeSetUp]
        public static void Setup() { }

        [OneTimeTearDown]
        public static void TearDown() {
            using ( Countable ) { }
        }

        [Test]
        public static void TestAdding() {
            var bob = new Action( () => {
                Parallel.Invoke( () => {
                    Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                        var key = Randem.NextString( 2 );
                        Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
                    } );
                }, () => {
                    Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                        var key = Randem.NextString( 2 );
                        Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
                    } );
                }, () => {
                    Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                        var key = Randem.NextString( 2 );
                        Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
                    } );
                } );
            } );

            TimeSpan timeTaken = bob.TimeStatement();
            Debug.WriteLine( timeTaken.Simpler() );
        }

        [Test]
        public static void TestSubtracting() {
            var bob = new Action( () => {
                Parallel.Invoke( () => {
                    Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                        var key = Randem.NextString( 2 );
                        Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
                    } );
                }, () => {
                    Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                        var key = Randem.NextString( 2 );
                        Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
                    } );
                }, () => {
                    Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                        var key = Randem.NextString( 2 );
                        Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
                    } );
                } );
            } );

            TimeSpan timeTaken = bob.TimeStatement();
            Debug.WriteLine( timeTaken.Simpler() );
        }
    }
}