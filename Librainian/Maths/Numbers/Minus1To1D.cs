// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Minus1To1D.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using Newtonsoft.Json;

/// <summary>Restricts the value to between -1.0 and 1.0</summary>
[JsonObject]
public struct Minus1To1D {

	/// <summary>ONLY used in the getter and setter.</summary>
	private Double _value;

	public const Double MaxValue = 1D;

	public const Double MidValue = 0D;

	public const Double MinValue = -1D;

	/// <summary>
	/// <para>Initializes a random number between -1.0 and 1.0</para>
	/// <para>Restricts the value to between -1.0 and 1.0</para>
	/// </summary>
	/// <param name="value"></param>
	public Minus1To1D( Double? value = null ) : this() {
		value ??= Randem.NextDouble( MinValue );

		this.Value = value.Value;
	}

	public Double Value {
		get => this._value;

		set =>
			this._value = value > MaxValue ? MaxValue :
				value < MinValue ? MinValue : value;
	}

	/// <summary>
	/// Return a new <see cref="Minus1To1D" /> with the value of <paramref name="value1" /> moved closer to the value of
	/// <paramref name="value2" /> .
	/// </summary>
	/// <param name="value1">The current value.</param>
	/// <param name="value2">The value to move closer towards.</param>
	/// <returns>
	/// Returns a new <see cref="Minus1To1D" /> with the value of <paramref name="value1" /> moved closer to the value of
	/// <paramref name="value2" /> .
	/// </returns>
	public static Minus1To1D Combine( Minus1To1D value1, Minus1To1D value2 ) => new( ( value1 + value2 ) / 2D );

	public static implicit operator Double( Minus1To1D special ) => special.Value;

	public static implicit operator Minus1To1D( Double value ) => new( value );

	public static Minus1To1D Parse( String value ) => new( Double.Parse( value ) );

	public override String ToString() => $"{this.Value:P}";
}