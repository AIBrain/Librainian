// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ABetterClassDispose.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "ABetterClassDispose.cs" was last formatted by Protiguous on 2020/03/16 at 3:13 PM.

namespace Librainian.Utilities {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    ///     <para>A class for easier implementation the proper <see cref="IDisposable" /> pattern.</para>
    ///     <para>Implement overrides on <see cref="DisposeManaged" />, and <see cref="DisposeNative" /> as needed.</para>
    ///     <code></code>
    /// </summary>
    /// <remarks>ABCD (hehe).</remarks>
    /// <copyright>Created by Protiguous.</copyright>
    public abstract class ABetterClassDispose : IDisposable {

        /// <summary>
        ///     <para>Disposes of managed resources, then unmanaged resources, and then calls <see cref="GC.SuppressFinalize" /> on this object.</para>
        /// </summary>
        [DebuggerStepThrough]
        public void Dispose() => this.Dispose( cleanupManaged: true );

        private Int32 _hasDisposedManaged;

        private Int32 _hasDisposedNative;

        private Int32 _hasSuppressedFinalize;

        public Boolean HasDisposedManaged {
            [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
            [DebuggerStepThrough]
            get => Interlocked.CompareExchange( location1: ref this._hasDisposedManaged, value: 0, comparand: 0 ) == 1;

            [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
            [DebuggerStepThrough]
            set {
                if ( this.HasDisposedManaged ) {
                    return; //don't allow the setting to be changed once it has been set.
                }

                Interlocked.Exchange( location1: ref this._hasDisposedManaged, value: value ? 1 : 0 );
            }
        }

        public Boolean HasDisposedNative {
            [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
            [DebuggerStepThrough]
            get => Interlocked.CompareExchange( location1: ref this._hasDisposedNative, value: 0, comparand: 0 ) == 1;

            [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
            [DebuggerStepThrough]
            set {
                if ( this.HasDisposedNative ) {
                    return; //don't allow the setting to be changed once it has been set.
                }

                Interlocked.Exchange( location1: ref this._hasDisposedNative, value: value ? 1 : 0 );
            }
        }

        public Boolean HasSuppressedFinalize {
            [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
            [DebuggerStepThrough]
            get => Interlocked.CompareExchange( location1: ref this._hasSuppressedFinalize, value: 0, comparand: 0 ) == 1;

            [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
            [DebuggerStepThrough]
            set {
                if ( this.HasSuppressedFinalize ) {
                    return; //don't allow the setting to be changed once it has been set.
                }

                Interlocked.Exchange( location1: ref this._hasSuppressedFinalize, value: value ? 1 : 0 );
            }
        }

        /// <summary>Can easily be changed to a property, if desired.</summary>
        /// <returns></returns>
        public Boolean IsDisposed => this.HasDisposedManaged && this.HasDisposedNative;

        /// <summary>
        ///     <para>If cleanupManaged, the method has been called by user's code. Managed and unmanaged resources can be disposed.</para>
        ///     <para>
        ///     If !cleanupManaged, the method has been called by the runtime from inside the finalizer and you should not reference other objects.. Only unmanaged resources can be
        ///     disposed.
        ///     </para>
        /// </summary>
        /// <param name="cleanupManaged"></param>
        [DebuggerStepThrough]
        protected void Dispose( Boolean cleanupManaged ) {
            if ( cleanupManaged && !this.HasDisposedManaged ) {
                try {
                    this.DisposeManaged(); //Any derived class should have overloaded this method and disposed of any managed objects inside.
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( value: exception );
                }
                finally {
                    this.HasDisposedManaged = true;
                }
            }

            if ( !this.HasDisposedNative ) {
                try {
                    this.DisposeNative(); //Any derived class should overload this method.
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( value: exception );
                }
                finally {
                    this.HasDisposedNative = true;
                }
            }

            if ( this.IsDisposed && !this.HasSuppressedFinalize ) {
                try {
                    GC.SuppressFinalize( obj: this );
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( value: exception );
                }
                finally {
                    this.HasSuppressedFinalize = true;
                }
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
            this.HasDisposedNative = true;
        }

        /*

        /// <summary>Call at any time to set a debugging hint as to the creator of this disposable.</summary>
        /// <param name="hint"></param>
        [Conditional( "DEBUG" )]
        public void SetDisposeHint( [CanBeNull] String hint ) => this.DisposeHint = hint;
        */

    }

}