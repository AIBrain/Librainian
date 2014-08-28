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
// "Librainian/Q.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion

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
    [DebuggerDisplay( "Count = {Count}" )]
    [Serializable]
    [HostProtection( SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true )]
    public class Q< T > : IProducerConsumerCollection< T > {
        [NonSerialized] private Segment _head;

        [NonSerialized] private Segment _tail;

        private T[] _serializationArray;

        public Q() {
            this._head = this._tail = new Segment( 0L );
        }

        public Q( IEnumerable< T > collection ) {
            if ( collection == null ) {
                throw new ArgumentNullException( "collection" );
            }
            this.InitializeFromCollection( collection );
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

        public int Count {
            get {
                Segment head;
                Segment tail;
                int headLow;
                int tailHigh;
                this.GetHeadTailPositions( out head, out tail, out headLow, out tailHigh );
                if ( head == tail ) {
                    return tailHigh - headLow + 1;
                }
                return 32 - headLow + 32*( int ) ( tail._index - head._index - 1L ) + ( tailHigh + 1 );
            }
        }

        Boolean ICollection.IsSynchronized { get { return false; } }

        object ICollection.SyncRoot { get { throw new NotSupportedException(); } }

        public void CopyTo( T[] array, int index ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            this.ToList().CopyTo( array, index );
        }

        // ReSharper restore RemoveToList.1
        public IEnumerator< T > GetEnumerator() {
            return this.ToList().GetEnumerator();
        }

        void ICollection.CopyTo( Array array, int index ) {
            if ( array == null ) {
                throw new ArgumentNullException( "array" );
            }
            this.ToArray().CopyTo( array, index );
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        Boolean IProducerConsumerCollection< T >.TryAdd( T item ) {
            this.Enqueue( item );
            return true;
        }

        // ReSharper disable RemoveToList.1
        public T[] ToArray() {
            return this.ToList().ToArray();
        }

        public Boolean TryTake( out T item ) {
            return this.TryDequeue( out item );
        }

        public void Enqueue( T item ) {
            while ( !this._tail.TryAppend( item, ref this._tail ) ) {
                Thread.Yield();
            }
        }

        public Boolean TryDequeue( out T result ) {
            while ( !this.IsEmpty ) {
                if ( this._head.TryRemove( out result, ref this._head ) ) {
                    return true;
                }
            }
            result = default( T );
            return false;
        }

        public Boolean TryPeek( out T result ) {
            while ( !this.IsEmpty ) {
                if ( this._head.TryPeek( out result ) ) {
                    return true;
                }
            }
            result = default( T );
            return false;
        }

        private void GetHeadTailPositions( out Segment head, out Segment tail, out int headLow, out int tailHigh ) {
            head = this._head;
            tail = this._tail;
            headLow = head.Low;
            tailHigh = tail.High;
            while ( head != this._head || tail != this._tail || ( headLow != head.Low || tailHigh != tail.High ) || head._index > tail._index ) {
                Thread.Yield();
                head = this._head;
                tail = this._tail;
                headLow = head.Low;
                tailHigh = tail.High;
            }
        }

        private void InitializeFromCollection( IEnumerable< T > collection ) {
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
        private void OnDeserialized( StreamingContext context ) {
            this.InitializeFromCollection( this._serializationArray );
            this._serializationArray = null;
        }

        [OnSerializing]
        private void OnSerializing( StreamingContext context ) {
            this._serializationArray = this.ToArray();
        }

        private List< T > ToList() {
            Segment head;
            Segment tail;
            int headLow;
            int tailHigh;
            this.GetHeadTailPositions( head: out head, tail: out tail, headLow: out headLow, tailHigh: out tailHigh );
            if ( head == tail ) {
                return head.ToList( headLow, tailHigh );
            }
            var list = new List< T >( head.ToList( headLow, 31 ) );
            for ( var next = head.Next; next != tail; next = next.Next ) {
                list.AddRange( next.ToList( 0, 31 ) );
            }
            list.AddRange( tail.ToList( 0, tailHigh ) );
            return list;
        }

        private sealed class Segment {
            private readonly T[] _array;
            internal readonly long _index;
            private readonly int[] _state;
            public Segment Next;
            private int _high;

            private int _low;

            internal Segment( long index ) {
                this._array = new T[32];
                this._state = new int[32];
                this._high = -1;
                this._index = index;
            }

            public int High { get { return Math.Min( this._high, 31 ); } }

            public Boolean IsEmpty { get { return this.Low > this.High; } }

            public int Low { get { return Math.Min( this._low, 32 ); } }

            public List< T > ToList( int start, int end ) {
                var list = new List< T >();

                for ( var index = start; index <= end; ++index ) {
                    while ( this._state[ index ] == 0 ) {
                        Thread.Yield();
                    }
                    list.Add( this._array[ index ] );
                }
                return list;
            }

            public Boolean TryAppend( T value, ref Segment tail ) {
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

            public Boolean TryPeek( out T result ) {
                result = default( T );
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

            public Boolean TryRemove( out T result, ref Segment head ) {
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
                result = default( T );
                return false;
            }

            public void UnsafeAdd( T value ) {
                ++this._high;
                this._array[ this._high ] = value;
                this._state[ this._high ] = 1;
            }

            public Segment UnsafeGrow() {
                var segment = new Segment( this._index + 1L );
                this.Next = segment;
                return segment;
            }

            private void Grow( out Segment tail ) {
                this.Next = new Segment( this._index + 1L );
                tail = this.Next;
            }
        }
    }
}
