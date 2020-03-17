// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "BackgroundThreadQueue.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "BackgroundThreadQueue.cs" was last formatted by Protiguous on 2020/03/16 at 3:02 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using JetBrains.Annotations;
    using Utilities;

    public class BackgroundThreadQueue<T> : ABetterClassDispose {

        private VolatileBoolean _quit;

        private Thread thread;

        [NotNull]
        private BlockingCollection<T> MessageQueue { get; } = new BlockingCollection<T>();

        private CancellationToken Token { get; set; }

        private void ProcessQueue( [NotNull] Action<T> action ) {
            if ( action is null ) {
                throw new ArgumentNullException( paramName: nameof( action ) );
            }

            try {
                var consume = this.MessageQueue.GetConsumingEnumerable( cancellationToken: this.Token );

                if ( consume is null || this._quit ) {
                    return;
                }

                foreach ( var item in consume ) {
                    if ( this._quit ) {
                        return; //check after blocking
                    }

                    action( obj: item );

                    if ( this._quit ) {
                        return; //check before blocking
                    }
                }
            }
            catch ( OperationCanceledException ) { }
            catch ( ObjectDisposedException ) { }
        }

        /// <summary>Same as <see cref="Enqueue" />.</summary>
        /// <param name="message"></param>
        public void Add( [CanBeNull] T message ) => this.MessageQueue.Add( item: message, cancellationToken: this.Token );

        public void Cancel() {
            this._quit = true;
            this.MessageQueue.CompleteAdding();
        }

        public override void DisposeManaged() => this.Cancel();

        /// <summary>Same as <see cref="Add" />.</summary>
        /// <param name="message"></param>
        public void Enqueue( [CanBeNull] T message ) => this.MessageQueue.Add( item: message, cancellationToken: this.Token );

        /// <summary></summary>
        /// <param name="each">Action to perform (poke into <see cref="MessageQueue" />).</param>
        /// <param name="token"></param>
        public void Start( [NotNull] Action<T> each, CancellationToken token ) {
            if ( each is null ) {
                throw new ArgumentNullException( paramName: nameof( each ) );
            }

            this.Token = token;

            this.thread = new Thread( start: () => this.ProcessQueue( action: each ) ) {

                IsBackground = true
            };

            this.thread.Start();
        }

    }

}