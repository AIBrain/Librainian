// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "BalancedResourceLoader.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "BalancedResourceLoader.cs" was last formatted by Protiguous on 2020/03/16 at 3:12 PM.

namespace Librainian.Threading {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public class BalancedResourceLoader<T> : IResourceLoader<T> {

        public Int32 Available => this._resourceLoaders.Sum( selector: r => r.Available );

        public Int32 Count => this._resourceLoaders.Sum( selector: r => r.Count );

        public Int32 MaxConcurrency => this._resourceLoaders.Sum( selector: r => r.MaxConcurrency );

        [CanBeNull]
        public Task<T> GetAsync( CancellationToken cancelToken = new CancellationToken() ) {
            lock ( this._lock ) {
                this.GetOrQueue( resource: out var resource, cancelToken: cancelToken, queueOnFailure: true );

                return resource;
            }
        }

        public Boolean TryGet( [CanBeNull] out Task<T> resource, CancellationToken cancelToken = new CancellationToken() ) {
            lock ( this._lock ) {
                return this.GetOrQueue( resource: out resource, cancelToken: cancelToken, queueOnFailure: false );
            }
        }

        private Int32 _index;

        [NotNull]
        private Object _lock { get; } = new Object();

        [NotNull]
        private Queue<(TaskCompletionSource<T>, CancellationToken)> _queue { get; } = new Queue<(TaskCompletionSource<T>, CancellationToken)>();

        [NotNull]
        [ItemNotNull]
        private IList<IResourceLoader<T>> _resourceLoaders { get; }

        public BalancedResourceLoader( [NotNull] params IResourceLoader<T>[] resourceLoaders ) : this( resourceLoaders: resourceLoaders as IList<IResourceLoader<T>> ) { }

        public BalancedResourceLoader( [NotNull] IList<IResourceLoader<T>> resourceLoaders ) =>
            this._resourceLoaders = resourceLoaders ?? throw new ArgumentNullException( paramName: nameof( resourceLoaders ) );

        private Boolean GetOrQueue( [CanBeNull] out Task<T> resource, CancellationToken cancelToken, Boolean queueOnFailure ) {
            var i = this._index;

            while ( true ) {
                if ( i >= this._resourceLoaders.Count ) {
                    i = 0;
                }

                if ( this._resourceLoaders[ index: i ].TryGet( resource: out resource, cancelToken: cancelToken ) ) {
                    resource.ContinueWith( continuationAction: this.OnResourceLoaded, cancellationToken: cancelToken );

                    this._index++;

                    return true;
                }

                i++;

                if ( i != this._index ) {
                    continue;
                }

                if ( queueOnFailure ) {
                    var tcs = new TaskCompletionSource<T>( creationOptions: TaskCreationOptions.RunContinuationsAsynchronously );
                    cancelToken.Register( callback: () => tcs.TrySetCanceled() );

                    this._queue.Enqueue( item: ( tcs, cancelToken ) );

                    resource = tcs.Task;
                }

                return default;
            }
        }

        private void OnResourceLoaded( [NotNull] Task<T> task ) {
            if ( task is null ) {
                throw new ArgumentNullException( paramName: nameof( task ) );
            }

            Task<T> _resource;
            (TaskCompletionSource<T>, CancellationToken) _tuple;

            lock ( this._lock ) {
                if ( this._queue.Count == 0 ) {
                    return;
                }

                _tuple = this._queue.Peek();

                if ( !this.GetOrQueue( resource: out _resource, cancelToken: _tuple.Item2, queueOnFailure: false ) ) {
                    return;
                }

                this._queue.Dequeue();
            }

            _resource?.ContinueWith( continuationAction: t => {
                if ( _tuple.Item1 is null ) {
                    return;
                }

                if ( t is null ) {
                    return;
                }

                _tuple.Item1.SetFromTask( task: t );
            } );
        }

    }

}