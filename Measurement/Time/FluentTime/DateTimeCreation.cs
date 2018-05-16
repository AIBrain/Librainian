// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DateTimeCreation.cs",
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
// "Librainian/Librainian/DateTimeCreation.cs" was last cleaned by Protiguous on 2018/05/15 at 10:47 PM.

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class DateTimeCreation {

        private static void Reject24HourTime( DateTime d ) {
            if ( d.Hour > 12 ) { throw new ArgumentOutOfRangeException( nameof( d ), $"Not a 12-hour time. Hour is {d.Hour}." ); }
        }

        public static DateTime Am( this DateTime d ) {
            Reject24HourTime( d );

            return d.Hour < 12 ? d : d.AddHours( -12 );
        }

        public static DateTime April( this Int32 day, Int32 year ) => new DateTime( year, 4, day );

        public static DateTime At( this DateTime d, Int32 hour, Int32 minute = 0, Int32 second = 0 ) => new DateTime( d.Year, d.Month, d.Day, hour, minute, second );

        public static DateTime August( this Int32 day, Int32 year ) => new DateTime( year, 8, day );

        public static DateTime December( this Int32 day, Int32 year ) => new DateTime( year, 12, day );

        public static DateTime February( this Int32 day, Int32 year ) => new DateTime( year, 2, day );

        public static DateTime January( this Int32 day, Int32 year ) => new DateTime( year, 1, day );

        public static DateTime July( this Int32 day, Int32 year ) => new DateTime( year, 7, day );

        public static DateTime June( this Int32 day, Int32 year ) => new DateTime( year, 6, day );

        public static DateTime Local( this DateTime d ) => DateTime.SpecifyKind( d, DateTimeKind.Local );

        public static DateTime March( this Int32 day, Int32 year ) => new DateTime( year, 3, day );

        public static DateTime May( this Int32 day, Int32 year ) => new DateTime( year, 5, day );

        public static DateTime November( this Int32 day, Int32 year ) => new DateTime( year, 11, day );

        public static DateTime October( this Int32 day, Int32 year ) => new DateTime( year, 10, day );

        public static DateTime Pm( this DateTime d ) {
            Reject24HourTime( d );

            return d.Hour == 12 ? d : d.AddHours( 12 );
        }

        public static DateTime September( this Int32 day, Int32 year ) => new DateTime( year, 9, day );

        public static DateTime Utc( this DateTime d ) => DateTime.SpecifyKind( d, DateTimeKind.Utc );
    }
}