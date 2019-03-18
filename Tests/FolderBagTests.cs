// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FolderBagTests.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "LibrainianTests", "FolderBagTests.cs" was last formatted by Protiguous on 2019/03/17 at 11:06 AM.

namespace LibrainianTests {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management;
    using FluentAssertions;
    using Librainian.ComputerSystem.Devices;
    using Librainian.Measurement.Time;
    using Librainian.OperatingSystem.FileSystem;
    using Librainian.Persistence;
    using Newtonsoft.Json;
    using NUnit.Framework;

    [TestFixture]
    public static class FolderBagTests {

        public static void OutputRamInformation() {
            var searcher = new ManagementObjectSearcher( "select * from Win32_PhysicalMemory" );

            var ram = 1;

            foreach ( var o in searcher.Get() ) {
                var managementObject = ( ManagementObject ) o;
                Console.WriteLine( $"RAM #{ram}:" );

                foreach ( var property in managementObject.Properties ) {

                    //if (property.Value != null) {
                    Console.WriteLine( $"{property.Name} = {property.Value}" );

                    //}
                }

                Console.WriteLine( "---------------------------------" );
                Console.WriteLine();

                ram++; // Increment our ram chip count
            }
        }

        [Test]
        public static void TestRAMInfo() => OutputRamInformation();

        [Test]
        public static void TestStorageAndRetrieval() {
            var counter = 0L;
            var watch = Stopwatch.StartNew();
            var pathTree = new FolderBag();

            foreach ( var drive in Disk.GetDrives().Where( drive => drive.Info.IsReady && drive.RootDirectory.StartsWith( "C" ) ).Take( 1 ) ) {
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
            File.WriteAllLines( temp.ContainingingFolder() + @"\allLines.txt", allPaths.Select( folder => folder.FullName ) );
        }
    }
}