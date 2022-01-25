// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "SingleAccess.cs" last formatted on 2022-12-22 at 5:21 PM by Protiguous.

#nullable enable

namespace Librainian.Threading;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using FileSystem;
using Logging;
using Measurement.Time;
using Persistence;
using Utilities.Disposables;

/// <summary>
///     Uses a named <see cref="Semaphore" /> to allow only 1 access to "name".
///     <para></para>
/// </summary>
/// <example>
///     <code>using ( new SingleAccess( anyName ) ) { DoCode(); }</code>
/// </example>
public class SingleAccess : ABetterClassDispose {

	private SingleAccess() {
		/* Disallow private contructor */
	}

	/// <summary>Uses a named semaphore to allow only ONE of <paramref name="id" />.</summary>
	/// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
	public SingleAccess( Guid id, TimeSpan? timeout = null ) : this( id.ToString( "D" ), timeout ) { }

	/// <summary>Uses a named semaphore to allow only ONE of <paramref name="name" />.</summary>
	/// <example>using ( var snag = new FileSingleton( info ) ) { DoCode(); }</example>
	public SingleAccess( FileSystemInfo name, TimeSpan? timeout = null ) : this( name.FullName, timeout ) { }

	/// <summary>Uses a named semaphore to allow only ONE of <paramref name="name" />.</summary>
	/// <example>using ( var snag = new FileSingleton( name ) ) { DoCode(); }</example>
	public SingleAccess( String name, TimeSpan? timeout = null ) {
		if ( String.IsNullOrWhiteSpace( name ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( name ) );
		}

		try {
			timeout ??= Minutes.One;

			this.Snagged = false;
			this.Semaphore = new Semaphore( 1, 1, name );
			this.Snagged = this.Semaphore.WaitOne( timeout.Value );
		}
		catch ( Exception exception ) {
			exception.Log();
		}
	}

	public SingleAccess( IDocument document, TimeSpan? timeout = null ) : this( document.FullPath, timeout ) {
	}

	private Semaphore? Semaphore { get; }

	public Boolean Snagged { get; private set; }

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		if ( !this.Snagged ) {
			return;
		}

		using ( this.Semaphore ) {
			try {
				this.Semaphore?.Release();
			}
			finally {
				this.Snagged = false;
			}
		}
	}
}

/// <summary>
///     Uses a named <see cref="Semaphore" /> to allow only 1 access to "self".
///     <para></para>
/// </summary>
/// <example>
///     <code>using ( new SingleAccess( anyName ) ) { DoCode(); }</code>
/// </example>
public class SingleAccess<T> : ABetterClassDispose {

	private SingleAccess() {
		/* Disallow private contructor */
	}

	/// <summary>Uses a named semaphore to allow only ONE of <paramref name="self" />.</summary>
	/// <example>using ( var snag = new FileSingleton( guid ) ) { DoCode(); }</example>
	public SingleAccess( [DisallowNull] T self, TimeSpan? timeout = null ) {
		try {
			timeout ??= Minutes.One;

			this.Snagged = false;
			this.Semaphore = new Semaphore( 1, 1, self.ToJSON() ); //what happens on a null?
			this.Snagged = this.Semaphore.WaitOne( timeout.Value );
		}
		catch ( Exception exception ) {
			exception.Log();
		}
	}

	private Semaphore? Semaphore { get; }

	public Boolean Snagged { get; private set; }

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		if ( !this.Snagged ) {
			return;
		}

		using ( this.Semaphore ) {
			try {
				this.Semaphore?.Release();
			}
			finally {
				this.Snagged = false;
			}
		}
	}
}