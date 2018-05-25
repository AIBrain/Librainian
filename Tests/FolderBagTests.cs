// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FolderBagTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/LibrainianTests/FolderBagTests.cs" was last formatted by Protiguous on 2018/05/24 at 7:36 PM.

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