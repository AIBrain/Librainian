// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ConcurrentList.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

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
    using Magic;
    using Maths;
    using Measurement.Time;
    using Microsoft.FSharp.Core;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    /// <para>A thread safe list. Uses a <see cref="ConcurrentQueue{T}"/> to buffer adds.</para>
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    /// <remarks>
    /// <para>This class was created on a spur of the moment idea, and is <b>thoroughly</b> UNTESTED.</para>
    /// </remarks>
    /// <copyright>
    ///     Protiguous
    /// </copyright>
    [JsonObject]
    [DebuggerDisplay( value: "Count={" + nameof( Count ) + "}" )]
    public class ConcurrentList<TType> : ABetterClassDispose, IList<TType>, IEquatable<IEnumerable<TType>> {

        /// <summary>
        /// Create an empty list with different timeout values.
        /// </summary>
        /// <param name="enumerable">  Fill the list with the given enumerable.</param>
        /// <param name="readTimeout"> </param>
        /// <param name="writeTimeout"></param>
        public ConcurrentList( [CanBeNull] IEnumerable<TType> enumerable = null, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null ) {
            this.InputBuffer = new ConcurrentQueue<TType>();
            if ( readTimeout.HasValue ) {
                this.TimeoutForReads = readTimeout.Value;
            }

            if ( writeTimeout.HasValue ) {
                this.TimeoutForWrites = writeTimeout.Value;
            }

            if ( null != enumerable ) {
                this.AddRange( items: enumerable );
            }
        }

        [JsonIgnore]
        private ConcurrentQueue<TType> InputBuffer { get; set; }

        /// <summary> threadsafe item counter (so we don't have to enter & exit the readerwriter). </summary>
        private ThreadLocal<Int32> ItemCounter { get; set; } = new ThreadLocal<Int32>( valueFactory: () => 0, trackAllValues: true );

        [JsonIgnore]
        private ReaderWriterLockSlim ReaderWriter { get; set; } = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );

        /// <summary>
        /// <para>The internal list actually used.</para>
        /// </summary>
        [JsonProperty]
        private List<TType> TheList { get; set; } = new List<TType>();

        /// <summary>
        /// <para>Count of items currently in this <see cref="ConcurrentList{TType}"/>.</para>
        /// </summary>
        [JsonIgnore]
        public Int32 Count => this.ItemCounter.Values.Aggregate( seed: 0, func: ( current, variable ) => current + variable );

        /// <summary>
        /// </summary>
        /// <seealso cref="AllowModifications"/>
        public Boolean IsReadOnly { get; private set; }

        [JsonProperty]
        public TimeSpan? TimeoutForReads { get; set; } = TimeSpan.FromMinutes( value: 1 );

        [JsonProperty]
        public TimeSpan? TimeoutForWrites { get; set; } = TimeSpan.FromMinutes( value: 1 );

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
        public TType this[Int32 index] {
            [CanBeNull]
            get {
                if ( index >= 0 && this.TheList.Count <= index ) {
                    return this.Read( func: () => this.TheList[index: index] );
                }

                return default;
            }

            set {
                if ( !this.AllowModifications() ) {
                    return;
                }

                this.Write( func: () => {
                    if ( !this.AllowModifications() ) {
                        return false;
                    }

                    this.TheList[index: index] = value;
                    return true;
                } );
            }
        }

        private void AnItemHasBeenAdded() => this.ItemCounter.Value++;

        private void AnItemHasBeenRemoved( [CanBeNull] Action action = null ) {
            this.ItemCounter.Value--;
            action?.Invoke();
        }

        [OnDeserialized]
        private void OnDeserialized( StreamingContext context ) {
            if ( !this.TimeoutForWrites.HasValue ) {
                this.TimeoutForWrites = TimeSpan.FromMinutes( value: 1 );
            }

            if ( !this.TimeoutForReads.HasValue ) {
                this.TimeoutForReads = TimeSpan.FromMinutes( value: 1 );
            }

            if ( null == this.ReaderWriter ) {
                this.ReaderWriter = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );
            }

            if ( null == this.TheList ) {
                this.TheList = new List<TType>();
            }

            if ( null == this.InputBuffer ) {
                this.InputBuffer = new ConcurrentQueue<TType>();
            }

            if ( null == this.ItemCounter ) {
                this.ItemCounter = new ThreadLocal<Int32>( valueFactory: () => 0, trackAllValues: true );
            }

            this.ItemCounter.Value += this.TheList.Count;
        }

        /// <summary>
        /// <para>Filter read requests through a <see cref="ReaderWriterLockSlim"/>.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        [CanBeNull]
        private TFuncResult Read<TFuncResult>( Func<TFuncResult> func ) {
            if ( !this.AllowModifications() && func != null ) {
                return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
            }

            this.CatchUp();

            if ( !this.ReaderWriter.TryEnterUpgradeableReadLock( timeout: this.TimeoutForReads ?? TimeSpan.FromMinutes( value: 1 ) ) ) {
                return default;
            }

            try {
                if ( func != null ) {
                    return func();
                }
            }
            finally {
                this.ReaderWriter.ExitUpgradeableReadLock();
            }

            return default;
        }

        //[OnSerializing]
        //private void OnSerializing( StreamingContext context ) => this.CatchUp();
        /// <summary>
        /// <para>Filter write requests through the <see cref="ReaderWriter"/>.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func">                         </param>
        /// <param name="ignoreAllowModificationsCheck"></param>
        /// <returns></returns>
        /// <seealso cref="CatchUp"/>
        [CanBeNull]
        private TFuncResult Write<TFuncResult>( [CanBeNull] Func<TFuncResult> func, Boolean ignoreAllowModificationsCheck = false ) {
            if ( !ignoreAllowModificationsCheck && !this.AllowModifications() && func != null ) {
                return default;
            }

            if ( !this.ReaderWriter.TryEnterWriteLock( timeout: this.TimeoutForWrites ?? TimeSpan.FromMinutes( value: 1 ) ) ) {
                return default;
            }

            try {
                if ( func != null ) {
                    return func();
                }
            }
            finally {
                this.ReaderWriter.ExitWriteLock();
            }

            return default;
        }

        /// <summary>
        /// Dispose any disposable members.
        /// </summary>
        protected override void DisposeManaged() {
            this.ReaderWriter?.Dispose();
            this.ItemCounter?.Dispose();
        }

        /// <summary>
        /// Static comparison function.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Boolean Equals( IEnumerable<TType> lhs, IEnumerable<TType> rhs ) {
            if ( ReferenceEquals( lhs, rhs ) ) {
                return true;
            }

            if ( lhs is null ) {
                return false;
            }

            if ( rhs is null ) {
                return false;
            }

            return lhs.SequenceEqual( second: rhs );
        }

        /// <summary>
        /// <para>Add the <typeparam name="TType">item</typeparam> to the end of this <see cref="ConcurrentList{TType}"/>.</para>
        /// </summary>
        /// <param name="item"></param>
        public void Add( TType item ) => this.Add( item: item, afterAdd: null );

        /// <summary>
        /// <para>Add the <typeparam name="TType">item</typeparam> to the end of this <see cref="ConcurrentList{TType}"/>.</para>
        /// </summary>
        /// <param name="item">    </param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        public Boolean Add( TType item, [CanBeNull] Action afterAdd ) {
            if ( !this.AllowModifications() ) {
                return false;
            }

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

        public async Task<Boolean> AddAsync( TType item, Action afterAdd = null ) => await Task.Run( function: () => this.TryAdd( item: item, afterAdd: afterAdd ) );

        /// <summary>
        /// Add a collection of items.
        /// </summary>
        /// <param name="items">          </param>
        /// <param name="useParallelism"> Enables parallelization of the <paramref name="items"/> No guarantee of the final order of items.</param>
        /// <param name="afterEachAdd">   <see cref="Action"/> to perform after each add.</param>
        /// <param name="afterRangeAdded"><see cref="Action"/> to perform after range added.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange( [NotNull] IEnumerable<TType> items, Byte useParallelism = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) {
            if ( null == items ) {
                throw new ArgumentNullException( paramName: nameof( items ) );
            }

            if ( !this.AllowModifications() ) {
                return;
            }

            try {
                if ( useParallelism >= Environment.ProcessorCount ) {
                    items.AsParallel().WithDegreeOfParallelism( degreeOfParallelism: useParallelism ).ForAll( item => this.TryAdd( item: item, afterAdd: afterEachAdd ) );
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

        public async Task AddRangeAsync( [CanBeNull] IEnumerable<TType> items, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null ) =>
            await Task.Run( () => {
                if ( items != null ) {
                    this.AddRange( items: items, afterEachAdd: afterEachAdd, afterRangeAdded: afterRangeAdded );
                }
            } ).ConfigureAwait( false );

        /// <summary>
        /// Returns true if this <see cref="ConcurrentList{TType}"/> has not been marked as <see cref="Complete"/>.
        /// </summary>
        public Boolean AllowModifications() => !this.IsReadOnly;

        /// <summary>
        /// </summary>
        /// <seealso cref="CatchUp"/>
        public Boolean AnyWritesPending() {
            var inputBuffer = this.InputBuffer;
            return inputBuffer != null && inputBuffer.Any();
        }

        /// <summary>
        /// Blocks, transfers items from <see cref="InputBuffer"/>, and then releases lock.
        /// </summary>
        public void CatchUp() {
            if ( !this.AnyWritesPending() ) {
                return;
            }

            try {
                this.ReaderWriter.EnterWriteLock();

                while ( this.InputBuffer.TryDequeue( result: out var bob ) ) {
                    this.TheList.Add( item: bob );
                    this.AnItemHasBeenAdded();
                }
            }
            finally {
                this.ReaderWriter.ExitWriteLock();
            }
        }

        /// <summary>
        /// Mark this <see cref="ConcurrentList{TType}"/> to be cleared.
        /// </summary>
        public void Clear() {
            if ( !this.AllowModifications() ) {
                return;
            }

            this.Write( func: () => {
                this.TheList.Clear();
                this.ItemCounter = new ThreadLocal<Int32>( valueFactory: () => 0, trackAllValues: true ); //BUG is this wrong? how else do we reset all the counters?
                return true;
            } );
        }

        /// <summary>
        /// <para>Returns a copy of this <see cref="ConcurrentList{TType}"/> at this moment in time.</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TType> Clone() => this.Read( func: () => this.TheList.ToList() );

        /// <summary>
        /// Signal that this <see cref="ConcurrentList{TType}"/> will not be modified any more.
        /// <para>Blocks.</para>
        /// </summary>
        /// <seealso cref="AllowModifications"/>
        /// <seealso cref="IsReadOnly"/>
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
        /// <para>Determines whether the <paramref name="item"/> is in this <see cref="ConcurrentList{TType}"/> at this moment in time.</para>
        /// </summary>
        public Boolean Contains( TType item ) => this.Read( func: () => this.TheList.Contains( item: item ) );

        /// <summary>
        /// Copies the entire <see cref="ConcurrentList{TType}"/> to the <paramref name="array"/>, starting at the specified index in the target array.
        /// </summary>
        /// <param name="array">     </param>
        /// <param name="arrayIndex"></param>
        public void CopyTo( TType[] array, Int32 arrayIndex ) {
            if ( array is null ) {
                throw new ArgumentNullException( paramName: nameof( array ) );
            }

            this.Read( func: () => {
                this.TheList.CopyTo( array: array, arrayIndex: arrayIndex );
                return true;
            } );
        }

        public Boolean Equals( IEnumerable<TType> other ) => Equals( lhs: this, rhs: other );

        /// <summary>
        /// The <seealso cref="List{T}.Capacity"/> is resized down to the <seealso cref="List{T}.Count"/>.
        /// </summary>
        public void FixCapacity() =>
            this.Write( func: () => {
                this.TheList.Capacity = this.TheList.Count;
                return true;
            } );

        /// <summary>
        /// <para>Returns an enumerator that iterates through a <see cref="Clone"/> of this <see cref="ConcurrentList{TType}"/> .</para>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TType> GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

        /// <summary>
        /// <para>Searches at this moment in time for the first occurrence of <paramref name="item"/> and returns the zero-based index, or -1 if not found.</para>
        /// </summary>
        /// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}"/>.</param>
        public Int32 IndexOf( TType item ) => this.Read( func: () => this.TheList.IndexOf( item: item ) );

        /// <summary>
        /// <para>Requests an insert of the <paramref name="item"/> into this <see cref="ConcurrentList{TType}"/> at the specified <paramref name="index"/>.</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"> </param>
        public void Insert( Int32 index, TType item ) {
            if ( !this.AllowModifications() ) {
                return;
            }

            this.Write( func: () => {
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
        /// <para>Returns true if the request to remove <paramref name="item"/> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove( TType item ) => this.Remove( item: item, afterRemoval: null );

        /// <summary>
        /// <para>Returns true if the request to remove <paramref name="item"/> was posted.</para>
        /// </summary>
        /// <param name="item">        </param>
        /// <param name="afterRemoval"></param>
        /// <returns></returns>
        public Boolean Remove( TType item, [CanBeNull] Action afterRemoval ) {
            if ( !this.AllowModifications() ) {
                return false;
            }

            return this.Write( func: () => {
                var result = this.TheList.Remove( item: item );
                if ( result ) {
                    this.AnItemHasBeenRemoved( afterRemoval );
                }

                return result;
            } );
        }

        public void RemoveAt( Int32 index ) {
            index.Should().BeGreaterOrEqualTo( expected: 0 );
            if ( index < 0 ) {
                return;
            }

            if ( !this.AllowModifications() ) {
                return;
            }

            this.Write( func: () => {
                try {
                    index.Should().BeLessOrEqualTo( expected: this.TheList.Count );

                    if ( index < this.TheList.Count ) {
                        this.TheList.RemoveAt( index: index );
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
        /// Harker's shuffle version. Untested!
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="iterations">      </param>
        /// <param name="howLong">         </param>
        /// <param name="orUntilCancelled"></param>
        [Experimental( message: "Untested" )]
        public UInt32 Shuffle( Int32 iterations = 1, TimeSpan? howLong = null, SimpleCancel orUntilCancelled = null ) {
            var stopWatch = StopWatch.StartNew();

            if ( orUntilCancelled is null ) {
                orUntilCancelled = new SimpleCancel();
            }

            var counter = 0U;
            var itemCount = this.Count;

            if ( iterations < 1 ) {
                iterations = 1;
            }

            iterations *= itemCount;

            do {
                var a = 0.Next( maxValue: itemCount );
                var b = 0.Next( maxValue: itemCount );
                var temp = this[index: a];
                this[index: a] = this[index: b];
                this[index: b] = temp;
                --iterations;
                counter++;
                if ( howLong.HasValue && stopWatch.Elapsed > howLong.Value ) {
                    orUntilCancelled.RequestCancel();
                }

                if ( !iterations.Any() ) {
                    orUntilCancelled.RequestCancel();
                }
            } while ( !orUntilCancelled.HaveAnyCancellationsBeenRequested() );

            return counter;
        }

        public Boolean TryAdd( TType item, [CanBeNull] Action afterAdd = null ) => this.Add( item: item, afterAdd: afterAdd );

        public Boolean TryCatchup( TimeSpan timeout ) {
            if ( !this.AnyWritesPending() ) {
                return true;
            }

            var gotLock = false;
            try {
                if ( !this.ReaderWriter.TryEnterWriteLock( timeout: timeout ) ) {
                    return false;
                }

                gotLock = true;
                while ( this.InputBuffer.TryDequeue( result: out var bob ) ) {
                    this.TheList.Add( item: bob );
                    this.AnItemHasBeenAdded();
                }

                return true;
            }
            finally {
                if ( gotLock ) {
                    this.ReaderWriter.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// <para>Try to get an item in this <see cref="ConcurrentList{TType}"/> by index.</para>
        /// <para>Returns true if the request was posted to the internal dataflow.</para>
        /// </summary>
        /// <param name="index">   </param>
        /// <param name="afterGet">Action to be ran after the item at the <paramref name="index"/> is got.</param>
        public Boolean TryGet( Int32 index, [CanBeNull] Action<TType> afterGet ) {
            if ( index < 0 ) {
                return false;
            }

            return this.Read( func: () => {
                if ( index >= this.TheList.Count ) {
                    return false;
                }

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