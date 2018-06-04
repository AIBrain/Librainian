// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Locks.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "Locks.cs" was last formatted by Protiguous on 2018/06/04 at 3:52 PM.

namespace Librainian.Extensions {

	using System;
	using System.Threading;
	using NUnit.Framework;

	public static class Locks {

		/// <summary>
		///     put a Read ( will-read ) lock on the access object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <example>ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }</example>
		public static IDisposable Read( this ReaderWriterLockSlim obj ) => new ReadLockToken( obj );

		/// <summary>
		///     put an upgradeable ( will-read / may-write ) lock on the access object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <example>ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }</example>
		public static IDisposable Upgrade( this ReaderWriterLockSlim obj ) => new UpgradeLockToken( obj );

		/// <summary>
		///     put a Write ( will-write ) lock on the access object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <example>ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }</example>
		public static IDisposable Write( this ReaderWriterLockSlim obj ) => new WriteLockToken( obj );

		private sealed class ReadLockToken : IDisposable {

			public void Dispose() {
				var slim = this._readerWriterLockSlim;

				if ( slim.IsReadLockHeld ) { slim.ExitReadLock(); }

				this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
			}

			private ReaderWriterLockSlim _readerWriterLockSlim;

			public ReadLockToken( ReaderWriterLockSlim slimLock ) {
				Assert.NotNull( slimLock );
				Assert.False( slimLock.IsReadLockHeld );
				Assert.False( slimLock.IsUpgradeableReadLockHeld );
				Assert.False( slimLock.IsWriteLockHeld );
				this._readerWriterLockSlim = slimLock;
				slimLock.EnterReadLock();
			}

		}

		private sealed class UpgradeLockToken : IDisposable {

			public void Dispose() {
				var slim = this._readerWriterLockSlim;

				if ( slim.IsUpgradeableReadLockHeld ) { slim.ExitUpgradeableReadLock(); }

				this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
			}

			private ReaderWriterLockSlim _readerWriterLockSlim;

			public UpgradeLockToken( ReaderWriterLockSlim slimLock ) {
				this._readerWriterLockSlim = slimLock;
				slimLock.EnterUpgradeableReadLock();
			}

		}

		private sealed class WriteLockToken : IDisposable {

			public void Dispose() {
				var slim = this._readerWriterLockSlim;

				if ( slim.IsWriteLockHeld ) { slim.ExitWriteLock(); }

				this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
			}

			private ReaderWriterLockSlim _readerWriterLockSlim;

			public WriteLockToken( ReaderWriterLockSlim slimLock ) {
				this._readerWriterLockSlim = slimLock;
				slimLock.EnterWriteLock();
			}

		}

		public sealed class Manager : IDisposable {

			public void Dispose() {
				this.Dispose( true );

				// ReSharper disable GCSuppressFinalizeForTypeWithoutDestructor
				GC.SuppressFinalize( this );

				// ReSharper restore GCSuppressFinalizeForTypeWithoutDestructor
			}

			private ReaderWriterLockSlim SlimLock { get; }

			private enum LockTypes {

				None,

				Read,

				Write

			}

			private Boolean _isDisposed;

			private LockTypes _lockType = LockTypes.None;

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

			public void EnterReadLock() {
				this.SlimLock.EnterReadLock();
				this._lockType = LockTypes.Read;
			}

			public void EnterWriteLock() {
				this.SlimLock.EnterWriteLock();
				this._lockType = LockTypes.Write;
			}

			public Manager( ReaderWriterLockSlim slimLock ) {
				Assert.NotNull( slimLock );
				this.SlimLock = slimLock;
			}

		}

	}

}