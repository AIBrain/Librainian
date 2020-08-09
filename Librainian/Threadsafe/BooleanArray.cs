#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>A <see cref="bool" /> array that may be updated atomically</summary>
	public class BooleanArray {

		private const Byte False = 0;

		private const Byte True = 1;

		/// <summary>Create a new <see cref="BooleanArray" /> of a given length</summary>
		/// <param name="length">Length of the array</param>
		public BooleanArray( Int32 length ) {
			if ( length < 1 ) {
				throw new ArgumentOutOfRangeException( nameof( length ) );
			}

			this.Array = new Int32[length];
		}

		/// <summary>
		///     Create a new <see cref="BooleanArray" /> with the same length as, and all elements copied from, the given
		///     array.
		/// </summary>
		/// <param name="array"></param>
		public BooleanArray( [NotNull] IEnumerable<Boolean> array ) => this.Array = array.Select( ToInt ).ToArray();

		[NotNull]
		private Int32[] Array { get; }

		/// <summary>Length of the array</summary>
		public Int32 Length => this.Array.Length;

		private static Boolean ToBool( Int32 value ) => value > False;

		private static Int32 ToInt( Boolean value ) => value ? True : False;

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <param name="index">    The index.</param>
		/// <returns>The original value</returns>
		public Boolean AtomicCompareExchange( Int32 index, Boolean newValue, Boolean comparand ) {
			var newValueInt = ToInt( newValue );
			var comparandInt = ToInt( comparand );

			return Interlocked.CompareExchange( ref this.Array[index], newValueInt, comparandInt ) == comparandInt;
		}

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <param name="index">   The index.</param>
		/// <returns>The original value</returns>
		public Boolean AtomicExchange( Int32 index, Boolean newValue ) {
			var result = Interlocked.Exchange( ref this.Array[index], ToInt( newValue ) );

			return ToBool( result );
		}

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		public Boolean ReadAcquireFence( Int32 index ) {
			var value = this.Array[index];
			Thread.MemoryBarrier();

			return ToBool( value );
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public Boolean ReadCompilerOnlyFence( Int32 index ) => ToBool( this.Array[index] );

		/// <summary>Read the value applying full fence semantic</summary>
		/// <param name="index">The element index</param>
		/// <returns>The current value</returns>
		public Boolean ReadFullFence( Int32 index ) {
			var value = this.Array[index];
			Thread.MemoryBarrier();

			return ToBool( value );
		}

		/// <summary>Read the value without applying any fence</summary>
		/// <param name="index">The index of the element.</param>
		/// <returns>The current value.</returns>
		public Boolean ReadUnfenced( Int32 index ) => ToBool( this.Array[index] );

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( Int32 index, Boolean newValue ) => this.Array[index] = ToInt( newValue );

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( Int32 index, Boolean newValue ) {
			this.Array[index] = ToInt( newValue );
			Thread.MemoryBarrier();
		}

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="index">   The element index</param>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( Int32 index, Boolean newValue ) {
			this.Array[index] = ToInt( newValue );
			Thread.MemoryBarrier();
		}

		/// <summary>Write without applying any fence</summary>
		/// <param name="index">   The index.</param>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( Int32 index, Boolean newValue ) => this.Array[index] = ToInt( newValue );

	}

}