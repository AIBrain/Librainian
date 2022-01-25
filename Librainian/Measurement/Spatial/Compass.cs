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
// File "Compass.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Spatial;

using System;
using System.Runtime.Serialization;
using Maths;

/// <summary>small number, constrained to between 0 and 360, with wrapping</summary>
[DataContract( IsReference = false )]
public class Compass {

	private volatile Single _degrees;

	public const Single Maximum = 360;

	public const Single Minimum = 0;

	/// <summary>Init with a random direction</summary>
	public Compass() : this( Randem.NextSingle( Minimum, Maximum ) ) { }

	/// <summary>ctor with <paramref name="degrees" />.</summary>
	/// <param name="degrees"></param>
	public Compass( Single degrees ) => this.Degrees = degrees;

	public Single Degrees {
		get => this._degrees;

		set {
			if ( Single.IsNaN( value ) ) {
				throw new ArgumentOutOfRangeException( nameof( value ), "Value is out of range 0 to 360" );
			}

			if ( Single.IsInfinity( value ) ) {
				throw new ArgumentOutOfRangeException( nameof( value ), "Value is out of range 0 to 360" );
			}

			while ( value < Minimum ) {
				value += Maximum; //TODO replace with math
			}

			while ( value > Maximum ) {
				value -= Maximum; //TODO replace with math
			}

			//value.Should().BeGreaterOrEqualTo( Minimum );
			//value.Should().BeLessOrEqualTo( Maximum );

			this._degrees = value;
		}
	}

	public Boolean RotateLeft( Single byAmount ) {
		if ( Single.IsNaN( byAmount ) ) {
			return false;
		}

		if ( Single.IsInfinity( byAmount ) ) {
			return false;
		}

		//TODO would a Lerp here make turning smoother?
		this.Degrees -= byAmount;

		return true;
	}

	/// <summary>Clockwise from a top-down view.</summary>
	/// <param name="byAmount"></param>
	public Boolean RotateRight( Single byAmount ) {
		if ( Single.IsNaN( byAmount ) ) {
			return false;
		}

		if ( Single.IsInfinity( byAmount ) ) {
			return false;
		}

		//TODO would a Lerp here make turning smoother?
		this.Degrees += byAmount;

		return true;
	}
}