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
// File "AtomicMassUnits.cs" last touched on 2021-06-19 at 12:55 AM by Protiguous.

namespace Librainian.Measurement.Physics {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Extensions;
	using JetBrains.Annotations;
	using Maths.Bigger;

	/// <summary>Units of mass and energy in ElectronVolts.</summary>
	/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
	/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
	/// <see cref="http://www.wolframalpha.com/input/?i=1+unified+atomic+mass+units+convert+to+electronvolts" />
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public record AtomicMassUnits( BigDecimal Value ) : IComparable<ElectronVolts>, IComparable<AtomicMassUnits> {

		/// <summary></summary>
		public const Decimal InOneElectronVolt = 0.000000001073544m;

		public const Decimal InOneGigaElectronVolt = 1.073544m;

		public const Decimal InOneKiloElectronVolt = 0.000001073544m;

		public const Decimal InOneMegaElectronVolt = 0.001073544m;

		public const Decimal InOneMilliElectronVolt = 0.000000000001073544m;

		public const Decimal InOneTeraElectronVolt = 1073.544m;

		/// <summary>About 79228162514264337593543950335.</summary>
		public static readonly AtomicMassUnits MaxValue = new(Decimal.MaxValue);

		/// <summary>About -79228162514264337593543950335.</summary>
		public static readonly AtomicMassUnits MinValue = new(Decimal.MinValue);

		public static readonly AtomicMassUnits NegativeOne = new(-1m);

		/// <summary></summary>
		public static readonly AtomicMassUnits NegativeZero = new(-Decimal.Zero);

		/// <summary></summary>
		public static readonly AtomicMassUnits One = new(1m);

		public static readonly ElectronVolts OneAtomicUnitEqualsElectronVolt = new MegaElectronVolts( 931.494095m );

		public static readonly AtomicMassUnits OneElectronVoltEqualsAtomicMassUnits = new(InOneElectronVolt);

		public static readonly AtomicMassUnits Zero = new(0m);

		public AtomicMassUnits( Decimal value ) : this( ( BigDecimal )value ) { }

		[Pure]
		public Int32 CompareTo( AtomicMassUnits? other ) => this.Value.CompareTo( other?.Value );

		[Pure]
		public Int32 CompareTo( ElectronVolts? other ) => this.ToElectronVolts().Value.CompareTo( other?.Value );

		public static AtomicMassUnits operator -( AtomicMassUnits electronVolts ) => new(-electronVolts.Value);

		//public static implicit operator AtomicMassUnits( GigaElectronVolts gigaElectronVolts ) {
		//    return gigaElectronVolts.ToElectronVolts();
		//}

		/// <summary></summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static AtomicMassUnits operator *( AtomicMassUnits left, AtomicMassUnits right ) => new(left.Value * right.Value);

		//public static implicit operator AtomicMassUnits( MegaElectronVolts megaElectronVolts ) {
		//    return megaElectronVolts.ToElectronVolts();
		//}

		/// <summary></summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static AtomicMassUnits operator *( AtomicMassUnits left, Decimal right ) => new(left.Value * right);

		public static AtomicMassUnits operator *( Decimal left, AtomicMassUnits right ) => new(left * right.Value);

		public static AtomicMassUnits operator *( BigDecimal left, AtomicMassUnits right ) {
			var res = left * right.Value;

			return new AtomicMassUnits( res );
		}

		public static AtomicMassUnits operator *( BigInteger left, AtomicMassUnits right ) {
			var res = left * right.Value;

			return new AtomicMassUnits( res );
		}

		public static AtomicMassUnits operator /( AtomicMassUnits left, AtomicMassUnits right ) => new(left.Value / right.Value);

		public static AtomicMassUnits operator /( AtomicMassUnits left, Decimal right ) => new(left.Value / right);

		public static MegaElectronVolts operator +( AtomicMassUnits left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

		public static GigaElectronVolts operator +( AtomicMassUnits left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

		public static AtomicMassUnits operator +( AtomicMassUnits left, AtomicMassUnits right ) => new(left.Value + right.Value);

		public static Boolean operator <( AtomicMassUnits left, AtomicMassUnits right ) => left.Value < right.Value;

		public static Boolean operator >( AtomicMassUnits left, AtomicMassUnits right ) => left.Value > right.Value;

		public Int32 CompareTo( TeraElectronVolts other ) => this.ToTeraElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( KiloElectronVolts other ) => this.ToKiloElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MilliElectronVolts other ) => this.ToMilliElectronVolts().Value.CompareTo( other.Value );

		public AtomicMassUnits ToElectronVolts() => new(this.Value * InOneElectronVolt);

		public GigaElectronVolts ToGigaElectronVolts() => new(this.Value * InOneGigaElectronVolt);

		public KiloElectronVolts ToKiloElectronVolts() => new(this.Value * InOneKiloElectronVolt);

		public MegaElectronVolts ToMegaElectronVolts() => new(this.Value * InOneMegaElectronVolt);

		public MilliElectronVolts ToMilliElectronVolts() => new(this.Value * InOneMilliElectronVolt);

		public override String ToString() => $"{this.Value} u";	//is this not "amu"?

		public TeraElectronVolts ToTeraElectronVolts() => new(this.Value * InOneTeraElectronVolt);

	}

}