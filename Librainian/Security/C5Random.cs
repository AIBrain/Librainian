// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "C5Random.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "C5Random.cs" was last formatted by Protiguous on 2018/07/13 at 1:37 AM.

namespace Librainian.Security
{

    using JetBrains.Annotations;
    using System;

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
    public sealed class C5Random : Random, IDisposable
    {

        private readonly UInt32[] _q = new UInt32[16];

        private UInt32 _c = 362436;

        private UInt32 _i = 15;

        /// <summary>Create a random number generator seed by system time.</summary>
        public C5Random() : this(seed: DateTime.Now.Ticks) { }

        /// <summary>Create a random number generator with a given seed</summary>
        /// <exception cref="ArgumentException">If seed is zero</exception>
        /// <param name="seed">The seed</param>
        public C5Random(Int64 seed)
        {
            if (seed == 0) { throw new ArgumentException("Seed must be non-zero"); }

            var j = (UInt32)(seed & 0xFFFFFFFF);

            for (var i = 0; i < 16; i++)
            {
                j ^= j << 13;
                j ^= j >> 17;
                j ^= j << 5;
                this._q[i] = j;
            }

            this._q[15] = (UInt32)(seed ^ (seed >> 32));
        }

        /// <summary>Create a random number generator with a specified internal start state.</summary>
        /// <exception cref="ArgumentException">If Q is not of length exactly 16</exception>
        /// <param name="q">
        ///     The start state. Must be a collection of random bits given by an array of exactly 16 uints.
        /// </param>
        public C5Random([NotNull] UInt32[] q)
        {
            if (q == null) { throw new ArgumentNullException(nameof(q)); }

            if (q.Length != 16) { throw new ArgumentException("Q must have length 16, was " + q.Length); }

            Buffer.BlockCopy(q, 0, this._q, 0, q.Length);
        }

        private UInt32 Cmwc()
        {
            const UInt64 a = 487198574UL;
            const UInt32 r = 0xfffffffe;

            this._i = (this._i + 1) & 15;
            var t = a * this._q[this._i] + this._c;
            this._c = (UInt32)(t >> 32);
            var x = (UInt32)(t + this._c);

            if (x >= this._c) { return this._q[this._i] = r - x; }

            x++;
            this._c++;

            return this._q[this._i] = r - x;
        }

        /// <summary>Get a new random System.Double value</summary>
        /// <returns>The random Double</returns>
        protected override Double Sample() => this.NextDouble();

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() { }

        /// <summary>Get a new random System.Int32 value</summary>
        /// <returns>The random int</returns>
        public override Int32 Next() => (Int32)this.Cmwc();

        /// <summary>Get a random integer between two given bounds</summary>
        /// <exception cref="ArgumentException">If max is less than min</exception>
        /// <param name="min">The lower bound (inclusive)</param>
        /// <param name="max">The upper bound (exclusive)</param>
        /// <returns></returns>
        public override Int32 Next(Int32 min, Int32 max)
        {
            if (min > max) { throw new ArgumentException("min must be less than or equal to max"); }

            return min + (Int32)(this.Cmwc() / 4294967296.0 * (max - min));
        }

        /// <summary>Get a random non-negative integer less than a given upper bound</summary>
        /// <exception cref="ArgumentException">If max is negative</exception>
        /// <param name="max">The upper bound (exclusive)</param>
        /// <returns></returns>
        public override Int32 Next(Int32 max)
        {
            if (max < 0) { throw new ArgumentException("max must be non-negative"); }

            return (Int32)(this.Cmwc() / 4294967296.0 * max);
        }

        /// <summary>Fill a array of byte with random bytes</summary>
        /// <param name="buffer">The array to fill</param>
        public override void NextBytes(Byte[] buffer)
        {
            if (buffer == null) { throw new ArgumentNullException(nameof(buffer)); }

            for (Int32 i = 0, length = buffer.Length; i < length; i++) { buffer[i] = (Byte)this.Cmwc(); }
        }

        /// <summary>Get a new random System.Double value</summary>
        /// <returns>The random Double</returns>
        public override Double NextDouble() => this.Cmwc() / 4294967296.0;
    }
}