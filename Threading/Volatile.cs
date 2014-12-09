namespace Librainian.Threading {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    ///<summary>
    /// A small toolkit of classes that support lock-free thread-safe programming on single variables and arrays
    ///</summary>
    /// <seealso cref="http://github.com/disruptor-net/Disruptor-net/blob/master/Atomic/Volatile.cs" date="Dec 30, 2011" />
    public static class Volatile {
        ///<summary>
        /// Size of a cache line in bytes
        ///</summary>
        private const int CacheLineSize = 64;

        /// <summary>
        /// An integer value that may be updated atomically
        /// </summary>
        public struct Integer {
            private int _value;

            /// <summary>
            /// Create a new <see cref="Integer"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public Integer( int value ) {
                this._value = value;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public int ReadUnfenced() => this._value;

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public int ReadAcquireFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public int ReadFullFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public int ReadCompilerOnlyFence() => this._value;

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( int newValue ) {
                this._value = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( int newValue ) {
                this._value = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( int newValue ) => this._value = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( int newValue ) => this._value = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( int newValue, int comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public int AtomicExchange( int newValue ) => Interlocked.Exchange( ref this._value, newValue );

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <returns>The sum of the current value and the given value</returns>
            public int AtomicAddAndGet( int delta ) => Interlocked.Add( ref this._value, delta );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The incremented value.</returns>
            public int AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The decremented value.</returns>
            public int AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }
        }

        /// <summary>
        /// A long value that may be updated atomically
        /// </summary>
        public struct Long {
            private long _value;

            /// <summary>
            /// Create a new <see cref="Long"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public Long( long value ) {
                this._value = value;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadUnfenced() => this._value;

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadAcquireFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadFullFence() {
                Thread.MemoryBarrier();
                return this._value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public long ReadCompilerOnlyFence() => this._value;

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( long newValue ) {
                Thread.MemoryBarrier();
                this._value = newValue;
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( long newValue ) {
                Thread.MemoryBarrier();
                this._value = newValue;
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( long newValue ) => this._value = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( long newValue ) => this._value = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( long newValue, long comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public long AtomicExchange( long newValue ) => Interlocked.Exchange( ref this._value, newValue );

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <returns>The sum of the current value and the given value</returns>
            public long AtomicAddAndGet( long delta ) => Interlocked.Add( ref this._value, delta );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The incremented value.</returns>
            public long AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The decremented value.</returns>
            public long AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }
        }

        /// <summary>
        /// A boolean value that may be updated atomically
        /// </summary>
        public struct Boolean {
            // bool stored as an int, CAS not available on bool
            private int _value;
            private const int False = 0;
            private const int True = 1;

            /// <summary>
            /// Create a new <see cref="Boolean"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public Boolean( bool value ) {
                this._value = value ? True : False;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public bool ReadUnfenced() => ToBool( this._value );

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public bool ReadAcquireFence() {
                var value = ToBool( this._value );
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public bool ReadFullFence() {
                var value = ToBool( this._value );
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public bool ReadCompilerOnlyFence() => ToBool( this._value );

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( bool newValue ) {
                var newValueInt = ToInt( newValue );
                Thread.MemoryBarrier();
                this._value = newValueInt;
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( bool newValue ) {
                var newValueInt = ToInt( newValue );
                Thread.MemoryBarrier();
                this._value = newValueInt;
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( bool newValue ) => this._value = ToInt( newValue );

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( bool newValue ) => this._value = ToInt( newValue );

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( bool newValue, bool comparand ) {
                var newValueInt = ToInt( newValue );
                var comparandInt = ToInt( comparand );

                return Interlocked.CompareExchange( ref this._value, newValueInt, comparandInt ) == comparandInt;
            }

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public bool AtomicExchange( bool newValue ) {
                var newValueInt = ToInt( newValue );
                var originalValue = Interlocked.Exchange( ref this._value, newValueInt );
                return ToBool( originalValue );
            }

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }

            private static bool ToBool( int value ) {
                if ( value != False && value != True ) {
                    throw new ArgumentOutOfRangeException( "value" );
                }

                return value == True;
            }

            private static int ToInt( bool value ) => value ? True : False;
        }

        /// <summary>
        /// A reference that may be updated atomically
        /// </summary>
        public struct Reference<T> where T : class {
            private T _value;

            /// <summary>
            /// Create a new <see cref="Reference{T}"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public Reference( T value ) {
                this._value = value;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public T ReadUnfenced() => this._value;

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public T ReadAcquireFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public T ReadFullFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public T ReadCompilerOnlyFence() => this._value;

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( T newValue ) {
                Thread.MemoryBarrier();
                this._value = newValue;
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( T newValue ) {
                Thread.MemoryBarrier();
                this._value = newValue;
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>

            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( T newValue ) => this._value = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( T newValue ) => this._value = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( T newValue, T comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public T AtomicExchange( T newValue ) => Interlocked.Exchange( ref this._value, newValue );

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }
        }

        /// <summary>
        /// An <see cref="int"/> array that may be updated atomically
        /// </summary>
        public class IntegerArray {
            private readonly int[] _array;

            /// <summary>
            /// Create a new <see cref="IntegerArray"/> of a given length
            /// </summary>
            /// <param name="length">Length of the array</param>
            public IntegerArray( int length ) {
                if ( length <= 0 )
                    throw new ArgumentOutOfRangeException( "length" );

                this._array = new int[ length ];
            }

            /// <summary>
            ///  Create a new AtomicIntegerArray with the same length as, and all elements copied from, the given array.
            /// </summary>
            /// <param name="array"></param>
            public IntegerArray( int[] array ) {
                if ( array == null )
                    throw new ArgumentNullException( "array" );

                this._array = new int[ array.Length ];
                array.CopyTo( this._array, 0 );
            }

            /// <summary>
            /// Length of the array
            /// </summary>
            public int Length => this._array.Length;

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <param name="index">The index of the element.</param>
            /// <returns>The current value.</returns>
            public int ReadUnfenced( int index ) => this._array[ index ];

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public int ReadAcquireFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public int ReadFullFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public int ReadCompilerOnlyFence( int index ) => this._array[ index ];

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( int index, int newValue ) {
                this._array[ index ] = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( int index, int newValue ) {
                this._array[ index ] = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( int index, int newValue ) => this._array[ index ] = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( int index, int newValue ) => this._array[ index ] = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public bool AtomicCompareExchange( int index, int newValue, int comparand ) => Interlocked.CompareExchange( ref this._array[ index ], newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public int AtomicExchange( int index, int newValue ) => Interlocked.Exchange( ref this._array[ index ], newValue );

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <param name="index">The index.</param>
            /// <returns>The sum of the current value and the given value</returns>
            public int AtomicAddAndGet( int index, int delta ) => Interlocked.Add( ref this._array[ index ], delta );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>The incremented value.</returns>
            public int AtomicIncrementAndGet( int index ) => Interlocked.Increment( ref this._array[ index ] );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>The decremented value.</returns>
            public int AtomicDecrementAndGet( int index ) => Interlocked.Decrement( ref this._array[ index ] );
        }

        /// <summary>
        /// A <see cref="long"/> array that may be updated atomically
        /// </summary>
        public class LongArray {
            private readonly long[] _array;

            /// <summary>
            /// Create a new <see cref="LongArray"/> of a given length
            /// </summary>
            /// <param name="length">Length of the array</param>
            public LongArray( int length ) {
                if ( length <= 0 )
                    throw new ArgumentOutOfRangeException( "length" );

                this._array = new long[ length ];
            }

            /// <summary>
            ///  Create a new <see cref="LongArray"/>with the same length as, and all elements copied from, the given array.
            /// </summary>
            /// <param name="array"></param>
            public LongArray( long[] array ) {
                if ( array == null )
                    throw new ArgumentNullException( "array" );

                this._array = new long[ array.Length ];
                array.CopyTo( this._array, 0 );
            }

            /// <summary>
            /// Length of the array
            /// </summary>
            public int Length => this._array.Length;

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <param name="index">The index of the element.</param>
            /// <returns>The current value.</returns>
            public long ReadUnfenced( int index ) => this._array[ index ];

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public long ReadAcquireFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public long ReadFullFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public long ReadCompilerOnlyFence( int index ) => this._array[ index ];

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( int index, long newValue ) {
                this._array[ index ] = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( int index, long newValue ) {
                this._array[ index ] = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( int index, long newValue ) => this._array[ index ] = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( int index, long newValue ) => this._array[ index ] = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public bool AtomicCompareExchange( int index, long newValue, long comparand ) => Interlocked.CompareExchange( ref this._array[ index ], newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public long AtomicExchange( int index, long newValue ) => Interlocked.Exchange( ref this._array[ index ], newValue );

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <param name="index">The index.</param>
            /// <returns>The sum of the current value and the given value</returns>
            public long AtomicAddAndGet( int index, long delta ) => Interlocked.Add( ref this._array[ index ], delta );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>The incremented value.</returns>
            public long AtomicIncrementAndGet( int index ) => Interlocked.Increment( ref this._array[ index ] );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>The decremented value.</returns>
            public long AtomicDecrementAndGet( int index ) => Interlocked.Decrement( ref this._array[ index ] );
        }

        /// <summary>
        /// A <see cref="bool"/> array that may be updated atomically
        /// </summary>
        public class BooleanArray {
            private readonly int[] _array;
            private const int False = 0;
            private const int True = 1;

            /// <summary>
            /// Create a new <see cref="BooleanArray"/> of a given length
            /// </summary>
            /// <param name="length">Length of the array</param>
            public BooleanArray( int length ) {
                if ( length <= 0 )
                    throw new ArgumentOutOfRangeException( "length" );

                this._array = new int[ length ];
            }

            /// <summary>
            ///  Create a new <see cref="BooleanArray"/>with the same length as, and all elements copied from, the given array.
            /// </summary>
            /// <param name="array"></param>
            public BooleanArray( IEnumerable< bool > array ) {
                if ( array == null )
                    throw new ArgumentNullException( "array" );

                this._array = array.Select( ToInt ).ToArray();
            }

            /// <summary>
            /// Length of the array
            /// </summary>
            public int Length => this._array.Length;

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <param name="index">The index of the element.</param>
            /// <returns>The current value.</returns>
            public bool ReadUnfenced( int index ) => ToBool( this._array[ index ] );

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public bool ReadAcquireFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return ToBool( value );
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public bool ReadFullFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return ToBool( value );
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public bool ReadCompilerOnlyFence( int index ) => ToBool( this._array[ index ] );

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( int index, bool newValue ) {
                this._array[ index ] = ToInt( newValue );
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( int index, bool newValue ) {
                this._array[ index ] = ToInt( newValue );
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( int index, bool newValue ) => this._array[ index ] = ToInt( newValue );

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( int index, bool newValue ) => this._array[ index ] = ToInt( newValue );

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public bool AtomicCompareExchange( int index, bool newValue, bool comparand ) {
                var newValueInt = ToInt( newValue );
                var comparandInt = ToInt( comparand );
                return Interlocked.CompareExchange( ref this._array[ index ], newValueInt, comparandInt ) == comparandInt;
            }

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public bool AtomicExchange( int index, bool newValue ) {
                var result = Interlocked.Exchange( ref this._array[ index ], ToInt( newValue ) );
                return ToBool( result );
            }

            private static bool ToBool( int value ) {
                if ( value != False && value != True ) {
                    throw new ArgumentOutOfRangeException( "value" );
                }

                return value == True;
            }

            private static int ToInt( bool value ) => value ? True : False;
        }

        /// <summary>
        /// A reference array that may be updated atomically
        /// </summary>
        public class ReferenceArray<T> where T : class {
            private readonly T[] _array;

            /// <summary>
            /// Create a new <see cref="ReferenceArray{T}"/> of a given length
            /// </summary>
            /// <param name="length">Length of the array</param>
            public ReferenceArray( int length ) {
                if ( length <= 0 )
                    throw new ArgumentOutOfRangeException( "length" );

                this._array = new T[ length ];
            }

            /// <summary>
            ///  Create a new <see cref="ReferenceArray{T}"/>with the same length as, and all elements copied from, the given array.
            /// </summary>
            /// <param name="array"></param>
            public ReferenceArray( IEnumerable< T > array ) {
                if ( array == null )
                    throw new ArgumentNullException( "array" );

                this._array = array.ToArray();
            }

            /// <summary>
            /// Length of the array
            /// </summary>
            public int Length => this._array.Length;

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <param name="index">The index of the element.</param>
            /// <returns>The current value.</returns>
            public T ReadUnfenced( int index ) => this._array[ index ];

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public T ReadAcquireFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            public T ReadFullFence( int index ) {
                var value = this._array[ index ];
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public T ReadCompilerOnlyFence( int index ) => this._array[ index ];

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( int index, T newValue ) {
                this._array[ index ] = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( int index, T newValue ) {
                this._array[ index ] = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="index">The element index</param>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( int index, T newValue ) => this._array[ index ] = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( int index, T newValue ) => this._array[ index ] = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public bool AtomicCompareExchange( int index, T newValue, T comparand ) => Interlocked.CompareExchange( ref this._array[ index ], newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="index">The index.</param>
            /// <returns>The original value</returns>
            public T AtomicExchange( int index, T newValue ) {
                var result = Interlocked.Exchange( ref this._array[ index ], newValue );
                return result;
            }
        }

        /// <summary>
        /// An integer value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false sharing)
        /// </summary>
        [StructLayout( LayoutKind.Explicit, Size = CacheLineSize * 2 )]
        public struct PaddedInteger {
            [FieldOffset( CacheLineSize )]
            private int _value;

            /// <summary>
            /// Create a new <see cref="PaddedInteger"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public PaddedInteger( int value ) {
                this._value = value;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public int ReadUnfenced() => this._value;

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public int ReadAcquireFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public int ReadFullFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public int ReadCompilerOnlyFence() => this._value;

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( int newValue ) {
                this._value = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( int newValue ) {
                this._value = newValue;
                Thread.MemoryBarrier();
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( int newValue ) => this._value = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( int newValue ) => this._value = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( int newValue, int comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public int AtomicExchange( int newValue ) => Interlocked.Exchange( ref this._value, newValue );

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <returns>The sum of the current value and the given value</returns>
            public int AtomicAddAndGet( int delta ) => Interlocked.Add( ref this._value, delta );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The incremented value.</returns>
            public int AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The decremented value.</returns>
            public int AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }
        }

        /// <summary>
        /// A long value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false sharing)
        /// </summary>
        [StructLayout( LayoutKind.Explicit, Size = CacheLineSize * 2 )]
        public struct PaddedLong {
            [FieldOffset( CacheLineSize )]
            private long _value;

            /// <summary>
            /// Create a new <see cref="PaddedLong"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public PaddedLong( long value ) {
                this._value = value;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadUnfenced() => this._value;

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadAcquireFence() {
                var value = this._value;
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public long ReadFullFence() {
                Thread.MemoryBarrier();
                return this._value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public long ReadCompilerOnlyFence() => this._value;

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( long newValue ) {
                Thread.MemoryBarrier();
                this._value = newValue;
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( long newValue ) {
                Thread.MemoryBarrier();
                this._value = newValue;
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( long newValue ) => this._value = newValue;

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( long newValue ) => this._value = newValue;

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( long newValue, long comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public long AtomicExchange( long newValue ) => Interlocked.Exchange( ref this._value, newValue );

            /// <summary>
            /// Atomically add the given value to the current value and return the sum
            /// </summary>
            /// <param name="delta">The value to be added</param>
            /// <returns>The sum of the current value and the given value</returns>
            public long AtomicAddAndGet( long delta ) => Interlocked.Add( ref this._value, delta );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The incremented value.</returns>
            public long AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

            /// <summary>
            /// Atomically increment the current value and return the new value
            /// </summary>
            /// <returns>The decremented value.</returns>
            public long AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }
        }

        /// <summary>
        /// A boolean value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false sharing)
        /// </summary>
        [StructLayout( LayoutKind.Explicit, Size = CacheLineSize * 2 )]
        public struct PaddedBoolean {
            // bool stored as an int, CAS not available on bool
            [FieldOffset( CacheLineSize )]
            private int _value;

            private const int False = 0;
            private const int True = 1;

            /// <summary>
            /// Create a new <see cref="PaddedBoolean"/> with the given initial value.
            /// </summary>
            /// <param name="value">Initial value</param>
            public PaddedBoolean( bool value ) {
                this._value = value ? True : False;
            }

            /// <summary>
            /// Read the value without applying any fence
            /// </summary>
            /// <returns>The current value</returns>
            public bool ReadUnfenced() => ToBool( this._value );

            /// <summary>
            /// Read the value applying acquire fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public bool ReadAcquireFence() {
                var value = ToBool( this._value );
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying full fence semantic
            /// </summary>
            /// <returns>The current value</returns>
            public bool ReadFullFence() {
                var value = ToBool( this._value );
                Thread.MemoryBarrier();
                return value;
            }

            /// <summary>
            /// Read the value applying a compiler only fence, no CPU fence is applied
            /// </summary>
            /// <returns>The current value</returns>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public bool ReadCompilerOnlyFence() => ToBool( this._value );

            /// <summary>
            /// Write the value applying release fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteReleaseFence( bool newValue ) {
                var newValueInt = ToInt( newValue );
                Thread.MemoryBarrier();
                this._value = newValueInt;
            }

            /// <summary>
            /// Write the value applying full fence semantic
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteFullFence( bool newValue ) {
                var newValueInt = ToInt( newValue );
                Thread.MemoryBarrier();
                this._value = newValueInt;
            }

            /// <summary>
            /// Write the value applying a compiler fence only, no CPU fence is applied
            /// </summary>
            /// <param name="newValue">The new value</param>
            [MethodImpl( MethodImplOptions.NoOptimization )]
            public void WriteCompilerOnlyFence( bool newValue ) => this._value = ToInt( newValue );

            /// <summary>
            /// Write without applying any fence
            /// </summary>
            /// <param name="newValue">The new value</param>
            public void WriteUnfenced( bool newValue ) => this._value = ToInt( newValue );

            /// <summary>
            /// Atomically set the value to the given updated value if the current value equals the comparand
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <param name="comparand">The comparand (expected value)</param>
            /// <returns></returns>
            public bool AtomicCompareExchange( bool newValue, bool comparand ) {
                var newValueInt = ToInt( newValue );
                var comparandInt = ToInt( comparand );

                return Interlocked.CompareExchange( ref this._value, newValueInt, comparandInt ) == comparandInt;
            }

            /// <summary>
            /// Atomically set the value to the given updated value
            /// </summary>
            /// <param name="newValue">The new value</param>
            /// <returns>The original value</returns>
            public bool AtomicExchange( bool newValue ) {
                var newValueInt = ToInt( newValue );
                var originalValue = Interlocked.Exchange( ref this._value, newValueInt );
                return ToBool( originalValue );
            }

            /// <summary>
            /// Returns the String representation of the current value.
            /// </summary>
            /// <returns>the String representation of the current value.</returns>
            public override String ToString() {
                var value = this.ReadFullFence();
                return value.ToString();
            }

            private static bool ToBool( int value ) {
                if ( value != False && value != True ) {
                    throw new ArgumentOutOfRangeException( "value" );
                }

                return value == True;
            }

            private static int ToInt( bool value ) => value ? True : False;
        }
    }
}