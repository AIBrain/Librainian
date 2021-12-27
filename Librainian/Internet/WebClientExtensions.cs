// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "WebClientExtensions.cs" last formatted on 2020-08-14 at 8:35 PM.

namespace Librainian.Internet;

using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Logging;

/// <summary>
///     <para>Extension methods for working with WebClient asynchronously.</para>
///     <remarks>Some of these extensions might be originally copyright by Microsoft Corporation.</remarks>
/// </summary>
public static class WebClientExtensions {

	/// <summary>
	///     <para>Provide to each thread its own <see cref="WebClient" />.</para>
	///     <para>Do NOT use Dispose on these clients.</para>
	/// </summary>
	public static ThreadLocal<Lazy<WebClient>> ThreadSafeWebClients { get; } =
		new( () => new Lazy<WebClient>( () => new WebClient() ), true );

	/// <summary>
	///     <para>Register to cancel the <paramref name="client" /> with a <see cref="CancellationToken" />.</para>
	///     <para>if a cancellationToken is not passed in, then nothing happens with the <paramref name="client" />.</para>
	/// </summary>
	/// <param name="client"></param>
	/// <param name="cancellationToken"></param>
	/// <copyright>Protiguous</copyright>
	public static WebClient Add( this WebClient client, CancellationToken cancellationToken ) {
		if ( client is null ) {
			throw new ArgumentEmptyException( nameof( client ) );
		}

		cancellationToken.Register( client.CancelAsync );

		return client;
	}

	/// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
	/// <param name="webClient">The WebClient.</param>
	/// <param name="address">The URI from which to download data.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>A Task that contains the downloaded data.</returns>
	public static Task<Byte[]?> DownloadDataTaskAsync( this WebClient webClient, String address, CancellationToken cancellationToken ) {
		if ( webClient is null ) {
			throw new ArgumentEmptyException( nameof( webClient ) );
		}

		if ( String.IsNullOrWhiteSpace( address ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( address ) );
		}

		return DownloadDataTaskAsync( webClient.Add( cancellationToken ), new Uri( address ) );
	}

	/// <summary>Downloads the resource with the specified URI as a byte array, asynchronously.</summary>
	/// <param name="webClient">The WebClient.</param>
	/// <param name="address">The URI from which to download data.</param>
	/// <returns>A Task that contains the downloaded data.</returns>
	public static async Task<Byte[]?> DownloadDataTaskAsync( this WebClient webClient, Uri address ) {
		if ( webClient is null ) {
			throw new ArgumentEmptyException( nameof( webClient ) );
		}

		if ( address is null ) {
			throw new ArgumentEmptyException( nameof( address ) );
		}

		try {
			return await webClient.DownloadDataTaskAsync( address ).ConfigureAwait( false );
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		return default( Byte[] );
	}

	/// <summary>This seems to work great!</summary>
	/// <param name="webClient"></param>
	/// <param name="address"></param>
	/// <param name="fileName"></param>
	/// <param name="progress"></param>
	public static async Task DownloadFileTaskAsync(
		this WebClient webClient,
		Uri address,
		String fileName,
		IProgress<(Int64 BytesReceived, Int32 ProgressPercentage, Int64 TotalBytesToReceive)>? progress
	) {
		if ( webClient is null ) {
			throw new ArgumentEmptyException( nameof( webClient ) );
		}

		if ( address is null ) {
			throw new ArgumentEmptyException( nameof( address ) );
		}

		if ( String.IsNullOrWhiteSpace( fileName ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( fileName ) );
		}

		var tcs = new TaskCompletionSource<Object?>( address, TaskCreationOptions.RunContinuationsAsynchronously );

		void CompletedHandler( Object? cs, AsyncCompletedEventArgs ce ) {
			if ( ce.UserState != tcs ) {
				return;
			}

			if ( ce.Error != null ) {
				tcs.TrySetException( ce.Error );
			}
			else if ( ce.Cancelled ) {
				tcs.TrySetCanceled();
			}
			else {
				tcs.TrySetResult( null );
			}
		}

		void ProgressChangedHandler( Object ps, DownloadProgressChangedEventArgs pe ) {
			if ( pe.UserState == tcs ) {
				progress?.Report( (pe.BytesReceived, pe.ProgressPercentage, pe.TotalBytesToReceive) );
			}
		}

		try {
			webClient.DownloadFileCompleted += CompletedHandler;
			webClient.DownloadProgressChanged += ProgressChangedHandler;
			webClient.DownloadFileAsync( address, fileName, tcs );
			await tcs.Task.ConfigureAwait( false );
		}
		finally {
			webClient.DownloadFileCompleted -= CompletedHandler;
			webClient.DownloadProgressChanged -= ProgressChangedHandler;
		}
	}

	/// <summary>
	///     A thread-local (threadsafe) <see cref="WebClient" />.
	///     <para>Do NOT use Dispose on these clients.</para>
	/// </summary>
	public static WebClient Instance() => ThreadSafeWebClients.Value!.Value;

	/// <summary>
	///     <para>Register to cancel the <paramref name="client" /> after a <paramref name="timeout" />.</para>
	/// </summary>
	/// <param name="client"></param>
	/// <param name="timeout"></param>
	/// <copyright>Protiguous</copyright>
	public static WebClient SetTimeout( this WebClient client, TimeSpan timeout ) {
		if ( client is null ) {
			throw new ArgumentEmptyException( nameof( client ) );
		}

		using var cancel = new CancellationTokenSource( timeout );

		cancel.Token.Register( client.CancelAsync );

		return client;
	}

	/// <summary>Register to cancel the <paramref name="client" /> after a <paramref name="timeout" />.</summary>
	/// <param name="client"></param>
	/// <param name="timeout"></param>
	/// <param name="cancellationToken"></param>
	/// <copyright>Protiguous</copyright>
	public static WebClient SetTimeoutAndCancel( this WebClient client, TimeSpan timeout, CancellationToken cancellationToken ) {
		if ( client is null ) {
			throw new ArgumentEmptyException( nameof( client ) );
		}

		return client.Add( cancellationToken ).SetTimeout( timeout );
	}
}