// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Instantiator.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

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

		static Instantiator() => Debug.Assert( typeof( TInstance ).IsValueType || typeof( TInstance ).IsClass && !typeof( TInstance ).IsAbstract, String.Concat( "The type ", typeof( TInstance ).Name, " is not constructable." ) );

		public static TInstance New() => InstantiatorImpl.CtorFunc();

        public static TInstance New<TA>( TA valueA ) => InstantiatorImpl<TA>.CtorFunc( valueA );

        public static TInstance New<TA, TB>( TA valueA, TB valueB ) => InstantiatorImpl<TA, TB>.CtorFunc( valueA, valueB );

        public static TInstance New<TA, TB, TC>( TA valueA, TB valueB, TC valueC ) => InstantiatorImpl<TA, TB, TC>.CtorFunc( valueA, valueB, valueC );

        public static TInstance New<TA, TB, TC, TD>( TA valueA, TB valueB, TC valueC, TD valueD ) => InstantiatorImpl<TA, TB, TC, TD>.CtorFunc( valueA, valueB, valueC, valueD );

        private static Expression<TDelegate> CreateLambdaExpression<TDelegate>( params Type[] argTypes ) {
            Debug.Assert( argTypes != null );

            var paramExpressions = new ParameterExpression[ argTypes.Length ];

            for ( var i = 0; i < paramExpressions.Length; i++ ) {
                paramExpressions[ i ] = Expression.Parameter( argTypes[ i ], String.Concat( "arg", i ) );
            }

            var ctorInfo = typeof( TInstance ).GetConstructor( argTypes );
            if ( ctorInfo is null ) {
                throw new ArgumentException( String.Concat( "The type ", typeof( TInstance ).Name, " has no constructor with the argument type(s) ", String.Join( ", ", argTypes.Select( t => t.Name ).ToArray() ), "." ), nameof( argTypes ) );
            }

            return Expression.Lambda<TDelegate>( Expression.New( ctorInfo, paramExpressions.Select( expression => expression as Expression ) ), paramExpressions );
        }

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