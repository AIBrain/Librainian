// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MersenneTwister.cs" was last cleaned by Rick on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;

    /// <summary>Mersenne Twister random number generator; from http://takel.jp/mt/MersenneTwister.cs</summary>
    internal class MersenneTwister : Random {
        /* Period parameters */
        private const UInt32 LowerMask = 0x7fffffff;
        private const Int32 M = 397;
        private const UInt32 MatrixA = 0x9908b0df;

        private const Int32 N = 624;

        /* constant vector a */
        private const UInt32 TemperingMaskB = 0x9d2c5680;
        private const UInt32 TemperingMaskC = 0xefc60000;

        private const UInt32 UpperMask = 0x80000000; /* most significant w-r bits */

        /* least significant r bits */
        /* Tempering parameters */
        private static readonly UInt32[] Mag01 = { 0x0, MatrixA };
        private readonly UInt32[] _mt = new UInt32[N]; /* the array for the state vector  */

        private Int16 _mti;
        /* initializing the array with a NONZERO seed */

        public MersenneTwister( UInt32 seed ) {
            /* setting initial seeds to mt[N] using         */
            /* the generator Line 25 of Table 1 in          */
            /* [KNUTH 1981, The Art of Computer Programming */
            /*    Vol. 2 (2nd Ed.), pp102]                  */
            this._mt[0] = seed & 0xffffffffU;
            for ( this._mti = 1; this._mti < N; ++this._mti ) {
                this._mt[this._mti] = ( 69069 * this._mt[this._mti - 1] ) & 0xffffffffU;
            }
        }

        /// <summary>a default initial seed is used</summary>
        public MersenneTwister() : this( seed: 4357 ) { }

        public override Int32 Next() => this.Next( maxValue: Int32.MaxValue );

        public override Int32 Next( Int32 maxValue ) /* throws ArgumentOutOfRangeException */ {
            if ( maxValue > 1 ) {
                return ( Int32 )( this.NextDouble() * maxValue );
            }

            if ( maxValue < 0 ) {
                throw new ArgumentOutOfRangeException();
            }

            return 0;
        }

        public override Int32 Next( Int32 minValue, Int32 maxValue ) {
            if ( maxValue < minValue ) {
                throw new ArgumentOutOfRangeException();
            }

            if ( maxValue == minValue ) {
                return minValue;
            }

            return this.Next( maxValue: maxValue - minValue ) + minValue;
        }

        /// <summary></summary>
        /// <param name="buffer"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void NextBytes( Byte[] buffer ) /* throws ArgumentNullException*/ {
            if ( buffer is null ) {
                throw new ArgumentNullException();
            }

            var bufLen = buffer.Length;

            for ( var idx = 0; idx < bufLen; ++idx ) {
                buffer[idx] = ( Byte )this.Next( maxValue: 256 );
            }
        }

        public override Double NextDouble() => ( Double )this.GenerateUInt() / ( ( UInt64 )UInt32.MaxValue + 1 );

        public virtual UInt32 NextUInt() => this.GenerateUInt();

        public virtual UInt32 NextUInt( UInt32 maxValue ) => ( UInt32 )( this.GenerateUInt() / ( ( Double )UInt32.MaxValue / maxValue ) );

        public virtual UInt32 NextUInt( UInt32 minValue, UInt32 maxValue ) /* throws ArgumentOutOfRangeException */ {
            if ( minValue >= maxValue ) {
                throw new ArgumentOutOfRangeException();
            }

            return ( UInt32 )( this.GenerateUInt() / ( ( Double )UInt32.MaxValue / ( maxValue - minValue ) ) + minValue );
        }

        protected UInt32 GenerateUInt() {
            UInt32 y;

            /* mag01[x] = x * MATRIX_A  for x=0,1 */
            if ( this._mti >= N ) /* generate N words at one time */ {
                Int16 kk = 0;

                for ( ; kk < N - M; ++kk ) {
                    y = ( this._mt[kk] & UpperMask ) | ( this._mt[kk + 1] & LowerMask );
                    this._mt[kk] = this._mt[kk + M] ^ ( y >> 1 ) ^ Mag01[y & 0x1];
                }

                for ( ; kk < N - 1; ++kk ) {
                    y = ( this._mt[kk] & UpperMask ) | ( this._mt[kk + 1] & LowerMask );
                    this._mt[kk] = this._mt[kk + ( M - N )] ^ ( y >> 1 ) ^ Mag01[y & 0x1];
                }

                y = ( this._mt[N - 1] & UpperMask ) | ( this._mt[0] & LowerMask );
                this._mt[N - 1] = this._mt[M - 1] ^ ( y >> 1 ) ^ Mag01[y & 0x1];

                this._mti = 0;
            }

            y = this._mt[this._mti++];
            y ^= TEMPERING_SHIFT_U( y: y );
            y ^= TEMPERING_SHIFT_S( y: y ) & TemperingMaskB;
            y ^= TEMPERING_SHIFT_T( y: y ) & TemperingMaskC;
            y ^= TEMPERING_SHIFT_L( y: y );

            return y;
        }

        private static UInt32 TEMPERING_SHIFT_L( UInt32 y ) => y >> 18;

        private static UInt32 TEMPERING_SHIFT_S( UInt32 y ) => y << 7;

        private static UInt32 TEMPERING_SHIFT_T( UInt32 y ) => y << 15;

        private static UInt32 TEMPERING_SHIFT_U( UInt32 y ) => y >> 11;
    }
}