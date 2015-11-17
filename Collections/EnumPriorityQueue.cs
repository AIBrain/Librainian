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
// "Librainian/EnumPriorityQueue.cs" was last cleaned by Rick on 2015/06/12 at 2:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Runtime.Serialization;
    using Threading;

    /// <summary>The highest priority is the highest</summary>
    [DataContract( IsReference = true )]
    public class EnumPriorityQueue  {

        [DataMember]
        protected readonly ConcurrentDictionary<Enum, PriorityTime> Queue = new ConcurrentDictionary<Enum, PriorityTime>();

        /// <summary>
        /// Calls the
        /// <see cref="ConcurrentDictionary{TKey,TValue}.AddOrUpdate(TKey,System.Func{TKey,TValue},System.Func{TKey,TValue,TValue})" /> method.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="priorityTime"></param>
        public void Add(Enum item, PriorityTime priorityTime) => this.Queue.AddOrUpdate( item, priorityTime, (o, f) => priorityTime );

        /// <summary>Inject the specified priority.</summary>
        /// <param name="item"></param>
        /// <param name="priority"></param>
        public void Add(Enum item, Single priority) => this.Add( item, new PriorityTime {
            Priority = priority,
            Time = DateTime.Now
        } );

        /// <summary>Use THIS one if you can!</summary>
        /// <param name="item"></param>
        /// <param name="positionial"></param>
        public void Add(Enum item, Positionial positionial) {
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

    }

}