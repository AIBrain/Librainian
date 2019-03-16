// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ShoppingItem.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "ShoppingItem.cs" was last formatted by Protiguous on 2019/02/12 at 8:08 PM.

namespace Librainian.Financial.Containers.Shopping {

    using System;
    using Exceptions;
    using Extensions;
    using Newtonsoft.Json;

    [JsonObject]
    [Immutable]
    public class ShoppingItem {

        [JsonProperty]
        public ItemCategory Category { get; }

        [JsonProperty]
        public String Description { get; private set; }

        [JsonProperty]
        public Guid ItemID { get; }

        [JsonProperty]
        public Decimal Price { get; private set; }

        [JsonProperty]
        public Boolean TaxExempt { get; protected set; }

        [JsonProperty]
        public Boolean Voided { get; private set; }

        public ShoppingItem( ItemCategory category, Guid itemID ) {
            if ( category == ItemCategory.Invalid ) {
                throw new ArgumentNullException( nameof( category ) );
            }

            if ( itemID == Guid.Empty ) {
                throw new InvalidParameterException( "", new ArgumentNullException( nameof( itemID ) ) );
            }

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
            if ( ReferenceEquals( left, rhs ) ) {
                return true;
            }

            if ( null == left || null == rhs ) {
                return false;
            }

            return left.Category == rhs.Category && left.ItemID == rhs.ItemID;
        }

        /// <summary>
        ///     Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode() => (this.Category, this.ItemID).GetHashCode();

        public Boolean IsValidData() => this.Category != ItemCategory.Invalid && !this.ItemID.Equals( Guid.Empty );
    }
}