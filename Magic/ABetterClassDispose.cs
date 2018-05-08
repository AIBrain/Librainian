// Copyright 2017 Protiguous.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/ABetterClassDispose.cs" was last cleaned by Protiguous on 2017/12/24 at 8:30 AM

namespace Librainian.Magic {
	using System;
	using System.Diagnostics;

	/// <summary>
	///     <para>A better class for implementing the <see cref="IDisposable" /> pattern.</para>
	///     <para>Implement <see cref="DisposeManaged" /> and <see cref="DisposeNative" />.</para>
	/// </summary>
	/// <remarks>ABCD hehe. Written by Rick Harker</remarks>
	public class ABetterClassDispose : IDisposable {

		public Boolean HasDisposedManaged { get; private set; }
		public Boolean HasDisposedNative { get; private set; }

		public void Dispose() {
			this.Dispose( disposing: true );
			GC.SuppressFinalize( this );
		}

	    private void Dispose( Boolean disposing ) {
			if ( !disposing ) {
				return;
			}

			if ( !this.HasDisposedManaged ) {
				try {
					this.DisposeManaged();
				}

				catch ( Exception exception) {
				    exception.Break();
				}
				finally {
					this.HasDisposedManaged = true;
				}
			}

			if ( !this.HasDisposedNative ) {
				try {
					this.DisposeNative();
				}

				catch ( Exception exception ) {
				    exception.Break();
				}

                finally {
					this.HasDisposedNative = true;
				}
			}
		}

		/// <summary>
		///     Dispose any disposable managed fields or properties.
		/// </summary>
		public virtual void DisposeManaged() { }

		/// <summary>
		///     Dispose of COM objects, Handles, etc. Then set those objects to null.
		/// </summary>
		public virtual void DisposeNative() { }

		~ABetterClassDispose() => this.Dispose( disposing: false );
	}
}