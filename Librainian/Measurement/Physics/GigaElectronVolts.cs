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
// File "GigaElectronVolts.cs" last touched on 2021-10-13 at 4:28 PM by Protiguous.

namespace Librainian.Measurement.Physics;

using System;
using System.Diagnostics;
using ExtendedNumerics;
using Extensions;

/// <summary>Units of mass and energy in <see cref="GigaElectronVolts" />.</summary>
/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
/// <see cref="http://wikipedia.org/wiki/Giga-" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record GigaElectronVolts( BigDecimal Value ) : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>,
	IComparable<GigaElectronVolts> {

	public const Decimal InOneElectronVolt = 1E-9m;

	public const Decimal InOneGigaElectronVolt = 1E0m;

	public const Decimal InOneKiloElectronVolt = 1E-6m;

	public const Decimal InOneMegaElectronVolt = 1E-3m;

	public const Decimal InOneMilliElectronVolt = 1E-12m;

	public const Decimal InOneTeraElectronVolt = 1E3m;

	public static readonly GigaElectronVolts One = new(Decimal.One);

	public static readonly GigaElectronVolts Zero = new(Decimal.Zero);

	public GigaElectronVolts( Decimal units ) : this( ( BigDecimal )units ) { }

	public GigaElectronVolts( Double units ) : this( ( BigDecimal )units ) { }

	public GigaElectronVolts( MegaElectronVolts megaElectronVolts ) : this( megaElectronVolts.ToGigaElectronVolts() ) { }

	public Int32 CompareTo( ElectronVolts? other ) => this.Value.CompareTo( other?.ToGigaElectronVolts().Value );

	public Int32 CompareTo( GigaElectronVolts? other ) => this.Value.CompareTo( other?.Value );

	public Int32 CompareTo( MegaElectronVolts? other ) => this.Value.CompareTo( other?.ToGigaElectronVolts().Value );

	public Int32 CompareTo( MilliElectronVolts? other ) => this.Value.CompareTo( other?.ToGigaElectronVolts().Value );

	public static GigaElectronVolts operator *( GigaElectronVolts left, BigDecimal right ) => new(left.Value * right);

	public static GigaElectronVolts operator *( BigDecimal left, GigaElectronVolts right ) => new(left * right.Value);

	public static GigaElectronVolts operator +( MegaElectronVolts left, GigaElectronVolts right ) => new GigaElectronVolts( left ) + right;

	public static GigaElectronVolts operator +( GigaElectronVolts left, GigaElectronVolts right ) => new(left.Value + right.Value);

	public static Boolean operator <( GigaElectronVolts left, GigaElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

	public static Boolean operator >( GigaElectronVolts left, GigaElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

	public ElectronVolts ToElectronVolts() => new(this.Value * InOneElectronVolt);

	public GigaElectronVolts ToGigaElectronVolts() => new(this.Value * InOneGigaElectronVolt);

	public KiloElectronVolts ToKiloElectronVolts() => new(this.Value * InOneKiloElectronVolt);

	public MegaElectronVolts ToMegaElectronVolts() => new(this.Value * InOneMegaElectronVolt);

	public MilliElectronVolts ToMilliElectronVolts() => new(this.Value * InOneMilliElectronVolt);

	/// <summary>Returns the fully qualified type name of this instance.</summary>
	/// <returns>A <see cref="String" /> containing a fully qualified type name.</returns>
	public override String ToString() => $"{this.Value} GeV";

	public TeraElectronVolts ToTeraElectronVolts() => new(this.Value * InOneTeraElectronVolt);

}