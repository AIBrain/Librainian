// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AtomicMassUnits.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "AtomicMassUnits.cs" was last formatted by Protiguous on 2018/07/13 at 1:23 AM.

namespace Librainian.Measurement.Physics {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using JetBrains.Annotations;
	using Librainian.Extensions;
	using Numerics;

	/// <summary>
	///     Units of mass and energy in ElectronVolts.
	/// </summary>
	/// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
	/// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
	/// <seealso cref="http://www.wolframalpha.com/input/?i=1+unified+atomic+mass+units+convert+to+electronvolts" />
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public struct AtomicMassUnits : IComparable<ElectronVolts>, IComparable<AtomicMassUnits> {

		/// <summary>
		/// </summary>
		public const Decimal InOneElectronVolt = 0.000000001073544m;

		public const Decimal InOneGigaElectronVolt = 1.073544m;

		public const Decimal InOneKiloElectronVolt = 0.000001073544m;

		public const Decimal InOneMegaElectronVolt = 0.001073544m;

		public const Decimal InOneMilliElectronVolt = 0.000000000001073544m;

		public const Decimal InOneTeraElectronVolt = 1073.544m;

		/// <summary>
		///     About 79228162514264337593543950335.
		/// </summary>
		public static readonly AtomicMassUnits MaxValue = new AtomicMassUnits( Decimal.MaxValue );

		/// <summary>
		///     About -79228162514264337593543950335.
		/// </summary>
		public static readonly AtomicMassUnits MinValue = new AtomicMassUnits( Decimal.MinValue );

		public static readonly AtomicMassUnits NegativeOne = new AtomicMassUnits( -1 );

		/// <summary>
		/// </summary>
		public static readonly AtomicMassUnits NegativeZero = new AtomicMassUnits( -Decimal.Zero );

		/// <summary>
		/// </summary>
		public static readonly AtomicMassUnits One = new AtomicMassUnits( 1 );

		public static readonly ElectronVolts OneAtomicUnitEqualsElectronVolt = new MegaElectronVolts( 931.494095m );

		public static readonly AtomicMassUnits OneElectronVoltEqualsAtomicMassUnits = new AtomicMassUnits( InOneElectronVolt );

		public static readonly AtomicMassUnits Zero = new AtomicMassUnits( 0 );

		/// <summary>
		/// </summary>
		public readonly Decimal Value;

		public AtomicMassUnits( Decimal value ) : this() => this.Value = value;

		public AtomicMassUnits( BigRational aBigFraction ) => this.Value = ( Decimal ) aBigFraction;

		public static AtomicMassUnits operator -( AtomicMassUnits electronVolts ) => new AtomicMassUnits( -electronVolts.Value );

		//public static implicit operator AtomicMassUnits( GigaElectronVolts gigaElectronVolts ) {
		//    return gigaElectronVolts.ToElectronVolts();
		//}

		/// <summary>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static AtomicMassUnits operator *( AtomicMassUnits left, AtomicMassUnits right ) => new AtomicMassUnits( left.Value * right.Value );

		//public static implicit operator AtomicMassUnits( MegaElectronVolts megaElectronVolts ) {
		//    return megaElectronVolts.ToElectronVolts();
		//}

		/// <summary>
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static AtomicMassUnits operator *( AtomicMassUnits left, Decimal right ) => new AtomicMassUnits( left.Value * right );

		public static AtomicMassUnits operator *( Decimal left, AtomicMassUnits right ) => new AtomicMassUnits( left * right.Value );

		public static AtomicMassUnits operator *( BigRational left, AtomicMassUnits right ) {
			var res = left * right.Value;

			return new AtomicMassUnits( ( Decimal ) res );
		}

		public static AtomicMassUnits operator *( BigInteger left, AtomicMassUnits right ) {
			var lhs = new BigRational( left );
			var rhs = new BigRational( right.Value );
			var res = lhs * rhs;

			return new AtomicMassUnits( ( Decimal ) res );
		}

		public static AtomicMassUnits operator /( AtomicMassUnits left, AtomicMassUnits right ) => new AtomicMassUnits( left.Value / right.Value );

		public static AtomicMassUnits operator /( AtomicMassUnits left, Decimal right ) => new AtomicMassUnits( left.Value / right );

		public static MegaElectronVolts operator +( AtomicMassUnits left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

		public static GigaElectronVolts operator +( AtomicMassUnits left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

		public static AtomicMassUnits operator +( AtomicMassUnits left, AtomicMassUnits right ) => new AtomicMassUnits( left.Value + right.Value );

		public static Boolean operator <( AtomicMassUnits left, AtomicMassUnits right ) => left.Value < right.Value;

		public static Boolean operator >( AtomicMassUnits left, AtomicMassUnits right ) => left.Value > right.Value;

		[Pure]
		public Int32 CompareTo( AtomicMassUnits other ) => this.Value.CompareTo( other.Value );

		[Pure]
		public Int32 CompareTo( ElectronVolts other ) => this.ToElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( TeraElectronVolts other ) => this.ToTeraElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( KiloElectronVolts other ) => this.ToKiloElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MilliElectronVolts other ) => this.ToMilliElectronVolts().Value.CompareTo( other.Value );

		public AtomicMassUnits ToElectronVolts() => new AtomicMassUnits( this.Value * InOneElectronVolt );

		public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

		public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

		public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

		public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

		/// <summary>
		///     Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
		public override String ToString() => $"{this.Value} u";

		public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );
	}
}