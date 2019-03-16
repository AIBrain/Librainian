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
// Project: "Librainian", "ABetterClassDispose.cs" was last formatted by Protiguous on 2019/01/14 at 2:22 AM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;
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

        private volatile Boolean _hasDisposedManaged;

        private volatile Boolean _hasDisposedNative;

        private volatile Boolean _hasSuppressedFinalize;

        public Boolean HasDisposedManaged {
            get => this._hasDisposedManaged;
            private set => this._hasDisposedManaged = value;
        }

        public Boolean HasDisposedNative {
            get => this._hasDisposedNative;
            private set => this._hasDisposedNative = value;
        }

        public Boolean HasSuppressedFinalize {
            get => this._hasSuppressedFinalize;
            private set => this._hasSuppressedFinalize = value;
        }

        /// <summary>
        ///     Return true if <see cref="HasDisposedManaged" /> and <see cref="HasDisposedNative" /> are both true.
        /// </summary>
        public Boolean IsDisposed => this.HasDisposedManaged && this.HasDisposedNative;

        [DebuggerStepThrough]
        public void Dispose() => this.Dispose( true );

        [DebuggerStepThrough]
        ~ABetterClassDispose() => this.Dispose( true );

        private void PreventFinalizer() {
            if ( this.IsDisposed && !this.HasSuppressedFinalize ) {
                GC.SuppressFinalize( this );
                this.HasSuppressedFinalize = true;
            }
        }

        protected virtual void Dispose( Boolean _ ) {
            if ( !this.HasDisposedManaged ) {
                try {
                    this.DisposeManaged();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
                finally {
                    if ( !this.HasDisposedManaged ) {
                        $"Error: derived class did not call \"base.{nameof( this.DisposeManaged )}\".".Break();
                    }
                }
            }

            if ( !this.HasDisposedNative ) {
                try {
                    this.DisposeNative();
                }
                catch ( Exception exception ) {
                    exception.Log();
                }
                finally {
                    if ( !this.HasDisposedNative ) {
                        $"Error: derived class did not call \"base.{nameof( this.DisposeNative )}\".".Break();
                    }
                }

            }
        }

        /// <summary>
        ///     Dispose any disposable managed fields or properties.
        ///     <para>
        ///         Providing the object inside a using construct will then call <see cref="Dispose" />, which in turn calls
        ///         <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.
        ///     </para>
        ///     <para>
        ///         <example>
        ///             Example usage:
        ///             <code>
        /// using ( this.mySink ) { this.mySink=null; }
        /// </code>
        ///         </example>
        ///     </para>
        /// </summary>
        public virtual void DisposeManaged() {
            this.HasDisposedManaged = true;
            this.PreventFinalizer();
        }

        /// <summary>
        ///     Dispose of COM objects, Handles, etc. Then set those objects to null.
        ///     <para>
        ///         Providing the object inside a using construct will then call <see cref="Dispose" />, which in turn calls
        ///         <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.
        ///     </para>
        ///     <para>
        ///         <example>
        ///             Example usage:
        ///             <code>
        /// using ( this.mySink ) { this.mySink=null; }
        /// </code>
        ///         </example>
        ///     </para>
        /// </summary>
        public virtual void DisposeNative() {
            this.HasDisposedNative = true;
            this.PreventFinalizer();
        }

    }

}