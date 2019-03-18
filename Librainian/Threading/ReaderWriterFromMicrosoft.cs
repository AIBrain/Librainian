// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "ReaderWriterFromMicrosoft.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "ReaderWriterFromMicrosoft.cs" was last formatted by Protiguous on 2019/03/17 at 8:31 PM.

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>
    ///     A reader-writer lock implementation that is intended to be simple, yet very
    ///     efficient.  In particular only 1 interlocked operation is taken for any lock
    ///     operation (we use spin locks to achieve this).  The spin lock is never held
    ///     for more than a few instructions (in particular, we never call event APIs
    ///     or in fact any non-trivial API while holding the spin lock).
    /// </summary>
    [HostProtection( SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true )]
    [HostProtection( MayLeakOnAbort = true )]
    public class ReaderWriterFromMicrosoft : IDisposable {

        public void Dispose() => this.Dispose( true );

        private readonly Int64 lockID;

        private Boolean _isDisposed;

        private Boolean fNoWaiters;

        private Boolean fUpgradeThreadHoldingRead;

        private UInt32 numReadWaiters;

        private UInt32 numUpgradeWaiters;

        private UInt32 numWriteUpgradeWaiters;

        //The variables controlling spinning behavior of Mylock(which is a spin-lock)
        // These variables allow use to avoid Setting events (which is expensive) if we don't have to.
        private UInt32 numWriteWaiters;

        //The uint, that contains info like if the writer lock is held, num of
        //readers etc.
        private UInt32 owners;

        private EventWaitHandle readEvent;

        private EventWaitHandle upgradeEvent;

        // maximum number of threads that can be doing a WaitOne on the upgradeEvent (at most 1).
        private Int32 upgradeLockOwnerId;

        private EventWaitHandle waitUpgradeEvent;

        // conditions we wait on.
        private EventWaitHandle writeEvent;

        // maximum number of threads that can be doing a WaitOne on the readEvent
        private Int32 writeLockOwnerId;

        public Int32 CurrentReadCount {
            get {
                var numreaders = ( Int32 ) this.GetNumReaders();

                if ( this.upgradeLockOwnerId != -1 ) {
                    return numreaders - 1;
                }

                return numreaders;
            }
        }

        public Boolean IsReadLockHeld {
            get {
                if ( this.RecursiveReadCount > 0 ) {
                    return true;
                }

                return false;
            }
        }

        public Boolean IsUpgradeableReadLockHeld {
            get {
                if ( this.RecursiveUpgradeCount > 0 ) {
                    return true;
                }

                return false;
            }
        }

        public Boolean IsWriteLockHeld {
            get {
                if ( this.RecursiveWriteCount > 0 ) {
                    return true;
                }

                return false;
            }
        }

        public LockRecursionPolicy RecursionPolicy => LockRecursionPolicy.SupportsRecursion;

        public Int32 RecursiveReadCount {
            get {
                var count = 0;
                var lrwc = this.GetThreadRWCount( true );

                if ( lrwc != null ) {
                    count = lrwc.readercount;
                }

                return count;
            }
        }

        public Int32 RecursiveUpgradeCount {
            get {
                if ( true ) {
                    var count = 0;

                    var lrwc = this.GetThreadRWCount( true );

                    if ( lrwc != null ) {
                        count = lrwc.upgradecount;
                    }

                    return count;
                }
            }
        }

        public Int32 RecursiveWriteCount {
            get {

                if ( true ) {
                    var count = 0;

                    var lrwc = this.GetThreadRWCount( true );

                    if ( lrwc != null ) {
                        count = lrwc.writercount;
                    }

                    return count;
                }
            }
        }

        public Int32 WaitingReadCount => ( Int32 ) this.numReadWaiters;

        public Int32 WaitingUpgradeCount => ( Int32 ) this.numUpgradeWaiters;

        public Int32 WaitingWriteCount => ( Int32 ) this.numWriteWaiters;

        private const Int32 LockSleep0Count = 5;

        private const Int32 LockSpinCount = 10;

        private const Int32 LockSpinCycles = 20;

        //The max readers is actually one less than it's theoretical max.
        //This is done in order to prevent reader count overflows. If the reader
        //count reaches max, other readers will wait.
        private const UInt32 MAX_READER = 0x10000000 - 2;

        private const Int32 MaxSpinCount = 20;

        private const UInt32 READER_MASK = 0x10000000 - 1;

        private const UInt32 WAITING_UPGRADER = 0x20000000;

        private const UInt32 WAITING_WRITERS = 0x40000000;

        private const UInt32 WRITER_HELD = 0x80000000;

        // Every lock instance has a unique ID, which is used by ReaderWriterCount to associate itself with the lock
        // without holding a reference to it.
        private static Int64 NextLockID;

        // See comments on ReaderWriterCount.
        [ThreadStatic]
        private static ReaderWriterCount t_rwc;

        public ReaderWriterFromMicrosoft() {

            this.InitializeThreadCounts();
            this.fNoWaiters = true;
            this.lockID = Interlocked.Increment( ref NextLockID );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static Boolean IsRWEntryEmpty( [NotNull] ReaderWriterCount rwc ) {
            if ( rwc == null ) {
                throw new ArgumentNullException( paramName: nameof( rwc ) );
            }

            if ( rwc.lockID == 0 ) {
                return true;
            }

            if ( rwc.readercount == 0 && rwc.writercount == 0 && rwc.upgradecount == 0 ) {
                return true;
            }

            return false;
        }

        private static void SpinWait( Int32 SpinCount ) {

            //Exponential backoff
            if ( SpinCount < 5 && Environment.ProcessorCount > 1 ) {
                Thread.SpinWait( LockSpinCycles * SpinCount );
            }
            else if ( SpinCount < MaxSpinCount - 3 ) {
                Thread.Sleep( 0 );
            }
            else {
                Thread.Sleep( 1 );
            }
        }

        private void ClearUpgraderWaiting() => this.owners &= ~WAITING_UPGRADER;

        private void ClearWriterAcquired() => this.owners &= ~WRITER_HELD;

        private void ClearWritersWaiting() => this.owners &= ~WAITING_WRITERS;

        private void Dispose( Boolean disposing ) {
            if ( this.IsWriteLockHeld ) {
                this.ExitWriteLock();
            }

            if ( this.IsUpgradeableReadLockHeld ) {
                this.ExitUpgradeableReadLock();
            }

            if ( this.IsReadLockHeld ) {
                this.ExitReadLock();
            }

            if ( disposing && !this._isDisposed ) {
                if ( this.WaitingReadCount > 0 || this.WaitingUpgradeCount > 0 || this.WaitingWriteCount > 0 ) {
                    throw new SynchronizationLockException( "Incorrect Dispose" );
                }

                if ( this.IsReadLockHeld || this.IsUpgradeableReadLockHeld || this.IsWriteLockHeld ) {
                    throw new SynchronizationLockException( "Incorrect Dispose" );
                }

                if ( this.writeEvent != null ) {
                    this.writeEvent.Close();
                    this.writeEvent = null;
                }

                if ( this.readEvent != null ) {
                    this.readEvent.Close();
                    this.readEvent = null;
                }

                if ( this.upgradeEvent != null ) {
                    this.upgradeEvent.Close();
                    this.upgradeEvent = null;
                }

                if ( this.waitUpgradeEvent != null ) {
                    this.waitUpgradeEvent.Close();
                    this.waitUpgradeEvent = null;
                }

                this._isDisposed = true;
            }
        }

        /// <summary>
        ///     Determines the appropriate events to set, leaves the locks, and sets the events.
        /// </summary>
        private void ExitAndWakeUpAppropriateWaiters() {

            if ( !this.fNoWaiters ) {
                this.ExitAndWakeUpAppropriateWaitersPreferringWriters();
            }
        }

        private void ExitAndWakeUpAppropriateWaitersPreferringWriters() {
            var setUpgradeEvent = false;
            var setReadEvent = false;
            var readercount = this.GetNumReaders();

            //We need this case for EU->ER->EW case, as the read count will be 2 in
            //that scenario.
            if ( true ) {
                if ( this.numWriteUpgradeWaiters > 0 && this.fUpgradeThreadHoldingRead && readercount == 2 ) {
                    this.waitUpgradeEvent.Set(); // release all upgraders (however there can be at most one).

                    return;
                }
            }

            if ( readercount == 1 && this.numWriteUpgradeWaiters > 0 ) {

                //We have to be careful now, as we are droppping the lock.
                //No new writes should be allowed to sneak in if an upgrade
                //was pending.

                this.waitUpgradeEvent.Set(); // release all upgraders (however there can be at most one).
            }
            else if ( readercount == 0 && this.numWriteWaiters > 0 ) {
                this.writeEvent.Set(); // release one writer.
            }
            else {
                if ( this.numReadWaiters != 0 || this.numUpgradeWaiters != 0 ) {
                    if ( this.numReadWaiters != 0 ) {
                        setReadEvent = true;
                    }

                    if ( this.numUpgradeWaiters != 0 && this.upgradeLockOwnerId == -1 ) {
                        setUpgradeEvent = true;
                    }

                    if ( setReadEvent ) {
                        this.readEvent.Set(); // release all readers.
                    }

                    if ( setUpgradeEvent ) {
                        this.upgradeEvent.Set(); //release one upgrader.
                    }
                }
            }
        }

        private UInt32 GetNumReaders() => this.owners & READER_MASK;

        /// <summary>
        ///     This routine retrieves/sets the per-thread counts needed to enforce the
        ///     various rules related to acquiring the lock.
        ///     DontAllocate is set to true if the caller just wants to get an existing
        ///     entry for this thread, but doesn't want to add one if an existing one
        ///     could not be found.
        /// </summary>
        [CanBeNull]
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private ReaderWriterCount GetThreadRWCount( Boolean dontAllocate ) {
            var rwc = t_rwc;
            ReaderWriterCount empty = null;

            while ( rwc != null ) {
                if ( rwc.lockID == this.lockID ) {
                    return rwc;
                }

                if ( !dontAllocate && empty == null && IsRWEntryEmpty( rwc ) ) {
                    empty = rwc;
                }

                rwc = rwc.next;
            }

            if ( dontAllocate ) {
                return null;
            }

            if ( empty == null ) {
                empty = new ReaderWriterCount {
                    next = t_rwc
                };

                t_rwc = empty;
            }

            empty.lockID = this.lockID;

            return empty;
        }

        // thread waiting to upgrade from the upgrade lock to a write lock go here (at most one)
        private void InitializeThreadCounts() {
            this.upgradeLockOwnerId = -1;
            this.writeLockOwnerId = -1;
        }

        // thread waiting to acquire the upgrade lock
        private Boolean IsRwHashEntryChanged( [NotNull] ReaderWriterCount lrwc ) => lrwc.lockID != this.lockID;

        private Boolean IsWriterAcquired() => ( this.owners & ~WAITING_WRITERS ) == 0;

        /// <summary>
        ///     A routine for lazily creating a event outside the lock (so if errors
        ///     happen they are outside the lock and that we don't do much work
        ///     while holding a spin lock).  If all goes well, reenter the lock and
        ///     set 'waitEvent'
        /// </summary>
        private void LazyCreateEvent( [NotNull] ref EventWaitHandle waitEvent, Boolean makeAutoResetEvent ) {
            if ( waitEvent == null ) {
                throw new ArgumentNullException( paramName: nameof( waitEvent ) );
            }

            EventWaitHandle newEvent;

            if ( makeAutoResetEvent ) {
                newEvent = new AutoResetEvent( false );
            }
            else {
                newEvent = new ManualResetEvent( false );
            }

            if ( waitEvent == null ) // maybe someone snuck in.
            {
                waitEvent = newEvent;
            }
            else {
                newEvent.Close();
            }
        }

        private void SetUpgraderWaiting() => this.owners |= WAITING_UPGRADER;

        private void SetWriterAcquired() => this.owners |= WRITER_HELD;

        // indicate we have a writer.
        private void SetWritersWaiting() => this.owners |= WAITING_WRITERS;

        private Boolean TryEnterReadLock( TimeoutTracker timeout ) {
            Thread.BeginCriticalRegion();
            var result = false;

            try {
                result = this.TryEnterReadLockCore( timeout );
            }
            finally {
                if ( !result ) {
                    Thread.EndCriticalRegion();
                }
            }

            return result;
        }

        private Boolean TryEnterReadLockCore( TimeoutTracker timeout ) {
            if ( this._isDisposed ) {
                throw new ObjectDisposedException( null );
            }

            var id = Thread.CurrentThread.ManagedThreadId;

            var lrwc = this.GetThreadRWCount( false );

            if ( lrwc != null && lrwc.readercount > 0 ) {
                lrwc.readercount++;

                return true;
            }

            if ( id == this.upgradeLockOwnerId ) {

                //The upgrade lock is already held.
                //Update the global read counts and exit.
                if ( lrwc != null ) {
                    lrwc.readercount++;
                }

                this.owners++;

                this.fUpgradeThreadHoldingRead = true;

                return true;
            }

            if ( id == this.writeLockOwnerId ) {

                //The write lock is already held.
                //Update global read counts here,
                if ( lrwc != null ) {
                    lrwc.readercount++;
                }

                this.owners++;

                return true;
            }

            var spincount = 0;

            for ( ;; ) {

                // We can enter a read lock if there are only read-locks have been given out
                // and a writer is not trying to get in.

                if ( this.owners < MAX_READER ) {

                    // Good case, there is no contention, we are basically done
                    this.owners++; // Indicate we have another reader

                    if ( lrwc != null ) {
                        lrwc.readercount++;
                    }

                    break;
                }

                if ( spincount < MaxSpinCount ) {

                    if ( timeout.IsExpired ) {
                        return false;
                    }

                    spincount++;
                    SpinWait( spincount );

                    //The per-thread structure may have been recycled as the lock is acquired (due to message pumping), load again.
                    if ( this.IsRwHashEntryChanged( lrwc ) ) {
                        lrwc = this.GetThreadRWCount( false );
                    }

                    continue;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait.
                if ( this.readEvent == null ) // Create the needed event
                {
                    this.LazyCreateEvent( ref this.readEvent, false );

                    if ( this.IsRwHashEntryChanged( lrwc ) ) {
                        lrwc = this.GetThreadRWCount( false );
                    }

                    continue; // since we left the lock, start over.
                }

                if ( !this.WaitOnEvent( this.readEvent, ref this.numReadWaiters, timeout ) ) {
                    return false;
                }

                if ( this.IsRwHashEntryChanged( lrwc ) ) {
                    lrwc = this.GetThreadRWCount( false );
                }
            }

            return true;
        }

        private Boolean TryEnterUpgradeableReadLock( TimeoutTracker timeout ) {

            Thread.BeginCriticalRegion();

            var result = false;

            try {
                result = this.TryEnterUpgradeableReadLockCore( timeout );

                return result;
            }
            finally {

                if ( !result ) {
                    Thread.EndCriticalRegion();
                }
            }
        }

        private Boolean TryEnterUpgradeableReadLockCore( TimeoutTracker timeout ) {
            if ( this._isDisposed ) {
                throw new ObjectDisposedException( null );
            }

            var id = Thread.CurrentThread.ManagedThreadId;

            var lrwc = this.GetThreadRWCount( false );

            if ( id == this.upgradeLockOwnerId ) {
                if ( lrwc != null ) {
                    lrwc.upgradecount++;
                }

                return true;
            }

            if ( id == this.writeLockOwnerId ) {

                //Write lock is already held, Just update the global state
                //to show presence of upgrader.
                Debug.Assert( ( this.owners & WRITER_HELD ) > 0 );
                this.owners++;
                this.upgradeLockOwnerId = id;

                if ( lrwc != null ) {
                    lrwc.upgradecount++;

                    if ( lrwc.readercount > 0 ) {
                        this.fUpgradeThreadHoldingRead = true;
                    }
                }

                return true;
            }

            if ( lrwc != null && lrwc.readercount > 0 ) {

                //Upgrade locks may not be acquired if only read locks have been
                //acquired.

                throw new LockRecursionException( "Upgrade After Read Not Allowed" );
            }

            var spincount = 0;

            for ( ;; ) {

                //Once an upgrade lock is taken, it's like having a reader lock held
                //until upgrade or downgrade operations are performed.

                if ( this.upgradeLockOwnerId == -1 && this.owners < MAX_READER ) {
                    this.owners++;
                    this.upgradeLockOwnerId = id;

                    break;
                }

                if ( spincount < MaxSpinCount ) {

                    if ( timeout.IsExpired ) {
                        return false;
                    }

                    spincount++;
                    SpinWait( spincount );

                    continue;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait.
                if ( this.upgradeEvent == null ) // Create the needed event
                {
                    this.LazyCreateEvent( ref this.upgradeEvent, true );

                    continue; // since we left the lock, start over.
                }

                //Only one thread with the upgrade lock held can proceed.
                if ( !this.WaitOnEvent( this.upgradeEvent, ref this.numUpgradeWaiters, timeout ) ) {
                    return false;
                }
            }

            if ( true ) {

                //The lock may have been dropped getting here, so make a quick check to see whether some other
                //thread did not grab the entry.
                if ( this.IsRwHashEntryChanged( lrwc ) ) {
                    lrwc = this.GetThreadRWCount( false );
                }

                lrwc.upgradecount++;
            }

            return true;
        }

        private Boolean TryEnterWriteLock( TimeoutTracker timeout ) {

            Thread.BeginCriticalRegion();

            var result = false;

            try {
                result = this.TryEnterWriteLockCore( timeout );
            }
            finally {

                if ( !result ) {
                    Thread.EndCriticalRegion();
                }
            }

            return result;
        }

        private Boolean TryEnterWriteLockCore( TimeoutTracker timeout ) {
            if ( this._isDisposed ) {
                throw new ObjectDisposedException( null );
            }

            var id = Thread.CurrentThread.ManagedThreadId;
            ReaderWriterCount lrwc;
            var upgradingToWrite = false;

            if ( true ) {

                lrwc = this.GetThreadRWCount( false );

                if ( id == this.writeLockOwnerId ) {
                    lrwc.writercount++;

                    return true;
                }

                if ( id == this.upgradeLockOwnerId ) {
                    upgradingToWrite = true;
                }
                else if ( lrwc.readercount > 0 ) {

                    //Write locks may not be acquired if only read locks have been
                    //acquired.

                    throw new LockRecursionException( "Write After Read Not Allowed" );
                }
            }

            var spincount = 0;

            for ( ;; ) {
                if ( this.IsWriterAcquired() ) {

                    // Good case, there is no contention, we are basically done
                    this.SetWriterAcquired();

                    break;
                }

                //Check if there is just one upgrader, and no readers.
                //Assumption: Only one thread can have the upgrade lock, so the
                //following check will fail for all other threads that may sneak in
                //when the upgrading thread is waiting.

                if ( upgradingToWrite ) {
                    var readercount = this.GetNumReaders();

                    if ( readercount == 1 ) {

                        //Good case again, there is just one upgrader, and no readers.
                        this.SetWriterAcquired(); // indicate we have a writer.

                        break;
                    }

                    if ( readercount == 2 ) {
                        if ( lrwc != null ) {
                            if ( this.IsRwHashEntryChanged( lrwc ) ) {
                                lrwc = this.GetThreadRWCount( false );
                            }

                            if ( lrwc.readercount > 0 ) {

                                //This check is needed for EU->ER->EW case, as the owner count will be two.
                                Debug.Assert( true );
                                Debug.Assert( this.fUpgradeThreadHoldingRead );

                                //Good case again, there is just one upgrader, and no readers.
                                this.SetWriterAcquired(); // indicate we have a writer.

                                break;
                            }
                        }
                    }
                }

                if ( spincount < MaxSpinCount ) {

                    if ( timeout.IsExpired ) {
                        return false;
                    }

                    spincount++;
                    SpinWait( spincount );

                    continue;
                }

                Boolean retVal;

                if ( upgradingToWrite ) {
                    if ( this.waitUpgradeEvent == null ) // Create the needed event
                    {
                        this.LazyCreateEvent( ref this.waitUpgradeEvent, true );

                        continue; // since we left the lock, start over.
                    }

                    Debug.Assert( this.numWriteUpgradeWaiters == 0, "There can be at most one thread with the upgrade lock held." );

                    retVal = this.WaitOnEvent( this.waitUpgradeEvent, ref this.numWriteUpgradeWaiters, timeout );

                    //The lock is not held in case of failure.
                    if ( !retVal ) {
                        return false;
                    }
                }
                else {

                    // Drat, we need to wait.  Mark that we have waiters and wait.
                    if ( this.writeEvent == null ) // create the needed event.
                    {
                        this.LazyCreateEvent( ref this.writeEvent, true );

                        continue; // since we left the lock, start over.
                    }

                    retVal = this.WaitOnEvent( this.writeEvent, ref this.numWriteWaiters, timeout );

                    //The lock is not held in case of failure.
                    if ( !retVal ) {
                        return false;
                    }
                }
            }

            Debug.Assert( ( this.owners & WRITER_HELD ) > 0 );

            if ( true ) {
                if ( this.IsRwHashEntryChanged( lrwc ) ) {
                    lrwc = this.GetThreadRWCount( false );
                }

                lrwc.writercount++;
            }

            this.writeLockOwnerId = id;

            return true;
        }

        /// <summary>
        ///     Waits on 'waitEvent' with a timeout
        ///     Before the wait 'numWaiters' is incremented and is restored before leaving this routine.
        /// </summary>
        private Boolean WaitOnEvent( [NotNull] EventWaitHandle waitEvent, ref UInt32 numWaiters, TimeoutTracker timeout ) {
            if ( waitEvent == null ) {
                throw new ArgumentNullException( paramName: nameof( waitEvent ) );
            }

            waitEvent.Reset();
            numWaiters++;
            this.fNoWaiters = false;

            //Setting these bits will prevent new readers from getting in.
            if ( this.numWriteWaiters == 1 ) {
                this.SetWritersWaiting();
            }

            if ( this.numWriteUpgradeWaiters == 1 ) {
                this.SetUpgraderWaiting();
            }

            var waitSuccessful = false;

            // Do the wait outside of any lock

            try {
                waitSuccessful = waitEvent.WaitOne( timeout.RemainingMilliseconds );
            }
            finally {

                --numWaiters;

                if ( this.numWriteWaiters == 0 && this.numWriteUpgradeWaiters == 0 && this.numUpgradeWaiters == 0 && this.numReadWaiters == 0 ) {
                    this.fNoWaiters = true;
                }

                if ( this.numWriteWaiters == 0 ) {
                    this.ClearWritersWaiting();
                }

                if ( this.numWriteUpgradeWaiters == 0 ) {
                    this.ClearUpgraderWaiting();
                }

                if ( !waitSuccessful ) // We may also be aboutto throw for some reason.  Exit myLock.
                { }
            }

            return waitSuccessful;
        }

        // threads waiting to acquire a read lock go here (will be released in bulk)
        public void EnterReadLock() => this.TryEnterReadLock( -1 );

        public void EnterUpgradeableReadLock() => this.TryEnterUpgradeableReadLock( -1 );

        public void EnterWriteLock() => this.TryEnterWriteLock( -1 );

        public void ExitReadLock() {

            var lrwc = this.GetThreadRWCount( true );

            if ( lrwc == null || lrwc.readercount < 1 ) {

                //You have to be holding the read lock to make this call.

                throw new SynchronizationLockException( "Mismatched Read" );
            }

            if ( true ) {
                if ( lrwc.readercount > 1 ) {
                    lrwc.readercount--;

                    Thread.EndCriticalRegion();

                    return;
                }

                if ( Thread.CurrentThread.ManagedThreadId == this.upgradeLockOwnerId ) {
                    this.fUpgradeThreadHoldingRead = false;
                }
            }

            Debug.Assert( this.owners > 0, "ReleasingReaderLock: releasing lock and no read lock taken" );

            --this.owners;

            Debug.Assert( lrwc.readercount == 1 );
            lrwc.readercount--;

            this.ExitAndWakeUpAppropriateWaiters();

            Thread.EndCriticalRegion();
        }

        public void ExitUpgradeableReadLock() {

            var lrwc = this.GetThreadRWCount( true );

            if ( lrwc == null ) {

                throw new SynchronizationLockException( "Mismatched Upgrade" );
            }

            if ( lrwc.upgradecount < 1 ) {

                throw new SynchronizationLockException( "Mismatched Upgrade" );
            }

            lrwc.upgradecount--;

            if ( lrwc.upgradecount > 0 ) {

                Thread.EndCriticalRegion();

                return;
            }

            this.fUpgradeThreadHoldingRead = false;

            this.owners--;
            this.upgradeLockOwnerId = -1;

            this.ExitAndWakeUpAppropriateWaiters();

            Thread.EndCriticalRegion();
        }

        public void ExitWriteLock() {

            var lrwc = this.GetThreadRWCount( false );

            if ( lrwc == null ) {

                throw new SynchronizationLockException( "Mismatched Write" );
            }

            if ( lrwc.writercount < 1 ) {

                throw new SynchronizationLockException( "Mismatched Write" );
            }

            lrwc.writercount--;

            if ( lrwc.writercount > 0 ) {

                Thread.EndCriticalRegion();

                return;
            }

            Debug.Assert( ( this.owners & WRITER_HELD ) > 0, "Calling ReleaseWriterLock when no write lock is held" );

            this.ClearWriterAcquired();

            this.writeLockOwnerId = -1;

            this.ExitAndWakeUpAppropriateWaiters();

            Thread.EndCriticalRegion();
        }

        public Boolean TryEnterReadLock( TimeSpan timeout ) => this.TryEnterReadLock( new TimeoutTracker( timeout ) );

        public Boolean TryEnterReadLock( Int32 millisecondsTimeout ) => this.TryEnterReadLock( new TimeoutTracker( millisecondsTimeout ) );

        public Boolean TryEnterUpgradeableReadLock( TimeSpan timeout ) => this.TryEnterUpgradeableReadLock( new TimeoutTracker( timeout ) );

        public Boolean TryEnterUpgradeableReadLock( Int32 millisecondsTimeout ) => this.TryEnterUpgradeableReadLock( new TimeoutTracker( millisecondsTimeout ) );

        public Boolean TryEnterWriteLock( TimeSpan timeout ) => this.TryEnterWriteLock( new TimeoutTracker( timeout ) );

        public Boolean TryEnterWriteLock( Int32 millisecondsTimeout ) => this.TryEnterWriteLock( new TimeoutTracker( millisecondsTimeout ) );

        // maximum number of threads that can be doing a WaitOne on the writeEvent
        // threads waiting to acquire a write lock go here.
        private struct TimeoutTracker {

            private readonly Int32 _start;

            private readonly Int32 _total;

            public Boolean IsExpired => this.RemainingMilliseconds == 0;

            public Int32 RemainingMilliseconds {
                get {
                    if ( this._total == -1 || this._total == 0 ) {
                        return this._total;
                    }

                    var elapsed = Environment.TickCount - this._start;

                    // elapsed may be negative if TickCount has overflowed by 2^31 milliseconds.
                    if ( elapsed < 0 || elapsed >= this._total ) {
                        return 0;
                    }

                    return this._total - elapsed;
                }
            }

            public TimeoutTracker( TimeSpan timeout ) {
                var ltm = ( Int64 ) timeout.TotalMilliseconds;

                if ( ltm < -1 || ltm > Int32.MaxValue ) {
                    throw new ArgumentOutOfRangeException( nameof( timeout ) );
                }

                this._total = ( Int32 ) ltm;

                if ( this._total != -1 && this._total != 0 ) {
                    this._start = Environment.TickCount;
                }
                else {
                    this._start = 0;
                }
            }

            public TimeoutTracker( Int32 millisecondsTimeout ) {
                if ( millisecondsTimeout < -1 ) {
                    throw new ArgumentOutOfRangeException( nameof( millisecondsTimeout ) );
                }

                this._total = millisecondsTimeout;

                if ( this._total != -1 && this._total != 0 ) {
                    this._start = Environment.TickCount;
                }
                else {
                    this._start = 0;
                }
            }

        }

        /// <summary>
        ///     ReaderWriterCount tracks how many of each kind of lock is held by each thread.
        ///     We keep a linked list for each thread, attached to a ThreadStatic field.
        ///     These are reused wherever possible, so that a given thread will only
        ///     allocate N of these, where N is the maximum number of locks held simultaneously
        ///     by that thread.
        /// </summary>
        internal class ReaderWriterCount {

            // Which lock does this object belong to?  This is a numeric ID for two reasons:
            // 1) We don't want this field to keep the lock object alive, and a WeakReference would
            //    be too expensive.
            // 2) Setting the value of a long is faster than setting the value of a reference.
            //    The "hot" paths in ReaderWriterLockSlim are short enough that this actually
            //    matters.
            public Int64 lockID;

            // Next RWC in this thread's list.
            public ReaderWriterCount next;

            // How many reader locks does this thread hold on this ReaderWriterLockSlim instance?
            public Int32 readercount;

            public Int32 upgradecount;

            // Ditto for writer/upgrader counts.  These are only used if the lock allows recursion.
            // But we have to have the fields on every ReaderWriterCount instance, because
            // we reuse it for different locks.
            public Int32 writercount;

        }

    }

}