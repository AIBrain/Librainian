// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/Locks.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM

namespace Librainian.Extensions {

    using System;
    using System.Threading;
    using NUnit.Framework;

    public static class Locks {

        /// <summary>
        /// put a Read ( will-read ) lock on the access object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <example>
        /// ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }
        /// </example>
        public static IDisposable Read( this ReaderWriterLockSlim obj ) => new ReadLockToken( obj );

        /// <summary>
        /// put an upgradeable ( will-read / may-write ) lock on the access object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <example>
        /// ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }
        /// </example>
        public static IDisposable Upgrade( this ReaderWriterLockSlim obj ) => new UpgradeLockToken( obj );

        /// <summary>
        /// put a Write ( will-write ) lock on the access object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <example>
        /// ReaderWriterLockSlim sync = new ReaderWriterLockSlim(); using (sync.Read()) { ... }
        /// </example>
        public static IDisposable Write( this ReaderWriterLockSlim obj ) => new WriteLockToken( obj );

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
        }

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
                if ( slim != null && slim.IsReadLockHeld ) {
                    slim.ExitReadLock();
                }
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
                if ( slim != null && slim.IsUpgradeableReadLockHeld ) {
                    slim.ExitUpgradeableReadLock();
                }
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
                if ( slim != null && slim.IsWriteLockHeld ) {
                    slim.ExitWriteLock();
                }
                this._readerWriterLockSlim = null; //don't hold a ref to the lock anymore.
            }
        }
    }
}