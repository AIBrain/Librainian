// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "ReferenceCS.cs" last formatted on 2021-11-30 at 7:23 PM by Protiguous.

#nullable enable

namespace Librainian.Threadsafe;

using System;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>A reference that may be updated atomically</summary>
public struct Reference<T> where T : class {

	private T? _value;

	/// <summary>Create a new <see cref="Reference{T}" /> with the given initial value.</summary>
	/// <param name="value">Initial value</param>
	public Reference( T? value ) => this._value = value;

	/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
	/// <param name="newValue">The new value</param>
	/// <param name="comparand">The comparand (expected value)</param>
	public Boolean AtomicCompareExchange( T? newValue, T? comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

	/// <summary>Atomically set the value to the given updated value</summary>
	/// <param name="newValue">The new value</param>
	/// <returns>The original value</returns>
	public T? AtomicExchange( T? newValue ) => Interlocked.Exchange( ref this._value, newValue );

	/// <summary>Read the value applying acquire fence semantic</summary>
	/// <returns>The current value</returns>
	public T? ReadAcquireFence() {
		var value = this._value;
		Thread.MemoryBarrier();

		return value;
	}

	/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
	/// <returns>The current value</returns>
	[MethodImpl( MethodImplOptions.NoOptimization )]
	public T? ReadCompilerOnlyFence() => this._value;

	/// <summary>Read the value applying full fence semantic</summary>
	/// <returns>The current value</returns>
	public T? ReadFullFence() {
		var value = this._value;
		Thread.MemoryBarrier();

		return value;
	}

	/// <summary>Read the value without applying any fence</summary>
	/// <returns>The current value</returns>
	public T? ReadUnfenced() => this._value;

	/// <summary>Returns the String representation of the current value.</summary>
	/// <returns>the String representation of the current value.</returns>
	public override String? ToString() {
		var value = this.ReadFullFence();

		return value?.ToString();
	}

	/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
	/// <param name="newValue">The new value</param>
	[MethodImpl( MethodImplOptions.NoOptimization )]
	public void WriteCompilerOnlyFence( T? newValue ) => this._value = newValue;

	/// <summary>Write the value applying full fence semantic</summary>
	/// <param name="newValue">The new value</param>
	public void WriteFullFence( T? newValue ) {
		Thread.MemoryBarrier();
		this._value = newValue;
	}

	/// <summary>Write the value applying release fence semantic</summary>
	/// <param name="newValue">The new value</param>
	public void WriteReleaseFence( T? newValue ) {
		Thread.MemoryBarrier();
		this._value = newValue;
	}

	/// <summary>Write without applying any fence</summary>
	/// <param name="newValue">The new value</param>
	public void WriteUnfenced( T? newValue ) => this._value = newValue;
}