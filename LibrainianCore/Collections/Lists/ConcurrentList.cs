// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentList.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "LibrainianCore", File: "ConcurrentList.cs" was last formatted by Protiguous on 2020/03/16 at 3:03 PM.

namespace Librainian.Collections.Lists {

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
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Newtonsoft.Json;
    using Threading;
    using Utilities;

    /// <summary>
    ///     <para>A thread safe generic list.</para>
    ///     <para>Use at your own risk.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     <para>This class was created on a spur of the moment idea, and is <b>THOROUGHLY UNTESTED™</b>.</para>
    ///     <para>Uses a <see cref="ConcurrentQueue{T}" /> to buffer adds.</para>
    /// </remarks>
    /// <copyright>Protiguous@Protiguous.com</copyright>
    [JsonObject( MemberSerialization.Fields )]
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    public class ConcurrentList<T> : ABetterClassDispose, IList<T>, IPossibleThrowable /*, IEquatable<IEnumerable<T>>*/ {

        private volatile Boolean _isReadOnly;

        /// <summary>Threadsafe item counter (so we don't have to enter and exit the readerwriter).</summary>
        [JsonIgnore]
        private Int64 ItemCount;

        [NotNull]
        private ConcurrentQueue<T> InputBuffer { get; set; } = new ConcurrentQueue<T>();

        [JsonIgnore]
        [NotNull]
        private ReaderWriterLockSlim ReaderWriter { get; }

        /// <summary>
        ///     <para>The internal list actually used.</para>
        /// </summary>
        [NotNull]
        [JsonProperty]
        private List<T> TheList { get; set; } = new List<T>();

        /// <summary>
        ///     <para>Count of items currently in this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        [JsonIgnore]
        public Int32 Count => ( Int32 )Interlocked.Read( ref this.ItemCount );

        /// <summary>Get or set if the list is .</summary>
        /// <see cref="AllowModifications" />
        public Boolean IsReadOnly {
            get => this._isReadOnly;

            private set => this._isReadOnly = value;
        }

        /// <summary>If set to DontThrowExceptions, anything that would normally cause an <see cref="Exception" /> is ignored.</summary>
        public ThrowSetting ThrowExceptions { get; set; } = ThrowSetting.Throw;

        [JsonProperty]
        public TimeSpan TimeoutForReads { get; set; }

        [JsonProperty]
        public TimeSpan TimeoutForWrites { get; set; }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not a valid index in the <see cref="IList" />.</exception>
        /// <exception cref="NotSupportedException">The property is set and the <see cref="IList" /> is read-only.</exception>
        [CanBeNull]
        public T this[ Int32 index ] {
            [CanBeNull]
            get {
                if ( index < 0 ) {
                    this.ThrowOutOfRange( index );

                    return default;
                }

                if ( index > this.TheList.Count ) {
                    this.ThrowOutOfRange( index );

                    return default;
                }

                return this.Read( () => this.TheList[ index ] );
            }

            set {
                if ( !this.AllowModifications() ) {
                    this.IfDisallowedModificationsThrow();

                    return;
                }

                this.Write( () => {
                    if ( !this.AllowModifications() ) {
                        this.IfDisallowedModificationsThrow();

                        return default;
                    }

                    try {
                        this.TheList[ index ] = value;

                        return true;
                    }
                    catch ( ArgumentOutOfRangeException ) {
                        this.ThrowOutOfRange( index );
                    }

                    return default;
                } );
            }
        }

        /// <summary>Create an empty list with different timeout values.</summary>
        /// <param name="enumerable">  Fill the list with the given enumerable.</param>
        /// <param name="readTimeout">Defaults to 60 seconds.</param>
        /// <param name="writeTimeout">Defaults to 60 seconds.</param>
        public ConcurrentList( [CanBeNull] IEnumerable<T> enumerable = null, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) {

            this.ReaderWriter = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            this.TimeoutForReads = readTimeout ?? TimeSpan.FromSeconds( value: 60 );
            this.TimeoutForWrites = writeTimeout ?? TimeSpan.FromSeconds( value: 60 );

            if ( !( enumerable is null ) ) {
                this.AddRange( enumerable );
            }
        }

        public ConcurrentList( Int32 initialCapacity ) : this() => this.TheList.Capacity = initialCapacity;

        private void AnItemHasBeenAdded() => Interlocked.Increment( ref this.ItemCount );

        private void AnItemHasBeenRemoved( [CanBeNull] Action action = null ) {
            Interlocked.Decrement( ref this.ItemCount );
            action?.Execute();
        }

        private void IfDisallowedModificationsThrow() {
            if ( this.ThrowExceptions == ThrowSetting.Throw ) {
                throw new InvalidOperationException( message: "List does not allow modifications." );
            }
        }

        [OnDeserialized]
        private void OnDeserialized( StreamingContext context ) {

            //These need to be tested for null when deserializing.

            //if ( this.ReaderWriter is null ) {
            //    this.ReaderWriter = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );
            //}

            this.ResetCount();
            Interlocked.Add( ref this.ItemCount, this.TheList.Count );
        }

        /// <summary>
        ///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        [CanBeNull]
        private TFuncResult Read<TFuncResult>( [NotNull] Func<TFuncResult> func ) {
            if ( func is null ) {
                throw new ArgumentNullException( nameof( func ) );
            }

            this.ThrowIfDisposed();

            if ( !this.AllowModifications() ) {
                return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
            }

            this.CatchUp();

            if ( this.ReaderWriter.TryEnterUpgradeableReadLock( this.TimeoutForReads ) ) {
                try {
                    return func();
                }
                finally {
                    this.ReaderWriter.ExitUpgradeableReadLock();
                }
            }

            return default;
        }

        private void ResetCount() => Interlocked.Add( ref this.ItemCount, -Interlocked.Read( ref this.ItemCount ) );

        /// <summary></summary>
        /// <exception cref="ObjectDisposedException"></exception>
        private void ThrowIfDisposed() {
            if ( this.IsDisposed && this.ThrowExceptions == ThrowSetting.Throw ) {
                throw new ObjectDisposedException( $"This {nameof( ConcurrentList<T> )} has been disposed." );
            }
        }

        private void ThrowOutOfRange( Int32 index ) {
            var message = $"The value {index.ToString()} is out of range. (It must be 0 or greater).";
            message.Log();

            if ( this.ThrowExceptions == ThrowSetting.Throw ) {
                throw new ArgumentOutOfRangeException( nameof( index ), index.ToString(), message );
            }
        }

        /// <summary>
        ///     <para>Filter write requests through the <see cref="ReaderWriter" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func">                         </param>
        /// <param name="ignoreAllowModificationsCheck"></param>
        /// <returns></returns>
        /// <see cref="CatchUp" />
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        [CanBeNull]
        private TFuncResult Write<TFuncResult>( [NotNull] Func<TFuncResult> func, Boolean ignoreAllowModificationsCheck = false ) {
            if ( func is null ) {
                throw new ArgumentNullException( nameof( func ) );
            }

            this.ThrowIfDisposed();

            if ( !ignoreAllowModificationsCheck ) {
                if ( !this.AllowModifications() ) {
                    return default;
                }
            }

            if ( this.ReaderWriter.TryEnterWriteLock( this.TimeoutForWrites ) ) {
                try {
                    this.ThrowIfDisposed();

                    return func();
                }
                finally {
                    this.ReaderWriter.ExitWriteLock();
                }
            }

            return default;
        }

        /// <summary>Static comparison function.</summary>
        /// <param name="left"></param>
        /// <param name="right"> </param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] IEnumerable<T> left, [CanBeNull] IEnumerable<T> right ) {
            if ( left is null || right is null ) {
                return default;
            }

            return ReferenceEquals( left, right ) || left.SequenceEqual( right );
        }

        /// <summary>
        ///     <para>Add the
        ///     <typeparam name="T">item</typeparam>
        ///     to the end of this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        /// <param name="item"></param>
        public void Add( T item ) => this.Add( item, afterAdd: null );

        /// <summary>
        ///     <para>Add the
        ///     <typeparam name="T">item</typeparam>
        ///     to the end of this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        /// <param name="item">    </param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        public Boolean Add( [CanBeNull] T item, [CanBeNull] Action afterAdd ) {
            this.ThrowIfDisposed();

            if ( !this.AllowModifications() ) {
                this.IfDisallowedModificationsThrow();

                return default;
            }

            return this.Write( () => {
                try {
                    this.ThrowIfDisposed();
                    this.TheList.Add( item );

                    return true;
                }
                finally {
                    this.AnItemHasBeenAdded();
                    afterAdd?.Invoke();
                }
            } );
        }

        public Boolean AddAndWait( [CanBeNull] T item ) => this.Add( item, this.CatchUp );

        /// <summary>Creates a hot task that needs to be awaited.</summary>
        /// <param name="item"></param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        [NotNull]
        public Task<Boolean> AddAsync( [CanBeNull] T item, [CanBeNull] Action afterAdd = null ) => Task.Run( () => this.TryAdd( item, afterAdd ) );

        /// <summary>Add a collection of items.</summary>
        /// <param name="items">          </param>
        /// <param name="useParallelism">Enables parallelization of the <paramref name="items" /> No guarantee of the final order of items.</param>
        /// <param name="afterEachAdd">   <see cref="Action" /> to perform after each add.</param>
        /// <param name="afterRangeAdded"><see cref="Action" /> to perform after range added.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange( [NotNull] IEnumerable<T> items, Byte useParallelism = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) {
            if ( items is null ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            this.ThrowIfDisposed();

            if ( !this.AllowModifications() ) {
                this.IfDisallowedModificationsThrow();

                return;
            }

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
                this.ThrowIfDisposed();
                afterRangeAdded?.Invoke();
            }
        }

        /// <summary>Returns a hot task that needs to be awaited.</summary>
        /// <param name="items"></param>
        /// <param name="token"></param>
        /// <param name="afterEachAdd"></param>
        /// <param name="afterRangeAdded"></param>
        /// <param name="useParallelism"></param>
        /// <returns></returns>
        [NotNull]
        public Task AddRangeAsync( [CanBeNull] IEnumerable<T> items, CancellationToken token, [CanBeNull] Action afterEachAdd = null,
            [CanBeNull] Action afterRangeAdded = null, Byte useParallelism = 0 ) =>
            Task.Run( () => {
                this.ThrowIfDisposed();

                if ( items != null ) {
                    this.AddRange( items, afterEachAdd: afterEachAdd, afterRangeAdded: afterRangeAdded, useParallelism: useParallelism );
                }
            }, token );

        /// <summary>Returns true if this <see cref="ConcurrentList{TType}" /> has not been marked as <see cref="Complete" />.</summary>
        public Boolean AllowModifications() => !this.IsReadOnly;

        /// <summary></summary>
        /// <see cref="CatchUp" />
        public Boolean AnyWritesPending() => this.InputBuffer.Any();

        /// <summary>Blocks, transfers items from <see cref="InputBuffer" />, and then releases write lock.</summary>
        public void CatchUp() {
            if ( !this.AnyWritesPending() ) {
                return;
            }

            this.ThrowIfDisposed();

            if ( this.ReaderWriter.TryEnterWriteLock( this.TimeoutForWrites ) ) {
                try {
                    this.ThrowIfDisposed();

                    while ( this.InputBuffer.TryDequeue( out var item ) ) {
                        this.ThrowIfDisposed();
                        this.TheList.Add( item );
                        this.AnItemHasBeenAdded();
                    }
                }
                finally {
                    this.ReaderWriter.ExitWriteLock();
                }
            }
        }

        /// <summary>Mark this <see cref="ConcurrentList{TType}" /> to be cleared.</summary>
        public void Clear() {
            if ( !this.AllowModifications() ) {
                this.IfDisallowedModificationsThrow();

                return;
            }

            this.Write( () => {
                this.TheList.Clear();
                this.ResetCount();

                return true;
            } );
        }

        /// <summary>
        ///     <para>Returns a copy of this <see cref="ConcurrentList{TType}" /> at this moment in time.</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public IList<T> Clone() => this.Read( () => this.TheList );

        /// <summary>Signal that this <see cref="ConcurrentList{TType}" /> will not be modified any more.
        /// <para>Blocking.</para>
        /// </summary>
        /// <see cref="AllowModifications" />
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

        /// <summary>
        ///     <para>Determines whether the <paramref name="item" /> is in this <see cref="ConcurrentList{TType}" /> at this moment in time.</para>
        /// </summary>
        public Boolean Contains( T item ) => this.Read( () => this.TheList.Contains( item ) );

        /// <summary>Copies the entire <see cref="ConcurrentList{TType}" /> to the <paramref name="array" />, starting at the specified index in the target array.</summary>
        /// <param name="array">     </param>
        /// <param name="arrayIndex"></param>
        public void CopyTo( T[] array, Int32 arrayIndex ) {
            if ( array is null ) {
                throw new ArgumentNullException( nameof( array ) );
            }

            this.Read( () => {
                this.TheList.CopyTo( array, arrayIndex );

                return true;
            } );
        }

        /// <summary>
        /// Dispose any disposable managed fields or properties.
        /// <para>
        /// Providing the object inside a using construct will then call <see cref="ABetterClassDispose.Dispose()" />, which in turn calls
        /// <see cref="ABetterClassDispose.DisposeManaged" /> and <see cref="ABetterClassDispose.DisposeNative" />.
        /// </para>
        /// </summary>
        public override void DisposeManaged() {
            this.Nop();
            this.Nop();

            //this.SetDisposeHint( $"{nameof( ConcurrentList<T> )}, count={this.Count}." );
        }

        /// <summary>
        ///     <para>Returns an enumerator that iterates through a <see cref="Clone" /> of this <see cref="ConcurrentList{TType}" /> .</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator();

        /// <summary>
        ///     <para>Searches at this moment in time for the first occurrence of <paramref name="item" /> and returns the zero-based index, or -1 if not found.</para>
        /// </summary>
        /// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}" />.</param>
        public Int32 IndexOf( T item ) => this.Read( () => this.TheList.IndexOf( item ) );

        //is this the proper way?
        /// <summary>
        ///     <para>Requests an insert of the <paramref name="item" /> into this <see cref="ConcurrentList{TType}" /> at the specified <paramref name="index" />.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"> </param>
        public void Insert( Int32 index, T item ) {
            if ( !this.AllowModifications() ) {
                this.IfDisallowedModificationsThrow();

                return;
            }

            this.Write( () => {
                try {
                    this.TheList.Insert( index, item );
                    this.AnItemHasBeenAdded();

                    return true;
                }
                catch ( ArgumentOutOfRangeException ) {
                    this.ThrowOutOfRange( index );

                    return default;
                }
            } );
        }

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove( T item ) => this.Remove( item, afterRemoval: default );

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item">        </param>
        /// <param name="afterRemoval"></param>
        /// <returns></returns>
        public Boolean Remove( [CanBeNull] T item, [CanBeNull] Action afterRemoval ) {
            if ( !this.AllowModifications() ) {
                this.IfDisallowedModificationsThrow();

                return default;
            }

            this.ThrowIfDisposed();

            return this.Write( () => {
                var result = this.TheList.Remove( item );

                if ( result ) {
                    this.AnItemHasBeenRemoved( afterRemoval );
                }

                return result;
            } );
        }

        public void RemoveAt( Int32 index ) {

            if ( index < 0 ) {

                if ( this.ThrowExceptions == ThrowSetting.Throw ) {
                    throw new ArgumentOutOfRangeException( nameof( index ), index, message: "Value must be 0 or greater." );
                }

                return;
            }

            if ( !this.AllowModifications() ) {
                this.IfDisallowedModificationsThrow();

                return;
            }

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

                return default;
            } );
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        [NotNull]
        public override String ToString() => $"{this.Take( count: 30 ).ToStrings( this.Count > 30 ? "..." : String.Empty )}";

        /// <summary>The <see cref="List{T}.Capacity" /> is resized down to the <see cref="List{T}.Count" />.</summary>
        public void TrimExcess() =>
            this.Write( () => {
                this.TheList.TrimExcess();

                return true;
            } );

        public Boolean TryAdd( [CanBeNull] T item, [CanBeNull] Action afterAdd = null ) => this.Add( item, afterAdd );

        public Boolean TryCatchup( TimeSpan timeout ) {
            if ( !this.AnyWritesPending() ) {
                return true;
            }

            if ( this.ReaderWriter.TryEnterWriteLock( timeout ) ) {
                try {
                    while ( this.InputBuffer.TryDequeue( out var bob ) ) {
                        this.TheList.Add( bob );
                        this.AnItemHasBeenAdded();
                    }

                    return true;
                }
                finally {
                    this.ReaderWriter.ExitWriteLock();
                }
            }

            return default;
        }

        /// <summary>
        ///     <para>Try to get an item in this <see cref="ConcurrentList{TType}" /> by index.</para>
        ///     <para>Returns true if the request was posted to the internal dataflow.</para>
        /// </summary>
        /// <param name="index">   </param>
        /// <param name="afterGet">Action to be ran after the item at the <paramref name="index" /> is got.</param>
        public Boolean TryGet( Int32 index, [CanBeNull] Action<T> afterGet ) {
            if ( index < 0 ) {
                return default;
            }

            return this.Read( () => {
                if ( index >= this.TheList.Count ) {
                    this.ThrowOutOfRange( index );

                    return default;
                }

                var result = this.TheList[ index ];
                afterGet?.Invoke( result );

                return true;
            } );
        }

        /// <summary>Returns the enumerator of this list's <see cref="Clone" />.</summary>
        /// <returns></returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

        /*

        /// <summary>
        ///     Returns true if <paramref name="other" /> is equal to this List.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals( IEnumerable<T> other ) => Equals( this, other );
        */
        /*

        /// <summary>
        ///     <para>Harker Shuffle Algorithm</para>
        ///     <para>
        ///         Not cryptographically guaranteed or tested to be the most performant, but it *should* shuffle *well enough*
        ///         in reasonable time.
        ///     </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iterations">At least 1 iterations to be done over the whole list.</param>
        /// <param name="forHowLong">Or for how long to run.</param>
        /// <param name="token">Or until cancelled.</param>
        public void ShuffleByHarker( Int32 iterations = 1, TimeSpan? forHowLong = null, CancellationToken? token = null ) =>
            this.Write( () => {

                Stopwatch started = null;

                if ( forHowLong.HasValue ) {
                    started = Stopwatch.StartNew(); //don't allocate a stopwatch unless we're waiting for time to pass.
                }

                var itemCount = this.Count;

                var leftTracker = new ReaderWriterLockSlim[ itemCount ];

                for ( var i = 0; i < leftTracker.Length; i++ ) {
                    leftTracker[ i ] = new ReaderWriterLockSlim();
                }

                var rightTracker = new ReaderWriterLockSlim[ itemCount ];

                for ( var i = 0; i < rightTracker.Length; i++ ) {
                    rightTracker[ i ] = new ReaderWriterLockSlim();
                }

                if ( !token.HasValue ) {
                    token = CancellationToken.None;
                }

                var parallelOptions = new ParallelOptions {
                    CancellationToken = token.Value, MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                var left = new TranslateBytesToInt32 {
                    Bytes = new Byte[ itemCount * sizeof( Int32 ) ]
                };

                var right = new TranslateBytesToInt32 {
                    Bytes = new Byte[ itemCount * sizeof( Int32 ) ]
                };

                do {
                    Parallel.Invoke( parallelOptions, () => Randem.NextBytes( left.Bytes ), () => Randem.NextBytes( right.Bytes ) );

                    //I don't know how well the list will handle this Parallel.For. It needs tested. I can think values can possibly overwrite each other and some may end up lost.
                    Parallel.For( 0, itemCount, parallelOptions, index => {

                        //so.. how badly will this fail? race conditions and all..
                        //and if we're locking, then is there any benefit to using Parallel.For?

                        if ( leftTracker[ index ].TryEnterWriteLock( 0 ) && rightTracker[ index ].TryEnterWriteLock( 0 ) ) {
                            try {
                                var indexA = left.Ints[ index ];
                                var indexB = right.Ints[ index ];

                                var c = this.TheList[ indexA ];
                                this.TheList[ indexA ] = this.TheList[ indexB ];
                                this.TheList[ indexB ] = c;
                            }
                            finally {
                                rightTracker[ index ].ExitWriteLock();
                                leftTracker[ index ].ExitWriteLock();
                            }
                        }
                    } );

                    --iterations;

                    if ( token.Value.IsCancellationRequested ) {
                        return true;
                    }

                    if ( forHowLong.HasValue ) {
                        iterations++; //we're waiting for time. reincrement the counter.

                        if ( started.Elapsed > forHowLong.Value ) {
                            return true;
                        }
                    }
                } while ( iterations.Any() );

                return true;
            } );
        */
    }
}