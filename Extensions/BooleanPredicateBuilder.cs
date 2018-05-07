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
// "Librainian/BooleanPredicateBuilder.cs" was last cleaned by Protiguous on 2016/06/18 at 10:50 PM

namespace Librainian.Extensions {

    using System;
    using System.Linq.Expressions;

    public static class BooleanPredicateBuilder {

        public static Expression<Func<TTt, Boolean>> And<TTt>( this Expression<Func<TTt, Boolean>> expr1, Expression<Func<TTt, Boolean>> expr2 ) => Expression.Lambda<Func<TTt, Boolean>>( Expression.AndAlso( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), expr1.Parameters );

        public static Expression<Func<TTt, Boolean>> False<TTt>() => f => false;

        public static Expression<Func<TTt, Boolean>> Or<TTt>( this Expression<Func<TTt, Boolean>> expr1, Expression<Func<TTt, Boolean>> expr2 ) => Expression.Lambda<Func<TTt, Boolean>>( Expression.OrElse( expr1.Body, Expression.Invoke( expr2, expr1.Parameters ) ), expr1.Parameters );

        public static Expression<Func<TTt, Boolean>> True<TTt>() => f => true;
    }
}