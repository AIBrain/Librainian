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
// File "ConcurrentList.cs" last formatted on 2021-10-26 at 3:22 PM by Protiguous.

#nullable enable

namespace Librainian.Collections.Lists;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using Extensions;
using JetBrains.Annotations;
using Logging;
using Maths;
using Newtonsoft.Json;
using Parsing;
using Threading;
using Utilities;
using Utilities.Disposables;

/// <summary>
///     <para>A thread safe generic list.</para>
///     <para>Use at your own risk.</para>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
///     <para>This class was created on a spur of the moment idea, and is <b>THOROUGHLY UNTESTED™</b>.</para>
///     <para>Uses a <see cref="ConcurrentQueue{T}" /> to buffer adds.</para>
///     <para>Call <see cref="CatchUp" /> to add any pending items.</para>
/// </remarks>
/// <copyright>Protiguous@Protiguous.com</copyright>
[JsonObject]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[NeedsTesting]
public class ConcurrentList<T> : ABetterClassDispose, IList<T?> /*, IEquatable<IEnumerable<T>>*/ {

	private const String CouldNotObtainReadLock = "Unable to obtain read-lock.";

	private const String CouldNotObtainWriteLock = "Could not obtain write-lock.";

	private Int64 _isReadOnly;

	/// <summary>Threadsafe item counter (so we don't have to enter and exit the readerwriter).</summary>
	[JsonIgnore]
	private Int64 _itemCount;

	/// <summary>Create an empty list with different timeout values.</summary>
	/// <param name="enumerable">  Fill the list with the given enumerable.</param>
	/// <param name="readTimeout">Defaults to 60 seconds.</param>
	/// <param name="writeTimeout">Defaults to 60 seconds.</param>
	public ConcurrentList( IEnumerable<T?>? enumerable = null, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) : base( nameof( ConcurrentList<T> ) ) {
		this.ReaderWriter = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
		this.TimeoutForReads = readTimeout ?? TimeSpan.FromSeconds( 60 );
		this.TimeoutForWrites = writeTimeout ?? TimeSpan.FromSeconds( 60 );

		if ( enumerable is not null ) {
			this.AddRange( enumerable );
		}
	}

	public ConcurrentList( Int32 capacity ) : this() => this.ResizeCapacity( capacity );

	private ConcurrentQueue<T> InputBuffer { get; } = new();

	[JsonIgnore]
	private ReaderWriterLockSlim ReaderWriter { get; }

	/// <summary>
	///     <para>The internal list actually used.</para>
	/// </summary>
	[JsonProperty]
	private List<T?> TheList { get; } = new();

	///// <summary>If set to DontThrowExceptions, anything that would normally cause an <see cref="Exception" /> is ignored.</summary>
	//public ThrowSetting ThrowExceptions { get; set; } = ThrowSetting.Throw;

	[JsonProperty]
	public TimeSpan TimeoutForReads { get; set; }

	[JsonProperty]
	public TimeSpan TimeoutForWrites { get; set; }

	/// <summary>
	///     <para>Count of items currently in this <see cref="ConcurrentList{TType}" />.</para>
	/// </summary>
	[JsonIgnore]
	public Int32 Count => ( Int32 ) Interlocked.Read( ref this._itemCount );

	/// <summary>Get or set if the list is read-only. (Readonly will prevent modifications to list.)</summary>
	public Boolean IsReadOnly {
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		get => Interlocked.Read( ref this._isReadOnly ) == 1;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		set => Interlocked.Exchange( ref this._isReadOnly, value ? 1 : 0 );
	}

	/// <summary>Gets or sets the element at the specified index.</summary>
	/// <returns>The element at the specified index.</returns>
	/// <param name="index">The zero-based index of the element to get or set.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	///     <paramref name="index" /> is not a valid index in the
	///     <see cref="IList" />.
	/// </exception>
	/// <exception cref="NotSupportedException">The property is set and the <see cref="IList" /> is read-only.</exception>
	public T? this[ Int32 index ] {
		[CanBeNull]
		get {
			if ( index < 0 || index > this.TheList.Count ) {
				this.ThrowOutOfRange( index );
			}

			return this.Read( () => this.TheList[ index ] );
		}

		set {
			this.ThrowOnReadOnly();

			this.Write( () => {
				this.ThrowOnReadOnly();

				try {
					this.TheList[ index ] = value;

					return true;
				}
				catch ( ArgumentOutOfRangeException ) {
					this.ThrowOutOfRange( index );
				}

				return false;
			} );
		}
	}

	/// <summary>
	///     <para>
	///         Add the
	///         <typeparam name="T">item</typeparam>
	///         to the end of this <see cref="ConcurrentList{TType}" />.
	///     </para>
	/// </summary>
	/// <param name="item"></param>
	public void Add( T? item ) => this.Add( item, null );

	/// <summary>Mark this <see cref="ConcurrentList{TType}" /> to be cleared.</summary>
	public void Clear() {
		this.ThrowOnReadOnly();

		this.Write( () => {
			this.TheList.Clear();
			this.ResetCount();

			return true;
		} );
	}

	/// <summary>
	///     <para>
	///         Determines whether the <paramref name="item" /> is in this <see cref="ConcurrentList{TType}" /> at this
	///         moment in time.
	///     </para>
	/// </summary>
	public Boolean Contains( T? item ) => this.Read( () => this.TheList.Contains( item ) );

	/// <summary>
	///     Copies the entire <see cref="ConcurrentList{TType}" /> to the <paramref name="array" />, starting at the
	///     specified index in the target array.
	/// </summary>
	/// <param name="array">     </param>
	/// <param name="arrayIndex"></param>
	public void CopyTo( T?[] array, Int32 arrayIndex ) {
		if ( array is null ) {
			throw new NullException( nameof( array ) );
		}

		this.Read( () => {
			this.TheList.CopyTo( array, arrayIndex );

			return true;
		} );
	}

	/// <summary>
	///     <para>
	///         Returns an enumerator that iterates through a <see cref="Clone" /> of this
	///         <see cref="ConcurrentList{TType}" /> .
	///     </para>
	/// </summary>
	public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator();

	/// <summary>
	///     <para>
	///         Searches at this moment in time for the first occurrence of <paramref name="item" /> and returns the
	///         zero-based index, or -1 if not found.
	///     </para>
	/// </summary>
	/// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}" />.</param>
	public Int32 IndexOf( T? item ) => this.Read( () => this.TheList.IndexOf( item ) );

	//is this the proper way?
	/// <summary>
	///     <para>
	///         Requests an insert of the <paramref name="item" /> into this <see cref="ConcurrentList{TType}" /> at the
	///         specified <paramref name="index" />.
	///     </para>
	/// </summary>
	/// <param name="index"></param>
	/// <param name="item"> </param>
	public void Insert( Int32 index, T? item ) {
		this.ThrowOnReadOnly();

		this.Write( () => {
			try {
				this.TheList.Insert( index, item );
				this.AnItemHasBeenAdded();

				return true;
			}
			catch ( ArgumentOutOfRangeException ) {
				this.ThrowOutOfRange( index );

				return false;
			}
		} );
	}

	/// <summary>
	///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
	/// </summary>
	/// <param name="item"></param>
	public Boolean Remove( T? item ) => this.Remove( item, default( Action? ) );

	public void RemoveAt( Int32 index ) {
		if ( index < 0 ) {
			this.ThrowOutOfRange( index );

			return;
		}

		this.ThrowOnReadOnly();

		this.Write( () => {
			try {
				if ( index <= this.TheList.Count ) {
					this.TheList.RemoveAt( index );
					this.AnItemHasBeenRemoved();

					return true;
				}
			}
			catch ( ArgumentOutOfRangeException ) {
				this.ThrowOutOfRange( index );
			}

			return false;
		} );
	}

	/// <summary>Returns the enumerator of this list's <see cref="Clone" />.</summary>
	IEnumerator IEnumerable.GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

	private void AnItemHasBeenAdded() => Interlocked.Increment( ref this._itemCount );

	private void AnItemHasBeenRemoved( Action? action = null ) {
		Interlocked.Decrement( ref this._itemCount );
		action?.Execute();
	}

	/// <summary>
	///     TODO These need to be tested!!
	/// </summary>
	/// <param name="_"></param>
	[OnDeserialized]
	private void OnDeserialized( StreamingContext _ ) => this.ResetCount( this.TheList.Count );

	/// <summary>
	///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
	/// </summary>
	/// <typeparam name="TFuncResult"></typeparam>
	/// <param name="func"></param>
	/// <exception cref="NullException"></exception>
	/// <exception cref="ObjectDisposedException"></exception>
	private TFuncResult? Read<TFuncResult>( Func<TFuncResult?> func ) {
		if ( func == null ) {
			throw new NullException( nameof( func ) );
		}

		this.ThrowWhenDisposed();

		this.CatchUp();

		if ( this.ReaderWriter.TryEnterUpgradeableReadLock( this.TimeoutForReads ) ) {
			try {
				return func();
			}
			finally {
				this.ReaderWriter.ExitUpgradeableReadLock();
			}
		}

		throw new TimeoutException( CouldNotObtainReadLock );
	}

	private void ResetCount( Int32 toCount = default ) => Interlocked.Add( ref this._itemCount, -Interlocked.Read( ref this._itemCount ) + toCount );

	private Int32 ResizeCapacity( Int32 capacity ) => this.Write( () => this.TheList.Capacity = capacity );

	[DoesNotReturn]
	private static void ThrowWhenDisallowedModifications() => throw new InvalidOperationException( "List does not allow modifications." );

	/// <summary>
	///     <para>
	///         Throw <exception cref="ObjectDisposedException" /> when list has been disposed.
	///     </para>
	///     <para>Otherwise, true is returned when this object has been disposed.</para>
	/// </summary>
	/// <exception cref="ObjectDisposedException"></exception>
	private void ThrowWhenDisposed() {
		if ( this.IsDisposed ) {
			throw new ObjectDisposedException( $"This {nameof( ConcurrentList<T> )} has been disposed." );
		}
	}

	[DoesNotReturn]
	private static void ThrowOnNoReadLock() => throw new TimeoutException( CouldNotObtainReadLock );

	[DoesNotReturn]
	private static void ThrowOnNoWriteLock() => throw new TimeoutException( CouldNotObtainWriteLock );

	/// <summary>
	///     <para>
	///         <exception cref="IndexOutOfRangeException" /> will be thrown.
	///     </para>
	/// </summary>
	/// <param name="index"></param>
	private void ThrowOutOfRange( Int32 index ) {
		var message = $"The value {index} is out of range. (It must be between 0 and {this.Count}).";

		throw new IndexOutOfRangeException( message ).Log();
	}

	private void ThrowOnReadOnly() {
		if ( this.IsReadOnly ) {
			throw new InvalidOperationException( $"This {nameof( ConcurrentList<T> )} is set to read-only." );
		}
	}

	/// <summary>
	///     <para>Filter write requests through the <see cref="ReaderWriter" />.</para>
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="func">                         </param>
	/// <see cref="CatchUp" />
	/// <exception cref="NullException"></exception>
	/// <exception cref="ObjectDisposedException"></exception>
	/// <exception cref="TimeoutException"></exception>
	private TResult? Write<TResult>( Func<TResult?> func ) {
		if ( func is null ) {
			throw new NullException( nameof( func ) );
		}

		this.ThrowWhenDisposed();

		this.ThrowOnReadOnly();

		if ( this.ReaderWriter.TryEnterWriteLock( this.TimeoutForWrites ) ) {
			try {
				this.ThrowWhenDisposed();

				return func();
			}
			finally {
				this.ReaderWriter.ExitWriteLock();
			}
		}

		ThrowOnNoWriteLock();

		return default( TResult? );
	}

	/// <summary>
	///     <para>
	///         Add the
	///         <typeparam name="T">item</typeparam>
	///         to the end of this <see cref="ConcurrentList{TType}" />.
	///     </para>
	/// </summary>
	/// <param name="item">    </param>
	/// <param name="afterAdd"></param>
	public Boolean Add( T? item, Action? afterAdd ) {
		this.ThrowWhenDisposed();

		this.ThrowOnReadOnly();

		return this.Write( () => {
			try {
				this.ThrowWhenDisposed();
				this.TheList.Add( item );

				return true;
			}
			finally {
				this.AnItemHasBeenAdded();
				afterAdd?.Invoke();
			}
		} );
	}

	public Boolean AddAndWait( T? item ) => this.Add( item, this.CatchUp );

	/// <summary>Creates a hot task that needs to be awaited.</summary>
	/// <param name="item"></param>
	/// <param name="afterAdd"></param>
	public Task<Boolean> AddAsync( T? item, Action? afterAdd = null ) => Task.Run( () => this.TryAdd( item, afterAdd ) );

	/// <summary>Add a collection of items.</summary>
	/// <param name="items">          </param>
	/// <param name="useParallelism">
	///     Enables parallelization of the <paramref name="items" /> No guarantee of the final order
	///     of items.
	/// </param>
	/// <param name="afterEachAdd">   <see cref="Action" /> to perform after each add.</param>
	/// <param name="afterRangeAdded"><see cref="Action" /> to perform after range added.</param>
	/// <exception cref="NullException"></exception>
	public void AddRange( IEnumerable<T?> items, Byte useParallelism = 0, Action? afterEachAdd = null, Action? afterRangeAdded = null ) {
		this.ThrowWhenDisposed();

		this.ThrowOnReadOnly();

		try {
			if ( useParallelism.Any() ) {
				items.AsParallel().WithDegreeOfParallelism( useParallelism ).ForAll( item => this.TryAdd( item, afterEachAdd ) );
			}
			else {
				foreach ( var item in items ) {
					this.TryAdd( item, afterEachAdd );
				}
			}
		}
		finally {
			this.TrimExcess();
			this.ThrowWhenDisposed();

			afterRangeAdded?.Invoke();
		}
	}

	/// <summary>Returns a hot task that needs to be awaited.</summary>
	/// <param name="items"></param>
	/// <param name="cancellationToken"></param>
	/// <param name="afterEachAdd"></param>
	/// <param name="afterRangeAdded"></param>
	/// <param name="useParallelism"></param>
	public Task AddRangeAsync(
		IEnumerable<T>? items,
		CancellationToken cancellationToken,
		Action? afterEachAdd = null,
		Action? afterRangeAdded = null,
		Byte useParallelism = 0
	) =>
		Task.Run( () => {
			this.ThrowWhenDisposed();

			if ( items != null ) {
				this.AddRange( items, useParallelism, afterEachAdd, afterRangeAdded );
			}
		}, cancellationToken );

	/// <summary>
	///     Returns true if any items have not been added to the list yet.
	/// </summary>
	/// <see cref="CatchUp" />
	public Boolean AnyWritesPending() => this.InputBuffer.Any();

	/// <summary>
	///     Blocks, transfers items from <see cref="InputBuffer" />, and then releases write lock.
	/// </summary>
	public void CatchUp() {
		this.ThrowWhenDisposed();

		if ( this.IsReadOnly || !this.AnyWritesPending() ) {
			return;
		}

		if ( this.ReaderWriter.TryEnterWriteLock( this.TimeoutForWrites ) ) {
			try {
				this.ThrowWhenDisposed();

				while ( this.InputBuffer.TryDequeue( out var item ) ) {
					if ( this.IsReadOnly ) {
						return;
					}

					this.ThrowWhenDisposed();

					try {
						this.TheList.Add( item );
					}
					finally {
						this.AnItemHasBeenAdded();
					}
				}
			}
			finally {
				this.ReaderWriter.ExitWriteLock();
			}
		}

		ThrowOnNoWriteLock();
	}

	/// <summary>
	///     <para>Returns a copy of this <see cref="ConcurrentList{TType}" /> (at this moment).</para>
	/// </summary>
	public ConcurrentList<T> Clone() {
		this.CatchUp();

		return new ConcurrentList<T>( this.Read( () => this.TheList ) );
	}

	/// <summary>
	///     Signal that this <see cref="ConcurrentList{TType}" /> will not be modified any more.
	///     <para>Blocking.</para>
	/// </summary>
	/// <see cref="IsReadOnly" />
	public void Complete() {
		try {
			this.CatchUp();
			this.TrimExcess();
		}
		finally {
			this.IsReadOnly = true;
		}
	}

	public override void DisposeManaged() {
		//nothing to do.. yet.
	}

	/// <summary>
	///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
	/// </summary>
	/// <param name="item">        </param>
	/// <param name="afterRemoval"></param>
	public Boolean Remove( T? item, Action? afterRemoval ) {
		this.ThrowWhenDisposed();

		if ( this.IsReadOnly ) {
			return false;
		}

		return this.Write( () => {
			var result = this.TheList.Remove( item );

			if ( result ) {
				this.AnItemHasBeenRemoved( afterRemoval );
			}

			return result;
		} );
	}

	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override String ToString() {
		var wiki = this.Take( 30 ).ToListTrimExcess();
		return $"{wiki.ToStrings( ", ", this.Count > 30 ? "..." : String.Empty )}";
	}

	/// <summary>The <see cref="List{T}.Capacity" /> is resized down to the <see cref="List{T}.Count" />.</summary>
	public void TrimExcess() =>
		this.Write( () => {
			this.TheList.TrimExcess();

			return true;
		} );

	public Boolean TryAdd( T? item, Action? afterAdd = null ) => this.Add( item, afterAdd );

	/// <summary>
	///     Returns true if there are no more incoming items.
	/// </summary>
	/// <param name="timeout"></param>
	public Boolean TryCatchup( TimeSpan? timeout = default ) {
		if ( this.IsReadOnly || this.IsDisposed ) {
			return true;
		}

		if ( this.ReaderWriter.TryEnterWriteLock( timeout ?? this.TimeoutForWrites ) ) {
			try {
				while ( !this.IsReadOnly && !this.IsDisposed && this.InputBuffer.TryDequeue( out var bob ) ) {
					this.TheList.Add( bob );
					this.AnItemHasBeenAdded();
				}
			}
			finally {
				this.ReaderWriter.ExitWriteLock();
			}
		}

		return !this.AnyWritesPending();
	}

	/// <summary>
	///     <para>Try to get an item in this <see cref="ConcurrentList{TType}" /> by index.</para>
	///     <para>Returns true if the request was posted to the internal dataflow.</para>
	/// </summary>
	/// <param name="index">   </param>
	/// <param name="afterGet">Action to be ran after the item at the <paramref name="index" /> is got.</param>
	public Boolean TryGet( Int32 index, Action<T?>? afterGet ) {
		if ( index < 0 ) {
			return false;
		}

		return this.Read( () => {
			if ( index >= this.TheList.Count ) {
				this.ThrowOutOfRange( index );

				return false;
			}

			var result = this.TheList[ index ];
			afterGet?.Invoke( result );

			return true;
		} );
	}

}