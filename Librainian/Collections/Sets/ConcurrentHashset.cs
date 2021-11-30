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
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ConcurrentHashset.cs" last touched on 2021-10-15 at 5:28 AM by Protiguous.

#nullable enable

namespace Librainian.Collections.Sets;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Utilities;

/// <summary>
///     Threadsafe set. Does not allow nulls inside the set.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>Class designed by Rick Harker</remarks>
/// //TODO someday add in set theory.. someday.. ISet
[Serializable]
[JsonObject]
[NeedsTesting]
public class ConcurrentHashset<T> : IEnumerable<T> where T : notnull {

	[DebuggerStepThrough]
	public ConcurrentHashset( ICollection<T> collection, CancellationToken cancellationToken ) : this( collection, Environment.ProcessorCount, EqualityComparer<T>.Default, cancellationToken ) { }

	[DebuggerStepThrough]
	public ConcurrentHashset( ICollection<T> collection, Int32 concurrency, CancellationToken cancellationToken ) : this( collection, concurrency, EqualityComparer<T>.Default, cancellationToken ) { }

	[DebuggerStepThrough]
	public ConcurrentHashset( ICollection<T> collection, Int32 concurrency, EqualityComparer<T>? comparer, CancellationToken cancellationToken ) {
		this.Set = new ConcurrentDictionary<T, Object?>( concurrency, collection.Count, comparer );
		this.AddRange( collection.ToAsyncEnumerable(), cancellationToken ).Wait(cancellationToken);
	}

	[DebuggerStepThrough]
	public ConcurrentHashset( EqualityComparer<T>? comparer = null ) => this.Set = new ConcurrentDictionary<T, Object?>( comparer );

	[JsonProperty]
	private ConcurrentDictionary<T, Object?> Set { get; }

	public Int32 Count => this.Set.Count;

	/// <summary>Gets the item in the set or throws.</summary>
	/// <param name="index"></param>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public T this[ Int32 index ] {
		[NotNull]
		get {
			if ( index < 0 ) {
				throw new IndexOutOfRangeException( $"The index {index} is less than 0." );
			}

			var list = this.Set.Keys;

			if ( index > list.Count ) {
				throw new IndexOutOfRangeException( $"The index {index} is greater than items in set ({list.Count})." );
			}

			//TODO Is this any better than var list = this.Set.Keys.ToList()?
			//TODO Will this skip work?
			//TODO Is it off by -1?
			return list.Skip( index ).First();
		}
	}

	public IEnumerator<T> GetEnumerator() => this.Set.Keys.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

	[DebuggerStepThrough]
	public void Add( T item ) {
		this.Set[ item ] = null;
	}

	[DebuggerStepThrough]
	public async Task AddRange( IAsyncEnumerable<T> items, CancellationToken cancellationToken ) {
		if ( items is null ) {
			throw new NullException( nameof( items ) );
		}

		await foreach ( var item in items.ConfigureAwait( false ) ) {
			if ( cancellationToken.IsCancellationRequested ) {
				break;
			}
			this.Add( item );
		}
	}

	[DebuggerStepThrough]
	public void Clear() => this.Set.Clear();

	[DebuggerStepThrough]
	public Boolean Contains( T item ) => this.Set.ContainsKey( item );

	[DebuggerStepThrough]
	public Boolean Remove( T item ) => this.Set.TryRemove( item, out var _ );

	/// <summary>
	///     Replace left with right. ( <see cref="Remove" /><paramref name="left" />, then <see cref="Add" />
	///     <paramref name="right" />)
	/// </summary>
	/// <param name="left"> </param>
	/// <param name="right"></param>
	public void Replace( T left, T right ) {
		this.Remove( left );
		this.Add( right );
	}

	/// <summary>Set the tag on an item.</summary>
	/// <param name="item"></param>
	/// <param name="tag"></param>
	public Boolean Tag( T item, Object? tag ) {
		this.Set[ item ] = tag;

		return true;
	}

	/// <summary>Get the tag on an item.</summary>
	/// <param name="item"></param>
	public Object? Tag( T item ) {
		this.Set.TryGetValue( item, out var tag );

		return tag;
	}

}