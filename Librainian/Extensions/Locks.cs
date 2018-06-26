// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "Locks.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "Locks.cs" was last formatted by Protiguous on 2018/06/26 at 1:03 AM.

namespace Librainian.Extensions {

	using System;
	using System.Threading;
	using JetBrains.Annotations;
	using NUnit.Framework;

	public static class Locks {

		/// <summary>
		///     put a Read ( will-read ) lock on the access object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <example>ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }</example>
		[NotNull]
		public static IDisposable Read( this ReaderWriterLockSlim obj ) => new ReadLockToken( obj );

		/// <summary>
		///     put an upgradeable ( will-read / may-write ) lock on the access object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <example>ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }</example>
		[NotNull]
		public static IDisposable Upgrade( this ReaderWriterLockSlim obj ) => new UpgradeLockToken( obj );

		/// <summary>
		///     put a Write ( will-write ) lock on the access object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <example>ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }</example>
		[NotNull]
		public static IDisposable Write( this ReaderWriterLockSlim obj ) => new WriteLockToken( obj );

		private sealed class ReadLockToken : IDisposable {

			private ReaderWriterLockSlim _readerWriterLockSlim;

			public ReadLockToken( [NotNull] ReaderWriterLockSlim slimLock ) {
				Assert.NotNull( slimLock );
				Assert.False( slimLock.IsReadLockHeld );
				Assert.False( slimLock.IsUpgradeableReadLockHeld );
				Assert.False( slimLock.IsWriteLockHeld );
				this._readerWriterLockSlim = slimLock;
				slimLock.EnterReadLock();
			}

			public void Dispose() {
				var slim = this._readerWriterLockSlim;

				if ( slim.IsReadLockHeld ) {
					slim.ExitReadLock();
				}

				this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
			}
		}

		private sealed class UpgradeLockToken : IDisposable {

			private ReaderWriterLockSlim _readerWriterLockSlim;

			public UpgradeLockToken( [NotNull] ReaderWriterLockSlim slimLock ) {
				this._readerWriterLockSlim = slimLock;
				slimLock.EnterUpgradeableReadLock();
			}

			public void Dispose() {
				var slim = this._readerWriterLockSlim;

				if ( slim.IsUpgradeableReadLockHeld ) {
					slim.ExitUpgradeableReadLock();
				}

				this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
			}
		}

		private sealed class WriteLockToken : IDisposable {

			private ReaderWriterLockSlim _readerWriterLockSlim;

			public WriteLockToken( [NotNull] ReaderWriterLockSlim slimLock ) {
				this._readerWriterLockSlim = slimLock;
				slimLock.EnterWriteLock();
			}

			public void Dispose() {
				var slim = this._readerWriterLockSlim;

				if ( slim.IsWriteLockHeld ) {
					slim.ExitWriteLock();
				}

				this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
			}
		}

		public sealed class Manager : IDisposable {

			private Boolean _isDisposed;

			private LockTypes _lockType = LockTypes.None;

			private ReaderWriterLockSlim SlimLock { get; }

			public Manager( [NotNull] ReaderWriterLockSlim slimLock ) {
				Assert.NotNull( slimLock );
				this.SlimLock = slimLock;
			}

			private enum LockTypes {

				None,

				Read,

				Write
			}

			private void Dispose( Boolean freeManagedObjectsAlso ) {
				if ( !this._isDisposed ) {
					if ( freeManagedObjectsAlso ) {
						switch ( this._lockType ) {
							case LockTypes.Read:
								this.SlimLock.ExitReadLock();

								break;

							case LockTypes.Write:
								this.SlimLock.ExitWriteLock();

								break;
						}
					}
				}

				this._isDisposed = true;
			}

			public void Dispose() {
				this.Dispose( true );

				// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
				GC.SuppressFinalize( this );

				// ReSharper restore GCSuppressFinalizeForTypeWithoutDestructor
			}

			public void EnterReadLock() {
				this.SlimLock.EnterReadLock();
				this._lockType = LockTypes.Read;
			}

			public void EnterWriteLock() {
				this.SlimLock.EnterWriteLock();
				this._lockType = LockTypes.Write;
			}
		}
	}
}