// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "MomentInTimeClock.cs" last formatted on 2020-08-14 at 8:37 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>A clock that stays at the set moment in time.</summary>
	[JsonObject]
	[Immutable]
	public class MomentInTimeClock : IStandardClock {

		[JsonProperty]
		public ClockHour Hour { get; }

		/// <summary></summary>
		[JsonProperty]
		public ClockMillisecond Millisecond { get; }

		[JsonProperty]
		public ClockMinute Minute { get; }

		[JsonProperty]
		public ClockSecond Second { get; }

		public MomentInTimeClock() : this( Measurement.Time.Time.Now() ) { }

		public MomentInTimeClock( Time time ) {
			this.Hour = time.Hour;
			this.Minute = time.Minute;
			this.Second = time.Second;
			this.Millisecond = time.Millisecond;
		}

		public MomentInTimeClock( DateTime time ) : this( ( Time )time ) { }

		public Boolean IsAm() => !this.IsPm();

		public Boolean IsPm() => this.Hour.Value >= 12;

		public Time Time() => new( this.Hour, this.Minute, this.Second );
	}
}