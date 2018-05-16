// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DefragTests.cs",
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
// "Librainian/Librainian/DefragTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.IO;
    using NUnit.Framework;
    using OperatingSystem;

    [TestFixture]
    public static class DefragTests {

        private static Document SourceDocument { get; set; }

        private static Document TargetDocument { get; set; }

        private static Folder TargetFolder { get; set; }

        //[OneTimeSetUp]
        public static void Setup() {
            SourceDocument = new Document( Path.Combine( Windows.WindowsSystem32Folder.Value.FullName, "mrt.exe" ) );

            TargetFolder = Folder.GetTempFolder();

            TargetDocument = new Document( Path.Combine( TargetFolder.FullName, "mrt.exe" ) );
            Console.WriteLine( SourceDocument.FullPathWithFileName );

            while ( TargetDocument.Exists() ) { TargetDocument.Delete(); }

            File.Copy( SourceDocument.FullPathWithFileName, TargetDocument.FullPathWithFileName );
        }

        //[OneTimeTearDown]
        public static void TearDown() => TargetDocument.Delete();

        [Test]
        public static void Test_something() {

            //var bob = new Document( new Uri("http://www.google.com/") );

            //var uri = new Uri( TargetDocument.FullPathWithFileName );

            //var bob = IOWrapper.GetFileMap( TargetDocument.FullPathWithFileName );

            //var frank = ( Int64[,] ) bob;

            //var lcn = new LinkedList<Int64 >(  );
            //var vcn = new LinkedList<Int64 >(  );

            //foreach ( Int64[,] a in bob. ) {
            //    lcn.AddLast( a[ 0, 0 ] );
            //    vcn.AddLast( a[ 1, 1 ] );
            //}

            //Console.WriteLine( lcn.Count );
        }
    }
}