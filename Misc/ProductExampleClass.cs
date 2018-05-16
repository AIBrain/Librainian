// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ProductExampleClass.cs",
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
// "Librainian/Librainian/ProductExampleClass.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

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

            public String Description { get; set; }
        }
    }
}