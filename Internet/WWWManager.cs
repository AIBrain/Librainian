// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "WWWManager.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/WWWManager.cs" was last formatted by Protiguous on 2018/05/24 at 7:17 PM.

namespace Librainian.Internet {

    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Threading;

    public class WwwManager {

        public static readonly ThreadLocal<WebClient> WebClients = new ThreadLocal<WebClient>( () => {
            var webClient = new WebClient();
            webClient.Headers.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15" );

            return webClient;
        }, true );

        public BufferBlock<Tuple<Uri, String>> DownloadedStrings { get; }

        public ActionBlock<String> StringsToDownload { get; }

        public WwwManager() {
            this.StringsToDownload = new ActionBlock<String>( address => this.StartDownloadingString( address ) );
            this.DownloadedStrings = new BufferBlock<Tuple<Uri, String>>();
        }

        //public void

        private void StartDownloadingString( String address ) {
            if ( Uri.TryCreate( address, UriKind.Absolute, out var uri ) ) {
                var webclient = WebClients.Value;
                var stringTaskAsync = webclient.DownloadStringTaskAsync( uri );
                stringTaskAsync.ContinueWith( task => this.DownloadedStrings.TryPost( new Tuple<Uri, String>( uri, stringTaskAsync.Result ) ), continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion );
            }
        }
    }
}