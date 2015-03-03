#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian 2015/ConcurrentList.cs" was last cleaned by RICK on 2015/03/01 at 6:41 AM

#endregion License & Information

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using JetBrains.Annotations;
    using Measurement.Time;

    /// <summary>
    ///     <para>A thread safe list. Uses the Microsoft TPL dataflow behind the scene.</para>
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <remarks>This class was created on a spur of the moment idea, and is <b>thoroughly</b> untested.</remarks>
    /// <copyright>Rick@AIBrain.org</copyright>
    [CollectionDataContract]
    [DebuggerDisplay("Count={Count)}")]
    public sealed class ConcurrentList<TType> : IList<TType> {

        /// <summary>
        ///     <para>Provide a dataflow block to process messages in a serial fashion.</para>
        /// </summary>
        [NotNull]
        private readonly ActionBlock<Action> _actionBlock = new ActionBlock<Action>( action => action(), new ExecutionDataflowBlockOptions {
            SingleProducerConstrained = false,
            MaxDegreeOfParallelism = 1
        } );

        /// <summary>
        ///     <para>The internal list actually used.</para>
        /// </summary>
        [DataMember]
        [NotNull]
        private readonly List<TType> _list = new List<TType>();

        [NotNull]
        private readonly ReaderWriterLockSlim _readerWriter = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        private readonly ThreadLocal<ManualResetEventSlim> _slims = new ThreadLocal<ManualResetEventSlim>( valueFactory: () => new ManualResetEventSlim( initialState: false ), trackAllValues: false );

        [NotNull]
        private readonly ThreadLocal<int> _waitingToBeAddedCounter = new ThreadLocal<int>( trackAllValues: true );

        [NotNull]
        private readonly ThreadLocal<int> _waitingToBeChangedCounter = new ThreadLocal<int>( trackAllValues: true );

        [NotNull]
        private readonly ThreadLocal<int> _waitingToBeInsertedCounter = new ThreadLocal<int>( trackAllValues: true );

        [NotNull]
        private readonly ThreadLocal<int> _waitingToBeRemovedCounter = new ThreadLocal<int>( trackAllValues: true );

        /// <summary>
        ///     Internal item counter.
        /// </summary>
        [ NotNull ]
        private ThreadLocal< int > ItemCounter { get; set; } = new ThreadLocal< int >( () => 0, trackAllValues: true );

        /// <summary>
        ///     <para>Tracks count of times this <see cref="ConcurrentList{TType}" /> has been marked as <see cref="Complete" />.</para>
        /// </summary>
        /// <seealso cref="AllowModifications" />
        private long _markedAsCompleteCounter;

        /// <summary>
        /// </summary>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        public ConcurrentList( Span? readTimeout = null, Span? writeTimeout = null ) {
            if ( readTimeout.HasValue ) {
                this.TimeoutForReads = readTimeout.Value;
            }
            if ( writeTimeout.HasValue ) {
                this.TimeoutForWrites = writeTimeout.Value;
            }
        }

        //private ConcurrentList() { }

        /// <summary>
        ///     Returns true if this <see cref="ConcurrentList{TType}" /> has not been marked as <see cref="Complete" />.
        /// </summary>
        public Boolean AllowModifications {
            get {
                if ( this.IsReadOnly ) {
                    return false;
                }
                return Interlocked.Read( ref this._markedAsCompleteCounter ) == 0;
            }
        }

        /// <summary>
        ///     <para>Returns the count of items waiting to be added to this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        public int CountOfItemsWaitingToBeAdded => this._waitingToBeAddedCounter.Values.Aggregate( 0, ( current, variable ) => current + variable );

        /// <summary>
        ///     <para>Returns the count of items waiting to be changed in this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        public int CountOfItemsWaitingToBeChanged => this._waitingToBeChangedCounter.Values.Aggregate( 0, ( current, variable ) => current + variable );

        /// <summary>
        ///     <para>Returns the count of items waiting to be inserted to this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        public int CountOfItemsWaitingToBeInserted => this._waitingToBeInsertedCounter.Values.Aggregate( 0, ( current, variable ) => current + variable );

        public Span TimeoutForReads { get; } = Minutes.One;

        public Span TimeoutForWrites { get; } = Minutes.One;

        /// <summary>
        /// </summary>
        /// <seealso cref="CatchUp" />
        public bool AnyWritesPending => 0 == this.CountOfItemsWaitingToBeAdded && 0 == this.CountOfItemsWaitingToBeChanged && 0 == this.CountOfItemsWaitingToBeInserted;

        /// <summary>
        ///     <para>Count of items currently in this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        public int Count => this.ItemCounter.Values.Aggregate( 0, ( current, variable ) => current + variable );

        /// <summary>
        /// </summary>
        /// <seealso cref="AllowModifications" />
        public Boolean IsReadOnly { get; private set; }

        public TType this[ int index ] {
            get {
                if ( index < 0 ) {
                    throw new IndexOutOfRangeException( String.Format("index {0} is out of range", index) );
                }
                return this.Read( () => this._list[ index ] );
            }

            set {
                if ( !this.AllowModifications ) {
                    return;
                }

                this.RequestToChangeAnItem();

                this._actionBlock.Post( () => this.Write( () => {
                    if ( !this.AllowModifications ) {
                        return false;
                    }
                    this._list[ index ] = value;
                    this.AnItemHasBeenChanged();
                    return true;
                } ) );
            }
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
        ///     Mark this <see cref="ConcurrentList{TType}" /> to be cleared.
        /// </summary>
        public void Clear() {
            if ( !this.AllowModifications ) {
                return;
            }
            this._actionBlock.Post( () => this.Write( () => {
                this._list.Clear();
                this.ItemCounter = new ThreadLocal<int>( () => 0, trackAllValues: true ); //BUG is this correct?
                return true;
            } ) );
        }

        /// <summary>
        ///     <para>
        ///         Determines whether the <paramref name="item" /> is in this <see cref="ConcurrentList{TType}" /> at this moment
        ///         in time.
        ///     </para>
        /// </summary>
        public Boolean Contains( TType item ) => this.Read( () => this._list.Contains( item ) );

        /// <summary>
        ///     Copies the entire <see cref="ConcurrentList{TType}" /> to the <paramref name="array" />, starting at the specified
        ///     index in the target array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo( TType[] array, int arrayIndex ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            this.Read( () => {
                this._list.CopyTo( array: array, arrayIndex: arrayIndex );
                return true;
            } );
        }

        /// <summary>
        ///     <para>
        ///         Returns an enumerator that iterates through a <see cref="Clone" /> of this <see cref="ConcurrentList{TType}" />
        ///         .
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TType> GetEnumerator() => this.Clone().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        ///     <para>
        ///         Searches at this moment in time for the first occurrence of <paramref name="item" /> and returns the
        ///         zero-based index, or -1 if not found.
        ///     </para>
        /// </summary>
        /// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}" />.</param>
        public int IndexOf( TType item ) => this.Read( () => this._list.IndexOf( item ) );

        /// <summary>
        ///     <para>
        ///         Requests an insert of the <paramref name="item" /> into this <see cref="ConcurrentList{TType}" /> at the
        ///         specified <paramref name="index" />.
        ///     </para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert( int index, TType item ) {
            if ( !this.AllowModifications ) {
                return;
            }

            this.RequestToInsertAnItem();

            this._actionBlock.Post( () => this.Write( () => {
                try {
                    this._list.Insert( index: index, item: item );
                    return true;
                }
                catch ( ArgumentOutOfRangeException) {
                    return false;
                }
                finally {
                    this.AnItemHasBeenInserted();
                }
            } ) );
        }

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove( TType item ) => this.Remove( item, null );

        public void RemoveAt( int index ) {
            if ( index < 0 ) {
                return;
            }

            if ( !this.AllowModifications ) {
                return;
            }

            this.RequestToRemoveAnItem();

            this._actionBlock.Post( () => this.Write( () => {
                try {
                    if ( index < this._list.Count ) {
                        this._list.RemoveAt( index );
                    }
                }
                catch ( ArgumentOutOfRangeException) {
                    return false;
                }
                finally {
                    this.AnItemHasBeenRemoved();
                }
                return true;
            } ) );
        }

        /// <summary>
        ///     <para>
        ///         Blocks until the list has no write operations pending, until the <paramref name="timeout" />, or the
        ///         <paramref name="cancellationToken" /> is set.
        ///     </para>
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns true if list is caught up. (No write operations pending)</returns>
        public Boolean CatchUp( Span timeout = default(Span), CancellationToken cancellationToken = default(CancellationToken) ) {
            if ( timeout == default(Span) ) {
                timeout = this.TimeoutForWrites;
            }
            var interval = Milliseconds.Hertz111;
            var stopWatch = Stopwatch.StartNew();
            while ( this.AllowModifications ) {
                if ( stopWatch.Elapsed > timeout ) {
                    break;
                }
                if ( !this.AnyWritesPending ) {
                    break;
                }
                Task.Delay( interval, cancellationToken ).Wait( cancellationToken );
            }
            return this.AnyWritesPending;
        }

        /// <summary>
        ///     Blocks and waits.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        public void Wait( Span timeout = default(Span), CancellationToken cancellationToken = default(CancellationToken) ) => this.CatchUp( timeout, cancellationToken );

        /// <summary>
        ///     <para>
        ///         Add the
        ///         <typeparam name="TType">item</typeparam>
        ///         to the end of this <see cref="ConcurrentList{TType}" />.
        ///     </para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        public Boolean Add( TType item, [CanBeNull] Action afterAdd ) {
            if ( !this.AllowModifications ) {
                return false;
            }

            this.RequestToAddAnItem();

            return this._actionBlock.Post( () => this.Write( () => {
                try {
                    this._list.Add( item: item );
                    return true;
                }
                finally {
                    this.AnItemHasBeenAdded();
                    afterAdd?.Invoke();
                }
            } ) );
        }

        public Boolean AddAndWait( TType item, CancellationToken cancellationToken = default(CancellationToken), TimeSpan timeout = default(TimeSpan) ) {

            //var slim = new ManualResetEventSlim( initialState: false );
            this._slims.Value.Reset();

            this.Add( item: item, afterAdd: () => this._slims.Value.Set() );

            try {
                if ( default(TimeSpan) != timeout && default(CancellationToken) != cancellationToken ) {
                    return this._slims.Value.Wait( timeout, cancellationToken );
                }
                if ( default(TimeSpan) != timeout ) {
                    return this._actionBlock.Completion.Wait( timeout: timeout );
                }
                this._slims.Value.Wait( cancellationToken );
                return true;
            }
            catch ( OperationCanceledException) { }
            catch ( ArgumentOutOfRangeException) { }
            catch ( ObjectDisposedException) { }
            catch ( AggregateException aggregateException ) {
                foreach ( var innerException in aggregateException.InnerExceptions ) {

                    //TODO what? throw?
                }
            }
            return false;
        }

        public Task AddAsync( TType item, Action afterAdd = null ) => Task.Run( () => { this.TryAdd( item: item, afterAdd: afterAdd ); } );

        /// <summary>
        ///     Add a collection of items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="useParallelism">Enables parallelization of the <paramref name="items" />.</param>
        /// <param name="afterEachAdd"><see cref="Action" /> to perform after each add.</param>
        /// <param name="afterRangeAdded"><see cref="Action" /> to perform after range added.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange( [NotNull] IEnumerable<TType> items, Byte useParallelism = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) {
            if ( null == items ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            if ( !this.AllowModifications ) {
                return;
            }

            try {
                if ( useParallelism >= 1 ) {
                    items.AsParallel().WithDegreeOfParallelism( useParallelism ).ForAll( item => this.TryAdd( item, afterEachAdd ) );
                } else {
                    foreach ( var item in items ) {
                        this.TryAdd( item: item, afterAdd: afterEachAdd );
                    }
                }
            }
            finally {
                afterRangeAdded?.Invoke();
            }
        }

        public Task AddRangeAsync( [CanBeNull] IEnumerable<TType> items, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) => Task.Run( () => {
            if ( items != null ) {
                this.AddRange( items, afterEachAdd: afterEachAdd, afterRangeAdded: afterRangeAdded );
            }
        } );

        /// <summary>
        ///     <para>
        ///         Returns a copy of this <see cref="ConcurrentList{TType}" /> as this moment in time.
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public List<TType> Clone() => this.Write( func: () => {
            var copy = this._list.ToList();
            return copy;
        }, ignoreAllowModificationsCheck: true );

        /// <summary>
        ///     Signal that this <see cref="ConcurrentList{TType}" /> will not be modified any more.
        /// </summary>
        /// <seealso cref="AllowModifications" />
        /// <seealso cref="IsReadOnly" />
        public void Complete() {
            try {
                this._actionBlock.Complete();
            }
            finally {
                Interlocked.Increment( ref this._markedAsCompleteCounter );
                this.IsReadOnly = true;
            }
        }

        /// <summary>
        ///     <para>Signal that nothing else will be added or removed from this <see cref="ConcurrentList{TType}" /> and then,</para>
        ///     <para>
        ///         If both <paramref name="timeout" /> and <paramref name="cancellationToken" /> are provided,
        ///         <see cref="Task.Wait()" /> with it.
        ///     </para>
        ///     <para>Otherwise, if only a <paramref name="timeout" /> is provided, <see cref="Task.Wait()" /> with it.</para>
        ///     <para>
        ///         Otherwise, if only a <paramref name="cancellationToken" /> is provided, <see cref="Task.Wait()" /> with
        ///         it.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     Returns <see cref="Boolean.True" /> if the Task completed execution within the allotted time or has already
        ///     waited.
        /// </returns>
        public Boolean CompleteAndWait( CancellationToken cancellationToken = default(CancellationToken), TimeSpan timeout = default(TimeSpan) ) {
            try {
                this.Complete();

                if ( default(TimeSpan) != timeout && default(CancellationToken) != cancellationToken ) {
                    return this._actionBlock.Completion.Wait( millisecondsTimeout: ( int )timeout.TotalMilliseconds, cancellationToken: cancellationToken );
                }
                if ( default(TimeSpan) != timeout ) {
                    return this._actionBlock.Completion.Wait( timeout: timeout );
                }
                if ( default(CancellationToken) != cancellationToken ) {
                    try {
                        this._actionBlock.Completion.Wait( cancellationToken: cancellationToken );
                    }
                    catch ( OperationCanceledException) {
                        return false; //BUG Is this correct?
                    }
                    catch ( ObjectDisposedException) {
                        return false;
                    }
                    catch ( AggregateException) {
                        return false; //BUG Is this correct?
                    }
                    return true;
                }

                // ReSharper disable once MethodSupportsCancellation
                this._actionBlock.Completion.Wait();
                return true;
            }
            finally {
                this._list.Capacity = this._list.Count; //optimize the memory used by this list.
            }
        }

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="afterRemoval"></param>
        /// <returns></returns>
        public Boolean Remove( TType item, [CanBeNull] Action afterRemoval ) {
            if ( !this.AllowModifications ) {
                return false;
            }

            this.RequestToRemoveAnItem();

            return this._actionBlock.Post( () => this.Write( () => {
                try {
                    this._list.Remove( item );
                    return true;
                }
                finally {
                    this.AnItemHasBeenRemoved( afterRemoval );
                }
            } ) );
        }

        public Boolean TryAdd( TType item, [CanBeNull] Action afterAdd = null ) => this.Add( item: item, afterAdd: afterAdd );

        /// <summary>
        ///     <para>Try to get an item in this <see cref="ConcurrentList{TType}" /> by index.</para>
        ///     <para>Returns true if the request was posted to the internal dataflow.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="afterGet">Action to be ran after the item at the <paramref name="index" /> is got.</param>
        public Boolean TryGet( int index, [CanBeNull] Action<TType> afterGet ) {
            if ( index < 0 ) {
                return false;
            }
            return this._actionBlock.Post( () => this.Read( () => {
                if ( index >= this._list.Count ) {
                    return false;
                }
                var result = this._list[ index ];
                afterGet?.Invoke( result );
                return true;
            } ) );
        }

        private void AnItemHasBeenAdded() {
            this._waitingToBeAddedCounter.Value--;
            this.ItemCounter.Value++;
        }

        private void AnItemHasBeenChanged() => this._waitingToBeChangedCounter.Value--;

        private void AnItemHasBeenInserted() => this._waitingToBeInsertedCounter.Value--;

        private void AnItemHasBeenRemoved( [CanBeNull] Action action = null ) {
            this._waitingToBeRemovedCounter.Value--;
            this.ItemCounter.Value--;
            action?.Invoke();
        }

        /// <summary>
        ///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private TFuncResult Read<TFuncResult>( Func<TFuncResult> func ) {
            if ( !this.AllowModifications && func != null ) {
                return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
            }

            if ( !this._readerWriter.TryEnterUpgradeableReadLock( this.TimeoutForReads ) ) {
                return default(TFuncResult);
            }
            try {
                if ( func != null ) {
                    return func();
                }
            }
            finally {
                this._readerWriter.ExitUpgradeableReadLock();
            }

            return default(TFuncResult);
        }

        private void RequestToAddAnItem() => this._waitingToBeAddedCounter.Value++;

        private void RequestToChangeAnItem() => this._waitingToBeChangedCounter.Value++;

        private void RequestToInsertAnItem() => this._waitingToBeInsertedCounter.Value++;

        private void RequestToRemoveAnItem() => this._waitingToBeRemovedCounter.Value++;

        /// <summary>
        ///     <para>Filter write requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="ignoreAllowModificationsCheck"></param>
        /// <returns></returns>
        /// <seealso cref="CatchUp" />
        private TFuncResult Write<TFuncResult>( Func<TFuncResult> func, Boolean ignoreAllowModificationsCheck = false ) {
            if ( !ignoreAllowModificationsCheck ) {
                if ( !this.AllowModifications && func != null ) {
                    return default(TFuncResult);
                }
            }

            //BUG what if we want a clone of the list, but it has been marked as !this.AllowModifications

            if ( !this._readerWriter.TryEnterWriteLock( this.TimeoutForWrites ) ) {
                return default(TFuncResult);
            }
            try {
                if ( func != null ) {
                    return func();
                }
            }
            finally {
                this._readerWriter.ExitWriteLock();
            }
            return default(TFuncResult);
        }
    }
}