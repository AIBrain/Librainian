// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and royalties can be paid via
//
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ShoppingCart.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

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
            if ( null == items ) {
                return added;
            }
            foreach ( var item in items.Where( this.AddItem ) ) {
                added++;
            }
            return added;
        }

        public UInt32 AddItems( [CanBeNull] ShoppingItem item, UInt32 quantity ) {
            if ( item is null ) {
                return 0;
            }

            UInt32 added = 0;
            while ( quantity.Any() ) {
                if ( this.Items.TryAdd( item ) ) {
                    added++;
                }
                quantity--;
            }
            return added;
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() => this.Items.Dispose();

        /// <summary>
        /// Removes the first <paramref name="item"/> from the list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean RemoveItem( [CanBeNull] ShoppingItem item ) => this.Items.Remove( item );

        public IEnumerable<KeyValuePair<ShoppingItem, Int32>> RunningList() {
            var items = new ConcurrentDictionary<ShoppingItem, Int32>();
            foreach ( var shoppingItem in this.Items ) {
                if ( !items.ContainsKey( shoppingItem ) ) {
                    items.TryAdd( shoppingItem, 0 );
                }
                items[shoppingItem]++;
            }
            return items;
        }
    }
}