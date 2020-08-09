#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>
	///     A boolean value that may be updated atomically and is guaranteed to live on its own cache line (to prevent
	///     false sharing)
	/// </summary>
	[StructLayout( LayoutKind.Explicit, Size = Volatiles.CacheLineSize * 2 )]
	public struct PaddedBoolean {

		// Boolean stored as an int, CAS not available on Boolean
		[FieldOffset( Volatiles.CacheLineSize )]
		private Int32 _value;

		private const Byte False = 0;

		private const Byte True = 1;

		/// <summary>Create a new <see cref="PaddedBoolean" /> with the given initial value.</summary>
		/// <param name="value">Initial value</param>
		public PaddedBoolean( Boolean value ) => this._value = value ? True : False;

		/// <summary>Read the value without applying any fence</summary>
		/// <returns>The current value</returns>
		public Boolean ReadUnfenced() => ToBool( this._value );

		/// <summary>Read the value applying acquire fence semantic</summary>
		/// <returns>The current value</returns>
		public Boolean ReadAcquireFence() {
			var value = ToBool( this._value );
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying full fence semantic</summary>
		/// <returns>The current value</returns>
		public Boolean ReadFullFence() {
			var value = ToBool( this._value );
			Thread.MemoryBarrier();

			return value;
		}

		/// <summary>Read the value applying a compiler only fence, no CPU fence is applied</summary>
		/// <returns>The current value</returns>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public Boolean ReadCompilerOnlyFence() => ToBool( this._value );

		/// <summary>Write the value applying release fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteReleaseFence( Boolean newValue ) {
			var newValueInt = ToInt( newValue );
			Thread.MemoryBarrier();
			this._value = newValueInt;
		}

		/// <summary>Write the value applying full fence semantic</summary>
		/// <param name="newValue">The new value</param>
		public void WriteFullFence( Boolean newValue ) {
			var newValueInt = ToInt( newValue );
			Thread.MemoryBarrier();
			this._value = newValueInt;
		}

		/// <summary>Write the value applying a compiler fence only, no CPU fence is applied</summary>
		/// <param name="newValue">The new value</param>
		[MethodImpl( MethodImplOptions.NoOptimization )]
		public void WriteCompilerOnlyFence( Boolean newValue ) => this._value = ToInt( newValue );

		/// <summary>Write without applying any fence</summary>
		/// <param name="newValue">The new value</param>
		public void WriteUnfenced( Boolean newValue ) => this._value = ToInt( newValue );

		/// <summary>Atomically set the value to the given updated value if the current value equals the comparand</summary>
		/// <param name="newValue"> The new value</param>
		/// <param name="comparand">The comparand (expected value)</param>
		/// <returns></returns>
		public Boolean AtomicCompareExchange( Boolean newValue, Boolean comparand ) {
			var newValueInt = ToInt( newValue );
			var comparandInt = ToInt( comparand );

			return Interlocked.CompareExchange( ref this._value, newValueInt, comparandInt ) == comparandInt;
		}

		/// <summary>Atomically set the value to the given updated value</summary>
		/// <param name="newValue">The new value</param>
		/// <returns>The original value</returns>
		public Boolean AtomicExchange( Boolean newValue ) {
			var newValueInt = ToInt( newValue );
			var originalValue = Interlocked.Exchange( ref this._value, newValueInt );

			return ToBool( originalValue );
		}

		/// <summary>Returns the String representation of the current value.</summary>
		/// <returns>the String representation of the current value.</returns>
		[NotNull]
		public override String ToString() {
			var value = this.ReadFullFence();

			return value.ToString();
		}

		private static Boolean ToBool( Int32 value ) {
			if ( value != False && value != True ) {
				throw new ArgumentOutOfRangeException( nameof( value ) );
			}

			return value == True;
		}

		private static Int32 ToInt( Boolean value ) => value ? True : False;

	}

}