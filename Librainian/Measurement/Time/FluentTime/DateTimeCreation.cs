// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "DateTimeCreation.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "DateTimeCreation.cs" was last formatted by Protiguous on 2018/07/13 at 1:28 AM.

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