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
// "Librainian2/GigaElectronVolts.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

namespace Librainian.Measurement.Physics {
    using System;
    using System.Diagnostics;
    using Annotations;
    using Librainian.Extensions;

    /// <summary>
    ///     Units of mass and energy in <see cref="GigaElectronVolts" />.
    /// </summary>
    /// <see cref="http://wikipedia.org/wiki/Electronvolt#As_a_unit_of_mass" />
    /// <see cref="http://wikipedia.org/wiki/SI_prefix" />
    /// <see cref="http://wikipedia.org/wiki/Giga-" />
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    [Immutable]
    public struct GigaElectronVolts : IComparable< MilliElectronVolts >, IComparable< ElectronVolts >, IComparable< MegaElectronVolts >, IComparable< GigaElectronVolts > {
        private const Decimal InOneElectronVolt = 1E-9m;

        private const Decimal InOneGigaElectronVolt = 1E0m;

        private const Decimal InOneKiloElectronVolt = 1E-6m;

        private const Decimal InOneMegaElectronVolt = 1E-3m;

        private const Decimal InOneMilliElectronVolt = 1E-12m;

        private const Decimal InOneTeraElectronVolt = 1E3m;

        /// <summary>
        ///     About 79228162514264337593543950335.
        /// </summary>
        public static readonly GigaElectronVolts MaxValue = new GigaElectronVolts( Decimal.MaxValue );

        /// <summary>
        ///     About -79228162514264337593543950335.
        /// </summary>
        public static readonly GigaElectronVolts MinValue = new GigaElectronVolts( Decimal.MinValue );

        /// <summary>
        /// </summary>
        public static readonly GigaElectronVolts One = new GigaElectronVolts( 1 );

        /// <summary>
        /// </summary>
        public static readonly GigaElectronVolts Zero = new GigaElectronVolts( 0 );

        /// <summary>
        /// </summary>
        public readonly Decimal Value;

        public GigaElectronVolts( Decimal units ) : this() {
            this.Value = units;
        }

        public GigaElectronVolts( MegaElectronVolts megaElectronVolts ) : this() {
            this.Value = megaElectronVolts.ToGigaElectronVolts().Value;
        }

        [UsedImplicitly]
        private string DebuggerDisplay { get { return this.Display(); } }

        public int CompareTo( ElectronVolts other ) {
            return this.Value.CompareTo( other.ToGigaElectronVolts().Value );
        }

        public int CompareTo( GigaElectronVolts other ) {
            return this.Value.CompareTo( other.Value );
        }

        public int CompareTo( MegaElectronVolts other ) {
            return this.Value.CompareTo( other.ToGigaElectronVolts().Value );
        }

        public int CompareTo( MilliElectronVolts other ) {
            return this.Value.CompareTo( other.ToGigaElectronVolts().Value );
        }

        public static GigaElectronVolts operator *( GigaElectronVolts left, Decimal right ) {
            return new GigaElectronVolts( left.Value*right );
        }

        public static GigaElectronVolts operator *( Decimal left, GigaElectronVolts right ) {
            return new GigaElectronVolts( left*right.Value );
        }

        public static GigaElectronVolts operator +( MegaElectronVolts left, GigaElectronVolts right ) {
            return new GigaElectronVolts( left ) + right;
        }

        public static GigaElectronVolts operator +( GigaElectronVolts left, GigaElectronVolts right ) {
            return new GigaElectronVolts( left.Value + right.Value );
        }

        public static Boolean operator <( GigaElectronVolts left, GigaElectronVolts right ) {
            return left.Value.CompareTo( right.Value ) < 0;
        }

        public static Boolean operator >( GigaElectronVolts left, GigaElectronVolts right ) {
            return left.Value.CompareTo( right.Value ) > 0;
        }

        public string Display() {
            return String.Format( "{0:R} GeV", this.Value );
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

        [Obsolete( "Use Display() instead" )]
        public override string ToString() {
            return base.ToString();
        }

        public TeraElectronVolts ToTeraElectronVolts() {
            return new TeraElectronVolts( this.Value*InOneTeraElectronVolt );
        }
    }
}
