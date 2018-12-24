// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FindEveryDocument.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "GROD", "FindEveryDocument.cs" was last formatted by Protiguous on 2018/11/03 at 7:54 PM.

namespace Librainian.ComputerSystem.FileSystem {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using JetBrains.Annotations;
	using Magic;
	using Parsing;

	public class FindEveryDocument : ABetterClassDispose {

		private BufferBlock<Document> DocumentsFound { get; }

		private ActionBlock<Disk> DrivesFound { get; set; }

		private ActionBlock<Folder> FoldersFound { get; set; }

		private String Status { get; set; }

		public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

		/// <summary>
		///     A list of drives that exist in the system.
		/// </summary>
		[NotNull]
		public List<Disk> PossibleDrives { get; } = new List<Disk>( ParsingExtensions.EnglishAlphabetUppercase.Length );

		public IProgress<Single> Progress { get; }

		public FindEveryDocument( [NotNull] BufferBlock<Document> documentsFound, [NotNull] IProgress<Single> progress ) {
			this.DocumentsFound = documentsFound ?? throw new ArgumentNullException( nameof( documentsFound ) );
			this.Progress = progress ?? throw new ArgumentNullException( nameof( progress ) );
		}

		[NotNull]
		public Task StartScanning() =>
			Task.Run( () => {

				foreach ( var ch in ParsingExtensions.EnglishAlphabetUppercase ) {
					var drive = new Disk( ch );

					if ( drive.Exists() && drive.Info.IsReady ) {
						this.PossibleDrives.Add( drive );
					}
				}

				this.PossibleDrives.TrimExcess();

				Int64 counter = 0;

				this.DrivesFound = new ActionBlock<Disk>( disk => {
					var root = new Folder( disk.Info.RootDirectory );

					Parallel.ForEach( root.BetterGetFolders().AsParallel(), folder => {
						if ( this.CancellationTokenSource.IsCancellationRequested ) {
							return;
						}

						this.Status = $"Found folder `{folder.FullName}`.";
						this.FoldersFound.Post( folder );
						Interlocked.Increment( ref counter );
						this.Progress.Report( counter );
					} );
				} );

				this.FoldersFound = new ActionBlock<Folder>( parent => {
					Parallel.ForEach( parent.BetterGetFolders().AsParallel(), folder => {
						if ( this.CancellationTokenSource.IsCancellationRequested ) {
							return;
						}

						this.Status = $"Found folder `{folder.FullName}`.";
						this.FoldersFound.Post( folder );
						Interlocked.Increment( ref counter );
						this.Progress.Report( counter );
					} );

					Parallel.ForEach( parent.GetDocuments().AsParallel(), document => {
						if ( this.CancellationTokenSource.IsCancellationRequested ) {
							return;
						}

						this.Status = $"Found document `{document.FullPathWithFileName}`.";
						this.DocumentsFound.Post( document );
						Interlocked.Increment( ref counter );

						//progress.Report(counter);
					} );
				} );
			}, this.CancellationTokenSource.Token );
	}
}