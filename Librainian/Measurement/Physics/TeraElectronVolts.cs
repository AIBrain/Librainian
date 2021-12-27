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
// File "TeraElectronVolts.cs" last touched on 2021-12-16 at 4:59 AM by Protiguous.

namespace Librainian.Measurement.Physics;

using System;
using System.Diagnostics;
using ExtendedNumerics;
using Extensions;

/// <summary>Units of mass and energy in <see cref="TeraElectronVolts" />.</summary>
/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
/// <see cref="http://wikipedia.org/wiki/Giga-" />
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[Immutable]
public record TeraElectronVolts( BigDecimal Value ) : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>,
	IComparable<TeraElectronVolts> {

	public const Decimal InOneElectronVolt = 1E-12m;

	public const Decimal InOneGigaElectronVolt = 1E-3m;

	public const Decimal InOneKiloElectronVolt = 1E-9m;

	public const Decimal InOneMegaElectronVolt = 1E-6m;

	public const Decimal InOneMilliElectronVolt = 1E-15m;

	public const Decimal InOneTeraElectronVolt = 1E0m;

	public static readonly TeraElectronVolts One = new(1m);

	public static readonly TeraElectronVolts Zero = new(0m);

	public TeraElectronVolts( Decimal units ) : this( ( BigDecimal ) units ) { }

	public TeraElectronVolts( Double units ) : this( ( BigDecimal ) units ) { }

	public TeraElectronVolts( GigaElectronVolts gigaElectronVolts ) : this( gigaElectronVolts.ToTeraElectronVolts() ) { }

	public TeraElectronVolts( MegaElectronVolts megaElectronVolts ) : this( megaElectronVolts.ToTeraElectronVolts() ) { }

	public TeraElectronVolts( KiloElectronVolts kiloElectronVolts ) : this( kiloElectronVolts.ToTeraElectronVolts() ) { }

	public Int32 CompareTo( ElectronVolts? other ) => this.Value.CompareTo( other?.ToTeraElectronVolts().Value );

	public Int32 CompareTo( MegaElectronVolts? other ) => this.Value.CompareTo( other?.ToTeraElectronVolts().Value );

	public Int32 CompareTo( MilliElectronVolts? other ) => this.Value.CompareTo( other?.ToTeraElectronVolts().Value );

	public Int32 CompareTo( TeraElectronVolts? other ) => this.Value.CompareTo( other?.Value );

	public static TeraElectronVolts operator *( TeraElectronVolts left, BigDecimal right ) => new(left.Value * right);

	public static TeraElectronVolts operator *( BigDecimal left, TeraElectronVolts right ) => new(left * right.Value);

	public static TeraElectronVolts operator +( GigaElectronVolts left, TeraElectronVolts right ) => new(left.ToTeraElectronVolts().Value + right.Value);

	public static TeraElectronVolts operator +( TeraElectronVolts left, TeraElectronVolts right ) => new(left.Value + right.Value);

	public static Boolean operator <( TeraElectronVolts left, TeraElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

	public static Boolean operator >( TeraElectronVolts left, TeraElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

	public ElectronVolts ToElectronVolts() => new(this.Value * InOneElectronVolt);

	public GigaElectronVolts ToGigaElectronVolts() => new(this.Value * InOneGigaElectronVolt);

	public KiloElectronVolts ToKiloElectronVolts() => new(this.Value * InOneKiloElectronVolt);

	public MegaElectronVolts ToMegaElectronVolts() => new(this.Value * InOneMegaElectronVolt);

	public MilliElectronVolts ToMilliElectronVolts() => new(this.Value * InOneMilliElectronVolt);

	public override String ToString() => $"{this.Value} TeV";

	public TeraElectronVolts ToTeraElectronVolts() => new(this.Value * InOneTeraElectronVolt);

	public static Boolean operator <=( TeraElectronVolts left, TeraElectronVolts right ) => left.CompareTo( right ) <= 0;

	public static Boolean operator >=( TeraElectronVolts left, TeraElectronVolts right ) => left.CompareTo( right ) >= 0;

}