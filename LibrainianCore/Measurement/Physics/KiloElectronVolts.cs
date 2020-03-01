// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "KiloElectronVolts.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "KiloElectronVolts.cs" was last formatted by Protiguous on 2020/01/31 at 12:27 AM.

namespace LibrainianCore.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using LibrainianCore.Extensions;
    using Rationals;

    /// <summary>Units of mass and energy in ElectronVolts.</summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct KiloElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<KiloElectronVolts>, IComparable<MegaElectronVolts>,
        IComparable<GigaElectronVolts> {

        public const Decimal InOneElectronVolt = 1E-3m;

        public const Decimal InOneGigaElectronVolt = 1E6m;

        public const Decimal InOneKiloElectronVolt = 1E0m;

        public const Decimal InOneMegaElectronVolt = 1E3m;

        public const Decimal InOneMilliElectronVolt = 1E-6m;

        public const Decimal InOneTeraElectronVolt = 1E9m;

        /// <summary>About 79228162514264337593543950335.</summary>
        public static readonly KiloElectronVolts MaxValue = new KiloElectronVolts( Decimal.MaxValue );

        /// <summary>About -79228162514264337593543950335.</summary>
        public static readonly KiloElectronVolts MinValue = new KiloElectronVolts( Decimal.MinValue );

        public static readonly KiloElectronVolts NegativeOne = new KiloElectronVolts( -1m );

        /// <summary></summary>
        public static readonly KiloElectronVolts NegativeZero = new KiloElectronVolts( -Decimal.Zero );

        /// <summary></summary>
        public static readonly KiloElectronVolts One = new KiloElectronVolts( 1m );

        public static readonly KiloElectronVolts Zero = new KiloElectronVolts( 0m );

        public readonly Rational Value;

        public KiloElectronVolts( Decimal value ) : this() => this.Value = ( Rational )value;

        public KiloElectronVolts( MegaElectronVolts megaElectronVolts ) => this.Value = megaElectronVolts.ToKiloElectronVolts().Value;

        public KiloElectronVolts( Rational aBigFraction ) => this.Value = aBigFraction;

        public KiloElectronVolts( GigaElectronVolts gigaElectronVolts ) => this.Value = gigaElectronVolts.ToKiloElectronVolts().Value;

        public KiloElectronVolts( ElectronVolts electronVolts ) => this.Value = electronVolts.ToKiloElectronVolts().Value;

        public static implicit operator KiloElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToKiloElectronVolts();

        public static implicit operator KiloElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToKiloElectronVolts();

        public static KiloElectronVolts operator -( KiloElectronVolts electronVolts ) => new KiloElectronVolts( -electronVolts.Value );

        public static KiloElectronVolts operator *( KiloElectronVolts left, KiloElectronVolts right ) => new KiloElectronVolts( left.Value * right.Value );

        public static KiloElectronVolts operator *( KiloElectronVolts left, Decimal right ) => new KiloElectronVolts( left.Value * ( Rational )right );

        public static KiloElectronVolts operator *( Decimal left, KiloElectronVolts right ) => new KiloElectronVolts( ( Rational )left * right.Value );

        public static KiloElectronVolts operator *( Rational left, KiloElectronVolts right ) => new KiloElectronVolts( left * right.Value );

        public static KiloElectronVolts operator *( BigInteger left, KiloElectronVolts right ) => new KiloElectronVolts( new Rational( left ) * right.Value );

        public static KiloElectronVolts operator /( KiloElectronVolts left, KiloElectronVolts right ) => new KiloElectronVolts( left.Value / right.Value );

        public static KiloElectronVolts operator /( KiloElectronVolts left, Decimal right ) => new KiloElectronVolts( left.Value / ( Rational )right );

        public static MegaElectronVolts operator +( KiloElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

        public static GigaElectronVolts operator +( KiloElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

        public static KiloElectronVolts operator +( KiloElectronVolts left, KiloElectronVolts right ) => new KiloElectronVolts( left.Value + right.Value );

        public static Boolean operator <( KiloElectronVolts left, KiloElectronVolts right ) => left.Value < right.Value;

        public static Boolean operator >( KiloElectronVolts left, KiloElectronVolts right ) => left.Value > right.Value;

        public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.ToKiloElectronVolts().Value );

        public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( KiloElectronVolts other ) => this.Value.CompareTo( other.Value );

        public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToKiloElectronVolts().Value );

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * ( Rational )InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * ( Rational )InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * ( Rational )InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * ( Rational )InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * ( Rational )InOneMilliElectronVolt );

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A <see cref="String" /> containing a fully qualified type name.</returns>
        [NotNull]
        public override String ToString() => $"{this.Value} eV";

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * ( Rational )InOneTeraElectronVolt );
    }
}