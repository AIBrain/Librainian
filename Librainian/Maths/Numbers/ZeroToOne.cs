// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ZeroToOne.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "ZeroToOne.cs" was last formatted by Protiguous on 2018/07/13 at 1:19 AM.

namespace Librainian.Maths.Numbers {

	using System;
	using System.Diagnostics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

#pragma warning disable IDE0015 // Use framework type

	/// <summary>
	///     Restricts the value to between 0.0 and 1.0
	///     <para>Uses the <see cref="float" /> type.</para>
	/// </summary>
	[Immutable]
#pragma warning restore IDE0015 // Use framework type
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject( memberSerialization: MemberSerialization.Fields )]
	public class ZeroToOne {

		/// <summary>
		///     ONLY used in the getter and setter.
		/// </summary>
		[JsonProperty( "v" )]
		private volatile Single _value;

		public Single Value {
			get => this._value;

			set => this._value = value > MaxValue ? MaxValue : ( value < MinValue ? MinValue : value );
		}

		public const Single MaxValue = 1f;

		public const Single MinValue = 0f;

		public const Single NeutralValue = MaxValue / 2.0f;

		private ZeroToOne( Double value ) : this() => this.Value = ( Single ) ( value > MaxValue ? MaxValue : ( value < MinValue ? MinValue : value ) );

		private ZeroToOne( Single value ) : this( ( Single? ) value ) { }

		/// <summary>
		///     <para>Restricts the value to between 0.0 and 1.0.</para>
		///     <para>If null is given, a random value (between 0.0 and 1.0) will be assigned.</para>
		/// </summary>
		/// <param name="value"></param>
		public ZeroToOne( Single? value = null ) {
			if ( !value.HasValue ) { value = Randem.NextSingle( min: MinValue, max: MaxValue ); }

			this.Value = value.Value;
		}

		/// <summary>
		///     Return a new <see cref="ZeroToOne" /> with the value of <paramref name="value1" /> moved closer to the value of
		///     <paramref name="value2" /> .
		/// </summary>
		/// <param name="value1">The current value.</param>
		/// <param name="value2">The value to move closer towards.</param>
		/// <returns>
		///     Returns a new <see cref="ZeroToOne" /> with the value of <paramref name="value1" /> moved closer to the value
		///     of <paramref name="value2" /> .
		/// </returns>
		[NotNull]
		public static ZeroToOne Combine( ZeroToOne value1, ZeroToOne value2 ) => new ZeroToOne( ( value1 + value2 ) / 2f );

		public static implicit operator Double( [NotNull] ZeroToOne special ) => special.Value;

		public static implicit operator Single( [NotNull] ZeroToOne special ) => special.Value;

		[NotNull]
		public static implicit operator ZeroToOne( Single value ) => new ZeroToOne( value );

		[NotNull]
		public static implicit operator ZeroToOne( Double value ) => new ZeroToOne( value );

		[NotNull]
		public static ZeroToOne Parse( [NotNull] String value ) => new ZeroToOne( Single.Parse( s: value ) );

		public override String ToString() => $"{this.Value:P}";
	}
}