// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Time.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Time.cs" was last formatted by Protiguous on 2018/06/04 at 4:16 PM.

namespace Librainian.Measurement.Time {

	using System;
	using Clocks;
	using Extensions;
	using Newtonsoft.Json;

	/// <summary>
	///     <para></para>
	/// </summary>
	[JsonObject]
	[Immutable]
	public struct Time {

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

		public static Time Zero = new Time( Hour.Minimum, Minute.Minimum, Second.Minimum, Millisecond.Minimum );

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
		/// <param name="span"></param>
		public Time( Span span ) : this( hour: ( Byte ) span.Hours.Value, minute: ( Byte ) span.Minutes.Value, second: ( Byte ) span.Seconds.Value, millisecond: ( UInt16 ) span.Milliseconds.Value,
			microsecond: ( UInt16 ) span.Microseconds.Value ) { }

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