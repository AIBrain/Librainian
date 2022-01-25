// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "ZeroToOne.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.Diagnostics;
using Extensions;
using Newtonsoft.Json;

/// <summary>
///     Restricts the value to between 0.0 and 1.0.
/// </summary>
[Immutable]
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public struct ZeroToOne {

	public const Single MaximumValue = 1;

	public const Single MidValue = ( MinimumValue + MaximumValue ) / 2f;

	public const Single MinimumValue = 0;

	/// <summary>
	///     <para>Restricts the value to between 0.0 and 1.0.</para>
	/// </summary>
	/// <param name="value"></param>
	public ZeroToOne( Single value ) {
		value = value switch {
			> MaximumValue => MaximumValue,
			< MinimumValue => MinimumValue,
			var _ => value
		};
		this.Value = value;
	}

	[field: JsonProperty( "v" )]
	private Single Value { get; }

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
	public static ZeroToOne Add( ZeroToOne left, ZeroToOne right ) => new( ( left + right ) / 2f );

	public static implicit operator Single( ZeroToOne value ) => value.Value;

	public static implicit operator ZeroToOne( Decimal value ) => new( ( Single )value );

	public static implicit operator ZeroToOne( Double value ) => new( ( Single )value );

	public static implicit operator ZeroToOne( Single value ) => new( value );

	public static ZeroToOne? Parse( String value ) {
		if ( Single.TryParse( value, out var sin ) ) {
			return ( Decimal )sin;
		}

		if ( Double.TryParse( value, out var dob ) ) {
			return ( Decimal )dob;
		}

		if ( Decimal.TryParse( value, out var dec ) ) {
			return dec;
		}

		return default( ZeroToOne? );
	}

	/// <summary>
	///     Attempt to parse <paramref name="value" />, otherwise return null.
	/// </summary>
	/// <param name="value"></param>
	public static ZeroToOne? TryParse( String value ) => Decimal.TryParse( value, out var result ) ? result : default( ZeroToOne? );

	public override String ToString() => $"{this.Value:P}";
}