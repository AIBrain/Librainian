namespace Librainian.Threading {
    using System;

    /// <summary>
    /// Mersenne Twister random number generator; from http://takel.jp/mt/MersenneTwister.cs
    /// </summary>
    class MersenneTwister : Random {
        /* Period parameters */
        private const int N = 624;
        private const int M = 397;
        private const uint MatrixA = 0x9908b0df; /* constant vector a */
        private const uint UpperMask = 0x80000000; /* most significant w-r bits */
        private const uint LowerMask = 0x7fffffff; /* least significant r bits */

        /* Tempering parameters */
        private const uint TemperingMaskB = 0x9d2c5680;
        private const uint TemperingMaskC = 0xefc60000;

        private static uint TEMPERING_SHIFT_U( uint y ) {
            return ( y >> 11 );
        }
        private static uint TEMPERING_SHIFT_S( uint y ) {
            return ( y << 7 );
        }
        private static uint TEMPERING_SHIFT_T( uint y ) {
            return ( y << 15 );
        }
        private static uint TEMPERING_SHIFT_L( uint y ) {
            return ( y >> 18 );
        }

        private readonly uint[] _mt = new uint[ N ]; /* the array for the state vector  */

        private short _mti;

        private static readonly uint[] Mag01 = { 0x0, MatrixA };

        /* initializing the array with a NONZERO seed */
        public MersenneTwister( uint seed ) {
            /* setting initial seeds to mt[N] using         */
            /* the generator Line 25 of Table 1 in          */
            /* [KNUTH 1981, The Art of Computer Programming */
            /*    Vol. 2 (2nd Ed.), pp102]                  */
            this._mt[ 0 ] = seed & 0xffffffffU;
            for ( this._mti = 1; this._mti < N; ++this._mti ) {
                this._mt[ this._mti ] = ( 69069 * this._mt[ this._mti - 1 ] ) & 0xffffffffU;
            }
        }

        /// <summary>
        ///  a default initial seed is used 
        /// </summary>
        public MersenneTwister()
            : this( 4357 ) {
        }

        protected uint GenerateUInt() {
            uint y;

            /* mag01[x] = x * MATRIX_A  for x=0,1 */
            if ( this._mti >= N ) /* generate N words at one time */ {
                short kk = 0;

                for ( ; kk < N - M; ++kk ) {
                    y = ( this._mt[ kk ] & UpperMask ) | ( this._mt[ kk + 1 ] & LowerMask );
                    this._mt[ kk ] = this._mt[ kk + M ] ^ ( y >> 1 ) ^ Mag01[ y & 0x1 ];
                }

                for ( ; kk < N - 1; ++kk ) {
                    y = ( this._mt[ kk ] & UpperMask ) | ( this._mt[ kk + 1 ] & LowerMask );
                    this._mt[ kk ] = this._mt[ kk + ( M - N ) ] ^ ( y >> 1 ) ^ Mag01[ y & 0x1 ];
                }

                y = ( this._mt[ N - 1 ] & UpperMask ) | ( this._mt[ 0 ] & LowerMask );
                this._mt[ N - 1 ] = this._mt[ M - 1 ] ^ ( y >> 1 ) ^ Mag01[ y & 0x1 ];

                this._mti = 0;
            }

            y = this._mt[ this._mti++ ];
            y ^= TEMPERING_SHIFT_U( y );
            y ^= TEMPERING_SHIFT_S( y ) & TemperingMaskB;
            y ^= TEMPERING_SHIFT_T( y ) & TemperingMaskC;
            y ^= TEMPERING_SHIFT_L( y );

            return y;
        }

        public virtual uint NextUInt() {
            return this.GenerateUInt();
        }

        public virtual uint NextUInt( uint maxValue ) {
            return ( uint )( this.GenerateUInt() / ( ( double )uint.MaxValue / maxValue ) );
        }

        public virtual uint NextUInt( uint minValue, uint maxValue ) /* throws ArgumentOutOfRangeException */
        {
            if ( minValue >= maxValue ) {
                throw new ArgumentOutOfRangeException();
            }

            return ( uint )( this.GenerateUInt() / ( ( double )uint.MaxValue / ( maxValue - minValue ) ) + minValue );
        }

        public override int Next() {
            return this.Next( int.MaxValue );
        }

        public override int Next( int maxValue ) /* throws ArgumentOutOfRangeException */
        {
            if ( maxValue > 1 ) {
                return ( int ) ( this.NextDouble()*maxValue );
            }
            if ( maxValue < 0 ) {
                throw new ArgumentOutOfRangeException();
            }

            return 0;
        }

        public override int Next( int minValue, int maxValue ) {
            if ( maxValue < minValue ) {
                throw new ArgumentOutOfRangeException();
            }
            if ( maxValue == minValue ) {
                return minValue;
            }
            return this.Next( maxValue - minValue ) + minValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void NextBytes( byte[] buffer ) /* throws ArgumentNullException*/
        {
            if ( buffer == null ) {
                throw new ArgumentNullException();
            }

            var bufLen = buffer.Length;

            for ( var idx = 0; idx < bufLen; ++idx ) {
                buffer[ idx ] = ( byte )this.Next( 256 );
            }
        }

        public override double NextDouble() {
            return ( double )this.GenerateUInt() / ( ( ulong )uint.MaxValue + 1 );
        }
    }
}
