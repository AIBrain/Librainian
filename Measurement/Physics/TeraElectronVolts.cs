// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "TeraElectronVolts.cs" belongs to Rick@AIBrain.org and
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
// File "TeraElectronVolts.cs" was last formatted by Protiguous on 2018/06/04 at 4:11 PM.

namespace Librainian.Measurement.Physics {

	using System;
	using System.Diagnostics;
	using Librainian.Extensions;

	/// <summary>Units of mass and energy in <see cref="TeraElectronVolts" />.</summary>
	/// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
	/// <see cref="http://wikipedia.org/wiki/SI_prefix" />
	/// <see cref="http://wikipedia.org/wiki/Giga-" />
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Immutable]
	public struct TeraElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<TeraElectronVolts> {

		public const Decimal InOneElectronVolt = 1E-12m;

		public const Decimal InOneGigaElectronVolt = 1E-3m;

		public const Decimal InOneKiloElectronVolt = 1E-9m;

		public const Decimal InOneMegaElectronVolt = 1E-6m;

		public const Decimal InOneMilliElectronVolt = 1E-15m;

		public const Decimal InOneTeraElectronVolt = 1E0m;

		/// <summary>About 79228162514264337593543950335.</summary>
		public static readonly TeraElectronVolts MaxValue = new TeraElectronVolts( Decimal.MaxValue );

		/// <summary>About -79228162514264337593543950335.</summary>
		public static readonly TeraElectronVolts MinValue = new TeraElectronVolts( Decimal.MinValue );

		/// <summary></summary>
		public static readonly TeraElectronVolts One = new TeraElectronVolts( 1 );

		/// <summary></summary>
		public static readonly TeraElectronVolts Zero = new TeraElectronVolts( 0 );

		/// <summary></summary>
		public readonly Decimal Value;

		public TeraElectronVolts( Decimal units ) : this() => this.Value = units;

		public TeraElectronVolts( GigaElectronVolts gigaElectronVolts ) : this() => this.Value = gigaElectronVolts.ToTeraElectronVolts().Value;

		public TeraElectronVolts( MegaElectronVolts megaElectronVolts ) : this() => this.Value = megaElectronVolts.ToTeraElectronVolts().Value;

		public static TeraElectronVolts operator *( TeraElectronVolts left, Decimal right ) => new TeraElectronVolts( left.Value * right );

		public static TeraElectronVolts operator *( Decimal left, TeraElectronVolts right ) => new TeraElectronVolts( left * right.Value );

		public static TeraElectronVolts operator +( GigaElectronVolts left, TeraElectronVolts right ) => new TeraElectronVolts( left.ToTeraElectronVolts().Value + right.Value );

		public static TeraElectronVolts operator +( TeraElectronVolts left, TeraElectronVolts right ) => new TeraElectronVolts( left.Value + right.Value );

		public static Boolean operator <( TeraElectronVolts left, TeraElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

		public static Boolean operator >( TeraElectronVolts left, TeraElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

		public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.ToTeraElectronVolts().Value );

		public Int32 CompareTo( MegaElectronVolts other ) => this.Value.CompareTo( other.ToTeraElectronVolts().Value );

		public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToTeraElectronVolts().Value );

		public Int32 CompareTo( TeraElectronVolts other ) => this.Value.CompareTo( other.Value );

		public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * InOneElectronVolt );

		public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

		public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

		public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

		public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

		public override String ToString() => $"{this.Value} TeV";

		public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );

	}

}