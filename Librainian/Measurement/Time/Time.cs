// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Time.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Time.cs" was last formatted by Protiguous on 2018/06/26 at 1:32 AM.

namespace Librainian.Measurement.Time {

	using System;
	using Clocks;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     <para></para>
	/// </summary>
	[JsonObject]
	[Immutable]
	public struct Time {

		public static Time Zero = new Time( Hour.Minimum, Minute.Minimum, Second.Minimum, Millisecond.Minimum );

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Hour Hour { get; }

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Microsecond Microsecond { get; }

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Millisecond Millisecond { get; }

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Minute Minute { get; }

		/// <summary>
		/// </summary>
		[JsonProperty]
		public Second Second { get; }

		/// <summary>
		/// </summary>
		/// <param name="hour"></param>
		/// <param name="minute"></param>
		/// <param name="second"></param>
		/// <param name="millisecond"></param>
		/// <param name="microsecond"></param>
		public Time( Byte hour = 0, Byte minute = 0, Byte second = 0, UInt16 millisecond = 0, UInt16 microsecond = 0 ) : this() {
			this.Hour = hour;
			this.Minute = minute;
			this.Second = second;
			this.Millisecond = millisecond;
			this.Microsecond = microsecond;
		}

		/// <summary>
		/// </summary>
		/// <param name="dateTime"></param>
		public Time( DateTime dateTime ) : this( hour: ( Byte ) dateTime.Hour, minute: ( Byte ) dateTime.Minute, second: ( Byte ) dateTime.Second, millisecond: ( UInt16 ) dateTime.Millisecond ) { }

		/// <summary>
		/// </summary>
		/// <param name="spanOfTime"></param>
		public Time( [NotNull] SpanOfTime spanOfTime ) : this( hour: ( Byte ) spanOfTime.Hours.Value, minute: ( Byte ) spanOfTime.Minutes.Value, second: ( Byte ) spanOfTime.Seconds.Value,
			millisecond: ( UInt16 ) spanOfTime.Milliseconds.Value, microsecond: ( UInt16 ) spanOfTime.Microseconds.Value ) { }

		public static explicit operator Time( DateTime dateTime ) => new Time( ( Byte ) dateTime.Hour, ( Byte ) dateTime.Minute, ( Byte ) dateTime.Second, ( UInt16 ) dateTime.Millisecond );

		/// <summary>
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static implicit operator DateTime( Time date ) =>
			new DateTime( year: DateTime.MinValue.Year, month: DateTime.MinValue.Month, day: DateTime.MinValue.Day, hour: date.Hour.Value, minute: date.Minute.Value, second: date.Second.Value,
				millisecond: date.Millisecond.Value );

		/// <summary>
		///     Get the local system's computer time.
		/// </summary>
		public static Time Now() {
			var now = DateTime.Now;

			return new Time( hour: ( Byte ) now.Hour, minute: ( Byte ) now.Minute, second: ( Byte ) now.Second, millisecond: ( UInt16 ) now.Millisecond );
		}

		public static Time UtcNow() {
			var now = DateTime.UtcNow;

			return new Time( hour: ( Byte ) now.Hour, minute: ( Byte ) now.Minute, second: ( Byte ) now.Second, millisecond: ( UInt16 ) now.Millisecond );
		}
	}
}