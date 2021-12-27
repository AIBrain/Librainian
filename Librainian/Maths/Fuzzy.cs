// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Fuzzy.cs" last formatted on 2020-08-14 at 8:36 PM.

namespace Librainian.Maths;

using System;
using Exceptions;
using Newtonsoft.Json;
using Numbers;

public enum LowMiddleHigh {

	Low,

	Middle,

	High
}

/// <summary>A Double number, constrained between 0 and 1. Kinda thread-safe by Interlocked</summary>
[JsonObject]
public class Fuzzy : ICloneable {

	/// <summary>ONLY used in the getter and setter.</summary>
	[JsonProperty]
	private AtomicDouble _value;

	public const Double HalfValue = ( MinValue + MaxValue ) / 2D;

	/// <summary>1</summary>
	public const Double MaxValue = 1D;

	/// <summary>0</summary>
	public const Double MinValue = 0D;

	/// <summary>~25 to 75% probability.</summary>
	private static PairOfDoubles Undecided { get; } = new( Combine( MinValue, HalfValue ), Combine( HalfValue, MaxValue ) );

	public static Fuzzy Empty { get; }

	public Double Value {
		get => this._value;

		set {
			if ( value > MaxValue ) {
				value = MaxValue;
			}
			else if ( value < MinValue ) {
				value = MinValue;
			}

			this._value.Value = value;
		}
	}

	/// <summary>If <paramref name="value" /> is null, then Initializes to a random number between 0 and 1.</summary>
	public Fuzzy( Double? value = null ) {
		if ( value.HasValue ) {
			this.Value = value.Value;
		}
		else {
			this.Randomize();
		}
	}

	//private static readonly Fuzzy Truer = Fuzzy.Combine( Undecided, MaxValue );
	//private static readonly Fuzzy Falser = Fuzzy.Combine( Undecided, MinValue );
	//private static readonly Fuzzy UndecidedUpper = Combine( Undecided, Truer);
	//private static readonly Fuzzy UndecidedLower = Combine( Undecided, Falser );
	public static Double Combine( Double left, Double right ) {
		if ( !left.IsNumber() ) {
			throw new ArgumentOutOfRangeException( nameof( left ) );
		}

		if ( !right.IsNumber() ) {
			throw new ArgumentOutOfRangeException( nameof( right ) );
		}

		return ( left + right ) / 2D;
	}

	public static Fuzzy? Parse( String? value ) {
		if ( String.IsNullOrWhiteSpace( value ) ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		if ( Double.TryParse( value, out var result ) ) {
			return new Fuzzy( result );
		}

		return default( Fuzzy? );
	}

	public static Boolean TryParse( String? value, out Fuzzy? result ) {
		if ( String.IsNullOrWhiteSpace( value ) ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		result = Double.TryParse( value, out var d ) ? new Fuzzy( d ) : null;

		return result != null;
	}

	public void AdjustTowardsMax() => this.Value = ( this.Value + MaxValue ) / 2D;

	public void AdjustTowardsMin() => this.Value = ( this.Value + MinValue ) / 2D;

	public Object Clone() => new Fuzzy( this.Value );

	//public Boolean IsUndecided( Fuzzy anotherFuzzy ) { return !IsTruer( anotherFuzzy ) && !IsFalser( anotherFuzzy ); }
	public Boolean IsFalseish() => this.Value < Undecided.Low;

	public Boolean IsTrueish() => this.Value > Undecided.High;

	public Boolean IsUndecided() => !this.IsTrueish() && !this.IsFalseish();

	/// <summary>Initializes a random number between 0 and 1 within a range, defaulting to Middle range (~0.50)</summary>
	public void Randomize( LowMiddleHigh? lmh = LowMiddleHigh.Middle ) {
		switch ( lmh ) {
			case null:
				this.Value = Randem.NextDouble();

				break;

			default:

				switch ( lmh.Value ) {
					case LowMiddleHigh.Low:

						do {
							this.Value = Randem.NextDouble( 0.0D, 0.25D );
						} while ( this.Value is < MinValue or > 0.25D );

						break;

					case LowMiddleHigh.Middle:

						do {
							this.Value = Randem.NextDouble( 0.25D, 0.75D );
						} while ( this.Value is < 0.25D or > 0.75D );

						break;

					case LowMiddleHigh.High:

						do {
							this.Value = Randem.NextDouble();
						} while ( this.Value is < 0.75D or > MaxValue );

						break;

					default:
						this.Value = Randem.NextDouble();

						break;
				}

				break;
		}
	}

	public override String ToString() => $"{this.Value:R}";
}