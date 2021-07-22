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
// File "ResourceScanner.cs" last formatted on 2020-08-14 at 8:44 PM.

namespace Librainian.Persistence {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Exceptions;
	using Threading;
	using Utilities;
	using Utilities.Disposables;

	public class ResourceScanner : ABetterClassDispose {

		private TaskCompletionSource<Status> CompletionSource { get; } = new( TaskCreationOptions.RunContinuationsAsynchronously );

		private Task DiscoveryTask { get; }

		private TimeTracker TimeTracker { get; } = new();

		public CancellationTokenSource CancellationSource { get; }

		public Boolean Waiting { get; private set; }

		/// <summary>await on this after creation.</summary>
		public Task<Status> Completion => this.CompletionSource.Task;

		/// <summary>Starts scanning the resource via <paramref name="discovery" /> function parameter.</summary>
		/// <param name="discovery">The function to run in a task.</param>
		/// <param name="cancellationSource"></param>
		/// <param name="timeout">Defaults to <see cref="Timeout.InfiniteTimeSpan" /></param>
		public ResourceScanner( Func<Status> discovery, CancellationTokenSource cancellationSource, TimeSpan? timeout = null ) {
			if ( discovery is null ) {
				throw new ArgumentEmptyException( nameof( discovery ) );
			}

			if ( cancellationSource is null ) {
				throw new ArgumentEmptyException( nameof( cancellationSource ) );
			}

			this.CancellationSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationSource.Token,
																					   new CancellationTokenSource( timeout ?? Timeout.InfiniteTimeSpan ).Token );

			this.TimeTracker.Started = DateTime.UtcNow;

			this.DiscoveryTask = Task.Run( discovery, this.CancellationSource.Token ).Then( code => {
				this.TimeTracker.Finished = DateTime.UtcNow;

				return code;
			} ).Then( code => this.CompletionSource.TrySetResult( code ) );
		}

		/// <summary>
		///     <para>If the <see cref="DiscoveryTask" /> has not finished (or cancelled or faulted),</para>
		///     <para>then requests a cancel on the <see cref="CancellationSource" />.</para>
		///     <para>Then waits (blocking) for the <see cref="DiscoveryTask" /> to complete.</para>
		///     <para>Dispose any disposable managed fields or properties.</para>
		///     <para>
		///         Providing the object inside a using construct will then call <see cref="ABetterClassDispose.Dispose()" />,
		///         which in turn calls
		///         <see cref="ABetterClassDispose.DisposeManaged" /> and <see cref="ABetterClassDispose.DisposeNative" />.
		///     </para>
		///     <para>
		///         <example>Example usage: <code>using ( this.Sink ) { this.Sink=null; }</code></example>
		///     </para>
		/// </summary>
		public override void DisposeManaged() {
			if ( !this.DiscoveryTask.IsDone() ) {
				this.RequestStop();
				this.Wait();
			}
		}

		public Boolean IsWaiting() => this.Waiting;

		public void RequestStop() => this.CancellationSource.Cancel( false );

		/// <summary>Blocks while waiting for the <see cref="DiscoveryTask" /> to finish.</summary>
		public void Wait() {
			this.Waiting = true;
			this.DiscoveryTask.Wait( this.CancellationSource.Token ); //TODO IDisposableAsync or IAsyncDisposable ???
		}

		/// <summary>awaits for the <see cref="CompletionSource" /> to finish.</summary>
		/// <returns></returns>
		public Task WaitAsync() {
			this.Waiting = true;

			return this.Completion;
		}
	}
}