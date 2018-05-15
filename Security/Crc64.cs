// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Crc64.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    /// Implements a 64-bit CRC hash algorithm for a given polynomial.
    /// </summary>
    /// <remarks>For ISO 3309 compliant 64-bit CRC's use Crc64Iso.</remarks>
    /// <copyright>
    ///     Damien Guard. All rights reserved.
    /// </copyright>
    /// <seealso cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs"/>
    public class Crc64 : HashAlgorithm {
        private readonly UInt64 _seed;
        private readonly UInt64[] _table;
        private UInt64 _hash;
        protected const UInt64 DefaultSeed = 0x0;

        public Crc64( UInt64 polynomial, UInt64 seed = DefaultSeed ) {
            this._table = InitializeTable( polynomial: polynomial );
            this._seed = this._hash = seed;
        }

        public override Int32 HashSize => 64;

        private static UInt64[] InitializeTable( UInt64 polynomial ) {
            if ( polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null ) {
                return Crc64Iso.Table;
            }

            var createTable = CreateTable( polynomial: polynomial );

            if ( polynomial == Crc64Iso.Iso3309Polynomial ) {
                Crc64Iso.Table = createTable;
            }

            return createTable;
        }

        private static Byte[] UInt64ToBigEndianBytes( UInt64 value ) {
            var result = BitConverter.GetBytes( value: value );

            if ( BitConverter.IsLittleEndian ) {
                Array.Reverse( array: result );
            }

            return result;
        }

        protected static UInt64 CalculateHash( UInt64 seed, UInt64[] table, IList<Byte> buffer, Int32 start, Int32 size ) {
            var crc = seed;

            for ( var i = start; i < size; i++ ) {
                unchecked {
                    crc = ( crc >> 8 ) ^ table[( buffer[index: i] ^ crc ) & 0xff];
                }
            }

            return crc;
        }

        protected static UInt64[] CreateTable( UInt64 polynomial ) {
            var createTable = new UInt64[256];
            for ( var i = 0; i < 256; ++i ) {
                var entry = ( UInt64 )i;
                for ( var j = 0; j < 8; ++j ) {
                    if ( ( entry & 1 ) == 1 ) {
                        entry = ( entry >> 1 ) ^ polynomial;
                    }
                    else {
                        entry = entry >> 1;
                    }
                }

                createTable[i] = entry;
            }

            return createTable;
        }

        protected override void HashCore( Byte[] buffer, Int32 start, Int32 length ) => this._hash = CalculateHash( seed: this._hash, table: this._table, buffer: buffer, start: start, size: length );

        protected override Byte[] HashFinal() {
            var hashBuffer = UInt64ToBigEndianBytes( value: this._hash );
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        public override void Initialize() => this._hash = this._seed;
    }
}