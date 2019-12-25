// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "UsableSemaphoreSlim.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "UsableSemaphoreSlim.cs" was last formatted by Protiguous on 2019/10/23 at 11:41 AM.

namespace LibrainianCore.Threading {

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    public class UsableSemaphoreSlim : IUsableSemaphore {

        public void Dispose() {
            this._semaphore.Dispose();
        }

        [ItemNotNull]
        public async Task<IUsableSemaphoreWrapper> WaitAsync() {
            var wrapper = new UsableSemaphoreWrapper( this._semaphore );

            try {
                await wrapper.WaitAsync().ConfigureAwait( false );

                return wrapper;
            }
            catch ( Exception ) {
                wrapper.Dispose();

                throw;
            }
        }

        [NotNull]
        private readonly SemaphoreSlim _semaphore;

        public UsableSemaphoreSlim( Int32 initialCount ) => this._semaphore = new SemaphoreSlim( initialCount );

        public UsableSemaphoreSlim( Int32 initialCount, Int32 maxCount ) => this._semaphore = new SemaphoreSlim( initialCount, maxCount );

        private class UsableSemaphoreWrapper : IUsableSemaphoreWrapper {

            public TimeSpan Elapsed => this._stopwatch.Elapsed;

            public void Dispose() {
                if ( this._isDisposed ) {
                    return;
                }

                if ( this._stopwatch.IsRunning ) {
                    this._stopwatch.Stop();
                    this._semaphore.Release();
                }

                this._isDisposed = true;
            }

            [NotNull]
            private readonly SemaphoreSlim _semaphore;

            [NotNull]
            private readonly Stopwatch _stopwatch;

            private Boolean _isDisposed;

            public UsableSemaphoreWrapper( [NotNull] SemaphoreSlim semaphore ) {
                this._semaphore = semaphore ?? throw new ArgumentNullException( nameof( semaphore ) );
                this._stopwatch = new Stopwatch();
            }

            [NotNull]
            public Task WaitAsync() {
                if ( this._stopwatch.IsRunning ) {
                    throw new InvalidOperationException( "Already Initialized" );
                }

                this._stopwatch.Start();

                return this._semaphore.WaitAsync();
            }

        }

    }

}