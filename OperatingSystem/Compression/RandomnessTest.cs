// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "RandomnessTest.cs",
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
// "Librainian/Librainian/RandomnessTest.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.OperatingSystem.Compression {

    using System;
    using System.Diagnostics;
    using Maths;
    using NUnit.Framework;

    [TestFixture]
    public static class RandomnessTest {

        public static RandomnessFeeding RandomnessFeeding;

        //[OneTimeTearDown]
        public static void Done() {
            using ( RandomnessFeeding ) { }
        }

        //[OneTimeSetUp]
        public static void Init() => RandomnessFeeding = new RandomnessFeeding();

        [Test]
        public static Boolean RunSimulation() {
            var buffer = new Byte[( UInt32 )Constants.Sizes.OneMegaByte]; //one megabyte
            var bufferLength = buffer.LongLength;
            var randem = Randem.ThreadSafeRandom;

            var counter = 10;

            while ( counter-- > 0 ) {
                Debug.WriteLine( $"Generating {bufferLength} bytes of data.." );
                randem.Value.Value.NextBytes( buffer );

                Debug.WriteLine( $"Feeding {bufferLength} bytes of data into compressor..." );
                var before = RandomnessFeeding.HowManyBytesFed;
                RandomnessFeeding.FeedItData( buffer );
                var after = RandomnessFeeding.HowManyBytesFed;

                RandomnessFeeding.Report();
            }

            return true;
        }
    }
}