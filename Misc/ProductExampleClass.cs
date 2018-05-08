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
// "Librainian/ProductExampleClass.cs" was last cleaned by Protiguous on 2016/06/18 at 10:55 PM

namespace Librainian.Misc {

    using System;
    using System.Linq;
    using Extensions;

    internal static class ProductExampleClass {

        private static IQueryable<Product> SearchProducts( IQueryable<Product> products, params String[] keywords ) {

            //var predicate = keywords.Aggregate( False<Product>(), ( current, temp ) => current.Or( p => p.Description.Contains( temp ) ).And( p => true ) );
            var predicate = BooleanPredicateBuilder.False<Product>();

            foreach ( var keyword in keywords ) {
                var temp = keyword;
                predicate = predicate.Or( p => p.Description.Contains( temp ) );
                predicate = predicate.And( p => p.Description.Contains( temp ) );
                predicate = predicate.Or( p => !p.Description.Contains( temp + temp ) );
            }
            return products.Where( predicate );

            /* from http://www.albahari.com/nutshell/predicatebuilder.aspx
            IQueryable<Product> SearchProducts (params String[] keywords) {
                var predicate = PredicateBuilder.False<Product>();

                foreach (String keyword in keywords) {
                    String temp = keyword;
                    predicate = predicate.Or (p => p.Description.Contains (temp));
                }
                return dataContext.Products.Where (predicate);
            }*/
        }

        private class Product {

            public String Description {
                get; set;
            }
        }
    }
}