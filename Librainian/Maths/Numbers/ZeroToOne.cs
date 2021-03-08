// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code
//  (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting.
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ZeroToOne.cs" last formatted on 2021-02-17 at 9:47 PM.

namespace Librainian.Maths.Numbers {
	using System;
	using System.Diagnostics;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     Restricts the value to between 0.0 and 1.0
	/// </summary>
	[Immutable]
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[JsonObject( MemberSerialization.Fields )]
	public record ZeroToOne {

		public const Single MaximumValue = 1f;

		public const Single MinimumValue = 0f;

		public const Single NeutralValue = MaximumValue / 2.0f;

		[JsonProperty( "v" )]
		private Single _value;

		/// <summary>
		///     <para>Restricts the value to between 0.0 and 1.0.</para>
		/// </summary>
		/// <param name="value"></param>
		public ZeroToOne( Single value ) => this.Value = value;

		/// <summary>
		///     <para>Restricts the value to between 0.0 and 1.0.</para>
		///     <para>If <paramref name="useRandomValue" /> is true then a random value between 0 and 1 will be assigned.</para>
		///     <para>Else <see cref="MinimumValue" /> will be assigned.</para>
		/// </summary>
		/// <param name="useRandomValue"></param>
		public ZeroToOne( Boolean useRandomValue ) : this( useRandomValue ? Randem.NextSingle() : MinimumValue ) { }

		public Single Value {
			get => this._value;

			init => this._value = value.Clamp( MinimumValue, MaximumValue );
		}

		/// <summary>
		///     Return a new <see cref="ZeroToOne" /> with the value of <paramref name="left" /> moved closer to the value of
		///     <paramref name="right" /> .
		/// </summary>
		/// <param name="left"> The current value.</param>
		/// <param name="right">The value to move closer towards.</param>
		/// <returns>
		///     Returns a new <see cref="ZeroToOne" /> with the value of <paramref name="left" /> moved closer to the value of
		///     <paramref name="right" /> .
		/// </returns>
		[NotNull]
		public static ZeroToOne Add( [CanBeNull] ZeroToOne left, [CanBeNull] ZeroToOne right ) => new( ( left + right ) / 2f );

		public static implicit operator Double( [NotNull] ZeroToOne value ) => value.Value;

		public static implicit operator Single( [NotNull] ZeroToOne value ) => value.Value;

		[NotNull]
		public static implicit operator ZeroToOne( Single value ) => new( value );

		[NotNull]
		public static implicit operator ZeroToOne( Double value ) => new( ( Single )value );

		[NotNull]
		public static ZeroToOne Parse( [NotNull] String value ) => new( Single.Parse( value ) );

		/// <summary>
		///     Attempt to parse <paramref name="value" />, otherwise return <see cref="MinimumValue" />.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		[NotNull]
		public static ZeroToOne TryParse( [NotNull] String value ) => Single.TryParse( value, out var result ) ? result : MinimumValue;

		[NotNull]
		public override String ToString() => $"{this.Value:P}";

	}
}