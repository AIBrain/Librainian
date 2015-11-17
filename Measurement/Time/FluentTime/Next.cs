// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Next.cs" was last cleaned by Rick on 2015/06/12 at 3:02 PM

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

        private static DateTime GetNextOfDay(DayOfWeek dayOfWeek) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta <= 0 ? delta + 7 : delta );
            return result;
        }
    }
}