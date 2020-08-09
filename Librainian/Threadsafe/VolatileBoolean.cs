#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	using System.Threading;

	/// <summary>
	///     <para>A threadsafe boolean.</para>
	/// </summary>
	/// <copyright>Protiguous</copyright>
	public struct VolatileBoolean {

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static VolatileBoolean Create( Boolean value ) => new VolatileBoolean( value );

		private volatile Boolean _value;

		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		public VolatileBoolean( Boolean value ) => this._value = value;

		public Boolean Value {
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			[DebuggerStepThrough]
			get => this.ReadFence();

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			[DebuggerStepThrough]
			set => this.WriteFence( value );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		private void WriteFence( Boolean value ) {
			Thread.MemoryBarrier();
			this._value = value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		private Boolean ReadFence() {
			try {
				return this._value;
			}
			finally {
				Thread.MemoryBarrier();
			}
		}

		public void Deconstruct( out Boolean value ) => value = this._value; //why?

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public override String ToString() => this._value ? "true" : "false";

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static implicit operator Boolean( VolatileBoolean value ) => value.ReadFence();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static implicit operator VolatileBoolean( Boolean value ) => Create( value );

	}

}