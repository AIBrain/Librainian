// Copyright � Protiguous. All Rights Reserved.
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
// File "PlanckLengths.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Length;

using System;
using System.Diagnostics;
using Maths.Numbers;
using Newtonsoft.Json;

/// <summary>
///     <see cref="http://wikipedia.org/wiki/Plank_length" />
/// </summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public record PlanckLengths( UBigInteger Value ) : IComparable<PlanckLengths> {

	/// <summary>One <see cref="PlanckLengths" />.</summary>
	public static readonly PlanckLengths One = new( 1 );

	/// <summary>Two <see cref="PlanckLengths" />.</summary>
	public static readonly PlanckLengths Two = new( 2 );

	/// <summary>Zero <see cref="PlanckLengths" />.</summary>
	public static readonly PlanckLengths Zero = new( 0 );

	public Int32 CompareTo( PlanckLengths? other ) => this.Value.CompareTo( other?.Value );

	public override Int32 GetHashCode() => this.Value.GetHashCode();

	public override String ToString() => $"{this.Value}";

	public static Boolean operator <( PlanckLengths left, PlanckLengths right ) => left.CompareTo( right ) < 0;

	public static Boolean operator <=( PlanckLengths left, PlanckLengths right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >( PlanckLengths left, PlanckLengths right ) => left.CompareTo( right ) > 0;

	public static Boolean operator >=( PlanckLengths left, PlanckLengths right ) => left.CompareTo( right ) >= 0;

	//public static implicit operator Span( PlanckUnits milliseconds ) {
	//    return Span.FromMilliseconds( milliseconds: milliseconds.Value );
	//}

	//public static Boolean operator <( PlanckUnits left, PlanckUnits right ) {
	//    return left.Value.CompareTo( right.Value ) < 0;
	//}

	//public static Boolean operator <( PlanckUnits left, Seconds right ) {
	//    return left.Comparison( right ) < 0;
	//}

	//public static Boolean operator <( PlanckUnits left, Minutes right ) {
	//    return left.Comparison( right ) < 0;
	//}

	//public static Boolean operator >( PlanckUnits left, PlanckUnits right ) {
	//    return left.Value.CompareTo( right.Value ) > 0;
	//}

	//public static Boolean operator >( PlanckUnits left, Seconds right ) {
	//    return left.Comparison( right ) > 0;
	//}
}