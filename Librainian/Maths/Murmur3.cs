// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Murmur3.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Murmur3.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace Librainian.Maths {

    using System;
    using JetBrains.Annotations;

    /// <summary>128 bit output, 64 bit platform version</summary>
    /// <see cref="http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html" />
    public class Murmur3 {

        private const UInt64 C1 = 0x87c37b91114253d5L;
        private const UInt64 C2 = 0x4cf5ad432745937fL;
        private readonly UInt32 _seed;

        private UInt64 _h1;

        private UInt64 _h2;

        private UInt64 _length;
        public const UInt64 ReadSize = 16;

        public Murmur3( UInt32 seed ) => this._seed = seed;

        private static UInt64 MixFinal( UInt64 k ) {

            // avalanche bits

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;

            return k;
        }

        private static UInt64 MixKey1( UInt64 k1 ) {
            k1 *= C1;
            k1 = k1.RotateLeft( 31 );
            k1 *= C2;

            return k1;
        }

        private static UInt64 MixKey2( UInt64 k2 ) {
            k2 *= C2;
            k2 = k2.RotateLeft( 33 );
            k2 *= C1;

            return k2;
        }

        private void MixBody( UInt64 k1, UInt64 k2 ) {
            this._h1 ^= MixKey1( k1 );

            this._h1 = this._h1.RotateLeft( 27 );
            this._h1 += this._h2;
            this._h1 = this._h1 * 5 + 0x52dce729;

            this._h2 ^= MixKey2( k2 );

            this._h2 = this._h2.RotateLeft( 31 );
            this._h2 += this._h1;
            this._h2 = this._h2 * 5 + 0x38495ab5;
        }

        private void ProcessBytes( [NotNull] Byte[] bb ) {
            this._h1 = this._seed;
            this._length = 0L;

            var pos = 0;
            var remaining = ( UInt64 )bb.Length;

            // read 128 bits, 16 bytes, 2 longs in eacy cycle
            while ( remaining >= ReadSize ) {
                var k1 = bb.ToUInt64( pos );
                pos += 8;

                var k2 = bb.ToUInt64( pos );
                pos += 8;

                this._length += ReadSize;
                remaining -= ReadSize;

                this.MixBody( k1, k2 );
            }

            // if the input MOD 16 != 0
            if ( remaining > 0 ) {
                this.ProcessBytesRemaining( bb, remaining, pos );
            }
        }

        private void ProcessBytesRemaining( [NotNull] Byte[] bb, UInt64 remaining, Int32 pos ) {
            UInt64 k1 = 0;
            UInt64 k2 = 0;
            this._length += remaining;

            // little endian (x86) processing
            switch ( remaining ) {
                case 15:
                    k2 ^= ( UInt64 )bb[ pos + 14 ] << 48; // fall through
                    goto case 14;

                case 14:
                    k2 ^= ( UInt64 )bb[ pos + 13 ] << 40; // fall through
                    goto case 13;

                case 13:
                    k2 ^= ( UInt64 )bb[ pos + 12 ] << 32; // fall through
                    goto case 12;

                case 12:
                    k2 ^= ( UInt64 )bb[ pos + 11 ] << 24; // fall through
                    goto case 11;

                case 11:
                    k2 ^= ( UInt64 )bb[ pos + 10 ] << 16; // fall through
                    goto case 10;

                case 10:
                    k2 ^= ( UInt64 )bb[ pos + 9 ] << 8; // fall through
                    goto case 9;

                case 9:
                    k2 ^= bb[ pos + 8 ]; // fall through
                    goto case 8;

                case 8:
                    k1 ^= bb.ToUInt64( pos );

                    break;

                case 7:
                    k1 ^= ( UInt64 )bb[ pos + 6 ] << 48; // fall through
                    goto case 6;

                case 6:
                    k1 ^= ( UInt64 )bb[ pos + 5 ] << 40; // fall through
                    goto case 5;

                case 5:
                    k1 ^= ( UInt64 )bb[ pos + 4 ] << 32; // fall through
                    goto case 4;

                case 4:
                    k1 ^= ( UInt64 )bb[ pos + 3 ] << 24; // fall through
                    goto case 3;

                case 3:
                    k1 ^= ( UInt64 )bb[ pos + 2 ] << 16; // fall through
                    goto case 2;

                case 2:
                    k1 ^= ( UInt64 )bb[ pos + 1 ] << 8; // fall through
                    goto case 1;

                case 1:
                    k1 ^= bb[ pos ]; // fall through

                    break;

                default: throw new InvalidOperationException( "Something went wrong with remaining bytes calculation." );
            }

            this._h1 ^= MixKey1( k1 );
            this._h2 ^= MixKey2( k2 );
        }

        [NotNull]
        public Byte[] ComputeHash( [NotNull] Byte[] bb ) {
            this.ProcessBytes( bb );

            return this.GetHash();
        }

        [NotNull]
        public Byte[] GetHash() {
            this._h1 ^= this._length;
            this._h2 ^= this._length;

            this._h1 += this._h2;
            this._h2 += this._h1;

            this._h1 = MixFinal( this._h1 );
            this._h2 = MixFinal( this._h2 );

            this._h1 += this._h2;
            this._h2 += this._h1;

            var hash = new Byte[ ReadSize ];

            Buffer.BlockCopy( BitConverter.GetBytes( this._h1 ), 0, hash, 0, 8 );
            Buffer.BlockCopy( BitConverter.GetBytes( this._h2 ), 0, hash, 8, 8 );

            return hash;
        }
    }
}