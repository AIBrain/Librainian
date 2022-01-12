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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ElectronVolts.cs" last touched on 2022-01-11 at 11:55 AM by Protiguous.

namespace Librainian.Measurement.Physics;

using System;
using System.Diagnostics;
using ExtendedNumerics;
using Extensions;
using Maths;

/// <summary>
///     Units of mass and energy in ElectronVolts.
/// </summary>
/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record ElectronVolts( BigDecimal Value ) : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {

	/// <summary>
	///     About 79228162514264337593543950335.
	/// </summary>
	public static readonly ElectronVolts MaxValue = new(Decimal.MaxValue);

	/// <summary>
	///     About -79228162514264337593543950335.
	/// </summary>
	public static readonly ElectronVolts MinValue = new(Decimal.MinValue);

	public static readonly ElectronVolts NegativeOne = new(-1m);

	public static readonly ElectronVolts NegativeZero = new(-Decimal.Zero);

	/// <summary>
	///     More than nothing (unknown but not massless).
	/// </summary>
	public static readonly ElectronVolts NonZero = new(MathExtensions.EpsilonDecimal);

	public static readonly ElectronVolts One = new(1m);

	public static readonly ElectronVolts? Zero = new(0m);

	public ElectronVolts( Decimal value ) : this( ( BigDecimal ) value ) { }

	public ElectronVolts( Double value ) : this( ( BigDecimal ) value ) { }

	public ElectronVolts( MegaElectronVolts megaElectronVolts ) : this( megaElectronVolts.ToElectronVolts() ) { }

	public ElectronVolts( GigaElectronVolts gigaElectronVolts ) : this( gigaElectronVolts.ToElectronVolts() ) { }

	public Int32 CompareTo( ElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.Value.CompareTo( other.Value );

	public Int32 CompareTo( GigaElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToGigaElectronVolts().Value.CompareTo( other.Value );

	public Int32 CompareTo( MegaElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.ToMegaElectronVolts().Value.CompareTo( other.Value );

	public Int32 CompareTo( MilliElectronVolts? other ) => other is null ? SortingOrder.NullsDefault : this.Value.CompareTo( other.ToElectronVolts().Value );

	public static implicit operator ElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToElectronVolts();

	public static implicit operator ElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToElectronVolts();

	public static implicit operator ElectronVolts( TeraElectronVolts teraElectronVolts ) => teraElectronVolts.ToElectronVolts();

	public static ElectronVolts operator -( ElectronVolts electronVolts ) => new(-electronVolts.Value);

	public static ElectronVolts operator *( ElectronVolts left, ElectronVolts right ) => new(left.Value * right.Value);

	public static ElectronVolts operator *( ElectronVolts left, Decimal right ) => new(left.Value * right);

	public static ElectronVolts operator *( Decimal left, ElectronVolts right ) => new(left * right.Value);

	public static ElectronVolts operator *( BigDecimal left, ElectronVolts right ) => new(left * right.Value);

	public static ElectronVolts operator /( ElectronVolts left, ElectronVolts right ) => new(left.Value / right.Value);

	public static ElectronVolts operator /( ElectronVolts left, Decimal right ) => new(left.Value / right);

	public static MegaElectronVolts operator +( ElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

	public static GigaElectronVolts operator +( ElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

	public static ElectronVolts operator +( ElectronVolts left, ElectronVolts right ) => new(left.Value + right.Value);

	public static Boolean operator <( ElectronVolts left, ElectronVolts right ) => left.Value < right.Value;

	public static Boolean operator <=( ElectronVolts left, ElectronVolts right ) => left.Value <= right.Value;

	public static Boolean operator >( ElectronVolts left, ElectronVolts right ) => left.Value > right.Value;

	public static Boolean operator >=( ElectronVolts left, ElectronVolts right ) => left.Value >= right.Value;

	public ElectronVolts ToElectronVolts() => new(this.Value * InOne.ElectronVolt);

	public GigaElectronVolts ToGigaElectronVolts() => new(this.Value * InOne.GigaElectronVolt);

	public KiloElectronVolts ToKiloElectronVolts() => new(this.Value * InOne.KiloElectronVolt);

	public MegaElectronVolts ToMegaElectronVolts() => new(this.Value * InOne.MegaElectronVolt);

	public MilliElectronVolts ToMilliElectronVolts() => new(this.Value * InOne.MilliElectronVolt);

	/// <summary>
	///     Returns the fully qualified type name of this instance.
	/// </summary>
	/// <returns>A <see cref="String" /> containing a fully qualified type name.</returns>
	public override String ToString() => $"{this.Value} eV";

	public TeraElectronVolts ToTeraElectronVolts() => new(this.Value * InOne.TeraElectronVolt);

	public static class InOne {

		public static readonly BigDecimal ElectronVolt = 1E0m;

		public static readonly BigDecimal GigaElectronVolt = 1E9m;

		public static readonly BigDecimal KiloElectronVolt = 1E3m;

		public static readonly BigDecimal MegaElectronVolt = 1E6m;

		public static readonly BigDecimal MilliElectronVolt = 1E-3m;

		public static readonly BigDecimal TeraElectronVolt = 1E12m;

	}

}