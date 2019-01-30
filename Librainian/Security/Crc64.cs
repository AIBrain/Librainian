// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Crc64.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Crc64.cs" was last formatted by Protiguous on 2018/07/13 at 1:37 AM.

namespace Librainian.Security {

	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using JetBrains.Annotations;

	/// <summary>
	///     Implements a 64-bit CRC hash algorithm for a given polynomial.
	/// </summary>
	/// <remarks>For ISO 3309 compliant 64-bit CRC's use Crc64Iso.</remarks>
	/// <copyright>
	///     Damien Guard. All rights reserved.
	/// </copyright>
	/// <see cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
	public class CRC64 : HashAlgorithm {

		private readonly UInt64 _seed;

		private readonly UInt64[] _table;

		private UInt64 _hash;

		public override Int32 HashSize => 64;

		protected const UInt64 DefaultSeed = 0x0;

		public CRC64( UInt64 polynomial, UInt64 seed = DefaultSeed ) {
			this._table = InitializeTable( polynomial: polynomial );
			this._seed = this._hash = seed;
		}

		[NotNull]
		private static UInt64[] InitializeTable( UInt64 polynomial ) {
			if ( polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null ) { return Crc64Iso.Table; }

			var createTable = CreateTable( polynomial: polynomial );

			if ( polynomial == Crc64Iso.Iso3309Polynomial ) { Crc64Iso.Table = createTable; }

			return createTable;
		}

		[NotNull]
		private static Byte[] UInt64ToBigEndianBytes( UInt64 value ) {
			var result = BitConverter.GetBytes( value );

			if ( BitConverter.IsLittleEndian ) { Array.Reverse( array: result ); }

			return result;
		}

		protected static UInt64 CalculateHash( UInt64 seed, UInt64[] table, IList<Byte> buffer, Int32 start, Int32 size ) {
			var crc = seed;

			for ( var i = start; i < size; i++ ) {
				unchecked { crc = ( crc >> 8 ) ^ table[ ( buffer[ index: i ] ^ crc ) & 0xff ]; }
			}

			return crc;
		}

		[NotNull]
		protected static UInt64[] CreateTable( UInt64 polynomial ) {
			var createTable = new UInt64[ 256 ]; //did they mean 255 here (Byte.MaxValue)??

			for ( var i = 0; i < 256; ++i ) {
				var entry = ( UInt64 ) i;

				for ( var j = 0; j < 8; ++j ) {
					if ( ( entry & 1 ) == 1 ) { entry = ( entry >> 1 ) ^ polynomial; }
					else { entry = entry >> 1; }
				}

				createTable[ i ] = entry;
			}

			return createTable;
		}

		protected override void HashCore( Byte[] buffer, Int32 start, Int32 length ) => this._hash = CalculateHash( seed: this._hash, table: this._table, buffer: buffer, start: start, size: length );

		[NotNull]
		protected override Byte[] HashFinal() {
			var hashBuffer = UInt64ToBigEndianBytes( this._hash );
			this.HashValue = hashBuffer;

			return hashBuffer;
		}

		public override void Initialize() => this._hash = this._seed;
	}
}