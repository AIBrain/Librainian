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
// "Librainian/Next.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    public static class Next {

        public static DateTime Friday() => GetNextOfDay( DayOfWeek.Friday );

        public static DateTime Monday() => GetNextOfDay( DayOfWeek.Monday );

        public static DateTime Saturday() => GetNextOfDay( DayOfWeek.Saturday );

        public static DateTime Sunday() => GetNextOfDay( DayOfWeek.Sunday );

        public static DateTime Thursday() => GetNextOfDay( DayOfWeek.Thursday );

        public static DateTime Tuesday() => GetNextOfDay( DayOfWeek.Tuesday );

        public static DateTime Wednesday() => GetNextOfDay( DayOfWeek.Wednesday );

        private static DateTime GetNextOfDay( DayOfWeek dayOfWeek ) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta <= 0 ? delta + 7 : delta );
            return result;
        }
    }
}