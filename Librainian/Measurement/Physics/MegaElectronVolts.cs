// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MegaElectronVolts.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "MegaElectronVolts.cs" was last formatted by Protiguous on 2019/08/08 at 8:49 AM.

namespace Librainian.Measurement.Physics {

    using System;
    using System.Diagnostics;
    using Librainian.Extensions;
    using Rationals;

    /// <summary>Units of mass and energy in ElectronVolts.</summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <see cref="http://wikipedia.org/wiki/Mega-" />
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Immutable]
    public struct MegaElectronVolts : IComparable<MilliElectronVolts>, IComparable<ElectronVolts>, IComparable<MegaElectronVolts>, IComparable<GigaElectronVolts> {

        public const Decimal InOneElectronVolt = 1E-6m;

        public const Decimal InOneGigaElectronVolt = 1E3m;

        public const Decimal InOneKiloElectronVolt = 1E-3m;

        public const Decimal InOneMegaElectronVolt = 1E0m;

        public const Decimal InOneMilliElectronVolt = 1E-9m;

        public const Decimal InOneTeraElectronVolt = 1E6m;

        public static readonly MegaElectronVolts One = new MegaElectronVolts( ( Rational ) 1 );

        /// <summary></summary>
        public static readonly MegaElectronVolts Zero = new MegaElectronVolts( ( Rational ) 0 );

        public readonly Rational Value;

        public MegaElectronVolts( Rational units ) : this() => this.Value = units;

        public MegaElectronVolts( Decimal units ) : this() => this.Value = ( Rational ) units;

        public MegaElectronVolts( Double units ) : this() => this.Value = ( Rational ) units;

        public MegaElectronVolts( GigaElectronVolts gigaElectronVolts ) => this.Value = gigaElectronVolts.ToMegaElectronVolts().Value;

        public MegaElectronVolts( KiloElectronVolts kiloElectronVolts ) => this.Value = kiloElectronVolts.ToMegaElectronVolts().Value;

        public static MegaElectronVolts operator +( MegaElectronVolts left, MegaElectronVolts right ) => new MegaElectronVolts( left.Value + right.Value );

        public static GigaElectronVolts operator +( MegaElectronVolts megaElectronVolts, GigaElectronVolts gigaElectronVolts ) =>
            megaElectronVolts.ToGigaElectronVolts() + gigaElectronVolts;

        public static Boolean operator <( MegaElectronVolts left, MegaElectronVolts right ) => left.Value.CompareTo( right.Value ) < 0;

        public static Boolean operator >( MegaElectronVolts left, MegaElectronVolts right ) => left.Value.CompareTo( right.Value ) > 0;

        public Int32 CompareTo( ElectronVolts other ) => this.Value.CompareTo( other.ToMegaElectronVolts().Value );

        public Int32 CompareTo( GigaElectronVolts other ) => this.ToMegaElectronVolts().Value.CompareTo( other.Value );

        public Int32 CompareTo( MegaElectronVolts other ) => this.Value.CompareTo( other.Value );

        public Int32 CompareTo( MilliElectronVolts other ) => this.Value.CompareTo( other.ToMegaElectronVolts().Value );

        public ElectronVolts ToElectronVolts() => new ElectronVolts( this.Value * ( Rational ) InOneElectronVolt );

        public GigaElectronVolts ToGigaElectronVolts() => new GigaElectronVolts( this.Value * ( Rational ) InOneGigaElectronVolt );

        public KiloElectronVolts ToKiloElectronVolts() => new KiloElectronVolts( this.Value * ( Rational ) InOneKiloElectronVolt );

        public MegaElectronVolts ToMegaElectronVolts() => new MegaElectronVolts( this.Value * ( Rational ) InOneMegaElectronVolt );

        public MilliElectronVolts ToMilliElectronVolts() => new MilliElectronVolts( this.Value * ( Rational ) InOneMilliElectronVolt );

        public override String ToString() => $"{this.Value} MeV";

        public TeraElectronVolts ToTeraElectronVolts() => new TeraElectronVolts( this.Value * ( Rational ) InOneTeraElectronVolt );
    }
}