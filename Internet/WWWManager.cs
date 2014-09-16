#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/WWWManager.cs" was last cleaned by Rick on 2014/08/19 at 1:27 PM
#endregion

namespace Librainian.Internet {
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Threading;

    public class WWWManager {
        public static readonly ThreadLocal< WebClient > WebClients = new ThreadLocal< WebClient >( () => {
                                                                                                       var webClient = new WebClient();
                                                                                                       webClient.Headers.Add( "user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15" );
                                                                                                       return webClient;
                                                                                                   }, true );

        public WWWManager() {
            this.StringsToDownload = new ActionBlock< String >( address => this.StartDownloadingString( address ) );
            this.DownloadedStrings = new BufferBlock< Tuple< Uri, String > >();
        }

        public ActionBlock< String > StringsToDownload { get; private set; }

        public BufferBlock< Tuple< Uri, String > > DownloadedStrings { get; private set; }

        //public void 

        private void StartDownloadingString( String address ) {
            Uri uri;
            if ( Uri.TryCreate( address, UriKind.Absolute, out uri ) ) {
                var webclient = WebClients.Value;
                var stringTaskAsync = webclient.DownloadStringTaskAsync( uri );
                stringTaskAsync.ContinueWith( task => this.DownloadedStrings.TryPost( new Tuple< Uri, String >( uri, stringTaskAsync.Result ) ), continuationOptions: TaskContinuationOptions.OnlyOnRanToCompletion );
            }
        }
    }
}
