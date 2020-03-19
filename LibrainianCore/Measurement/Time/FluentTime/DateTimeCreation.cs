// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "DateTimeCreation.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "LibrainianCore", File: "DateTimeCreation.cs" was last formatted by Protiguous on 2020/03/16 at 3:07 PM.

namespace Librainian.Measurement.Time.FluentTime {

    using System;

    /// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
    public static class DateTimeCreation {

        private static void Reject24HourTime( DateTime d ) {
            if ( d.Hour > 12 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( d ), message: $"Not a 12-hour time. Hour is {d.Hour}." );
            }
        }

        public static DateTime Am( this DateTime d ) {
            Reject24HourTime( d: d );

            return d.Hour < 12 ? d : d.AddHours( value: -12 );
        }

        public static DateTime April( this Int32 day, Int32 year ) => new DateTime( year: year, month: 4, day: day );

        public static DateTime At( this DateTime d, Int32 hour, Int32 minute = 0, Int32 second = 0 ) =>
            new DateTime( year: d.Year, month: d.Month, day: d.Day, hour: hour, minute: minute, second: second );

        public static DateTime August( this Int32 day, Int32 year ) => new DateTime( year: year, month: 8, day: day );

        public static DateTime December( this Int32 day, Int32 year ) => new DateTime( year: year, month: 12, day: day );

        public static DateTime February( this Int32 day, Int32 year ) => new DateTime( year: year, month: 2, day: day );

        public static DateTime January( this Int32 day, Int32 year ) => new DateTime( year: year, month: 1, day: day );

        public static DateTime July( this Int32 day, Int32 year ) => new DateTime( year: year, month: 7, day: day );

        public static DateTime June( this Int32 day, Int32 year ) => new DateTime( year: year, month: 6, day: day );

        public static DateTime Local( this DateTime d ) => DateTime.SpecifyKind( value: d, kind: DateTimeKind.Local );

        public static DateTime March( this Int32 day, Int32 year ) => new DateTime( year: year, month: 3, day: day );

        public static DateTime May( this Int32 day, Int32 year ) => new DateTime( year: year, month: 5, day: day );

        public static DateTime November( this Int32 day, Int32 year ) => new DateTime( year: year, month: 11, day: day );

        public static DateTime October( this Int32 day, Int32 year ) => new DateTime( year: year, month: 10, day: day );

        public static DateTime Pm( this DateTime d ) {
            Reject24HourTime( d: d );

            return d.Hour == 12 ? d : d.AddHours( value: 12 );
        }

        public static DateTime September( this Int32 day, Int32 year ) => new DateTime( year: year, month: 9, day: day );

        public static DateTime Utc( this DateTime d ) => DateTime.SpecifyKind( value: d, kind: DateTimeKind.Utc );

    }

}