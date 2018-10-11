// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConcurrentHashset.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ConcurrentHashset.cs" was last formatted by Protiguous on 2018/07/10 at 8:50 PM.

namespace Librainian.Collections
{

    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///     Threadsafe set. Does not allow nulls inside the set.
    ///     <para>Add will throw an <see cref="ArgumentNullException" /> on <see cref="Add" /> ing a null</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>Class designed by Rick Harker</remarks>
    [Serializable]
    [JsonObject]
    public class ConcurrentHashset<T> : IEnumerable<T> //, ISet<T>  //TODO someday add in set theory.. someday..
    {

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<T, Object> Set { get; }

        public Int32 Count => this.Set.Count;

        public ConcurrentHashset([NotNull] IEnumerable<T> list) : this(Environment.ProcessorCount)
        {
            if (list == null) { throw new ArgumentNullException(paramName: nameof(list)); }

            this.AddRange(list);
        }

        public ConcurrentHashset(Int32 concurrency) => this.Set = new ConcurrentDictionary<T, Object>(concurrency, 3);

        public ConcurrentHashset() => this.Set = new ConcurrentDictionary<T, Object>();

        public void Add([NotNull] T item)
        {
            if (item == null) { throw new ArgumentNullException(paramName: nameof(item)); }

            this.Set[item] = null;
        }

        public void AddRange([NotNull] IEnumerable<T> items)
        {
            if (items == null) { throw new ArgumentNullException(paramName: nameof(items)); }

            Parallel.ForEach(items.Where(type => type != null).AsParallel(), this.Add);
        }

        public void Clear() => this.Set.Clear();

        public Boolean Contains([NotNull] T item)
        {
            if (item == null) { throw new ArgumentNullException(paramName: nameof(item)); }

            return this.Set.ContainsKey(item);
        }

        public T Find([NotNull] T item)
        {
            if (item == null) { throw new ArgumentNullException(paramName: nameof(item)); }

            foreach (var pair in this.Set)
            {
                if (Equals(pair.Key, item)) { return pair.Key; }
            }

            return default;
        }

        public IEnumerator<T> GetEnumerator() => this.Set.Keys.GetEnumerator();

        public Boolean Remove([NotNull] T item)
        {
            if (item == null) { throw new ArgumentNullException(paramName: nameof(item)); }

            return this.Set.TryRemove(item, out _);
        }

        /// <summary>
        ///     Replace left with right. ( <see cref="Remove" /><paramref name="left" />, then <see cref="Add" />
        ///     <paramref name="right" />)
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public void Replace([CanBeNull] T left, [CanBeNull] T right)
        {

            if (left != null) { this.Remove(left); }

            if (right != null) { this.Add(right); }
        }

        /// <summary>
        ///     Set the tag on an item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Boolean Tag([NotNull] T item, [CanBeNull] Object tag)
        {
            if (item == null) { throw new ArgumentNullException(paramName: nameof(item)); }

            this.Set[item] = tag;

            return true;
        }

        /// <summary>
        ///     Get the tag on an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [CanBeNull]
        public Object Tag([NotNull] T item)
        {
            if (item == null) { throw new ArgumentNullException(paramName: nameof(item)); }

            this.Set.TryGetValue(item, out var tag);

            return tag;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}