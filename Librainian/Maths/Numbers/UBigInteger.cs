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
// File "UBigInteger.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

#nullable enable

namespace Librainian.Maths.Numbers;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Exceptions;
using Extensions;
using Rationals;
using Utilities;

/// <summary>
///     <para>Unsigned biginteger class.</para>
/// </summary>
[Immutable]
public readonly record struct UBigInteger : IComparable<UBigInteger> {

	/// <summary>
	///     <para>The lowest <see cref="UBigInteger" /> that is higher than <see cref="Zero" />.</para>
	///     <para>Should be "1".</para>
	/// </summary>
	public static readonly UBigInteger Epsilon = new(1);

	/// <summary>1</summary>
	public static readonly UBigInteger One = new(1);

	/// <summary>2</summary>
	public static readonly UBigInteger Two = new(2);

	/// <summary>0</summary>
	public static readonly UBigInteger Zero = new(0);

	private UBigInteger( BigInteger value ) {
		//value.Should().BeGreaterOrEqualTo(expected: BigInteger.Zero);

		if ( value < BigInteger.Zero ) {
			throw new ArgumentOutOfRangeException( nameof( value ), $"{nameof( value )} cannot be less than 0." );
		}

		this.Value = value;
	}

	public UBigInteger( UInt64 value ) => this.Value = value;

	public UBigInteger( Byte[] bytes ) {
		// http: //stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger
		if ( bytes is null ) {
			throw new ArgumentEmptyException( nameof( bytes ) );
		}

		var bytesWith00Attheendnd = new Byte[ bytes.Length + 1 ];
		bytes.CopyTo( bytesWith00Attheendnd, 0 );
		bytesWith00Attheendnd[ bytes.Length ] = 0;
		this.Value = new BigInteger( bytesWith00Attheendnd );

		Debug.Assert( this.Value >= BigInteger.Zero );

		if ( this.Value < 0 ) {
			throw new ArgumentOutOfRangeException( nameof( bytes ), $"Error converting {nameof( bytes )} to {nameof( UBigInteger )}." );
		}
	}

	public UBigInteger( Int64 value ) {
		if ( value < 0 ) {
			throw new ArgumentOutOfRangeException( nameof( value ), $"Error converting {nameof( value )} to {nameof( UBigInteger )}." );
		}

		this.Value = value;
	}

	private BigInteger Value { get; }

	public Int32 CompareTo( UBigInteger? number ) => this.Value.CompareTo( number?.Value );

	public static UBigInteger Add( UBigInteger left, UBigInteger right ) => new(BigInteger.Add( left.Value, right.Value ));

	public static explicit operator Decimal( UBigInteger number ) => ( Decimal ) number.Value;

	public static explicit operator Int32( UBigInteger number ) => ( Int32 ) number.Value;

	public static explicit operator Int64( UBigInteger number ) => ( Int64 ) number.Value;

	public static explicit operator UInt64( UBigInteger number ) => ( UInt64 ) number.Value;

	public static implicit operator BigInteger( UBigInteger number ) => number.Value;

	public static implicit operator UBigInteger( Int64 number ) => new(number);

	public static UBigInteger Multiply( UBigInteger left, UBigInteger right ) => new(BigInteger.Multiply( left.Value, right.Value ));

	public static UBigInteger operator -( UBigInteger number ) => new(-number.Value);

	public static UBigInteger operator -( UBigInteger left, UBigInteger right ) => new(left.Value - right.Value);

	public static UBigInteger operator %( UBigInteger dividend, UBigInteger divisor ) => new(dividend.Value % divisor.Value);

	public static UBigInteger operator &( UBigInteger left, UBigInteger right ) => new(left.Value & right.Value);

	public static UBigInteger operator *( UBigInteger left, UBigInteger right ) => new(left.Value * right.Value);

	public static UBigInteger operator /( UBigInteger left, UBigInteger right ) => new(left.Value / right.Value);

	public static Double operator /( Double left, UBigInteger right ) {
		Debug.Assert( right > Zero );

		var rational = new Rational( new BigInteger( left ), right.Value );

		return ( Double ) rational;
	}

	public static UBigInteger operator +( UBigInteger left, UBigInteger right ) => new(left.Value + right.Value);

	public static Boolean operator <( UBigInteger left, Int64 right ) => left.Value < right;

	public static Boolean operator <( UBigInteger left, UBigInteger right ) => left.Value < right.Value;

	public static Boolean operator <( UBigInteger left, UInt64 right ) => left.Value < right;

	public static Boolean operator <( UInt64 left, UBigInteger right ) => left < right.Value;

	public static UBigInteger operator <<( UBigInteger number, Int32 shift ) => new(number.Value << shift);

	public static Boolean operator <=( UBigInteger left, UInt64 right ) => left.Value <= right;

	public static Boolean operator <=( UBigInteger left, UBigInteger right ) => left.Value <= right.Value;

	public static Boolean operator >( UBigInteger left, Int64 right ) => left.Value > right;

	public static Boolean operator >( UBigInteger left, UInt64 right ) => left.Value > right;

	public static Boolean operator >( UInt64 left, UBigInteger right ) => left > right.Value;

	public static Boolean operator >( UBigInteger left, UBigInteger right ) => left.Value > right.Value;

	public static Boolean operator >=( UBigInteger left, UInt64 right ) => left.Value >= right;

	public static Boolean operator >=( UBigInteger left, UBigInteger right ) => left.Value >= right.Value;

	public static UBigInteger Parse( String number, NumberStyles style ) {
		if ( number is null ) {
			throw new ArgumentEmptyException( nameof( number ) );
		}

		return new UBigInteger( BigInteger.Parse( number, style ) );
	}

	public static UBigInteger Pow( UBigInteger number, Int32 exponent ) => new(BigInteger.Pow( number.Value, exponent ));

	public Int32 CompareTo( Int64 other ) => this.Value.CompareTo( other );

	public Int32 CompareTo( UInt64 other ) => this.Value.CompareTo( other );

	public Byte[] ToByteArray() => this.Value.ToByteArray();

	public override String ToString() => this.Value.ToString();

	public String ToString( String format ) => this.Value.ToString( format );

	public Int32 CompareTo( UBigInteger other ) => this.Value.CompareTo( other.Value );

}