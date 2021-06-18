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
// File "PauseableClock.cs" last formatted on 2020-08-14 at 8:37 PM.

namespace Librainian.Measurement.Time.Clocks {

	using System;
	using System.Timers;
	using Logging;
	using Newtonsoft.Json;
	using Threadsafe;

	/// <summary>A 'pause-able' clock.</summary>
	[JsonObject]
	public class PauseableClock : IStandardClock {

		/// <summary></summary>
		private VolatileBoolean _isPaused;

		/// <summary></summary>
		private Timer Timer { get; } = new( ( Double )Milliseconds.One.Value ) {
			AutoReset = false
		};

		[JsonProperty]
		public Day Day { get; private set; }

		/// <summary></summary>
		[JsonProperty]
		public ClockHour Hour { get; private set; }

		[JsonProperty]
		public Boolean IsPaused {
			get => this._isPaused;

			private set => this._isPaused = value;
		}

		/// <summary></summary>
		[JsonProperty]
		public ClockMillisecond Millisecond { get; private set; }

		/// <summary></summary>
		[JsonProperty]
		public ClockMinute Minute { get; private set; }

		[JsonProperty]
		public Month? Month { get; private set; }

		public Action<DateAndTime>? OnDay { get; set; }

		public Action<DateAndTime>? OnHour { get; set; }

		public Action<DateAndTime>? OnMillisecond { get; set; }

		public Action<DateAndTime>? OnMinute { get; set; }

		public Action<DateAndTime>? OnMonth { get; set; }

		public Action<DateAndTime>? OnSecond { get; set; }

		public Action<DateAndTime>? OnYear { get; set; }

		/// <summary></summary>
		[JsonProperty]
		public ClockSecond Second { get; private set; }

		[JsonProperty]
		public Year? Year { get; private set; }

		/// <summary>Default to year 0.</summary>
		public PauseableClock() : this( Measurement.Time.Date.Zero, Measurement.Time.Time.Minimum ) { }

		public PauseableClock( Date date, Time time ) {
			this.Year = date.Year;
			this.Month = date.Month;
			this.Day = date.Day;
			this.Hour = time.Hour;
			this.Minute = time.Minute;
			this.Second = time.Second;
			this.Millisecond = time.Millisecond;
			this.Timer.Elapsed += this.OnTimerElapsed;
			this.Resume();
		}

		private Boolean DaysTocked( Boolean fireEvents ) {
			this.Day = this.Day.Next( out var tocked );

			if ( !tocked ) {
				return false;
			}

			try {
				if ( fireEvents ) {
					this.OnDay?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return true;
		}

		private Boolean HoursTocked( Boolean fireEvents ) {
			this.Hour = this.Hour.Next( out var tocked );

			if ( !tocked ) {
				return false;
			}

			try {
				if ( fireEvents ) {
					this.OnHour?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return true;
		}

		private Boolean MillisecondsTocked( Boolean fireEvents ) {
			this.Millisecond = this.Millisecond.Next( out var tocked );

			if ( !tocked ) {
				return false;
			}

			try {
				if ( fireEvents ) {
					this.OnMillisecond?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return true;
		}

		private Boolean MinutesTocked( Boolean fireEvents ) {
			this.Minute = this.Minute.Next( out var tocked );

			if ( !tocked ) {
				return false;
			}

			try {
				if ( fireEvents ) {
					this.OnMinute?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return true;
		}

		private Boolean MonthsTocked( Boolean fireEvents ) {
			this.Month = this.Month.Next( out var tocked );

			if ( !tocked ) {
				return false;
			}

			try {
				if ( fireEvents ) {
					this.OnMonth?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return true;
		}

		private void OnTimerElapsed( Object? sender, ElapsedEventArgs? elapsedEventArgs ) {
			this.Pause();

			try {
				this.TickTock();
			}
			catch ( Exception exception ) {
				exception.Log();
			}
			finally {
				this.Resume();
			}
		}

		private Boolean SecondsTocked( Boolean fireEvents ) {
			this.Second = this.Second.Next( out var tocked );

			if ( !tocked ) {
				return false;
			}

			try {
				if ( fireEvents ) {
					this.OnSecond?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}

			return true;
		}

		private void TickTock( Boolean fireEvents = true ) {
			if ( !this.MillisecondsTocked( fireEvents ) ) {
				return;
			}

			if ( !this.SecondsTocked( fireEvents ) ) {
				return;
			}

			if ( !this.MinutesTocked( fireEvents ) ) {
				return;
			}

			if ( !this.HoursTocked( fireEvents ) ) {
				return;
			}

			if ( !this.DaysTocked( fireEvents ) ) {
				return;
			}

			if ( !this.MonthsTocked( fireEvents ) ) {
				return;
			}

			this.YearsTocked( fireEvents );
		}

		private void YearsTocked( Boolean fireEvents ) {
			this.Year = this.Year.Next();

			try {
				if ( fireEvents ) {
					this.OnYear?.Invoke( this.DateAndTime() );
				}
			}
			catch ( Exception exception ) {
				exception.Log();
			}
		}

		/// <summary>Advance the clock by <paramref name="amount" /><see cref="Milliseconds" />.</summary>
		/// <param name="amount">    </param>
		/// <param name="skipEvents"></param>
		/// <returns></returns>
		public Boolean Advance( Milliseconds amount, Boolean skipEvents = true ) {
			try {
				this.Pause();
				var right = amount.Value;

				while ( right > Decimal.Zero ) {
					this.TickTock( false );
					right--;
				}

				return true;
			}
			finally {
				this.Resume();
			}
		}

		public Date Date() => new( this.Year, this.Month, this.Day );

		public DateAndTime DateAndTime() => new( this.Date(), this.Time() );

		public Boolean IsAm() => !this.IsPm();

		public Boolean IsPm() => this.Hour >= 12;

		public Boolean Pause() {
			this.Timer.Stop();
			this.IsPaused = true;

			return this.IsPaused;
		}

		public Boolean Resume() {
			this.IsPaused = false;
			this.Timer.Start();

			return !this.IsPaused;
		}

		/// <summary>Rewind the clock by <paramref name="amount" /><see cref="Milliseconds" />.</summary>
		/// <param name="amount"></param>
		/// <returns></returns>
		public Boolean Rewind( Milliseconds amount ) {
			try {
				this.Pause();

				//TODO
				throw new NotImplementedException();

				// ReSharper disable once HeuristicUnreachableCode
				//return default;
			}
			finally {
				this.Resume();
			}
		}

		public Time Time() => new( this.Hour, this.Minute, this.Second, this.Millisecond );
	}
}