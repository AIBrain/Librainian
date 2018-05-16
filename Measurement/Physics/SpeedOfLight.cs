// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "SpeedOfLight.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/SpeedOfLight.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Physics {

    using System;

    /// <summary>
    ///     Speed in meters per second at which light travels in a vacuum.
    ///     http: //wikipedia.org/wiki/Speed_of_light
    /// </summary>
    public static class SpeedOfLight {

        /// <summary>http: //www.wolframalpha.com/input/?i=299%2C792%2C458+metres+per+second</summary>
        public static Decimal KiloMetersPerSecond { get; } = 299792M;

        public static Decimal MetersPerSecond { get; } = 299792458M;

        public static Decimal MetersPerSecondSquared { get; } = 299792458M * 299792458M;

        /// <summary>http: //www.wolframalpha.com/input/?i=299%2C792%2C458+metres+per+second</summary>
        public static Decimal MilesPerSecond { get; } = 186282M;

        /// <summary>http: //wikipedia.org/wiki/Planck_units</summary>
        public static Decimal PlanckUnits { get; } = 1M;
    }
}