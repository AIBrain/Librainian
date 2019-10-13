// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ResourceScanner.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "ResourceScanner.cs" was last formatted by Protiguous on 2019/10/11 at 9:16 AM.

namespace Librainian.Persistence {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Magic;
    using Threading;

    public class ResourceScanner : ABetterClassDispose {

        private TaskCompletionSource<Status> CompletionSource { get; } = new TaskCompletionSource<Status>();

        [CanBeNull]
        private Task DiscoveryTask { get; }

        [NotNull]
        private TimeTracker TimeTracker { get; } = new TimeTracker();

        [NotNull]
        public CancellationTokenSource CancellationSource { get; }

        public Boolean Waiting { get; private set; }

        /// <summary>
        ///     await on this after creation.
        /// </summary>
        public Task<Status> Completion => this.CompletionSource.Task;

        /// <summary>
        ///     Starts scanning the resource via <paramref name="discovery" /> function parameter.
        /// </summary>
        /// <param name="discovery">The function to run in a task.</param>
        /// <param name="cancellationSource"></param>
        /// <param name="timeout">Defaults to <see cref="Timeout.InfiniteTimeSpan" /></param>
        public ResourceScanner( [NotNull] Func<Status> discovery, [NotNull] CancellationTokenSource cancellationSource, TimeSpan? timeout = null ) {
            if ( discovery == null ) {
                throw new ArgumentNullException( paramName: nameof( discovery ) );
            }

            if ( cancellationSource == null ) {
                throw new ArgumentNullException( paramName: nameof( cancellationSource ) );
            }

            this.CancellationSource =
                CancellationTokenSource.CreateLinkedTokenSource( cancellationSource.Token, new CancellationTokenSource( timeout ?? Timeout.InfiniteTimeSpan ).Token );

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
        ///         which
        ///         in turn calls
        ///         <see cref="ABetterClassDispose.DisposeManaged" /> and <see cref="ABetterClassDispose.DisposeNative" />.
        ///     </para>
        ///     <para>
        ///         <example>Example usage: <code>using ( this.Sink ) { this.Sink=null; }</code></example>
        ///     </para>
        /// </summary>
        public override void DisposeManaged() {

            if ( this.DiscoveryTask?.IsDone() == false ) {
                this.RequestStop();
                this.Wait();
            }
        }

        public Boolean IsWaiting() => this.Waiting;

        public void RequestStop() => this.CancellationSource.Cancel( throwOnFirstException: false );

        /// <summary>
        ///     Blocks while waiting for the <see cref="DiscoveryTask" /> to finish.
        /// </summary>
        public void Wait() {
            this.Waiting = true;
            this.DiscoveryTask?.Wait( this.CancellationSource.Token ); //eh? or ok?
        }

        /// <summary>
        ///     awaits for the <see cref="CompletionSource" /> to finish.
        /// </summary>
        /// <returns></returns>
        public async Task WaitAsync() {
            this.Waiting = true;
            await this.Completion;
        }

    }

}