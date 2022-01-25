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
// File "DeserializeReportStats.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Persistence;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Measurement.Time;
using Threading;
using Utilities.Disposables;

public sealed class DeserializeReportStats : ABetterClassDispose {

	public DeserializeReportStats( Action<DeserializeReportStats>? handler, TimeSpan? timing = null ) {
		this.Gains.Values.Clear();
		this.Gains.Value = 0;

		this.Losses.Values.Clear();
		this.Losses.Value = 0;

		this.Total = 0;
		this.Handler = handler;
		this.Timing = timing ?? Milliseconds.ThreeHundredThirtyThree;
	}

	private ThreadLocal<Int64> Gains { get; } = new( true );

	private Action<DeserializeReportStats> Handler { get; }

	private ThreadLocal<Int64> Losses { get; } = new( true );

	public Boolean Enabled { get; set; }

	public TimeSpan Timing { get; }

	public Int64 Total { get; set; }

	/// <summary>Perform a Report.</summary>
	private async Task ReportAsync() {
		if ( !this.Enabled ) {
			return;
		}

		this.Handler( this );

		if ( this.Enabled ) {
			await this.Timing.Then( () => this.ReportAsync().ConfigureAwait( false ) ).ConfigureAwait( false ); //TODO is this correct?
		}
	}

	public void AddFailed( Int64 amount = 1 ) => this.Losses.Value += amount;

	public void AddSuccess( Int64 amount = 1 ) => this.Gains.Value += amount;

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		this.Gains.Dispose();
		this.Losses.Dispose();
	}

	public Int64 GetGains() => this.Gains.Values.Sum( arg => arg );

	public Int64 GetLoss() => this.Losses.Values.Sum( arg => arg );

	public Task StartReporting() {
		this.Enabled = true;

		return this.Timing.Then( () => this.ReportAsync().ConfigureAwait( false ) );
	}

	public void StopReporting() => this.Enabled = false;
}