// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EasierSemaphore.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "EasierSemaphore.cs" was last formatted by Protiguous on 2019/08/08 at 9:36 AM.

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using JetBrains.Annotations;

    public static class EasierSemaphore {

        /// <summary>Blocks the current thread until the current <see cref="System.Threading.WaitHandle" /> receives a signal.</summary>
        /// <returns>
        ///     <see langword="true" /> if the current instance receives a signal. If the current instance is never signaled,
        ///     <see cref="System.Threading.WaitHandle.WaitOne(System.Int32,System.Boolean)" /> never returns.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">The current instance has already been disposed. </exception>
        /// <exception cref="System.Threading.AbandonedMutexException">
        ///     The wait completed because a thread exited without
        ///     releasing a mutex. This exception is not thrown on Windows 98 or Windows Millennium Edition.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The current instance is a transparent proxy for a
        ///     <see cref="System.Threading.WaitHandle" /> in another application domain.
        /// </exception>
        public static Token WaitOneThenRelease( [NotNull] this Semaphore semaphore, TimeSpan? timeout = null ) {
            if ( semaphore is null ) {
                throw new ArgumentNullException(  nameof( semaphore ) );
            }

            if ( timeout.HasValue ) {
                semaphore.WaitOne( timeout: timeout.Value );
            }
            else {
                semaphore.WaitOne();
            }

            return new Token( semaphore );
        }

        public struct Token : IDisposable {

            [NotNull]
            private readonly Semaphore _semaphore;

            public Token( [NotNull] Semaphore semaphore ) => this._semaphore = semaphore ?? throw new ArgumentNullException( nameof( semaphore ) );

            public void Dispose() => this._semaphore.Release();
        }
    }
}