// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ConcurrentCollection.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using Maths;

    public class ConcurrentCollection<T> : IProducerConsumerCollection<T> {
        private const Int32 BackoffMaxYields = 8;

        [NonSerialized]
        private Node _mHead;

        private T[] _mSerializationArray;

        public ConcurrentCollection() {
        }

        public ConcurrentCollection( IEnumerable<T> collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }
            this.InitializeFromCollection( collection );
        }

        public Int32 Count {
            get {
                var num = 0;
                for ( var node = this._mHead; node != null; node = node.MNext ) {
                    ++num;
                }
                return num;
            }
        }

        public Boolean IsSynchronized => false;

        public Object SyncRoot {
            get {
                throw new NotSupportedException( "ConcurrentCollection_SyncRoot_NotSupported" );
            }
        }

        public Boolean IsEmpty() => this._mHead == null;

        public void Clear() => this._mHead = null;

        public void CopyTo( T[] array, Int32 index ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            this.ToList().CopyTo( array, index );
        }

        public IEnumerator<T> GetEnumerator() => GetEnumerator( this._mHead );

        void ICollection.CopyTo( Array array, Int32 index ) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            if ( !(array is T[]) ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            this.ToList().CopyTo( ( T[] )array, index );
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Boolean TryAdd( T item ) {
            this.Push( item );
            return true;
        }

        public Boolean TryTake( out T item ) => this.TryPop( out item );

        public void Push( T item ) {
            var node = new Node( item ) { MNext = this._mHead };
            var mHead = this._mHead;
            if ( Interlocked.CompareExchange( ref mHead, node, node.MNext ) == node.MNext ) {
                return;
            }
            this.PushCore( node, node );
        }

        public void PushRange( T[] items ) {
            if ( items == null ) {
                throw new ArgumentNullException( nameof( items ) );
            }
            this.PushRange( items, 0, items.Length );
        }

        public void PushRange( T[] items, Int32 startIndex, Int32 count ) {
            ValidatePushPopRangeInput( items, startIndex, count );
            if ( count == 0 ) {
                return;
            }
            Node tail;
            var head = tail = new Node( items[ startIndex ] );
            for ( var index = startIndex + 1; index < startIndex + count; ++index ) {
                head = new Node( items[ index ] ) { MNext = head };
            }
            tail.MNext = this._mHead;
            var mHead = this._mHead;
            if ( Interlocked.CompareExchange( ref mHead, head, tail.MNext ) == tail.MNext ) {
                return;
            }
            this.PushCore( head, tail );
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once RemoveToList.1
        public T[] ToArray() => this.ToList().ToArray();

        public Boolean TryPeek( out T result ) {
            var node = this._mHead;
            if ( node == null ) {
                result = default( T );
                return false;
            }
            result = node.MValue;
            return true;
        }

        public Boolean TryPop( out T result ) {
            var comparand = this._mHead;
            if ( comparand != null ) {
                var mHead = this._mHead;
                if ( Interlocked.CompareExchange( ref mHead, comparand.MNext, comparand ) != comparand ) {
                    return this.TryPopCore( out result );
                }
                result = comparand.MValue;
                return true;
            }

            result = default( T );
            return false;
        }

        public Int32 TryPopRange( T[] items ) {
            if ( items == null ) {
                throw new ArgumentNullException( nameof( items ) );
            }
            return this.TryPopRange( items, 0, items.Length );
        }

        public Int32 TryPopRange( T[] items, Int32 startIndex, Int32 count ) {
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

        private static void CopyRemovedItems( Node head, IList<T> collection, Int32 startIndex, Int32 nodesCount ) {
            var node = head;
            for ( var index = startIndex; index < startIndex + nodesCount; ++index ) {
                collection[ index ] = node.MValue;
                node = node.MNext;
            }
        }

        private static IEnumerator<T> GetEnumerator( Node head ) {
            for ( var current = head; current != null; current = current.MNext ) {
                yield return current.MValue;
            }
        }

        private static void ValidatePushPopRangeInput( ICollection<T> items, Int32 startIndex, Int32 count ) {
            if ( items == null ) {
                throw new ArgumentNullException( nameof( items ) );
            }
            if ( count < 0 ) {
                throw new ArgumentOutOfRangeException( nameof( count ), "ConcurrentStack_PushPopRange_CountOutOfRange" );
            }
            var length = items.Count;
            if ( ( startIndex >= length ) || ( startIndex < 0 ) ) {
                throw new ArgumentOutOfRangeException( nameof( startIndex ), "ConcurrentStack_PushPopRange_StartOutOfRange" );
            }
            if ( length - count < startIndex ) {
                throw new ArgumentException( "ConcurrentStack_PushPopRange_InvalidCount" );
            }
        }

        private void InitializeFromCollection( IEnumerable<T> collection ) {
            var node = collection.Aggregate<T, Node>( null, ( current, obj ) => new Node( obj ) { MNext = current } );
            this._mHead = node;
        }

        [OnDeserialized]
        private void OnDeserialized( StreamingContext context ) {
            Node node1 = null;
            Node node2 = null;
            foreach ( var node3 in this._mSerializationArray.Select( t => new Node( t ) ) ) {
                if ( node1 == null ) {
                    node2 = node3;
                }
                else {
                    node1.MNext = node3;
                }
                node1 = node3;
            }
            this._mHead = node2;
            this._mSerializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing( StreamingContext context ) => this._mSerializationArray = this.ToArray();

        private void PushCore( Node head, Node tail ) {
            var spinWait = new SpinWait();
            var mHead = this._mHead;
            do {
                spinWait.SpinOnce();
                tail.MNext = this._mHead;
            } while ( Interlocked.CompareExchange( ref mHead, head, tail.MNext ) != tail.MNext );
        }

        private List<T> ToList() {
            var list = new List<T>();
            for ( var node = this._mHead; node != null; node = node.MNext ) {
                list.Add( node.MValue );
            }
            return list;
        }

        private Boolean TryPopCore( out T result ) {
            Node poppedHead;
            if ( this.TryPopCore( 1, out poppedHead ) == 1 ) {
                result = poppedHead.MValue;
                return true;
            }
            result = default( T );
            return false;
        }

        private Int32 TryPopCore( Int32 count, out Node poppedHead ) {
            var spinWait = new SpinWait();
            var num1 = 1;

            //var random = new Random( Environment.TickCount & int.MaxValue );
            Node comparand;
            Int32 num2;
            while ( true ) {
                comparand = this._mHead;
                if ( comparand == null ) {
                    break;
                }
                var node = comparand;
                for ( num2 = 1; ( num2 < count ) && ( node.MNext != null ); ++num2 ) {
                    node = node.MNext;
                }
                var mHead = this._mHead;
                if ( Interlocked.CompareExchange( ref mHead, node.MNext, comparand ) == comparand ) {
                    goto label_9;
                }
                for ( var index = 0; index < num1; ++index ) {
                    spinWait.SpinOnce();
                }
                num1 = spinWait.NextSpinWillYield ? 1.Next( 8 ) : num1 * 2;
            }
            poppedHead = null;
            return 0;
            label_9:

            poppedHead = comparand;
            return num2;
        }

        private sealed class Node {
            internal readonly T MValue;
            internal Node MNext;

            internal Node( T value ) {
                this.MValue = value;
                this.MNext = null;
            }
        }

        // ReSharper restore RemoveToList.1
    }
}