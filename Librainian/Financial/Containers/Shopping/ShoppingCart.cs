// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ShoppingCart.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.Financial.Containers.Shopping;

using System;
using System.Linq;
using Collections.Lists;
using Maths;
using Newtonsoft.Json;
using Utilities.Disposables;

/// <summary>Just a concept class.</summary>
[JsonObject]
public class ShoppingCart : ABetterClassDispose {

	[JsonProperty]
	private ConcurrentList<ShoppingItem> Items { get; } = new(); //TODO make this a dictionary of Item.Counts

	public Boolean Add( ShoppingItem item ) => this.Items.TryAdd( item );

	public UInt32 AddItems( params ShoppingItem[]? items ) {
		if ( items is null ) {
			return 0;
		}

		return ( UInt32 )items.Count( this.Add );
	}

	public UInt32 AddItems( ShoppingItem? item, UInt32 quantity = 1 ) {
		UInt32 added = 0;

		if ( item is not null ) {
			while ( quantity.Any() ) {
				if ( this.Add( item ) ) {
					added++;
				}

				quantity--;
			}
		}

		return added;
	}

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		using ( this.Items ) { }
	}

	/// <summary>Removes the first <paramref name="item" /> from the list.</summary>
	/// <param name="item"></param>
	public Boolean RemoveItem( ShoppingItem? item ) => this.Items.Remove( item );
}