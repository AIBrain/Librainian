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
// File "TimeClock.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Time.Clocks;

using System;
using Extensions;
using Newtonsoft.Json;

/// <summary>
///     <para></para>
/// </summary>
[JsonObject]
[Immutable]
public record TimeClock( ClockHour Hour, ClockMinute Minute, ClockSecond Second, ClockMillisecond Millisecond, ClockMicrosecond Microsecond ) : IStandardClock {

	/// <summary>
	/// </summary>
	/// <param name="dateTime"></param>
	public TimeClock( DateTime dateTime ) : this( new ClockHour( ( Byte )dateTime.Hour ), new ClockMinute( ( Byte )dateTime.Minute ),
		new ClockSecond( ( Byte )dateTime.Second ), new ClockMillisecond( ( UInt16 )dateTime.Millisecond ), 0 ) { }

	public static TimeClock Minimum { get; } = new( 0, 0, 0, 0, 0 );

	public static TimeClock Maximum { get; } = new( new ClockHour( Hours.InOneDay - 1 ), new ClockMinute( Minutes.InOneHour - 1 ), new ClockSecond( Seconds.InOneMinute - 1 ),
		new ClockMillisecond( Milliseconds.InOneSecond - 1 ), Microseconds.InOneMillisecond - 1 );

	public Boolean IsAm() => this.Hour.Value < 12;

	public Boolean IsPm() => !this.IsAm();

	public TimeClock Time() => this;

	public static implicit operator TimeClock( DateTime dateTime ) =>
		new( new ClockHour( ( Byte )dateTime.Hour ), new ClockMinute( ( Byte )dateTime.Minute ), new ClockSecond( ( Byte )dateTime.Second ),
			new ClockMillisecond( ( UInt16 )dateTime.Millisecond ), 0 );

	/// <summary>
	/// </summary>
	/// <param name="date"></param>
	public static implicit operator DateTime( TimeClock date ) =>
		new( DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, date.Hour.Value, date.Minute.Value, date.Second.Value, date.Millisecond.Value );

	/// <summary>Get the local system's computer time.</summary>
	public static TimeClock Now() {
		var now = DateTime.Now;

		return new TimeClock( new ClockHour( ( Byte )now.Hour ), new ClockMinute( ( Byte )now.Minute ), new ClockSecond( ( Byte )now.Second ),
			new ClockMillisecond( ( UInt16 )now.Millisecond ), 0 );
	}

	public static TimeClock UtcNow() {
		var now = DateTime.UtcNow;

		return new TimeClock( new ClockHour( ( Byte )now.Hour ), new ClockMinute( ( Byte )now.Minute ), new ClockSecond( ( Byte )now.Second ),
			new ClockMillisecond( ( UInt16 )now.Millisecond ), 0 );
	}
}