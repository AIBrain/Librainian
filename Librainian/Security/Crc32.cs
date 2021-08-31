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
// File "Crc32.cs" last formatted on 2020-08-14 at 8:44 PM.

#nullable enable

namespace Librainian.Security {

	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;

	/// <summary>Implements a 32-bit CRC hash algorithm compatible with Zip etc.</summary>
	/// <remarks>
	///     Crc32 should only be used for backward compatibility with older file formats and algorithms. It is not secure
	///     enough for new applications. If you need to call multiple
	///     times for the same data either use the HashAlgorithm interface or remember that the result of one Compute call
	///     needs to be ~ (XOR) before being passed in as the seed for the next
	///     Compute call.
	/// </remarks>
	/// <copyright>
	///     Copyright (c) Damien Guard. All rights reserved. Licensed under the Apache License. Originally published at
	///     http://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net
	/// </copyright>
	public sealed class CRC32 : HashAlgorithm {

		private static UInt32[]? _defaultTable;

		private UInt32 _hash;

		public const UInt32 DefaultPolynomial = 0xEDB88320;

		public const UInt32 DefaultSeed = 0xFFFFFFFFu;

		private UInt32 Seed { get; }

		private UInt32[] Table { get; }

		public override Int32 HashSize => 0x20;

		public CRC32() : this( DefaultPolynomial, DefaultSeed ) { }

		public CRC32( UInt32 polynomial, UInt32 seed ) {
			this.Table = InitializeTable( polynomial );
			this.Seed = this._hash = seed;
		}

		private static UInt32[] InitializeTable( UInt32 polynomial ) {
			if ( _defaultTable != null ) {
				return _defaultTable;
			}

			var createTable = new UInt32[256];

			for ( var i = 0; i < 256; i++ ) {
				var entry = ( UInt32 )i;

				for ( var j = 0; j < 8; j++ ) {
					if ( ( entry & 1 ) == 1 ) {
						entry = ( entry >> 1 ) ^ polynomial;
					}
					else {
						entry >>= 1;
					}
				}

				createTable[i] = entry;
			}

			if ( polynomial == DefaultPolynomial ) {
				_defaultTable = createTable;
			}

			return createTable;
		}

		protected override void HashCore( Byte[] buffer, Int32 start, Int32 length ) => this._hash = CalculateHash( this.Table, this._hash, buffer, start, length );

		protected override Byte[] HashFinal() {
			var hashBuffer = UInt32ToBigEndianBytes( ~this._hash );
			this.HashValue = hashBuffer;

			return hashBuffer;
		}

		internal static Byte[] UInt32ToBigEndianBytes( UInt32 uint32 ) {
			var result = BitConverter.GetBytes( uint32 );

			if ( BitConverter.IsLittleEndian ) {
				Array.Reverse( result );
			}

			return result;
		}

		/// <summary></summary>
		/// <param name="table"> </param>
		/// <param name="seed">  </param>
		/// <param name="buffer"></param>
		/// <param name="start"> </param>
		/// <param name="size">  </param>
		public static UInt32 CalculateHash( UInt32[] table, UInt32 seed, IList<Byte> buffer, Int32 start, Int32 size ) {
			var crc = seed;

			for ( var i = start; i < size - start; i++ ) {
				crc = ( crc >> 8 ) ^ table[buffer[i] ^ ( crc & 0xff )];
			}

			return crc;
		}

		public static UInt32 Compute( Byte[] buffer ) => Compute( DefaultSeed, buffer );

		public static UInt32 Compute( UInt32 seed, Byte[] buffer ) => Compute( DefaultPolynomial, seed, buffer );

		public static UInt32 Compute( UInt32 polynomial, UInt32 seed, Byte[] buffer ) =>
			~CalculateHash( InitializeTable( polynomial ), seed, buffer, 0, buffer.Length );

		public override void Initialize() => this._hash = this.Seed;
	}
}