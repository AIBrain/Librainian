// Copyright 2016 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/SimpleMutex.cs" was last cleaned by Rick on 2016/12/10 at 9:42 PM

namespace Librainian.Threading {

	using System;
	using System.Diagnostics;
	using System.Threading;
	using JetBrains.Annotations;
	using Magic;

	/// <summary>
	///     A simple, cross application mutex.
	///     Use <see cref="Acquire" /> to acquire it
	/// </summary>
	/// <remarks>
	///     Only one thread (and thus process) can have the mutex acquired at the same time.
	/// </remarks>
	public sealed class SimpleMutex : ABetterClassDispose {

		private readonly Mutex _mutex;

		private SimpleMutex( String mutexName ) => this._mutex = new Mutex( false, mutexName );

		protected override void DisposeManaged() {
			using ( this._mutex ) {
				this._mutex.ReleaseMutex();
			}
		}

		/// <summary>
		///     Acquires the mutex with the specified name.
		/// </summary>
		/// <param name="mutexName">the mutex's name</param>
		/// <param name="timeout">how long to try to acquire the mutex</param>
		/// <returns>
		///     Returns the mutex or <c>null</c>,
		///     if the mutex couldn't be acquired in time (i.e. the current mutex holder didn't release it in time).
		/// </returns>
		[ CanBeNull ]
		public static SimpleMutex Acquire( String mutexName, TimeSpan timeout ) {
			var mutex = new SimpleMutex( mutexName );
			try {
				if ( !mutex._mutex.WaitOne( timeout ) ) {
					// We could not acquire the mutex in time. 
					mutex._mutex.Dispose();
					return null;
				}
			}
			catch ( AbandonedMutexException ex ) {
				// We now own this mutex. The previous owner didn't release it properly, though.
				Debug.WriteLine( ex );
			}
			return mutex;
		}

	}

}
