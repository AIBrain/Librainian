// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Crc32.cs",
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
// "Librainian/Librainian/Crc32.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

namespace Librainian.FileSystem {

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    ///     Implements a 32-bit CRC hash algorithm compatible with Zip etc.
    /// </summary>
    /// <remarks>
    ///     Crc32 should only be used for backward compatibility with older file formats and algorithms. It is not secure
    ///     enough for new applications. If you need to call multiple times for the same data either use the
    ///     HashAlgorithm interface or remember that the result of one Compute call needs to be ~ (XOR) before being passed in
    ///     as the seed for the next Compute call.
    /// </remarks>
    /// <copyright>
    ///     Copyright (c) Damien Guard. All rights reserved. Licensed under the Apache License. Originally published at
    ///     http://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net
    /// </copyright>
    public sealed class Crc32 : HashAlgorithm {

        private static UInt32[] _defaultTable;
        private readonly UInt32 _seed;
        private readonly UInt32[] _table;
        private UInt32 _hash;
        public const UInt32 DefaultPolynomial = 3988292384;

        public const UInt32 DefaultSeed = 0xffffffffu;

        public Crc32() : this( DefaultPolynomial, DefaultSeed ) { }

        public Crc32( UInt32 polynomial, UInt32 seed ) {
            this._table = InitializeTable( polynomial );
            this._seed = this._hash = seed;
        }

        public override Int32 HashSize => 32;

        private static UInt32[] InitializeTable( UInt32 polynomial ) {
            if ( polynomial == DefaultPolynomial && _defaultTable != null ) { return _defaultTable; }

            var createTable = new UInt32[256];

            for ( var i = 0; i < 256; i++ ) {
                var entry = ( UInt32 )i;

                for ( var j = 0; j < 8; j++ ) {
                    if ( ( entry & 1 ) == 1 ) { entry = ( entry >> 1 ) ^ polynomial; }
                    else { entry = entry >> 1; }
                }

                createTable[i] = entry;
            }

            if ( polynomial == DefaultPolynomial ) { _defaultTable = createTable; }

            return createTable;
        }

        private static Byte[] UInt32ToBigEndianBytes( UInt32 uint32 ) {
            var result = BitConverter.GetBytes( uint32 );

            if ( BitConverter.IsLittleEndian ) { Array.Reverse( result ); }

            return result;
        }

        protected override void HashCore( Byte[] buffer, Int32 start, Int32 length ) => this._hash = CalculateHash( this._table, this._hash, buffer, start, length );

        protected override Byte[] HashFinal() {
            var hashBuffer = UInt32ToBigEndianBytes( ~this._hash );
            this.HashValue = hashBuffer;

            return hashBuffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="table"> </param>
        /// <param name="seed">  </param>
        /// <param name="buffer"></param>
        /// <param name="start"> </param>
        /// <param name="size">  </param>
        /// <returns></returns>
        // ReSharper disable once SuggestBaseTypeForParameter
        public static UInt32 CalculateHash( UInt32[] table, UInt32 seed, IList<Byte> buffer, Int32 start, Int32 size ) {
            var crc = seed;

            for ( var i = start; i < size - start; i++ ) { crc = ( crc >> 8 ) ^ table[buffer[i] ^ ( crc & 0xff )]; }

            return crc;
        }

        public static UInt32 Compute( Byte[] buffer ) => Compute( DefaultSeed, buffer );

        public static UInt32 Compute( UInt32 seed, Byte[] buffer ) => Compute( DefaultPolynomial, seed, buffer );

        public static UInt32 Compute( UInt32 polynomial, UInt32 seed, Byte[] buffer ) => ~CalculateHash( InitializeTable( polynomial ), seed, buffer, 0, buffer.Length );

        public override void Initialize() => this._hash = this._seed;
    }
}