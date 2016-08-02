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
// "Librainian/DateTimeShortCreators.cs" was last cleaned by Rick on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class Last {

        public static DateTime Friday() => GetLastOfDay( DayOfWeek.Friday );

        public static DateTime Monday() => GetLastOfDay( DayOfWeek.Monday );

        public static DateTime Saturday() => GetLastOfDay( DayOfWeek.Saturday );

        public static DateTime Sunday() => GetLastOfDay( DayOfWeek.Sunday );

        public static DateTime Thursday() => GetLastOfDay( DayOfWeek.Thursday );

        public static DateTime Tuesday() => GetLastOfDay( DayOfWeek.Tuesday );

        public static DateTime Wednesday() => GetLastOfDay( DayOfWeek.Wednesday );

        private static DateTime GetLastOfDay( DayOfWeek dayOfWeek ) {
            var today = AdjustableCurrentTime.Today;
            var delta = dayOfWeek - today.DayOfWeek;

            var result = today.AddDays( delta >= 0 ? delta - 7 : delta );
            return result;
        }
    }
}