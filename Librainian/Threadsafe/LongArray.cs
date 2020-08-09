﻿#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>A <see cref="Int64" /> array that may be updated atomically</summary>
	public class LongArray {

		[NotNull]
		private Int64[] Array { get; }

		/// <summary>Length of the array</summary>
		public Int32 Length => this.Array.Length;

		/// <summary>Create a new <see cref="LongArray" /> of a given length</summary>
		/// <param name="length">Length of the array</param>
		public LongArray( Int32 length ) {
			if ( length <= 0 ) {
				throw new ArgumentOutOfRangeException( nameof( length ) );
			}

			this.Array = new Int64[ length ];
		}

		/// <summary>Create a new <see cref="LongArray" /> with the same length as, and all elements copied from, the given array.</summary>
		/// <param name="array"></param>
		public LongArray( [NotNull] Int64[] array ) {
			this.Array = new Int64[ array.Length ];
			array.CopyTo( this.Array, 0 );
		}

		/// <summary>Atomically add the given value to the current value and return the sum</summary>
		/// <param name="delta">The value to be added</param>
		/// <param name="index">The index.</param>
		/// <returns>The sum of the current value and the given value</returns>
		public Int64 AtomicAddAndGet( Int32 index, Int64 delta ) => Interlocked.Add( ref this.Array[ index ], delta );

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <param name="index">    The index.</param>
		/// <returns>The original value</returns>
		public Boolean AtomicCompareExchange( Int32 index, Int64 newValue, Int64 comparand ) =>
			Interlocked.CompareExchange( ref this.Array[ index ], newValue, comparand ) == comparand;

		/// <summary>Atomically increment the current value and return the new value</summary>
		/// <param name="index">The index.</param>
		/// <returns>The decremented value.</returns>
		public Int64 AtomicDecrementAndGet( Int32 index ) => Interlocked.Decrement( ref this.Array[ index ] );

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <param name="index">   The index.</param>
		/// <returns>The original value</returns>
		public Int64 AtomicExchange( Int32 index, Int64 newValue ) => Interlocked.Exchange( ref this.Array[ index ], newValue );

		/// <summary>Atomically increment the current value and return the new value</summary>
		/// <param name="index">The index.</param>
		/// <returns>The incremented value.</returns>
		public Int64 AtomicIncrementAndGet( Int32 index ) => Interlocked.Increment( ref this.Array[ index ] );

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		public Int64 ReadAcquireFence( Int32 index ) {
			var value = this.Array[ index ];
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public Int64 ReadCompilerOnlyFence( Int32 index ) => this.Array[ index ];

		/// <summary>Read the value applying full fence semantic</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		public Int64 ReadFullFence( Int32 index ) {
			var value = this.Array[ index ];
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value without applying any fence</summary>
		/// <param name="index">The index of the element.</param>
		/// <returns>The current value.</returns>
		public Int64 ReadUnfenced( Int32 index ) => this.Array[ index ];

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( Int32 index, Int64 newValue ) => this.Array[ index ] = newValue;

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( Int32 index, Int64 newValue ) {
			this.Array[ index ] = newValue;
			Thread.MemoryBarrier();
		}

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( Int32 index, Int64 newValue ) {
			this.Array[ index ] = newValue;
			Thread.MemoryBarrier();
		}

		/// <summary>Write without applying any fence</summary>
		/// <param name="index">   The index.</param>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( Int32 index, Int64 newValue ) => this.Array[ index ] = newValue;

	}

}