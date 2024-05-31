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
// File "MersenneTwister.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

#nullable enable

namespace Librainian.Security;

using System;
using Exceptions;

/// <summary>
/// <para>Mersenne Twister random number generator; from <see cref="http://takel.jp/mt/MersenneTwister.cs" /></para>
/// </summary>
internal class MersenneTwister : Random {

	private const UInt32 LowerMask = 0b1111111111111111111111111111111;

	private const Int32 M = 0b110001101;

	private const UInt32 MatrixA = 0b10011001000010001011000011011111;

	private const Int32 N = 0b1001110000;

	private const UInt32 TemperingMaskB = 0x9d2c5680;

	private const UInt32 TemperingMaskC = 0xefc60000;

	private const UInt32 UpperMask = 0x80000000;

	private static readonly UInt32[] Mag01 = {
		0x0, MatrixA
	};

	private readonly UInt32[] _mt = new UInt32[ N ]; /* the array for the state vector  */

	private Int16 _mti;

	public MersenneTwister( UInt32 seed ) {
		/* setting initial seeds to mt[N] using         */
		/* the generator Line 25 of Table 1 in          */
		/* [KNUTH 1981, The Art of Computer Programming */
		/*    Vol. 2 (2nd Ed.), pp102]                  */
		this._mt[ 0 ] = seed & 0xffffffffU;

		for ( this._mti = 1; this._mti < N; ++this._mti ) {
			this._mt[ this._mti ] = ( 69069 * this._mt[ this._mti - 1 ] ) & 0xffffffffU;
		}
	}

	/// <summary>a default initial seed is used</summary>
	public MersenneTwister() : this( 4357 ) { }

	private static UInt32 TEMPERING_SHIFT_L( UInt32 y ) => y >> 18;

	private static UInt32 TEMPERING_SHIFT_S( UInt32 y ) => y << 7;

	private static UInt32 TEMPERING_SHIFT_T( UInt32 y ) => y << 15;

	private static UInt32 TEMPERING_SHIFT_U( UInt32 y ) => y >> 11;

	protected UInt32 GenerateUInt() {
		UInt32 y;

		/* mag01[x] = x * MATRIX_A  for x=0,1 */
		if ( this._mti >= N ) /* generate N words at one time */ {
			Int16 kk = 0;

			for ( ; kk < N - M; ++kk ) {
				y = ( this._mt[ kk ] & UpperMask ) | ( this._mt[ kk + 1 ] & LowerMask );
				this._mt[ kk ] = this._mt[ kk + M ] ^ ( y >> 1 ) ^ Mag01[ y & 0x1 ];
			}

			for ( ; kk < N - 1; ++kk ) {
				y = ( this._mt[ kk ] & UpperMask ) | ( this._mt[ kk + 1 ] & LowerMask );
				this._mt[ kk ] = this._mt[ kk + ( M - N ) ] ^ ( y >> 1 ) ^ Mag01[ y & 0x1 ];
			}

			y = ( this._mt[ N - 1 ] & UpperMask ) | ( this._mt[ 0 ] & LowerMask );
			this._mt[ N - 1 ] = this._mt[ M - 1 ] ^ ( y >> 1 ) ^ Mag01[ y & 0x1 ];

			this._mti = 0;
		}

		y = this._mt[ this._mti++ ];
		y ^= TEMPERING_SHIFT_U( y );
		y ^= TEMPERING_SHIFT_S( y ) & TemperingMaskB;
		y ^= TEMPERING_SHIFT_T( y ) & TemperingMaskC;
		y ^= TEMPERING_SHIFT_L( y );

		return y;
	}

	public override Int32 Next() => this.Next( Int32.MaxValue );

	public override Int32 Next( Int32 maxValue ) /* throws ArgumentOutOfRangeException */ {
		if ( maxValue > 1 ) {
			return ( Int32 )( this.NextDouble() * maxValue );
		}

		if ( maxValue < 0 ) {
			throw new ArgumentOutOfRangeException();
		}

		return 0;
	}

	public override Int32 Next( Int32 minValue, Int32 maxValue ) {
		if ( maxValue < minValue ) {
			throw new ArgumentOutOfRangeException();
		}

		if ( maxValue == minValue ) {
			return minValue;
		}

		return this.Next( maxValue - minValue ) + minValue;
	}

	/// <param name="buffer"></param>
	/// <exception cref="NullException"></exception>
	public override void NextBytes( Byte[] buffer ) {
		if ( buffer is null ) {
			throw new NullException( nameof( buffer ) );
		}

		var bufLen = buffer.Length;

		for ( var idx = 0; idx < bufLen; ++idx ) {
			buffer[ idx ] = ( Byte )this.Next( 256 );
		}
	}

	public override Double NextDouble() => ( Double )this.GenerateUInt() / ( ( UInt64 )UInt32.MaxValue + 1 );

	public virtual UInt32 NextUInt() => this.GenerateUInt();

	public virtual UInt32 NextUInt( UInt32 maxValue ) => ( UInt32 )( this.GenerateUInt() / ( ( Double )UInt32.MaxValue / maxValue ) );

	public virtual UInt32 NextUInt( UInt32 minValue, UInt32 maxValue ) /* throws ArgumentOutOfRangeException */ {
		if ( minValue >= maxValue ) {
			throw new ArgumentOutOfRangeException();
		}

		return ( UInt32 )( this.GenerateUInt() / ( ( Double )UInt32.MaxValue / ( maxValue - minValue ) ) + minValue );
	}

	/* Period parameters */
	/* constant vector a */
	/* most significant w-r bits */

	/* least significant r bits */
	/* Tempering parameters */
	/* initializing the array with a NONZERO seed */
}