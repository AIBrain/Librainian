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
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Extensions.cs" was last cleaned by Protiguous on 2016/06/18 at 10:53 PM

namespace Librainian.Measurement.Physics {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using JetBrains.Annotations;

    public static class Extensions {
        public static Expression<Func<Double, Double>> FahrenheitToCelsius = fahrenheit => ( fahrenheit - 32.0 ) * 5.0 / 9.0;

        public static String Simpler( this ElectronVolts volts ) {
            var list = new HashSet<String> { volts.ToTeraElectronVolts().ToString(), volts.ToGigaElectronVolts().ToString(), volts.ToMegaElectronVolts().ToString(), volts.ToElectronVolts().ToString(), volts.ToMilliElectronVolts().ToString() };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this KiloElectronVolts volts ) {
            var list = new HashSet<String> { volts.ToTeraElectronVolts().ToString(), volts.ToGigaElectronVolts().ToString(), volts.ToMegaElectronVolts().ToString(), volts.ToElectronVolts().ToString(), volts.ToMilliElectronVolts().ToString() };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this MegaElectronVolts volts ) {
            var list = new HashSet<String> { volts.ToTeraElectronVolts().ToString(), volts.ToGigaElectronVolts().ToString(), volts.ToMegaElectronVolts().ToString(), volts.ToElectronVolts().ToString(), volts.ToMilliElectronVolts().ToString() };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this GigaElectronVolts volts ) {
            var list = new HashSet<String> { volts.ToTeraElectronVolts().ToString(), volts.ToGigaElectronVolts().ToString(), volts.ToMegaElectronVolts().ToString(), volts.ToElectronVolts().ToString(), volts.ToMilliElectronVolts().ToString() };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static String Simpler( this TeraElectronVolts volts ) {
            var list = new HashSet<String> { volts.ToTeraElectronVolts().ToString(), volts.ToGigaElectronVolts().ToString(), volts.ToMegaElectronVolts().ToString(), volts.ToElectronVolts().ToString(), volts.ToMilliElectronVolts().ToString() };
            return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
        }

        public static MegaElectronVolts Sum( [NotNull] this IEnumerable<ElectronVolts> volts ) {
            if ( volts is null ) {
                throw new ArgumentNullException( nameof( volts ) );
            }
            var result = volts.Aggregate( MegaElectronVolts.Zero, ( current, electronVolts ) => current + electronVolts.ToMegaElectronVolts() );

            return result;
        }

        public static GigaElectronVolts Sum( [NotNull] this IEnumerable<MegaElectronVolts> volts ) {
            if ( volts is null ) {
                throw new ArgumentNullException( nameof( volts ) );
            }
            return volts.Aggregate( GigaElectronVolts.Zero, ( current, megaElectronVolts ) => current + megaElectronVolts.ToGigaElectronVolts() );
        }

        public static TeraElectronVolts Sum( [NotNull] this IEnumerable<GigaElectronVolts> volts ) {
            if ( volts is null ) {
                throw new ArgumentNullException( nameof( volts ) );
            }
            return volts.Aggregate( TeraElectronVolts.Zero, ( current, gigaElectronVolts ) => current + gigaElectronVolts.ToTeraElectronVolts() );
        }
    }
}