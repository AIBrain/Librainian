// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ShoppingCart.cs",
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
// "Librainian/Librainian/ShoppingCart.cs" was last cleaned by Protiguous on 2018/05/15 at 10:42 PM.

namespace Librainian.Financial.Containers.Shopping {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Collections;
    using JetBrains.Annotations;
    using Magic;
    using Maths;
    using Newtonsoft.Json;

    [JsonObject]
    public class ShoppingCart : ABetterClassDispose {

        [JsonProperty]
        private ConcurrentList<ShoppingItem> Items { get; } = new ConcurrentList<ShoppingItem>();

        public Boolean AddItem( [CanBeNull] ShoppingItem item ) => item != null && this.Items.TryAdd( item );

        public UInt32 AddItems( params ShoppingItem[] items ) {
            UInt32 added = 0;

            if ( null == items ) { return added; }

            foreach ( var item in items.Where( this.AddItem ) ) { added++; }

            return added;
        }

        public UInt32 AddItems( [CanBeNull] ShoppingItem item, UInt32 quantity ) {
            if ( item is null ) { return 0; }

            UInt32 added = 0;

            while ( quantity.Any() ) {
                if ( this.Items.TryAdd( item ) ) { added++; }

                quantity--;
            }

            return added;
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Items.Dispose();

        /// <summary>
        ///     Removes the first <paramref name="item" /> from the list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean RemoveItem( [CanBeNull] ShoppingItem item ) => this.Items.Remove( item );

        public IEnumerable<KeyValuePair<ShoppingItem, Int32>> RunningList() {
            var items = new ConcurrentDictionary<ShoppingItem, Int32>();

            foreach ( var shoppingItem in this.Items ) {
                if ( !items.ContainsKey( shoppingItem ) ) { items.TryAdd( shoppingItem, 0 ); }

                items[shoppingItem]++;
            }

            return items;
        }
    }
}