// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentCollection.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ConcurrentCollection.cs" was last formatted by Protiguous on 2018/07/10 at 8:50 PM.

namespace Librainian.Collections
{

    using JetBrains.Annotations;
    using Maths;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;

    public class ConcurrentCollection<T> : IProducerConsumerCollection<T>
    {

        private const Int32 BackoffMaxYields = 8;

        private Node Head { get; set; }

        private T[] SerializationArray { get; set; }

        public Int32 Count {
            get {
                var num = 0;

                for (var node = this.Head; node != null; node = node.MNext) { ++num; }

                return num;
            }
        }

        public Boolean IsSynchronized => false;

        public Object SyncRoot => throw new NotSupportedException("ConcurrentCollection_SyncRoot_NotSupported");

        public ConcurrentCollection() { }

        public ConcurrentCollection([NotNull] IEnumerable<T> collection)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }

            this.InitializeFromCollection(collection: collection);
        }

        private static void CopyRemovedItems(Node head, IList<T> collection, Int32 startIndex, Int32 nodesCount)
        {
            var node = head;

            for (var index = startIndex; index < startIndex + nodesCount; ++index)
            {
                collection[index: index] = node.MValue;
                node = node.MNext;
            }
        }

        private static IEnumerator<T> GetEnumerator(Node head)
        {
            for (var current = head; current != null; current = current.MNext) { yield return current.MValue; }
        }

        private static void ValidatePushPopRangeInput([NotNull] ICollection<T> items, Int32 startIndex, Int32 count)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count), "ConcurrentStack_PushPopRange_CountOutOfRange"); }

            var length = items.Count;

            if (startIndex >= length || startIndex < 0) { throw new ArgumentOutOfRangeException(nameof(startIndex), "ConcurrentStack_PushPopRange_StartOutOfRange"); }

            if (length - count < startIndex) { throw new ArgumentException("ConcurrentStack_PushPopRange_InvalidCount"); }
        }

        private void InitializeFromCollection([NotNull] IEnumerable<T> collection)
        {
            var node = collection.Aggregate<T, Node>(seed: null, func: (current, obj) => new Node(obj)
            {
                MNext = current
            });

            this.Head = node;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Node node1 = null;
            Node node2 = null;

            foreach (var node3 in this.SerializationArray.Select(selector: t => new Node(t)))
            {
                if (node1 == null) { node2 = node3; }
                else { node1.MNext = node3; }

                node1 = node3;
            }

            this.Head = node2;
            this.SerializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context) => this.SerializationArray = this.ToArray();

        private void PushCore(Node head, [NotNull] Node tail)
        {
            var spinWait = new SpinWait();
            var mHead = this.Head;

            do
            {
                spinWait.SpinOnce();
                tail.MNext = this.Head;
            } while (Interlocked.CompareExchange(location1: ref mHead, head, comparand: tail.MNext) != tail.MNext);
        }

        [NotNull]
        private List<T> ToList()
        {
            var list = new List<T>();

            for (var node = this.Head; node != null; node = node.MNext) { list.Add(item: node.MValue); }

            return list;
        }

        private Boolean TryPopCore(out T result)
        {
            if (this.TryPopCore(count: 1, poppedHead: out var poppedHead) == 1)
            {
                result = poppedHead.MValue;

                return true;
            }

            result = default;

            return false;
        }

        private Int32 TryPopCore(Int32 count, [CanBeNull] out Node poppedHead)
        {
            var spinWait = new SpinWait();
            var num1 = 1;

            //var random = new Random( Environment.TickCount & int.MaxValue );
            Node comparand;
            Int32 num2;

            while (true)
            {
                comparand = this.Head;

                if (comparand == null) { break; }

                var node = comparand;

                for (num2 = 1; num2 < count && node.MNext != null; ++num2) { node = node.MNext; }

                var mHead = this.Head;

                if (Interlocked.CompareExchange(location1: ref mHead, node.MNext, comparand: comparand) == comparand) { goto label_9; }

                for (var index = 0; index < num1; ++index) { spinWait.SpinOnce(); }

                num1 = spinWait.NextSpinWillYield ? 1.Next(maxValue: 8) : num1 * 2;
            }

            poppedHead = null;

            return 0;
        label_9:

            poppedHead = comparand;

            return num2;
        }

        public void Clear() => this.Head = null;

        public void CopyTo([NotNull] T[] array, Int32 index)
        {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            this.ToList().CopyTo(array: array, arrayIndex: index);
        }

        public IEnumerator<T> GetEnumerator() => GetEnumerator(head: this.Head);

        public Boolean IsEmpty() => this.Head == null;

        public void Push(T item)
        {
            var node = new Node(item)
            {
                MNext = this.Head
            };

            var mHead = this.Head;

            if (Interlocked.CompareExchange(location1: ref mHead, node, comparand: node.MNext) == node.MNext) { return; }

            this.PushCore(head: node, tail: node);
        }

        public void PushRange([NotNull] T[] items)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            this.PushRange(items: items, startIndex: 0, count: items.Length);
        }

        public void PushRange([NotNull] T[] items, Int32 startIndex, Int32 count)
        {
            ValidatePushPopRangeInput(items: items, startIndex: startIndex, count: count);

            if (count == 0) { return; }

            Node tail;
            var head = tail = new Node(items[startIndex]);

            for (var index = startIndex + 1; index < startIndex + count; ++index)
            {
                head = new Node(items[index])
                {
                    MNext = head
                };
            }

            tail.MNext = this.Head;
            var mHead = this.Head;

            if (Interlocked.CompareExchange(location1: ref mHead, head, comparand: tail.MNext) == tail.MNext) { return; }

            this.PushCore(head: head, tail: tail);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>

        // ReSharper disable once RemoveToList.1
        [NotNull]
        public T[] ToArray() => this.ToList().ToArray();

        public Boolean TryAdd(T item)
        {
            this.Push(item: item);

            return true;
        }

        public Boolean TryPeek(out T result)
        {
            var node = this.Head;

            if (node == null)
            {
                result = default;

                return false;
            }

            result = node.MValue;

            return true;
        }

        public Boolean TryPop(out T result)
        {
            var comparand = this.Head;

            if (comparand != null)
            {
                var mHead = this.Head;

                if (Interlocked.CompareExchange(location1: ref mHead, comparand.MNext, comparand: comparand) != comparand) { return this.TryPopCore(result: out result); }

                result = comparand.MValue;

                return true;
            }

            result = default;

            return false;
        }

        public Int32 TryPopRange([NotNull] T[] items)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            return this.TryPopRange(items: items, startIndex: 0, count: items.Length);
        }

        public Int32 TryPopRange([NotNull] T[] items, Int32 startIndex, Int32 count)
        {
            ValidatePushPopRangeInput(items: items, startIndex: startIndex, count: count);

            if (count == 0) { return 0; }

            var nodesCount = this.TryPopCore(count: count, poppedHead: out var poppedHead);

            if (nodesCount <= 0) { return nodesCount; }

            CopyRemovedItems(head: poppedHead, collection: items, startIndex: startIndex, nodesCount: nodesCount);

            return nodesCount;
        }

        public Boolean TryTake(out T item) => this.TryPop(result: out item);

        void ICollection.CopyTo(Array array, Int32 index)
        {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            if (!(array is T[])) { throw new ArgumentNullException(nameof(array)); }

            this.ToList().CopyTo(array: (T[])array, arrayIndex: index);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private sealed class Node
        {

            internal readonly T MValue;

            internal Node MNext;

            internal Node(T value)
            {
                this.MValue = value;
                this.MNext = null;
            }
        }

        // ReSharper restore RemoveToList.1
    }
}