// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "WWWManager.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/WWWManager.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

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

        public WwwManager() {
            this.StringsToDownload = new ActionBlock<String>( address => this.StartDownloadingString( address ) );
            this.DownloadedStrings = new BufferBlock<Tuple<Uri, String>>();
        }

        public BufferBlock<Tuple<Uri, String>> DownloadedStrings { get; }

        public ActionBlock<String> StringsToDownload { get; }

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