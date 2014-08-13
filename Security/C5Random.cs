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
// "Librainian/C5Random.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Security {
    using System;
    using System.Threading;
    using Annotations;

    /*
        Copyright (c) 2003-2006 Niels Kokholm and Peter Sestoft
        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in
        all copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
*/

    /// <summary>
    ///     <para>
    ///         A modern random number generator based on G. Marsaglia:
    ///         Seeds for Random Number Generators, Communications of the
    ///         ACM 46, 5 (May 2003) 90-93; and a posting by Marsaglia to
    ///         comp.lang.c on 2003-04-03.
    ///     </para>
    /// </summary>
    /// <remarks>Modified by Rick to be threadsafe. I hope.</remarks>
    [UsedImplicitly]
    public class C5Random : Random {
        private readonly ThreadLocal< uint > _c = new ThreadLocal< uint >( () => 362436, false );
        private readonly ThreadLocal< uint > _i = new ThreadLocal< uint >( () => 15, false );
        private readonly ThreadLocal< uint[] > _q = new ThreadLocal< uint[] >( () => new uint[16], false );

        /// <summary>
        ///     Create a random number generator seed by system time.
        /// </summary>
        public C5Random() : this( DateTime.Now.Ticks ) { }

        /// <summary>
        ///     Create a random number generator with a given seed
        /// </summary>
        /// <exception cref="ArgumentException">If seed is zero</exception>
        /// <param name="seed">The seed</param>
        public C5Random( long seed ) {
            if ( seed == 0 ) {
                throw new ArgumentException( "Seed must be non-zero" );
            }

            var j = ( uint ) ( seed & 0xFFFFFFFF );

            for ( var i = 0; i < 16; i++ ) {
                j ^= j << 13;
                j ^= j >> 17;
                j ^= j << 5;
                this._q.Value[ i ] = j;
            }

            this._q.Value[ 15 ] = ( uint ) ( seed ^ ( seed >> 32 ) );
        }

        /// <summary>
        ///     Create a random number generator with a specified internal start state.
        /// </summary>
        /// <exception cref="ArgumentException">If Q is not of length exactly 16</exception>
        /// <param name="q">The start state. Must be a collection of random bits given by an array of exactly 16 uints.</param>
        public C5Random( [NotNull] uint[] q ) {
            if ( q == null ) {
                throw new ArgumentNullException( "q" );
            }
            if ( q.Length > 16 ) {
                throw new ArgumentException( "Q must have length 16, was " + q.Length );
            }
            Array.Copy( q, this._q.Value, this._q.Value.Length );
        }

        /// <summary>
        ///     Get a new random System.Double value
        /// </summary>
        /// <returns>The random Double</returns>
        protected override Double Sample() {
            return this.NextDouble();
        }

        /// <summary>
        ///     Get a new random System.Double value
        /// </summary>
        /// <returns>The random Double</returns>
        public override Double NextDouble() {
            return this.Cmwc()/4294967296.0;
        }

        private uint Cmwc() {
            const ulong a = 487198574UL;
            const uint r = 0xfffffffe;

            this._i.Value = ( this._i.Value + 1 ) & 15;
            var t = a*this._q.Value[ this._i.Value ] + this._c.Value;
            this._c.Value = ( uint ) ( t >> 32 );
            var x = ( uint ) ( t + this._c.Value );
            if ( x >= this._c.Value ) {
                return this._q.Value[ this._i.Value ] = r - x;
            }
            x++;
            this._c.Value++;

            return this._q.Value[ this._i.Value ] = r - x;
        }

        /// <summary>
        ///     Get a new random System.Int32 value
        /// </summary>
        /// <returns>The random int</returns>
        public override int Next() {
            return ( int ) this.Cmwc();
        }

        /// <summary>
        ///     Get a random integer between two given bounds
        /// </summary>
        /// <exception cref="ArgumentException">If max is less than min</exception>
        /// <param name="min">The lower bound (inclusive)</param>
        /// <param name="max">The upper bound (exclusive)</param>
        /// <returns></returns>
        public override int Next( int min, int max ) {
            if ( min > max ) {
                throw new ArgumentException( "min must be less than or equal to max" );
            }

            return min + ( int ) ( this.Cmwc()/4294967296.0*( max - min ) );
        }

        /// <summary>
        ///     Get a random non-negative integer less than a given upper bound
        /// </summary>
        /// <exception cref="ArgumentException">If max is negative</exception>
        /// <param name="max">The upper bound (exclusive)</param>
        /// <returns></returns>
        public override int Next( int max ) {
            if ( max < 0 ) {
                throw new ArgumentException( "max must be non-negative" );
            }

            return ( int ) ( this.Cmwc()/4294967296.0*max );
        }

        /// <summary>
        ///     Fill a array of byte with random bytes
        /// </summary>
        /// <param name="buffer">The array to fill</param>
        public override void NextBytes( [NotNull] byte[] buffer ) {
            if ( buffer == null ) {
                throw new ArgumentNullException( "buffer" );
            }
            for ( int i = 0, length = buffer.Length; i < length; i++ ) {
                buffer[ i ] = ( byte ) this.Cmwc();
            }
        }
    }
}
