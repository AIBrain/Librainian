// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.FileSystem {

	using System;
	using System.Collections.Concurrent;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Collections.Lists;
	using Logging;
	using Utilities.Disposables;

	public class AllSeeingFileWatcher : ABetterClassDisposeAsync {

		private ConcurrentList<FileSystemWatcher> FileWatchers { get; } = new();

		public ConcurrentDictionary<DateTime, FileSystemEventArgs> Changed { get; } = new( Environment.ProcessorCount, 512 );

		public ConcurrentDictionary<DateTime, FileSystemEventArgs> Created { get; } = new( Environment.ProcessorCount, 512 );

		public ConcurrentDictionary<DateTime, FileSystemEventArgs> Deleted { get; } = new( Environment.ProcessorCount, 512 );

		public ConcurrentDictionary<DateTime, RenamedEventArgs> Renamed { get; } = new( Environment.ProcessorCount, 512 );

		private void OnChanged( Object? sender, FileSystemEventArgs? args ) => this.Changed[DateTime.UtcNow] = args;

		private void OnCreated( Object? sender, FileSystemEventArgs? args ) => this.Created[DateTime.UtcNow] = args;

		private void OnDeleted( Object? sender, FileSystemEventArgs? args ) => this.Deleted[DateTime.UtcNow] = args;

		private void OnRenamed( Object? sender, RenamedEventArgs? args ) => this.Renamed[DateTime.UtcNow] = args;

		public override async ValueTask DisposeManagedAsync() {
			this.Nop();
			await this.Stop().ConfigureAwait( false );
		}

		public void Start() {
			var drives = DriveInfo.GetDrives();

			foreach ( var drive in drives ) {
				var watcher = new FileSystemWatcher( drive.RootDirectory.FullName ) {
					IncludeSubdirectories = true,
					NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.LastWrite,
					InternalBufferSize = UInt16.MaxValue
				};

				watcher.Created += this.OnCreated;

				watcher.Changed += this.OnChanged;

				watcher.Renamed += this.OnRenamed;

				watcher.Deleted += this.OnDeleted;

				this.FileWatchers.Add( watcher );
				$"Added {nameof( AllSeeingFileWatcher )} to drive {drive.RootDirectory}.".Info();

				watcher.EnableRaisingEvents = true;
			}
		}

		public async Task Stop() {
			await foreach ( var watcher in this.FileWatchers.ToAsyncEnumerable() ) {
				using ( watcher ) { }
			}
		}
	}
}