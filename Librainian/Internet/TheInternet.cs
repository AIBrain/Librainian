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

namespace Librainian.Internet {

	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Exceptions;
	using FileSystem;
	using Logging;
	using Maths.Numbers;

	public static class TheInternet {

		/// <summary>
		///     <para>Attempt to download the address to a local temp file.</para>
		///     <para>Reports progress via <paramref name="reportProgress" /> by a <see cref="ZeroToOne" />.</para>
		/// </summary>
		/// <param name="address"></param>
		/// <param name="reportProgress"></param>
		/// <param name="timeOut"></param>
		/// <param name="credentials"></param>
		/// <param name="onWebException"></param>
		public static async Task<IDocument?> DownloadAsync( Uri address, TimeSpan timeOut, IProgress<ZeroToOne>? reportProgress = null,
			ICredentials? credentials = null, Action<Uri, WebExceptionStatus>? onWebException = null ) {
			if ( address is null ) {
				throw new ArgumentEmptyException( nameof( address ) );
			}

			try {
				reportProgress?.Report( ZeroToOne.MinimumValue );

				var tempDocument = Document.GetTempDocument();

				using var webclient = new WebClient {
					Credentials = credentials
				};

				webclient.DownloadProgressChanged += ( sender, args ) => {
					var progress = args.BytesReceived / ( Double )args.TotalBytesToReceive;
					reportProgress?.Report( progress );
				};

				var timeoutTask = Task.Delay( timeOut );
				var downloadTask = webclient.DownloadFileTaskAsync( address, tempDocument.FullPath );

				var task = await Task.WhenAny( timeoutTask, downloadTask ).ConfigureAwait( false );

				if ( task.Id == timeoutTask.Id ) {
					webclient.CancelAsync();
				}

				return tempDocument;
			}
			catch ( WebException exception ) {
				try {
					onWebException?.Invoke( address, exception.Status );
				}
				catch ( Exception exception2 ) {
					exception2.Log();
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}
			finally {
				reportProgress?.Report( ZeroToOne.MaximumValue );
			}

			return default( IDocument );
		}

		public static IEnumerable<Document> FindFile( String filename, IEnumerable<String> locationClues ) {
			if ( locationClues is null ) {
				throw new ArgumentEmptyException( nameof( locationClues ) );
			}

			if ( String.IsNullOrWhiteSpace( filename ) ) {
				throw new ArgumentException( "Value cannot be null or whitespace.", nameof( filename ) );
			}

			foreach ( var locationClue in locationClues ) {
				if ( !Uri.TryCreate( locationClue, UriKind.Absolute, out var internetAddress ) ) {
					continue;
				}

				//TODO this /totally/ is not finished yet.

				yield return new Document( internetAddress.ToString() ); //should download file to a document in the user's temp folder.
			}
		}
	}
}