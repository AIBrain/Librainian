// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ABetterClassDispose.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ABetterClassDispose.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Magic {

    using System;
    using FluentAssertions;

    /// <summary>
    ///     <para>A better class for implementing the <see cref="IDisposable" /> pattern.</para>
    ///     <para>Implement <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.</para>
    /// </summary>
    /// <remarks>ABCD (hehe). Designed by Rick Harker</remarks>
    public class ABetterClassDispose : IDisposable {

        ~ABetterClassDispose() => this.Dispose();

        public Boolean HasDisposedManaged { get; private set; }

        public Boolean HasDisposedNative { get; private set; }

        public Boolean IsDisposed => this.HasDisposedManaged && this.HasDisposedNative;

        public void Dispose() {
            try {
                if ( !this.HasDisposedManaged ) {
                    try { this.DisposeManaged(); }
                    catch ( Exception exception ) { exception.Break(); }
                    finally { this.HasDisposedManaged = true; }
                }

                if ( !this.HasDisposedNative ) {
                    try { this.DisposeNative(); }
                    catch ( Exception exception ) { exception.Break(); }
                    finally { this.HasDisposedNative = true; }
                }
            }
            finally {
                this.HasDisposedManaged.Should().BeTrue();
                this.HasDisposedNative.Should().BeTrue();
                GC.SuppressFinalize( this );
            }
        }

        /// <summary>
        ///     <para>Dispose any disposable managed fields or properties.</para>
        ///     <para>Call "base.DisposeManaged();" when possible.</para>
        /// </summary>
        public virtual void DisposeManaged() {
            this.HasDisposedManaged = true; //yay or nay?
        }

        /// <summary>
        ///     <para>Dispose of COM objects, Handles, etc. Then set those objects to null if possible.</para>
        ///     <para>Call "base.DisposeNative();" when possible.</para>
        /// </summary>
        public virtual void DisposeNative() {
            this.HasDisposedNative = true; //yay or nay?
        }
    }
}