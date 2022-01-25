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
// File "Bits.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

namespace Librainian.Maths;

using System;
using System.Linq;
using Exceptions;
using Numbers;

public static class Bits {

	public static Boolean IsLittleEndian => BitConverter.IsLittleEndian;

	private static UInt64 ToUInt64( this Byte[] value, Int32 startIndex = 0 ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return BitConverter.ToUInt64( Order( value ), startIndex );
	}

	public static Byte[] GetBytes( this Int16 value ) => Order( BitConverter.GetBytes( value ) );

	public static Byte[] GetBytes( this UInt16 value ) => Order( BitConverter.GetBytes( value ) );

	public static Byte[] GetBytes( this Int32 value ) => Order( BitConverter.GetBytes( value ) );

	public static Byte[] GetBytes( this UInt32 value ) => Order( BitConverter.GetBytes( value ) );

	public static Byte[] GetBytes( this Int64 value ) => Order( BitConverter.GetBytes( value ) );

	public static Byte[] GetBytes( this UInt64 value ) => Order( BitConverter.GetBytes( value ) );

	public static Byte[] GetBytes( this UInt256 value ) => value.ToByteArray();

	public static Byte[] Order( this Byte[] value ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return IsLittleEndian ? value : value.Reverse().ToArray();
	}

	public static Int32 ToInt32( this Byte[] value, Int32 startIndex = 0 ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return BitConverter.ToInt32( Order( value ), startIndex );
	}

	public static String ToString( this Byte[] value, Int32 startIndex = 0 ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return BitConverter.ToString( Order( value ), startIndex );
	}

	public static UInt16 ToUInt16( this Byte[] value, Int32 startIndex = 0 ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return BitConverter.ToUInt16( Order( value ), startIndex );
	}

	public static UInt256 ToUInt256( this Byte[] value ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return new UInt256( value );
	}

	public static UInt32 ToUInt32( this Byte[] value, Int32 startIndex = 0 ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return BitConverter.ToUInt32( Order( value ), startIndex );
	}
}