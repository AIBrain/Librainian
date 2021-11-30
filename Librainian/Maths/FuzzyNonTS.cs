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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FuzzyNonTS.cs" last touched on 2021-10-13 at 4:27 PM by Protiguous.

namespace Librainian.Maths;

using System;
using Newtonsoft.Json;

/// <summary>A Double number, constrained between 0 and 1. Not thread safe!</summary>
[JsonObject]
public class FuzzyNonTs {

	public const Double MaxValue = 1D;

	public const Double MinValue = 0D;

	/// <summary>ONLY used in the getter and setter.</summary>
	[JsonProperty]
	private Double _value;

	public FuzzyNonTs( Double value ) => this.Value = value;

	public FuzzyNonTs( LowMiddleHigh lmh = LowMiddleHigh.Middle ) => this.Randomize( lmh );

	public static FuzzyNonTs Falser { get; } = new(new FuzzyNonTs( 0.5D ) - new FuzzyNonTs( 0.5D ) / 2);

	public static FuzzyNonTs Truer { get; } = new(new FuzzyNonTs( 0.5D ) + new FuzzyNonTs( 0.5D ) / 2);

	public static FuzzyNonTs Undecided { get; } = new(0.5D);

	public Double Value {
		get => this._value;

		set {
			var correctedvalue = value;

			if ( value > MaxValue ) {
				correctedvalue = MaxValue;
			}
			else if ( value < MinValue ) {
				correctedvalue = MinValue;
			}

			this._value = correctedvalue;
		}
	}

	public static FuzzyNonTs Combine( FuzzyNonTs? value1, FuzzyNonTs? value2 ) => new(( value1 + value2 ) / 2D);

	public static FuzzyNonTs Combine( FuzzyNonTs? value1, Double value2 ) => new(( value1 + value2 ) / 2D);

	public static FuzzyNonTs Combine( Double value1, FuzzyNonTs? value2 ) => new(( value1 + value2 ) / 2D);

	public static Double Combine( Double value1, Double value2 ) => ( value1 + value2 ) / 2D;

	public static implicit operator Double( FuzzyNonTs special ) => special.Value;

	public static Boolean IsFalser( FuzzyNonTs special ) => special.Value <= Falser.Value;

	public static Boolean IsTruer( FuzzyNonTs special ) => special.Value >= Truer.Value;

	public static Boolean IsUndecided( FuzzyNonTs special ) => !IsTruer( special ) && !IsFalser( special );

	public static FuzzyNonTs Parse( String value ) => new(Double.Parse( value ));

	public void LessLikely() => this.Value = ( this.Value + MinValue ) / 2D;

	public void MoreLikely( FuzzyNonTs? towards = null ) => this.Value = ( this.Value + ( towards ?? MaxValue ) ) / 2D;

	public void MoreLikely( Double towards ) => this.Value = ( this.Value + ( towards >= MinValue ? towards : MaxValue ) ) / 2D;

	/// <summary>
	///     Initializes a random number between 0 and 1 within a range, default is <see cref="LowMiddleHigh.Middle" />
	/// </summary>
	public void Randomize( LowMiddleHigh lmh = LowMiddleHigh.Middle ) {
		this.Value = lmh switch {
			LowMiddleHigh.Low => Randem.NextDouble() / 10,
			LowMiddleHigh.Middle => ( 1 - Randem.NextDouble() / 10 ) / 2,
			LowMiddleHigh.High => 1 - Randem.NextDouble() / 10,
			var _ => Randem.NextDouble()
		};
	}

	public override String ToString() => $"{this.Value:R}";

}