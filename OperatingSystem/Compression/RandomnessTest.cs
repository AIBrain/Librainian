// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/RandomnessTest.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

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
            using ( RandomnessFeeding ) {
            }
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