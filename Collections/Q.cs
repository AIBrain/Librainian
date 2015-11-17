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
// "Librainian/Q.cs" was last cleaned by Rick on 2015/06/12 at 2:51 PM

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible( false )]
    [DebuggerDisplay( "Count={Count}" )]
    [Serializable]
    [HostProtection( SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true )]
    public class Q<T> : IProducerConsumerCollection<T> {

        [NonSerialized]
        private Segment _head;

        private T[] _serializationArray;

        [NonSerialized]
        private Segment _tail;

        public Int32 Count {
            get {
                Segment head;
                Segment tail;
                Int32 headLow;
                Int32 tailHigh;
                this.GetHeadTailPositions( out head, out tail, out headLow, out tailHigh );
                if ( head == tail ) {
                    return tailHigh - headLow + 1;
                }
                return 32 - headLow + 32 * ( Int32 )( tail.Index - head.Index - 1L ) + tailHigh + 1;
            }
        }

        public Boolean IsEmpty {
            get {
                var segment = this._head;
                if ( !segment.IsEmpty ) {
                    return false;
                }
                if ( segment.Next == null ) {
                    return true;
                }

                for ( ; segment.IsEmpty; segment = this._head ) {
                    if ( segment.Next == null ) {
                        return true;
                    }
                    Thread.Yield();
                }
                return false;
            }
        }

        Boolean ICollection.IsSynchronized => false;

        Object ICollection.SyncRoot {
            get {
                throw new NotSupportedException();
            }
        }

        public Q() {
            this._head = this._tail = new Segment( 0L );
        }

        public Q(IEnumerable<T> collection) {
            if ( collection == null ) {
                throw new ArgumentNullException( nameof( collection ) );
            }
            this.InitializeFromCollection( collection );
        }

        public void CopyTo(T[] array, Int32 index) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            this.ToList().CopyTo( array, index );
        }

        public void Enqueue(T item) {
            while ( !this._tail.TryAppend( item, ref this._tail ) ) {
                Thread.Yield();
            }
        }

        public IEnumerator<T> GetEnumerator() => this.ToList().GetEnumerator();

        public T[] ToArray() => this.ToList().ToArray();

        public Boolean TryDequeue(out T result) {
            while ( !this.IsEmpty ) {
                if ( this._head.TryRemove( out result, ref this._head ) ) {
                    return true;
                }
            }
            result = default(T);
            return false;
        }

        public Boolean TryPeek(out T result) {
            while ( !this.IsEmpty ) {
                if ( this._head.TryPeek( out result ) ) {
                    return true;
                }
            }
            result = default(T);
            return false;
        }

        public Boolean TryTake(out T item) => this.TryDequeue( out item );

        void ICollection.CopyTo(Array array, Int32 index) {
            if ( array == null ) {
                throw new ArgumentNullException( nameof( array ) );
            }
            this.ToArray().CopyTo( array, index );
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void GetHeadTailPositions(out Segment head, out Segment tail, out Int32 headLow, out Int32 tailHigh) {
            head = this._head;
            tail = this._tail;
            headLow = head.Low;
            tailHigh = tail.High;
            while ( ( head != this._head ) || ( tail != this._tail ) || ( headLow != head.Low ) || ( tailHigh != tail.High ) || ( head.Index > tail.Index ) ) {
                Thread.Yield();
                head = this._head;
                tail = this._tail;
                headLow = head.Low;
                tailHigh = tail.High;
            }
        }

        private void InitializeFromCollection(IEnumerable<T> collection) {
            this._head = this._tail = new Segment( 0L );
            var num = 0;
            foreach ( var obj in collection ) {
                this._tail.UnsafeAdd( obj );
                ++num;
                if ( num < 32 ) {
                    continue;
                }
                this._tail = this._tail.UnsafeGrow();
                num = 0;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            this.InitializeFromCollection( this._serializationArray );
            this._serializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context) => this._serializationArray = this.ToArray();

        private List<T> ToList() {
            Segment head;
            Segment tail;
            Int32 headLow;
            Int32 tailHigh;
            this.GetHeadTailPositions( head: out head, tail: out tail, headLow: out headLow, tailHigh: out tailHigh );
            if ( head == tail ) {
                return head.ToList( headLow, tailHigh );
            }
            var list = new List<T>( head.ToList( headLow, 31 ) );
            for ( var next = head.Next; next != tail; next = next.Next ) {
                list.AddRange( next.ToList( 0, 31 ) );
            }
            list.AddRange( tail.ToList( 0, tailHigh ) );
            return list;
        }

        Boolean IProducerConsumerCollection<T>.TryAdd(T item) {
            this.Enqueue( item );
            return true;
        }

        private sealed class Segment {
            public Segment Next;
            internal readonly Int64 Index;
            private readonly T[] _array;
            private readonly Int32[] _state;
            private Int32 _high;
            private Int32 _low;

            public Int32 High => Math.Min( this._high, 31 );

            public Boolean IsEmpty => this.Low > this.High;

            public Int32 Low => Math.Min( this._low, 32 );

            internal Segment(Int64 index) {
                this._array = new T[ 32 ];
                this._state = new Int32[ 32 ];
                this._high = -1;
                this.Index = index;
            }

            public List<T> ToList(Int32 start, Int32 end) {
                var list = new List<T>();

                for ( var index = start; index <= end; ++index ) {
                    while ( this._state[ index ] == 0 ) {
                        Thread.Yield();
                    }
                    list.Add( this._array[ index ] );
                }
                return list;
            }

            public Boolean TryAppend(T value, ref Segment tail) {
                if ( this._high >= 31 ) {
                    return false;
                }

#pragma warning disable 420
                var index = Interlocked.Increment( ref this._high );
#pragma warning restore 420
                if ( index <= 31 ) {
                    this._array[ index ] = value;
                    this._state[ index ] = 1;
                }
                if ( index == 31 ) {
                    this.Grow( out tail );
                }

                return index <= 31;
            }

            public Boolean TryPeek(out T result) {
                result = default(T);
                var low = this.Low;
                if ( low > this.High ) {
                    return false;
                }

                while ( this._state[ low ] == 0 ) {
                    Thread.Yield();
                }
                result = this._array[ low ];
                return true;
            }

            public Boolean TryRemove(out T result, ref Segment head) {
                var low = this.Low;
                for ( var high = this.High; low <= high; high = this.High ) {
#pragma warning disable 420
                    if ( Interlocked.CompareExchange( ref this._low, low + 1, low ) != low ) {
#pragma warning restore 420
                        Thread.Yield();
                        low = this.Low;
                    }
                    else {
                        while ( this._state[ low ] == 0 ) {
                            Thread.Yield();
                        }
                        result = this._array[ low ];
                        if ( low + 1 >= 32 ) {
                            while ( this.Next == null ) {
                                Thread.Yield();
                            }
                            head = this.Next;
                        }
                        return true;
                    }
                }
                result = default(T);
                return false;
            }

            public void UnsafeAdd(T value) {
                ++this._high;
                this._array[ this._high ] = value;
                this._state[ this._high ] = 1;
            }

            public Segment UnsafeGrow() {
                var segment = new Segment( this.Index + 1L );
                this.Next = segment;
                return segment;
            }

            private void Grow(out Segment tail) {
                this.Next = new Segment( this.Index + 1L );
                tail = this.Next;
            }
        }

        // ReSharper restore RemoveToList.1 ReSharper disable RemoveToList.1
    }
}