#nullable enable

namespace Librainian {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Text;
	using JetBrains.Annotations;
	using Logging;
	using Newtonsoft.Json;
	using Parsing;

	public static class Common {

		public static Encoding DefaultEncoding { get; } = Encoding.Unicode;

		/// <summary>Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two inclusive values.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target">        </param>
		/// <param name="startInclusive"></param>
		/// <param name="endInclusive">  </param>
		/// <returns></returns>
		/// <example>5. Between(1, 10)</example>
		/// <example>5. Between(10, 1)</example>
		/// <example>5. Between(10, 6) == false</example>
		/// <example>5. Between(5, 5))</example>
		[Pure]
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public static Boolean Between<T>( [NotNull] this T target, [NotNull] T startInclusive, [NotNull] T endInclusive ) where T : IComparable {
			var t2sI = target.CompareTo( startInclusive );

			return startInclusive.CompareTo( endInclusive ) switch {
				1 => t2sI <= 0 && target.CompareTo( endInclusive ) >= 0,
				_ => t2sI >= 0 && target.CompareTo( endInclusive ) <= 0
			};
		}

		[DebuggerStepThrough]
		[Conditional( "DEBUG" )]
		public static void BreakIfDebug<T>( [CanBeNull] this T _, [CanBeNull] String? breakReason = null ) {
			if ( !Debugger.IsAttached ) {
				return;
			}

			if ( breakReason != null ) {
				Debug.WriteLine( $"Break reason: {breakReason}" );
			}

			Debugger.Break();
		}

		/// <summary>
		///     Returns a new <typeparamref name="T" /> that is the value of <paramref name="self" />, constrained between
		///     <paramref name="min" /> and <paramref name="max" />.
		/// </summary>
		/// <param name="self">The extended T.</param>
		/// <param name="min"> The minimum value of the <typeparamref name="T" /> that can be returned.</param>
		/// <param name="max"> The maximum value of the <typeparamref name="T" /> that can be returned.</param>
		/// <returns>The equivalent to: <c>this &lt; min ? min : this &gt; max ? max : this</c>.</returns>
		[NotNull]
		public static T Clamp<T>( [NotNull] this T self, [NotNull] T min, [NotNull] T max ) where T : IComparable<T> =>
			self.CompareTo( min ) < 0 ? min : self.CompareTo( max ) > 0 ? max : self;

		[ItemCanBeNull]
		public static IEnumerable<T> Concat<T>( [NotNull] this IEnumerable<T> first, [CanBeNull] T second ) {
			foreach ( var item in first ) {
				yield return item;
			}

			yield return second;
		}

		public static T[] Concat<T>( this T[] array1, T[] array2 ) {
			var result = new T[array1.LongLength + array2.LongLength];
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

		/// <summary>Just a no-op for setting a breakpoint on.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		[Conditional( "DEBUG" )]
		public static void Nop<T>( [CanBeNull] this T _ ) { }

		/// <summary>Just a no-op for setting a breakpoint on.</summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		[Conditional( "DEBUG" )]
		public static void Nop() { }

		/// <summary>
		///     <para>Works like the SQL "nullif" function.</para>
		///     <para>
		///         If <paramref name="left" /> is equal to <paramref name="right" /> then return null (or the default value for
		///         value types).
		///     </para>
		///     <para>Otherwise return <paramref name="left" />.</para>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		[CanBeNull]
		[DebuggerStepThrough]
		public static T NullIf<T>( [NotNull] this T left, [NotNull] T right ) where T : IComparer<T> => ( Comparer<T>.Default.Compare( left, right ) == 0 ? default : left )!;

		[CanBeNull]
		public static String? OnlyDigits( [CanBeNull] this String? input ) => String.IsNullOrWhiteSpace( input ) ? null : new String( input.Where( Char.IsDigit ).ToArray() );

		[CanBeNull]
		public static String? OnlyLetters( [CanBeNull] String? input ) => String.IsNullOrWhiteSpace( input ) ? null : new String( input.Where( Char.IsLetter ).ToArray() );

		[CanBeNull]
		public static String? OnlyLettersAndNumbers( [CanBeNull] String? input ) =>
			String.IsNullOrWhiteSpace( input ) ? null : new String( input.Where( c => Char.IsDigit( c ) || Char.IsLetter( c ) ).ToArray() );

		/// <summary>Swap <paramref name="left" /> with <paramref name="right" />.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		public static void Swap<T>( [CanBeNull] ref T left, [CanBeNull] ref T right ) {
			var temp = left;
			left = right;
			right = temp;
		}

		/// <summary>Given (T left, T right), Return (T right, T left).</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		[Pure]
		public static (T right, T left) Swap<T>( [CanBeNull] this T left, [CanBeNull] T right ) => ( right, left );

		/// <summary>
		///     Gets a <b>horribly</b> ROUGH guesstimate of the memory consumed by an object by using
		///     <see cref="Newtonsoft.Json.JsonConvert" /> .
		/// </summary>
		/// <param name="bob"></param>
		/// <returns></returns>
		[Pure]
		public static UInt64 MemoryUsed<T>( [CanBeNull] this T bob ) {
			try {
				if ( !( bob is null ) ) {
					return JsonConvert.SerializeObject( bob, Formatting.None ).LengthReal();
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return 0;
		}

	}

}