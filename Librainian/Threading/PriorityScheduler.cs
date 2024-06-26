// Copyright � Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "PriorityScheduler.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

#nullable enable

namespace Librainian.Threading;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <example>
/// Task.Run(() =&gt; { //everything here will be executed in a thread whose priority is BelowNormal }, null,
/// TaskCreationOptions.None, PriorityScheduler.BelowNormal);
/// </example>
/// <see cref="http://stackoverflow.com/questions/3836584/lowering-priority-of-task-factory-startnew-thread" />
public class PriorityScheduler : TaskScheduler, IDisposable {

	private readonly ThreadPriority _priority;

	private readonly BlockingCollection<Task> _tasks = new();

	private Thread[]? _threads;

	public PriorityScheduler( ThreadPriority priority ) => this._priority = priority;

	public static PriorityScheduler AboveNormal { get; } = new( ThreadPriority.AboveNormal );

	public static PriorityScheduler BelowNormal { get; } = new( ThreadPriority.BelowNormal );

	public static PriorityScheduler Lowest { get; } = new( ThreadPriority.Lowest );

	public override Int32 MaximumConcurrencyLevel { get; } = Math.Max( 1, Environment.ProcessorCount );

	protected override IEnumerable<Task> GetScheduledTasks() => this._tasks;

	protected override void QueueTask( Task? task ) {
		this._tasks.Add( task );

		if ( this._threads is null ) {
			this._threads = new Thread[ Math.Max( 1, Environment.ProcessorCount ) ];

			var threads = this._threads;

			for ( var i = 0; i < threads.Length; i++ ) {
				threads[ i ] = new Thread( () => {
					foreach ( var t in this._tasks.GetConsumingEnumerable() ) {
						this.TryExecuteTask( t );
					}
				} ) {
					Name = $"PriorityScheduler: {i}",
					Priority = this._priority,
					IsBackground = true
				};

				threads[ i ].Start();
			}
		}
	}

	protected override Boolean TryExecuteTaskInline( Task? task, Boolean taskWasPreviouslyQueued ) => false;

	public void Dispose() {
		this._tasks.Dispose();
		GC.SuppressFinalize( this );
	}
}