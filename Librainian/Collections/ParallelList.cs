// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ParallelList.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ParallelList.cs" was last formatted by Protiguous on 2018/07/10 at 8:51 PM.

namespace Librainian.Collections {

    using JetBrains.Annotations;
    using Magic;
    using Measurement.Time;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Threading;

    /// <summary>
    ///     <para>A thread safe list. Uses the Microsoft TPL dataflow behind the scene.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>This class was created on a spur of the moment idea, and is thoroughly untested.</remarks>
    /// <copyright>
    ///     Protiguous 2018
    /// </copyright>
    [Obsolete("Use ConcurrentList instead.")]
    [JsonObject]

    // ReSharper disable once UseNameofExpression
    [DebuggerDisplay("Count={Count}")]
    public sealed class ParallelList<T> : ABetterClassDispose, IList<T> {

        /// <summary>
        ///     <para>Tracks count of times this <see cref="ParallelList{TType}" /> has been marked as <see cref="Complete" />.</para>
        /// </summary>
        /// <see cref="AllowModifications" />
        private Int64 _markedAsCompleteCounter;

        /// <summary>
        ///     <para>Provide a dataflow block to process messages in a serial fashion.</para>
        /// </summary>
        [NotNull]
        private ActionBlock<Action> ActionBlock { get; } = new ActionBlock<Action>(action => action(), new ExecutionDataflowBlockOptions {
            SingleProducerConstrained = false,
            MaxDegreeOfParallelism = 1
        });

        /// <summary>
        ///     Internal item counter.
        /// </summary>
        [NotNull]
        private ThreadLocal<Int32> ItemCounter { get; set; } = new ThreadLocal<Int32>(valueFactory: () => 0, trackAllValues: true);

        /// <summary>
        ///     <para>The internal list actually used.</para>
        /// </summary>
        [JsonProperty]
        [NotNull]
        private List<T> List { get; } = new List<T>();

        [NotNull]
        private ReaderWriterLockSlim ReaderWriter { get; }

        private ThreadLocal<ManualResetEventSlim> Slims { get; } = new ThreadLocal<ManualResetEventSlim>(valueFactory: () => new ManualResetEventSlim(initialState: false), trackAllValues: true);

        [NotNull]
        private ThreadLocal<Int32> WaitingToBeAddedCounter { get; } = new ThreadLocal<Int32>(trackAllValues: true);

        [NotNull]
        private ThreadLocal<Int32> WaitingToBeChangedCounter { get; } = new ThreadLocal<Int32>(trackAllValues: true);

        [NotNull]
        private ThreadLocal<Int32> WaitingToBeInsertedCounter { get; } = new ThreadLocal<Int32>(trackAllValues: true);

        [NotNull]
        private ThreadLocal<Int32> WaitingToBeRemovedCounter { get; } = new ThreadLocal<Int32>(trackAllValues: true);

        /// <summary>
        ///     Returns true if this <see cref="ParallelList{TType}" /> has not been marked as <see cref="Complete" />.
        /// </summary>
        public Boolean AllowModifications {
            get {
                if (this.IsReadOnly) { return false; }

                return Interlocked.Read(location: ref this._markedAsCompleteCounter) == 0;
            }
        }

        /// <summary>
        /// </summary>
        /// <see cref="CatchUp" />
        public Boolean AnyWritesPending => 0 == this.CountOfItemsWaitingToBeAdded && 0 == this.CountOfItemsWaitingToBeChanged && 0 == this.CountOfItemsWaitingToBeInserted;

        /// <summary>
        ///     <para>Count of items currently in this <see cref="ParallelList{TType}" />.</para>
        /// </summary>
        public Int32 Count => this.ItemCounter.Values.Aggregate(seed: 0, func: (current, variable) => current + variable);

        /// <summary>
        ///     <para>Returns the count of items waiting to be added to this <see cref="ParallelList{TType}" />.</para>
        /// </summary>
        public Int32 CountOfItemsWaitingToBeAdded => this.WaitingToBeAddedCounter.Values.Aggregate(seed: 0, func: (current, variable) => current + variable);

        /// <summary>
        ///     <para>Returns the count of items waiting to be changed in this <see cref="ParallelList{TType}" />.</para>
        /// </summary>
        public Int32 CountOfItemsWaitingToBeChanged => this.WaitingToBeChangedCounter.Values.Aggregate(seed: 0, func: (current, variable) => current + variable);

        /// <summary>
        ///     <para>Returns the count of items waiting to be inserted to this <see cref="ParallelList{TType}" />.</para>
        /// </summary>
        public Int32 CountOfItemsWaitingToBeInserted => this.WaitingToBeInsertedCounter.Values.Aggregate(seed: 0, func: (current, variable) => current + variable);

        /// <summary>
        /// </summary>
        /// <see cref="AllowModifications" />
        public Boolean IsReadOnly { get; private set; }

        public SpanOfTime TimeoutForReads { get; set; }

        public SpanOfTime TimeoutForWrites { get; set; }

        [CanBeNull]
        public T this[Int32 index] {
            [CanBeNull]
            get {
                if (index > 0 && index < this.List.Count) { return this.Read(func: () => this.List[index: index]); }

                return default;
            }

            set {
                if (!this.AllowModifications) { return; }

                this.RequestToChangeAnItem();

                this.ActionBlock.Post(item: () => this.Write(func: () => {
                    if (!this.AllowModifications) { return false; }

                    this.List[index: index] = value;
                    this.AnItemHasBeenChanged();

                    return true;
                }));
            }
        }

        private ParallelList() {
            this.ReaderWriter = new ReaderWriterLockSlim(recursionPolicy: LockRecursionPolicy.SupportsRecursion);
            this.TimeoutForReads = Minutes.One;
            this.TimeoutForWrites = Minutes.One;
        }

        /// <summary>
        /// </summary>
        /// <param name="readTimeout"> </param>
        /// <param name="writeTimeout"></param>
        public ParallelList(SpanOfTime readTimeout, SpanOfTime writeTimeout) : this() {
            this.TimeoutForReads = readTimeout;

            this.TimeoutForWrites = writeTimeout;
        }

        private void AnItemHasBeenAdded() {
            this.WaitingToBeAddedCounter.Value--;
            this.ItemCounter.Value++;
        }

        private void AnItemHasBeenChanged() => this.WaitingToBeChangedCounter.Value--;

        private void AnItemHasBeenInserted() => this.WaitingToBeInsertedCounter.Value--;

        private void AnItemHasBeenRemoved([CanBeNull] Action action = null) {
            this.WaitingToBeRemovedCounter.Value--;
            this.ItemCounter.Value--;
            action?.Invoke();
        }

        /// <summary>
        ///     <para>Filter read requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private TFuncResult Read<TFuncResult>([CanBeNull] Func<TFuncResult> func) {
            if (!this.AllowModifications && func != null) {
                return func(); //list has been marked to not allow any more modifications, go ahead and perform the read function.
            }

            if (!this.ReaderWriter.TryEnterUpgradeableReadLock(timeout: this.TimeoutForReads)) { return default; }

            try {
                if (func != null) { return func(); }
            }
            finally { this.ReaderWriter.ExitUpgradeableReadLock(); }

            return default;
        }

        private void RequestToAddAnItem() => this.WaitingToBeAddedCounter.Value++;

        private void RequestToChangeAnItem() => this.WaitingToBeChangedCounter.Value++;

        private void RequestToInsertAnItem() => this.WaitingToBeInsertedCounter.Value++;

        private void RequestToRemoveAnItem() => this.WaitingToBeRemovedCounter.Value++;

        /// <summary>
        ///     <para>Filter write requests through a <see cref="ReaderWriterLockSlim" />.</para>
        /// </summary>
        /// <typeparam name="TFuncResult"></typeparam>
        /// <param name="func">                         </param>
        /// <param name="ignoreAllowModificationsCheck"></param>
        /// <returns></returns>
        /// <see cref="CatchUp" />
        [CanBeNull]
        private TFuncResult Write<TFuncResult>([CanBeNull] Func<TFuncResult> func, Boolean ignoreAllowModificationsCheck = false) {
            if (!ignoreAllowModificationsCheck) {
                if (!this.AllowModifications && func != null) { return default; }
            }

            //BUG what if we want a clone of the list, but it has been marked as !this.AllowModifications

            if (!this.ReaderWriter.TryEnterWriteLock(timeout: this.TimeoutForWrites)) { return default; }

            try {
                if (func != null) { return func(); }
            }
            finally { this.ReaderWriter.ExitWriteLock(); }

            return default;
        }

        /// <summary>
        ///     <para>
        ///         Add the
        ///         <typeparam name="T">item</typeparam>
        ///         to the end of this <see cref="ParallelList{TType}" />.
        ///     </para>
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) => this.Add(item: item, afterAdd: null);

        /// <summary>
        ///     <para>
        ///         Add the
        ///         <typeparam name="T">item</typeparam>
        ///         to the end of this <see cref="ParallelList{TType}" />.
        ///     </para>
        /// </summary>
        /// <param name="item">    </param>
        /// <param name="afterAdd"></param>
        /// <returns></returns>
        public Boolean Add(T item, [CanBeNull] Action afterAdd) {
            if (!this.AllowModifications) { return false; }

            this.RequestToAddAnItem();

            return this.ActionBlock.Post(item: () => this.Write(func: () => {
                try {
                    this.List.Add(item: item);

                    return true;
                }
                finally {
                    this.AnItemHasBeenAdded();
                    afterAdd?.Invoke();
                }
            }));
        }

        public Boolean AddAndWait(T item, CancellationToken cancellationToken = default, TimeSpan timeout = default) {

            //var slim = new ManualResetEventSlim( initialState: false );
            this.Slims.Value.Reset();

            this.Add(item: item, afterAdd: () => this.Slims.Value.Set());

            try {
                if (default != timeout && default != cancellationToken) { return this.Slims.Value.Wait(timeout: timeout, cancellationToken: cancellationToken); }

                if (default != timeout) { return this.ActionBlock.Completion.Wait(timeout: timeout); }

                this.Slims.Value.Wait(cancellationToken: cancellationToken);

                return true;
            }
            catch (OperationCanceledException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (ObjectDisposedException) { }
            catch (AggregateException) { }

            return false;
        }

        public async Task AddAsync(T item, [CanBeNull] Action afterAdd = null) =>
                    await Task.Run(() => {
                        this.TryAdd(item: item, afterAdd: afterAdd);
                    }).NoUI();

        /// <summary>
        ///     Add a collection of items.
        /// </summary>
        /// <param name="items">          </param>
        /// <param name="useParallels">   Enables parallelization of the <paramref name="items" />.</param>
        /// <param name="afterEachAdd">   <see cref="Action" /> to perform after each add.</param>
        /// <param name="afterRangeAdded"><see cref="Action" /> to perform after range added.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddRange([NotNull] IEnumerable<T> items, Byte useParallels = 0, [CanBeNull] Action afterEachAdd = null, [CanBeNull] Action afterRangeAdded = null) {
            if (null == items) { throw new ArgumentNullException(nameof(items)); }

            if (!this.AllowModifications) { return; }

            try {
                if (useParallels >= 1) { items.AsParallel().WithDegreeOfParallelism(degreeOfParallelism: useParallels).ForAll(item => this.TryAdd(item: item, afterAdd: afterEachAdd)); }
                else {
                    foreach (var item in items) { this.TryAdd(item: item, afterAdd: afterEachAdd); }
                }
            }
            finally { afterRangeAdded?.Invoke(); }
        }

        [NotNull]
        public Task AddRangeAsync([CanBeNull] IEnumerable<T> items) =>
                    Task.Run(() => {
                        if (items != null) { this.AddRange(items: items); }
                    });

        /// <summary>
        ///     <para>
        ///         Blocks until the list has no write operations pending, until the <paramref name="timeout" />, or the
        ///         <paramref name="cancellationToken" /> is set.
        ///     </para>
        /// </summary>
        /// <param name="timeout">          </param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns true if list is caught up. (No write operations pending)</returns>
        public Boolean CatchUp(SpanOfTime timeout = default, CancellationToken cancellationToken = default) {
            if (timeout == default) { timeout = this.TimeoutForWrites; }

            var interval = Milliseconds.Hertz111;
            var stopWatch = Stopwatch.StartNew();

            while (this.AllowModifications) {
                if (stopWatch.Elapsed > timeout) { break; }

                if (!this.AnyWritesPending) { break; }

                Task.Delay(delay: interval, cancellationToken: cancellationToken).Wait(cancellationToken: cancellationToken);
            }

            return this.AnyWritesPending;
        }

        /// <summary>
        ///     Mark this <see cref="ParallelList{TType}" /> to be cleared.
        /// </summary>
        public void Clear() {
            if (!this.AllowModifications) { return; }

            this.ActionBlock.Post(item: () => this.Write(func: () => {
                this.List.Clear();
                this.ItemCounter = new ThreadLocal<Int32>(valueFactory: () => 0, trackAllValues: true); //BUG is this correct?

                return true;
            }));
        }

        /// <summary>
        ///     <para>Returns a copy of this <see cref="ParallelList{TType}" /> as this moment in time.</para>
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public List<T> Clone() =>
            this.Write(func: () => {
                var copy = this.List.ToList();

                return copy;
            }, ignoreAllowModificationsCheck: true);

        /// <summary>
        ///     Signal that this <see cref="ParallelList{TType}" /> will not be modified any more.
        /// </summary>
        /// <see cref="AllowModifications" />
        /// <see cref="IsReadOnly" />
        public void Complete() {
            try { this.ActionBlock.Complete(); }
            finally {
                Interlocked.Increment(location: ref this._markedAsCompleteCounter);
                this.IsReadOnly = true;
            }
        }

        /// <summary>
        ///     <para>Signal that nothing else will be added or removed from this <see cref="ParallelList{TType}" /> and then,</para>
        ///     <para>
        ///         If both <paramref name="timeout" /> and <paramref name="cancellationToken" /> are provided,
        ///         <see cref="Task.Wait()" /> with it.
        ///     </para>
        ///     <para>Otherwise, if only a <paramref name="timeout" /> is provided, <see cref="Task.Wait()" /> with it.</para>
        ///     <para>Otherwise, if only a <paramref name="cancellationToken" /> is provided, <see cref="Task.Wait()" /> with it.</para>
        /// </summary>
        /// <returns>
        ///     Returns <see cref="Boolean.True" /> if the Task completed execution within the allotted time or has already
        ///     waited.
        /// </returns>
        public Boolean CompleteAndWait(CancellationToken cancellationToken = default, TimeSpan timeout = default) {
            try {
                this.Complete();

                if (default != timeout && default != cancellationToken) { return this.ActionBlock.Completion.Wait(millisecondsTimeout: (Int32)timeout.TotalMilliseconds, cancellationToken: cancellationToken); }

                if (default != timeout) { return this.ActionBlock.Completion.Wait(timeout: timeout); }

                if (default != cancellationToken) {
                    try { this.ActionBlock.Completion.Wait(cancellationToken: cancellationToken); }
                    catch (OperationCanceledException) {
                        return false; //BUG Is this correct?
                    }
                    catch (ObjectDisposedException) { return false; }
                    catch (AggregateException) { return false; }

                    return true;
                }

                // ReSharper disable once MethodSupportsCancellation
                this.ActionBlock.Completion.Wait();

                return true;
            }
            finally {
                this.List.Capacity = this.List.Count; //optimize the memory used by this list.
            }
        }

        /// <summary>
        ///     <para>
        ///         Determines whether the <paramref name="item" /> is in this <see cref="ParallelList{TType}" /> at this moment
        ///         in time.
        ///     </para>
        /// </summary>
        public Boolean Contains(T item) => this.Read(func: () => this.List.Contains(item: item));

        /// <summary>
        ///     Copies the entire <see cref="ParallelList{TType}" /> to the <paramref name="array" />, starting at the specified
        ///     index in the target array.
        /// </summary>
        /// <param name="array">     </param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, Int32 arrayIndex) {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            this.Read(func: () => {
                this.List.CopyTo(array: array, arrayIndex: arrayIndex);

                return true;
            });
        }

        /// <summary>
        ///     Dispose any disposable members.
        /// </summary>
        public override void DisposeManaged() {
            using (this.ReaderWriter) { }

            using (this.Slims) { }
        }

        /// <summary>
        ///     <para>
        ///         Returns an enumerator that iterates through a <see cref="Clone" /> of this <see cref="ParallelList{TType}" />
        ///         .
        ///     </para>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => this.Clone().GetEnumerator();

        /// <summary>
        ///     <para>
        ///         Searches at this moment in time for the first occurrence of <paramref name="item" /> and returns the
        ///         zero-based index, or -1 if not found.
        ///     </para>
        /// </summary>
        /// <param name="item">The object to locate in this <see cref="ParallelList{TType}" />.</param>
        public Int32 IndexOf(T item) => this.Read(func: () => this.List.IndexOf(item: item));

        /// <summary>
        ///     <para>
        ///         Requests an insert of the <paramref name="item" /> into this <see cref="ParallelList{TType}" /> at the
        ///         specified <paramref name="index" />.
        ///     </para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"> </param>
        public void Insert(Int32 index, T item) {
            if (!this.AllowModifications) { return; }

            this.RequestToInsertAnItem();

            this.ActionBlock.Post(item: () => this.Write(func: () => {
                try {
                    this.List.Insert(index: index, item: item);

                    return true;
                }
                catch (ArgumentOutOfRangeException) { return false; }
                finally { this.AnItemHasBeenInserted(); }
            }));
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
            if (!this.AllowModifications) { return false; }

            this.RequestToRemoveAnItem();

            return this.ActionBlock.Post(item: () => this.Write(func: () => {
                try {
                    this.List.Remove(item: item);

                    return true;
                }
                finally { this.AnItemHasBeenRemoved(afterRemoval); }
            }));
        }

        public void RemoveAt(Int32 index) {
            if (index < 0) { return; }

            if (!this.AllowModifications) { return; }

            this.RequestToRemoveAnItem();

            this.ActionBlock.Post(item: () => this.Write(func: () => {
                try {
                    if (index < this.List.Count) { this.List.RemoveAt(index: index); }
                }
                catch (ArgumentOutOfRangeException) { return false; }
                finally { this.AnItemHasBeenRemoved(); }

                return true;
            }));
        }

        public Boolean TryAdd(T item, [CanBeNull] Action afterAdd = null) => this.Add(item: item, afterAdd: afterAdd);

        /// <summary>
        ///     <para>Try to get an item in this <see cref="ParallelList{TType}" /> by index.</para>
        ///     <para>Returns true if the request was posted to the internal dataflow.</para>
        /// </summary>
        /// <param name="index">   </param>
        /// <param name="afterGet">Action to be ran after the item at the <paramref name="index" /> is got.</param>
        public Boolean TryGet(Int32 index, [CanBeNull] Action<T> afterGet) {
            if (index < 0) { return false; }

            return this.ActionBlock.Post(item: () => this.Read(func: () => {
                if (index >= this.List.Count) { return false; }

                var result = this.List[index: index];
                afterGet?.Invoke(result);

                return true;
            }));
        }

        /// <summary>
        ///     Blocks and waits.
        /// </summary>
        /// <param name="timeout">          </param>
        /// <param name="cancellationToken"></param>
        public void Wait(SpanOfTime timeout = default, CancellationToken cancellationToken = default) => this.CatchUp(timeout: timeout, cancellationToken: cancellationToken);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}