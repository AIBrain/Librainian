// Copyright � Protiguous. All Rights Reserved.
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
// File "AbandonableTask.cs" last touched on 2021-03-07 at 1:57 PM by Protiguous.

namespace Librainian.Threading {

	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;

	/// <summary></summary>
	/// <see cref="http://stackoverflow.com/a/4749401/956364" />
	public sealed class AbandonableTask {

		private AbandonableTask( [CanBeNull] Action? beginWork, [NotNull] Action blockingWork, [CanBeNull] Action<Task>? afterComplete, CancellationToken cancellationToken ) {
			this._beginWork = beginWork;
			this._blockingWork = blockingWork ?? throw new ArgumentNullException( nameof( blockingWork ) );
			this.AfterComplete = afterComplete;
			this._cancellationToken = cancellationToken;
		}

		[CanBeNull]
		private Action? _beginWork { get; }

		[CanBeNull]
		private Action _blockingWork { get; }

		private CancellationToken _cancellationToken { get; }

		[CanBeNull]
		public Action<Task>? AfterComplete { get; }

		private void RunTask() {
			this._beginWork?.Invoke();

			var innerTask = new Task( this._blockingWork, this._cancellationToken, TaskCreationOptions.LongRunning );
			innerTask.Start();

			innerTask.Wait( this._cancellationToken );

			if ( innerTask.IsCompleted ) {
				this.AfterComplete?.Invoke( innerTask );
			}
		}

		[NotNull]
		public static Task Start(
			[NotNull] Action blockingWork,
			[CanBeNull] Action? beginWork,
			[CanBeNull] Action<Task>? afterComplete,
			CancellationToken cancellationToken
		) {
			if ( blockingWork is null ) {
				throw new ArgumentNullException( nameof( blockingWork ) );
			}

			var worker = new AbandonableTask( beginWork, blockingWork, afterComplete, cancellationToken );
			var outerTask = new Task( worker.RunTask, cancellationToken );
			outerTask.Start();

			return outerTask;
		}

	}

}