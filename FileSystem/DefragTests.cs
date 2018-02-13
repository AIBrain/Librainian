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
// "Librainian/DefragTests.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.IO;
    using NUnit.Framework;
    using OperatingSystem;

    [TestFixture]
    public static class DefragTests {

        private static Document SourceDocument {
            get; set;
        }

        private static Document TargetDocument {
            get; set;
        }

        private static Folder TargetFolder {
            get; set;
        }

        //[OneTimeSetUp]
        public static void Setup() {
            SourceDocument = new Document( Path.Combine( Windows.WindowsSystem32Folder.Value.FullName, "mrt.exe" ) );

            TargetFolder = Folder.GetTempFolder();

            TargetDocument = new Document( Path.Combine( TargetFolder.FullName, "mrt.exe" ) );
            Console.WriteLine( SourceDocument.FullPathWithFileName );

            while ( TargetDocument.Exists() ) {
                TargetDocument.Delete();
            }
            File.Copy( SourceDocument.FullPathWithFileName, TargetDocument.FullPathWithFileName );
        }

        //[OneTimeTearDown]
        public static void TearDown() {
            TargetDocument.Delete();
        }

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