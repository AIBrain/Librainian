// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/FuzzyPredicateBuilder.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.Linq.Expressions;

    [Obsolete( "warning: totally untested and unfinished" )]
    public static class FuzzyPredicateBuilder {

        public static Expression<Func<TTt, Boolean>> And<TTt>( this Expression<Func<TTt, Single>> expr1, Expression<Func<TTt, Single>> expr2 ) => Expression.Lambda<Func<TTt, Boolean>>( Expression.AndAlso( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), expr1.Parameters );

        public static Expression<Func<TTt, Boolean>> False<TTt>() => f => false;

        public static Expression<Func<TTt, Boolean>> Or<TTt>( this Expression<Func<TTt, Single>> expr1, Expression<Func<TTt, Single>> expr2 ) => Expression.Lambda<Func<TTt, Boolean>>( body: Expression.GreaterThanOrEqual( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), parameters: expr1.Parameters );

        public static Expression<Func<TTt, Boolean>> True<TTt>() => f => true;
    }
}