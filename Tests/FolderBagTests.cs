// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "LibrainianTests/FolderBagTests.cs" was last cleaned by Protiguous on 2018/05/08 at 10:25 PM

namespace LibrainianTests {

    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Librainian.FileSystem;
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
            pathTree.Save( temp, formatting: Formatting.None );
            File.WriteAllLines( temp.Folder + @"\allLines.txt", allPaths.Select( folder => folder.FullName ) );
        }
    }
}