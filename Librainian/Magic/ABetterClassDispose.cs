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
// Project: "Librainian", "ABetterClassDispose.cs" was last formatted by Protiguous on 2018/07/13 at 1:15 AM.

namespace Librainian.Magic {

	using System;
	using System.Diagnostics;

	/// <summary>
	///     <para>A better class for implementing the <see cref="IDisposable" /> pattern.</para>
	///     <para><see cref="Dispose()" /> can be called multiple times with no side effects.</para>
	///     <para>Override <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.</para>
	/// </summary>
	/// <remarks>ABCD (hehe). Designed by Rick Harker</remarks>
	public class ABetterClassDispose : IDisposable {

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public void Dispose() => this.Dispose( true );

		private Boolean Suppressed { get; set; }

		public Boolean HasDisposedManaged { get; private set; }

		public Boolean HasDisposedNative { get; private set; }

		public Boolean IsDisposed => this.HasDisposedManaged && this.HasDisposedNative;

		/// <summary>
		///     Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by
		///     garbage collection.
		/// </summary>
		~ABetterClassDispose() { this.Dispose( true ); }

		private void ReleaseUnmanagedResources() {

			// TODO release unmanaged resources here
		}

		public void Dispose( Boolean _ ) {

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
		///     <para>Call "base.DisposeManaged();" or "base.<see cref="Dispose(Boolean)" />;" when possible.</para>
		/// </summary>
		/// <remarks>Call sooner rathar than later for garbage collection.</remarks>
		public virtual void DisposeManaged() => this.HasDisposedManaged = true;

		/// <summary>
		///     <para>Dispose of COM objects, Handles, etc. Then set those objects to null if possible.</para>
		///     <para>Call "base.DisposeNative();" or "base.<see cref="Dispose(Boolean)" />;" when possible.</para>
		/// </summary>
		/// <remarks>Call sooner rathar than later for garbage collection.</remarks>
		public virtual void DisposeNative() => this.HasDisposedNative = true;
	}
}