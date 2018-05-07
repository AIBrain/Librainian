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
// "Librainian/DateTimeOffsetting.cs" was last cleaned by Protiguous on 2016/06/18 at 10:54 PM

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class DateTimeOffsetting {

        public static DateTimeOffset Offset( this DateTime d, TimeSpan offset ) => new DateTimeOffset( d, offset );

        public static DateTimeOffset Offset( this DateTime d, Int32 hours ) => d.Offset( TimeSpan.FromHours( hours ) );

        public static DateTimeOffset OffsetFor( this DateTime d, TimeZoneInfo zone ) => d.Offset( zone.GetUtcOffset( d ) );

        public static DateTimeOffset OffsetFor( this DateTime d, String timeZoneId ) => d.OffsetFor( TimeZoneInfo.FindSystemTimeZoneById( timeZoneId ) );

        public static DateTimeOffset OffsetForLocal( this DateTime d ) => d.OffsetFor( TimeZoneInfo.Local );
    }
}