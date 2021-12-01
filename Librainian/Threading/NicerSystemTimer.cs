// Copyright © Protiguous. All Rights Reserved.
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
// File "NicerSystemTimer.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

namespace Librainian.Threading;

using System;
using System.Diagnostics;
using System.Threading;
using Exceptions;
using Utilities.Disposables;
using Timer = System.Timers.Timer;

/// <summary>Updated the code.</summary>
public class NicerSystemTimer : ABetterClassDispose {

	/// <summary>Perform an <paramref name="action" /> after the given interval (in <paramref name="milliseconds" />).</summary>
	/// <param name="action"></param>
	/// <param name="repeat">Perform the <paramref name="action" /> again. (Restarts the <see cref="Timer" />.)</param>
	/// <param name="milliseconds"></param>
	public NicerSystemTimer( Action action, Boolean repeat, Double? milliseconds = null ) : base( nameof( NicerSystemTimer ) ) {
		if ( action == null ) {
			throw new NullException( nameof( action ) );
		}

		this.access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

		this.Timer = new Timer {
			AutoReset = false,
			Interval = milliseconds.GetValueOrDefault( 1 )
		};

		this.Timer.Elapsed += ( _, _ ) => {
			try {
				if ( this.access.TryEnterReadLock( 0 ) ) {
					this.Timer.Stop();
					action.Invoke();
				}

				if ( repeat ) {
					this.Timer.Start();
				}
			}
			catch ( Exception exception ) {
				Debug.WriteLine( exception );
			}
		};

		this.Timer.Start();
	}

	private ReaderWriterLockSlim? access { get; set; }

	private Timer? Timer { get; set; }

	public override void DisposeManaged() {
		using ( this.Timer ) {
			this.Timer?.Stop();
			this.Timer = null;
		}

		using ( this.access ) {
			this.access = null;
		}

		base.DisposeManaged();
	}
}