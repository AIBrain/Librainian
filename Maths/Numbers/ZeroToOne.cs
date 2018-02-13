// Copyright 2017 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/ZeroToOne.cs" was last cleaned by Rick on 2017/09/09 at 11:44 AM

namespace Librainian.Maths.Numbers {

	using System;
	using System.Diagnostics;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     Restricts the value to between 0.0 and 1.0
	///     <para>Uses the <see cref="Single" /> type.</para>
	/// </summary>
	[ Immutable ]
	[ DebuggerDisplay( value: "{" + nameof(ZeroToOne.ToString) + "(),nq}" ) ]
	[ JsonObject( memberSerialization: MemberSerialization.Fields ) ]
	public class ZeroToOne {

		/// <summary>ONLY used in the getter and setter.</summary>
		[ JsonProperty( "v" ) ]
		private volatile Single _value;

		public const Single MaxValue = 1f;

		public const Single MinValue = 0f;

		public const Single NeutralValue = ZeroToOne.MaxValue / 2.0f;

		/// <summary>
		///     <para>Restricts the value to between 0.0 and 1.0.</para>
		///     <para>If null is given, a random value (between 0.0 and 1.0) will be assigned.</para>
		/// </summary>
		/// <param name="value"></param>
		public ZeroToOne( Single? value = null ) {
			if ( !value.HasValue ) {
				value = Randem.NextSingle( min: ZeroToOne.MinValue, max: ZeroToOne.MaxValue );
			}
			this.Value = value.Value;
		}

		private ZeroToOne( Double value ) : this() => this.Value = ( Single )( value > ZeroToOne.MaxValue ? ZeroToOne.MaxValue : ( value < ZeroToOne.MinValue ? ZeroToOne.MinValue : value ) );

		private ZeroToOne( Single value ) : this( value: ( Single? )value ) { }

		public Single Value {
			get => this._value;

			set => this._value = value > ZeroToOne.MaxValue ? ZeroToOne.MaxValue : ( value < ZeroToOne.MinValue ? ZeroToOne.MinValue : value );
		}

		/// <summary>
		///     Return a new <see cref="ZeroToOne" /> with the value of <paramref name="value1" /> moved
		///     closer to the value of <paramref name="value2" /> .
		/// </summary>
		/// <param name="value1">The current value.</param>
		/// <param name="value2">The value to move closer towards.</param>
		/// <returns>
		///     Returns a new <see cref="ZeroToOne" /> with the value of <paramref name="value1" />
		///     moved closer to the value of <paramref name="value2" /> .
		/// </returns>
		public static ZeroToOne Combine( ZeroToOne value1, ZeroToOne value2 ) => new ZeroToOne( value: ( value1 + value2 ) / 2f );

		public static implicit operator Single( ZeroToOne special ) => special.Value;

		public static implicit operator Double( ZeroToOne special ) => special.Value;

		public static implicit operator ZeroToOne( Single value ) => new ZeroToOne( value: value );

		public static implicit operator ZeroToOne( Double value ) => new ZeroToOne( value: value );

		public static ZeroToOne Parse( String value ) => new ZeroToOne( value: Single.Parse( s: value ) );

		public override String ToString() => $"{this.Value:P}";

	}

}
