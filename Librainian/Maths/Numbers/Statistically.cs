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
// File "Statistically.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using Logging;
using Newtonsoft.Json;

/// <summary>
///     <para>Ups: Probability between 0.0 and 100.0%</para>
///     <para>Downs: Probability between 0.0 and 100.0%</para>
/// </summary>
/// <remarks>Not thread safe.</remarks>
[JsonObject]
public class Statistically {

	public static readonly Statistically Zero = new();

	//public static Double Combine( Double value1, Double value2 ) { return ( value1 + value2 ) / 2D; }
	public Statistically( Double ups = 0d, Double downs = 0d ) => Reset( this, ups, downs );

	public static Statistically Undecided { get; } = new( 0.5, 0.5 );

	[JsonProperty]
	public Double Downs { get; private set; }

	public Boolean IsDowner => this.Downs > this.Ups;

	public Boolean IsProtiguous => this.IsUpper && !this.Downs.Near( 0 ) && !this.Ups.Near( 0 );

	public Boolean IsUpper => this.Ups > this.Downs;

	[JsonProperty]
	public Double Total { get; private set; }

	[JsonProperty]
	public Double Ups { get; private set; }

	public static Statistically Combine( Statistically value1, Statistically value2 ) => new( value1.Ups + value2.Ups, value1.Downs + value2.Downs );

	public static void Reset( Statistically statistically, Double newUps = 0.0, Double newDowns = 0.0 ) {
		statistically.Ups = 0d;
		statistically.Downs = 0d;
		statistically.Total = 0d;
		statistically.IncrementUps( newUps );
		statistically.IncrementDowns( newDowns );
	}

	public void Add( Statistically other ) {
		this.IncrementUps( other.Ups );
		this.IncrementDowns( other.Downs );
	}

	public Statistically Clone() => new( this.Ups, this.Downs );

	public void DecrementDowns( Double byAmount = 1d ) {
		this.Downs -= byAmount;
		this.Total -= byAmount;
	}

	public void DecrementDownsIfAny( Double byAmount = 1d ) {
		if ( this.Downs < byAmount ) {
			return;
		}

		this.Downs -= byAmount;
		this.Total -= byAmount;
	}

	public void DecrementUps( Double byAmount = 1d ) {
		this.Ups -= byAmount;
		this.Total -= byAmount;
	}

	public void DecrementUpsIfAny( Double byAmount = 1d ) {
		if ( this.Ups < byAmount ) {
			return;
		}

		this.Ups -= byAmount;
		this.Total -= byAmount;
	}

	public Double GetDownProbability() {
		try {
			var total = this.Total;

			if ( !total.Near( 0 ) ) {
				return this.Downs / total;
			}
		}
		catch ( DivideByZeroException exception ) {
			exception.Log();
		}

		return 0;
	}

	public Double GetUpProbability() {
		try {
			var total = this.Total;

			if ( !total.Near( 0 ) ) {
				return this.Ups / total;
			}
		}
		catch ( DivideByZeroException exception ) {
			exception.Log();
		}

		return 0;
	}

	/// <summary>Increments <see cref="Downs" /> and <see cref="Total" /> by <paramref name="byAmount" />.</summary>
	/// <param name="byAmount"></param>
	public void IncrementDowns( Double byAmount = 1 ) {
		this.Downs += byAmount;
		this.Total += byAmount;
	}

	/// <summary>Increments <see cref="Ups" /> and <see cref="Total" /> by <paramref name="byAmount" />.</summary>
	/// <param name="byAmount"></param>
	public void IncrementUps( Double byAmount = 1 ) {
		this.Ups += byAmount;
		this.Total += byAmount;
	}

	public override String ToString() => $"U:{this.Ups:f1} vs D:{this.Downs:f1} out of {this.Total:f1}";
}