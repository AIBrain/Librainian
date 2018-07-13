// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FileSingleton.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "FileSingleton.cs" was last formatted by Protiguous on 2018/07/13 at 1:40 AM.

namespace Librainian.Threading {

	using System;
	using System.IO;
	using System.Threading;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Magic;
	using Measurement.Time;
	using Security;

	/// <summary>
	///     Uses a named semaphore to allow only ONE of name.
	/// </summary>
	/// <example>using ( new FileSingleton( anyName ) ) { DoCode(); }</example>
	public class FileSingleton : ABetterClassDispose {

		[CanBeNull]
		private Semaphore Semaphore { get; }

		public Boolean Snagged { get; private set; }

		private FileSingleton() {
			/* Disallow private contructor */
		}

		/// <summary>
		///     Uses a named semaphore to allow only ONE of <paramref name="id" />.
		/// </summary>
		/// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
		public FileSingleton( Guid id ) {
			try {
				id.Should().NotBeEmpty();
				this.Snagged = false;
				this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: id.ToString( "D" ) );
				this.Snagged = this.Semaphore.WaitOne( Minutes.One );
			}
			catch ( Exception exception ) { exception.More(); }
		}

		/// <summary>
		///     Uses a named semaphore to allow only ONE of name.
		/// </summary>
		/// <example>using ( var snag = new FileSingleton( info ) ) { DoCode(); }</example>
		public FileSingleton( [NotNull] FileSystemInfo name ) {
			name.Should().NotBeNull();

			if ( name == null ) { throw new ArgumentNullException( paramName: nameof( name ) ); }

			try {
				this.Snagged = false;
				var encoded = name.FullName.Sha512().GetHexString();
				this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: encoded );
				this.Snagged = this.Semaphore.WaitOne( Minutes.One );
			}
			catch ( Exception exception ) { exception.More(); }
		}

		/// <summary>
		///     Uses a named semaphore to allow only ONE of name.
		/// </summary>
		/// <example>using ( var snag = new FileSingleton( name ) ) { DoCode(); }</example>
		public FileSingleton( String name ) {
			name.Should().NotBeNull();

			try {
				this.Snagged = false;
				var encoded = name.Sha512().GetHexString();
				this.Semaphore = new Semaphore( initialCount: 1, maximumCount: 1, name: encoded );
				this.Snagged = this.Semaphore.WaitOne( Minutes.One );
			}
			catch ( Exception exception ) { exception.More(); }
		}

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() {
			if ( !this.Snagged ) { return; }

			var semaphore = this.Semaphore;

			if ( null == semaphore ) { return; }

			using ( semaphore ) {
				semaphore.Release();
				this.Snagged = false;
			}
		}
	}
}