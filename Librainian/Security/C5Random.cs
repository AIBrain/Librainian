// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// File "C5Random.cs" last formatted on 2020-08-14 at 8:44 PM.

#nullable enable

namespace Librainian.Security;

using System;
using Exceptions;

/*
    Copyright (c) 2003-2006 Niels Kokholm and Peter Sestoft
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

/// <summary>
///     <para>
///         A modern random number generator based on G. Marsaglia: Seeds for Random Number Generators, Communications of
///         the ACM 46, 5 (May 2003) 90-93; and a posting by Marsaglia
///         to comp.lang.c on 2003-04-03.
///     </para>
/// </summary>
public class C5Random : Random {

	private const Byte _qLength = 16;

	private UInt32 _c = 362436;

	private UInt32 _i = 15;

	private UInt32[] _q { get; } = new UInt32[_qLength];

	/// <summary>
	///     <para>Create a random number generator with a given seed.</para>
	///     <para>If 0 or no seed is provided, the current date &amp; time is used.</para>
	/// </summary>
	/// <param name="seed"></param>
	public C5Random( Int64 seed = 0 ) {
		if ( seed == 0 ) {
			seed = DateTime.UtcNow.Ticks;
		}

		var j = ( UInt32 )( seed & 0xFFFFFFFF );

		for ( var i = 0; i < this._q.Length; i++ ) {
			j ^= j << 0xD;
			j ^= j >> 0x11;
			j ^= j << 0x5;
			this._q[i] = j;
		}

		this._q[^1] = ( UInt32 )( seed ^ ( seed >> 32 ) );
	}

	/// <summary>
	///     <para>Create a random number generator with a specified internal start state.</para>
	///     <para>Uses the first
	///         <value>16</value>
	///         <see cref="UInt32" /> of the <paramref name="array" />.
	///     </para>
	/// </summary>
	/// <exception cref="ArgumentException">If <paramref name="array" /> is not at least length exactly 16.</exception>
	/// <param name="array">
	///     The start state. Must be a collection of random bits given by an array of exactly 16
	///     <see cref="UInt32" />.
	/// </param>
	public C5Random( UInt32[] array ) {
		if ( array is null ) {
			throw new ArgumentEmptyException( nameof( array ) );
		}

		if ( array.Length > 0 ) {
			Buffer.BlockCopy( array, 0, this._q, 0, _qLength );
		}
		else if ( array.Length is > 0 and < _qLength ) {
			var b = Array.ConvertAll( array, j => {
				j ^= j << 0xD;
				j ^= j >> 0x11;
				j ^= j << 0x5;

				return j;
			} );
			Buffer.BlockCopy( b, 0, this._q, 0, _qLength );
		}
		else {
			var j = DateTime.UtcNow.Ticks;

			var b = new[] {
				j >> 0x11, j << 0x5
			};

			Buffer.BlockCopy( b, 0, this._q, 0, _qLength );
		}
	}

	private UInt32 Cmwc() {
		const UInt64 a = 487198574UL;
		const UInt32 r = 0xfffffffe;

		this._i = ( this._i + 1 ) & 15;
		var t = a * this._q[this._i] + this._c;
		this._c = ( UInt32 )( t >> 32 );
		var x = ( UInt32 )( t + this._c );

		if ( x >= this._c ) {
			return this._q[this._i] = r - x;
		}

		x++;
		this._c++;

		return this._q[this._i] = r - x;
	}

	/// <summary>Get a new random System.Double value</summary>
	/// <returns>The random Double</returns>
	protected override Double Sample() => this.NextDouble();

	/// <summary>Get a new random System.Int32 value</summary>
	/// <returns>The random int</returns>
	public override Int32 Next() => ( Int32 )this.Cmwc();

	/// <summary>Get a random integer between two given bounds</summary>
	/// <exception cref="ArgumentException">If max is less than min</exception>
	/// <param name="min">The lower bound (inclusive)</param>
	/// <param name="max">The upper bound (exclusive)</param>
	public override Int32 Next( Int32 min, Int32 max ) {
		if ( min > max ) {
			throw new ArgumentException( "min must be less than or equal to max" );
		}

		return min + ( Int32 )( this.Cmwc() / 4294967296.0 * ( max - min ) );
	}

	/// <summary>Get a random non-negative integer less than a given upper bound</summary>
	/// <exception cref="ArgumentException">If max is negative</exception>
	/// <param name="max">The upper bound (exclusive)</param>
	public override Int32 Next( Int32 max ) {
		if ( max < 0 ) {
			throw new ArgumentException( "max must be non-negative" );
		}

		return ( Int32 )( this.Cmwc() / 4294967296.0 * max );
	}

	/// <summary>Fill a array of byte with random bytes</summary>
	/// <param name="buffer">The array to fill</param>
	public override void NextBytes( Byte[] buffer ) {
		if ( buffer is null ) {
			throw new ArgumentEmptyException( nameof( buffer ) );
		}

		for ( Int32 i = 0, length = buffer.Length; i < length; i++ ) {
			buffer[i] = ( Byte )this.Cmwc();
		}
	}

	/// <summary>Get a new random System.Double value</summary>
	/// <returns>The random Double</returns>
	public override Double NextDouble() => this.Cmwc() / 4294967296.0;
}