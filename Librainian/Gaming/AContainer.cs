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
// File "AContainer.cs" last formatted on 2022-12-22 at 5:16 PM by Protiguous.

namespace Librainian.Gaming;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Exceptions;

public abstract record AContainer( Guid Identifier, String Name ) : AbstractGameObject( Identifier ), IHasName, IHasInventory, IEnumerable<AbstractGameObject> {
	public IEnumerator<AbstractGameObject> GetEnumerator() => this.Items.Values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	public ConcurrentDictionary<Guid, AbstractGameObject> Items { get; } = new();

	public Boolean MoveAll( AContainer destination ) {
		if ( destination is null ) {
			throw new NullException( nameof( destination ) );
		}

		if ( ReferenceEquals( this, destination ) ) {
			return false;
		}

		var moved = 0ul;

		while ( this.Items.Any() ) {
			if ( this.MoveOne( destination ) ) {
				moved++;
			}
		}

		return moved > 0;
	}

	/// <summary>
	///     <para>
	///         Move one random <see cref="AbstractGameObject" /> from this <see cref="AContainer" /> to another
	///         <see cref="AContainer" />( <paramref name="destination" />).
	///     </para>
	///     <para>
	///         If the <see cref="AbstractGameObject" /> is a <see cref="Dice" />, then the <see cref="Dice" /> will be
	///         <see
	///             cref="Dice.Roll" />
	///         ed.
	///     </para>
	/// </summary>
	/// <param name="destination"></param>
	/// <returns></returns>
	public Boolean MoveOne( AContainer destination ) {
		if ( destination is null ) {
			throw new NullException( nameof( destination ) );
		}

		var gameItem = this.Items.Values.FirstOrDefault();

		if ( gameItem is default( AbstractGameObject ) ) {
			return false;
		}

		if ( !this.TryTake( gameItem ) ) {
			return false;
		}

		( gameItem as Dice )?.Roll(); //If we're "pouring" a dice, roll it.

		destination.Add( gameItem );
		return true;
	}

	/// <summary>Try to take one game item.</summary>
	/// <param name="item"></param>
	public Boolean TryTake( AbstractGameObject item ) => this.Items.TryRemove( item.Identifier, out var _ );

	/// <summary>Add one non-null game item</summary>
	/// <param name="item"></param>
	public void Add( AbstractGameObject item ) {
		if ( item is null ) {
			throw new NullException( nameof( item ) );
		}

		this.Items.TryAdd( item.Identifier, item );
	}

	public void AddRange( IEnumerable<AbstractGameObject> list ) {
		if ( list is null ) {
			throw new NullException( nameof( list ) );
		}

		foreach ( var gameObject in list ) {
			this.Add( gameObject );
		}
	}

	public Boolean IsEmpty() => !this.Items.Any();
}