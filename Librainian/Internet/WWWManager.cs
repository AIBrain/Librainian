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
// File "WWWManager.cs" last formatted on 2022-12-22 at 7:20 AM by Protiguous.

namespace Librainian.Internet;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks.Dataflow;

[Obsolete( "Needs rewriting" )]
public class WwwManager {

	public static readonly ThreadLocal<WebClient> WebClients = new( () => {
		var webClient = new WebClient();
		webClient.Headers.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15" );

		return webClient;
	}, true );

	//public void
	public WwwManager() {
		this.StringsToDownload = new ActionBlock<String>( StartDownloadingString );
		this.DownloadedStrings = new BufferBlock<Tuple<Uri, String>>();
	}

	public BufferBlock<Tuple<Uri, String>> DownloadedStrings { get; }

	public ActionBlock<String> StringsToDownload { get; }

	private static void StartDownloadingString( String? address ) {
		if ( Uri.TryCreate( address, UriKind.Absolute, out var uri ) ) {
			var webclient = WebClients.Value;
			var stringTaskAsync = webclient.DownloadStringTaskAsync( uri );

			//stringTaskAsync.ContinueWith( task => this.DownloadedStrings.TryPost( new Tuple<Uri, String>( uri, stringTaskAsync.Result ) ), continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion );
		}
	}
}