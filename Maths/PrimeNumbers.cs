// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PrimeNumbers.cs",
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
// "Librainian/Librainian/PrimeNumbers.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PrimeNumbers {

        public static readonly HashSet<Int32> MemoizedPrimes = new HashSet<Int32>();

        public static Int64[][] GoldenPrimes = {
            new Int64[] { 1, 1 }, new Int64[] { 41, 59 }, new Int64[] { 2377, 1677 }, new Int64[] { 147299, 187507 }, new Int64[] { 9132313, 5952585 }, new Int64[] { 566201239, 643566407 },
            new[] { 35104476161, 22071637057 }, new[] { 2176477521929, 294289236153 }, new[] { 134941606358731, 88879354792675 }, new[] { 8366379594239857, 7275288500431249 },
            new[] { 518715534842869223, 280042546585394647 }
        };

        private static Int32 GetSqrtCeiling( Int32 value, Int32 start ) {
            while ( start * start < value ) { start++; }

            return start;
        }

        public static IEnumerable<Int32> PotentialPrimes() {
            yield return 2;
            yield return 3;
            var k = 1;
            loop:

            yield return k * 6 - 1;
            yield return k * 6 + 1;
            k++;

            goto loop;

            // ReSharper disable FunctionNeverReturns
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        ///     Untested. Returns a list of integers that COULD be prime, not that ARE prime.
        /// </summary>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static IEnumerable<Int32> PotentialPrimes( Int32 lowEnd, Int32 highEnd ) {
            var k = lowEnd;

            yield return k;
            loop:

            yield return k * 6 - 1;
            yield return k * 6 + 1;
            k++;

            if ( k >= highEnd ) { yield break; }

            goto loop;
        }

        public static IEnumerable<Int32> Primes() {

            //var memoized = new List<int>();
            var sqrt = 1;

            var primes = PotentialPrimes().Where( x => {
                sqrt = GetSqrtCeiling( x, sqrt );

                return MemoizedPrimes.TakeWhile( y => y <= sqrt ).All( y => x % y != 0 );
            } );

            foreach ( var prime in primes ) {
                yield return prime;
                MemoizedPrimes.Add( prime );
            }
        }

        /// <summary>
        ///     Untested. Should return a list of prime numbers between <paramref name="lowEnd" /> and <paramref name="highEnd" />
        /// </summary>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static IEnumerable<Int32> Primes( Int32 lowEnd, Int32 highEnd ) {

            //var memoized = new HashSet<int>(); //TODO move this over to a static variable?
            var sqrt = 1;

            var primes = PotentialPrimes( lowEnd, highEnd ).Where( x => {
                sqrt = GetSqrtCeiling( x, sqrt );

                return MemoizedPrimes.TakeWhile( y => y <= sqrt ).All( y => x % y != 0 );
            } );

            foreach ( var prime in primes ) {
                yield return prime;
                MemoizedPrimes.Add( prime );
            }
        }

        // ReSharper restore FunctionNeverReturns
    }
}