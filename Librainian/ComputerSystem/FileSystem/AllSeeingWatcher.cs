// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AllSeeingWatcher.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "AllSeeingWatcher.cs" was last formatted by Protiguous on 2018/07/10 at 8:53 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections.Concurrent;
	using System.IO;
	using Collections;
	using JetBrains.Annotations;
	using Magic;

	/// <summary>
	/// </summary>
	public class AllSeeingWatcher : ABetterClassDispose {

		[ItemNotNull]
		[NotNull]
		private ConcurrentList<FileSystemWatcher> FileWatchers { get; } = new ConcurrentList<FileSystemWatcher>();

		public ConcurrentDictionary<DateTime, FileSystemEventArgs> Changed { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 512 );

		public ConcurrentDictionary<DateTime, FileSystemEventArgs> Created { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 512 );

		public ConcurrentDictionary<DateTime, FileSystemEventArgs> Deleted { get; } = new ConcurrentDictionary<DateTime, FileSystemEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 512 );

		public ConcurrentDictionary<DateTime, RenamedEventArgs> Renamed { get; } = new ConcurrentDictionary<DateTime, RenamedEventArgs>( concurrencyLevel: Environment.ProcessorCount, capacity: 512 );

		private void OnChanged( Object sender, FileSystemEventArgs args ) => this.Changed[ DateTime.UtcNow ] = args;

		private void OnCreated( Object sender, FileSystemEventArgs args ) => this.Created[ DateTime.UtcNow ] = args;

		private void OnDeleted( Object sender, FileSystemEventArgs args ) => this.Deleted[ DateTime.UtcNow ] = args;

		private void OnRenamed( Object sender, RenamedEventArgs args ) => this.Renamed[ DateTime.UtcNow ] = args;

		public override void DisposeManaged() {
			this.Stop();
			base.DisposeManaged();
		}

		public void Start() {
			var drives = DriveInfo.GetDrives();

			foreach ( var drive in drives ) {
				var watcher = new FileSystemWatcher( drive.RootDirectory.FullName ) {
					IncludeSubdirectories = true,
					NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.LastWrite,
					InternalBufferSize = UInt16.MaxValue
				};

				watcher.Deleted += this.OnDeleted;

				watcher.Renamed += this.OnRenamed;

				watcher.Changed += this.OnChanged;

				watcher.Created += this.OnCreated;

				this.FileWatchers.Add( watcher );
				( "Added " + nameof( AllSeeingWatcher ) + " to drive " + drive.RootDirectory ).Info();

				watcher.EnableRaisingEvents = true;
			}
		}

		public void Stop() {
			foreach ( var watcher in this.FileWatchers ) {
				using ( watcher ) { }
			}
		}
	}
}