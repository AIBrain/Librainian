// Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.

namespace Librainian.Measurement.Time.FluentTime {
    using System;

    internal static class AdjustableCurrentTime {
        private static DateTime? _overrideNow;

        public static DateTime Now => _overrideNow ?? DateTime.Now;

        public static DateTime Today => _overrideNow?.Date ?? DateTime.Today;

        internal static void Reset() => _overrideNow = null;

        internal static void SetNow( DateTime now ) => _overrideNow = now;
    }
}