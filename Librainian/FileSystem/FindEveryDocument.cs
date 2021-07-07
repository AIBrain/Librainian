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
// File "FindEveryDocument.cs" last touched on 2021-03-07 at 5:15 AM by Protiguous.

#nullable enable

namespace Librainian.FileSystem {

	using System;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using ComputerSystem.Devices;
	using Exceptions;
	using Measurement.Time;
	using Parsing;
	using Threadsafe;
	using Utilities;

	/// <summary>
	///     <para>Scan every drive.</para>
	///     <para>Scan every folder.</para>
	///     <para>Scan every document.</para>
	///     <para>await <see cref="StartScanning" /> to start the scan.</para>
	///     <para>Cancel scanning via <see cref="CancellationTokenSource" />.</para>
	/// </summary>
	public class FindEveryDocument : ABetterClassDispose {

		private VolatileBoolean _pauseScanning;

		private CancellationTokenSource CancellationTokenSource { get; } = new();

		/// <summary>
		///     A reference to the out parameter in the ctor.
		/// </summary>
		private BufferBlock<IDocument> DocumentsFound { get; }

		private ActionBlock<Disk>? DrivesFound { get; set; }

		private ActionBlock<IFolder>? FoldersFound { get; set; }

		public String? CurrentStatus { get; private set; }

		public IProgress<(Single counter, String message)> Progress { get; }

		public CancellationToken CancellationToken => this.CancellationTokenSource.Token;

		public FindEveryDocument( IProgress<(Single, String)> progress, out BufferBlock<IDocument> documentsFound ) {
			this.Progress = progress ?? throw new ArgumentEmptyException( nameof( progress ) );
			documentsFound = new BufferBlock<IDocument>();
			this.DocumentsFound = documentsFound;
		}

		private void SetCurrentStatus( String? message ) => this.CurrentStatus = message;

		/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
		public override void DisposeManaged() { }

		public void PauseScanning() => this._pauseScanning.Value = true;

		public void ResumeScanning() => this._pauseScanning.Value = false;

		public async Task StartScanning() {
			Int64 counter = 0;

			this.SetCurrentStatus( "Creating ActionBlocks.." );

			this.DrivesFound = new ActionBlock<Disk>( async disk => await ScanDisk( disk ).ConfigureAwait( false ) );

			this.FoldersFound = new ActionBlock<IFolder>( async parent => {
				await PauseWhilePaused().ConfigureAwait( false );

				await foreach ( var folder in parent.EnumerateFolders( "*.*", SearchOption.TopDirectoryOnly, this.CancellationToken ) ) {
					await AddFoundFolder( folder ).ConfigureAwait( false );
				}

				await foreach ( var document in parent.EnumerateDocuments( "*.*", this.CancellationToken ) ) {
					await AddFoundDocument( document ).ConfigureAwait( false );
				}
			} );

			ScanAllDisks();

			try {
				var drivesFound = this.DrivesFound;
				if ( drivesFound != null ) {
					await drivesFound.Completion.ConfigureAwait( false );
				}
			}
			catch ( TaskCanceledException ) { }

			try {
				var foldersFound = this.FoldersFound;
				if ( foldersFound != null ) {
					await foldersFound.Completion.ConfigureAwait( false );
				}
			}
			catch ( TaskCanceledException ) { }

			try {
				await this.DocumentsFound.Completion.ConfigureAwait( false );
			}
			catch ( TaskCanceledException ) { }

			void ScanAllDisks() {
				this.SetCurrentStatus( "Scanning over drives.." );
				foreach ( var drive in ParsingConstants.English.Alphabet.Uppercase.Select( ch => new Disk( ch ) ).Where( drive => drive.Exists() && drive.Info.IsReady ) ) {
					if ( this.CancellationToken.IsCancellationRequested ) {
						return;
					}

					this.SetCurrentStatus( $"Found drive {drive.DriveLetter.ToString().SmartQuote()}." );

					this.DrivesFound?.Post( drive );
				}

				this.DrivesFound?.Complete();
			}

			async Task ScanDisk( Disk disk ) {
				var root = new Folder( disk.Info.RootDirectory.FullName );
				this.SetCurrentStatus( $"Found root folder {root.FullPath.SmartQuote()}." );

				await PauseWhilePaused().ConfigureAwait( false );

				await foreach ( var folder in root.EnumerateFolders( "*.*", SearchOption.TopDirectoryOnly, this.CancellationToken ) ) {
					await PauseWhilePaused().ConfigureAwait( false );

					this.SetCurrentStatus( $"Found main folder {folder.FullPath.SmartQuote()}." );
					this.FoldersFound?.Post( folder );

					Interlocked.Increment( ref counter );
					this.Progress.Report( (counter, $"Found main folder {folder.FullPath.SmartQuote()}.") );
				}
			}

			async Task AddFoundFolder( IFolder folder ) {
				await PauseWhilePaused().ConfigureAwait( false );

				if ( this.CancellationToken.IsCancellationRequested ) {
					return;
				}

				this.SetCurrentStatus( $"Found sub folder {folder.FullPath.SmartQuote()}." );
				this.FoldersFound?.Post( folder );

				Interlocked.Increment( ref counter );
				this.Progress.Report( (counter, $"Found sub folder {folder.FullPath.SmartQuote()}.") );
			}

			async Task AddFoundDocument( IDocument document ) {
				await PauseWhilePaused().ConfigureAwait( false );
				if ( this.CancellationToken.IsCancellationRequested ) {
					return;
				}

				this.SetCurrentStatus( $"Found document {document.FullPath.SmartQuote()}." );
				this.DocumentsFound.Post( document );

				Interlocked.Increment( ref counter );

				this.Progress.Report( (counter, $"Found document {document.FullPath.SmartQuote()}.") );
			}

			async Task PauseWhilePaused() {
				if ( this._pauseScanning ) {
					this.SetCurrentStatus( "Paused.." );

					while ( this._pauseScanning ) {
						try {
							await Task.Delay( Seconds.One, this.CancellationToken ).ConfigureAwait( false );
						}
						catch ( TaskCanceledException ) { }
					}
					this.SetCurrentStatus( "Resuming.." );
				}
			}
		}

		public void StopScanning() {
			this.CancellationTokenSource.Cancel( false );
			this.DrivesFound.Complete();
			this.FoldersFound.Complete();
			this.ResumeScanning();
		}
	}
}