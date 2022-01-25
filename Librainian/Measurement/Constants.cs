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
// File "Constants.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement;

using System;
using System.Numerics;
using Rationals;
using Utilities;

public static class Constants {

	/// <summary>
	///     <para>This value is too high. like really high.</para>
	/// </summary>
	[NeedsTesting]
	public const String ValueIsTooHigh = "this value is too high. like really high.";

	/// <summary>
	///     <para>This value is too low. like really low.</para>
	/// </summary>
	[NeedsTesting]
	public const String ValueIsTooLow = "this value is too low. like really low.";

	/// <summary><see cref="BigInteger" /> copy of System.Decimal.MaxValue (79228162514264337593543950335M)</summary>
	public static readonly BigInteger MaximumUsefulDecimal = new( Decimal.MaxValue );

	/// <summary><see cref="BigInteger" /> copy of System.Decimal.MinValue (-79228162514264337593543950335M)</summary>
	public static readonly BigInteger MinimumUsefulDecimal = new( Decimal.MinValue );

	public static readonly Rational OneOverTwo = new( 1, 2 ); //TODO overkill. But correct. lol.
}