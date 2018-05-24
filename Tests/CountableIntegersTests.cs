// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CountableIntegersTests.cs" belongs to Rick@AIBrain.org and
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
//
// "Librainian/LibrainianTests/CountableIntegersTests.cs" was last formatted by Protiguous on 2018/05/23 at 10:27 PM.

namespace LibrainianTests {

    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Librainian.Maths;
    using Librainian.Maths.Numbers;
    using Librainian.Measurement.Time;
    using Librainian.Threading;
    using NUnit.Framework;

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
            var bob = new Action( () => Parallel.Invoke( () => Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                var key = Randem.NextString( 2 );
                Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                var key = Randem.NextString( 2 );
                Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                var key = Randem.NextString( 2 );
                Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ) ) );

            TimeSpan timeTaken = bob.TimeStatement();
            Debug.WriteLine( timeTaken.Simpler() );
        }

        [Test]
        public static void TestSubtracting() {
            var bob = new Action( () => Parallel.Invoke( () => Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                var key = Randem.NextString( 2 );
                Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                var key = Randem.NextString( 2 );
                Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, ThreadingExtensions.CPUIntensive, l => {
                var key = Randem.NextString( 2 );
                Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ) ) );

            TimeSpan timeTaken = bob.TimeStatement();
            Debug.WriteLine( timeTaken.Simpler() );
        }
    }
}