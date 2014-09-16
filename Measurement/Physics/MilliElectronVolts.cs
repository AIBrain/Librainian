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
// "Librainian/MilliElectronVolts.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Physics {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Annotations;
    using Librainian.Extensions;

    /// <summary>
    ///     Units of mass and energy in Thousandths of <see cref="ElectronVolts" />.
    /// </summary>
    /// <seealso cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <seealso cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <seealso cref="http://wikipedia.org/wiki/Milli-" />
    /// <seealso cref="http://wikipedia.org/wiki/Electronvolt" />
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct MilliElectronVolts : IComparable< MilliElectronVolts >, IComparable< ElectronVolts >, IComparable< MegaElectronVolts >, IComparable< GigaElectronVolts > {
        private const  Decimal InOneElectronVolt = 1E3m;

        private const  Decimal InOneGigaElectronVolt = 1E12m;

        private const  Decimal InOneKiloElectronVolt = 1E6m;

        private const  Decimal InOneMegaElectronVolt = 1E9m;

        private const  Decimal InOneMilliElectronVolt = 1E0m;

        private const  Decimal InOneTeraElectronVolt = 1E15m;

        /// <summary>
        ///     About 79228162514264337593543950335.
        /// </summary>
        public static readonly MilliElectronVolts MaxValue = new MilliElectronVolts(Decimal.MaxValue );

        /// <summary>
        ///     About -79228162514264337593543950335.
        /// </summary>
        public static readonly MilliElectronVolts MinValue = new MilliElectronVolts(Decimal.MinValue );

        public static readonly MilliElectronVolts Zero = new MilliElectronVolts( 0 );
        public readonly  Decimal Value;

        public MilliElectronVolts(Decimal units ) : this() {
            this.Value = units;
        }

        [UsedImplicitly]
        private String DebuggerDisplay { get { return this.Display(); } }

        public int CompareTo( ElectronVolts other ) {
            return this.ToElectronVolts().Value.CompareTo( other.Value );
        }

        public int CompareTo( GigaElectronVolts other ) {
            return this.ToGigaElectronVolts().Value.CompareTo( other.Value );
        }

        public int CompareTo( MegaElectronVolts other ) {
            return this.ToMegaElectronVolts().Value.CompareTo( other.Value );
        }

        public int CompareTo( MilliElectronVolts other ) {
            return this.Value.CompareTo( other.Value );
        }

        public static implicit operator ElectronVolts( MilliElectronVolts milliElectronVolts ) {
            return milliElectronVolts.ToElectronVolts();
        }

        public static Boolean operator <( MilliElectronVolts left, MilliElectronVolts right ) {
            return left.Value.CompareTo( right.Value ) < 0;
        }

        public static Boolean operator >( MilliElectronVolts left, MilliElectronVolts right ) {
            return left.Value.CompareTo( right.Value ) > 0;
        }

        public int CompareTo( KiloElectronVolts other ) {
            return this.ToKiloElectronVolts().Value.CompareTo( other.Value );
        }

        public String Display() {
            return String.Format( "{0} meV", this.Value );
        }

        public String Simpler() {
            var list = new HashSet< String > {
                                                 this.ToTeraElectronVolts().Display(),
                                                 this.ToGigaElectronVolts().Display(),
                                                 this.ToMegaElectronVolts().Display(),
                                                 this.ToElectronVolts().Display(),
                                                 this.ToMilliElectronVolts().Display()
                                             };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public ElectronVolts ToElectronVolts() {
            return new ElectronVolts( this.Value*InOneElectronVolt );
        }

        public GigaElectronVolts ToGigaElectronVolts() {
            return new GigaElectronVolts( this.Value*InOneGigaElectronVolt );
        }

        public KiloElectronVolts ToKiloElectronVolts() {
            return new KiloElectronVolts( this.Value*InOneKiloElectronVolt );
        }

        public MegaElectronVolts ToMegaElectronVolts() {
            return new MegaElectronVolts( this.Value*InOneMegaElectronVolt );
        }

        public MilliElectronVolts ToMilliElectronVolts() {
            return new MilliElectronVolts( this.Value*InOneMilliElectronVolt );
        }

        public TeraElectronVolts ToTeraElectronVolts() {
            return new TeraElectronVolts( this.Value*InOneTeraElectronVolt );
        }
    }
}
