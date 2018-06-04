// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ProductExampleClass.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "ProductExampleClass.cs" was last formatted by Protiguous on 2018/06/04 at 4:18 PM.

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