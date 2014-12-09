#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/ConcurrentCollection.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Threading;

    public class ConcurrentCollection< T > : IProducerConsumerCollection< T > {
        private const int BACKOFF_MAX_YIELDS = 8;

        [NonSerialized] private Node m_head;

        private T[] m_serializationArray;

        public ConcurrentCollection() { }

        public ConcurrentCollection( IEnumerable< T > collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            this.InitializeFromCollection( collection );
        }

        public Boolean IsEmpty => this.m_head == null;

        public int Count {
            get {
                var num = 0;
                for ( var node = this.m_head; node != null; node = node.m_next ) {
                    ++num;
                }
                return num;
            }
        }

        Boolean ICollection.IsSynchronized => false;

        object ICollection.SyncRoot { get { throw new NotSupportedException( "ConcurrentCollection_SyncRoot_NotSupported" ); } }

        public void CopyTo( T[] array, int index ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            this.ToList().CopyTo( array, index );
        }

        public IEnumerator< T > GetEnumerator() => GetEnumerator( this.m_head );

        void ICollection.CopyTo( Array array, int index ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            if ( ( array as T[] ) == null ) {
                throw new ArgumentNullException( "array" );
            }
            this.ToList().CopyTo( ( T[] ) array, index );
        }

        // ReSharper restore RemoveToList.1
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        Boolean IProducerConsumerCollection< T >.TryAdd( T item ) {
            this.Push( item );
            return true;
        }

        Boolean IProducerConsumerCollection< T >.TryTake( out T item ) => this.TryPop( out item );

        public T[] ToArray() => this.ToList().ToArray();

        public void Clear() => this.m_head = null;

        public void Push( T item ) {
            var node = new Node( item ) {
                                            m_next = this.m_head
                                        };
            var mHead = this.m_head;
            if ( Interlocked.CompareExchange( ref mHead, node, node.m_next ) == node.m_next ) {
                return;
            }
            this.PushCore( node, node );
        }

        public void PushRange( T[] items ) {
            if ( items == null ) {
                throw new ArgumentNullException( "items" );
            }
            this.PushRange( items, 0, items.Length );
        }

        public void PushRange( T[] items, int startIndex, int count ) {
            ValidatePushPopRangeInput( items, startIndex, count );
            if ( count == 0 ) {
                return;
            }
            Node tail;
            var head = tail = new Node( items[ startIndex ] );
            for ( var index = startIndex + 1; index < startIndex + count; ++index ) {
                head = new Node( items[ index ] ) {
                                                      m_next = head
                                                  };
            }
            tail.m_next = this.m_head;
            var mHead = this.m_head;
            if ( Interlocked.CompareExchange( ref mHead, head, tail.m_next ) == tail.m_next ) {
                return;
            }
            this.PushCore( head, tail );
        }

        // ReSharper disable RemoveToList.1

        public Boolean TryPeek( out T result ) {
            var node = this.m_head;
            if ( node == null ) {
                result = default( T );
                return false;
            }
            result = node.m_value;
            return true;
        }

        public Boolean TryPop( out T result ) {
            var comparand = this.m_head;
            if ( comparand != null ) {
                var mHead = this.m_head;
                if ( Interlocked.CompareExchange( ref mHead, comparand.m_next, comparand ) != comparand ) {
                    return this.TryPopCore( out result );
                }
                result = comparand.m_value;
                return true;
            }

            result = default( T );
            return false;
        }

        public int TryPopRange( T[] items ) {
            if ( items == null ) {
                throw new ArgumentNullException( "items" );
            }
            return this.TryPopRange( items, 0, items.Length );
        }

        public int TryPopRange( T[] items, int startIndex, int count ) {
            ValidatePushPopRangeInput( items, startIndex, count );
            if ( count == 0 ) {
                return 0;
            }
            Node poppedHead;
            var nodesCount = this.TryPopCore( count, out poppedHead );
            if ( nodesCount <= 0 ) {
                return nodesCount;
            }
            CopyRemovedItems( poppedHead, items, startIndex, nodesCount );
            return nodesCount;
        }

        private static void CopyRemovedItems( Node head, IList< T > collection, int startIndex, int nodesCount ) {
            var node = head;
            for ( var index = startIndex; index < startIndex + nodesCount; ++index ) {
                collection[ index ] = node.m_value;
                node = node.m_next;
            }
        }

        private static IEnumerator< T > GetEnumerator( Node head ) {
            for ( var current = head; current != null; current = current.m_next ) {
                yield return current.m_value;
            }
        }

        private static void ValidatePushPopRangeInput( ICollection< T > items, int startIndex, int count ) {
            if ( items == null ) {
                throw new ArgumentNullException( "items" );
            }
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( "count", "ConcurrentStack_PushPopRange_CountOutOfRange" );
            }
            var length = items.Count;
            if ( startIndex >= length || startIndex < 0 ) {
                throw new ArgumentOutOfRangeException( "startIndex", "ConcurrentStack_PushPopRange_StartOutOfRange" );
            }
            if ( length - count < startIndex ) {
                throw new ArgumentException( "ConcurrentStack_PushPopRange_InvalidCount" );
            }
        }

        private void InitializeFromCollection( IEnumerable< T > collection ) {
            var node = collection.Aggregate< T, Node >( null, ( current, obj ) => new Node( obj ) {
                                                                                                      m_next = current
                                                                                                  } );
            this.m_head = node;
        }

        [OnDeserialized]
        private void OnDeserialized( StreamingContext context ) {
            Node node1 = null;
            Node node2 = null;
            foreach ( var node3 in this.m_serializationArray.Select( t => new Node( t ) ) ) {
                if ( node1 == null ) {
                    node2 = node3;
                }
                else {
                    node1.m_next = node3;
                }
                node1 = node3;
            }
            this.m_head = node2;
            this.m_serializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing( StreamingContext context ) => this.m_serializationArray = this.ToArray();

        private void PushCore( Node head, Node tail ) {
            var spinWait = new SpinWait();
            var mHead = this.m_head;
            do {
                spinWait.SpinOnce();
                tail.m_next = this.m_head;
            } while ( Interlocked.CompareExchange( ref mHead, head, tail.m_next ) != tail.m_next );
        }

        private List< T > ToList() {
            var list = new List< T >();
            for ( var node = this.m_head; node != null; node = node.m_next ) {
                list.Add( node.m_value );
            }
            return list;
        }

        private Boolean TryPopCore( out T result ) {
            Node poppedHead;
            if ( this.TryPopCore( 1, out poppedHead ) == 1 ) {
                result = poppedHead.m_value;
                return true;
            }
            result = default( T );
            return false;
        }

        private int TryPopCore( int count, out Node poppedHead ) {
            var spinWait = new SpinWait();
            var num1 = 1;

            //var random = new Random( Environment.TickCount & int.MaxValue );
            Node comparand;
            int num2;
            while ( true ) {
                comparand = this.m_head;
                if ( comparand == null ) {
                    break;
                }
                var node = comparand;
                for ( num2 = 1; num2 < count && node.m_next != null; ++num2 ) {
                    node = node.m_next;
                }
                var mHead = this.m_head;
                if ( Interlocked.CompareExchange( ref mHead, node.m_next, comparand ) == comparand ) {
                    goto label_9;
                }
                for ( var index = 0; index < num1; ++index ) {
                    spinWait.SpinOnce();
                }
                num1 = spinWait.NextSpinWillYield ? Randem.Next( 1, 8 ) : num1*2;
            }
            poppedHead = null;
            return 0;
            label_9:

            poppedHead = comparand;
            return num2;
        }

        private sealed class Node {
            internal readonly T m_value;

            internal Node m_next;

            internal Node( T value ) {
                this.m_value = value;
                this.m_next = null;
            }
        }
    }
}
