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
// "Librainian/FuzzyPredicateBuilder.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.Linq.Expressions;

    [Obsolete( "warning: totally untested and unfinished" )]
    public static class FuzzyPredicateBuilder {
        public static Expression< Func< TTT, Boolean > > And< TTT >( this Expression< Func< TTT, Single > > expr1, Expression< Func< TTT, Single > > expr2 ) => Expression.Lambda< Func< TTT, Boolean > >( Expression.AndAlso( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), expr1.Parameters );

        public static Expression< Func< TTT, Boolean > > False< TTT >() => f => false;

        public static Expression< Func< TTT, Boolean > > Or< TTT >( this Expression< Func< TTT, Single > > expr1, Expression< Func< TTT, Single > > expr2 ) => Expression.Lambda< Func< TTT, Boolean > >( body: Expression.GreaterThanOrEqual( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), parameters: expr1.Parameters );

        public static Expression< Func< TTT, Boolean > > True< TTT >() => f => true;
    }
}
