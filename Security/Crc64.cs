// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Crc64.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Crc64.cs" was last formatted by Protiguous on 2018/05/17 at 3:58 PM.

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    ///     Implements a 64-bit CRC hash algorithm for a given polynomial.
    /// </summary>
    /// <remarks>For ISO 3309 compliant 64-bit CRC's use Crc64Iso.</remarks>
    /// <copyright>
    ///     Damien Guard. All rights reserved.
    /// </copyright>
    /// <seealso cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
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
            if ( polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null ) { return Crc64Iso.Table; }

            var createTable = CreateTable( polynomial: polynomial );

            if ( polynomial == Crc64Iso.Iso3309Polynomial ) { Crc64Iso.Table = createTable; }

            return createTable;
        }

        private static Byte[] UInt64ToBigEndianBytes( UInt64 value ) {
            var result = BitConverter.GetBytes( value );

            if ( BitConverter.IsLittleEndian ) { Array.Reverse( array: result ); }

            return result;
        }

        protected static UInt64 CalculateHash( UInt64 seed, UInt64[] table, IList<Byte> buffer, Int32 start, Int32 size ) {
            var crc = seed;

            for ( var i = start; i < size; i++ ) {
                unchecked { crc = ( crc >> 8 ) ^ table[( buffer[index: i] ^ crc ) & 0xff]; }
            }

            return crc;
        }

        protected static UInt64[] CreateTable( UInt64 polynomial ) {
            var createTable = new UInt64[256]; //did they mean 255 here (Byte.MaxValue)??

            for ( var i = 0; i < 256; ++i ) {
                var entry = ( UInt64 )i;

                for ( var j = 0; j < 8; ++j ) {
                    if ( ( entry & 1 ) == 1 ) { entry = ( entry >> 1 ) ^ polynomial; }
                    else { entry = entry >> 1; }
                }

                createTable[i] = entry;
            }

            return createTable;
        }

        protected override void HashCore( Byte[] buffer, Int32 start, Int32 length ) => this._hash = CalculateHash( seed: this._hash, table: this._table, buffer: buffer, start: start, size: length );

        protected override Byte[] HashFinal() {
            var hashBuffer = UInt64ToBigEndianBytes( this._hash );
            this.HashValue = hashBuffer;

            return hashBuffer;
        }

        public override void Initialize() => this._hash = this._seed;
    }
}