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
// File "Degrees.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Spatial;

using System;
using System.Diagnostics;
using Extensions;
using Newtonsoft.Json;
using Utilities;

/// <summary>A degree is a measurement of plane angle, representing 1⁄360 of a full rotation.</summary>
/// <see cref="http://wikipedia.org/wiki/Degree_(angle)" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
[Immutable]
public record Degrees : IComparable<Degrees> {

	/// <summary>Math.PI / 180</summary>
	public const Single DegreesToRadiansFactor = ( Single )( Math.PI / 180.0f );

	/// <summary>360</summary>
	public const Single MaximumValue = ( Single )CardinalDirection.FullNorth;

	/// <summary>Just above Zero. Not Zero. Zero is <see cref="CardinalDirection.FullNorth" />.</summary>
	public const Single MinimumValue = Single.Epsilon;

	/// <summary>One <see cref="Degrees" />.</summary>
	public static readonly Degrees One = new( 1 );

	[JsonProperty]
	private volatile Single _value;

	public Degrees( Single value ) => this.Value = value;

	public Single Value {
		get => this._value;

		set {

			//BUG Is this even correct?
			value = value switch {
				< MinimumValue => MinimumValue,
				> MaximumValue => MaximumValue,
				var _ => value
			};

			this._value = value;
		}
	}

	public Int32 CompareTo( Degrees? other ) => this.Value.CompareTo( other?.Value );

	public static Degrees Combine( Degrees left, Single degrees ) => new( left.Value + degrees );

	/// <summary>
	///     <para>static equality test</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( Degrees left, Degrees right ) => Math.Abs( left.Value - right.Value ) < Double.Epsilon;

	public static implicit operator Decimal( Degrees degrees ) => ( Decimal )degrees.Value;

	public static implicit operator Double( Degrees degrees ) => degrees.Value;

	public static implicit operator Radians( Degrees degrees ) => ToRadians( degrees );

	public static implicit operator Single( Degrees degrees ) => degrees.Value;

	public static Degrees operator -( Degrees degrees ) => new( degrees.Value * -1f );

	public static Degrees operator -( Degrees left, Degrees right ) => Combine( left, -right.Value );

	public static Degrees operator -( Degrees left, Single degrees ) => Combine( left, -degrees );

	//public static Boolean operator !=( Degrees left, Degrees right ) => !Equals( left, right );

	public static Degrees operator +( Degrees left, Degrees right ) => Combine( left, right.Value );

	public static Degrees operator +( Degrees left, Single degrees ) => Combine( left, degrees );

	public static Boolean operator <( Degrees left, Degrees right ) => left.Value < right.Value;

	//public static Boolean operator ==( Degrees left, Degrees right ) => Equals( left, right );

	public static Boolean operator >( Degrees left, Degrees right ) => left.Value > right.Value;

	public static Radians ToRadians( Degrees degrees ) => new( degrees.Value * DegreesToRadiansFactor );

	public static Radians ToRadians( Single degrees ) => new( degrees * DegreesToRadiansFactor );

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public Radians ToRadians() => ToRadians( this );

	[NeedsTesting]
	public override String ToString() => $"{this.Value:F0} °";

	public static Boolean operator <=( Degrees left, Degrees right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( Degrees left, Degrees right ) => left.CompareTo( right ) >= 0;
}