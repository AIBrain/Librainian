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
// "Librainian/SpeedOfLight.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

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