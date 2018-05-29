// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PhysicalDiskTests.cs" was last cleaned by Rick on 2016/07/29 at 9:54 AM

namespace Librainian.FileSystem {

    using System;
    using Maths;
    using NUnit.Framework;

    [TestFixture]
    public static class PhysicalDiskTests {

        [Test]
        public static void TestAllDisks() {
            foreach ( var diskNumber in 0.To( 26 ) ) {
                var disk = new PhysicalDisk( diskNumber );
                Console.WriteLine( disk + " " + disk.SerialNumber + " " );
            }
        }
    }
}