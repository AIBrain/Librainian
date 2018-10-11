// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Instantiator.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Instantiator.cs" was last formatted by Protiguous on 2018/07/13 at 1:15 AM.

namespace Librainian.Magic
{

    using JetBrains.Annotations;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;

    /// <typeparam name="TInstance"></typeparam>
    /// <example>
    ///     Cat myCat = Instantiator
    ///     Cat.New("furry", isCute: true);
    /// </example>
    public static class Instantiator<TInstance>
    {

        static Instantiator() =>
            Debug.Assert(typeof(TInstance).IsValueType || typeof(TInstance).IsClass && !typeof(TInstance).IsAbstract, String.Concat("The type ", typeof(TInstance).Name, " is not constructable."));

        [NotNull]
        private static Expression<TDelegate> CreateLambdaExpression<TDelegate>([NotNull] params Type[] argTypes)
        {
            Debug.Assert(argTypes != null);

            var paramExpressions = new ParameterExpression[argTypes.Length];

            for (var i = 0; i < paramExpressions.Length; i++) { paramExpressions[i] = Expression.Parameter(argTypes[i], String.Concat("arg", i)); }

            var ctorInfo = typeof(TInstance).GetConstructor(argTypes);

            if (ctorInfo == null)
            {
                throw new ArgumentException(String.Concat("The type ", typeof(TInstance).Name, " has no constructor with the argument type(s) ", String.Join(", ", argTypes.Select(t => t.Name).ToArray()), "."),
                    nameof(argTypes));
            }

            return Expression.Lambda<TDelegate>(Expression.New(ctorInfo, paramExpressions.Select(expression => expression as Expression)), paramExpressions);
        }

        public static TInstance New() => InstantiatorImpl.CtorFunc();

        public static TInstance New<TA>(TA valueA) => InstantiatorImpl<TA>.CtorFunc(valueA);

        public static TInstance New<TA, TB>(TA valueA, TB valueB) => InstantiatorImpl<TA, TB>.CtorFunc(valueA, valueB);

        public static TInstance New<TA, TB, TC>(TA valueA, TB valueB, TC valueC) => InstantiatorImpl<TA, TB, TC>.CtorFunc(valueA, valueB, valueC);

        public static TInstance New<TA, TB, TC, TD>(TA valueA, TB valueB, TC valueC, TD valueD) => InstantiatorImpl<TA, TB, TC, TD>.CtorFunc(valueA, valueB, valueC, valueD);

        private static class InstantiatorImpl
        {

            public static readonly Func<TInstance> CtorFunc = Expression.Lambda<Func<TInstance>>(Expression.New(typeof(TInstance))).Compile();
        }

        private static class InstantiatorImpl<TA>
        {

            public static readonly Func<TA, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TInstance>>(typeof(TA)).Compile();
        }

        private static class InstantiatorImpl<TA, TB>
        {

            public static readonly Func<TA, TB, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TB, TInstance>>(typeof(TA), typeof(TB)).Compile();
        }

        private static class InstantiatorImpl<TA, TB, TC>
        {

            public static readonly Func<TA, TB, TC, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, TInstance>>(typeof(TA), typeof(TB), typeof(TC)).Compile();
        }

        private static class InstantiatorImpl<TA, TB, TC, TD>
        {

            public static readonly Func<TA, TB, TC, TD, TInstance> CtorFunc = CreateLambdaExpression<Func<TA, TB, TC, TD, TInstance>>(typeof(TA), typeof(TB), typeof(TC), typeof(TD)).Compile();
        }
    }
}