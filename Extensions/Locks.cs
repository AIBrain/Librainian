// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Locks.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Locks.cs" was last cleaned by Protiguous on 2018/05/15 at 10:40 PM.

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

            private ReaderWriterLockSlim _readerWriterLockSlim;

            public ReadLockToken( ReaderWriterLockSlim slimLock ) {
                Assert.NotNull( slimLock );
                Assert.False( slimLock.IsReadLockHeld );
                Assert.False( slimLock.IsUpgradeableReadLockHeld );
                Assert.False( slimLock.IsWriteLockHeld );
                this._readerWriterLockSlim = slimLock;
                slimLock.EnterReadLock();
            }

            public void Dispose() {
                var slim = this._readerWriterLockSlim;

                if ( slim?.IsReadLockHeld ) { slim.ExitReadLock(); }

                this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
            }
        }

        private sealed class UpgradeLockToken : IDisposable {

            private ReaderWriterLockSlim _readerWriterLockSlim;

            public UpgradeLockToken( ReaderWriterLockSlim slimLock ) {
                this._readerWriterLockSlim = slimLock;
                slimLock.EnterUpgradeableReadLock();
            }

            public void Dispose() {
                var slim = this._readerWriterLockSlim;

                if ( slim?.IsUpgradeableReadLockHeld ) { slim.ExitUpgradeableReadLock(); }

                this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
            }
        }

        private sealed class WriteLockToken : IDisposable {

            private ReaderWriterLockSlim _readerWriterLockSlim;

            public WriteLockToken( ReaderWriterLockSlim slimLock ) {
                this._readerWriterLockSlim = slimLock;
                slimLock.EnterWriteLock();
            }

            public void Dispose() {
                var slim = this._readerWriterLockSlim;

                if ( slim != null && slim.IsWriteLockHeld ) { slim.ExitWriteLock(); }

                this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
            }
        }

        public sealed class Manager : IDisposable {

            private readonly ReaderWriterLockSlim _slimLock;

            private Boolean _isDisposed;

            private LockTypes _lockType = LockTypes.None;

            public Manager( ReaderWriterLockSlim slimLock ) {
                Assert.NotNull( slimLock );
                this._slimLock = slimLock;
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
                                this._slimLock.ExitReadLock();

                                break;

                            case LockTypes.Write:
                                this._slimLock.ExitWriteLock();

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
                this._slimLock.EnterReadLock();
                this._lockType = LockTypes.Read;
            }

            public void EnterWriteLock() {
                this._slimLock.EnterWriteLock();
                this._lockType = LockTypes.Write;
            }
        }
    }
}