// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ElectronVolts.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "LibrainianCore", File: "ElectronVolts.cs" was last formatted by Protiguous on 2020/03/16 at 3:07 PM.

namespace Librainian.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Librainian.Extensions;
    using Maths;
    using Rationals;

    /// <summary>Units of mass and energy in ElectronVolts.</summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct ElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {

        public static class InOne {

            public static readonly Rational ElectronVolt = ( Rational ) 1E0m;

            public static readonly Rational GigaElectronVolt = ( Rational ) 1E9m;

            public static readonly Rational KiloElectronVolt = ( Rational ) 1E3m;

            public static readonly Rational MegaElectronVolt = ( Rational ) 1E6m;

            public static readonly Rational MilliElectronVolt = ( Rational ) 1E-3m;

            public static readonly Rational TeraElectronVolt = ( Rational ) 1E12m;

        }

        /// <summary>About 79228162514264337593543950335.</summary>
        public static readonly ElectronVolts MaxValue = new ElectronVolts( value: Decimal.MaxValue );

        /// <summary>About -79228162514264337593543950335.</summary>
        public static readonly ElectronVolts MinValue = new ElectronVolts( value: Decimal.MinValue );

        public static readonly ElectronVolts NegativeOne = new ElectronVolts( value: -1m );

        /// <summary></summary>
        public static readonly ElectronVolts NegativeZero = new ElectronVolts( value: -Decimal.Zero );

        /// <summary>More than nothing (unknown but not massless).</summary>
        public static readonly ElectronVolts NonZero = new ElectronVolts( value: MathExtensions.EpsilonDecimal );

        /// <summary></summary>
        public static readonly ElectronVolts One = new ElectronVolts( value: 1m );

        public static readonly ElectronVolts Zero = new ElectronVolts( value: 0m );

        public ElectronVolts( Decimal value ) : this() => this.Value = ( Rational ) value;

        public Rational Value { get; }

        public ElectronVolts( Double value ) : this() => this.Value = ( Rational ) value;

        public ElectronVolts( Rational value ) : this() => this.Value = value;

        public ElectronVolts( MegaElectronVolts megaElectronVolts ) => this.Value = megaElectronVolts.ToElectronVolts().Value;

        public ElectronVolts( GigaElectronVolts gigaElectronVolts ) => this.Value = gigaElectronVolts.ToElectronVolts().Value;

        public static implicit operator ElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToElectronVolts();

        public static implicit operator ElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToElectronVolts();

        public static ElectronVolts operator -( ElectronVolts electronVolts ) => new ElectronVolts( value: -electronVolts.Value );

        public static ElectronVolts operator *( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( value: left.Value * right.Value );

        public static ElectronVolts operator *( ElectronVolts left, Decimal right ) => new ElectronVolts( value: left.Value * ( Rational ) right );

        public static ElectronVolts operator *( Decimal left, ElectronVolts right ) => new ElectronVolts( value: ( Rational ) left * right.Value );

        public static ElectronVolts operator *( Rational left, ElectronVolts right ) => new ElectronVolts( value: left * right.Value );

        public static ElectronVolts operator /( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( value: left.Value / right.Value );

        public static ElectronVolts operator /( ElectronVolts left, Decimal right ) => new ElectronVolts( value: left.Value / ( Rational ) right );

        public static MegaElectronVolts operator +( ElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

        public static GigaElectronVolts operator +( ElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

        public static ElectronVolts operator +( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( value: left.Value + right.Value );

        public static Boolean operator <( ElectronVolts left, ElectronVolts right ) => left.Value < right.Value;

        public static Boolean operator <=( ElectronVolts left, ElectronVolts right ) => left.Value <= right.Value;

        public static Boolean operator >( ElectronVolts left, ElectronVolts right ) => left.Value > right.Value;

        public static Boolean operator >=( ElectronVolts left, ElectronVolts right ) => left.Value >= right.Value;

        public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other: other.Value );

        public Int32 CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other: other.Value );

        public Int32 CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other: other.Value );

        public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other: other.ToElectronVolts().Value );

        public ElectronVolts ToElectronVolts() => new ElectronVolts( value: this.Value * InOne.ElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( units: this.Value * InOne.GigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( aBigFraction: this.Value * InOne.KiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( units: this.Value * InOne.MegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( units: this.Value * InOne.MilliElectronVolt );

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>A <see cref="String" /> containing a fully qualified type name.</returns>
        [NotNull]
        public override String ToString() => $"{this.Value} eV";

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( units: this.Value * InOne.TeraElectronVolt );

    }

}