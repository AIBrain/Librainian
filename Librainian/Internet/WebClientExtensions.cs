// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WebClientExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "WebClientExtensions.cs" was last formatted by Protiguous on 2018/07/10 at 9:11 PM.

namespace Librainian.Internet {

	using System;
	using System.IO;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using JetBrains.Annotations;
	using Parsing;
	using Threading;

	/// <summary>
	///     <para>Extension methods for working with WebClient asynchronously.</para>
	///     <para>Some of these extensions might be copyright (c) Microsoft Corporation.</para>
	/// </summary>
	public static class WebClientExtensions {

		/// <summary>
		///     <para>Register to cancel the <paramref name="client" /> with a <see cref="CancellationToken" />.</para>
		///     <para>if a token is not passed in, then nothing happens with the <paramref name="client" />.</para>
		/// </summary>
		/// <param name="client"></param>
		/// <param name="token"></param>
		/// <copyright>Protiguous</copyright>
		[NotNull]
		public static WebClient Add( [NotNull] this WebClient client, CancellationToken? token ) {
			if ( client == null ) { throw new ArgumentNullException( paramName: nameof( client ) ); }

			token?.Register( client.CancelAsync );

			return client;
		}

		/// <summary>
		///     <para>Register to cancel the <paramref name="client" /> after a <paramref name="timeout" />.</para>
		///     <para>If no timeout is passed in, then nothing happenswith the <paramref name="client" /></para>
		/// </summary>
		/// <param name="client"></param>
		/// <param name="timeout"></param>
		/// <copyright>Protiguous</copyright>
		[NotNull]
		public static WebClient Add( [NotNull] this WebClient client, TimeSpan? timeout ) {
			if ( client == null ) { throw new ArgumentNullException( paramName: nameof( client ) ); }

			if ( timeout.HasValue ) { new CancellationTokenSource( timeout.Value ).Token.Register( client.CancelAsync ); }

			return client;
		}

		/// <summary>
		///     Register to cancel the <paramref name="client" /> after a <paramref name="timeout" />.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="timeout"></param>
		/// <param name="token"></param>
		/// <copyright>Protiguous</copyright>
		[NotNull]
		public static WebClient Add( [NotNull] this WebClient client, TimeSpan? timeout, CancellationToken? token ) {
			if ( client == null ) { throw new ArgumentNullException( paramName: nameof( client ) ); }

			return client.Add( token ).Add( timeout );
		}

		/// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI from which to download data.</param>
		/// <param name="token"></param>
		/// <returns>A Task that contains the downloaded data.</returns>
		public static async Task<Byte[]> DownloadDataTask( [NotNull] this WebClient webClient, [NotNull] String address, CancellationToken token ) {
			if ( webClient is null ) { throw new ArgumentNullException( paramName: nameof( webClient ) ); }

			if ( String.IsNullOrWhiteSpace( value: address ) ) { throw new ArgumentException( message: "Value cannot be null or whitespace.", paramName: nameof( address ) ); }

			return await DownloadDataTask( webClient.Add( token ), new Uri( address ) ).NoUI();
		}

		/// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI from which to download data.</param>
		/// <returns>A Task that contains the downloaded data.</returns>
		public static async Task<Byte[]> DownloadDataTask( [NotNull] this WebClient webClient, [NotNull] Uri address ) {
			if ( webClient is null ) { throw new ArgumentNullException( paramName: nameof( webClient ) ); }

			if ( address is null ) { throw new ArgumentNullException( paramName: nameof( address ) ); }

			try { return await webClient.DownloadDataTaskAsync( address ).NoUI(); }
			catch ( Exception exception ) { exception.More(); }

			return null;
		}

		/// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI for which the stream should be opened.</param>
		/// <returns>A Task that contains the opened stream.</returns>
		public static Task<Stream> OpenReadTask( [NotNull] this WebClient webClient, TrimmedString address ) {
			if ( webClient == null ) { throw new ArgumentNullException( paramName: nameof( webClient ) ); }

			if ( address.IsEmpty ) { throw new ArgumentEmptyException( message: "Value cannot be null or whitespace.", paramName: nameof( address ) ); }

			return OpenReadTask( webClient, new Uri( address ) );
		}

		/// <summary>Opens a readable stream for the data downloaded from a resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI for which the stream should be opened.</param>
		/// <returns>A Task that contains the opened stream.</returns>
		public static Task<Stream> OpenReadTask( [NotNull] this WebClient webClient, [NotNull] Uri address ) {
			if ( webClient == null ) { throw new ArgumentNullException( paramName: nameof( webClient ) ); }

			if ( address == null ) { throw new ArgumentNullException( paramName: nameof( address ) ); }

			var taskCompletionSource = new TaskCompletionSource<Stream>( address );

			void Handler( Object sender, OpenReadCompletedEventArgs e ) => taskCompletionSource.HandleCompletion( e, () => e.Result, () => webClient.OpenReadCompleted -= Handler );

			webClient.OpenReadCompleted += Handler;

			try { webClient.OpenReadAsync( address, taskCompletionSource ); }
			catch ( Exception exception ) {
				webClient.OpenReadCompleted -= Handler;
				taskCompletionSource.TrySetException( exception );
			}

			return taskCompletionSource.Task;
		}

		/// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI for which the stream should be opened.</param>
		/// <param name="method">The HTTP method that should be used to open the stream.</param>
		/// <returns>A Task that contains the opened stream.</returns>
		public static Task<Stream> OpenWriteTask( [NotNull] this WebClient webClient, TrimmedString address, TrimmedString method ) => OpenWriteTask( webClient, new Uri( address ), method );

		/// <summary>Opens a writeable stream for uploading data to a resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI for which the stream should be opened.</param>
		/// <param name="method">The HTTP method that should be used to open the stream.</param>
		/// <returns>A Task that contains the opened stream.</returns>
		public static Task<Stream> OpenWriteTask( [NotNull] this WebClient webClient, [NotNull] Uri address, TrimmedString method ) {
			if ( webClient == null ) { throw new ArgumentNullException( paramName: nameof( webClient ) ); }

			if ( address == null ) { throw new ArgumentNullException( paramName: nameof( address ) ); }

			if ( method.IsEmpty ) { throw new ArgumentEmptyException( message: "Value cannot be empty.", paramName: nameof( method ) ); }

			var taskCompletionSource = new TaskCompletionSource<Stream>( address );

			void Handler( Object sender, OpenWriteCompletedEventArgs e ) => taskCompletionSource.HandleCompletion( e, () => e.Result, () => webClient.OpenWriteCompleted -= Handler );

			webClient.OpenWriteCompleted += Handler;

			// Start the async work
			try { webClient.OpenWriteAsync( address, method, taskCompletionSource ); }
			catch ( Exception exc ) {

				// If something goes wrong kicking off the async work, unregister the callback and
				// cancel the created task
				webClient.OpenWriteCompleted -= Handler;
				taskCompletionSource.TrySetException( exc );
			}

			// Return the task that represents the async operation
			return taskCompletionSource.Task;
		}

		/// <summary>Uploads data to the specified resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI to which the data should be uploaded.</param>
		/// <param name="method">The HTTP method that should be used to upload the data.</param>
		/// <param name="data">The data to upload.</param>
		/// <returns>A Task containing the data in the response from the upload.</returns>
		public static Task<Byte[]> UploadDataTask( this WebClient webClient, [NotNull] String address, String method, Byte[] data ) => UploadDataTask( webClient, new Uri( address ), method, data );

		/// <summary>Uploads data to the specified resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI to which the data should be uploaded.</param>
		/// <param name="method">The HTTP method that should be used to upload the data.</param>
		/// <param name="data">The data to upload.</param>
		/// <returns>A Task containing the data in the response from the upload.</returns>
		public static Task<Byte[]> UploadDataTask( [NotNull] this WebClient webClient, Uri address, String method, Byte[] data ) {

			// Create the task to be returned
			var tcs = new TaskCompletionSource<Byte[]>( address );

			// Setup the callback event handler
			void Handler( Object sender, UploadDataCompletedEventArgs e ) => tcs.HandleCompletion( e, () => e.Result, () => webClient.UploadDataCompleted -= Handler );

			webClient.UploadDataCompleted += Handler;

			// Start the async work
			try { webClient.UploadDataAsync( address, method, data, tcs ); }
			catch ( Exception exc ) {

				// If something goes wrong kicking off the async work, unregister the callback and
				// cancel the created task
				webClient.UploadDataCompleted -= Handler;
				tcs.TrySetException( exc );
			}

			// Return the task that represents the async operation
			return tcs.Task;
		}

		/// <summary>Uploads a file to the specified resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI to which the file should be uploaded.</param>
		/// <param name="method">The HTTP method that should be used to upload the file.</param>
		/// <param name="fileName">A path to the file to upload.</param>
		/// <returns>A Task containing the data in the response from the upload.</returns>
		public static Task<Byte[]> UploadFileTask( this WebClient webClient, [NotNull] String address, String method, String fileName ) => UploadFileTask( webClient, new Uri( address ), method, fileName );

		/// <summary>Uploads a file to the specified resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI to which the file should be uploaded.</param>
		/// <param name="method">The HTTP method that should be used to upload the file.</param>
		/// <param name="fileName">A path to the file to upload.</param>
		/// <returns>A Task containing the data in the response from the upload.</returns>
		public static Task<Byte[]> UploadFileTask( [NotNull] this WebClient webClient, Uri address, String method, String fileName ) {

			// Create the task to be returned
			var tcs = new TaskCompletionSource<Byte[]>( address );

			// Setup the callback event handler
			void Handler( Object sender, UploadFileCompletedEventArgs e ) => tcs.HandleCompletion( e, () => e.Result, () => webClient.UploadFileCompleted -= Handler );

			webClient.UploadFileCompleted += Handler;

			// Start the async work
			try { webClient.UploadFileAsync( address, method, fileName, tcs ); }
			catch ( Exception exc ) {

				// If something goes wrong kicking off the async work, unregister the callback and
				// cancel the created task
				webClient.UploadFileCompleted -= Handler;
				tcs.TrySetException( exc );
			}

			// Return the task that represents the async operation
			return tcs.Task;
		}

		/// <summary>Uploads data in a String to the specified resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI to which the data should be uploaded.</param>
		/// <param name="method">The HTTP method that should be used to upload the data.</param>
		/// <param name="data">The data to upload.</param>
		/// <returns>A Task containing the data in the response from the upload.</returns>
		public static Task<String> UploadStringTask( this WebClient webClient, [NotNull] String address, String method, String data ) => UploadStringTask( webClient, new Uri( address ), method, data );

		/// <summary>Uploads data in a String to the specified resource, asynchronously.</summary>
		/// <param name="webClient">The WebClient.</param>
		/// <param name="address">The URI to which the data should be uploaded.</param>
		/// <param name="method">The HTTP method that should be used to upload the data.</param>
		/// <param name="data">The data to upload.</param>
		/// <returns>A Task containing the data in the response from the upload.</returns>
		public static Task<String> UploadStringTask( [NotNull] this WebClient webClient, Uri address, String method, String data ) {

			// Create the task to be returned
			var tcs = new TaskCompletionSource<String>( address );

			// Setup the callback event handler
			void Handler( Object sender, UploadStringCompletedEventArgs e ) => tcs.HandleCompletion( e, () => e.Result, () => webClient.UploadStringCompleted -= Handler );

			webClient.UploadStringCompleted += Handler;

			// Start the async work
			try { webClient.UploadStringAsync( address, method, data, tcs ); }
			catch ( Exception exc ) {

				// If something goes wrong kicking off the async work, unregister the callback and
				// cancel the created task
				webClient.UploadStringCompleted -= Handler;
				tcs.TrySetException( exc );
			}

			// Return the task that represents the async operation
			return tcs.Task;
		}
	}
}