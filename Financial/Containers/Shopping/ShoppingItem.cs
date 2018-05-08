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
// "Librainian/ShoppingItem.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

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
            if ( category == ItemType.Invalid ) {
                throw new ArgumentNullException( nameof( category ) );
            }

	        this.Category = category;
            this.ItemID = itemID ?? throw new ArgumentNullException( nameof( itemID ) );
        }

        public ItemType Category {
            get;
        }

        public String Description {
            get; set;
        }

        public String ItemID {
            get;
        }

        public Decimal Price {
            get; set;
        }

        public Boolean TaxExempt {
            get; set;
        }

        public Boolean Voided {
            get; set;
        }

        /// <summary>
        ///     Compares <see cref="ItemID" /> and <see cref="Category" />.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean Equals( ShoppingItem lhs, ShoppingItem rhs ) {
            if ( ReferenceEquals( lhs, rhs ) ) {
                return true;
            }
            if ( null == lhs || null == rhs ) {
                return false;
            }
            return lhs.Category == rhs.Category && lhs.ItemID == rhs.ItemID;
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => this.Category.GetHashCode();

	    public Boolean IsValidData() => this.Category != ItemType.Invalid && !this.ItemID.IsNullOrWhiteSpace();

    }

    public class TaxableShoppingItem : ShoppingItem {

        public TaxableShoppingItem( ItemType category, [NotNull] String itemID ) : base( category, itemID ) {
        }
    }

    public class TaxTable {

        //TODO this should look up factors like area/state/zip/country
        public static Dictionary<ItemType, Decimal> Taxes { get; } = new Dictionary<ItemType, Decimal> { { ItemType.Invalid, 0m }, { ItemType.Book, 0.06m }, { ItemType.Food, 0.06m }, { ItemType.Medical, 0.06m }, { ItemType.Import, 0.06m }, { ItemType.Other, 0.06m } };
    }
}