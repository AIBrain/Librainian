// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MasterDocumentTable.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "MasterDocumentTable.cs" was last formatted by Protiguous on 2018/11/21 at 10:26 PM.

namespace Librainian.OperatingSystem.FileSystem {

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Persistence;

	/// <summary>
	///     A persisted cache of all found <see cref="Document" />.
	/// </summary>
	public static class MasterDocumentTable {

		public static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

		/// <summary>
		///     DocumentInfos[StringPath]=DocumentInfo
		/// </summary>
		public static PersistTable<String, DocumentInfo> DocumentInfos { get; } = new PersistTable<String, DocumentInfo>( Environment.SpecialFolder.CommonApplicationData, nameof( DocumentInfos ) );

		/// <summary>
		///     Documents[StringPath]=Document
		/// </summary>
		public static PersistTable<String, Document> Documents { get; } =
			new PersistTable<String, Document>( Environment.SpecialFolder.CommonApplicationData, Path.GetFileNameWithoutExtension( Process.GetCurrentProcess().ProcessName ), nameof( Documents ) );

		public static StringKVPTable Settings { get; } =
			new StringKVPTable( Environment.SpecialFolder.LocalApplicationData, Path.GetFileNameWithoutExtension( Process.GetCurrentProcess().ProcessName ), nameof( MasterDocumentTable ) );

		/// <summary>
		///     Start searching in the givin <paramref name="folder" />, and update our "MFT" with found documents.
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="folders"></param>
		/// <param name="documents"></param>
		/// <returns></returns>
		[NotNull]
		public static Task SearchAsync( IFolder folder, [CanBeNull] IProgress<IFolder> folders = null, [CanBeNull] IProgress<Document> documents = null ) {
			var task = Task.Run( async () => {
				if ( CancellationTokenSource.IsCancellationRequested ) {
					return Task.FromResult( default( IFolder ) );   //TODO
				}

				//Find all documents in this folder...
				var files = folder.GetDocuments( "*.*" );

				//And update the MFT
				Parallel.ForEach( files.AsParallel(), document => {
					if ( CancellationTokenSource.IsCancellationRequested ) {
						return;
					}

					Documents[ document.FullPath ] = document;
					documents?.Report( document );
				} );

				if ( CancellationTokenSource.IsCancellationRequested ) {
					return Task.FromResult( default( IFolder ) );   //TODO
				}

				//And then scan down into any subfolders.
				foreach ( var subFolder in folder.BetterGetFolders( "*.*" ) ) {
					if ( CancellationTokenSource.IsCancellationRequested ) {
						return Task.FromResult( default( IFolder ) );
					}

					await SearchAsync( subFolder, folders, documents ).ConfigureAwait( false );

					folders?.Report( subFolder ); //best before or after await?
				}

				return Task.FromResult( default( IFolder ) );	//TODO
			} );

			return task;
		}
	}
}