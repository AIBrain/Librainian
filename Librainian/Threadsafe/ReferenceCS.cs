#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>A reference that may be updated atomically</summary>
	public struct Reference<T> where T : class {

		private T? _value;

		/// <summary>Create a new <see cref="Reference{T}" /> with the given initial value.</summary>
		/// <param name="value">Initial value</param>
		public Reference( [CanBeNull] T? value ) => this._value = value;

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <returns></returns>
		public Boolean AtomicCompareExchange( [CanBeNull] T? newValue, [CanBeNull] T? comparand ) =>
			Interlocked.CompareExchange( ref this._value, newValue, comparand ) == comparand;

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <returns>The original value</returns>
		[CanBeNull]
		public T? AtomicExchange( [CanBeNull] T? newValue ) => Interlocked.Exchange( ref this._value, newValue );

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <returns>The current value</returns>
		[CanBeNull]
		public T? ReadAcquireFence() {
			var value = this._value;
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		[CanBeNull]
		public T? ReadCompilerOnlyFence() => this._value;

		/// <summary>Read the value applying full fence semantic</summary>
		/// <returns>The current value</returns>
		[CanBeNull]
		public T? ReadFullFence() {
			var value = this._value;
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value without applying any fence</summary>
		/// <returns>The current value</returns>
		[CanBeNull]
		public T? ReadUnfenced() => this._value;

		/// <summary>Returns the String representation of the current value.</summary>
		/// <returns>the String representation of the current value.</returns>
		public override String? ToString() {
			var value = this.ReadFullFence();

			return value?.ToString();
		}

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( [CanBeNull] T? newValue ) => this._value = newValue;

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( [CanBeNull] T? newValue ) {
			Thread.MemoryBarrier();
			this._value = newValue;
		}

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( [CanBeNull] T? newValue ) {
			Thread.MemoryBarrier();
			this._value = newValue;
		}

		/// <summary>Write without applying any fence</summary>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( [CanBeNull] T? newValue ) => this._value = newValue;

	}

}