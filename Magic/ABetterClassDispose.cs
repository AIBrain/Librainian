// Copyright 2016 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/BetterDisposableClass.cs" was last cleaned by Rick on 2016/08/06 at 7:06 AM

namespace Librainian.Magic {

    using System;

    /// <summary>
    /// <para>A better class for implementing <see cref="IDisposable"/>.</para>
    /// Override <see cref="DisposeNative"/> as needed.
    /// </summary>
    /// <remarks>ABCD hehe</remarks>
    public abstract class ABetterClassDispose : IDisposable {

        protected virtual void Dispose( Boolean disposeManaged ) {
            if ( disposeManaged ) {
                try {
                    this.DisposeManaged();
                }
                catch ( Exception exception ) {
                    exception.More();
                }
            }
            try {
                this.DisposeNative();
            }
            catch ( Exception exception ) {
                exception.More();
            }
        }

        public void Dispose() {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected abstract void DisposeManaged();

        /// <summary>
        /// Dispose of COM objects, Handles, etc...
        /// </summary>
        protected virtual void DisposeNative() {
        }

        ~ABetterClassDispose() { this.Dispose( false ); }

    }

    ///// <summary>Hopefully, a more useful <see cref="IDisposable" /> pattern.</summary>
    //[ JsonObject ]
    //public abstract class BetterDisposableClass : IDisposable {

    //    private Boolean IsDisposedOfManaged { get; set; }

    //    private Boolean IsDisposedOfNative { get; set; }

    //    public void Dispose() {
    //        try {
    //            if ( this.IsDisposedOfManaged ) {
    //                return;
    //            }

    //            this.CleanUpManagedResources();
    //            this.IsDisposedOfManaged = true;
    //        }
    //        catch ( Exception exception ) {
    //            exception.More();
    //        }
    //        finally {
    //            try {
    //                if ( !IsDisposedOfNative ) {
    //                    this.CleanUpNativeResources();
    //                    this.IsDisposedOfNative = true;
    //                    GC.SuppressFinalize( this );
    //                }
    //            }
    //            catch ( Exception exception ) {
    //                exception.More();
    //            }
    //        }
    //    }

    //    ~BetterDisposableClass() { this.Dispose(); }

    //    protected virtual void CleanUpManagedResources() { }

    //    protected virtual void CleanUpNativeResources() { }

    //}

}
