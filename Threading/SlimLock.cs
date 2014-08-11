#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/SlimLock.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Security.Permissions;
    using System.Threading;
    using NUnit.Framework;

    /// <summary>
    ///     A reader-writer lock implementation that is intended to be simple, yet very efficient.
    ///     In particular only 1 interlocked operation is taken for any lock operation (we use spin locks to achieve this).
    ///     The spin lock is never held for more than a few instructions
    ///     (in particular, we never call event APIs or in fact any non-trivial API while holding the spin lock).
    /// </summary>
    [HostProtection( Synchronization = true, ExternalThreading = true )]
    [HostProtection( MayLeakOnAbort = true )]
    public class SlimLock : IDisposable {
        //Specifying if locked can be reacquired recursively. 
        private const int LockSpinCycles = 20;
        private const int LockSpinCount = 10;
        private const int LockSleep0Count = 5;
        private const int hashTableSize = 0xff;
        private const int MaxSpinCount = 20;
        private const uint WriterHeld = 0x80000000;
        private const uint WaitingWriters = 0x40000000;
        private const uint WaitingUpgrader = 0x20000000;
        private const uint MAX_READER = 0x10000000 - 2;
        private const uint READER_MASK = 0x10000000 - 1;
        private static readonly int ProcessorCount = Threads.ProcessorCount;
        private readonly Boolean _fIsReentrant;

        // Lock specifiation for myLock:  This lock protects exactly the local fields associted
        // instance of ReaderWriterLockSlim.  It does NOT protect the memory associted with the
        // the events that hang off this lock (eg writeEvent, readEvent upgradeEvent).
        private int _myLock;

        //The variables controlling spinning behaviior of Mylock(which is a spin-lock) 

        // These variables allow use to avoid Setting events (which is expensive) if we don't have to.
        private uint _numWriteWaiters; // maximum number of threads that can be doing a WaitOne on the writeEvent 

        private uint _numReadWaiters; // maximum number of threads that can be doing a WaitOne on the readEvent

        private uint _numWriteUpgradeWaiters; // maximum number of threads that can be doing a WaitOne on the upgradeEvent (at most 1). 

        private uint _numUpgradeWaiters;

        //Variable used for quick check when there are no waiters. 
        private Boolean _fNoWaiters;

        private int _upgradeLockOwnerId;

        private int _writeLockOwnerId;

        // conditions we wait on. 
        private EventWaitHandle _writeEvent; // threads waiting to aquire a write lock go here. 

        private EventWaitHandle _readEvent; // threads waiting to aquire a read lock go here (will be released in bulk)

        private EventWaitHandle _upgradeEvent; // thread waiting to acquire the upgrade lock 

        private EventWaitHandle _waitUpgradeEvent; // thread waiting to upgrade from the upgrade lock to a write lock go here (at most one)

        private ReaderWriterCount[] _rwc;

        private Boolean _fUpgradeThreadHoldingRead;

        //Per thread Hash; 

        //The uint, that contains info like if the writer lock is held, num of
        //readers etc. 
        private uint _owners;

        //Various R/W masks 
        //Note:
        //The Uint is divided as follows: 
        //
        //Writer-Owned  Waiting-Writers   Waiting Upgraders     Num-REaders
        //    31          30                 29                 28.......0
        // 
        //Dividing the uint, allows to vastly simplify logic for checking if a
        //reader should go in etc. Setting the writer bit, will automatically 
        //make the value of the uint much larger than the max num of readers 
        //allowed, thus causing the check for max_readers to fail.

        //The max readers is actually one less then it's theoretical max.
        //This is done in order to prevent reader count overflows. If the reader 
        //count reaches max, other readers will wait. 

        private Boolean _fDisposed;

        public LockRecursionPolicy RecursionPolicy { get { return this._fIsReentrant ? LockRecursionPolicy.SupportsRecursion : LockRecursionPolicy.NoRecursion; } }

        public int CurrentReadCount {
            get {
                var numreaders = ( int ) this.GetNumReaders();

                return this._upgradeLockOwnerId != -1 ? numreaders - 1 : numreaders;
            }
        }

        public Boolean IsReadLockHeld { get { return this.RecursiveReadCount > 0; } }

        public Boolean IsUpgradeableReadLockHeld { get { return this.RecursiveUpgradeCount > 0; } }

        public Boolean IsWriteLockHeld { get { return this.RecursiveWriteCount > 0; } }

        public int RecursiveReadCount {
            get {
                var id = Thread.CurrentThread.ManagedThreadId;
                var count = 0;

                Thread.BeginCriticalRegion();
                this.EnterMyLock();
                var lrwc = this.GetThreadRWCount( id, true );
                if ( lrwc != null ) {
                    count = lrwc.Readercount;
                }
                this.ExitMyLock();
                Thread.EndCriticalRegion();

                return count;
            }
        }

        public int RecursiveUpgradeCount {
            get {
                var id = Thread.CurrentThread.ManagedThreadId;

                if ( !this._fIsReentrant ) {
                    return id == this._upgradeLockOwnerId ? 1 : 0;
                }
                var count = 0;

                Thread.BeginCriticalRegion();
                this.EnterMyLock();
                var lrwc = this.GetThreadRWCount( id, true );
                if ( lrwc != null ) {
                    count = lrwc.RecursiveCounts.upgradecount;
                }
                this.ExitMyLock();
                Thread.EndCriticalRegion();

                return count;
            }
        }

        public int RecursiveWriteCount {
            get {
                var id = Thread.CurrentThread.ManagedThreadId;
                var count = 0;

                if ( !this._fIsReentrant ) {
                    return id == this._writeLockOwnerId ? 1 : 0;
                }
                Thread.BeginCriticalRegion();
                this.EnterMyLock();
                var lrwc = this.GetThreadRWCount( id, true );
                if ( lrwc != null ) {
                    count = lrwc.RecursiveCounts.writercount;
                }
                this.ExitMyLock();
                Thread.EndCriticalRegion();

                return count;
            }
        }

        public int WaitingReadCount { get { return ( int ) this._numReadWaiters; } }

        public int WaitingUpgradeCount { get { return ( int ) this._numUpgradeWaiters; } }

        public int WaitingWriteCount { get { return ( int ) this._numWriteWaiters; } }

        private Boolean MyLockHeld { get { return this._myLock != 0; } }

        public SlimLock() : this( LockRecursionPolicy.NoRecursion ) { }

        public SlimLock( LockRecursionPolicy recursionPolicy ) {
            if ( recursionPolicy == LockRecursionPolicy.SupportsRecursion ) {
                this._fIsReentrant = true;
            }
            this.InitializeThreadCounts();
        }

        private uint GetNumReaders() {
            return this._owners & READER_MASK;
        }

        private void EnterMyLock() {
            if ( Interlocked.CompareExchange( ref this._myLock, 1, 0 ) != 0 ) {
                this.EnterMyLockSpin();
            }
        }

        private void EnterMyLockSpin() {
            var pc = ProcessorCount;
            for ( var i = 0;; i++ ) {
                if ( i < LockSpinCount && pc > 1 ) {
                    Thread.SpinWait( LockSpinCycles*( i + 1 ) ); // Wait a few dozen instructions to let another processor release lock. 
                }
                else if ( i < ( LockSpinCount + LockSleep0Count ) ) {
                    Thread.Sleep( 0 ); // Give up my quantum. 
                }
                else {
                    Thread.Sleep( 1 ); // Give up my quantum.
                }

                if ( this._myLock == 0 && Interlocked.CompareExchange( ref this._myLock, 1, 0 ) == 0 ) {
                    return;
                }
            }
        }

        /// <summary>
        ///     This routine retrieves/sets the per-thread counts needed to enforce the
        ///     various rules related to acquiring the lock. It's a simple hash table,
        ///     where the first entry is pre-allocated for optimizing the common case.
        ///     After the first element has been allocated, duplicates are kept of in
        ///     linked-list. The entries are never freed, and the max size of the
        ///     table would be bounded by the max number of threads that held the lock
        ///     simultaneously.
        ///     DontAllocate is set to true if the caller just wants to get an existing
        ///     entry for this thread, but doesn't want to add one if an existing one
        ///     could not be found.
        /// </summary>
        private ReaderWriterCount GetThreadRWCount( int id, Boolean DontAllocate ) {
            var hash = id & hashTableSize;
            ReaderWriterCount firstfound = null;

            Assert.That( this.MyLockHeld );

            if ( null == this._rwc[ hash ] ) {
                if ( DontAllocate ) {
                    return null;
                }
                this._rwc[ hash ] = new ReaderWriterCount( this._fIsReentrant );
            }

            if ( this._rwc[ hash ].Threadid == id ) {
                return this._rwc[ hash ];
            }

            if ( IsRWEntryEmpty( this._rwc[ hash ] ) && !DontAllocate ) {
                //No more entries in chain, so no more searching required. 
                if ( this._rwc[ hash ].Next == null ) {
                    this._rwc[ hash ].Threadid = id;
                    return this._rwc[ hash ];
                }
                firstfound = this._rwc[ hash ];
            }

            //SlowPath

            var temp = this._rwc[ hash ].Next;

            while ( temp != null ) {
                if ( temp.Threadid == id ) {
                    return temp;
                }

                if ( firstfound == null ) {
                    if ( IsRWEntryEmpty( temp ) ) {
                        firstfound = temp;
                    }
                }

                temp = temp.Next;
            }

            if ( DontAllocate ) {
                return null;
            }

            if ( firstfound == null ) {
                temp = new ReaderWriterCount( this._fIsReentrant ) {
                                                                       Threadid = id,
                                                                       Next = this._rwc[ hash ].Next
                                                                   };
                this._rwc[ hash ].Next = temp;
                return temp;
            }
            firstfound.Threadid = id;
            return firstfound;
        }

        private static Boolean IsRWEntryEmpty( ReaderWriterCount rwc ) {
            return rwc.Threadid == -1 || ( rwc.Readercount == 0 && rwc.RecursiveCounts == null || rwc.Readercount == 0 && rwc.RecursiveCounts.writercount == 0 && rwc.RecursiveCounts.upgradecount == 0 );
        }

        private void ExitMyLock() {
            Assert.That( this._myLock != 0, "Exiting spin lock that is not held" );
            this._myLock = 0;
        }

        private void InitializeThreadCounts() {
            this._rwc = new ReaderWriterCount[hashTableSize + 1];
            this._upgradeLockOwnerId = -1;
            this._writeLockOwnerId = -1;
        }

        public void EnterReadLock() {
            this.TryEnterReadLock( -1 );
        }

        public Boolean TryEnterReadLock( int millisecondsTimeout ) {
            Thread.BeginCriticalRegion();
            var result = false;
            try {
                result = this.TryEnterReadLockCore( millisecondsTimeout );
            }
            finally {
                if ( !result ) {
                    Thread.EndCriticalRegion();
                }
            }
            return result;
        }

        private Boolean TryEnterReadLockCore( int millisecondsTimeout ) {
            if ( millisecondsTimeout < -1 ) {
                throw new ArgumentOutOfRangeException( "millisecondsTimeout" );
            }

            if ( this._fDisposed ) {
                throw new ObjectDisposedException( null );
            }

            ReaderWriterCount lrwc;
            var id = Thread.CurrentThread.ManagedThreadId;

            if ( this._fIsReentrant ) {
                this.EnterMyLock();
                lrwc = this.GetThreadRWCount( id, false );
                if ( lrwc.Readercount > 0 ) {
                    lrwc.Readercount++;
                    this.ExitMyLock();
                    return true;
                }
                if ( id == this._upgradeLockOwnerId ) {
                    //The upgrade lock is already held.
                    //Update the global read counts and exit. 
                    lrwc.Readercount++;
                    this._owners++;
                    this.ExitMyLock();
                    this._fUpgradeThreadHoldingRead = true;
                    return true;
                }
                if ( id == this._writeLockOwnerId ) {
                    //The write lock is already held. 
                    //Update global read counts here,
                    lrwc.Readercount++;
                    this._owners++;
                    this.ExitMyLock();
                    return true;
                }
            }
            else {
                if ( id == this._writeLockOwnerId ) {
                    //Check for AW->AR
                    throw new LockRecursionException();
                }

                this.EnterMyLock();

                lrwc = this.GetThreadRWCount( id, false );

                //Check if the reader lock is already acquired. Note, we could
                //check the presence of a reader by not allocating rwc (But that
                //would lead to two lookups in the common case. It's better to keep 
                //a count in the struucture).
                if ( lrwc.Readercount > 0 ) {
                    this.ExitMyLock();
                    throw new LockRecursionException();
                }
                if ( id == this._upgradeLockOwnerId ) {
                    //The upgrade lock is already held.
                    //Update the global read counts and exit. 

                    lrwc.Readercount++;
                    this._owners++;
                    this.ExitMyLock();
                    return true;
                }
            }

            var spincount = 0;

            for ( ;; ) {
                // We can enter a read lock if there are only read-locks have been given out 
                // and a writer is not trying to get in. 

                if ( this._owners < MAX_READER ) {
                    // Good case, there is no contention, we are basically done
                    this._owners++; // Indicate we have another reader
                    lrwc.Readercount++;
                    break;
                }

                if ( spincount < MaxSpinCount ) {
                    this.ExitMyLock();
                    if ( millisecondsTimeout == 0 ) {
                        return false;
                    }
                    spincount++;
                    SpinWait( spincount );
                    this.EnterMyLock();
                    //The per-thread structure may have been recycled as the lock is released, load again. 
                    if ( IsRwHashEntryChanged( lrwc, id ) ) {
                        lrwc = this.GetThreadRWCount( id, false );
                    }
                    continue;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait. 
                if ( this._readEvent == null ) // Create the needed event
                {
                    this.LazyCreateEvent( ref this._readEvent, false );
                    if ( IsRwHashEntryChanged( lrwc, id ) ) {
                        lrwc = this.GetThreadRWCount( id, false );
                    }
                    continue; // since we left the lock, start over.
                }

                if ( !this.WaitOnEvent( this._readEvent, ref this._numReadWaiters, millisecondsTimeout ) ) {
                    return false;
                }
                if ( IsRwHashEntryChanged( lrwc, id ) ) {
                    lrwc = this.GetThreadRWCount( id, false );
                }
            }

            this.ExitMyLock();
            return true;
        }

        private static void SpinWait( int spinCount ) {
            //Exponential backoff
            if ( spinCount < 5 && ProcessorCount > 1 ) {
                Thread.SpinWait( LockSpinCycles*spinCount );
            }
            else if ( spinCount < MaxSpinCount - 3 ) {
                Thread.Sleep( 0 );
            }
            else {
                Thread.Sleep( 1 );
            }
        }

        private static Boolean IsRwHashEntryChanged( ReaderWriterCount lrwc, int id ) {
            return lrwc.Threadid != id;
        }

        /// <summary>
        ///     A routine for lazily creating a event outside the lock (so if errors
        ///     happen they are outside the lock and that we don't do much work
        ///     while holding a spin lock).  If all goes well, reenter the lock and
        ///     set 'waitEvent'
        /// </summary>
        private void LazyCreateEvent( ref EventWaitHandle waitEvent, Boolean makeAutoResetEvent ) {
#if DEBUG
            Assert.That( this.MyLockHeld );
            Assert.That( waitEvent == null );
#endif
            this.ExitMyLock();
            var newEvent = makeAutoResetEvent ? ( EventWaitHandle ) new AutoResetEvent( false ) : new ManualResetEvent( false );
            this.EnterMyLock();
            if ( waitEvent == null ) {
                waitEvent = newEvent;
            }
            else {
                newEvent.Close();
            }
        }

        /// <summary>
        ///     Waits on 'waitEvent' with a timeout of 'millisceondsTimeout.
        ///     Before the wait 'numWaiters' is incremented and is restored before leaving this routine.
        /// </summary>
        private Boolean WaitOnEvent( EventWaitHandle waitEvent, ref uint numWaiters, int millisecondsTimeout ) {
#if DEBUG
            Assert.That( this.MyLockHeld );
#endif
            waitEvent.Reset();
            numWaiters++;
            this._fNoWaiters = false;

            //Setting these bits will prevent new readers from getting in.
            if ( this._numWriteWaiters == 1 ) {
                this.SetWritersWaiting();
            }
            if ( this._numWriteUpgradeWaiters == 1 ) {
                this.SetUpgraderWaiting();
            }

            var waitSuccessful = false;
            this.ExitMyLock(); // Do the wait outside of any lock

            try {
                waitSuccessful = waitEvent.WaitOne( millisecondsTimeout, false );
            }
            finally {
                this.EnterMyLock();
                --numWaiters;

                if ( this._numWriteWaiters == 0 && this._numWriteUpgradeWaiters == 0 && this._numUpgradeWaiters == 0 && this._numReadWaiters == 0 ) {
                    this._fNoWaiters = true;
                }

                if ( this._numWriteWaiters == 0 ) {
                    this.ClearWritersWaiting();
                }
                if ( this._numWriteUpgradeWaiters == 0 ) {
                    this.ClearUpgraderWaiting();
                }

                if ( !waitSuccessful ) // We may also be aboutto throw for some reason.  Exit myLock. 
                {
                    this.ExitMyLock();
                }
            }
            return waitSuccessful;
        }

        private void SetWritersWaiting() {
            this._owners |= WaitingWriters;
        }

        private void SetUpgraderWaiting() {
            this._owners |= WaitingUpgrader;
        }

        private void ClearWritersWaiting() {
            this._owners &= ~WaitingWriters;
        }

        private void ClearUpgraderWaiting() {
            this._owners &= ~WaitingUpgrader;
        }

        public void EnterWriteLock() {
            this.TryEnterWriteLock( -1 );
        }

        public Boolean TryEnterWriteLock( int millisecondsTimeout ) {
            Thread.BeginCriticalRegion();
            var result = false;
            try {
                result = this.TryEnterWriteLockCore( millisecondsTimeout );
            }
            finally {
                if ( !result ) {
                    Thread.EndCriticalRegion();
                }
            }
            return result;
        }

        private Boolean TryEnterWriteLockCore( int millisecondsTimeout ) {
            if ( millisecondsTimeout < -1 ) {
                throw new ArgumentOutOfRangeException( "millisecondsTimeout" );
            }

            if ( this._fDisposed ) {
                throw new ObjectDisposedException( null );
            }

            var id = Thread.CurrentThread.ManagedThreadId;
            ReaderWriterCount lrwc;
            var upgradingToWrite = false;

            if ( !this._fIsReentrant ) {
                if ( id == this._writeLockOwnerId ) {
                    //Check for AW->AW
                    throw new LockRecursionException();
                }
                if ( id == this._upgradeLockOwnerId ) {
                    //AU->AW case is allowed once. 
                    upgradingToWrite = true;
                }

                this.EnterMyLock();
                lrwc = this.GetThreadRWCount( id, true );

                //Can't acquire write lock with reader lock held.
                if ( lrwc != null && lrwc.Readercount > 0 ) {
                    this.ExitMyLock();
                    throw new LockRecursionException();
                }
            }
            else {
                this.EnterMyLock();
                lrwc = this.GetThreadRWCount( id, false );

                if ( id == this._writeLockOwnerId ) {
                    lrwc.RecursiveCounts.writercount++;
                    this.ExitMyLock();
                    return true;
                }
                if ( id == this._upgradeLockOwnerId ) {
                    upgradingToWrite = true;
                }
                else if ( lrwc.Readercount > 0 ) {
                    //Write locks may not be acquired if only read locks have been
                    //acquired.
                    this.ExitMyLock();
                    throw new LockRecursionException();
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

                    switch ( readercount ) {
                        case 1:
                            this.SetWriterAcquired(); // indicate we have a writer. 
                            break;
                        case 2:
                            if ( lrwc != null ) {
                                if ( IsRwHashEntryChanged( lrwc, id ) ) {
                                    lrwc = this.GetThreadRWCount( id, false );
                                }

                                if ( lrwc.Readercount > 0 ) {
                                    //This check is needed for EU->ER->EW case, as the owner count will be two. 
                                    Assert.That( this._fIsReentrant );
                                    Assert.That( this._fUpgradeThreadHoldingRead );

                                    //Good case again, there is just one upgrader, and no readers. 
                                    this.SetWriterAcquired(); // indicate we have a writer.
                                }
                            }
                            break;
                    }
                }

                if ( spincount < MaxSpinCount ) {
                    this.ExitMyLock();
                    if ( millisecondsTimeout == 0 ) {
                        return false;
                    }
                    spincount++;
                    SpinWait( spincount );
                    this.EnterMyLock();
                    continue;
                }

                Boolean retVal;
                if ( upgradingToWrite ) {
                    if ( this._waitUpgradeEvent == null ) // Create the needed event 
                    {
                        this.LazyCreateEvent( ref this._waitUpgradeEvent, true );
                        continue; // since we left the lock, start over.
                    }

                    Assert.That( this._numWriteUpgradeWaiters == 0, "There can be at most one thread with the upgrade lock held." );

                    retVal = this.WaitOnEvent( this._waitUpgradeEvent, ref this._numWriteUpgradeWaiters, millisecondsTimeout );

                    //The lock is not held in case of failure.
                    if ( !retVal ) {
                        return false;
                    }
                }
                else {
                    // Drat, we need to wait.  Mark that we have waiters and wait.
                    if ( this._writeEvent == null ) // create the needed event. 
                    {
                        this.LazyCreateEvent( ref this._writeEvent, true );
                        continue; // since we left the lock, start over. 
                    }

                    retVal = this.WaitOnEvent( this._writeEvent, ref this._numWriteWaiters, millisecondsTimeout );
                    //The lock is not held in case of failure. 
                    if ( !retVal ) {
                        return false;
                    }
                }
            }

            Assert.That( ( this._owners & WriterHeld ) > 0 );

            if ( this._fIsReentrant ) {
                if ( IsRwHashEntryChanged( lrwc, id ) ) {
                    lrwc = this.GetThreadRWCount( id, false );
                }
                if ( lrwc != null ) {
                    lrwc.RecursiveCounts.writercount++;
                }
            }

            this.ExitMyLock();

            this._writeLockOwnerId = id;

            return true;
        }

        private Boolean IsWriterAcquired() {
            return ( this._owners & ~WaitingWriters ) == 0;
        }

        private void SetWriterAcquired() {
            this._owners |= WriterHeld; // indicate we have a writer.
        }

        public void EnterUpgradeableReadLock() {
            this.TryEnterUpgradeableReadLock( -1 );
        }

        public Boolean TryEnterUpgradeableReadLock( int millisecondsTimeout ) {
            Thread.BeginCriticalRegion();
            var result = false;
            try {
                result = this.TryEnterUpgradeableReadLockCore( millisecondsTimeout );
            }
            finally {
                if ( !result ) {
                    Thread.EndCriticalRegion();
                }
            }
            return result;
        }

        private Boolean TryEnterUpgradeableReadLockCore( int millisecondsTimeout ) {
            if ( millisecondsTimeout < -1 ) {
                throw new ArgumentOutOfRangeException( "millisecondsTimeout" );
            }

            if ( this._fDisposed ) {
                throw new ObjectDisposedException( null );
            }

            var id = Thread.CurrentThread.ManagedThreadId;

            ReaderWriterCount lrwc;

            if ( !this._fIsReentrant ) {
                if ( id == this._upgradeLockOwnerId ) {
                    //Check for AU->AU 
                    throw new LockRecursionException();
                }
                if ( id == this._writeLockOwnerId ) {
                    //Check for AU->AW
                    throw new LockRecursionException();
                }

                this.EnterMyLock();
                lrwc = this.GetThreadRWCount( id, true );
                //Can't acquire upgrade lock with reader lock held.
                if ( lrwc != null && lrwc.Readercount > 0 ) {
                    this.ExitMyLock();
                    throw new LockRecursionException();
                }
            }
            else {
                this.EnterMyLock();
                lrwc = this.GetThreadRWCount( id, false );

                if ( id == this._upgradeLockOwnerId ) {
                    lrwc.RecursiveCounts.upgradecount++;
                    this.ExitMyLock();
                    return true;
                }
                if ( id == this._writeLockOwnerId ) {
                    //Write lock is already held, Just update the global state to show presence of upgrader.
                    Assert.That( ( this._owners & WriterHeld ) > 0 );
                    this._owners++;
                    this._upgradeLockOwnerId = id;
                    lrwc.RecursiveCounts.upgradecount++;
                    if ( lrwc.Readercount > 0 ) {
                        this._fUpgradeThreadHoldingRead = true;
                    }
                    this.ExitMyLock();
                    return true;
                }
                if ( lrwc.Readercount > 0 ) {
                    //Upgrade locks may not be acquired if only read locks have been acquired. 
                    this.ExitMyLock();
                    throw new LockRecursionException();
                }
            }

            var spincount = 0;

            for ( ;; ) {
                //Once an upgrade lock is taken, it's like having a reader lock held
                //until upgrade or downgrade operations are performed. 

                if ( ( this._upgradeLockOwnerId == -1 ) && ( this._owners < MAX_READER ) ) {
                    this._owners++;
                    this._upgradeLockOwnerId = id;
                    break;
                }

                if ( spincount < MaxSpinCount ) {
                    this.ExitMyLock();
                    if ( millisecondsTimeout == 0 ) {
                        return false;
                    }
                    spincount++;
                    SpinWait( spincount );
                    this.EnterMyLock();
                    continue;
                }

                // Drat, we need to wait.  Mark that we have waiters and wait. 
                if ( this._upgradeEvent == null ) // Create the needed event 
                {
                    this.LazyCreateEvent( ref this._upgradeEvent, true );
                    continue; // since we left the lock, start over.
                }

                //Only one thread with the upgrade lock held can proceed. 
                var retVal = this.WaitOnEvent( this._upgradeEvent, ref this._numUpgradeWaiters, millisecondsTimeout );
                if ( !retVal ) {
                    return false;
                }
            }

            if ( this._fIsReentrant ) {
                //The lock may have been dropped getting here, so make a quick check to see whether some other
                //thread did not grab the entry. 
                if ( IsRwHashEntryChanged( lrwc, id ) ) {
                    lrwc = this.GetThreadRWCount( id, false );
                }
                if ( lrwc != null ) {
                    lrwc.RecursiveCounts.upgradecount++;
                }
            }

            this.ExitMyLock();

            return true;
        }

        public void ExitReadLock() {
            var id = Thread.CurrentThread.ManagedThreadId;

            this.EnterMyLock();

            var lrwc = this.GetThreadRWCount( id, true );

            if ( !this._fIsReentrant ) {
                if ( lrwc == null ) {
                    //You have to be holding the read lock to make this call.
                    this.ExitMyLock();
                    throw new SynchronizationLockException();
                }
            }
            else {
                if ( lrwc == null || lrwc.Readercount < 1 ) {
                    this.ExitMyLock();
                    throw new SynchronizationLockException();
                }

                if ( lrwc.Readercount > 1 ) {
                    lrwc.Readercount--;
                    this.ExitMyLock();
                    Thread.EndCriticalRegion();
                    return;
                }

                if ( id == this._upgradeLockOwnerId ) {
                    this._fUpgradeThreadHoldingRead = false;
                }
            }

            Assert.That( this._owners > 0, "ReleasingReaderLock: releasing lock and no read lock taken" );

            --this._owners;

            Assert.That( lrwc.Readercount == 1 );
            lrwc.Readercount--;

            this.ExitAndWakeUpAppropriateWaiters();
            Thread.EndCriticalRegion();
        }

        /// <summary>
        ///     Determines the appropriate events to set, leaves the locks, and sets the events.
        /// </summary>
        private void ExitAndWakeUpAppropriateWaiters() {
#if DEBUG
            Assert.That( this.MyLockHeld );
#endif
            if ( this._fNoWaiters ) {
                this.ExitMyLock();
                return;
            }

            this.ExitAndWakeUpAppropriateWaitersPreferringWriters();
        }

        private void ExitAndWakeUpAppropriateWaitersPreferringWriters() {
            var setUpgradeEvent = false;
            var setReadEvent = false;
            var readercount = this.GetNumReaders();

            //We need this case for EU->ER->EW case, as the read count will be 2 in
            //that scenario.
            if ( this._fIsReentrant ) {
                if ( this._numWriteUpgradeWaiters > 0 && this._fUpgradeThreadHoldingRead && readercount == 2 ) {
                    this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock)
                    this._waitUpgradeEvent.Set(); // release all upgraders (however there can be at most one). 
                    return;
                }
            }

            if ( readercount == 1 && this._numWriteUpgradeWaiters > 0 ) {
                //We have to be careful now, as we are droppping the lock. 
                //No new writes should be allowed to sneak in if an upgrade
                //was pending. 

                this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock)
                this._waitUpgradeEvent.Set(); // release all upgraders (however there can be at most one).
            }
            else if ( readercount == 0 && this._numWriteWaiters > 0 ) {
                this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock) 
                this._writeEvent.Set(); // release one writer.
            }
            else {
                if ( this._numReadWaiters != 0 || this._numUpgradeWaiters != 0 ) {
                    if ( this._numReadWaiters != 0 ) {
                        setReadEvent = true;
                    }

                    if ( this._numUpgradeWaiters != 0 && this._upgradeLockOwnerId == -1 ) {
                        setUpgradeEvent = true;
                    }

                    this.ExitMyLock(); // Exit before signaling to improve efficiency (wakee will need the lock) 

                    if ( setReadEvent ) {
                        this._readEvent.Set(); // release all readers. 
                    }

                    if ( setUpgradeEvent ) {
                        this._upgradeEvent.Set(); //release one upgrader.
                    }
                }
                else {
                    this.ExitMyLock();
                }
            }
        }

        public void ExitWriteLock() {
            var id = Thread.CurrentThread.ManagedThreadId;

            if ( !this._fIsReentrant ) {
                if ( id != this._writeLockOwnerId ) {
                    //You have to be holding the write lock to make this call.
                    throw new SynchronizationLockException();
                }
                this.EnterMyLock();
            }
            else {
                this.EnterMyLock();
                var lrwc = this.GetThreadRWCount( id, false );

                if ( lrwc == null ) {
                    this.ExitMyLock();
                    throw new SynchronizationLockException();
                }

                var rc = lrwc.RecursiveCounts;

                if ( rc.writercount < 1 ) {
                    this.ExitMyLock();
                    throw new SynchronizationLockException();
                }

                rc.writercount--;
                if ( rc.writercount > 0 ) {
                    this.ExitMyLock();
                    Thread.EndCriticalRegion();
                    return;
                }
            }

            Assert.That( ( this._owners & WriterHeld ) > 0, "Calling ReleaseWriterLock when no write lock is held" );

            this.ClearWriterAcquired();

            this._writeLockOwnerId = -1;

            this.ExitAndWakeUpAppropriateWaiters();
            Thread.EndCriticalRegion();
        }

        private void ClearWriterAcquired() {
            this._owners &= ~WriterHeld;
        }

        public void ExitUpgradeableReadLock() {
            var id = Thread.CurrentThread.ManagedThreadId;

            if ( !this._fIsReentrant ) {
                if ( id != this._upgradeLockOwnerId ) {
                    //You have to be holding the upgrade lock to make this call.
                    throw new SynchronizationLockException();
                }
                this.EnterMyLock();
            }
            else {
                this.EnterMyLock();
                var lrwc = this.GetThreadRWCount( id: id, DontAllocate: true );

                if ( lrwc == null ) {
                    this.ExitMyLock();
                    throw new SynchronizationLockException();
                }

                var rc = lrwc.RecursiveCounts;

                if ( rc.upgradecount < 1 ) {
                    this.ExitMyLock();
                    throw new SynchronizationLockException();
                }

                rc.upgradecount--;

                if ( rc.upgradecount > 0 ) {
                    this.ExitMyLock();
                    Thread.EndCriticalRegion();
                    return;
                }

                this._fUpgradeThreadHoldingRead = false;
            }

            this._owners--;
            this._upgradeLockOwnerId = -1;

            this.ExitAndWakeUpAppropriateWaiters();
            Thread.EndCriticalRegion();
        }

        public void Dispose() {
            this.Dispose( true );
        }

        private void Dispose( Boolean disposing ) {
            if ( !disposing ) {
                return;
            }
            if ( this._fDisposed ) {
                throw new ObjectDisposedException( null );
            }

            if ( this.WaitingReadCount > 0 || this.WaitingUpgradeCount > 0 || this.WaitingWriteCount > 0 ) {
                throw new SynchronizationLockException();
            }

            if ( this.IsReadLockHeld || this.IsUpgradeableReadLockHeld || this.IsWriteLockHeld ) {
                throw new SynchronizationLockException();
            }

            if ( this._writeEvent != null ) {
                this._writeEvent.Close();
                this._writeEvent = null;
            }

            if ( this._readEvent != null ) {
                this._readEvent.Close();
                this._readEvent = null;
            }

            if ( this._upgradeEvent != null ) {
                this._upgradeEvent.Close();
                this._upgradeEvent = null;
            }

            if ( this._waitUpgradeEvent != null ) {
                this._waitUpgradeEvent.Close();
                this._waitUpgradeEvent = null;
            }

            this._fDisposed = true;
        }

        public Boolean TryEnterReadLock( TimeSpan timeout ) {
            var ltm = ( long ) timeout.TotalMilliseconds;
            if ( ltm < -1 || ltm > Int32.MaxValue ) {
                throw new ArgumentOutOfRangeException( "timeout" );
            }
            var tm = ( int ) timeout.TotalMilliseconds;
            return this.TryEnterReadLock( tm );
        }

        public Boolean TryEnterWriteLock( TimeSpan timeout ) {
            var ltm = ( long ) timeout.TotalMilliseconds;
            if ( ltm < -1 || ltm > Int32.MaxValue ) {
                throw new ArgumentOutOfRangeException( "timeout" );
            }

            var tm = ( int ) timeout.TotalMilliseconds;
            return this.TryEnterWriteLock( tm );
        }

        public Boolean TryEnterUpgradeableReadLock( TimeSpan timeout ) {
            var ltm = ( long ) timeout.TotalMilliseconds;
            if ( ltm < -1 || ltm > Int32.MaxValue ) {
                throw new ArgumentOutOfRangeException( "timeout" );
            }

            var tm = ( int ) timeout.TotalMilliseconds;
            return this.TryEnterUpgradeableReadLock( tm );
        }

#if DEBUG
#endif
    }
}
