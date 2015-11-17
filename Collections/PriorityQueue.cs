// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/PriorityQueue.cs" was last cleaned by Rick on 2015/06/12 at 2:51 PM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Threading;

    /// <summary>The highest priority is the highest</summary>
    [DataContract( IsReference = true )]
    public class PriorityQueue<TKey> : IEnumerable<KeyValuePair<TKey,PriorityTime> > {

        [DataMember]
        private ConcurrentDictionary< TKey, PriorityTime > Queue { get; set; } = new ConcurrentDictionary< TKey, PriorityTime >();

        public TKey Highest => this.Queue.OrderByDescending( pair => pair.Value.Priority ).ThenBy( pair => pair.Value.Time ).Select( pair => pair.Key ).FirstOrDefault();

        public ICollection<TKey> Keys => this.Queue.Keys;

        public TKey Lowest => this.Queue.OrderBy( pair => pair.Value.Priority ).ThenBy( pair => pair.Value.Time ).Select( pair => pair.Key ).FirstOrDefault();

        public ICollection<PriorityTime> Values => this.Queue.Values;

        public PriorityTime this[ TKey key ] {
            get {
                PriorityTime result;
                if ( !this.Queue.TryGetValue( key, out result ) ) {
                    result = new PriorityTime();
                }
                return result;
            }

            set {
                this.Queue.AddOrUpdate( key: key, addValue: value, updateValueFactory: (s, n) => n );
            }
        }

        /// <summary>
        /// Calls the
        /// <see cref="ConcurrentDictionary{TKey,TValue}.AddOrUpdate(TKey,System.Func{TKey,TValue},System.Func{TKey,TValue,TValue})" /> method.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priorityTime"></param>
        public void Add(TKey item, PriorityTime priorityTime) {
            this.Queue.AddOrUpdate( item, priorityTime, (o, f) => priorityTime );
        }

        /// <summary>Inject the specified priority.</summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Add(TKey item, Single priority) {
            this.Add( item, new PriorityTime {
                Priority = priority,
                Time = DateTime.Now
            } );
        }

        /// <summary>Use THIS one if you can!</summary>
        /// <param name="item"></param>
        /// <param name="positionial"></param>
        public void Add(TKey item, Positionial positionial) {
            var newPriority = Randem.NextSingle();

            if ( this.Queue.IsEmpty ) {
                this.Add( item, newPriority );
                this.ReNormalize();
                return;
            }

            var highestPriority = this.Queue.Values.Max( priorityTime => priorityTime.Priority );
            var averagePriority = this.Queue.Values.Average( priorityTime => priorityTime.Priority );
            var lowestPriority = this.Queue.Values.Max( priorityTime => priorityTime.Priority );

            if ( averagePriority <= 1.0 ) {
                this.ReNormalize();
                highestPriority = this.Queue.Values.Max( priorityTime => priorityTime.Priority );
                averagePriority = this.Queue.Values.Average( priorityTime => priorityTime.Priority );
                lowestPriority = this.Queue.Values.Max( priorityTime => priorityTime.Priority );
            }

            switch ( positionial ) {
                case Positionial.Highest:
                    {
                        newPriority = 2.0f * highestPriority;
                        break;
                    }
                case Positionial.Higher:
                    {
                        newPriority = ( highestPriority + highestPriority + averagePriority ) / 4.0f;
                        break;
                    }
                case Positionial.Highish:
                    {
                        newPriority = ( ( Int32 )averagePriority ).Next( ( Int32 )highestPriority );
                        break;
                    }
                case Positionial.High:
                    {
                        newPriority = ( highestPriority + averagePriority ) / 2.0f;
                        break;
                    }
                case Positionial.Middle:
                    {
                        newPriority = averagePriority;
                        break;
                    }
                case Positionial.Low:
                    {
                        newPriority = ( lowestPriority + averagePriority ) / 2.0f;
                        break;
                    }
                case Positionial.Lower:
                    {
                        newPriority = ( lowestPriority + averagePriority ) / 4.0f;
                        break;
                    }
                case Positionial.Lowish:
                    {
                        newPriority = ( ( Int32 )lowestPriority ).Next( ( Int32 )averagePriority );
                        break;
                    }
                case Positionial.Lowest:
                    {
                        newPriority = lowestPriority;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException( nameof( positionial ) );
            }

            this.Add( item, newPriority );
            this.ReNormalize();
        }

        public TKey PeekNext() => this.Highest;

        public Tuple<TKey, Single, DateTime> PullNext() {
            var peekNext = this.PeekNext();
            if ( Equals( default(TKey), peekNext ) ) {
                return null;
            }
            PriorityTime priorityTime;
            return this.Queue.TryRemove( peekNext, out priorityTime ) ? new Tuple<TKey, Single, DateTime>( peekNext, priorityTime.Priority, priorityTime.Time ) : null;
        }

        /// <summary>
        /// Renumber everything. If this is called enough.. I need it to be fast and efficient.
        /// </summary>
        public void ReNormalize() {
            if ( this.Queue.IsEmpty ) {
                return;
            }

            var items = this.Queue.OrderBy( bob => bob.Value.Time ).ToList();
            var count = items.Count;

            //I know other items could sneak in in here, but I don't care atm...
            foreach ( var item in items ) {
                var newPriority = count * ( 2 + items.IndexOf( item ) ); // (-1, +1, +1) > 0
                this.Queue[ item.Key ].Priority = newPriority;
            }
        }

        public Boolean TryRemove(TKey item) {
            PriorityTime dummy;
            return this.Queue.TryRemove( item, out dummy );
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator< KeyValuePair< TKey, PriorityTime > > IEnumerable< KeyValuePair< TKey, PriorityTime > >.GetEnumerator() {
            return this.Queue.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator() {
            return this.Queue.GetEnumerator();
        }

    }
}