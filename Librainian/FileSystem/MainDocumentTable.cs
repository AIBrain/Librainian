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
// File "MainDocumentTable.cs" last touched on 2021-03-07 at 11:00 PM by Protiguous.

#nullable enable

namespace Librainian.FileSystem {

	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Persistence;

	/// <summary>A persisted cache of all found <see cref="Document" />.</summary>
	public static class MainDocumentTable {

		public static CancellationTokenSource CTS { get; } = new();

		/// <summary>DocumentInfos[StringPath]=DocumentInfo</summary>
		public static PersistTable<String, DocumentInfo?> DocumentInfos { get; } = new( Environment.SpecialFolder.CommonApplicationData, nameof( DocumentInfos ) );

		/// <summary>Documents[StringPath]=Document</summary>
		public static PersistTable<String, Document> Documents { get; } = new( Environment.SpecialFolder.CommonApplicationData,
			Path.GetFileNameWithoutExtension( Process.GetCurrentProcess().ProcessName ), nameof( Documents ) );

		public static StringKVPTable Settings { get; } = new( Environment.SpecialFolder.LocalApplicationData,
			Path.GetFileNameWithoutExtension( Process.GetCurrentProcess().ProcessName ), nameof( MainDocumentTable ) );

		/// <summary>Start searching in the givin <paramref name="folder" />, and update our "MFT" with found documents.</summary>
		/// <param name="folder"></param>
		/// <param name="folders"></param>
		/// <param name="reportDocument"></param>
		/// <returns></returns>
		[NotNull]
		public static async Task SearchAsync(
			[NotNull] IFolder folder,
			[CanBeNull] IProgress<IFolder>? folders = null,
			[CanBeNull] IProgress<Document>? reportDocument = null
		) {
			if ( CTS.IsCancellationRequested ) {
				return;
			}

			//Update the MFT
			await foreach ( var document in folder.EnumerateDocuments( "*.*", CTS.Token ) ) {
				if ( CTS.IsCancellationRequested ) {
					return;
				}

				reportDocument?.Report( document );
				Documents[ document.FullPath ] = document;

				var docinfo = new DocumentInfo( document );
				DocumentInfos[ document.FullPath ] = docinfo;
				await docinfo.ScanAsync( CTS.Token ).ConfigureAwait( false );
			}

			if ( CTS.IsCancellationRequested ) {
				return;
			}

			//And then scan down into any subfolders.
			await foreach ( var subFolder in folder.EnumerateFolders( "*.*", SearchOption.TopDirectoryOnly, CTS.Token ) ) {
				if ( CTS.IsCancellationRequested ) {
					return;
				}

				folders?.Report( subFolder );
				await SearchAsync( subFolder, folders, reportDocument ).ConfigureAwait( false );
			}
		}
	}
}