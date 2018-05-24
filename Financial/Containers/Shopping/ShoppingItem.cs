// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ShoppingItem.cs" belongs to Rick@AIBrain.org and
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
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ShoppingItem.cs" was last formatted by Protiguous on 2018/05/21 at 10:02 PM.

namespace Librainian.Financial.Containers.Shopping {

    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Extensions;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>
    /// Obviously there are thousands of real categories that should be loaded from a json document. But this is just a sample/experimental class.
    /// </summary>
    public enum ItemCategory {

        Invalid = -1,

        Other,

        Book,

        Food,

        Medical,

        Import
    }

    [JsonObject]
    [Immutable]
    public class ShoppingItem {

        [JsonProperty]
        public ItemCategory Category { get; private set; }

        [JsonProperty]
        public String Description { get; private set; }

        [JsonProperty]
        public Guid ItemID { get; private set; }

        [JsonProperty]
        public Decimal Price { get; private set; }

        [JsonProperty]
        public Boolean TaxExempt { get; protected set; }

        [JsonProperty]
        public Boolean Voided { get; private set; }

        public ShoppingItem( ItemCategory category, Guid itemID ) {
            if ( category == ItemCategory.Invalid ) { throw new ArgumentNullException( nameof( category ) ); }

            if ( itemID == Guid.Empty ) { throw new InvalidParameterException( $"", new ArgumentNullException( nameof( itemID ) ) ); }

            this.Category = category;
            this.ItemID = itemID;
        }

        /// <summary>
        ///     Static comparison. Compares <see cref="ItemID" /> and <see cref="Category" />.
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
        public override Int32 GetHashCode() => this.Category.GetHashCodes( this.ItemID ); //non  { get; }  props.. what to do here?

        public Boolean IsValidData() => this.Category != ItemCategory.Invalid && !this.ItemID.Equals( Guid.Empty );
    }

    public class TaxableShoppingItem : ShoppingItem {

        public TaxableShoppingItem( ItemCategory category, Guid itemID ) : base( category, itemID ) => this.TaxExempt = false;
    }

    public class TaxTable {

        //TODO this should look up factors like area/state/zip/country, if this were a real project.
        public static Dictionary<ItemCategory, Decimal> Taxes { get; } = new Dictionary<ItemCategory, Decimal> {
            { ItemCategory.Invalid, 0m },
            { ItemCategory.Book, 0.06m },
            { ItemCategory.Food, 0.06m },
            { ItemCategory.Medical, 0.06m },
            { ItemCategory.Import, 0.06m },
            { ItemCategory.Other, 0.06m }
        };
    }
}