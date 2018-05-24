// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FolderBagTests.cs",
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
// "Librainian/LibrainianTests/FolderBagTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:51 PM.

namespace LibrainianTests {

    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Librainian.ComputerSystems.FileSystem;
    using Librainian.Measurement.Time;
    using Librainian.Persistence;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public static class FolderBagTests {

        [Test]
        public static void TestStorageAndRetrieval() {
            var counter = 0L;
            var watch = StopWatch.StartNew();
            var pathTree = new FolderBag();

            foreach ( var drive in Drive.GetDrives().Where( drive => drive.Info.IsReady && drive.RootDirectory.StartsWith( "C" ) ).Take( 1 ) ) {
                var root = new Folder( drive.DriveLetter + ":" + Path.DirectorySeparatorChar );

                foreach ( var folder in root.BetterGetFolders() ) {
                    pathTree.FoundAnotherFolder( folder );
                    counter++;
                }
            }

            var allPaths = pathTree.ToList();
            watch.Stop();

            counter.Should().Be( allPaths.LongCount() );
            Console.WriteLine( $"Found & stored {counter} folders in {watch.Elapsed.Simpler()}." );

            var temp = Document.GetTempDocument();
            pathTree.TrySave( temp, formatting: Formatting.None );
            File.WriteAllLines( temp.Folder + @"\allLines.txt", allPaths.Select( folder => folder.FullName ) );
        }
    }
}