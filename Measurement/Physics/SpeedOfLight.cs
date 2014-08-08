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
// "Librainian2/SpeedOfLight.cs" was last cleaned by Rick on 2014/08/08 at 2:29 PM
#endregion

namespace Librainian.Measurement.Physics {
    using System;

    /// <summary>
    ///     Speed in meters per second at which light travels in a vacuum.
    ///     http://wikipedia.org/wiki/Speed_of_light
    /// </summary>
    public static class SpeedOfLight {
        /// <summary>
        ///     http://www.wolframalpha.com/input/?i=299%2C792%2C458+metres+per+second
        /// </summary>
        public static readonly Decimal KiloMetersPerSecond = 299792M;

        public static readonly Decimal MetersPerSecond = 299792458M;

        /// <summary>
        ///     http://www.wolframalpha.com/input/?i=299%2C792%2C458+metres+per+second
        /// </summary>
        public static readonly Decimal MilesPerSecond = 186282M;

        /// <summary>
        ///     http://wikipedia.org/wiki/Planck_units
        /// </summary>
        public static readonly Decimal PlanckUnits = 1M;
    }
}
