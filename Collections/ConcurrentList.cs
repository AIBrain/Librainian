// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentList.cs" belongs to Rick@AIBrain.org and
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
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
//
// ***  Project "Librainian"  ***
// File "ConcurrentList.cs" was last formatted by Protiguous on 2018/06/04 at 3:42 PM.

namespace Librainian.Collections {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.Serialization;
	using System.Threading;
	using System.Threading.Tasks;
	using Extensions;
	using FluentAssertions;
	using JetBrains.Annotations;
	using Magic;
	using Maths;
	using Newtonsoft.Json;
	using Threading;

	/// <summary>
	///     <para>A thread safe list. Uses a <see cref="ConcurrentQueue{T}" /> to buffer adds.</para>
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	/// <remarks>
	///     <para>This class was created on a spur of the moment idea, and is <b>thoroughly</b> UNTESTED.</para>
	/// </remarks>
	/// <copyright>
	///     Protiguous
	/// </copyright>
	[JsonObject]
	[DebuggerDisplay( "Count={" + nameof( Count ) + "}" )]
	public class ConcurrentList<TType> : ABetterClassDispose, IList<TType>, IEquatable<IEnumerable<TType>> {

		[JsonIgnore]
		private ConcurrentQueue<TType> InputBuffer { get; set; }

		/// <summary> threadsafe item counter (so we don't have to enter and exit the readerwriter). </summary>
		private ThreadLocal<Int32> ItemCounter { get; set; } = new ThreadLocal<Int32>( valueFactory: () => 0, trackAllValues: true );

		[JsonIgnore]
		private ReaderWriterLockSlim ReaderWriter { get; set; } = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );

		/// <summary>
		///     <para>The internal list actually used.</para>
		/// </summary>
		[NotNull]
		[JsonProperty]
		private List<TType> TheList { get; set; } = new List<TType>();

		private void AnItemHasBeenAdded() => this.ItemCounter.Value++;

		private void AnItemHasBeenRemoved( [CanBeNull] Action action = null ) {
			this.ItemCounter.Value--;
			action?.Invoke();
		}

		[OnDeserialized]
		private void OnDeserialized( StreamingContext context ) {
			if ( !this.TimeoutForWrites.HasValue ) { this.TimeoutForWrites = TimeSpan.FromMinutes( 1 ); }

			if ( !this.TimeoutForReads.HasValue ) { this.TimeoutForReads = TimeSpan.FromMinutes( 1 ); }

			if ( this.ReaderWriter is null ) { this.ReaderWriter = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion ); }

			//if ( null == this.TheList ) { this.TheList = new List<TType>(); }

			if ( this.InputBuffer is null ) { this.InputBuffer = new ConcurrentQueue<TType>(); }

			if ( this.ItemCounter is null ) { this.ItemCounter = new ThreadLocal<Int32>( valueFactory: () => 0, trackAllValues: true ); }

			this.ItemCounter.Value += this.TheList.Count;
		}

		/// <summary>
		///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
		/// </summary>
		/// <typeparam name="TFuncResult"></typeparam>
		/// <param name="func"></param>
		/// <returns></returns>
		[CanBeNull]
		private TFuncResult Read<TFuncResult>( [CanBeNull] Func<TFuncResult> func ) {
			if ( !this.AllowModifications() && func != null ) {
				return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
			}

			this.CatchUp();

			if ( !this.ReaderWriter.TryEnterUpgradeableReadLock( timeout: this.TimeoutForReads ?? TimeSpan.FromMinutes( 1 ) ) ) { return default; }

			try {
				if ( func != null ) { return func(); }
			}
			finally { this.ReaderWriter.ExitUpgradeableReadLock(); }

			return default;
		}

		/// <summary>
		///     <para>Filter write requests through the <see cref="ReaderWriter" />.</para>
		/// </summary>
		/// <typeparam name="TFuncResult"></typeparam>
		/// <param name="func">                         </param>
		/// <param name="ignoreAllowModificationsCheck"></param>
		/// <returns></returns>
		/// <seealso cref="CatchUp" />
		[CanBeNull]
		private TFuncResult Write<TFuncResult>( [CanBeNull] Func<TFuncResult> func, Boolean ignoreAllowModificationsCheck = false ) {
			if ( !ignoreAllowModificationsCheck && !this.AllowModifications() && func != null ) { return default; }

			if ( !this.ReaderWriter.TryEnterWriteLock( timeout: this.TimeoutForWrites ?? TimeSpan.FromMinutes( 1 ) ) ) { return default; }

			try {
				if ( func != null ) { return func(); }
			}
			finally { this.ReaderWriter.ExitWriteLock(); }

			return default;
		}

		/// <summary>
		///     <para>Count of items currently in this <see cref="ConcurrentList{TType}" />.</para>
		/// </summary>
		[JsonIgnore]
		public Int32 Count => this.ItemCounter.Values.Aggregate( seed: 0, func: ( current, variable ) => current + variable );

		/// <summary>
		/// </summary>
		/// <seealso cref="AllowModifications" />
		public Boolean IsReadOnly { get; private set; }

		[JsonProperty]
		public TimeSpan? TimeoutForReads { get; set; } = TimeSpan.FromMinutes( 1 );

		[JsonProperty]
		public TimeSpan? TimeoutForWrites { get; set; } = TimeSpan.FromMinutes( 1 );

		/// <summary>
		///     Create an empty list with different timeout values.
		/// </summary>
		/// <param name="enumerable">  Fill the list with the given enumerable.</param>
		/// <param name="readTimeout"> </param>
		/// <param name="writeTimeout"></param>
		public ConcurrentList( [CanBeNull] IEnumerable<TType> enumerable = null, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) {
			this.InputBuffer = new ConcurrentQueue<TType>();

			if ( readTimeout.HasValue ) { this.TimeoutForReads = readTimeout.Value; }

			if ( writeTimeout.HasValue ) { this.TimeoutForWrites = writeTimeout.Value; }

			if ( null != enumerable ) { this.AddRange( items: enumerable ); }
		}

		/// <summary>
		///     Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>The element at the specified index.</returns>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///     <paramref name="index" /> is not a valid index in the
		///     <see cref="T:System.Collections.Generic.IList`1" />.
		/// </exception>
		/// <exception cref="T:System.NotSupportedException">
		///     The property is set and the
		///     <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
		/// </exception>
		[CanBeNull]
		public TType this[Int32 index] {
			[CanBeNull]
			get {
				if ( index >= 0 && this.TheList.Count <= index ) { return this.Read( func: () => this.TheList[index: index] ); }

				return default;
			}

			set {
				if ( !this.AllowModifications() ) { return; }

				this.Write( func: () => {
					if ( !this.AllowModifications() ) { return false; }

					this.TheList[index: index] = value;

					return true;
				} );
			}
		}

		/// <summary>
		///     Static comparison function.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="rhs"> </param>
		/// <returns></returns>
		public static Boolean Equals( IEnumerable<TType> left, IEnumerable<TType> rhs ) {
			if ( ReferenceEquals( left, rhs ) ) { return true; }

			if ( left is null || rhs is null ) { return false; }

			return left.SequenceEqual( second: rhs );
		}

		/// <summary>
		///     <para>
		///         Add the
		///         <typeparam name="TType">item</typeparam>
		///         to the end of this <see cref="ConcurrentList{TType}" />.
		///     </para>
		/// </summary>
		/// <param name="item"></param>
		public void Add( TType item ) => this.Add( item: item, afterAdd: null );

		/// <summary>
		///     <para>
		///         Add the
		///         <typeparam name="TType">item</typeparam>
		///         to the end of this <see cref="ConcurrentList{TType}" />.
		///     </para>
		/// </summary>
		/// <param name="item">    </param>
		/// <param name="afterAdd"></param>
		/// <returns></returns>
		public Boolean Add( TType item, [CanBeNull] Action afterAdd ) {
			if ( !this.AllowModifications() ) { return false; }

			return this.Write( func: () => {
				try {
					this.TheList.Add( item: item );

					return true;
				}
				finally {
					this.AnItemHasBeenAdded();
					afterAdd?.Invoke();
				}
			} );
		}

		public Boolean AddAndWait( TType item ) => this.Add( item: item, afterAdd: this.CatchUp );

		public async Task<Boolean> AddAsync( TType item, [CanBeNull] Action afterAdd = null ) => await Task.Run( function: () => this.TryAdd( item: item, afterAdd: afterAdd ) );

		/// <summary>
		///     Add a collection of items.
		/// </summary>
		/// <param name="items">          </param>
		/// <param name="useParallelism">
		///     Enables parallelization of the <paramref name="items" /> No guarantee of the final order
		///     of items.
		/// </param>
		/// <param name="afterEachAdd">   <see cref="Action" /> to perform after each add.</param>
		/// <param name="afterRangeAdded"><see cref="Action" /> to perform after range added.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void AddRange( [NotNull] IEnumerable<TType> items, Byte useParallelism = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) {
			if ( null == items ) { throw new ArgumentNullException( nameof( items ) ); }

			if ( !this.AllowModifications() ) { return; }

			try {
				if ( useParallelism >= Environment.ProcessorCount ) {
					items.AsParallel().WithDegreeOfParallelism( degreeOfParallelism: useParallelism ).ForAll( item => this.TryAdd( item: item, afterAdd: afterEachAdd ) );
				}
				else {
					foreach ( var item in items ) { this.TryAdd( item: item, afterAdd: afterEachAdd ); }
				}
			}
			finally { afterRangeAdded?.Invoke(); }
		}

		public async Task AddRangeAsync( [CanBeNull] IEnumerable<TType> items, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) =>
			await Task.Run( () => {
				if ( items != null ) { this.AddRange( items: items, afterEachAdd: afterEachAdd, afterRangeAdded: afterRangeAdded ); }
			} ).NoUI();

		/// <summary>
		///     Returns true if this <see cref="ConcurrentList{TType}" /> has not been marked as <see cref="Complete" />.
		/// </summary>
		public Boolean AllowModifications() => !this.IsReadOnly;

		/// <summary>
		/// </summary>
		/// <seealso cref="CatchUp" />
		public Boolean AnyWritesPending() => this.InputBuffer?.Any() == true;

		/// <summary>
		///     Blocks, transfers items from <see cref="InputBuffer" />, and then releases lock.
		/// </summary>
		public void CatchUp() {
			if ( !this.AnyWritesPending() ) { return; }

			try {
				this.ReaderWriter.EnterWriteLock();

				while ( this.InputBuffer.TryDequeue( result: out var bob ) ) {
					this.TheList.Add( item: bob );
					this.AnItemHasBeenAdded();
				}
			}
			finally { this.ReaderWriter.ExitWriteLock(); }
		}

		/// <summary>
		///     Mark this <see cref="ConcurrentList{TType}" /> to be cleared.
		/// </summary>
		public void Clear() {
			if ( !this.AllowModifications() ) { return; }

			this.Write( func: () => {
				this.TheList.Clear();
				this.ItemCounter = new ThreadLocal<Int32>( valueFactory: () => 0, trackAllValues: true ); //BUG is this wrong? how else do we reset all the counters?

				return true;
			} );
		}

		/// <summary>
		///     <para>Returns a copy of this <see cref="ConcurrentList{TType}" /> at this moment in time.</para>
		/// </summary>
		/// <returns></returns>
		[CanBeNull]
		public IEnumerable<TType> Clone() => this.Read( func: () => this.TheList.ToList() );

		/// <summary>
		///     Signal that this <see cref="ConcurrentList{TType}" /> will not be modified any more.
		///     <para>Blocks.</para>
		/// </summary>
		/// <seealso cref="AllowModifications" />
		/// <seealso cref="IsReadOnly" />
		public void Complete() {
			try {
				this.CatchUp();
				this.FixCapacity();
			}
			finally {
				this.IsReadOnly = true;
				this.AllowModifications().Should().BeFalse();
			}
		}

		/// <summary>
		///     <para>
		///         Determines whether the <paramref name="item" /> is in this <see cref="ConcurrentList{TType}" /> at this
		///         moment in time.
		///     </para>
		/// </summary>
		public Boolean Contains( TType item ) => this.Read( func: () => this.TheList.Contains( item: item ) );

		/// <summary>
		///     Copies the entire <see cref="ConcurrentList{TType}" /> to the <paramref name="array" />, starting at the specified
		///     index in the target array.
		/// </summary>
		/// <param name="array">     </param>
		/// <param name="arrayIndex"></param>
		public void CopyTo( TType[] array, Int32 arrayIndex ) {
			if ( array is null ) { throw new ArgumentNullException( nameof( array ) ); }

			this.Read( func: () => {
				this.TheList.CopyTo( array: array, arrayIndex: arrayIndex );

				return true;
			} );
		}

		/// <summary>
		///     Dispose any disposable members.
		/// </summary>
		public override void DisposeManaged() {
			this.ReaderWriter?.Dispose();
			this.ItemCounter?.Dispose();
		}

		public Boolean Equals( IEnumerable<TType> other ) => Equals( left: this, rhs: other );

		/// <summary>
		///     The <seealso cref="List{T}.Capacity" /> is resized down to the <seealso cref="List{T}.Count" />.
		/// </summary>
		public void FixCapacity() =>
			this.Write( func: () => {
				this.TheList.Capacity = this.TheList.Count;

				return true;
			} );

		/// <summary>
		///     <para>
		///         Returns an enumerator that iterates through a <see cref="Clone" /> of this
		///         <see cref="ConcurrentList{TType}" /> .
		///     </para>
		/// </summary>
		/// <returns></returns>
		public IEnumerator<TType> GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

		/// <summary>
		///     <para>
		///         Searches at this moment in time for the first occurrence of <paramref name="item" /> and returns the
		///         zero-based index, or -1 if not found.
		///     </para>
		/// </summary>
		/// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}" />.</param>
		public Int32 IndexOf( TType item ) => this.Read( func: () => this.TheList.IndexOf( item: item ) );

		/// <summary>
		///     <para>
		///         Requests an insert of the <paramref name="item" /> into this <see cref="ConcurrentList{TType}" /> at the
		///         specified <paramref name="index" />.
		///     </para>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"> </param>
		public void Insert( Int32 index, TType item ) {
			if ( !this.AllowModifications() ) { return; }

			this.Write( func: () => {
				try {
					this.TheList.Insert( index: index, item: item );
					this.AnItemHasBeenAdded();

					return true;
				}
				catch ( ArgumentOutOfRangeException ) { return false; }
			} );
		}

		/// <summary>
		///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public Boolean Remove( TType item ) => this.Remove( item: item, afterRemoval: null );

		/// <summary>
		///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
		/// </summary>
		/// <param name="item">        </param>
		/// <param name="afterRemoval"></param>
		/// <returns></returns>
		public Boolean Remove( TType item, [CanBeNull] Action afterRemoval ) {
			if ( !this.AllowModifications() ) { return false; }

			return this.Write( func: () => {
				var result = this.TheList.Remove( item: item );

				if ( result ) { this.AnItemHasBeenRemoved( afterRemoval ); }

				return result;
			} );
		}

		public void RemoveAt( Int32 index ) {
			index.Should().BeGreaterOrEqualTo( expected: 0 );

			if ( index < 0 ) { return; }

			if ( !this.AllowModifications() ) { return; }

			this.Write( func: () => {
				try {
					index.Should().BeLessOrEqualTo( expected: this.TheList.Count );

					if ( index < this.TheList.Count ) {
						this.TheList.RemoveAt( index: index );
						this.AnItemHasBeenRemoved();
					}
				}
				catch ( ArgumentOutOfRangeException ) { return false; }

				return true;
			} );
		}

		/// <summary>
		///     Harker's shuffle version. Untested!
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="iterations">      </param>
		/// <param name="howLong">         </param>
		/// <param name="orUntilCancelled"></param>
		[Experimental( "Untested" )]
		public UInt32 Shuffle( Int32 iterations = 1, TimeSpan? howLong = null, SimpleCancel orUntilCancelled = null ) {
			var stopWatch = Stopwatch.StartNew();

			if ( orUntilCancelled is null ) { orUntilCancelled = new SimpleCancel(); }

			var counter = 0U;
			var itemCount = this.Count;

			if ( iterations < 1 ) { iterations = 1; }

			iterations *= itemCount;

			do {
				var a = 0.Next( maxValue: itemCount );
				var b = 0.Next( maxValue: itemCount );
				var temp = this[index: a];
				this[index: a] = this[index: b];
				this[index: b] = temp;
				--iterations;
				counter++;

				if ( howLong.HasValue && stopWatch.Elapsed > howLong.Value ) { orUntilCancelled.RequestCancel(); }

				if ( !iterations.Any() ) { orUntilCancelled.RequestCancel(); }
			} while ( !orUntilCancelled.HaveAnyCancellationsBeenRequested() );

			return counter;
		}

		public Boolean TryAdd( TType item, [CanBeNull] Action afterAdd = null ) => this.Add( item: item, afterAdd: afterAdd );

		public Boolean TryCatchup( TimeSpan timeout ) {
			if ( !this.AnyWritesPending() ) { return true; }

			var gotLock = false;

			try {
				if ( !this.ReaderWriter.TryEnterWriteLock( timeout: timeout ) ) { return false; }

				gotLock = true;

				while ( this.InputBuffer.TryDequeue( result: out var bob ) ) {
					this.TheList.Add( item: bob );
					this.AnItemHasBeenAdded();
				}

				return true;
			}
			finally {
				if ( gotLock ) { this.ReaderWriter.ExitWriteLock(); }
			}
		}

		/// <summary>
		///     <para>Try to get an item in this <see cref="ConcurrentList{TType}" /> by index.</para>
		///     <para>Returns true if the request was posted to the internal dataflow.</para>
		/// </summary>
		/// <param name="index">   </param>
		/// <param name="afterGet">Action to be ran after the item at the <paramref name="index" /> is got.</param>
		public Boolean TryGet( Int32 index, [CanBeNull] Action<TType> afterGet ) {
			if ( index < 0 ) { return false; }

			return this.Read( func: () => {
				if ( index >= this.TheList.Count ) { return false; }

				var result = this.TheList[index: index];
				afterGet?.Invoke( result );

				return true;
			} );
		}

		IEnumerator IEnumerable.GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

		//[OnDeserializing]
		//private void OnDeserializing( StreamingContext context ) => this.CatchUp();

		//[OnSerialized]
		//private void OnSerialized( StreamingContext context ) {
		//}
	}
}