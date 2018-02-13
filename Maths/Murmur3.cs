// Copyright 2018 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Murmur3.cs" was last cleaned by Rick on 2018/02/03 at 1:14 AM

namespace Librainian.Maths {
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     128 bit output, 64 bit platform version
    /// </summary>
    /// <seealso cref="http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html" />
    public class Murmur3 {
        public const UInt64 READ_SIZE = 16;
        private const UInt64 C1 = 0x87c37b91114253d5L;
        private const UInt64 C2 = 0x4cf5ad432745937fL;
        private readonly UInt32 _seed; // if want to start with a seed, create a constructor
        private UInt64 h1;
        private UInt64 h2;

        private UInt64 length;

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public Murmur3( UInt32 seed ) => this._seed = seed;

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
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

            Array.Copy( sourceArray: BitConverter.GetBytes( value: this.h1 ), sourceIndex: 0, destinationArray: hash, destinationIndex: 0, length: 8 );
            Array.Copy( sourceArray: BitConverter.GetBytes( value: this.h2 ), sourceIndex: 0, destinationArray: hash, destinationIndex: 8, length: 8 );

            return hash;
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
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

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        private static UInt64 MixKey1( UInt64 k1 ) {
            k1 *= C1;
            k1 = k1.RotateLeft( bits: 31 );
            k1 *= C2;
            return k1;
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        private static UInt64 MixKey2( UInt64 k2 ) {
            k2 *= C2;
            k2 = k2.RotateLeft( bits: 33 );
            k2 *= C1;
            return k2;
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        private static UInt64 MixFinal( UInt64 k ) {
            // avalanche bits

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
        public Byte[] ComputeHash( Byte[] bb ) {
            this.ProcessBytes( bb: bb );
            return this.GetHash();
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
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
            if ( remaining > 0 ) {
                this.ProcessBytesRemaining( bb: bb, remaining: remaining, pos: pos );
            }
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.AggressiveInlining )]
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
                default:
                    throw new Exception( "Something went wrong with remaining bytes calculation." );
            }

            this.h1 ^= MixKey1( k1: k1 );
            this.h2 ^= MixKey2( k2: k2 );
        }
    }
}