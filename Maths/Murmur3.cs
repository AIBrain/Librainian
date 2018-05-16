// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Murmur3.cs",
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
// "Librainian/Librainian/Murmur3.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths {

    using System;

    /// <summary>
    ///     128 bit output, 64 bit platform version
    /// </summary>
    /// <seealso cref="http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html" />
    public class Murmur3 {

        private const UInt64 C1 = 0x87c37b91114253d5L;
        private const UInt64 C2 = 0x4cf5ad432745937fL;
        private readonly UInt32 _seed;
        private UInt64 h1;

        // if want to start with a seed, create a constructor
        private UInt64 h2;

        private UInt64 length;
        public const UInt64 READ_SIZE = 16;

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
            k1 = k1.RotateLeft( bits: 31 );
            k1 *= C2;

            return k1;
        }

        private static UInt64 MixKey2( UInt64 k2 ) {
            k2 *= C2;
            k2 = k2.RotateLeft( bits: 33 );
            k2 *= C1;

            return k2;
        }

        private void MixBody( UInt64 k1, UInt64 k2 ) {
            this.h1 ^= MixKey1( k1: k1 );

            this.h1 = this.h1.RotateLeft( bits: 27 );
            this.h1 += this.h2;
            this.h1 = this.h1 * 5 + 0x52dce729;

            this.h2 ^= MixKey2( k2: k2 );

            this.h2 = this.h2.RotateLeft( bits: 31 );
            this.h2 += this.h1;
            this.h2 = this.h2 * 5 + 0x38495ab5;
        }

        private void ProcessBytes( Byte[] bb ) {
            this.h1 = this._seed;
            this.length = 0L;

            var pos = 0;
            var remaining = ( UInt64 )bb.Length;

            // read 128 bits, 16 bytes, 2 longs in eacy cycle
            while ( remaining >= READ_SIZE ) {
                var k1 = bb.ToUInt64( pos: pos );
                pos += 8;

                var k2 = bb.ToUInt64( pos: pos );
                pos += 8;

                this.length += READ_SIZE;
                remaining -= READ_SIZE;

                this.MixBody( k1: k1, k2: k2 );
            }

            // if the input MOD 16 != 0
            if ( remaining > 0 ) { this.ProcessBytesRemaining( bb: bb, remaining: remaining, pos: pos ); }
        }

        private void ProcessBytesRemaining( Byte[] bb, UInt64 remaining, Int32 pos ) {
            UInt64 k1 = 0;
            UInt64 k2 = 0;
            this.length += remaining;

            // little endian (x86) processing
            switch ( remaining ) {
                case 15:
                    k2 ^= ( UInt64 )bb[pos + 14] << 48; // fall through
                    goto case 14;
                case 14:
                    k2 ^= ( UInt64 )bb[pos + 13] << 40; // fall through
                    goto case 13;
                case 13:
                    k2 ^= ( UInt64 )bb[pos + 12] << 32; // fall through
                    goto case 12;
                case 12:
                    k2 ^= ( UInt64 )bb[pos + 11] << 24; // fall through
                    goto case 11;
                case 11:
                    k2 ^= ( UInt64 )bb[pos + 10] << 16; // fall through
                    goto case 10;
                case 10:
                    k2 ^= ( UInt64 )bb[pos + 9] << 8; // fall through
                    goto case 9;
                case 9:
                    k2 ^= bb[pos + 8]; // fall through
                    goto case 8;
                case 8:
                    k1 ^= bb.ToUInt64( pos: pos );

                    break;

                case 7:
                    k1 ^= ( UInt64 )bb[pos + 6] << 48; // fall through
                    goto case 6;
                case 6:
                    k1 ^= ( UInt64 )bb[pos + 5] << 40; // fall through
                    goto case 5;
                case 5:
                    k1 ^= ( UInt64 )bb[pos + 4] << 32; // fall through
                    goto case 4;
                case 4:
                    k1 ^= ( UInt64 )bb[pos + 3] << 24; // fall through
                    goto case 3;
                case 3:
                    k1 ^= ( UInt64 )bb[pos + 2] << 16; // fall through
                    goto case 2;
                case 2:
                    k1 ^= ( UInt64 )bb[pos + 1] << 8; // fall through
                    goto case 1;
                case 1:
                    k1 ^= bb[pos]; // fall through

                    break;

                default: throw new Exception( "Something went wrong with remaining bytes calculation." );
            }

            this.h1 ^= MixKey1( k1: k1 );
            this.h2 ^= MixKey2( k2: k2 );
        }

        public Byte[] ComputeHash( Byte[] bb ) {
            this.ProcessBytes( bb: bb );

            return this.GetHash();
        }

        public Byte[] GetHash() {
            this.h1 ^= this.length;
            this.h2 ^= this.length;

            this.h1 += this.h2;
            this.h2 += this.h1;

            this.h1 = MixFinal( k: this.h1 );
            this.h2 = MixFinal( k: this.h2 );

            this.h1 += this.h2;
            this.h2 += this.h1;

            var hash = new Byte[READ_SIZE];

            Array.Copy( sourceArray: BitConverter.GetBytes( this.h1 ), sourceIndex: 0, destinationArray: hash, destinationIndex: 0, 8 );
            Array.Copy( sourceArray: BitConverter.GetBytes( this.h2 ), sourceIndex: 0, destinationArray: hash, destinationIndex: 8, 8 );

            return hash;
        }
    }
}