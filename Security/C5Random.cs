// Copyright 2016 Rick@AIBrain.org.
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
// "Librainian/C5Random.cs" was last cleaned by Rick on 2016/08/06 at 9:41 PM

namespace Librainian.Security {

    using System;
    using System.Threading;
    using JetBrains.Annotations;

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
    ///         A modern random number generator based on G. Marsaglia: Seeds for Random Number Generators,
    ///         Communications of the ACM 46, 5 (May 2003) 90-93; and a posting by Marsaglia to comp.lang.c
    ///         on 2003-04-03.
    ///     </para>
    /// </summary>
    public sealed class C5Random : Random, IDisposable {

        /// <summary>Create a random number generator seed by system time.</summary>
        public C5Random() : this( DateTime.Now.Ticks ) { }

        /// <summary>Create a random number generator with a given seed</summary>
        /// <exception cref="ArgumentException">If seed is zero</exception>
        /// <param name="seed">The seed</param>
        public C5Random( Int64 seed ) {
            if ( seed == 0 ) {
                throw new ArgumentException( "Seed must be non-zero" );
            }

            var j = ( UInt32 )( seed & 0xFFFFFFFF );

            for ( var i = 0; i < 16; i++ ) {
                j ^= j << 13;
                j ^= j >> 17;
                j ^= j << 5;
                this.Q.Value[ i ] = j;
            }

            this.Q.Value[ 15 ] = ( UInt32 )( seed ^ ( seed >> 32 ) );
        }

        /// <summary>Create a random number generator with a specified internal start state.</summary>
        /// <exception cref="ArgumentException">If Q is not of length exactly 16</exception>
        /// <param name="q">
        ///     The start state. Must be a collection of random bits given by an array of exactly 16 uints.
        /// </param>
        public C5Random( [NotNull] UInt32[] q ) {
            if ( q == null ) {
                throw new ArgumentNullException( nameof( q ) );
            }
            if ( q.Length > 16 ) {
                throw new ArgumentException( "Q must have length 16, was " + q.Length );
            }

            Array.Copy( q, this.Q.Value, this.Q.Value.Length );
        }

        private ThreadLocal< UInt32 > C { get; } = new ThreadLocal<UInt32>( () => 362436, false );

        private ThreadLocal<UInt32> I { get; } = new ThreadLocal<UInt32>( () => 15, false );

        private ThreadLocal<UInt32[]> Q { get; } = new ThreadLocal<UInt32[]>( () => new UInt32[ 16 ], false );

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<Q>k__BackingField" )]
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<I>k__BackingField" )]
        [System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "<C>k__BackingField" )]
        public void Dispose() {
            this.C.Dispose();
            this.I.Dispose();
            this.Q.Dispose();
        }

        /// <summary>Get a new random System.Int32 value</summary>
        /// <returns>The random int</returns>
        public override Int32 Next() => ( Int32 )this.Cmwc();

        /// <summary>Get a random integer between two given bounds</summary>
        /// <exception cref="ArgumentException">If max is less than min</exception>
        /// <param name="min">The lower bound (inclusive)</param>
        /// <param name="max">The upper bound (exclusive)</param>
        /// <returns></returns>
        public override Int32 Next( Int32 min, Int32 max ) {
            if ( min > max ) {
                throw new ArgumentException( "min must be less than or equal to max" );
            }

            return min + ( Int32 )( this.Cmwc() / 4294967296.0 * ( max - min ) );
        }

        /// <summary>Get a random non-negative integer less than a given upper bound</summary>
        /// <exception cref="ArgumentException">If max is negative</exception>
        /// <param name="max">The upper bound (exclusive)</param>
        /// <returns></returns>
        public override Int32 Next( Int32 max ) {
            if ( max < 0 ) {
                throw new ArgumentException( "max must be non-negative" );
            }

            return ( Int32 )( this.Cmwc() / 4294967296.0 * max );
        }

        /// <summary>Fill a array of byte with random bytes</summary>
        /// <param name="buffer">The array to fill</param>
        public override void NextBytes( Byte[] buffer ) {
            if ( buffer == null ) {
                throw new ArgumentNullException( nameof( buffer ) );
            }

            for ( Int32 i = 0, length = buffer.Length; i < length; i++ ) {
                buffer[ i ] = ( Byte )this.Cmwc();
            }
        }

        /// <summary>Get a new random System.Double value</summary>
        /// <returns>The random Double</returns>
        public override Double NextDouble() => this.Cmwc() / 4294967296.0;

        /// <summary>Get a new random System.Double value</summary>
        /// <returns>The random Double</returns>
        protected override Double Sample() => this.NextDouble();

        private UInt32 Cmwc() {
            const UInt64 a = 487198574UL;
            const UInt32 r = 0xfffffffe;

            this.I.Value = ( this.I.Value + 1 ) & 15;
            var t = a * this.Q.Value[ this.I.Value ] + this.C.Value;
            this.C.Value = ( UInt32 )( t >> 32 );
            var x = ( UInt32 )( t + this.C.Value );
            if ( x >= this.C.Value ) {
                return this.Q.Value[ this.I.Value ] = r - x;
            }

            x++;
            this.C.Value++;

            return this.Q.Value[ this.I.Value ] = r - x;
        }
    }
}
