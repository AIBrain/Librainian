// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "PrimeNumbers.cs" last formatted on 2020-08-14 at 8:36 PM.

namespace Librainian.Maths {

	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class PrimeNumbers {

		public static readonly HashSet<Int32> MemoizedPrimes = new();

		public static Int64[][] GoldenPrimes { get; } = {
			new Int64[] {
				1, 1
			},
			new Int64[] {
				41, 59
			},
			new Int64[] {
				2377, 1677
			},
			new Int64[] {
				147299, 187507
			},
			new Int64[] {
				9132313, 5952585
			},
			new Int64[] {
				566201239, 643566407
			},
			new[] {
				35104476161, 22071637057
			},
			new[] {
				2176477521929, 294289236153
			},
			new[] {
				134941606358731, 88879354792675
			},
			new[] {
				8366379594239857, 7275288500431249
			},
			new[] {
				518715534842869223, 280042546585394647
			}
		};

		private static Int32 GetSqrtCeiling( Int32 value, Int32 start ) {
			while ( start * start < value ) {
				start++;
			}

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

			// ReSharper disable once IteratorNeverReturns
		}

		/// <summary>Untested. Returns a list of integers that COULD be prime, not that ARE prime.</summary>
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

			if ( k >= highEnd ) {
				yield break;
			}

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
		///     Untested. Should return a list of prime numbers between <paramref name="lowEnd" /> and
		///     <paramref name="highEnd" />
		/// </summary>
		/// <param name="lowEnd"></param>
		/// <param name="highEnd"></param>
		/// <returns></returns>
		public static IEnumerable<Int32> Primes( Int32 lowEnd, Int32 highEnd ) {
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
	}
}