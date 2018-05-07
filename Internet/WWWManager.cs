// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/WWWManager.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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

        public BufferBlock<Tuple<Uri, String>> DownloadedStrings {
            get;
        }

        public ActionBlock<String> StringsToDownload {
            get;
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