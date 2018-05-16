// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ShoppingItem.cs",
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
// "Librainian/Librainian/ShoppingItem.cs" was last cleaned by Protiguous on 2018/05/15 at 10:42 PM.

namespace Librainian.Financial.Containers.Shopping {

    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Parsing;

    public enum ItemType {

        Invalid = -1,

        Other,

        Book,

        Food,

        Medical,

        Import
    }

    [JsonObject]
    public class ShoppingItem {

        public ShoppingItem( ItemType category, [NotNull] String itemID ) {
            if ( category == ItemType.Invalid ) { throw new ArgumentNullException( nameof( category ) ); }

            this.Category = category;
            this.ItemID = itemID ?? throw new ArgumentNullException( nameof( itemID ) );
        }

        public ItemType Category { get; }

        public String Description { get; set; }

        public String ItemID { get; }

        public Decimal Price { get; set; }

        public Boolean TaxExempt { get; set; }

        public Boolean Voided { get; set; }

        /// <summary>
        ///     Compares <see cref="ItemID" /> and <see cref="Category" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rhs"> </param>
        /// <returns></returns>
        public static Boolean Equals( ShoppingItem left, ShoppingItem rhs ) {
            if ( ReferenceEquals( left, rhs ) ) { return true; }

            if ( null == left || null == rhs ) { return false; }

            return left.Category == rhs.Category && left.ItemID == rhs.ItemID;
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Category.GetHashCode();

        public Boolean IsValidData() => this.Category != ItemType.Invalid && !this.ItemID.IsNullOrWhiteSpace();
    }

    public class TaxableShoppingItem : ShoppingItem {

        public TaxableShoppingItem( ItemType category, [NotNull] String itemID ) : base( category, itemID ) { }
    }

    public class TaxTable {

        //TODO this should look up factors like area/state/zip/country
        public static Dictionary<ItemType, Decimal> Taxes { get; } = new Dictionary<ItemType, Decimal> {
            { ItemType.Invalid, 0m },
            { ItemType.Book, 0.06m },
            { ItemType.Food, 0.06m },
            { ItemType.Medical, 0.06m },
            { ItemType.Import, 0.06m },
            { ItemType.Other, 0.06m }
        };
    }
}