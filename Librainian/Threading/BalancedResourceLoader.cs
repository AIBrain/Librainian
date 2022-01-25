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
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "BalancedResourceLoader.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Threading;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;

public class BalancedResourceLoader<T> : IResourceLoader<T> {

	private Int32 _index;

	public BalancedResourceLoader( params IResourceLoader<T>[] resourceLoaders ) : this( resourceLoaders as IList<IResourceLoader<T>> ) {
	}

	public BalancedResourceLoader( IList<IResourceLoader<T>> resourceLoaders ) =>
		this.ResourceLoaders = resourceLoaders ?? throw new ArgumentEmptyException( nameof( resourceLoaders ) );

	private Object Lock { get; } = new();

	private Queue<(TaskCompletionSource<T>, CancellationToken)> Queue { get; } = new();

	private IList<IResourceLoader<T>> ResourceLoaders { get; }

	public Int32 Available => this.ResourceLoaders.Sum( r => r.Available );

	public Int32 Count => this.ResourceLoaders.Sum( r => r.Count );

	public Int32 MaxConcurrency => this.ResourceLoaders.Sum( r => r.MaxConcurrency );

	private Boolean GetOrQueue( out Task<T>? resource, Boolean queueOnFailure, CancellationToken cancelToken ) {
		var i = this._index;

		while ( true ) {
			if ( i >= this.ResourceLoaders.Count ) {
				i = 0;
			}

			if ( this.ResourceLoaders[ i ].TryGet( out resource, cancelToken ) ) {
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

				this.Queue.Enqueue( (tcs, cancelToken) );

				resource = tcs.Task;
			}

			return false;
		}
	}

	private void OnResourceLoaded( Task<T> task ) {
		if ( task is null ) {
			throw new ArgumentEmptyException( nameof( task ) );
		}

		Task<T> resource;
		(TaskCompletionSource<T>, CancellationToken) tuple;

		lock ( this.Lock ) {
			if ( this.Queue.Count == 0 ) {
				return;
			}

			tuple = this.Queue.Peek();

			if ( !this.GetOrQueue( out resource, false, tuple.Item2 ) ) {
				return;
			}

			this.Queue.Dequeue();
		}

		resource.ContinueWith( t => tuple.Item1.SetFromTask( t ) );
	}

	public Task<T> GetAsync( CancellationToken cancelToken = new() ) {
		lock ( this.Lock ) {
			this.GetOrQueue( out var resource, true, cancelToken );

			return resource;
		}
	}

	public Boolean TryGet( out Task<T>? resource, CancellationToken cancelToken = default ) {
		lock ( this.Lock ) {
			return this.GetOrQueue( out resource, false, cancelToken );
		}
	}
}