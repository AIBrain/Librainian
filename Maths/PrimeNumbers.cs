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
// "Librainian/PrimeNumbers.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Maths {
    using System.Collections.Generic;
    using System.Linq;

    public class PrimeNumbers {
        public static readonly HashSet< int > MemoizedPrimes = new HashSet< int >();

        public static IEnumerable< int > Primes() {
            //var memoized = new List<int>();
            var sqrt = 1;
            var primes = PotentialPrimes().Where( x => {
                                                      sqrt = GetSqrtCeiling( x, sqrt );
                                                      return MemoizedPrimes.TakeWhile( y => y <= sqrt ).All( y => x%y != 0 );
                                                  } );
            foreach ( var prime in primes ) {
                yield return prime;
                MemoizedPrimes.Add( prime );
            }
        }

        public static IEnumerable< int > PotentialPrimes() {
            yield return 2;
            yield return 3;
            var k = 1;
            loop:
            yield return k*6 - 1;
            yield return k*6 + 1;
            k++;
            goto loop;
            // ReSharper disable FunctionNeverReturns
        }

        private static int GetSqrtCeiling( int value, int start ) {
            while ( start*start < value ) {
                start++;
            }
            return start;
        }

        /// <summary>
        ///     Untested. Should return a list of prime numbers between <paramref name="lowEnd" /> and <paramref name="highEnd" />
        /// </summary>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static IEnumerable< int > Primes( int lowEnd, int highEnd ) {
            //var memoized = new HashSet<int>(); //TODO move this over to a static variable?
            var sqrt = 1;
            var primes = PotentialPrimes( lowEnd, highEnd ).Where( x => {
                                                                       sqrt = GetSqrtCeiling( x, sqrt );
                                                                       return MemoizedPrimes.TakeWhile( y => y <= sqrt ).All( y => x%y != 0 );
                                                                   } );
            foreach ( var prime in primes ) {
                yield return prime;
                MemoizedPrimes.Add( prime );
            }
        }

        /// <summary>
        ///     Untested. Returns a list of integers that COULD be prime, not that ARE prime.
        /// </summary>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static IEnumerable< int > PotentialPrimes( int lowEnd, int highEnd ) {
            var k = lowEnd;
            yield return k;
            loop:
            yield return k*6 - 1;
            yield return k*6 + 1;
            k++;
            if ( k >= highEnd ) {
                yield break;
            }
            goto loop;
        }

        // ReSharper restore FunctionNeverReturns
    }
}
