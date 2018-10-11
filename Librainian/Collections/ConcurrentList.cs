// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentList.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "ConcurrentList.cs" was last formatted by Protiguous on 2018/08/30 at 6:50 AM.

namespace Librainian.Collections {

    using Extensions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     <para>A thread safe generic list.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    ///     <para>This class was created on a spur of the moment idea, and is <b>thoroughly</b> UNTESTED.</para>
    ///     <para>Uses a <see cref="ConcurrentQueue{T}" /> to buffer adds.</para>
    /// </remarks>
    /// <copyright>
    ///     Protiguous@Protiguous.com. Used with full permissions from original copyright holder Rick@AIBrain.org.
    /// </copyright>
    [JsonObject(MemberSerialization.Fields)]

    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("Count={Count}")]
    public class ConcurrentList<T> : IDisposable, IList<T>, IEquatable<IEnumerable<T>> {

        [JsonIgnore]
        public Configuration Config;

        /// <summary>
        ///     A thread-local (threadsafe) <see cref="Random" />.
        /// </summary>
        [NotNull]
        private static Random Randem => ThreadSafeRandom.Value.Value;

        /// <summary>
        ///     Provide to each thread its own <see cref="Random" /> with a random seed.
        /// </summary>
        [NotNull]

        // ReSharper disable once StaticMemberInGenericType
        private static ThreadLocal<Lazy<Random>> ThreadSafeRandom { get; } =
            new ThreadLocal<Lazy<Random>>(() => new Lazy<Random>(() => new Random(Seed: (Int32)(DateTime.Now.Ticks ^ (Thread.CurrentThread.ManagedThreadId + 1)))), true);

        [NotNull]
        private ConcurrentQueue<T> InputBuffer { get; set; } = new ConcurrentQueue<T>();

        /// <summary> threadsafe item counter (so we don't have to enter and exit the readerwriter). </summary>
        private ThreadLocal<Int32> ItemCounter { get; set; } = new ThreadLocal<Int32>(valueFactory: () => 0, trackAllValues: true);

        [JsonIgnore]
        private ReaderWriterLockSlim ReaderWriter { get; set; } = new ReaderWriterLockSlim(recursionPolicy: LockRecursionPolicy.SupportsRecursion);

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
        public Int32 Count => this.ItemCounter.Values.Aggregate(seed: 0, func: (current, variable) => current + variable);

        [JsonIgnore]
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// </summary>
        /// <see cref="AllowModifications" />
        public Boolean IsReadOnly { get; private set; }

        /// <summary>
        ///     If set to false, anything that would normally cause an <see cref="Exception" /> is ignored.
        /// </summary>
        public Boolean ThrowExceptions { get; set; }

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
        public T this[Int32 index] {
            [CanBeNull]
            get {
                if (index < 0 || index > this.TheList.Count) {
                    this.ThrowIfOutOfRange(index);

                    return default;
                }

                return this.Read(func: () => this.TheList[index: index]);
            }

            set {
                if (!this.AllowModifications()) {
                    this.IfDisallowedModificationsThrow();

                    return;
                }

                this.Write(func: () => {
                    if (!this.AllowModifications()) {
                        this.IfDisallowedModificationsThrow();

                        return false;
                    }

                    try { this.TheList[index: index] = value; }
                    catch (ArgumentOutOfRangeException) {
                        this.ThrowIfOutOfRange(index);

                        return false;
                    }

                    return true;
                });
            }
        }

        /// <summary>
        ///     Create an empty list with different timeout values.
        /// </summary>
        /// <param name="enumerable">  Fill the list with the given enumerable.</param>
        /// <param name="readTimeout">Defaults to 60 seconds.</param>
        /// <param name="writeTimeout">Defaults to 60 seconds.</param>
        public ConcurrentList([CanBeNull] IEnumerable<T> enumerable = null, TimeSpan? readTimeout = null, TimeSpan? writeTimeout = null) {

            this.Config.TimeoutForReads = readTimeout ?? TimeSpan.FromSeconds(60);

            this.Config.TimeoutForWrites = writeTimeout ?? TimeSpan.FromSeconds(60);

            if (null != enumerable) { this.AddRange(items: enumerable); }
        }

        private void AnItemHasBeenAdded() => this.ItemCounter.Value++;

        private void AnItemHasBeenRemoved([CanBeNull] Action action = null) {
            this.ItemCounter.Value--;
            action?.Invoke();
        }

        private void IfDisallowedModificationsThrow() {
            if (this.ThrowExceptions) { throw new InvalidOperationException("List does not allow modifications."); }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            if (this.ReaderWriter == null) { this.ReaderWriter = new ReaderWriterLockSlim(recursionPolicy: LockRecursionPolicy.SupportsRecursion); }

            if (this.ItemCounter == null) { this.ItemCounter = new ThreadLocal<Int32>(valueFactory: () => 0, trackAllValues: true); }

            this.ItemCounter.Value += this.TheList.Count;
        }

        /// <summary>
        ///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        [CanBeNull]
        private TFuncResult Read<TFuncResult>([NotNull] Func<TFuncResult> func) {
            if (func == null) { throw new ArgumentNullException(paramName: nameof(func)); }

            if (!this.AllowModifications()) {
                return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
            }

            this.CatchUp();

            if (this.ReaderWriter.TryEnterUpgradeableReadLock(timeout: this.Config.TimeoutForReads)) {
                try { return func(); }
                finally { this.ReaderWriter.ExitUpgradeableReadLock(); }
            }

            return default;
        }

        private void ThrowIfDisposed() {
            if (!this.IsDisposed) { return; }

            if (Debugger.IsAttached) { Debugger.Break(); }

            if (this.ThrowExceptions) { throw new ObjectDisposedException($"{nameof(ConcurrentList<T>)} has already been disposed."); }
        }

        private void ThrowIfOutOfRange(Int32 index) {
            if (this.ThrowExceptions) { throw new ArgumentOutOfRangeException(nameof(index), index, "Value must be 0 or greater."); }
        }

        /// <summary>
        ///     <para>Filter write requests through the <see cref="ReaderWriter" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func">                         </param>
        /// <param name="ignoreAllowModificationsCheck"></param>
        /// <returns></returns>
        /// <see cref="CatchUp" />
        [CanBeNull]
        private TFuncResult Write<TFuncResult>([NotNull] Func<TFuncResult> func, Boolean ignoreAllowModificationsCheck = false) {
            if (func == null) { throw new ArgumentNullException(paramName: nameof(func)); }

            if (!ignoreAllowModificationsCheck && !this.AllowModifications()) { return default; }

            if (!this.ReaderWriter.TryEnterWriteLock(timeout: this.Config.TimeoutForWrites)) { return default; }

            try { return func(); }
            finally { this.ReaderWriter.ExitWriteLock(); }
        }

        /// <summary>
        ///     Static comparison function.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="rhs"> </param>
        /// <returns></returns>
        public static Boolean Equals([CanBeNull] IEnumerable<T> left, [CanBeNull] IEnumerable<T> rhs) {
            if ( ReferenceEquals( left, rhs ) ) { }

            if (left == null || rhs == null) { return false; }

            return left.SequenceEqual(second: rhs);
        }

        /// <summary>
        ///     <para>
        ///         Add the
        ///         <typeparam name="T">item</typeparam>
        ///         to the end of this <see cref="ConcurrentList{TType}" />.
        ///     </para>
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) => this.Add(item: item, afterAdd: null);

        /// <summary>
        ///     <para>
        ///         Add the
        ///         <typeparam name="T">item</typeparam>
        ///         to the end of this <see cref="ConcurrentList{TType}" />.
        ///     </para>
        /// </summary>
        /// <param name="item">    </param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        public Boolean Add(T item, [CanBeNull] Action afterAdd) {
            if (!this.AllowModifications()) {
                this.IfDisallowedModificationsThrow();

                return false;
            }

            return this.Write(func: () => {
                try {
                    this.TheList.Add(item: item);

                    return true;
                }
                finally {
                    this.AnItemHasBeenAdded();
                    afterAdd?.Invoke();
                }
            });
        }

        public Boolean AddAndWait(T item) => this.Add(item: item, afterAdd: this.CatchUp);

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
        public void AddRange([NotNull] IEnumerable<T> items, Byte useParallelism = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null) {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            if (!this.AllowModifications()) {
                this.IfDisallowedModificationsThrow();

                return;
            }

            try {
                if (useParallelism > 0) { items.AsParallel().WithDegreeOfParallelism(degreeOfParallelism: useParallelism).ForAll(item => this.TryAdd(item: item, afterAdd: afterEachAdd)); }
                else {
                    foreach (var item in items) { this.TryAdd(item: item, afterAdd: afterEachAdd); }
                }
            }
            finally { afterRangeAdded?.Invoke(); }
        }

        /// <summary>
        ///     Returns a hot task that needs to be awaited.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="afterEachAdd"></param>
        /// <param name="afterRangeAdded"></param>
        /// <returns></returns>
        [NotNull]
        public Task AddRangeTask([CanBeNull] IEnumerable<T> items, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null) =>
            Task.Run(() => {
                if (items != null) { this.AddRange(items: items, afterEachAdd: afterEachAdd, afterRangeAdded: afterRangeAdded); }
            });

        /// <summary>
        ///     Creates a hot task that needs to be awaited.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        [NotNull]
        public Task<Boolean> AddTask(T item, [CanBeNull] Action afterAdd = null) => Task.Run(function: () => this.TryAdd(item: item, afterAdd: afterAdd));

        /// <summary>
        ///     Returns true if this <see cref="ConcurrentList{TType}" /> has not been marked as <see cref="Complete" />.
        /// </summary>
        public Boolean AllowModifications() => !this.IsReadOnly;

        /// <summary>
        /// </summary>
        /// <see cref="CatchUp" />
        public Boolean AnyWritesPending() => this.InputBuffer.Any();

        /// <summary>
        ///     Blocks, transfers items from <see cref="InputBuffer" />, and then releases lock.
        /// </summary>
        public void CatchUp() {
            if (!this.AnyWritesPending()) { return; }

            try {
                this.ReaderWriter.EnterWriteLock();

                while (this.InputBuffer.TryDequeue(result: out var bob)) {
                    this.TheList.Add(item: bob);
                    this.AnItemHasBeenAdded();
                }
            }
            finally { this.ReaderWriter.ExitWriteLock(); }
        }

        /// <summary>
        ///     Mark this <see cref="ConcurrentList{TType}" /> to be cleared.
        /// </summary>
        public void Clear() {
            if (!this.AllowModifications()) {
                this.IfDisallowedModificationsThrow();

                return;
            }

            this.Write(func: () => {
                this.TheList.Clear();

                //BUG Is this 'new' wrong to do? How else do we reset all the counters? (This is blocked by using "this.Write" at least..)
                this.ItemCounter = new ThreadLocal<Int32>(valueFactory: () => 0, trackAllValues: true);

                return true;
            });
        }

        /// <summary>
        ///     <para>Returns a copy of this <see cref="ConcurrentList{TType}" /> at this moment in time.</para>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public IReadOnlyList<T> Clone() => this.Read(func: () => this.TheList /*.ToList() maybe?*/ ) ?? throw new InvalidOperationException("Possible timeout when waiting to read list.");

        /// <summary>
        ///     Signal that this <see cref="ConcurrentList{TType}" /> will not be modified any more.
        ///     <para>Blocks.</para>
        /// </summary>
        /// <see cref="AllowModifications" />
        /// <see cref="IsReadOnly" />
        public void Complete() {
            try {
                this.CatchUp();
                this.FixCapacity();
            }
            finally { this.IsReadOnly = true; }
        }

        /// <summary>
        ///     <para>
        ///         Determines whether the <paramref name="item" /> is in this <see cref="ConcurrentList{TType}" /> at this
        ///         moment in time.
        ///     </para>
        /// </summary>
        public Boolean Contains(T item) => this.Read(func: () => this.TheList.Contains(item: item));

        /// <summary>
        ///     Copies the entire <see cref="ConcurrentList{TType}" /> to the <paramref name="array" />, starting at the specified
        ///     index in the target array.
        /// </summary>
        /// <param name="array">     </param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, Int32 arrayIndex) {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            this.Read(func: () => {
                this.TheList.CopyTo(array: array, arrayIndex: arrayIndex);

                return true;
            });
        }

        /// <summary>
        ///     Dispose any disposable members with a using statement so we don't have to deal with nulls.
        /// </summary>
        public void Dispose() {
            this.ThrowIfDisposed();

            using (this.ItemCounter) { }

            using (this.ReaderWriter) { }

            this.IsDisposed = true;
        }

        public Boolean Equals(IEnumerable<T> other) => Equals(left: this, rhs: other);

        /// <summary>
        ///     The <see cref="List{T}.Capacity" /> is resized down to the <see cref="List{T}.Count" />.
        /// </summary>
        public void FixCapacity() =>
            this.Write(func: () => {
                this.TheList.TrimExcess();

                return true;
            });

        /// <summary>
        ///     <para>
        ///         Returns an enumerator that iterates through a <see cref="Clone" /> of this
        ///         <see cref="ConcurrentList{TType}" /> .
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

        /// <summary>
        ///     <para>
        ///         Searches at this moment in time for the first occurrence of <paramref name="item" /> and returns the
        ///         zero-based index, or -1 if not found.
        ///     </para>
        /// </summary>
        /// <param name="item">The object to locate in this <see cref="ConcurrentList{TType}" />.</param>
        public Int32 IndexOf(T item) => this.Read(func: () => this.TheList.IndexOf(item: item));

        /// <summary>
        ///     <para>
        ///         Requests an insert of the <paramref name="item" /> into this <see cref="ConcurrentList{TType}" /> at the
        ///         specified <paramref name="index" />.
        ///     </para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"> </param>
        public void Insert(Int32 index, T item) {
            if (!this.AllowModifications()) {
                this.IfDisallowedModificationsThrow();

                return;
            }

            this.Write(func: () => {
                try {
                    this.TheList.Insert(index: index, item: item);
                    this.AnItemHasBeenAdded();

                    return true;
                }
                catch (ArgumentOutOfRangeException) {
                    this.ThrowIfOutOfRange(index);

                    return false;
                }
            });
        }

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove(T item) => this.Remove(item: item, afterRemoval: null);

        /// <summary>
        ///     <para>Returns true if the request to remove <paramref name="item" /> was posted.</para>
        /// </summary>
        /// <param name="item">        </param>
        /// <param name="afterRemoval"></param>
        /// <returns></returns>
        public Boolean Remove(T item, [CanBeNull] Action afterRemoval) {
            if (!this.AllowModifications()) {
                this.IfDisallowedModificationsThrow();

                return false;
            }

            return this.Write(func: () => {
                var result = this.TheList.Remove(item: item);

                if (result) { this.AnItemHasBeenRemoved(afterRemoval); }

                return result;
            });
        }

        public void RemoveAt(Int32 index) {

            if (index < 0) {

                if (this.ThrowExceptions) { throw new ArgumentOutOfRangeException(nameof(index), index, "Value must be 0 or greater."); }

                return;
            }

            if (!this.AllowModifications()) {
                if (this.ThrowExceptions) { throw new InvalidOperationException("List does not allow modifications."); }

                return;
            }

            this.Write(func: () => {
                try {
                    if (index <= this.TheList.Count) {
                        this.TheList.RemoveAt(index: index);
                        this.AnItemHasBeenRemoved();

                        return true;
                    }
                }
                catch (ArgumentOutOfRangeException) { this.ThrowIfOutOfRange(index); }

                return false;
            });
        }

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
        public void ShuffleByHarker(Int32 iterations = 1, TimeSpan? forHowLong = null, CancellationToken? token = null) =>
            this.Write(() => {

                Stopwatch started = null;

                if (forHowLong.HasValue) {
                    started = Stopwatch.StartNew(); //don't allocate a stopwatch unless we're waiting for time to pass.
                }

                var itemCount = this.Count;

                var leftTracker = new ReaderWriterLockSlim[itemCount];

                for (var i = 0; i < leftTracker.Length; i++) { leftTracker[i] = new ReaderWriterLockSlim(); }

                var rightTracker = new ReaderWriterLockSlim[itemCount];

                for (var i = 0; i < rightTracker.Length; i++) { rightTracker[i] = new ReaderWriterLockSlim(); }

                if (!token.HasValue) { token = CancellationToken.None; }

                var parallelOptions = new ParallelOptions {
                    CancellationToken = token.Value,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                var left = new TranslateBytesToInt32 {
                    Bytes = new Byte[itemCount * sizeof(Int32)]
                };

                var right = new TranslateBytesToInt32 {
                    Bytes = new Byte[itemCount * sizeof(Int32)]
                };

                do {
                    Parallel.Invoke(parallelOptions, () => Randem.NextBytes(left.Bytes), () => Randem.NextBytes(right.Bytes));

                    //I don't know how well the list will handle this Parallel.For. It needs tested. I can think values can possibly overwrite each other and some may end up lost.
                    Parallel.For(0, itemCount, parallelOptions, index => {

                        //so.. how badly will this fail? race conditions and all..
                        //and if we're locking, then is there any benefit to using Parallel.For?

                        if (leftTracker[index].TryEnterWriteLock(0) && rightTracker[index].TryEnterWriteLock(0)) {
                            try {
                                var indexA = left.Ints[index];
                                var indexB = right.Ints[index];

                                var c = this.TheList[indexA];
                                this.TheList[indexA] = this.TheList[indexB];
                                this.TheList[indexB] = c;
                            }
                            finally {
                                rightTracker[index].ExitWriteLock();
                                leftTracker[index].ExitWriteLock();
                            }
                        }
                    });

                    --iterations;

                    if (token.Value.IsCancellationRequested) { return true; }

                    if (forHowLong.HasValue) {
                        iterations++; //we're waiting for time. reincrement the counter.

                        if (started.Elapsed > forHowLong.Value) { return true; }
                    }
                } while (iterations.Any());

                return true;
            });

        public Boolean TryAdd(T item, [CanBeNull] Action afterAdd = null) => this.Add(item: item, afterAdd: afterAdd);

        public Boolean TryCatchup(TimeSpan timeout) {
            if (!this.AnyWritesPending()) { return true; }

            var haveLock = false;

            try {
                if (!this.ReaderWriter.TryEnterWriteLock(timeout: timeout)) { return false; }

                haveLock = true;

                while (this.InputBuffer.TryDequeue(result: out var bob)) {
                    this.TheList.Add(item: bob);
                    this.AnItemHasBeenAdded();
                }

                return true;
            }
            finally {
                if (haveLock) { this.ReaderWriter.ExitWriteLock(); }
            }
        }

        /// <summary>
        ///     <para>Try to get an item in this <see cref="ConcurrentList{TType}" /> by index.</para>
        ///     <para>Returns true if the request was posted to the internal dataflow.</para>
        /// </summary>
        /// <param name="index">   </param>
        /// <param name="afterGet">Action to be ran after the item at the <paramref name="index" /> is got.</param>
        public Boolean TryGet(Int32 index, [CanBeNull] Action<T> afterGet) {
            if (index < 0) { return false; }

            return this.Read(func: () => {
                if (index >= this.TheList.Count) {
                    this.ThrowIfOutOfRange(index);

                    return false;
                }

                var result = this.TheList[index: index];
                afterGet?.Invoke(result);

                return true;
            });
        }

        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => this.Clone().GetEnumerator(); //is this the proper way?

        public struct Configuration {

            [JsonProperty]
            public TimeSpan TimeoutForReads { get; set; }

            [JsonProperty]
            public TimeSpan TimeoutForWrites { get; set; }
        }

        [StructLayout(layoutKind: LayoutKind.Explicit)]
        public struct TranslateBytesToInt32 {

            [FieldOffset(offset: 0)]
            public Byte[] Bytes;

            [FieldOffset(offset: 0)]
            public readonly Int32[] Ints;
        }
    }
}