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
// File "UniversalConstants.cs" last formatted on 2021-11-30 at 7:19 PM by Protiguous.

namespace Librainian.Measurement.Physics;

using System;
using Maths;

public static class UniversalConstants {

	/// <see cref="http://wikipedia.org/wiki/Elementary_charge" />
	public static ElectronVolts ElementaryCharge { get; } = new( 0.0000000000000000001602176565m );

	public static Decimal HalfSpin { get; } = ( Decimal )MathConstants.OneOverTwo;

	/// <summary>Precalculated -1/3 of <see cref="ElementaryCharge" />.</summary>
	public static ElectronVolts NegativeOneThirdElementaryCharge { get; } = -1m * ElementaryCharge / 3m;

	/// <summary>Precalculated -2/3 of <see cref="ElementaryCharge" />.</summary>
	public static ElectronVolts NegativeTwoThirdsElementaryCharge { get; } = -2m * ElementaryCharge / 3m;

	public static ElectronVolts PositiveOneElementaryCharge { get; } = 1m * ElementaryCharge;

	/// <summary>Precalculated 1/3 of <see cref="ElementaryCharge" />.</summary>
	public static ElectronVolts PositiveOneThirdElementaryCharge { get; } = 1m * ElementaryCharge / 3m;

	public static ElectronVolts PositiveTwoElementaryCharge { get; } = 2m * ElementaryCharge;

	/// <summary>Precalculated +2/3 of <see cref="ElementaryCharge" />.</summary>
	public static ElectronVolts PositiveTwoThirdsElementaryCharge { get; } = 2m * ElementaryCharge / 3m;

	/// <summary>Precalculated +0 of <see cref="ElementaryCharge" />.</summary>
	public static ElectronVolts ZeroElementaryCharge { get; } = 0.0M * ElementaryCharge;
}