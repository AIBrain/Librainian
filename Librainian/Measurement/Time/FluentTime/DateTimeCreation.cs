// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DateTimeCreation.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Time.FluentTime;

using System;

/// <summary>Copyright 2011 ThoughtWorks, Inc. See LICENSE.txt for licensing info.</summary>
public static class DateTimeCreation {

	private static void Reject24HourTime( DateTime d ) {
		if ( d.Hour > 12 ) {
			throw new ArgumentOutOfRangeException( nameof( d ), $"Not a 12-hour time. Hour is {d.Hour}." );
		}
	}

	public static DateTime Am( this DateTime d ) {
		Reject24HourTime( d );

		return d.Hour < 12 ? d : d.AddHours( -12 );
	}

	public static DateTime April( this Int32 day, Int32 year ) => new( year, 4, day );

	public static DateTime At( this DateTime d, Int32 hour, Int32 minute = 0, Int32 second = 0 ) => new( d.Year, d.Month, d.Day, hour, minute, second );

	public static DateTime August( this Int32 day, Int32 year ) => new( year, 8, day );

	public static DateTime December( this Int32 day, Int32 year ) => new( year, 12, day );

	public static DateTime February( this Int32 day, Int32 year ) => new( year, 2, day );

	public static DateTime January( this Int32 day, Int32 year ) => new( year, 1, day );

	public static DateTime July( this Int32 day, Int32 year ) => new( year, 7, day );

	public static DateTime June( this Int32 day, Int32 year ) => new( year, 6, day );

	public static DateTime Local( this DateTime d ) => DateTime.SpecifyKind( d, DateTimeKind.Local );

	public static DateTime March( this Int32 day, Int32 year ) => new( year, 3, day );

	public static DateTime May( this Int32 day, Int32 year ) => new( year, 5, day );

	public static DateTime November( this Int32 day, Int32 year ) => new( year, 11, day );

	public static DateTime October( this Int32 day, Int32 year ) => new( year, 10, day );

	public static DateTime Pm( this DateTime d ) {
		Reject24HourTime( d );

		return d.Hour == 12 ? d : d.AddHours( 12 );
	}

	public static DateTime September( this Int32 day, Int32 year ) => new( year, 9, day );

	public static DateTime Utc( this DateTime d ) => DateTime.SpecifyKind( d, DateTimeKind.Utc );
}