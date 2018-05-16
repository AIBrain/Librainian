// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Extensions.cs",
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
// "Librainian/Librainian/Extensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Physics {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    public static class Extensions {

        public static Expression<Func<Double, Double>> FahrenheitToCelsius = fahrenheit => ( fahrenheit - 32.0 ) * 5.0 / 9.0;

        public static String Simpler( this ElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this KiloElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this MegaElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this GigaElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this TeraElectronVolts volts ) {
            var list = new HashSet<String> {
                volts.ToTeraElectronVolts().ToString(),
                volts.ToGigaElectronVolts().ToString(),
                volts.ToMegaElectronVolts().ToString(),
                volts.ToElectronVolts().ToString(),
                volts.ToMilliElectronVolts().ToString()
            };

            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static MegaElectronVolts Sum( [NotNull] this IEnumerable<ElectronVolts> volts ) {
            if ( volts is null ) { throw new ArgumentNullException( nameof( volts ) ); }

            var result = volts.Aggregate( MegaElectronVolts.Zero, ( current, electronVolts ) => current + electronVolts.ToMegaElectronVolts() );

            return result;
        }

        public static GigaElectronVolts Sum( [NotNull] this IEnumerable<MegaElectronVolts> volts ) {
            if ( volts is null ) { throw new ArgumentNullException( nameof( volts ) ); }

            return volts.Aggregate( GigaElectronVolts.Zero, ( current, megaElectronVolts ) => current + megaElectronVolts.ToGigaElectronVolts() );
        }

        public static TeraElectronVolts Sum( [NotNull] this IEnumerable<GigaElectronVolts> volts ) {
            if ( volts is null ) { throw new ArgumentNullException( nameof( volts ) ); }

            return volts.Aggregate( TeraElectronVolts.Zero, ( current, gigaElectronVolts ) => current + gigaElectronVolts.ToTeraElectronVolts() );
        }
    }
}