// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Threading {

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using JetBrains.Annotations;
    using Maths;
    using Utilities;

    /// <summary>
    ///     <para>A threadsafe way to mark anything as cancelled.</para>
    /// </summary>
    /// <remarks>Not superior to <see cref="CancellationTokenSource" />, just different. And a class.</remarks>
    [Experimental( "Somewhat untested. Should work though." )]
    [Obsolete( "Just use CancellationTokenSource..." )]
    public sealed class SimpleCancel : ABetterClassDispose {

        /// <summary></summary>
        private Int64 _cancelRequests;

        public Boolean IsCancellationRequested => this.HaveAnyCancellationsBeenRequested();

        /// <summary></summary>
        public SimpleCancel() => this.Reset();

        /// <summary>Returns true if the cancel request was approved.</summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage">           </param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean Cancel( Boolean throwIfAlreadyRequested = false, [CanBeNull] String throwMessage = "" ) =>
            this.RequestCancel( throwIfAlreadyRequested, throwMessage );

        public override void DisposeManaged() => this.RequestCancel( false );

        /// <summary></summary>
        /// <returns></returns>
        public Int64 GetCancelsRequestedCounter() => Interlocked.Read( ref this._cancelRequests );

        /// <summary></summary>
        public Boolean HaveAnyCancellationsBeenRequested() => this.GetCancelsRequestedCounter().Any();

        /// <summary>Returns true if the cancel request was approved.</summary>
        /// <param name="throwIfAlreadyRequested"></param>
        /// <param name="throwMessage">           </param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">Thrown if a cancellation has already been requested.</exception>
        public Boolean RequestCancel( Boolean throwIfAlreadyRequested = false, [CanBeNull] String throwMessage = "" ) {
            if ( throwIfAlreadyRequested && this.HaveAnyCancellationsBeenRequested() ) {
                throw new TaskCanceledException( throwMessage );
            }

            Interlocked.Increment( ref this._cancelRequests );

            return true;
        }

        /// <summary>Resets all requests back to starting values.</summary>
        public void Reset() => Interlocked.Add( ref this._cancelRequests, -Interlocked.Read( ref this._cancelRequests ) );

    }

}