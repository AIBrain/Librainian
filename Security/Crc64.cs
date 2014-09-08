#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Crc64.cs" was last cleaned by Rick on 2014/09/08 at 11:35 AM
#endregion

namespace Librainian.Security {
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    ///     Implements a 64-bit CRC hash algorithm for a given polynomial.
    /// </summary>
    /// <remarks>
    ///     For ISO 3309 compliant 64-bit CRC's use Crc64Iso.
    /// </remarks>
    /// <copyright>Damien Guard.  All rights reserved.</copyright>
    /// <seealso cref="http://github.com/damieng/DamienGKit/blob/master/CSharp/DamienG.Library/Security/Cryptography/Crc64.cs" />
    public class Crc64 : HashAlgorithm {
        public const UInt64 DefaultSeed = 0x0;

        private readonly UInt64 _seed;
        private readonly UInt64[] _table;
        private UInt64 _hash;

        public Crc64( UInt64 polynomial, UInt64 seed = DefaultSeed ) {
            this._table = InitializeTable( polynomial );
            this._seed = this._hash = seed;
        }

        public override int HashSize { get { return 64; } }

        public override void Initialize() {
            this._hash = this._seed;
        }

        protected override void HashCore( byte[] buffer, int start, int length ) {
            this._hash = CalculateHash( this._hash, this._table, buffer, start, length );
        }

        protected override byte[] HashFinal() {
            var hashBuffer = UInt64ToBigEndianBytes( this._hash );
            this.HashValue = hashBuffer;
            return hashBuffer;
        }

        protected static UInt64 CalculateHash( UInt64 seed, UInt64[] table, IList< byte > buffer, int start, int size ) {
            var crc = seed;

            for ( var i = start; i < size; i++ ) {
                unchecked {
                    crc = ( crc >> 8 ) ^ table[ ( buffer[ i ] ^ crc ) & 0xff ];
                }
            }

            return crc;
        }

        private static byte[] UInt64ToBigEndianBytes( UInt64 value ) {
            var result = BitConverter.GetBytes( value );

            if ( BitConverter.IsLittleEndian ) {
                Array.Reverse( result );
            }

            return result;
        }

        private static UInt64[] InitializeTable( UInt64 polynomial ) {
            if ( polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null ) {
                return Crc64Iso.Table;
            }

            var createTable = CreateTable( polynomial );

            if ( polynomial == Crc64Iso.Iso3309Polynomial ) {
                Crc64Iso.Table = createTable;
            }

            return createTable;
        }

        protected static ulong[] CreateTable( ulong polynomial ) {
            var createTable = new UInt64[256];
            for ( var i = 0; i < 256; ++i ) {
                var entry = ( UInt64 ) i;
                for ( var j = 0; j < 8; ++j ) {
                    if ( ( entry & 1 ) == 1 ) {
                        entry = ( entry >> 1 ) ^ polynomial;
                    }
                    else {
                        entry = entry >> 1;
                    }
                }
                createTable[ i ] = entry;
            }
            return createTable;
        }
    }
}
