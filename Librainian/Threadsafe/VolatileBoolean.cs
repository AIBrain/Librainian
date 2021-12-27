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
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "$FILENAME$" last touched on $CURRENT_YEAR$-$CURRENT_MONTH$-$CURRENT_DAY$ at $CURRENT_TIME$ by Protiguous.

#nullable enable

namespace Librainian.Threadsafe;

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
///     <para>A threadsafe boolean.</para>
/// </summary>
/// <copyright>Protiguous</copyright>
public record VolatileBoolean( Boolean _value = false ) {

	private volatile Boolean _value = _value;

	public Boolean Value {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		get => this.ReadFence();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		set => this.WriteFence( value );
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	private Boolean ReadFence() {
		try {
			return this._value;
		}
		finally {
			Thread.MemoryBarrier();
		}
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	private void WriteFence( Boolean value ) {
		Thread.MemoryBarrier();
		this._value = value;
	}

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static VolatileBoolean Create( Boolean value ) => new( value );

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static implicit operator Boolean( VolatileBoolean value ) => value.ReadFence();

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public static implicit operator VolatileBoolean( Boolean value ) => Create( value );

	public void Deconstruct( out Boolean value ) => value = this._value;

	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	[DebuggerStepThrough]
	public override String ToString() => this._value ? "true" : "false";
}