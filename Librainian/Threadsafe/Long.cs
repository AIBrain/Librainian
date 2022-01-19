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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "Long.cs" last formatted on 2022-12-22 at 5:21 PM by Protiguous.

#nullable enable

namespace Librainian.Threadsafe;

using System;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>A long value that may be updated atomically</summary>
public struct Long {

	private Int64 _value;

	/// <summary>Create a new <see cref="Long" /> with the given initial value.</summary>
	/// <param name="value">Initial value</param>
	public Long( Int64 value ) => this._value = value;

	/// <summary>Atomically add the given value to the current value and return the sum</summary>
	/// <param name="delta">The value to be added</param>
	/// <returns>The sum of the current value and the given value</returns>
	public Int64 AtomicAddAndGet( Int64 delta ) => Interlocked.Add( ref this._value, delta );

	/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
	/// <param name="newValue"> The new value</param>
	/// <param name="comparand">The comparand (expected value)</param>
	public Boolean AtomicCompareExchange( Int64 newValue, Int64 comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

	/// <summary>Atomically increment the current value and return the new value</summary>
	/// <returns>The decremented value.</returns>
	public Int64 AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

	/// <summary>Atomically set the value to the given updated value</summary>
	/// <param name="newValue">The new value</param>
	/// <returns>The original value</returns>
	public Int64 AtomicExchange( Int64 newValue ) => Interlocked.Exchange( ref this._value, newValue );

	/// <summary>Atomically increment the current value and return the new value</summary>
	/// <returns>The incremented value.</returns>
	public Int64 AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

	/// <summary>Read the value applying acquire fence semantic</summary>
	/// <returns>The current value</returns>
	public Int64 ReadAcquireFence() {
		var value = this._value;
		Thread.MemoryBarrier();

		return value;
	}

	/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
	/// <returns>The current value</returns>
	[MethodImpl( MethodImplOptions.NoOptimization )]
	public Int64 ReadCompilerOnlyFence() => this._value;

	/// <summary>Read the value applying full fence semantic</summary>
	/// <returns>The current value</returns>
	public Int64 ReadFullFence() {
		Thread.MemoryBarrier();

		return this._value;
	}

	/// <summary>Read the value without applying any fence</summary>
	/// <returns>The current value</returns>
	public Int64 ReadUnfenced() => this._value;

	/// <summary>Returns the String representation of the current value.</summary>
	/// <returns>the String representation of the current value.</returns>
	public override String ToString() {
		var value = this.ReadFullFence();

		return value.ToString();
	}

	/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
	/// <param name="newValue">The new value</param>
	[MethodImpl( MethodImplOptions.NoOptimization )]
	public void WriteCompilerOnlyFence( Int64 newValue ) => this._value = newValue;

	/// <summary>Write the value applying full fence semantic</summary>
	/// <param name="newValue">The new value</param>
	public void WriteFullFence( Int64 newValue ) {
		Thread.MemoryBarrier();
		this._value = newValue;
	}

	/// <summary>Write the value applying release fence semantic</summary>
	/// <param name="newValue">The new value</param>
	public void WriteReleaseFence( Int64 newValue ) {
		Thread.MemoryBarrier();
		this._value = newValue;
	}

	/// <summary>Write without applying any fence</summary>
	/// <param name="newValue">The new value</param>
	public void WriteUnfenced( Int64 newValue ) => this._value = newValue;

}