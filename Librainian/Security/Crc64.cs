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
// File "Crc64.cs" last formatted on 2020-08-14 at 8:44 PM.

namespace Librainian.Security {

	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;

	/// <summary>Implements a 64-bit CRC hash algorithm for a given polynomial.</summary>
	/// <remarks>For ISO 3309 compliant 64-bit CRC's use Crc64Iso.</remarks>
	/// <copyright>Damien Guard. All rights reserved.</copyright>
	/// <see cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
	public class CRC64 : HashAlgorithm {

		private readonly UInt64 _seed;

		private readonly UInt64[] _table;

		private UInt64 _hash;

		protected const UInt64 DefaultSeed = 0x0;

		public override Int32 HashSize => 64;

		public CRC64( UInt64 polynomial, UInt64 seed = DefaultSeed ) {
			this._table = InitializeTable( polynomial );
			this._seed = this._hash = seed;
		}

		private static UInt64[] InitializeTable( UInt64 polynomial ) {
			if ( Crc64Iso.Table != null && polynomial == Crc64Iso.Iso3309Polynomial ) {
				return Crc64Iso.Table;
			}

			var createTable = CreateTable( polynomial );

			if ( polynomial == Crc64Iso.Iso3309Polynomial ) {
				Crc64Iso.Table = createTable;
			}

			return createTable;
		}

		private static Byte[] UInt64ToBigEndianBytes( UInt64 value ) {
			var result = BitConverter.GetBytes( value );

			if ( BitConverter.IsLittleEndian ) {
				Array.Reverse( result );
			}

			return result;
		}

		protected static UInt64 CalculateHash( UInt64 seed, UInt64[]? table, IList<Byte>? buffer, Int32 start, Int32 size ) {
			var crc = seed;

			for ( var i = start; i < size; i++ ) {
				unchecked {
					crc = ( crc >> 8 ) ^ table[ ( buffer[ i ] ^ crc ) & 0xff ];
				}
			}

			return crc;
		}

		protected static UInt64[] CreateTable( UInt64 polynomial ) {
			var createTable = new UInt64[ 256 ]; //did they mean 255 here (Byte.MaxValue)??

			for ( var i = 0; i < 256; ++i ) {
				var entry = ( UInt64 )i;

				for ( var j = 0; j < 8; ++j ) {
					if ( ( entry & 1 ) == 1 ) {
						entry = ( entry >> 1 ) ^ polynomial;
					}
					else {
						entry >>= 1;
					}
				}

				createTable[ i ] = entry;
			}

			return createTable;
		}

		protected override void HashCore( Byte[] buffer, Int32 start, Int32 length ) => this._hash = CalculateHash( this._hash, this._table, buffer, start, length );

		protected override Byte[] HashFinal() {
			var hashBuffer = UInt64ToBigEndianBytes( this._hash );
			this.HashValue = hashBuffer;

			return hashBuffer;
		}

		public override void Initialize() => this._hash = this._seed;
	}
}