#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/ElectronVolts.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Physics {
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using JetBrains.Annotations;
    using Librainian.Extensions;
    using Maths;
    using Numerics;
    using NUnit.Framework;

	/// <summary>
	///     Units of mass and energy in ElectronVolts.
	/// </summary>
	/// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
	/// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
	// ReSharper disable once UseNameofExpression
	[DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct ElectronVolts : IComparable< MilliElectronVolts >, IComparable< ElectronVolts >, IComparable< MegaElectronVolts >, IComparable< GigaElectronVolts > {
        private const  Decimal InOneElectronVolt = 1E0m;

        private const  Decimal InOneGigaElectronVolt = 1E9m;

        private const  Decimal InOneKiloElectronVolt = 1E3m;

        private const  Decimal InOneMegaElectronVolt = 1E6m;

        private const  Decimal InOneMilliElectronVolt = 1E-3m;

        private const  Decimal InOneTeraElectronVolt = 1E12m;

        /// <summary>
        ///     About 79228162514264337593543950335.
        /// </summary>
        public static readonly ElectronVolts MaxValue = new ElectronVolts(Decimal.MaxValue );

        /// <summary>
        ///     About -79228162514264337593543950335.
        /// </summary>
        public static readonly ElectronVolts MinValue = new ElectronVolts(Decimal.MinValue );

        public static readonly ElectronVolts NegativeOne = new ElectronVolts( -1 );


        /// <summary>
        /// </summary>
        public static readonly ElectronVolts NegativeZero = new ElectronVolts( -Decimal.Zero );

        /// <summary>
        /// </summary>
        public static readonly ElectronVolts One = new ElectronVolts( 1 );

        public static readonly ElectronVolts Zero = new ElectronVolts( 0 );

        /// <summary>
        /// More than nothing (unknown but not massless).
        /// </summary>
        public static readonly ElectronVolts NonZero = new ElectronVolts( MathExtensions.EpsilonDecimal );


        public readonly  Decimal Value;

        static ElectronVolts() {
            Assert.Greater( UniversalConstants.ElementaryCharge.Value, UniversalConstants.ZeroElementaryCharge.Value );
            Assert.Greater( UniversalConstants.ElementaryCharge.Value, UniversalConstants.NegativeTwoThirdsElementaryCharge.Value );
            Assert.Greater( UniversalConstants.ZeroElementaryCharge.Value, UniversalConstants.NegativeTwoThirdsElementaryCharge.Value );
            Assert.Greater( UniversalConstants.PositiveTwoThirdsElementaryCharge.Value, UniversalConstants.NegativeTwoThirdsElementaryCharge.Value );
        }

        public ElectronVolts(Decimal value ) : this() {
            this.Value = value;
        }

        public ElectronVolts( MegaElectronVolts megaElectronVolts ) {
            this.Value = megaElectronVolts.ToElectronVolts().Value;
        }

        public ElectronVolts( BigRational aBigFraction ) {
            this.Value = (Decimal ) aBigFraction;
        }

        public ElectronVolts( GigaElectronVolts gigaElectronVolts ) {
            this.Value = gigaElectronVolts.ToElectronVolts().Value;
        }

        [UsedImplicitly]
        private String DebuggerDisplay => this.Display();


        public int CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.Value );

        public int CompareTo( GigaElectronVolts other ) => this.ToGigaElectronVolts().Value.CompareTo( other.Value );

        public int CompareTo( MegaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

        public int CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToElectronVolts().Value );

        public static implicit operator ElectronVolts( MegaElectronVolts megaElectronVolts ) => megaElectronVolts.ToElectronVolts();

        public static implicit operator ElectronVolts( GigaElectronVolts gigaElectronVolts ) => gigaElectronVolts.ToElectronVolts();

        public static ElectronVolts operator -( ElectronVolts electronVolts ) => new ElectronVolts( -electronVolts.Value );

        public static ElectronVolts operator *( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value*right.Value );

        public static ElectronVolts operator *( ElectronVolts left,Decimal right ) => new ElectronVolts( left.Value*right );

        public static ElectronVolts operator *(Decimal left, ElectronVolts right ) => new ElectronVolts( left*right.Value );

        public static ElectronVolts operator *( BigDecimal left, ElectronVolts right ) {
            var res = left*right.Value;
            return new ElectronVolts( (Decimal ) res );
        }

        public static ElectronVolts operator *( BigInteger left, ElectronVolts right ) {
            var lhs = new BigDecimal( left, 0 );
            var rhs = new BigDecimal( right.Value );
            var res = lhs*rhs;
            return new ElectronVolts( (Decimal ) res );
        }

        public static ElectronVolts operator /( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value/right.Value );

        public static ElectronVolts operator /( ElectronVolts left,Decimal right ) => new ElectronVolts( left.Value/right );

        public static MegaElectronVolts operator +( ElectronVolts left, MegaElectronVolts right ) => left.ToMegaElectronVolts() + right;

        public static GigaElectronVolts operator +( ElectronVolts left, GigaElectronVolts right ) => left.ToGigaElectronVolts() + right;

        public static ElectronVolts operator +( ElectronVolts left, ElectronVolts right ) => new ElectronVolts( left.Value + right.Value );

        public static Boolean operator <( ElectronVolts left, ElectronVolts right ) => left.Value < right.Value;

        public static Boolean operator >( ElectronVolts left, ElectronVolts right ) => left.Value > right.Value;

        public static Boolean operator <=( ElectronVolts left, ElectronVolts right ) => left.Value <= right.Value;

        public static Boolean operator >=( ElectronVolts left, ElectronVolts right ) => left.Value >= right.Value;

        public String Display() => String.Format( "{0} eV", this.Value );

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value*InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value*InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value*InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value*InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value*InOneMilliElectronVolt );

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value*InOneTeraElectronVolt );
    }
}
