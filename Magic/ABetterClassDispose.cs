// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ABetterClassDispose.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ABetterClassDispose.cs" was last formatted by Protiguous on 2018/05/23 at 9:18 PM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;

    /// <summary>
    ///     <para>A better class for implementing the <see cref="IDisposable" /> pattern.</para>
    ///     <para><see cref="Dispose" /> can be called multiple times with no side effects.</para>
    ///     <para>Override <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.</para>
    /// </summary>
    /// <remarks>ABCD (hehe). Designed by Rick Harker</remarks>
    public class ABetterClassDispose : IDisposable {

        private Boolean Suppressed { get; set; }

        public Boolean HasDisposedManaged { get; private set; }

        public Boolean HasDisposedNative { get; private set; }

        public Boolean IsDisposed => this.HasDisposedManaged && this.HasDisposedNative;

        ~ABetterClassDispose() { this.Dispose(); }

        public void Dispose() {
            if ( this.Suppressed ) { return; }

            try {
                if ( !this.HasDisposedManaged ) {
                    try {
                        this.DisposeManaged();
                        this.HasDisposedManaged = true;
                    }
                    catch ( Exception ) {
                        if ( Debugger.IsAttached ) { Debugger.Break(); }
                    }
                }

                if ( !this.HasDisposedNative ) {
                    try {
                        this.DisposeNative();
                        this.HasDisposedNative = true;
                    }
                    catch ( Exception ) {
                        if ( Debugger.IsAttached ) { Debugger.Break(); }
                    }
                }
            }
            finally {
                if ( this.HasDisposedManaged && this.HasDisposedNative && !this.Suppressed ) {
                    GC.SuppressFinalize( this );
                    this.Suppressed = true;
                }
            }
        }

        /// <summary>
        ///     <para>Dispose any disposable managed fields or properties.</para>
        ///     <para>Call "base.DisposeManaged();" or "base.<see cref="Dispose" />;" when possible.</para>
        /// </summary>
        /// <remarks>Call sooner rathar than later for garbage collection.</remarks>
        public virtual void DisposeManaged() => this.HasDisposedManaged = true;

        /// <summary>
        ///     <para>Dispose of COM objects, Handles, etc. Then set those objects to null if possible.</para>
        ///     <para>Call "base.DisposeNative();" or "base.<see cref="Dispose" />;" when possible.</para>
        /// </summary>
        /// <remarks>Call sooner rathar than later for garbage collection.</remarks>
        public virtual void DisposeNative() => this.HasDisposedNative = true;
    }
}