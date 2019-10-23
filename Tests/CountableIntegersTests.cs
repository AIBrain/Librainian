// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "CountableIntegersTests.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "LibrainianTests", "CountableIntegersTests.cs" was last formatted by Protiguous on 2019/03/17 at 11:06 AM.

namespace LibrainianTests {

    using System;
    using System.Threading.Tasks;
    using Librainian.Maths;
    using Librainian.Maths.Numbers;
    using Librainian.Measurement.Time;
    using Xunit;

    public static class CountableIntegersTests {

        public static Countable<String> Countable { get; } = new Countable<String>( readTimeout: Seconds.One, writeTimeout: Seconds.One );

        [Theory]
        public static void Setup() { }

        [Fact]
        public static void TestAdding() {
            var bob = new Action( () => Parallel.Invoke( () => Parallel.For( 0, 102400, l => {
                var key = Randem.NextString( 2 );
                Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, l => {
                var key = Randem.NextString( 2 );
                Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, l => {
                var key = Randem.NextString( 2 );
                Countable.Add( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ) ) );

            TimeSpan timeTaken = bob.TimeStatement();
            Console.WriteLine( timeTaken.Simpler() );
        }

        [Fact]
        public static void TestSubtracting() {
            var bob = new Action( () => Parallel.Invoke( () => Parallel.For( 0, 102400, l => {
                var key = Randem.NextString( 2 );
                Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, l => {
                var key = Randem.NextString( 2 );
                Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ), () => Parallel.For( 0, 102400, l => {
                var key = Randem.NextString( 2 );
                Countable.Subtract( key, Randem.NextBigInteger( Randem.NextByte( 1, 255 ) ) );
            } ) ) );

            TimeSpan timeTaken = bob.TimeStatement();
            Console.WriteLine( timeTaken.Simpler() );
        }
    }
}