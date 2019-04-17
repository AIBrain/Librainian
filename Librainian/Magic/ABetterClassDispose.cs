// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ABetterClassDispose.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "ABetterClassDispose.cs" was last formatted by Protiguous on 2019/04/10 at 3:43 PM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Logging;

    /// <summary>
    ///     <para>A better class for implementing the <see cref="IDisposable" /> pattern.</para>
    ///     <para>Implement overrides on <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.</para>
    ///     <para>Have each call base.Dispose(), or base.DisposeManaged() and base.DisposeNative() respectively.</para>
    /// </summary>
    /// <remarks>ABCD (hehe). Written by Rick Harker.</remarks>
    /// <copyright>
    ///     Created by Rick Harker.
    /// </copyright>
    public class ABetterClassDispose : IDisposable {

        [DebuggerStepThrough]
        public void Dispose() => this.Dispose( true );

        private Int32 _hasDisposedManaged;

        private Int32 _hasDisposedNative;

        private Int32 _hasSuppressedFinalize;

        [DebuggerStepThrough]
        ~ABetterClassDispose() => this.Dispose( true );

        protected void Dispose( Boolean _ ) {
            try {

                //allow once
                if ( Interlocked.Exchange( ref this._hasDisposedManaged, 1 ) == 0 ) {
                    try {
                        this.DisposeManaged();
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                }

                //allow once
                if ( Interlocked.Exchange( ref this._hasDisposedNative, 1 ) == 0 ) {
                    try {
                        this.DisposeNative();
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                }
            }
            finally {

                //allow once
                if ( Interlocked.Exchange( ref this._hasSuppressedFinalize, 1 ) == 0 ) {
                    GC.SuppressFinalize( this );
                }
            }
        }

        /// <summary>
        ///     Dispose any disposable managed fields or properties.
        ///     <para>
        ///         Providing the object inside a using construct will then call <see cref="Dispose" />, which in turn calls
        ///         <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.
        ///     </para>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public virtual void DisposeManaged() { }

        /// <summary>
        ///     Dispose of COM objects, Handles, etc. (do we need to set those objects to null?)
        ///     <para>
        ///         Providing the object inside a using construct will then call <see cref="Dispose" />, which in turn calls
        ///         <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.
        ///     </para>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public virtual void DisposeNative() { }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasDisposedManaged() => Interlocked.CompareExchange( ref this._hasDisposedManaged, 0, 0 ) != 0;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasDisposedNative() => Interlocked.CompareExchange( ref this._hasDisposedNative, 0, 0 ) != 0;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasSuppressedFinalize() => Interlocked.CompareExchange( ref this._hasSuppressedFinalize, 0, 0 ) != 0;

        /// <summary>
        ///     Return true if <see cref="HasDisposedManaged" /> and <see cref="HasDisposedNative" /> are both true.
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean IsDisposed() => this.HasDisposedManaged() && this.HasDisposedNative();

    }

}