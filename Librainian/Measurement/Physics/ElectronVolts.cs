// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ElectronVolts.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ElectronVolts.cs" was last formatted by Protiguous on 2018/07/13 at 1:23 AM.

namespace Librainian.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using Librainian.Extensions;
    using Maths;
    using Rationals;

    /// <summary>
    ///     Units of mass and energy in ElectronVolts.
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
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

        public static readonly ElectronVolts NegativeOne = new ElectronVolts( -1m );

        /// <summary>
        /// </summary>
        public static readonly ElectronVolts NegativeZero = new ElectronVolts( -Decimal.Zero );

        /// <summary>
        ///     More than nothing (unknown but not massless).
        /// </summary>
        public static readonly ElectronVolts NonZero = new ElectronVolts( MathExtensions.EpsilonDecimal );

        /// <summary>
        /// </summary>
        public static readonly ElectronVolts One = new ElectronVolts( 1m );

        public static readonly ElectronVolts Zero = new ElectronVolts( 0m );

        public readonly Rational Value;

        public ElectronVolts( Decimal value ) : this() => this.Value = ( Rational )value;

        public ElectronVolts( Double value ) : this() => this.Value = ( Rational )value;

        public ElectronVolts( Rational value ) : this() => this.Value = value;

        public ElectronVolts( MegaElectronVolts megaElectronVolts ) => this.Value = megaElectronVolts.ToElectronVolts().Value;

        public ElectronVolts( GigaElectronVolts gigaElectronVolts ) => this.Value = gigaElectronVolts.ToElectronVolts().Value;

        public static implicit operator ElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToElectronVolts();

        public static implicit operator ElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToElectronVolts();

        public static ElectronVolts operator -( ElectronVolts electronVolts ) => new ElectronVolts( -electronVolts.Value );

        public static ElectronVolts operator *( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value * right.Value );

        public static ElectronVolts operator *( ElectronVolts left, Decimal right ) => new ElectronVolts( left.Value * ( Rational )right );

        public static ElectronVolts operator *( Decimal left, ElectronVolts right ) => new ElectronVolts( ( Rational )left * right.Value );

        public static ElectronVolts operator *( Rational left, ElectronVolts right ) => new ElectronVolts( left * right.Value );

        public static ElectronVolts operator /( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value / right.Value );

        public static ElectronVolts operator /( ElectronVolts left, Decimal right ) => new ElectronVolts( left.Value / ( Rational )right );

        public static MegaElectronVolts operator +( ElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

        public static GigaElectronVolts operator +( ElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

        public static ElectronVolts operator +( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value + right.Value );

        public static Boolean operator <( ElectronVolts left, ElectronVolts right ) => left.Value < right.Value;

        public static Boolean operator <=( ElectronVolts left, ElectronVolts right ) => left.Value <= right.Value;

        public static Boolean operator >( ElectronVolts left, ElectronVolts right ) => left.Value > right.Value;

        public static Boolean operator >=( ElectronVolts left, ElectronVolts right ) => left.Value >= right.Value;

        public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.Value );

        public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToElectronVolts().Value );

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * ( Rational )InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * ( Rational )InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * ( Rational )InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * ( Rational )InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * ( Rational )InOneMilliElectronVolt );

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>A <see cref="T:System.String" /> containing a fully qualified type name.</returns>
        public override String ToString() => $"{this.Value} eV";

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * ( Rational )InOneTeraElectronVolt );
    }
}