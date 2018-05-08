// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/TheInternet.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Internet {

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Media;
	using System.Net;
	using System.Threading.Tasks;
	using FileSystem;
	using JetBrains.Annotations;
	using Maths.Numbers;
	using Measurement.Time;
	using NUnit.Framework;
	using Threading;

	public static class TheInternet {

		/// <summary>
		///     <para>Attempt to download the address to a local temp file.</para>
		///     <para>Reports progress via <paramref name="reportProgress" /> by a <seealso cref="ZeroToOne" />.</para>
		/// </summary>
		/// <param name="address"></param>
		/// <param name="reportProgress"></param>
		/// <param name="inProgress"></param>
		/// <param name="timeOut"></param>
		/// <param name="credentials"></param>
		/// <param name="onWebException"></param>
		/// <returns></returns>
		[ItemCanBeNull]
		public static async Task<Document> DownloadAsync( [NotNull] Uri address, TimeSpan timeOut, [CanBeNull] IProgress<ZeroToOne> reportProgress = null, VolatileBoolean inProgress = null, [CanBeNull] ICredentials credentials = null, [CanBeNull] Action<Uri, WebExceptionStatus> onWebException = null ) {
			if ( address is null ) {
				throw new ArgumentNullException( nameof( address ) );
			}
			try {
				if ( inProgress != null ) {
					inProgress.Value = true;
				}

				reportProgress?.Report( ZeroToOne.MinValue );

				var tempDocument = Document.GetTempDocument();

				// ReSharper disable once UseObjectOrCollectionInitializer
				var webclient = new WebClient { Credentials = credentials };

				webclient.DownloadProgressChanged += ( sender, args ) => {
					var progress = args.BytesReceived / ( Double )args.TotalBytesToReceive;
					reportProgress?.Report( progress );
				};

				var timeoutTask = Task.Delay( timeOut );
				var downloadTask = webclient.DownloadFileTaskAsync( address, tempDocument.FullPathWithFileName );

				var task = await Task.WhenAny( timeoutTask, downloadTask );
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
					exception2.More();
				}
			}
			catch ( Exception exception ) {
				exception.More();
			}
			finally {
				reportProgress?.Report( ZeroToOne.MaxValue );
				if ( inProgress != null ) {
					inProgress.Value = false;
				}
			}
			return null;
		}

		public static IEnumerable<Document> FindFile( String filename, IEnumerable<String> locationClues ) {
			foreach ( var locationClue in locationClues ) {
				if ( !Uri.TryCreate( locationClue, UriKind.Absolute, out var internetAddress ) ) {
					continue;
				}

				//TODO this /totally/ is not finished yet.
				
				yield return new Document( internetAddress );   //should download file to a document in the user's temp folder.
			}
		}
	}

	public static class TheInternetTests {

		public static SoundPlayer Player { get; } = new SoundPlayer();

		[Test]
		public static void Test1() {
			var inprogress = new VolatileBoolean();
			var creds = new NetworkCredential( "AIBrain", @"hP&Y@bYsM5qT0tr" );
			var bob = TheInternet.DownloadAsync( new Uri( "https://www.freesound.org/people/BDWRekordings.com/sounds/98104/" ), Seconds.Ten, null, inprogress, creds, OnWebException ).Result;

			if ( null != bob ) {
				Player.Stream = File.OpenRead( bob.FullPathWithFileName );
				try {
					Player.PlaySync();
				}
				catch ( Exception exception ) {
					exception.More();
				}
			}
		}

		private static void OnWebException( Uri uri, WebExceptionStatus webExceptionStatus ) {
			Console.WriteLine( uri );
			Console.WriteLine( webExceptionStatus );
		}
	}
}