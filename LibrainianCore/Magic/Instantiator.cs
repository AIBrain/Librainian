// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Instantiator.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "Instantiator.cs" was last formatted by Protiguous on 2020/03/16 at 3:06 PM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    /// <typeparam name="T"></typeparam>
    /// <example>
    ///     <code>var cat = Instantiator&lt;ReaderWriterLockSlim&gt;.New("furry", isCute: true);</code>
    /// </example>
    public static class Instantiator<T> {

        static Instantiator() =>
            Debug.Assert( condition: typeof( T ).IsValueType || typeof( T ).IsClass && !typeof( T ).IsAbstract,
                message: String.Concat( str0: "The type ", str1: typeof( T ).Name, str2: " is not constructable." ) );

        [NotNull]
        private static Expression<TDelegate> CreateLambdaExpression<TDelegate>( [NotNull] params Type[] argTypes ) {
            if ( argTypes is null ) {
                throw new ArgumentNullException( paramName: nameof( argTypes ) );
            }

            var paramExpressions = new ParameterExpression[ argTypes.Length ];

            for ( var i = 0; i < paramExpressions.Length; i++ ) {
                paramExpressions[ i ] = Expression.Parameter( type: argTypes[ i ], name: String.Concat( arg0: "arg", arg1: i ) );
            }

            var ctorInfo = typeof( T ).GetConstructor( types: argTypes );

            if ( ctorInfo is null ) {
                throw new ArgumentException(
                    message: String.Concat( "The type ", typeof( T ).Name, " has no constructor with the argument type(s) ",
                        String.Join( separator: ", ", value: argTypes.Select( selector: t => t.Name ).ToArray() ), "." ), paramName: nameof( argTypes ) );
            }

            return Expression.Lambda<TDelegate>(
                body: Expression.New( constructor: ctorInfo, arguments: paramExpressions.Select( selector: expression => expression as Expression ) ),
                parameters: paramExpressions );
        }

        /// <summary>Create a new instance of type <see cref="T" /> with no parameters.</summary>
        /// <returns></returns>
        [NotNull]
        public static T New() => InstantiatorImpl.CtorFunc();

        /// <summary>Create a new instance of type <see cref="T" /> with one parameter.</summary>
        /// <typeparam name="TA"></typeparam>
        /// <param name="valueA"></param>
        /// <returns></returns>
        [NotNull]
        public static T New<TA>( [CanBeNull] TA valueA ) => InstantiatorImpl<TA>.CtorFunc( arg: valueA );

        /// <summary>Create a new instance of type <see cref="T" /> with two parameters.</summary>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <returns></returns>
        [NotNull]
        public static T New<TA, TB>( [CanBeNull] TA valueA, [CanBeNull] TB valueB ) => InstantiatorImpl<TA, TB>.CtorFunc( arg1: valueA, arg2: valueB );

        /// <summary>Create a new instance of type <see cref="T" /> with three parameters.</summary>
        /// <typeparam name="TA"></typeparam>
        /// <typeparam name="TB"></typeparam>
        /// <typeparam name="TC"></typeparam>
        /// <param name="valueA"></param>
        /// <param name="valueB"></param>
        /// <param name="valueC"></param>
        /// <returns></returns>
        [NotNull]
        public static T New<TA, TB, TC>( [CanBeNull] TA valueA, [CanBeNull] TB valueB, [CanBeNull] TC valueC ) =>
            InstantiatorImpl<TA, TB, TC>.CtorFunc( arg1: valueA, arg2: valueB, arg3: valueC );

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
        [NotNull]
        public static T New<TA, TB, TC, TD>( [CanBeNull] TA valueA, [CanBeNull] TB valueB, [CanBeNull] TC valueC, [CanBeNull] TD valueD ) =>
            InstantiatorImpl<TA, TB, TC, TD>.CtorFunc( arg1: valueA, arg2: valueB, arg3: valueC, arg4: valueD );

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
        [NotNull]
        public static T New<TA, TB, TC, TD, TE>( [CanBeNull] TA valueA, [CanBeNull] TB valueB, [CanBeNull] TC valueC, [CanBeNull] TD valueD, [CanBeNull] TE valueE ) =>
            InstantiatorImpl<TA, TB, TC, TD, TE>.CtorFunc( arg1: valueA, arg2: valueB, arg3: valueC, arg4: valueD, arg5: valueE );

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
        [NotNull]
        public static T New<TA, TB, TC, TD, TE, TF>( [CanBeNull] TA valueA, [CanBeNull] TB valueB, [CanBeNull] TC valueC, [CanBeNull] TD valueD, [CanBeNull] TE valueE,
            [CanBeNull] TF valueF ) =>
            InstantiatorImpl<TA, TB, TC, TD, TE, TF>.CtorFunc( arg1: valueA, arg2: valueB, arg3: valueC, arg4: valueD, arg5: valueE, arg6: valueF );

        private static class InstantiatorImpl {

            public static readonly Func<T> CtorFunc = Expression.Lambda<Func<T>>( body: Expression.New( type: typeof( T ) ) ).Compile();

        }

        private static class InstantiatorImpl<TA> {

            [NotNull]
            public static readonly Func<TA, T> CtorFunc = CreateLambdaExpression<Func<TA, T>>( typeof( TA ) ).Compile();

        }

        private static class InstantiatorImpl<TA, TB> {

            [NotNull]
            public static readonly Func<TA, TB, T> CtorFunc = CreateLambdaExpression<Func<TA, TB, T>>( typeof( TA ), typeof( TB ) ).Compile();

        }

        private static class InstantiatorImpl<TA, TB, TC> {

            [NotNull]
            public static readonly Func<TA, TB, TC, T> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, T>>( typeof( TA ), typeof( TB ), typeof( TC ) ).Compile();

        }

        private static class InstantiatorImpl<TA, TB, TC, TD> {

            [NotNull]
            public static readonly Func<TA, TB, TC, TD, T> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, TD, T>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ) )
                .Compile();

        }

        private static class InstantiatorImpl<TA, TB, TC, TD, TE> {

            [NotNull]
            public static readonly Func<TA, TB, TC, TD, TE, T> CtorFunc =
                CreateLambdaExpression<Func<TA, TB, TC, TD, TE, T>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ), typeof( TE ) ).Compile();

        }

        private static class InstantiatorImpl<TA, TB, TC, TD, TE, TF> {

            [NotNull]
            public static readonly Func<TA, TB, TC, TD, TE, TF, T> CtorFunc =
                CreateLambdaExpression<Func<TA, TB, TC, TD, TE, TF, T>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ), typeof( TE ), typeof( TF ) ).Compile();

        }

    }

}