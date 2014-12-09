// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/RandomnessTest.cs" was last cleaned by Rick on 2014/12/09 at 5:56 AM

namespace Librainian.IO.Compression {

    using System;
    using System.Diagnostics;
    using Annotations;
    using Maths;
    using NUnit.Framework;
    using Threading;

    [TestFixture]
    public static class RandomnessTest {
        public static RandomnessFeeeding RandomnessFeeeding;

        [TestFixtureTearDown]
        public static void Done() => RandomnessFeeeding.Dispose();

        [TestFixtureSetUp]
        [UsedImplicitly]
        public static void Init() => RandomnessFeeeding = new RandomnessFeeeding();

        [Test]
        [UsedImplicitly]
        public static Boolean RunSimulation() {
            var buffer = new Byte[ ( UInt32 )MathExtensions.OneMegaByte ]; //one megabyte
            var bufferLength = buffer.LongLength;
            var randem = Randem.ThreadSafeRandom;

            var counter = 10;

            while ( counter-- > 0 ) {
                Debug.WriteLine( "Generating {0} bytes of data..", bufferLength );
                randem.Value.NextBytes( buffer );

                Debug.WriteLine( "Feeding {0} bytes of data into compressor...", bufferLength );
                var before = RandomnessFeeeding.HowManyBytesFed;
                RandomnessFeeeding.FeedItData( buffer );
                var after = RandomnessFeeeding.HowManyBytesFed;

                RandomnessFeeeding.Report();
            }

            return true;
        }
    }
}