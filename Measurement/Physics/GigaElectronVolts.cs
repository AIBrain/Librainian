// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/GigaElectronVolts.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using Librainian.Extensions;

    /// <summary>Units of mass and energy in <see cref="GigaElectronVolts" />.</summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <see cref="http://wikipedia.org/wiki/Giga-" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct GigaElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {
        public const Decimal InOneElectronVolt = 1E-9m;

        public const Decimal InOneGigaElectronVolt = 1E0m;

        public const Decimal InOneKiloElectronVolt = 1E-6m;

        public const Decimal InOneMegaElectronVolt = 1E-3m;

        public const Decimal InOneMilliElectronVolt = 1E-12m;

        public const Decimal InOneTeraElectronVolt = 1E3m;

        /// <summary>About 79228162514264337593543950335.</summary>
        public static readonly GigaElectronVolts MaxValue = new GigaElectronVolts( Decimal.MaxValue );

        /// <summary>About -79228162514264337593543950335.</summary>
        public static readonly GigaElectronVolts MinValue = new GigaElectronVolts( Decimal.MinValue );

        /// <summary></summary>
        public static readonly GigaElectronVolts One = new GigaElectronVolts( 1 );

        /// <summary></summary>
        public static readonly GigaElectronVolts Zero = new GigaElectronVolts( 0 );

        /// <summary></summary>
        public readonly Decimal Value;

        public GigaElectronVolts( Decimal units ) : this() => this.Value = units;

	    public GigaElectronVolts( MegaElectronVolts megaElectronVolts ) : this() => this.Value = megaElectronVolts.ToGigaElectronVolts().Value;

	    public static GigaElectronVolts operator *( GigaElectronVolts left, Decimal right ) => new GigaElectronVolts( left.Value * right );

        public static GigaElectronVolts operator *( Decimal left, GigaElectronVolts right ) => new GigaElectronVolts( left * right.Value );

        public static GigaElectronVolts operator +( MegaElectronVolts left, GigaElectronVolts right ) => new GigaElectronVolts( left ) + right;

        public static GigaElectronVolts operator +( GigaElectronVolts left, GigaElectronVolts right ) => new GigaElectronVolts( left.Value + right.Value );

        public static Boolean operator <( GigaElectronVolts left, GigaElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

        public static Boolean operator >( GigaElectronVolts left, GigaElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

        public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.ToGigaElectronVolts().Value );

        public Int32 CompareTo( GigaElectronVolts other ) => this.Value.CompareTo( other.Value );

        public Int32 CompareTo( MegaElectronVolts other ) => this.Value.CompareTo( other.ToGigaElectronVolts().Value );

        public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToGigaElectronVolts().Value );

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
        public override String ToString() => $"{this.Value} GeV";

	    public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * InOneTeraElectronVolt );
    }
}