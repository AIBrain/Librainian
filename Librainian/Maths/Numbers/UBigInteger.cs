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
// File "UBigInteger.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

#nullable enable

namespace Librainian.Maths.Numbers;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Exceptions;
using Extensions;
using JetBrains.Annotations;
using Rationals;

/// <summary>
/// <para>Unsigned biginteger class.</para>
/// </summary>
[Immutable]
public struct UBigInteger : IComparable, IComparable<UBigInteger> {

	/// <summary>
	/// <para>The lowest <see cref="UBigInteger" /> that is higher than <see cref="Zero" />.</para>
	/// <para>Should be "1".</para>
	/// </summary>
	public static readonly UBigInteger Epsilon = new( 1 );

	/// <summary>1</summary>
	public static readonly UBigInteger One = new( 1 );

	/// <summary>2</summary>
	public static readonly UBigInteger Two = new( 2 );

	/// <summary>0</summary>
	public static readonly UBigInteger Zero = new( 0 );

	private UBigInteger( BigInteger value ) {

		//value.Should().BeGreaterOrEqualTo(expected: BigInteger.Zero);

		if ( value < BigInteger.Zero ) {
			throw new ArgumentOutOfRangeException( nameof( value ), $"{nameof( value )} cannot be less than 0." );
		}

		this.InternalValue = value;
	}

	public UBigInteger( UInt64 value ) => this.InternalValue = value;

	public UBigInteger( Byte[] bytes ) {

		// http: //stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger
		if ( bytes is null ) {
			throw new NullException( nameof( bytes ) );
		}

		var bytesWith00Attheendnd = new Byte[ bytes.Length + 1 ];
		bytes.CopyTo( bytesWith00Attheendnd, 0 );
		bytesWith00Attheendnd[ bytes.Length ] = 0;
		this.InternalValue = new BigInteger( bytesWith00Attheendnd );

		Debug.Assert( this.InternalValue >= BigInteger.Zero );

		if ( this.InternalValue < 0 ) {
			throw new ArgumentOutOfRangeException( nameof( bytes ), $"Error converting {nameof( bytes )} to {nameof( UBigInteger )}." );
		}
	}

	public UBigInteger( Int64 value ) {

		//value.Should().BeGreaterOrEqualTo(expected: 0);

		if ( value < 0 ) {
			throw new ArgumentOutOfRangeException( nameof( value ), $"Error converting {nameof( value )} to {nameof( UBigInteger )}." );
		}

		this.InternalValue = value;
	}

	private BigInteger InternalValue { get; }

	public static UBigInteger Add( UBigInteger left, UBigInteger right ) => new( BigInteger.Add( left.InternalValue, right.InternalValue ) );

	public static explicit operator Decimal( UBigInteger number ) => ( Decimal )number.InternalValue;

	public static explicit operator Int32( UBigInteger number ) => ( Int32 )number.InternalValue;

	public static explicit operator Int64( UBigInteger number ) => ( Int64 )number.InternalValue;

	public static explicit operator UInt64( UBigInteger number ) => ( UInt64 )number.InternalValue;

	public static implicit operator BigInteger( UBigInteger number ) => number.InternalValue;

	public static implicit operator UBigInteger( Int64 number ) => new( number );

	public static UBigInteger Multiply( UBigInteger left, UBigInteger right ) => new( BigInteger.Multiply( left.InternalValue, right.InternalValue ) );

	public static UBigInteger operator -( UBigInteger number ) => new( -number.InternalValue );

	public static UBigInteger operator -( UBigInteger left, UBigInteger right ) => new( left.InternalValue - right.InternalValue );

	public static UBigInteger operator %( UBigInteger dividend, UBigInteger divisor ) => new( dividend.InternalValue % divisor.InternalValue );

	public static UBigInteger operator &( UBigInteger left, UBigInteger right ) => new( left.InternalValue & right.InternalValue );

	public static UBigInteger operator *( UBigInteger left, UBigInteger right ) => new( left.InternalValue * right.InternalValue );

	public static UBigInteger operator /( UBigInteger left, UBigInteger right ) => new( left.InternalValue / right.InternalValue );

	public static Double operator /( Double left, UBigInteger right ) {
		Debug.Assert( right > Zero );

		var rational = new Rational( new BigInteger( left ), right.InternalValue );

		return ( Double )rational;
	}

	public static UBigInteger operator +( UBigInteger left, UBigInteger right ) => new( left.InternalValue + right.InternalValue );

	public static Boolean operator <( UBigInteger left, Int64 right ) => left.InternalValue < right;

	public static Boolean operator <( UBigInteger left, UBigInteger right ) => left.InternalValue < right.InternalValue;

	public static Boolean operator <( UBigInteger left, UInt64 right ) => left.InternalValue < right;

	public static Boolean operator <( UInt64 left, UBigInteger right ) => left < right.InternalValue;

	public static UBigInteger operator <<( UBigInteger number, Int32 shift ) => new( number.InternalValue << shift );

	public static Boolean operator <=( UBigInteger left, UInt64 right ) => left.InternalValue <= right;

	public static Boolean operator <=( UBigInteger left, UBigInteger right ) => left.InternalValue <= right.InternalValue;

	public static Boolean operator >( UBigInteger left, Int64 right ) => left.InternalValue > right;

	public static Boolean operator >( UBigInteger left, UInt64 right ) => left.InternalValue > right;

	public static Boolean operator >( UInt64 left, UBigInteger right ) => left > right.InternalValue;

	public static Boolean operator >( UBigInteger left, UBigInteger right ) => left.InternalValue > right.InternalValue;

	public static Boolean operator >=( UBigInteger left, UInt64 right ) => left.InternalValue >= right;

	public static Boolean operator >=( UBigInteger left, UBigInteger right ) => left.InternalValue >= right.InternalValue;

	public static UBigInteger Parse( String number, NumberStyles style ) {
		if ( number is null ) {
			throw new NullException( nameof( number ) );
		}

		return new UBigInteger( BigInteger.Parse( number, style ) );
	}

	public static UBigInteger Pow( UBigInteger number, Int32 exponent ) => new( BigInteger.Pow( number.InternalValue, exponent ) );

	[Pure]
	public Int32 CompareTo( Object? obj ) =>
		obj switch {
			null => throw new NullException( nameof( obj ) ),
			UBigInteger uBigInteger => this.InternalValue.CompareTo( uBigInteger ),
			var _ => throw new InvalidCastException( $"Error casting {nameof( obj )} to a {nameof( UBigInteger )}" )
		};

	public Int32 CompareTo( UBigInteger number ) => this.InternalValue.CompareTo( number.InternalValue );

	public Int32 CompareTo( Int64 other ) => this.InternalValue.CompareTo( other );

	public Int32 CompareTo( UInt64 other ) => this.InternalValue.CompareTo( other );

	public Byte[] ToByteArray() => this.InternalValue.ToByteArray();

	public override String ToString() => this.InternalValue.ToString();

	public String ToString( String format ) => this.InternalValue.ToString( format );
}