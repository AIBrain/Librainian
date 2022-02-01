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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Downloader.cs" last formatted on 2022-01-25 at 5:38 AM by Protiguous.

namespace Librainian.Internet;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Logging;
using Utilities;

public class Downloader {

	/// <summary>ctor</summary>
	/// <param name="timeout"></param>
	/// <exception cref="InvalidOperationException">Thrown when the <see cref="WebClient" /> is busy.</exception>
	public Downloader( TimeSpan timeout ) {
		this.Id = Guid.NewGuid();

		Internet.WebClients.Value ??= new HttpClientWithTimeout( timeout );
		this.Client = Internet.WebClients.Value ?? throw new NullException( nameof( Internet.WebClients ) );
	}

	public HttpClientWithTimeout Client { get; }

	/// <summary>The unique identifier assigned to this download.</summary>
	public Guid Id { get; }

	public virtual void Cancel() {
		try {
			this.Client.CancelPendingRequests();
		}
		catch ( Exception exception ) {
			exception.Log();
		}
	}

	[NeedsTesting]
	public async Task<(Status status, Stream? stream)> DownloadStream( Uri source, CancellationToken cancellationToken ) {
		try {
			await using var stream = await this.Client.GetStreamAsync( source, cancellationToken ).ConfigureAwait( false );
			stream.Seek( 0, SeekOrigin.Begin ); //TODO Needed?

			return (Status.Good, stream);
		}
		catch ( HttpRequestException exception ) {
			exception.Log();

			return (Status.Exception, default( Stream? ));
		}
		finally {
			this.Cancel();
		}
	}

	[NeedsTesting]
	public async Task<(HttpStatusCode? responseCode, UInt64? fileLength)> GetContentLength( Uri source, CancellationToken cancellationToken ) {
		try {
			using var responseMessage = await this.Client.GetAsync( source, HttpCompletionOption.ResponseHeadersRead, cancellationToken ).ConfigureAwait( false );
			var statusCode = responseMessage.EnsureSuccessStatusCode();

			return (statusCode.StatusCode, ( UInt64? )responseMessage.RequestMessage?.Content?.Headers.ContentLength);
		}
		catch ( HttpRequestException exception ) {
			exception.Log();

			return (exception.StatusCode, default( UInt64? ));
		}
		finally {
			this.Cancel();
		}
	}
}