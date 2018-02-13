// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/KiloElectronVolts.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using Librainian.Extensions;
    using Numerics;

    /// <summary>Units of mass and energy in ElectronVolts.</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct KiloElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<KiloElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {
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

        public static readonly KiloElectronVolts NegativeOne = new KiloElectronVolts( -1 );

        /// <summary></summary>
        public static readonly KiloElectronVolts NegativeZero = new KiloElectronVolts( -Decimal.Zero );

        /// <summary></summary>
        public static readonly KiloElectronVolts One = new KiloElectronVolts( 1 );

        public static readonly KiloElectronVolts Zero = new KiloElectronVolts( 0 );

        public readonly Decimal Value;

        public KiloElectronVolts( Decimal value ) : this() => this.Value = value;

	    public KiloElectronVolts( MegaElectronVolts megaElectronVolts ) => this.Value = megaElectronVolts.ToKiloElectronVolts().Value;

	    public KiloElectronVolts( BigRational aBigFraction ) => this.Value = ( Decimal )aBigFraction;

	    public KiloElectronVolts( GigaElectronVolts gigaElectronVolts ) => this.Value = gigaElectronVolts.ToKiloElectronVolts().Value;

	    public KiloElectronVolts( ElectronVolts electronVolts ) => this.Value = electronVolts.ToKiloElectronVolts().Value;

	    public static implicit operator KiloElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToKiloElectronVolts();

        public static implicit operator KiloElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToKiloElectronVolts();

        public static KiloElectronVolts operator -( KiloElectronVolts electronVolts ) => new KiloElectronVolts( -electronVolts.Value );

        public static KiloElectronVolts operator *( KiloElectronVolts left, KiloElectronVolts right ) => new KiloElectronVolts( left.Value * right.Value );

        public static KiloElectronVolts operator *( KiloElectronVolts left, Decimal right ) => new KiloElectronVolts( left.Value * right );

        public static KiloElectronVolts operator *( Decimal left, KiloElectronVolts right ) => new KiloElectronVolts( left * right.Value );

        public static KiloElectronVolts operator *( BigRational left, KiloElectronVolts right ) {
            var res = left * right.Value;
            return new KiloElectronVolts( ( Decimal )res );
        }

        public static KiloElectronVolts operator *( BigInteger left, KiloElectronVolts right ) {
            var lhs = new BigRational( left );
            var rhs = new BigRational( right.Value );
            var res = lhs * rhs;
            return new KiloElectronVolts( ( Decimal )res );
        }

        public static KiloElectronVolts operator /( KiloElectronVolts left, KiloElectronVolts right ) => new KiloElectronVolts( left.Value / right.Value );

        public static KiloElectronVolts operator /( KiloElectronVolts left, Decimal right ) => new KiloElectronVolts( left.Value / right );

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

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override String ToString() => $"{this.Value} eV";

	    public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );
    }
}