// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "PriorityQueue.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/PriorityQueue.cs" was last cleaned by Protiguous on 2018/05/15 at 10:37 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    ///     The highest priority is the highest
    /// </summary>
    [JsonObject]
    public class PriorityQueue<TValue> : IEnumerable<KeyValuePair<Single, TValue>> {

        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<Single, TValue> Dictionary { get; } = new ConcurrentDictionary<Single, TValue>( concurrencyLevel: Environment.ProcessorCount, capacity: 1 );

        /// <summary>
        ///     Inject the specified priority.
        /// </summary>
        /// <param name="item">    </param>
        /// <param name="priority"></param>
        public void Add( TValue item, Single priority ) =>

            //while ( this.Dictionary.ContainsKey( priority ) ) {
            //    priority += Constants.EpsilonSingle;
            //}
            this.Dictionary[priority] = item;

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator() => this.Dictionary.GetEnumerator();

        //    this.Add( item, priority );
        //    this.ReNormalize();
        //}
        [CanBeNull]
        public TValue PullNext() {
            var highest = this.Dictionary.OrderByDescending( keySelector: pair => pair.Key ).FirstOrDefault();

            return highest.Value;
        }

        // Single priority; switch ( positionial ) { case Positionial.Highest: { priority = this.Dictionary.Max( pair => pair.Key ) + Constants.EpsilonSingle; break; } case Positionial.Highish: { priority =
        // Randem.NextSingle( this.Dictionary.Average( pair => pair.Key ), this.Dictionary.Max( pair => pair.Key ) ) + Constants.EpsilonSingle; break; } case Positionial.Middle: { priority = this.Dictionary.Average( pair
        // => pair.Key ) + Constants.EpsilonSingle; break; } case Positionial.Lowish: { priority = Randem.NextSingle( this.Dictionary.Min( pair => pair.Key ), this.Dictionary.Average( pair => pair.Key ) ) -
        // Constants.EpsilonSingle; break; } case Positionial.Lowest: { priority = this.Dictionary.Min( pair => pair.Key ) - Constants.EpsilonSingle; break; } default: throw new ArgumentOutOfRangeException( nameof(
        // positionial ) ); }
        /// <summary>
        ///     Renumber everything to between 0 and 1, evenly distributed and I need it to be *fast*.
        /// </summary>
        public void ReNormalize() {
            if ( this.Dictionary.IsEmpty ) { return; }

            var max = this.Dictionary.Max( selector: pair => pair.Key );
            var min = this.Dictionary.Min( selector: pair => pair.Key );
            var maxMinusMin = max - min;

            if ( Math.Abs( maxMinusMin ) < Single.Epsilon ) { return; }

            //I know other items could sneak in in here, but I don't care...
            foreach ( var key in this.Dictionary.Keys ) {
                var newPriority = ( key - min ) / maxMinusMin;

                if ( Math.Abs( key - newPriority ) < Single.Epsilon ) { continue; }

                this.Dictionary.TryRemove( key, out var value );
                this.Add( item: value, priority: newPriority );
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<Single, TValue>> IEnumerable<KeyValuePair<Single, TValue>>.GetEnumerator() => this.Dictionary.GetEnumerator();

        ///// <param name="item"></param>
        ///// <param name="positionial"></param>
        //public void Add( TValue item, Positionial positionial ) {
        //    if ( this.Dictionary.IsEmpty ) {
        //        this.Add( item, Randem.NextSingle(0,1) );
        //        this.ReNormalize();
        //        return;
        //    }
        //public PriorityTime this[ TValue key ] {
        //    get {
        //        PriorityTime result;
        //        if ( !this.Dictionary.TryGetValue( key, out result ) ) {
        //            result = new PriorityTime();
        //        }
        //        return result;
        //    }

        //    set {
        //        this.Dictionary.AddOrUpdate(key, addValue: value, updateValueFactory: ( s, n ) => n );
        //    }
        //}

        ///// <summary>
        /////     Calls the
        /////     <see
        /////         cref="ConcurrentDictionary{TKey,TValue}.AddOrUpdate(TKey,System.Func{TKey,TValue},System.Func{TKey,TValue,TValue})" />
        /////     method.
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="priorityTime"></param>
        //public void Add( TValue item, PriorityTime priorityTime ) {
        //    this.Dictionary.AddOrUpdate( item, priorityTime, ( o, f ) => priorityTime );
        //}
        //[CanBeNull]
        //public TValue Highest() => this.Dictionary.OrderByDescending( pair => pair.Value.Priority )
        //                                       .ThenBy( pair => pair.Value.Time )
        //                                       .Select( pair => pair.Key )
        //                                       .FirstOrDefault();

        //public ICollection<TValue> Keys() => this.Dictionary.Keys;

        //[CanBeNull]
        //public TValue Lowest() => this.Dictionary.OrderBy( pair => pair.Value.Priority )
        //                              .ThenBy( pair => pair.Value.Time )
        //                              .Select( pair => pair.Key )
        //                              .FirstOrDefault();

        //[CanBeNull]
        //public TValue PeekNext() => this.Highest();

        //[CanBeNull]
        //public Tuple<TValue, Single, DateTime> PullNext() {
        //    var peekNext = this.PeekNext();
        //    if ( null == peekNext ) {
        //        return null;
        //    }
        //    PriorityTime priorityTime;
        //    return this.Dictionary.TryRemove( peekNext, out priorityTime ) ? new Tuple<TValue, Single, DateTime>( peekNext, priorityTime.Priority, priorityTime.Time ) : null;
        //}
    }
}