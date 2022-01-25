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
// File "KiloElectronVolts.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Physics;

using System;
using System.Diagnostics;
using System.Numerics;
using ExtendedNumerics;
using Extensions;

/// <summary>
///     Units of mass and energy in ElectronVolts.
/// </summary>
/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record KiloElectronVolts( BigDecimal Value ) : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<KiloElectronVolts>,
	IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {
	public const Decimal InOneElectronVolt = 1E-3m;

	public const Decimal InOneGigaElectronVolt = 1E6m;

	public const Decimal InOneKiloElectronVolt = 1E0m;

	public const Decimal InOneMegaElectronVolt = 1E3m;

	public const Decimal InOneMilliElectronVolt = 1E-6m;

	public const Decimal InOneTeraElectronVolt = 1E9m;

	public KiloElectronVolts( Decimal value ) : this( ( BigDecimal )value ) { }

	public KiloElectronVolts( MegaElectronVolts megaElectronVolts ) : this( megaElectronVolts.ToKiloElectronVolts() ) { }

	public KiloElectronVolts( GigaElectronVolts gigaElectronVolts ) : this( gigaElectronVolts.ToKiloElectronVolts() ) { }

	public KiloElectronVolts( ElectronVolts electronVolts ) : this( electronVolts.ToKiloElectronVolts() ) { }

	/// <summary>
	///     About 79228162514264337593543950335.
	/// </summary>
	public static KiloElectronVolts MaxValue { get; } = new( Decimal.MaxValue );

	/// <summary>
	///     About -79228162514264337593543950335.
	/// </summary>
	public static KiloElectronVolts MinValue { get; } = new( Decimal.MinValue );

	public static KiloElectronVolts NegativeOne { get; } = new( -1m );

	public static KiloElectronVolts NegativeZero { get; } = new( -Decimal.Zero );

	public static KiloElectronVolts One { get; } = new( 1m );

	public static KiloElectronVolts Zero { get; } = new( 0m );

	public Int32 CompareTo( ElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.Value.CompareTo( other.ToKiloElectronVolts().Value );

	public Int32 CompareTo( GigaElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToGigaElectronVolts().Value.CompareTo( other.Value );

	public Int32 CompareTo( KiloElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.Value.CompareTo( other.Value );

	public Int32 CompareTo( MegaElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToMegaElectronVolts().Value.CompareTo( other.Value );

	public Int32 CompareTo( MilliElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.Value.CompareTo( other.ToKiloElectronVolts().Value );

	public static implicit operator KiloElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToKiloElectronVolts();

	public static implicit operator KiloElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToKiloElectronVolts();

	public static KiloElectronVolts operator -( KiloElectronVolts electronVolts ) => new( -electronVolts.Value );

	public static KiloElectronVolts operator *( KiloElectronVolts left, KiloElectronVolts right ) => new( left.Value * right.Value );

	public static KiloElectronVolts operator *( KiloElectronVolts left, Decimal right ) => new( left.Value * right );

	public static KiloElectronVolts operator *( Decimal left, KiloElectronVolts right ) => new( left * right.Value );

	public static KiloElectronVolts operator *( BigDecimal left, KiloElectronVolts right ) => new( left * right.Value );

	public static KiloElectronVolts operator *( BigInteger left, KiloElectronVolts right ) => new( new BigDecimal( left ) * right.Value );

	public static KiloElectronVolts operator /( KiloElectronVolts left, KiloElectronVolts right ) => new( left.Value / right.Value );

	public static KiloElectronVolts operator /( KiloElectronVolts left, Decimal right ) => new( left.Value / right );

	public static MegaElectronVolts operator +( KiloElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

	public static GigaElectronVolts operator +( KiloElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

	public static KiloElectronVolts operator +( KiloElectronVolts left, KiloElectronVolts right ) => new( left.Value + right.Value );

	public static Boolean operator <( KiloElectronVolts left, KiloElectronVolts right ) => left.Value < right.Value;

	public static Boolean operator >( KiloElectronVolts left, KiloElectronVolts right ) => left.Value > right.Value;

	public ElectronVolts ToElectronVolts() => new( this.Value * InOneElectronVolt );

	public GigaElectronVolts ToGigaElectronVolts() => new( this.Value * InOneGigaElectronVolt );

	public KiloElectronVolts ToKiloElectronVolts() => new( this.Value * InOneKiloElectronVolt );

	public MegaElectronVolts ToMegaElectronVolts() => new( this.Value * InOneMegaElectronVolt );

	public MilliElectronVolts ToMilliElectronVolts() => new( this.Value * InOneMilliElectronVolt );

	/// <summary>
	///     Returns the fully qualified type name of this instance.
	/// </summary>
	/// <returns>A <see cref="String" /> containing a fully qualified type name.</returns>
	public override String ToString() => $"{this.Value} eV";

	public TeraElectronVolts ToTeraElectronVolts() => new( this.Value * InOneTeraElectronVolt );

	public static Boolean operator <=( KiloElectronVolts left, KiloElectronVolts right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( KiloElectronVolts left, KiloElectronVolts right ) => left.CompareTo( right ) >= 0;
}