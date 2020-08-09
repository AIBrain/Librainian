#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>An integer value that may be updated atomically</summary>
	public struct Integer {

		private Int32 _value;

		/// <summary>Create a new <see cref="Integer" /> with the given initial value.</summary>
		/// <param name="value">Initial value</param>
		public Integer( Int32 value ) => this._value = value;

		/// <summary>Atomically add the given value to the current value and return the sum</summary>
		/// <param name="delta">The value to be added</param>
		/// <returns>The sum of the current value and the given value</returns>
		public Int32 AtomicAddAndGet( Int32 delta ) => Interlocked.Add( ref this._value, delta );

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <returns></returns>
		public Boolean AtomicCompareExchange( Int32 newValue, Int32 comparand ) => Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

		/// <summary>Atomically increment the current value and return the new value</summary>
		/// <returns>The decremented value.</returns>
		public Int32 AtomicDecrementAndGet() => Interlocked.Decrement( ref this._value );

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <returns>The original value</returns>
		public Int32 AtomicExchange( Int32 newValue ) => Interlocked.Exchange( ref this._value, newValue );

		/// <summary>Atomically increment the current value and return the new value</summary>
		/// <returns>The incremented value.</returns>
		public Int32 AtomicIncrementAndGet() => Interlocked.Increment( ref this._value );

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <returns>The current value</returns>
		public Int32 ReadAcquireFence() {
			var value = this._value;
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public Int32 ReadCompilerOnlyFence() => this._value;

		/// <summary>Read the value applying full fence semantic</summary>
		/// <returns>The current value</returns>
		public Int32 ReadFullFence() {
			var value = this._value;
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value without applying any fence</summary>
		/// <returns>The current value</returns>
		public Int32 ReadUnfenced() => this._value;

		/// <summary>Returns the String representation of the current value.</summary>
		/// <returns>the String representation of the current value.</returns>
		[NotNull]
		public override String ToString() {
			var value = this.ReadFullFence();

			return value.ToString();
		}

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( Int32 newValue ) => this._value = newValue;

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( Int32 newValue ) {
			this._value = newValue;
			Thread.MemoryBarrier();
		}

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( Int32 newValue ) {
			this._value = newValue;
			Thread.MemoryBarrier();
		}

		/// <summary>Write without applying any fence</summary>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( Int32 newValue ) => this._value = newValue;

	}

}