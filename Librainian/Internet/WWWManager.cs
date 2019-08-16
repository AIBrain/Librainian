// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WWWManager.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "WWWManager.cs" was last formatted by Protiguous on 2019/08/08 at 8:04 AM.

namespace Librainian.Internet {

    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks.Dataflow;

    [Obsolete( "Needs rewriting" )]
    public class WwwManager {

        public BufferBlock<Tuple<Uri, String>> DownloadedStrings { get; }

        public ActionBlock<String> StringsToDownload { get; }

        public static readonly ThreadLocal<WebClient> WebClients = new ThreadLocal<WebClient>( () => {
            var webClient = new WebClient();
            webClient.Headers.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15" );

            return webClient;
        }, true );

        //public void
        public WwwManager() {
            this.StringsToDownload = new ActionBlock<String>( StartDownloadingString );
            this.DownloadedStrings = new BufferBlock<Tuple<Uri, String>>();
        }

        private static void StartDownloadingString( String address ) {
            if ( Uri.TryCreate( address, UriKind.Absolute, out var uri ) ) {
                var webclient = WebClients.Value;
                var stringTaskAsync = webclient.DownloadStringTaskAsync( uri );

                //stringTaskAsync.ContinueWith( task => this.DownloadedStrings.TryPost( new Tuple<Uri, String>( uri, stringTaskAsync.Result ) ), continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion );
            }
        }
    }
}