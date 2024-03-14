// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "SetsOfSets.cs" last formatted on 2021-11-30 at 7:17 PM by Protiguous.

#nullable enable

namespace Librainian.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public static class SetsOfSets {

	/// <summary>Merge two sets, order a, then order b. No distinct is done. No nulls are removed.</summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static IEnumerable<T?> Append<T>( this IEnumerable<T?>? a, IEnumerable<T?>? b ) {
		if ( a is not null ) {
			foreach ( var item in a ) {
				yield return item;
			}
		}

		if ( b is not null ) {
			foreach ( var item in b ) {
				yield return item;
			}
		}
	}

	/// <summary>add the new item <paramref name="a" /> at the beginning of the collection <paramref name="b" /></summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static IEnumerable<T?> Append<T>( this T? a, IEnumerable<T?>? b ) {
		if ( a is not null ) {
			yield return a;
		}

		if ( b is not null ) {
			foreach ( var item in b ) {
				yield return item;
			}
		}
	}

	/// <summary>add the collection <paramref name="a" /> at the beginning of the new item <paramref name="b" /></summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public static IEnumerable<T?> Append<T>( this IEnumerable<T?>? a, T? b ) {
		if ( a is not null ) {
			foreach ( var item in a ) {
				yield return item;
			}
		}

		yield return b;
	}

	public static IEnumerable<IEnumerable<T>?> CartesianProduct<T>( this IEnumerable<IEnumerable<T>?>? sequences ) {
		IEnumerable<IEnumerable<T>> emptyProduct = new[] {
			Enumerable.Empty<T>()
		};
		if ( sequences is null ) {
			return emptyProduct;
		}

		return sequences.Aggregate( emptyProduct, ( accumulator, sequence ) => {
			var enumerable = sequence as IList<T> ?? sequence?.ToList() ?? Enumerable.Empty<T>();

			return from accseq in accumulator
				   from item in enumerable
				   where !accseq.Contains( item )
				   select accseq.Concat( new[] {
					   item
				   } );
		} );
	}

	public static IEnumerable<IEnumerable<T>?>? Combinations<T>( params IEnumerable<T?>?[]? input ) {
		IEnumerable<IEnumerable<T>> result = Array.Empty<T[]>();

		//TODO Don't know what to do with nulls in Combinations()..
		return input?.Aggregate( result, ( current, item ) => current.Combine( item ) );
	}

	public static IEnumerable<IEnumerable<T?>?> Combine<T>( this IEnumerable<IEnumerable<T?>?> setA, IEnumerable<IEnumerable<T?>?>? setB ) {
		var found = false;

		if ( setB is null ) {
			yield break;
		}

		var eachB = setB as IEnumerable<T>[] ?? setB.ToArray();

		foreach ( var a in setA ) {
			if ( a is not null ) {
				var subA = a.ToList();

				found = true;

				foreach ( var b in eachB ) {
					if ( b is not null ) {
						yield return subA.Append( b );
					}
				}
			}
		}

		if ( found ) {
			yield break;
		}

		foreach ( var b in eachB ) {
			if ( b is not null ) {
				yield return b;
			}
		}
	}

	public static IEnumerable<IEnumerable<T?>?>? Combine<T>( this IEnumerable<T?>? setA, IEnumerable<IEnumerable<T?>?>? setB ) {
		var found = false;

		if ( setB != null ) {
			foreach ( var b in setB ) {
				found = true;

				if ( setA is not null ) {

					// ReSharper disable once PossibleMultipleEnumeration
					if ( b != null ) {
						yield return setA.Append( b );
					}
				}
			}
		}

		if ( !found ) {
			if ( setA is not null ) {

				// ReSharper disable once PossibleMultipleEnumeration
				yield return setA;
			}
		}
	}

	public static IEnumerable<IEnumerable<T?>?> Combine<T>( this IEnumerable<IEnumerable<T?>?> setA, IEnumerable<T?> setB ) {
		var found = false;

		var b = setB.ToList();

		foreach ( var a in setA ) {
			found = true;
			yield return a.Append( b );
		}

		if ( !found ) {
			yield return b;
		}
	}

	public static IEnumerable<IEnumerable<T?>?>? Combine<T>( [DisallowNull] this T a, IEnumerable<IEnumerable<T>> setB ) {
		var found = false;

		foreach ( var bGroup in setB ) {
			found = true;

			yield return a.Append( bGroup );
		}

		if ( !found ) {
			yield return new[] {
				a
			};
		}
	}

	public static IEnumerable<IEnumerable<T?>>? Combine<T>( this IEnumerable<IEnumerable<T>> setA, [DisallowNull] T b ) {
		var found = false;

		foreach ( var aGroup in setA ) {
			found = true;

			yield return aGroup.Append( b );
		}

		if ( !found ) {
			yield return new[] {
				b
			};
		}
	}

	public static IEnumerable<T?> Group<T>( this T? a, T? b ) {
		yield return a;
		yield return b;
	}

	/// <summary>
	/// <para>
	/// The power set of a set <paramref name="s" /> is the set of all subsets of <paramref name="s" />, including the empty
	/// set and <paramref name="s" /> itself.
	/// </para>
	/// <para>(Return a jagged array of every possible combination of the <paramref name="s" />.)</para>
	/// <para>Note: Does not exclude duplicates.</para>
	/// </summary>
	public static T[][] PowerSet<T>( this T[] s ) {
		var arrayLength = s.Length;

		var powerSet = new T[ 1 << arrayLength ][];
		powerSet[ 0 ] = Array.Empty<T>();

		for ( var i = 0; i < arrayLength; i++ ) {
			var current = s[ i ];
			var count = 1 << i;

			for ( var j = 0; j < count; j++ ) {
				var source = powerSet[ j ];

				var sourceLength = source.Length;

				var destination = powerSet[ count + j ] = new T[ sourceLength + 1 ];

				for ( var q = 0; q < sourceLength; q++ ) {
					destination[ q ] = source[ q ];
				}

				destination[ sourceLength ] = current;
			}
		}

		return powerSet;
	}

	/// <summary>
	/// <para>
	/// The power set of a set <paramref name="s" /> is the set of all subsets of <paramref name="s" />, including the empty
	/// set and <paramref name="s" /> itself.
	/// </para>
	/// <para>(Return a jagged array of every possible combination of the <paramref name="s" />.)</para>
	/// <para>Note: Does not exclude duplicates.</para>
	/// </summary>
	public static T[][] PowerSet<T>( this IEnumerable<T> list ) {
		var enumerable = list as T[] ?? list.ToArray();
		var length = enumerable.Length;

		var powerSet = new T[ 1 << length ][];
		powerSet[ 0 ] = Array.Empty<T>();

		for ( var i = 0; i < length; i++ ) {
			var current = enumerable[ i ];
			var count = 1 << i;

			for ( var j = 0; j < count; j++ ) {
				var source = powerSet[ j ];

				var sourceLength = source.Length;

				var destination = powerSet[ count + j ] = new T[ sourceLength + 1 ];

				for ( var q = 0; q < sourceLength; q++ ) {
					destination[ q ] = source[ q ];
				}

				destination[ sourceLength ] = current;
			}
		}

		return powerSet;
	}
}