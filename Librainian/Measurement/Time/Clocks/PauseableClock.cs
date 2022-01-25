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
// File "PauseableClock.cs" last formatted on 2022-12-22 at 5:17 PM by Protiguous.

namespace Librainian.Measurement.Time.Clocks;

using System;
using System.Timers;
using Logging;
using Newtonsoft.Json;
using Threadsafe;
using Utilities;

/// <summary>A 'pause-able' clock.</summary>
[JsonObject]
[NeedsTesting]
public class PauseableClock : IStandardClock {

	private VolatileBoolean _isPaused = new( false );

	public PauseableClock( TimeClock time ) {
		this.Hour = time.Hour;
		this.Minute = time.Minute;
		this.Second = time.Second;
		this.Millisecond = time.Millisecond;
		this.Microsecond = time.Microsecond;
		this.Timer.Elapsed += this.OnTimerElapsed;
		this.Resume();
	}

	private Timer Timer { get; } = new( ( Double )Milliseconds.One.Value ) {
		AutoReset = false
	};

	[JsonProperty]
	public ClockHour Hour { get; private set; }

	[JsonProperty]
	public Boolean IsPaused {
		get => this._isPaused;

		private set => this._isPaused = value;
	}

	[JsonProperty]
	public ClockMicrosecond Microsecond { get; private set; }

	[JsonProperty]
	public ClockMillisecond Millisecond { get; private set; }

	[JsonProperty]
	public ClockMinute Minute { get; private set; }

	public Action<TimeClock>? OnHour { get; set; }

	public Action<TimeClock>? OnMillisecond { get; set; }

	public Action<TimeClock>? OnMinute { get; set; }

	public Action<TimeClock>? OnMonth { get; set; }

	public Action<TimeClock>? OnSecond { get; set; }

	[JsonProperty]
	public ClockSecond Second { get; private set; }

	private Boolean HoursTocked( Boolean fireEvents ) {
		this.Hour = this.Hour.Next( out var tocked );

		if ( !tocked ) {
			return false;
		}

		try {
			if ( fireEvents ) {
				this.OnHour?.Invoke( this.Time() );
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
				this.OnMillisecond?.Invoke( this.Time() );
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
				this.OnMinute?.Invoke( this.Time() );
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
				this.OnSecond?.Invoke( this.Time() );
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

		if ( !this.HoursTocked( fireEvents ) ) { }
	}

	/// <summary>Advance the clock by <paramref name="amount" /><see cref="Milliseconds" />.</summary>
	/// <param name="amount">    </param>
	/// <param name="skipEvents"></param>
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
	public Boolean Rewind( Milliseconds amount ) {
		try {
			this.Pause();

			//TODO
			throw new NotImplementedException();
		}
		finally {
			this.Resume();
		}
	}

	public TimeClock Time() => new( this.Hour, this.Minute, this.Second, this.Millisecond, this.Microsecond );
}