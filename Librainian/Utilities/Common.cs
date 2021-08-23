// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
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
// File "Common.cs" last touched on 2021-06-11 at 7:36 AM by Protiguous.

#nullable enable

// ReSharper disable once CheckNamespace
namespace Librainian {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using Exceptions;
	using Measurement;
	using Newtonsoft.Json;
	using Parsing;

	public static class Common {

		public static Encoding DefaultEncoding { get; } = Encoding.Unicode;

		public static String ToKey<T>( this T? value, String className, String methodName, String argumentName ) {
			if ( className is null ) {
				throw new ArgumentEmptyException( nameof( className ) );
			}

			if ( methodName is null ) {
				throw new ArgumentEmptyException( nameof( methodName ) );
			}

			if ( argumentName is null ) {
				throw new ArgumentEmptyException( nameof( argumentName ) );
			}

			//using the reasoning that a string lookup will match sooner by having the most selective "key" first, which would be the value.
			return $"{value}{Symbols.TripleTilde}{className}{Symbols.TripleTilde}{methodName}{Symbols.TripleTilde}{argumentName}";
		}

		public static String ToKey<T>( this T? value, String className, String methodName ) {
			if ( className is null ) {
				throw new ArgumentEmptyException( nameof( className ) );
			}

			if ( methodName is null ) {
				throw new ArgumentEmptyException( nameof( methodName ) );
			}

			//using the reasoning that a string lookup will match sooner by having the most selective "key" first, which would be the value.
			return $"{value}{Symbols.TripleTilde}{methodName}{Symbols.TripleTilde}{className}";
		}

		/// <summary>
		///     Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two inclusive values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <example>5.Between(1, 10)</example>
		/// <example>5.Between(10, 1)</example>
		/// <example>5.Between(10, 6) == false</example>
		/// <example>5.Between(5, 5))</example>
		[Pure]
		public static Boolean Between<T>( this T target, T startInclusive, T endInclusive ) where T : IComparable {
			if ( startInclusive.CompareTo( endInclusive ) is Order.After ) {
				return target.CompareTo( startInclusive ) <= Order.Same && target.CompareTo( endInclusive ) >= Order.Same;
			}

			if ( target.CompareTo( startInclusive ) >= Order.Same ) {
				return target.CompareTo( endInclusive ) <= Order.Same;
			}

			return false;
		}

		/// <summary>
		///     Return true if a value is <see cref="Between{T}" /> two inclusive values.
		/// </summary>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <example>5.Between(1, 10)</example>
		/// <example>5.Between(10, 1)</example>
		/// <example>5.Between(10, 6) == false</example>
		/// <example>5.Between(5, 5))</example>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Between( this Byte target, Byte startInclusive, Byte endInclusive ) => target >= startInclusive && target <= endInclusive;

		/// <summary>
		///     Return true if a value is <see cref="Between{T}" /> two inclusive values.
		/// </summary>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <example>5.Between(1, 10)</example>
		/// <example>5.Between(10, 1)</example>
		/// <example>5.Between(10, 6) == false</example>
		/// <example>5.Between(5, 5))</example>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Between( this Int32 target, Int32 startInclusive, Int32 endInclusive ) => target >= startInclusive && target <= endInclusive;

		/// <summary>
		///     Return true if a value is <see cref="Between{T}" /> two inclusive values.
		/// </summary>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <example>5.Between(1, 10)</example>
		/// <example>5.Between(10, 1)</example>
		/// <example>5.Between(10, 6) == false</example>
		/// <example>5.Between(5, 5))</example>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Between( this Int64 target, Int64 startInclusive, Int64 endInclusive ) => target >= startInclusive && target <= endInclusive;

		/// <summary>
		///     Return true if a value is <see cref="Between{T}" /> two inclusive values.
		/// </summary>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <example>5.Between(1, 10)</example>
		/// <example>5.Between(10, 1)</example>
		/// <example>5.Between(10, 6) == false</example>
		/// <example>5.Between(5, 5))</example>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Between( this UInt64 target, UInt64 startInclusive, UInt64 endInclusive ) => target >= startInclusive && target <= endInclusive;

		/// <summary>
		///     Returns a new <typeparamref name="T" /> that is the value of <paramref name="self" />, constrained between
		///     <paramref name="min" /> and <paramref name="max" />.
		/// </summary>
		/// <param name="self">The extended T.</param>
		/// <param name="min"> The minimum value of the <typeparamref name="T" /> that can be returned.</param>
		/// <param name="max"> The maximum value of the <typeparamref name="T" /> that can be returned.</param>
		/// <returns>The equivalent to: <c>this &lt; min ? min : this &gt; max ? max : this</c>.</returns>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static T Clamp<T>( this T self, T min, T max ) where T : IComparable<T> =>
			self.CompareTo( min ) < 0 ? min :
			self.CompareTo( max ) > 0 ? max : self;

		public static IEnumerable<T?> Concat<T>( this IEnumerable<T> first, T? second ) {
			foreach ( var item in first ) {
				yield return item;
			}

			yield return second;
		}

		public static T[] Concat<T>( this T[] array1, T[] array2 ) {
			var result = new T[ array1.LongLength + array2.LongLength ];
			array1.CopyTo( result, 0 );
			array2.CopyTo( result, array1.LongLength );

			return result;
		}

		public static IEnumerable<T> Concat<T>( this IEnumerable<T> left, IEnumerable<T> right ) {
			foreach ( var a in left ) {
				yield return a;
			}

			foreach ( var b in right ) {
				yield return b;
			}
		}

		public static Boolean IsDevelopmentEnviroment() {
			var devEnvironmentVariable = Environment.GetEnvironmentVariable( "NETCORE_ENVIRONMENT" );

			var isDevelopment = String.IsNullOrEmpty( devEnvironmentVariable ) || devEnvironmentVariable.Like( "development" );
			return isDevelopment;
		}

		[Pure]
		public static UInt64 LengthReal( this String? s ) => s is null ? 0 : ( UInt64 )new StringInfo( s ).LengthInTextElements;

		/// <summary>
		///     Gets a <b>horribly</b> ROUGH guesstimate of the memory consumed by an object by using
		///     <see cref="Newtonsoft.Json.JsonConvert" /> .
		/// </summary>
		/// <param name="bob"></param>
		[Pure]
		public static UInt64 MemoryUsed<T>( [DisallowNull] this T bob ) => JsonConvert.SerializeObject( bob, Formatting.None ).LengthReal();

		/// <summary>
		///     Just a no-op for setting a breakpoint on.
		/// </summary>
		[DebuggerStepThrough]
		[Conditional( "DEBUG" )]
		public static void Nop<T>( this T? _ ) { }

		/// <summary>
		///     Just a no-op for setting a breakpoint on.
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		[Conditional( "DEBUG" )]
		public static void Nop() { }

		/// <summary>
		///     <para>Works like the SQL "nullif" function.</para>
		///     <para>
		///         If <paramref name="left" /> is equal to <paramref name="right" /> then return null for classes or the default
		///         value for value types.
		///     </para>
		///     <para>Otherwise return <paramref name="left" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		[DebuggerStepThrough]
		public static T? NullIf<T>( this T? left, T? right ) where T : class => Comparer<T>.Default.Compare( left, right ) == 0 ? null : left;

		/// <summary>
		///     Swap <paramref name="left" /> with <paramref name="right" />.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		[DebuggerStepThrough]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void SwapNullable<T>( ref T? left, ref T? right ) => ( left, right ) = ( right, left );
		public static void Swap<T>( ref T left, ref T right ) => ( left, right ) = ( right, left );

		/// <summary>
		///     Given (T left, T right), Return (T right, T left).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static (T? right, T? left) Swap<T>( this T? left, T? right ) => ( right, left );

		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static (T? right, T? left) Swap<T>( (T? left, T? right) tuple ) => ( tuple.right, tuple.left );

		/// <summary>
		///     Create only 1 instance of <see cref="T" /> per thread. (Only unique when using this method!)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static class Cache<T> where T : notnull, new() {

			private static ThreadLocal<T> LocalCache { get; } = new(() => new T(), false);

			public static T? Instance { get; } = LocalCache.Value;

		}

		/// <summary>
		///     Only create 1 instance of <see cref="T" /> per all threads. (only unique when using this method!)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static class CacheGlobal<T> where T : notnull, new() {

			public static T Instance { get; } = new();

		}

		/// <summary>Swap the two indexes</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"> </param>
		/// <param name="index1"></param>
		/// <param name="index2"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static void Swap<T>(
			this T[] array,
			Int32 index1,
			Int32 index2
		) {
			if ( array is null ) {
				throw new ArgumentEmptyException( nameof( array ) );
			}

			var length = array.Length;

			if ( index1 < 0 ) {
				throw new OutOfRangeException( $"{nameof( index1 )} cannot be lower than 0." );
			}

			if ( index1 >= length ) {
				throw new OutOfRangeException( $"{nameof( index1 )} cannot be higher than {length - 1}." );
			}

			if ( index2 < 0 ) {
				throw new OutOfRangeException( $"{nameof( index2 )} cannot be lower than 0." );
			}

			if ( index2 >= length ) {
				throw new OutOfRangeException( $"{nameof( index2 )} cannot be higher than {length - 1}." );
			}

			(array[ index1 ], array[ index2 ]) = (array[ index2 ], array[ index1 ]);
		}

		private static void YieldFor( TimeSpan timeSpan ) {
			var stopwatch = Stopwatch.StartNew();
			while ( stopwatch.Elapsed < timeSpan ) {
				Thread.Yield();
			} 
		}

		public static String GetApplicationName( String defaultOtherwise ) => Application.ProductName.Trimmed() ?? defaultOtherwise.Trimmed() ?? throw new NullException(nameof( GetApplicationName));

	}

}