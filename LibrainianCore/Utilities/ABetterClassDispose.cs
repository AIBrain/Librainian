// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ABetterClassDispose.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
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
// Project: "Librainian", "ABetterClassDispose.cs" was last formatted by Protiguous on 2019/12/22 at 4:59 AM.

namespace LibrainianCore.Utilities {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    ///     <para>A non-sealed class for easier implementation the <see cref="IDisposable" /> pattern.</para>
    ///     <para>Implement overrides on <see cref="DisposeManaged" />, and <see cref="DisposeNative" /> if needed.</para>
    ///     <code></code>
    /// </summary>
    /// <remarks>ABCD (hehe).
    /// <para>I wanted to make the <see cref="GC.SuppressFinalize" /> be called only once, but the rules were complaining..</para>
    /// <para>I also wanted to add in the optional dispose hint, so we could diagnose which object was being disposed..</para>
    /// </remarks>
    /// <copyright>Created by Protiguous.</copyright>
    public abstract class ABetterClassDispose : IDisposable {

        /// <summary>
        ///     <para>Disposes of managed resources, then unmanaged resources, and then calls <see cref="GC.SuppressFinalize" /> on this object.</para>
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose() {
            this.Dispose( true );
        }

        private Int32 _hasDisposedManaged;

        private Int32 _hasDisposedNative;

        private Int32 _hasSuppressedFinalize;

        protected ABetterClassDispose() {
            GC.SuppressFinalize( this );
            this._hasSuppressedFinalize = 1;
        }

        [DebuggerStepThrough]
        ~ABetterClassDispose() {

            /*
            if ( !String.IsNullOrEmpty( this.DisposeHint ) && Debugger.IsAttached ) {
                Debug.WriteLine( $"{nameof( this.DisposeHint )}=\"{this.DisposeHint}\"" );
                Debugger.Break();
            }
            */

            this.Dispose( false );
        }

        /// <summary>
        ///     <para>If cleanupManaged, the method has been called by user's code. Managed and unmanaged resources can be disposed.</para>
        ///     <para>
        ///     If !cleanupManaged, the method has been called by the runtime from inside the finalizer and you should not reference other objects.. Only unmanaged resources can be
        ///     disposed.
        ///     </para>
        /// </summary>
        /// <param name="cleanupManaged"></param>
        [DebuggerStepThrough]
        protected virtual void Dispose( Boolean cleanupManaged ) {
            if ( cleanupManaged ) {

                if ( Interlocked.Exchange( ref this._hasDisposedManaged, 1 ) == 0 /*allow once*/ ) {
                    try {
                        this.DisposeManaged(); //Any derived class should have overloaded this method and disposed of any managed objects inside.
                    }
                    catch ( Exception exception ) {
                        Debug.WriteLine( exception );
                    }
                }
            }

            if ( Interlocked.Exchange( ref this._hasDisposedNative, 1 ) == 0 /*allow once*/ ) {
                try {
                    this.DisposeNative(); //Any derived class should overload this method.
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( exception );
                }
            }

            if ( Interlocked.Exchange( ref this._hasSuppressedFinalize, 1 ) == 0 /*allow once*/ ) {
                GC.SuppressFinalize( this );
            }
        }

        /*

        /// <summary>Set via <see cref="SetDisposeHint" /> to help find if an object has not been disposed of properly.</summary>
        [CanBeNull]
        private String DisposeHint { get; set; }
        */

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
        /// <remarks>This is the method that you will Dispose of any disposable managed objects. (ie <code>using ( var bob = )</code>)</remarks>
        [DebuggerStepThrough]
        public abstract void DisposeManaged();

        /// <summary>Dispose of COM objects, Handles, etc in this method.</summary>
        [DebuggerStepThrough]
        public virtual void DisposeNative() {
            /*make this virtual so it is optional*/
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasDisposedManaged() => Interlocked.CompareExchange( ref this._hasDisposedManaged, 0, 0 ) == 1;

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasDisposedNative() => Interlocked.CompareExchange( ref this._hasDisposedNative, 0, 0 ) == 1;

        /*
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasSuppressedFinalize() => Interlocked.CompareExchange( ref this._hasSuppressedFinalize, 0, 0 ) == 1;
        */

        /// <summary>Can easily be changed to a property, if desired.</summary>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean IsDisposed() => this.HasDisposedManaged() && this.HasDisposedNative();

        /*

        /// <summary>Call at any time to set a debugging hint as to the creator of this disposable.</summary>
        /// <param name="hint"></param>
        [Conditional( "DEBUG" )]
        public void SetDisposeHint( [CanBeNull] String hint ) => this.DisposeHint = hint;
        */

    }

}