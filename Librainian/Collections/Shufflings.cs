// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Shufflings.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Shufflings.cs" was last formatted by Protiguous on 2018/07/26 at 5:28 PM.

namespace Librainian.Collections {

    using Converters;
    using Extensions;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Maths;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Threading;

    public static class Shufflings {

        /// <summary>
        ///     <para>Shuffle an array[] in <paramref name="iterations" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">     </param>
        /// <param name="iterations"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        [Obsolete("Bad, Broken, and untested. Just an idea to learn with. See also Shuffle<List>()")]
        public static void Shuffle<T>([NotNull] this T[] array, Int32 iterations = 1) {
            if (array == null) { throw new ArgumentNullException(nameof(array)); }

            if (iterations < 1) { iterations = 1; }

            if (array.Length < 1) {
                return; //nothing to shuffle
            }

            while (iterations > 0) {
                iterations--;

                // make a copy of all items
                var bag = new ConcurrentBag<T>( array );
                bag.Should().NotBeEmpty();
                var originalcount = bag.Count;

                var sqrt = (Int32)Math.Sqrt(d: originalcount);

                if (sqrt <= 1) { sqrt = 1; }

                // make some buckets.
                var buckets = new List<ConcurrentBag<T>>(capacity: sqrt);
                buckets.AddRange(collection: 1.To(end: sqrt).Select( i => new ConcurrentBag<T>()));

                // pull the items out of the bag, and put them into a random bucket each
                T item;

                while (bag.TryTake(result: out item)) {
                    var index = 0.Next(maxValue: sqrt);
                    buckets[index: index].Add(item: item);
                }

                bag.Should().BeEmpty(because: "All items should have been taken out of the bag.");

                while (bag.Count < originalcount) {
                    var index = 0.Next(maxValue: buckets.Count);
                    var bucket = buckets[index: index];

                    if (bucket.TryTake(result: out item)) { bag.Add(item: item); }

                    if (bucket.IsEmpty) { buckets.Remove(item: bucket); }
                }

                bag.Count.Should().Be(expected: originalcount);

                // put them back into the array
                var newArray = bag.ToArray();
                newArray.CopyTo(array: array, index: 0);
            }
        }

        /// <summary>
        ///     <para>Shuffle a list in <paramref name="iterations" />.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">            </param>
        /// <param name="iterations">      </param>
        /// <param name="shufflingType">   </param>
        /// <param name="forHowLong">      </param>
        /// <param name="orUntilCancelled"></param>
        /// <example>Deck.Shuffle( 7 );</example>
        public static void Shuffle<T>([NotNull] this List<T> list, Int32 iterations = 1, ShufflingType shufflingType = ShufflingType.AutoChoice, TimeSpan? forHowLong = null,
            [CanBeNull] SimpleCancel orUntilCancelled = null) {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            try {
                if (!list.Any()) {
                    return; //nothing to shuffle
                }

                if (!iterations.Any()) { iterations = 1; }

                switch (shufflingType) {
                    case ShufflingType.ByGuid: {
                            CollectionExtensions.ShuffleByGuid(list: ref list, iterations: iterations);

                            break;
                        }

                    case ShufflingType.ByRandom: {
                            ShuffleByRandomThenByRandom(list: ref list, iterations: iterations);

                            break;
                        }

                    case ShufflingType.ByHarker: {
                            ShuffleByHarker(list: list, iterations: iterations, forHowLong: forHowLong, orUntilCancelled: orUntilCancelled);

                            break;
                        }

                    case ShufflingType.ByBags: {
                            ShuffleByBags(list: ref list, iterations: iterations, originalcount: list.LongCount());

                            break;
                        }

                    case ShufflingType.AutoChoice: {
                            ShuffleByHarker(list: list, iterations: iterations, forHowLong: forHowLong, orUntilCancelled: orUntilCancelled);

                            break;
                        }

                    default: throw new ArgumentOutOfRangeException(nameof(shufflingType));
                }
            }
            catch (IndexOutOfRangeException exception) { exception.Log(); }
        }

        /// <summary>
        ///     Untested for speed and cpu/threading impact. Also, a lot of elements will/could NOT be shuffled much.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">         </param>
        /// <param name="iterations">   </param>
        /// <param name="originalcount"></param>
        public static void ShuffleByBags<T>(ref List<T> list, Int32 iterations, Int64 originalcount) {
            var bag = new ConcurrentBag<T>();

            while (iterations > 0) {
                iterations--;

                bag.AddRange(items: list.AsParallel());
                bag.Should().NotBeEmpty(because: "made an unordered copy of all items");

                list.Clear();
                list.Should().BeEmpty(because: "emptied the original list");

                list.AddRange(collection: bag);
                list.LongCount().Should().Be(expected: originalcount);

                bag.RemoveAll();
            }
        }

        /// <summary>
        ///     Not cryptographically guaranteed or tested to be the most performant, but it *should* shuffle *well enough* in
        ///     reasonable time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to be shuffled.</param>
        /// <param name="iterations">At least 1 iterations to be done over the whole list.</param>
        /// <param name="forHowLong">Or for how long to run.</param>
        /// <param name="orUntilCancelled">Or until cancelled.</param>
        /// <param name="token">Or until cancelled.</param>
        public static void ShuffleByHarker<T>([NotNull] List<T> list, Int32 iterations = 1, TimeSpan? forHowLong = null, [CanBeNull] SimpleCancel orUntilCancelled = null, CancellationToken? token = null) {
            if (list == null) { throw new ArgumentNullException(nameof(list)); }

            Stopwatch started = null;

            if (forHowLong.HasValue) {
                started = Stopwatch.StartNew(); //don't allocate a stopwatch unless we're waiting for time to pass.
            }

            var itemCount = list.Count;

            var left = new TranslateBytesToInt32 {
                Bytes = new Byte[itemCount * sizeof(Int32)]
            };

            var right = new TranslateBytesToInt32 {
                Bytes = new Byte[itemCount * sizeof(Int32)]
            };

            var leftTracker = new ReaderWriterLockSlim[itemCount];

            for (var i = 0; i < leftTracker.Length; i++) { leftTracker[i] = new ReaderWriterLockSlim(); }

            var rightTracker = new ReaderWriterLockSlim[itemCount];

            for (var i = 0; i < rightTracker.Length; i++) { rightTracker[i] = new ReaderWriterLockSlim(); }

            if (!token.HasValue) { token = CancellationToken.None; }

            var parallelOptions = new ParallelOptions {
                CancellationToken = token.Value,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            do {
                Randem.Instance.NextBytes(left.Bytes); //fill the left buffer with random numbers
                Randem.Instance.NextBytes(right.Bytes); //fill the right buffer with random numbers

                //I don't know how well the list will handle this Parallel.For. It needs tested. I can think values are going to overwrite each other and some may end up lost.
                Parallel.For(0, itemCount, parallelOptions, index => {

                    //so.. how badly will this fail? race conditions and all..
                    //and.. if we're locking, then is there *any* benefit to using Parallel.For?

                    if (leftTracker[index].TryEnterWriteLock(0) && rightTracker[index].TryEnterWriteLock(0)) {
                        try {
                            var indexA = left.Ints[index];
                            var indexB = right.Ints[index];

                            var c = list[indexA];
                            list[indexA] = list[indexB];
                            list[indexB] = c;
                        }
                        finally {
                            rightTracker[index].ExitWriteLock();
                            leftTracker[index].ExitWriteLock();
                        }
                    }
                });

                --iterations;

                if (token.Value.IsCancellationRequested) { return; }

                if (forHowLong.HasValue) {
                    iterations++; //we're waiting for time. reincrement the counter.

                    if (started.Elapsed > forHowLong.Value) { return; }
                }

                if (orUntilCancelled != null) {
                    iterations++; //we're waiting for a cancellation. reincrement the counter.

                    if (orUntilCancelled.HaveAnyCancellationsBeenRequested()) { return; }
                }
            } while (iterations.Any());
        }

        /// <summary>
        ///     Shuffle the whole list using OrderBy and ThenBy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="iterations"></param>
        public static void ShuffleByRandomThenByRandom<T>(ref List<T> list, Int32 iterations = 1) {
            while (iterations.Any()) {
                iterations--;
                list = list.OrderBy(keySelector: o => Randem.Next()).ThenBy(keySelector: o => Randem.Next()).ToList();
            }
        }
    }
}