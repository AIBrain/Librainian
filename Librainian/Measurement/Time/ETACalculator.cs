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
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ETACalculator.cs" last formatted on 2022-12-22 at 5:18 PM by Protiguous.

namespace Librainian.Measurement.Time;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using FluentTime;
using Maths;
using Utilities.Disposables;

/// <summary>
///     <para>Calculates the "Estimated Time of Arrival", aka ETA</para>
/// </summary>
public class EtaCalculator : ABetterClassDispose {

	/// <summary>At these points in time, how far along have we progressed?</summary>
	private readonly ConcurrentDictionary<TimeSpan, Single> _datapoints = new();

	/// <summary>Start our timer so we can keep track of elapsed time.</summary>
	private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

	private volatile Single _progress;

	private Timer? _timer;

	public EtaCalculator() => this.Reset( Seconds.One );

	/// <summary>
	///     <para>The value to be updated to a value between 0 and 1 when possible.</para>
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public Single Progress {
		get => this._progress;

		set {
			if ( !value.IsNumber() ) {
				throw new InvalidOperationException();
			}

			if ( value is < 0 or > 1 ) {
				throw new ArgumentOutOfRangeException( nameof( this.Progress ), $"{value:R} is out of the range 0 to 1." );
			}

			this._progress = value;
		}
	}

	/// <summary>
	///     <para>Returns True when there is enough data to calculate the ETA.</para>
	/// </summary>
	public Boolean CanWeHaveAnEta() => this._datapoints.Any();

	/// <summary>Dispose of any <see cref="IDisposable" /> (managed) fields or properties in this method.</summary>
	public override void DisposeManaged() {
		using ( this._timer ) { }
	}

	/// <summary>
	///     <para>Calculates the Estimated Time of Completion</para>
	/// </summary>
	public DateTime Eta() => DateTime.Now + this.Etr();

	/// <summary>Get the internal data points we have so far.</summary>
	public IEnumerable<TimeProgression> GetDataPoints() =>
		this._datapoints.OrderBy( pair => pair.Key ).Select( pair => new TimeProgression( pair.Key.TotalMilliseconds, pair.Value ) );

	public void Reset( TimeSpan samplingPeriod ) {
		using ( this._timer ) {
			//this._timer?.Close();
		}

		this._stopwatch.Stop();
		this._datapoints.Clear();
		this._stopwatch.Reset();
		this._stopwatch.Start();
		this.Progress = 0;

		this._timer = new Timer {
			Interval = samplingPeriod.TotalMilliseconds,
			AutoReset = true
		};

		this._timer.Elapsed += ( _, _ ) => this.Update();
		this._timer?.Start();
	}

	/// <summary>
	///     <para>Manually add the known <see cref="Progress" /> to the internal data points.</para>
	/// </summary>
	public void Update() {
		if ( this.Progress >= 0 && this.Progress <= 1 && !this.Progress.IsNumber() ) {
			this._datapoints.TryAdd( this._stopwatch.Elapsed, this.Progress );
		}

		//throw new ArgumentOutOfRangeException( "Progress", "The Progress is out of the range 0 to 1." );
	}

}