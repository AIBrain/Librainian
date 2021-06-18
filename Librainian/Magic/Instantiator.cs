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
// File "Instantiator.cs" last formatted on 2020-08-14 at 8:35 PM.

namespace Librainian.Magic {

	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;
	using Exceptions;

	/// <typeparam name="T"></typeparam>
	/// <example>
	///     <code>var cat = Instantiator&lt;ReaderWriterLockSlim&gt;.New("furry", isCute: true);</code>
	/// </example>
	public static class Instantiator<T> {

		static Instantiator() =>
			Debug.Assert( typeof( T ).IsValueType || typeof( T ).IsClass && !typeof( T ).IsAbstract,
						  String.Concat( "The type ", typeof( T ).Name, " is not constructable." ) );

		private static Expression<TDelegate> CreateLambdaExpression<TDelegate>( params Type[] argTypes ) {
			if ( argTypes is null ) {
				throw new ArgumentEmptyException( nameof( argTypes ) );
			}

			var paramExpressions = new ParameterExpression[ argTypes.Length ];

			for ( var i = 0; i < paramExpressions.Length; i++ ) {
				paramExpressions[ i ] = Expression.Parameter( argTypes[ i ], String.Concat( "arg", i ) );
			}

			var ctorInfo = typeof( T ).GetConstructor( argTypes );

			if ( ctorInfo is null ) {
				throw new ArgumentException(
					String.Concat( "The type ", typeof( T ).Name, " has no constructor with the argument type(s) ",
								   String.Join( ", ", argTypes.Select( t => t.Name ).ToArray() ), "." ), nameof( argTypes ) );
			}

			return Expression.Lambda<TDelegate>( Expression.New( ctorInfo, paramExpressions.Select( expression => expression as Expression ) ), paramExpressions );
		}

		/// <summary>Create a new instance of type <see cref="T" /> with no parameters.</summary>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New() => InstantiatorImpl.CtorFunc();

		/// <summary>Create a new instance of type <see cref="T" /> with one parameter.</summary>
		/// <typeparam name="TA"></typeparam>
		/// <param name="valueA"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New<TA>( TA? valueA ) => InstantiatorImpl<TA>.CtorFunc( valueA );

		/// <summary>Create a new instance of type <see cref="T" /> with two parameters.</summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <param name="valueA"></param>
		/// <param name="valueB"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New<TA, TB>( TA? valueA, TB? valueB ) => InstantiatorImpl<TA, TB>.CtorFunc( valueA, valueB );

		/// <summary>Create a new instance of type <see cref="T" /> with three parameters.</summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <typeparam name="TC"></typeparam>
		/// <param name="valueA"></param>
		/// <param name="valueB"></param>
		/// <param name="valueC"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New<TA, TB, TC>( TA? valueA, TB? valueB, TC? valueC ) =>
			InstantiatorImpl<TA, TB, TC>.CtorFunc( valueA, valueB, valueC );

		/// <summary>Create a new instance of type <see cref="T" /> with four parameters.</summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <typeparam name="TC"></typeparam>
		/// <typeparam name="TD"></typeparam>
		/// <param name="valueA"></param>
		/// <param name="valueB"></param>
		/// <param name="valueC"></param>
		/// <param name="valueD"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New<TA, TB, TC, TD>( TA? valueA, TB? valueB, TC? valueC, TD? valueD ) =>
			InstantiatorImpl<TA, TB, TC, TD>.CtorFunc( valueA, valueB, valueC, valueD );

		/// <summary>Create a new instance of type <see cref="T" /> with five parameters.</summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <typeparam name="TC"></typeparam>
		/// <typeparam name="TD"></typeparam>
		/// <typeparam name="TE"></typeparam>
		/// <param name="valueA"></param>
		/// <param name="valueB"></param>
		/// <param name="valueC"></param>
		/// <param name="valueD"></param>
		/// <param name="valueE"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New<TA, TB, TC, TD, TE>( TA? valueA, TB? valueB, TC? valueC, TD? valueD, TE? valueE ) =>
			InstantiatorImpl<TA, TB, TC, TD, TE>.CtorFunc( valueA, valueB, valueC, valueD, valueE );

		/// <summary>Create a new instance of type <see cref="T" /> with six parameters.</summary>
		/// <typeparam name="TA"></typeparam>
		/// <typeparam name="TB"></typeparam>
		/// <typeparam name="TC"></typeparam>
		/// <typeparam name="TD"></typeparam>
		/// <typeparam name="TE"></typeparam>
		/// <typeparam name="TF"></typeparam>
		/// <param name="valueA"></param>
		/// <param name="valueB"></param>
		/// <param name="valueC"></param>
		/// <param name="valueD"></param>
		/// <param name="valueE"></param>
		/// <param name="valueF"></param>
		/// <returns></returns>
		[return: System.Diagnostics.CodeAnalysis.NotNull]
		public static T New<TA, TB, TC, TD, TE, TF>(
			TA? valueA,
			TB? valueB,
			TC? valueC,
			TD? valueD,
			TE? valueE,
			TF? valueF
		) =>
			InstantiatorImpl<TA, TB, TC, TD, TE, TF>.CtorFunc( valueA, valueB, valueC, valueD, valueE, valueF );

		private static class InstantiatorImpl {

			public static readonly Func<T> CtorFunc = Expression.Lambda<Func<T>>( Expression.New( typeof( T ) ) ).Compile();
		}

		private static class InstantiatorImpl<TA> {

			public static readonly Func<TA, T> CtorFunc = CreateLambdaExpression<Func<TA, T>>( typeof( TA ) ).Compile();
		}

		private static class InstantiatorImpl<TA, TB> {

			public static readonly Func<TA, TB, T> CtorFunc = CreateLambdaExpression<Func<TA, TB, T>>( typeof( TA ), typeof( TB ) ).Compile();
		}

		private static class InstantiatorImpl<TA, TB, TC> {

			public static readonly Func<TA, TB, TC, T> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, T>>( typeof( TA ), typeof( TB ), typeof( TC ) ).Compile();
		}

		private static class InstantiatorImpl<TA, TB, TC, TD> {

			public static readonly Func<TA, TB, TC, TD, T> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, TD, T>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ) )
				.Compile();
		}

		private static class InstantiatorImpl<TA, TB, TC, TD, TE> {

			public static readonly Func<TA, TB, TC, TD, TE, T> CtorFunc =
				CreateLambdaExpression<Func<TA, TB, TC, TD, TE, T>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ), typeof( TE ) ).Compile();
		}

		private static class InstantiatorImpl<TA, TB, TC, TD, TE, TF> {

			public static readonly Func<TA, TB, TC, TD, TE, TF, T> CtorFunc =
				CreateLambdaExpression<Func<TA, TB, TC, TD, TE, TF, T>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ), typeof( TE ), typeof( TF ) ).Compile();
		}
	}
}