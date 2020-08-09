#nullable enable

namespace Librainian.Threadsafe {

	using System;
	using System.Diagnostics;
	using System.Runtime.CompilerServices;
	using System.Threading;

	/// <summary>
	///     <para>A threadsafe bit.</para>
	/// </summary>
	/// <copyright>Protiguous</copyright>
	public struct VolatileBit {

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static VolatileBit Create( Boolean value ) => new VolatileBit( value );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static VolatileBit Create( Byte value ) => new VolatileBit( value );

		public const Byte On = 1;

		public const Byte Off = 0;

		private volatile Boolean _value;

		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		public VolatileBit( Byte value ) => this._value = TranslateToBoolean( value );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		private static Boolean TranslateToBoolean( Byte value ) => value != Off;

		private VolatileBit( Boolean value ) => this._value = value;

		public Byte Value {
			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			[DebuggerStepThrough]
			get => this.ReadFence();

			[MethodImpl( MethodImplOptions.AggressiveInlining )]
			[DebuggerStepThrough]
			set => this.WriteFence( TranslateToBoolean( value ) );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		private void WriteFence( Boolean value ) {
			Thread.MemoryBarrier();
			this._value = value;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		private Byte ReadFence() {
			try {
				return TranslateToByte( this._value );
			}
			finally {
				Thread.MemoryBarrier();
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		private static Byte TranslateToByte( Boolean value ) => value ? On : Off;

		public void Deconstruct( out Boolean value ) => value = this._value;                 //why?
		public void Deconstruct( out Byte value ) => value = TranslateToByte( this._value ); //why?

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public override String ToString() => this._value ? "on" : "off";

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static implicit operator Byte( VolatileBit value ) => value.ReadFence();

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static implicit operator VolatileBit( Boolean value ) => Create( value );

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[DebuggerStepThrough]
		public static implicit operator VolatileBit( Byte value ) => Create( value );

	}

}