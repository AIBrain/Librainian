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
// "Librainian/PathSplitterTests.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public static class PathSplitterTests {

        [Test]
        public static void TestTheSplittingOrder() {
            const String example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";
            const String newExample = @"C:\recovered\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

            var bob = new PathSplitter( example );

            var reconstructed = bob.Recombined();

            reconstructed.FullPathWithFileName.Should().Be( example );

            bob.InsertRoot( @"C:\recovered" );
            reconstructed = bob.Recombined();

            reconstructed.FullPathWithFileName.Should().Be( newExample );

            Console.WriteLine( reconstructed.FullPathWithFileName );
        }
    }
}