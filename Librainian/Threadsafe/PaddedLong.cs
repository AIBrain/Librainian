#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>
	///     A long value that may be updated atomically and is guaranteed to live on its own cache line (to prevent false
	///     sharing)
	/// </summary>
	[StructLayout( LayoutKind.Explicit, Size = Volatiles.CacheLineSize * 2 )]
	public struct PaddedLong {

		[FieldOffset( Volatiles.CacheLineSize )]
		private Int64 _value;

		/// <summary>Create a new <see cref="PaddedLong" /> with the given initial value.</summary>
		/// <param name="value">Initial value</param>
		public PaddedLong( Int64 value ) => this._value = value;

		/// <summary>Read the value without applying any fence</summary>
		/// <returns>The current value</returns>
		public Int64 ReadUnfenced() => this._value;

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <returns>The current value</returns>
		public Int64 ReadAcquireFence() {
			var value = this._value;
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying full fence semantic</summary>
		/// <returns>The current value</returns>
		public Int64 ReadFullFence() {
			Thread.MemoryBarrier();

			return this._value;
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public Int64 ReadCompilerOnlyFence() => this._value;

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( Int64 newValue ) {
			Thread.MemoryBarrier();
			this._value = newValue;
		}

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( Int64 newValue ) {
			Thread.MemoryBarrier();
			this._value = newValue;
		}

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( Int64 newValue ) => this._value = newValue;

		/// <summary>Write without applying any fence</summary>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( Int64 newValue ) => this._value = newValue;

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <returns></returns>
		public Boolean AtomicCompareExchange( Int64 newValue, Int64 comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <returns>The original value</returns>
		public Int64 AtomicExchange( Int64 newValue ) => Interlocked.Exchange( ref this._value, newValue );

		/// <summary>Atomically add the given value to the current value and return the sum</summary>
		/// <param name="delta">The value to be added</param>
		/// <returns>The sum of the current value and the given value</returns>
		public Int64 AtomicAddAndGet( Int64 delta ) => Interlocked.Add( ref this._value, delta );

		/// <summary>Atomically increment the current value and return the new value</summary>
		/// <returns>The incremented value.</returns>
		public Int64 AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

		/// <summary>Atomically increment the current value and return the new value</summary>
		/// <returns>The decremented value.</returns>
		public Int64 AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

		/// <summary>Returns the String representation of the current value.</summary>
		/// <returns>the String representation of the current value.</returns>
		[NotNull]
		public override String ToString() {
			var value = this.ReadFullFence();

			return value.ToString();
		}

	}

}