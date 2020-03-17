﻿// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "WWWManager.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "WWWManager.cs" was last formatted by Protiguous on 2020/03/16 at 2:55 PM.

namespace Librainian.Internet {

    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks.Dataflow;
    using JetBrains.Annotations;

    [Obsolete( message: "Needs rewriting" )]
    public class WwwManager {

        public static readonly ThreadLocal<WebClient> WebClients = new ThreadLocal<WebClient>( valueFactory: () => {
            var webClient = new WebClient();
            webClient.Headers.Add( name: "user-agent", value: "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15" );

            return webClient;
        }, trackAllValues: true );

        public BufferBlock<Tuple<Uri, String>> DownloadedStrings { get; }

        public ActionBlock<String> StringsToDownload { get; }

        //public void
        public WwwManager() {
            this.StringsToDownload = new ActionBlock<String>( action: StartDownloadingString );
            this.DownloadedStrings = new BufferBlock<Tuple<Uri, String>>();
        }

        private static void StartDownloadingString( [CanBeNull] String? address ) {
            if ( Uri.TryCreate( uriString: address, uriKind: UriKind.Absolute, result: out var uri ) ) {
                var webclient = WebClients.Value;
                var stringTaskAsync = webclient.DownloadStringTaskAsync( address: uri );

                //stringTaskAsync.ContinueWith( task => this.DownloadedStrings.TryPost( new Tuple<Uri, String>( uri, stringTaskAsync.Result ) ), continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion );
            }
        }
    }
}