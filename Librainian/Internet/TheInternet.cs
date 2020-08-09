namespace Librainian.Internet {

	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Logging;
	using Maths.Numbers;
	using OperatingSystem.FileSystem;

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
		/// <returns></returns>
		[ItemCanBeNull]
		public static async Task<IDocument> DownloadAsync(
			[NotNull] Uri address,
			TimeSpan timeOut,
			[CanBeNull] IProgress<ZeroToOne> reportProgress = null,
			[CanBeNull] ICredentials credentials = null,
			[CanBeNull] Action<Uri, WebExceptionStatus> onWebException = null
		) {
			if ( address is null ) {
				throw new ArgumentNullException( nameof( address ) );
			}

			try {
				reportProgress?.Report( ZeroToOne.MinValue );

				var tempDocument = Document.GetTempDocument();

				// ReSharper disable once UseObjectOrCollectionInitializer
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
				reportProgress?.Report( ZeroToOne.MaxValue );
			}

			return null;
		}

		[ItemNotNull]
		public static IEnumerable<Document> FindFile( [NotNull] String filename, [NotNull] IEnumerable<String> locationClues ) {
			if ( locationClues is null ) {
				throw new ArgumentNullException( nameof( locationClues ) );
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