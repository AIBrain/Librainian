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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Idler.cs" last touched on 2021-04-25 at 10:38 AM by Protiguous.

namespace Librainian.Threading;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Collections.Sets;
using Exceptions;
using Extensions;
using Logging;
using Utilities.Disposables;

public enum JobStatus {

	Exception = -1,

	Unknown = 0,

	Running = 1,

	Finished
}

public interface IIdler {

	/// <summary>Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.</summary>
	/// <param name="name"></param>
	/// <param name="action"> </param>
	void Add( String name, Action action );

	Boolean Any();

	/// <summary>
	///     Run any remaining jobs.
	///     <para>Will exit while loop if <see cref="CancellationToken" /> is signaled to cancel.</para>
	/// </summary>
	void Finish();
}

public class Idler : ABetterClassDispose, IIdler {

	private ConcurrentDictionary<String, Action> Jobs { get; } = new();

	private ConcurrentHashset<Task> Runners { get; } = new();

	private CancellationToken Token { get; }

	public Idler( CancellationToken cancellationToken ) : base( nameof( Idler ) ) {
		this.Token = cancellationToken;

		//this.Jobs.CollectionChanged += ( sender, args ) => this.NextJob();
		Application.Idle += this.OnIdle;
	}

	/// <summary>Pull next <see cref="Action" /> to run from the queue and execute it.</summary>
	private void NextJob() {
		if ( !this.Any() || this.Token.IsCancellationRequested ) {
			return;
		}

		var job = this.Jobs.Keys.FirstOrDefault();

		if ( job is null || !this.Jobs.TryRemove( job, out var jack ) ) {
			return;
		}

		try {

			$"{nameof(Idler)}: Running next job..".Verbose();
			jack.Execute();
		}
		catch ( Exception exception ) {
			exception.Log();
		}
		finally {
			this.Nop();

			$"{nameof( Idler )}: Done with job.".Verbose();
		}
	}

	private void OnIdle( Object? sender, EventArgs? e ) => this.NextJob();

	private void RemoveHandler() {
		if ( !this.IsDisposed ) {
			Application.Idle -= this.OnIdle;
		}
	}

	/// <summary>Add an <paramref name="action" /> as a job to be ran on the next <see cref="Application.Idle" /> event.</summary>
	/// <param name="name"></param>
	/// <param name="action"> </param>
	public void Add( String name, Action action ) {
		if ( action is null ) {
			throw new ArgumentEmptyException( nameof( action ) );
		}

		if ( String.IsNullOrWhiteSpace( name ) ) {
			throw new ArgumentException( "Value cannot be null or whitespace.", nameof( name ) );
		}

		Error.Trap( () => this.Jobs.TryAdd( name, action ) );
	}

	public Boolean Any() => this.Jobs.Any();

	public override void DisposeManaged() => this.RemoveHandler();

	/// <summary>
	///     Run any remaining jobs.
	///     <para>Will exit prematurely if <see cref="Token" /> is signaled to cancel.</para>
	/// </summary>
	public void Finish() {
		while ( this.Any() && !this.Token.IsCancellationRequested ) {
			this.NextJob();
		}
	}
}