// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Instantiator.cs",
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
// "Librainian/Librainian/Instantiator.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Magic {

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    /// <typeparam name="TInstance"></typeparam>
    /// <example>
    ///     Cat myCat = Instantiator
    ///     Cat.New("furry", isCute: true);
    /// </example>
    public static class Instantiator<TInstance> {

        static Instantiator() =>
            Debug.Assert( typeof( TInstance ).IsValueType || typeof( TInstance ).IsClass && !typeof( TInstance ).IsAbstract, String.Concat( "The type ", typeof( TInstance ).Name, " is not constructable." ) );

        private static Expression<TDelegate> CreateLambdaExpression<TDelegate>( params Type[] argTypes ) {
            Debug.Assert( argTypes != null );

            var paramExpressions = new ParameterExpression[argTypes.Length];

            for ( var i = 0; i < paramExpressions.Length; i++ ) { paramExpressions[i] = Expression.Parameter( argTypes[i], String.Concat( "arg", i ) ); }

            var ctorInfo = typeof( TInstance ).GetConstructor( argTypes );

            if ( ctorInfo is null ) {
                throw new ArgumentException( String.Concat( "The type ", typeof( TInstance ).Name, " has no constructor with the argument type(s) ", String.Join( ", ", argTypes.Select( t => t.Name ).ToArray() ), "." ),
                    nameof( argTypes ) );
            }

            return Expression.Lambda<TDelegate>( Expression.New( ctorInfo, paramExpressions.Select( expression => expression as Expression ) ), paramExpressions );
        }

        public static TInstance New() => InstantiatorImpl.CtorFunc();

        public static TInstance New<TA>( TA valueA ) => InstantiatorImpl<TA>.CtorFunc( valueA );

        public static TInstance New<TA, TB>( TA valueA, TB valueB ) => InstantiatorImpl<TA, TB>.CtorFunc( valueA, valueB );

        public static TInstance New<TA, TB, TC>( TA valueA, TB valueB, TC valueC ) => InstantiatorImpl<TA, TB, TC>.CtorFunc( valueA, valueB, valueC );

        public static TInstance New<TA, TB, TC, TD>( TA valueA, TB valueB, TC valueC, TD valueD ) => InstantiatorImpl<TA, TB, TC, TD>.CtorFunc( valueA, valueB, valueC, valueD );

        private static class InstantiatorImpl {

            public static readonly Func<TInstance> CtorFunc = Expression.Lambda<Func<TInstance>>( Expression.New( typeof( TInstance ) ) ).Compile();
        }

        private static class InstantiatorImpl<TA> {

            public static readonly Func<TA, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TInstance>>( typeof( TA ) ).Compile();
        }

        private static class InstantiatorImpl<TA, TB> {

            public static readonly Func<TA, TB, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TB, TInstance>>( typeof( TA ), typeof( TB ) ).Compile();
        }

        private static class InstantiatorImpl<TA, TB, TC> {

            public static readonly Func<TA, TB, TC, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, TInstance>>( typeof( TA ), typeof( TB ), typeof( TC ) ).Compile();
        }

        private static class InstantiatorImpl<TA, TB, TC, TD> {

            public static readonly Func<TA, TB, TC, TD, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, TD, TInstance>>( typeof( TA ), typeof( TB ), typeof( TC ), typeof( TD ) ).Compile();
        }
    }
}