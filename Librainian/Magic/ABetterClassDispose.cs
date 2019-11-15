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
// Project: "Librainian", "ABetterClassDispose.cs" was last formatted by Protiguous on 2019/11/14 at 10:24 AM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;
    using Logging;
    using Parsing;

    /// <summary>
    ///     <para>A non-sealed class for easier implementation the <see cref="IDisposable" /> pattern.</para>
    ///     <para>Implement overrides on <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.</para>
    /// </summary>
    /// <remarks>ABCD (hehe)</remarks>
    /// <copyright>Created by Rick Harker.</copyright>
    public class ABetterClassDispose : IDisposable {

        /// <summary>
        ///     <para>Disposes of managed resources, then unmanaged resources, and then calls <see cref="GC.SuppressFinalize" /> on this object.</para>
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose() => this.Dispose( true );

        private volatile Stages Stage = Stages.None;

        /// <summary>
        /// Call at any time to set a debugging hint as to the creator of this disposable.
        /// </summary>
        /// <param name="hint"></param>
        [Conditional("DEBUG")]
        private void SetDisposeHint( [CanBeNull] String hint ) {
            this.DisposeHint = hint;
        }

        private String DisposeHint { get; set; }

        //[DebuggerStepThrough]
        ~ABetterClassDispose() {
            if ( !String.IsNullOrWhiteSpace( this.DisposeHint ) && Debugger.IsAttached ) {
                Debug.WriteLine( $"{nameof( this.DisposeHint )}={this.DisposeHint.DoubleQuote()}" );
            }

            this.Dispose( false );
        }

        [Flags]
        private enum Stages : Byte {

            None = 0b0,

            DisposedManaged = 0b1,

            DisposedNative = 0b10,

            SuppressedFinalize = 0b100

        }

        /// <summary>
        ///     <para>If cleanupManaged, the method has been called by user's code. Managed and unmanaged resources can be disposed.</para>
        ///     <para>
        ///     If !cleanupManaged, the method has been called by the runtime from inside the finalizer and you should not reference other objects.. Only unmanaged resources can be
        ///     disposed.
        ///     </para>
        /// </summary>
        /// <param name="cleanupManaged"></param>
        /// TODO [DebuggerStepThrough]
        protected void Dispose( Boolean cleanupManaged ) {
            if ( cleanupManaged ) {

                //if ( Interlocked.Exchange( ref this._hasDisposedManaged, 1 ) == 0 ) {
                if ( !this.HasDisposedManaged() ) {
                    try {
                        this.Stage |= Stages.DisposedManaged;
                        this.DisposeManaged(); //allow once. Any derived class should have overloaded this method.
                    }
                    catch ( Exception exception ) {
                        exception.Log();
                    }
                }
            }

            //if ( Interlocked.Exchange( ref this._hasDisposedNative, 1 ) == 0 ) {
            if ( !this.HasDisposedNative() ) {
                try {
                    this.Stage |= Stages.DisposedNative;
                    this.DisposeNative(); //allow once. Any derived class should have overloaded this method.
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
            }

            //if ( Interlocked.Exchange( ref this._hasSuppressedFinalize, 1 ) == 0 ) {
            if ( !this.HasSuppressedFinalize() ) {
                this.Stage |= Stages.SuppressedFinalize;
                GC.SuppressFinalize( this ); //allow once
            }
        }

        /// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in the derived class in this overloaded method.</summary>
        [DebuggerStepThrough]
        public virtual void DisposeManaged() { }

        /// <summary>Dispose of COM objects, Handles, etc. (Do they now need set to null?) in this overloaded method.</summary>
        [DebuggerStepThrough]
        public virtual void DisposeNative() { }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasDisposedManaged() => this.Stage.HasFlag( Stages.DisposedManaged );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasDisposedNative() => this.Stage.HasFlag( Stages.DisposedNative );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean HasSuppressedFinalize() => this.Stage.HasFlag( Stages.SuppressedFinalize );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [DebuggerStepThrough]
        public Boolean IsDisposed() => this.HasDisposedManaged() && this.HasDisposedNative();

    }

}