// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code, "DefragTests.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by automatic formatting.
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
// "Librainian/LibrainianTests/DefragTests.cs" was last formatted by Protiguous on 2018/05/20 at 7:07 PM.

namespace LibrainianTests {

    using System;
    using System.IO;
    using Librainian.ComputerSystems.FileSystem;
    using Librainian.OperatingSystem;
    using NUnit.Framework;

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