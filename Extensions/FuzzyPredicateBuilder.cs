// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FuzzyPredicateBuilder.cs" was last cleaned by Protiguous on 2018/05/12 at 1:22 AM

namespace Librainian.Extensions {

    using System;
    using System.Linq.Expressions;

    [Obsolete( "warning: totally untested and unfinished" )]
    public static class FuzzyPredicateBuilder {

        public static Expression<Func<TTt, Boolean>> And<TTt>( this Expression<Func<TTt, Single>> expr1, Expression<Func<TTt, Single>> expr2 ) =>
            Expression.Lambda<Func<TTt, Boolean>>( Expression.AndAlso( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), expr1.Parameters );

        public static Expression<Func<TTt, Boolean>> False<TTt>() => f => false;

        public static Expression<Func<TTt, Boolean>> Or<TTt>( this Expression<Func<TTt, Single>> expr1, Expression<Func<TTt, Single>> expr2 ) =>
            Expression.Lambda<Func<TTt, Boolean>>( body: Expression.GreaterThanOrEqual( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), parameters: expr1.Parameters );

        public static Expression<Func<TTt, Boolean>> True<TTt>() => f => true;
    }
}