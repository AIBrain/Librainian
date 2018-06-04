// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ElectronVolts.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
// 
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "ElectronVolts.cs" was last formatted by Protiguous on 2018/06/04 at 4:10 PM.

namespace Librainian.Measurement.Physics {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Librainian.Extensions;
	using Maths;
	using Numerics;
	using NUnit.Framework;

	/// <summary>
	///     Units of mass and energy in ElectronVolts.
	/// </summary>
	/// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
	/// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public struct ElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {

		public const Decimal InOneElectronVolt = 1E0m;

		public const Decimal InOneGigaElectronVolt = 1E9m;

		public const Decimal InOneKiloElectronVolt = 1E3m;

		public const Decimal InOneMegaElectronVolt = 1E6m;

		public const Decimal InOneMilliElectronVolt = 1E-3m;

		public const Decimal InOneTeraElectronVolt = 1E12m;

		/// <summary>
		///     About 79228162514264337593543950335.
		/// </summary>
		public static readonly ElectronVolts MaxValue = new ElectronVolts( Decimal.MaxValue );

		/// <summary>
		///     About -79228162514264337593543950335.
		/// </summary>
		public static readonly ElectronVolts MinValue = new ElectronVolts( Decimal.MinValue );

		public static readonly ElectronVolts NegativeOne = new ElectronVolts( -1 );

		/// <summary>
		/// </summary>
		public static readonly ElectronVolts NegativeZero = new ElectronVolts( -Decimal.Zero );

		/// <summary>
		///     More than nothing (unknown but not massless).
		/// </summary>
		public static readonly ElectronVolts NonZero = new ElectronVolts( Constants.EpsilonDecimal );

		/// <summary>
		/// </summary>
		public static readonly ElectronVolts One = new ElectronVolts( 1 );

		public static readonly ElectronVolts Zero = new ElectronVolts( 0 );

		public readonly Decimal Value;

		public ElectronVolts( Decimal value ) : this() => this.Value = value;

		public ElectronVolts( MegaElectronVolts megaElectronVolts ) => this.Value = megaElectronVolts.ToElectronVolts().Value;

		public ElectronVolts( BigRational aBigFraction ) => this.Value = ( Decimal ) aBigFraction;

		public ElectronVolts( GigaElectronVolts gigaElectronVolts ) => this.Value = gigaElectronVolts.ToElectronVolts().Value;

		public static implicit operator ElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToElectronVolts();

		public static implicit operator ElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToElectronVolts();

		public static ElectronVolts operator -( ElectronVolts electronVolts ) => new ElectronVolts( -electronVolts.Value );

		public static ElectronVolts operator *( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value * right.Value );

		public static ElectronVolts operator *( ElectronVolts left, Decimal right ) => new ElectronVolts( left.Value * right );

		public static ElectronVolts operator *( Decimal left, ElectronVolts right ) => new ElectronVolts( left * right.Value );

		public static ElectronVolts operator *( BigRational left, ElectronVolts right ) {
			var res = left * right.Value;

			return new ElectronVolts( ( Decimal ) res );
		}

		public static ElectronVolts operator *( BigInteger left, ElectronVolts right ) {
			var res = new BigRational( left ) * new BigRational( right.Value );

			return new ElectronVolts( ( Decimal ) res );
		}

		public static ElectronVolts operator /( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value / right.Value );

		public static ElectronVolts operator /( ElectronVolts left, Decimal right ) => new ElectronVolts( left.Value / right );

		public static MegaElectronVolts operator +( ElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

		public static GigaElectronVolts operator +( ElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

		public static ElectronVolts operator +( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value + right.Value );

		public static Boolean operator <( ElectronVolts left, ElectronVolts right ) => left.Value < right.Value;

		public static Boolean operator <=( ElectronVolts left, ElectronVolts right ) => left.Value <= right.Value;

		public static Boolean operator >( ElectronVolts left, ElectronVolts right ) => left.Value > right.Value;

		public static Boolean operator >=( ElectronVolts left, ElectronVolts right ) => left.Value >= right.Value;

		[Test]
		public static void TestSomeElectronVolts() {
			Assert.Greater( UniversalConstants.ElementaryCharge.Value, UniversalConstants.ZeroElementaryCharge.Value );
			Assert.Greater( UniversalConstants.ElementaryCharge.Value, UniversalConstants.NegativeTwoThirdsElementaryCharge.Value );
			Assert.Greater( UniversalConstants.ZeroElementaryCharge.Value, UniversalConstants.NegativeTwoThirdsElementaryCharge.Value );
			Assert.Greater( UniversalConstants.PositiveTwoThirdsElementaryCharge.Value, UniversalConstants.NegativeTwoThirdsElementaryCharge.Value );
		}

		public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.Value );

		public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

		public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToElectronVolts().Value );

		public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * InOneElectronVolt );

		public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

		public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

		public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

		public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

		/// <summary>
		///     Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
		public override String ToString() => $"{this.Value} eV";

		public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );

	}

}