// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "FolderBagTests.cs" last touched on 2021-04-30 at 11:34 AM by Protiguous.

namespace LibrainianUnitTests;

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Librainian.ComputerSystem.Devices;
using Librainian.FileSystem;
using Librainian.Measurement.Time;
using NUnit.Framework;

[TestFixture]
public static class FolderBagTests {

	[Test]
	public static async Task TestStorageAndRetrieval() {
		var counter = 0L;
		var watch = Stopwatch.StartNew();
		var pathTree = new FolderBag();

		await foreach ( var drive in Disk.GetDrives( CancellationToken.None ).Take( 2 ) ) {
			var root = new Folder( $"{drive.DriveLetter}:{Path.DirectorySeparatorChar}" );
			Debug.WriteLine( root );

			await foreach ( var folder in root.EnumerateFolders( "*.*", SearchOption.TopDirectoryOnly, CancellationToken.None ).Take( 2 ) ) {
				pathTree.FoundAnotherFolder( folder );
				counter++;
			}
		}

		var allPaths = pathTree.ToList();
		watch.Stop();

		Assert.True( counter == allPaths.Count );
		Debug.WriteLine( $"Found & stored {counter} folders in {watch.Elapsed.Simpler()}." );

		var temp = Document.GetTempDocument().ContainingingFolder();

		await File.WriteAllLinesAsync( $@"{temp}\allLines.txt", allPaths.Select( folder => folder.FullPath ) ).ConfigureAwait( false );
	}

}