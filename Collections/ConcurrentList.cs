// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/ConcurrentList.cs" was last cleaned by Rick on 2015/11/03 at 5:21 PM

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
    using FluentAssertions;
    using JetBrains.Annotations;

    /// <summary>
    ///     <para>A thread safe list. Uses a <see cref="ConcurrentQueue{T}" /> to buffer adds.</para>
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <remarks>
    ///     <para>This class was created on a spur of the moment idea, and is <b>thoroughly</b> untested.</para>
    /// </remarks>
    /// <copyright>
    ///     Rick@AIBrain.org
    /// </copyright>
    [DataContract( IsReference = true )]
    [DebuggerDisplay( "Count={Count}" )]
    public class ConcurrentList< TType > : IList< TType > {

        /// <summary>
        ///     Create an empty list.
        /// </summary>
        [DebuggerStepThrough]
        // ReSharper disable once NotNullMemberIsNotInitialized
        public ConcurrentList() {
            this.Initialize();
        }

        /// <summary>
        ///     Create an empty list with different timeout values.
        /// </summary>
        /// <param name="enumerable">Fill the list with the given enumerable.</param>
        /// <param name="readTimeout"></param>
        /// <param name="writeTimeout"></param>
        [DebuggerStepThrough]

        // ReSharper disable once NotNullMemberIsNotInitialized
        public ConcurrentList( [CanBeNull] IEnumerable< TType > enumerable = null, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) : this() {
            if ( readTimeout.HasValue ) {
                this.TimeoutForReads = readTimeout.Value;
            }
            if ( writeTimeout.HasValue ) {
                this.TimeoutForWrites = writeTimeout.Value;
            }
            if ( null != enumerable ) {
                this.AddRange( enumerable );
            }
        }

        /// <summary>
        ///     Returns true if this <see cref="ConcurrentList{TType}" /> has not been marked as <see cref="Complete" />.
        /// </summary>
        public Boolean AllowModifications {
            [DebuggerStepThrough] get { return !this.IsReadOnly; }
        }

        /// <summary>
        /// </summary>
        /// <seealso cref="CatchUp" />
        public Boolean AnyWritesPending {
            [DebuggerStepThrough] get { return this.InputBuffer.Any(); }
        }

        public static TimeSpan OneMinute => TimeSpan.FromMinutes( 1 );

        public static TimeSpan OneSecond => TimeSpan.FromSeconds( 1 );

        [DataMember]
        public TimeSpan TimeoutForReads { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        [DataMember]
        public TimeSpan TimeoutForWrites { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        [DataMember]
        private ConcurrentQueue< TType > InputBuffer { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        /// <summary> threadsafe item counter (so we don't have to enter & exit the readerwriter). </summary>
        [NotNull]
        private ThreadLocal< Int32 > ItemCounter { get; set; }

        [DataMember]
        [NotNull]
        private ReaderWriterLockSlim ReaderWriter { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        /// <summary>
        ///     <para>The internal list actually used.</para>
        /// </summary>
        [DataMember]
        [NotNull]
        private List< TType > TheList { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        // 0 != this.CountOfItemsWaitingToBeAdded + this.CountOfItemsWaitingToBeInserted /*+ this.CountOfItemsWaitingToBeChanged*/;

        /// <summary>
        ///     <para>Count of items currently in this <see cref="ConcurrentList{TType}" />.</para>
        /// </summary>
        public Int32 Count {
            [DebuggerStepThrough] get { return this.ItemCounter.Values.Aggregate( 0, ( current, variable ) => current + variable ); }
        }

        /// <summary>
        /// </summary>
        /// <seealso cref="AllowModifications" />
        public Boolean IsReadOnly { [DebuggerStepThrough] get; private set; }

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
        /// </exception>
        public TType this[ Int32 index ] {
            [DebuggerStepThrough] [CanBeNull] get {
                if ( index < 0 ) {
                    throw new IndexOutOfRangeException( $"index {index} is out of range" );
                }
                return this.Read( () => this.TheList[ index ] );
            }

            [DebuggerStepThrough] set {
                if ( !this.AllowModifications ) {
                    return;
                }

                this.Write( () => {
                                if ( !this.AllowModifications ) {
                                    return false;
                                }
                                this.TheList[ index ] = value;
                                return true;
                            } );
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
        [DebuggerStepThrough]
        public void Add( TType item ) => this.Add( item: item, afterAdd: null );

        /// <summary>
        ///     Mark this <see cref="ConcurrentList{TType}" /> to be cleared.
        /// </summary>
        [DebuggerStepThrough]
        public void Clear() {
            if ( !this.AllowModifications ) {
                return;
            }
            this.Write( () => {
                            this.TheList.Clear();
                            this.ItemCounter = new ThreadLocal< Int32 >( () => 0, trackAllValues: true ); //BUG is this wrong? how else do we reset all the counters?
                            return true;
                        } );
        }

        /// <summary>
        ///     <para>
        ///         Determines whether the <paramref name="item" /> is in this
        ///         <see cref="ConcurrentList{TType}" /> at this moment in time.
        ///     </para>
        /// </summary>
        [DebuggerStepThrough]
        public Boolean Contains( TType item ) => this.Read( () => this.TheList.Contains( item ) );

        /// <summary>
        ///     Copies the entire <see cref="ConcurrentList{TType}" /> to the <paramref name="array" />,
        ///     starting at the specified index in the target array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        [DebuggerStepThrough]
        public void CopyTo( TType[] array, Int32 arrayIndex ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            this.Read( () => {
                           this.TheList.CopyTo( array: array, arrayIndex: arrayIndex );
                           return true;
                       } );
        }

        /// <summary>
        ///     <para>
        ///         Returns an enumerator that iterates through a <see cref="Clone" /> of this
        ///         <see cref="ConcurrentList{TType}" /> .
        ///     </para>
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public IEnumerator< TType > GetEnumerator() => this.Clone()
                                                           .GetEnumerator();

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator() => this.Clone()
                                                       .GetEnumerator();

        /// <summary>
        ///     <para>
        ///         Searches at this moment in time for the first occurrence of <paramref name="item" /> and
        ///         returns the zero-based index, or -1 if not found.
        ///     </para>
        /// </summary>
        /// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}" />.</param>
        [DebuggerStepThrough]
        public Int32 IndexOf( TType item ) => this.Read( () => this.TheList.IndexOf( item ) );

        /// <summary>
        ///     <para>
        ///         Requests an insert of the <paramref name="item" /> into this
        ///         <see cref="ConcurrentList{TType}" /> at the specified <paramref name="index" />.
        ///     </para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        [DebuggerStepThrough]
        public void Insert( Int32 index, TType item ) {
            if ( !this.AllowModifications ) {
                return;
            }

            this.Write( () => {
                            try {
                                this.TheList.Insert( index: index, item: item );
                                this.AnItemHasBeenAdded();
                                return true;
                            }
                            catch ( ArgumentOutOfRangeException ) {
                                return false;
                            }
                        } );
        }

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Boolean Remove( TType item ) => this.Remove( item, null );

        [DebuggerStepThrough]
        public void RemoveAt( Int32 index ) {
            index.Should()
                 .BeGreaterOrEqualTo( 0 );
            if ( index < 0 ) {
                return;
            }

            if ( !this.AllowModifications ) {
                return;
            }

            this.Write( () => {
                            try {
                                index.Should()
                                     .BeLessOrEqualTo( this.TheList.Count );

                                if ( index < this.TheList.Count ) {
                                    this.TheList.RemoveAt( index );
                                    this.AnItemHasBeenRemoved();
                                }
                            }
                            catch ( ArgumentOutOfRangeException ) {
                                return false;
                            }
                            return true;
                        } );
        }

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
        [DebuggerStepThrough]
        public Boolean Add( TType item, [CanBeNull] Action afterAdd ) {
            if ( !this.AllowModifications ) {
                return false;
            }

            return this.Write( () => {
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

        [DebuggerStepThrough]
        public Boolean AddAndWait( TType item ) {
            return this.Add( item: item, afterAdd: this.CatchUp );
        }

        [DebuggerStepThrough]
        public async Task< Boolean > AddAsync( TType item, Action afterAdd = null ) => await Task.Run( () => this.TryAdd( item: item, afterAdd: afterAdd ) );

        /// <summary>
        ///     Add a collection of items.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="useParallelism">Enables parallelization of the <paramref name="items" />.</param>
        /// <param name="afterEachAdd"><see cref="Action" /> to perform after each add.</param>
        /// <param name="afterRangeAdded"><see cref="Action" /> to perform after range added.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerStepThrough]
        public void AddRange( [NotNull] IEnumerable< TType > items, Byte useParallelism = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) {
            if ( null == items ) {
                throw new ArgumentNullException( nameof( items ) );
            }

            if ( !this.AllowModifications ) {
                return;
            }

            try {
                if ( useParallelism >= Environment.ProcessorCount ) {
                    items.AsParallel()
                         .WithDegreeOfParallelism( useParallelism )
                         .ForAll( item => this.TryAdd( item, afterEachAdd ) );
                }
                else {
                    foreach ( var item in items ) {
                        this.TryAdd( item: item, afterAdd: afterEachAdd );
                    }
                }
            }
            finally {
                afterRangeAdded?.Invoke();
            }
        }

        [DebuggerStepThrough]
        public async Task AddRangeAsync( [CanBeNull] IEnumerable< TType > items, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) => await Task.Run( () => {
                                                                                                                                                                                            if ( items != null ) {
                                                                                                                                                                                                this.AddRange( items, afterEachAdd: afterEachAdd, afterRangeAdded: afterRangeAdded );
                                                                                                                                                                                            }
                                                                                                                                                                                        } );

        /// <summary>
        ///     Blocks, transfers items from <see cref="InputBuffer" />, and then releases lock.
        /// </summary>
        [DebuggerStepThrough]
        public void CatchUp() {
            if ( !AnyWritesPending ) {
                return;
            }
            try {
                this.ReaderWriter.EnterWriteLock();

                TType bob;
                while ( this.InputBuffer.TryDequeue( out bob ) ) {
                    this.TheList.Add( bob );
                    this.AnItemHasBeenAdded();
                }
            }
            finally {
                this.ReaderWriter.ExitWriteLock();
            }
        }

        /// <summary>
        ///     <para>
        ///         Returns a copy of this <see cref="ConcurrentList{TType}" /> at this moment in time.
        ///     </para>
        /// </summary>
        /// <returns></returns>
        [DebuggerStepThrough]
        public IEnumerable< TType > Clone() => this.Read( func: () => this.TheList.ToList() );

        /// <summary>
        ///     Signal that this <see cref="ConcurrentList{TType}" /> will not be modified any more.
        ///     <para>Blocks.</para>
        /// </summary>
        /// <seealso cref="AllowModifications" />
        /// <seealso cref="IsReadOnly" />
        [DebuggerStepThrough]
        public void Complete() {
            try {
                this.CatchUp();
                this.Fix();
            }
            finally {
                this.IsReadOnly = true;
                this.AllowModifications.Should()
                    .BeFalse();
            }
        }

        /// <summary>
        ///     The <seealso cref="List{T}.Capacity" /> is resized down to the <seealso cref="List{T}.Count" />.
        /// </summary>
        [DebuggerStepThrough]
        public void Fix() {
            this.Write( () => {
                            this.TheList.Capacity = this.TheList.Count;
                            return true;
                        } );
        }

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="afterRemoval"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Boolean Remove( TType item, [CanBeNull] Action afterRemoval ) {
            if ( !this.AllowModifications ) {
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

        [DebuggerStepThrough]
        public Boolean TryAdd( TType item, [CanBeNull] Action afterAdd = null ) => this.Add( item: item, afterAdd: afterAdd );

        /// <summary>
        ///     <para>Try to get an item in this <see cref="ConcurrentList{TType}" /> by index.</para>
        ///     <para>Returns true if the request was posted to the internal dataflow.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="afterGet">
        ///     Action to be ran after the item at the <paramref name="index" /> is got.
        /// </param>
        [DebuggerStepThrough]
        public Boolean TryGet( Int32 index, [CanBeNull] Action< TType > afterGet ) {
            if ( index < 0 ) {
                return false;
            }

            return this.Read( () => {
                                  if ( index >= this.TheList.Count ) {
                                      return false;
                                  }
                                  var result = this.TheList[ index ];
                                  afterGet?.Invoke( result );
                                  return true;
                              } );
        }

        [DebuggerStepThrough]
        private void AnItemHasBeenAdded() {
            this.ItemCounter.Value++;
        }

        [DebuggerStepThrough]
        private void AnItemHasBeenRemoved( [CanBeNull] Action action = null ) {
            this.ItemCounter.Value--;
            action?.Invoke();
        }

        [DebuggerStepThrough]
        private void Initialize() {
            this.TimeoutForWrites = OneMinute;
            this.TimeoutForReads = OneMinute;
            this.TheList = new List< TType >();
            this.InputBuffer = new ConcurrentQueue< TType >();
            this.ItemCounter = new ThreadLocal< Int32 >( () => 0, trackAllValues: true );
            this.ReaderWriter = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
        }

        [DebuggerStepThrough]
        [OnDeserialized]
        private void OnDeserialized( StreamingContext context ) {
            this.Initialize();
            this.ItemCounter.Value += this.TheList.Count;
        }

        [DebuggerStepThrough]
        [OnDeserializing]
        private void OnDeserializing( StreamingContext context ) { }

        [DebuggerStepThrough]
        [OnSerialized]
        private void OnSerialized( StreamingContext context ) { }

        [DebuggerStepThrough]
        [OnSerializing]
        private void OnSerializing( StreamingContext context ) => this.CatchUp();

        /// <summary>
        ///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        [CanBeNull]
        private TFuncResult Read< TFuncResult >( Func< TFuncResult > func ) {
            if ( !this.AllowModifications && ( func != null ) ) {
                return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
            }

            this.CatchUp();

            if ( !this.ReaderWriter.TryEnterUpgradeableReadLock( this.TimeoutForReads ) ) {
                return default ( TFuncResult );
            }
            try {
                if ( func != null ) {
                    return func();
                }
            }
            finally {
                this.ReaderWriter.ExitUpgradeableReadLock();
            }

            return default ( TFuncResult );
        }

        /// <summary>
        ///     <para>Filter write requests through the <see cref="ReaderWriter" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="ignoreAllowModificationsCheck"></param>
        /// <returns></returns>
        /// <seealso cref="CatchUp" />
        [DebuggerStepThrough]
        [CanBeNull]
        private TFuncResult Write< TFuncResult >( [CanBeNull] Func< TFuncResult > func, Boolean ignoreAllowModificationsCheck = false ) {
            if ( !ignoreAllowModificationsCheck && !this.AllowModifications && ( func != null ) ) {
                return default ( TFuncResult );
            }

            if ( !this.ReaderWriter.TryEnterWriteLock( this.TimeoutForWrites ) ) {
                return default ( TFuncResult );
            }
            try {
                if ( func != null ) {
                    return func();
                }
            }
            finally {
                this.ReaderWriter.ExitWriteLock();
            }
            return default ( TFuncResult );
        }

    }

}
