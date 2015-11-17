// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/AtomicMassUnits.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

namespace Librainian.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Librainian.Extensions;
    using Numerics;

    /// <summary>Units of mass and energy in ElectronVolts.</summary>
    /// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <seealso cref="http://www.wolframalpha.com/input/?i=1+unified+atomic+mass+units+convert+to+electronvolts" />
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct AtomicMassUnits : IComparable<ElectronVolts>, IComparable<AtomicMassUnits> {

        /// <summary>About 79228162514264337593543950335.</summary>
        public static readonly AtomicMassUnits MaxValue = new AtomicMassUnits( Decimal.MaxValue );

        /// <summary>About -79228162514264337593543950335.</summary>
        public static readonly AtomicMassUnits MinValue = new AtomicMassUnits( Decimal.MinValue );

        public static readonly AtomicMassUnits NegativeOne = new AtomicMassUnits( -1 );

        /// <summary></summary>
        public static readonly AtomicMassUnits NegativeZero = new AtomicMassUnits( -Decimal.Zero );

        /// <summary></summary>
        public static readonly AtomicMassUnits One = new AtomicMassUnits( 1 );

        public static readonly ElectronVolts OneAtomicUnitEqualsElectronVolt = new MegaElectronVolts( 931.494m );

        public static readonly AtomicMassUnits OneElectronVoltEqualsAtomicMassUnits = new AtomicMassUnits( 0.000000001073544m );

        public static readonly AtomicMassUnits Zero = new AtomicMassUnits( 0 );

        /// <summary></summary>
        public readonly Decimal Value;

        /// <summary></summary>
        private const Decimal InOneElectronVolt = 0.000000001073544m;

        private const Decimal InOneGigaElectronVolt = 1.073544m;
        private const Decimal InOneKiloElectronVolt = 0.000001073544m;
        private const Decimal InOneMegaElectronVolt = 0.001073544m;
        private const Decimal InOneMilliElectronVolt = 0.000000000001073544m;
        private const Decimal InOneTeraElectronVolt = 1073.544m;

        [UsedImplicitly]
        private String DebuggerDisplay => this.Display();

        public AtomicMassUnits(Decimal value) : this() {
            this.Value = value;
        }

        public AtomicMassUnits(BigRational aBigFraction) {
            this.Value = ( Decimal )aBigFraction;
        }

        public static AtomicMassUnits operator -(AtomicMassUnits electronVolts) => new AtomicMassUnits( -electronVolts.Value );

        //public static implicit operator AtomicMassUnits( GigaElectronVolts gigaElectronVolts ) {
        //    return gigaElectronVolts.ToElectronVolts();
        //}
        public static AtomicMassUnits operator *(AtomicMassUnits left, AtomicMassUnits right) => new AtomicMassUnits( left.Value * right.Value );

        //public static implicit operator AtomicMassUnits( MegaElectronVolts megaElectronVolts ) {
        //    return megaElectronVolts.ToElectronVolts();
        //}
        public static AtomicMassUnits operator *(AtomicMassUnits left, Decimal right) => new AtomicMassUnits( left.Value * right );

        public static AtomicMassUnits operator *(Decimal left, AtomicMassUnits right) => new AtomicMassUnits( left * right.Value );

        public static AtomicMassUnits operator *( BigRational left, AtomicMassUnits right) {
            var res = left * right.Value;
            return new AtomicMassUnits( ( Decimal )res );
        }

        public static AtomicMassUnits operator *(BigInteger left, AtomicMassUnits right) {
            var lhs = new BigRational( left );
            var rhs = new BigRational( right.Value );
            var res = lhs * rhs;
            return new AtomicMassUnits( ( Decimal )res );
        }

        public static AtomicMassUnits operator /(AtomicMassUnits left, AtomicMassUnits right) => new AtomicMassUnits( left.Value / right.Value );

        public static AtomicMassUnits operator /(AtomicMassUnits left, Decimal right) => new AtomicMassUnits( left.Value / right );

        public static MegaElectronVolts operator +(AtomicMassUnits left, MegaElectronVolts right) => left.ToMegaElectronVolts() + right;

        public static GigaElectronVolts operator +(AtomicMassUnits left, GigaElectronVolts right) => left.ToGigaElectronVolts() + right;

        public static AtomicMassUnits operator +(AtomicMassUnits left, AtomicMassUnits right) => new AtomicMassUnits( left.Value + right.Value );

        public static Boolean operator <(AtomicMassUnits left, AtomicMassUnits right) => left.Value < right.Value;

        public static Boolean operator >(AtomicMassUnits left, AtomicMassUnits right) => left.Value > right.Value;

        [Pure]
        public Int32 CompareTo(AtomicMassUnits other) => this.Value.CompareTo( other.Value );

        [Pure]
        public Int32 CompareTo(ElectronVolts other) => this.ToElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo(TeraElectronVolts other) => this.ToTeraElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo(GigaElectronVolts other) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo(MegaElectronVolts other) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo(KiloElectronVolts other) => this.ToKiloElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo(MilliElectronVolts other) => this.ToMilliElectronVolts().Value.CompareTo( other.Value );

        public String Display() => $"{this.Value} u";

        public AtomicMassUnits ToElectronVolts() => new AtomicMassUnits( this.Value * InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * InOneMilliElectronVolt );

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );
    }
}