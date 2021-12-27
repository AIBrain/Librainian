// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "BalancedResourceLoader.cs" last formatted on 2020-08-14 at 8:46 PM.

namespace Librainian.Threading;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;

public class BalancedResourceLoader<T> : IResourceLoader<T> {

	private Int32 _index;

	private Object _lock { get; } = new();

	private Queue<(TaskCompletionSource<T>, CancellationToken)> _queue { get; } = new();

	private IList<IResourceLoader<T>> _resourceLoaders { get; }

	public Int32 Available => this._resourceLoaders.Sum( r => r.Available );

	public Int32 Count => this._resourceLoaders.Sum( r => r.Count );

	public Int32 MaxConcurrency => this._resourceLoaders.Sum( r => r.MaxConcurrency );

	public BalancedResourceLoader( params IResourceLoader<T>[] resourceLoaders ) : this( resourceLoaders as IList<IResourceLoader<T>> ) { }

	public BalancedResourceLoader( IList<IResourceLoader<T>> resourceLoaders ) =>
		this._resourceLoaders = resourceLoaders ?? throw new ArgumentEmptyException( nameof( resourceLoaders ) );

	private Boolean GetOrQueue( out Task<T>? resource, Boolean queueOnFailure, CancellationToken cancelToken ) {
		var i = this._index;

		while ( true ) {
			if ( i >= this._resourceLoaders.Count ) {
				i = 0;
			}

			if ( this._resourceLoaders[i].TryGet( out resource, cancelToken ) ) {
				resource.ContinueWith( this.OnResourceLoaded, cancelToken );

				this._index++;

				return true;
			}

			i++;

			if ( i != this._index ) {
				continue;
			}

			if ( queueOnFailure ) {
				var tcs = new TaskCompletionSource<T>( TaskCreationOptions.RunContinuationsAsynchronously );
				cancelToken.Register( () => tcs.TrySetCanceled() );

				this._queue.Enqueue( (tcs, cancelToken) );

				resource = tcs.Task;
			}

			return false;
		}
	}

	private void OnResourceLoaded( Task<T> task ) {
		if ( task is null ) {
			throw new ArgumentEmptyException( nameof( task ) );
		}

		Task<T> _resource;
		(TaskCompletionSource<T>, CancellationToken) _tuple;

		lock ( this._lock ) {
			if ( this._queue.Count == 0 ) {
				return;
			}

			_tuple = this._queue.Peek();

			if ( !this.GetOrQueue( out _resource, false, _tuple.Item2 ) ) {
				return;
			}

			this._queue.Dequeue();
		}

		_resource.ContinueWith( t => _tuple.Item1.SetFromTask( t ) );
	}

	public Task<T> GetAsync( CancellationToken cancelToken = new() ) {
		lock ( this._lock ) {
			this.GetOrQueue( out var resource, true, cancelToken );

			return resource;
		}
	}

	public Boolean TryGet( out Task<T>? resource, CancellationToken cancelToken = default ) {
		lock ( this._lock ) {
			return this.GetOrQueue( out resource, false, cancelToken );
		}
	}
}