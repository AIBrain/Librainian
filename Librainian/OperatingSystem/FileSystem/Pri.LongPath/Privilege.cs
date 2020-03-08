namespace Librainian.OperatingSystem.FileSystem.Pri.LongPath {

    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Threading;
    using JetBrains.Annotations;
    using Utilities;

    public delegate void PrivilegedCallback( Object state );

    /// <summary>From MSDN Magazine March 2005</summary>
    public sealed class Privilege : ABetterClassDispose {

        [NotNull]
        private static readonly HybridDictionary luids = new HybridDictionary();

        [NotNull]
        private static readonly ReaderWriterLock privilegeLock = new ReaderWriterLock();

        [NotNull]
        private static readonly HybridDictionary privileges = new HybridDictionary();

        [NotNull]
        private static readonly LocalDataStoreSlot tlsSlot = Thread.AllocateDataSlot();

        [NotNull]
        private readonly Thread currentThread = Thread.CurrentThread;

        private readonly LUID luid;

        private Boolean initialState;

        private Boolean stateWasChanged;

        private TlsContents tlsContents;

        public const String AssignPrimaryToken = "SeAssignPrimaryTokenPrivilege";

        public const String Audit = "SeAuditPrivilege";

        public const String Backup = "SeBackupPrivilege";

        public const String ChangeNotify = "SeChangeNotifyPrivilege";

        public const String CreateGlobal = "SeCreateGlobalPrivilege";

        public const String CreatePageFile = "SeCreatePagefilePrivilege";

        public const String CreatePermanent = "SeCreatePermanentPrivilege";

        public const String CreateToken = "SeCreateTokenPrivilege";

        public const String Debug = "SeDebugPrivilege";

        public const String EnableDelegation = "SeEnableDelegationPrivilege";

        public const String Impersonate = "SeImpersonatePrivilege";

        public const String IncreaseBasePriority = "SeIncreaseBasePriorityPrivilege";

        public const String IncreaseQuota = "SeIncreaseQuotaPrivilege";

        public const String LoadDriver = "SeLoadDriverPrivilege";

        public const String LockMemory = "SeLockMemoryPrivilege";

        public const String MachineAccount = "SeMachineAccountPrivilege";

        public const String ManageVolume = "SeManageVolumePrivilege";

        public const String ProfileSingleProcess = "SeProfileSingleProcessPrivilege";

        public const String RemoteShutdown = "SeRemoteShutdownPrivilege";

        public const String ReserveProcessor = "SeReserveProcessorPrivilege";

        public const String Restore = "SeRestorePrivilege";

        public const String Security = "SeSecurityPrivilege";

        public const String Shutdown = "SeShutdownPrivilege";

        public const String SyncAgent = "SeSyncAgentPrivilege";

        public const String SystemEnvironment = "SeSystemEnvironmentPrivilege";

        public const String SystemProfile = "SeSystemProfilePrivilege";

        public const String SystemTime = "SeSystemtimePrivilege";

        public const String TakeOwnership = "SeTakeOwnershipPrivilege";

        public const String TrustedComputingBase = "SeTcbPrivilege";

        public const String TrustedCredentialManagerAccess = "SeTrustedCredManAccessPrivilege";

        public const String Undock = "SeUndockPrivilege";

        public const String UnsolicitedInput = "SeUnsolicitedInputPrivilege";

        public Boolean NeedToRevert { get; private set; }

        //
        // This routine is a wrapper around a hashtable containing mappings
        // of privilege names to luids
        //
        public Privilege( [NotNull] String privilegeName ) {
            if ( privilegeName == null ) {
                throw new ArgumentNullException( nameof( privilegeName ) );
            }

            this.luid = LuidFromPrivilege( privilegeName );
        }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        private static LUID LuidFromPrivilege( [NotNull] String privilege ) {
            LUID luid;
            luid.LowPart = 0;
            luid.HighPart = 0;

            //
            // Look up the privilege LUID inside the cache
            //

            RuntimeHelpers.PrepareConstrainedRegions();

            try {
                privilegeLock.AcquireReaderLock( Timeout.Infinite );

                if ( luids.Contains( privilege ) ) {
                    var o = luids[ privilege ];

                    if ( o != null ) {
                        luid = ( LUID )o;
                    }

                    privilegeLock.ReleaseReaderLock();
                }
                else {
                    privilegeLock.ReleaseReaderLock();

                    if ( !NativeMethods.LookupPrivilegeValue( null, privilege, ref luid ) ) {
                        var error = Marshal.GetLastWin32Error();

                        switch ( error ) {
                            case NativeMethods.ERROR_NOT_ENOUGH_MEMORY: throw new OutOfMemoryException();
                            case NativeMethods.ERROR_ACCESS_DENIED: throw new UnauthorizedAccessException( "Caller does not have the rights to look up privilege local unique identifier" );
                            case NativeMethods.ERROR_NO_SUCH_PRIVILEGE: throw new ArgumentException( $"{privilege} is not a valid privilege name", nameof( privilege ) );
                            default: throw new Win32Exception( error );
                        }
                    }

                    privilegeLock.AcquireWriterLock( Timeout.Infinite );
                }
            }
            finally {
                if ( privilegeLock.IsReaderLockHeld ) {
                    privilegeLock.ReleaseReaderLock();
                }

                if ( privilegeLock.IsWriterLockHeld ) {
                    if ( !luids.Contains( privilege ) ) {
                        luids[ privilege ] = luid;
                        privileges[ luid ] = privilege;
                    }

                    privilegeLock.ReleaseWriterLock();
                }
            }

            return luid;
        }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        private void Reset() {
            RuntimeHelpers.PrepareConstrainedRegions();

            try {

                // Payload is in the finally block
                // as a way to guarantee execution
            }
            finally {
                this.stateWasChanged = false;
                this.initialState = false;
                this.NeedToRevert = false;

                if ( 0 == this.tlsContents?.DecrementReferenceCount() ) {
                    this.tlsContents = null;
                    Thread.SetData( tlsSlot, null );
                }
            }
        }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        private void ToggleState( Boolean enable ) {
            var error = 0;

            //
            // All privilege operations must take place on the same thread
            //

            if ( !this.currentThread.Equals( Thread.CurrentThread ) ) {
                throw new InvalidOperationException( "Operation must take place on the thread that created the object" );
            }

            //
            // This privilege was already altered and needs to be reverted before it can be altered again
            //

            if ( this.NeedToRevert ) {
                throw new InvalidOperationException( "Must revert the privilege prior to attempting this operation" );
            }

            //
            // Need to make this block of code non-interruptible so that it would preserve
            // consistency of thread oken state even in the face of catastrophic exceptions
            //

            RuntimeHelpers.PrepareConstrainedRegions();

            try {

                //
                // The payload is entirely in the finally block
                // This is how we ensure that the code will not be
                // interrupted by catastrophic exceptions
                //
            }
            finally {
                try {

                    //
                    // Retrieve TLS state
                    //

                    this.tlsContents = Thread.GetData( tlsSlot ) as TlsContents;

                    if ( this.tlsContents == null ) {
                        this.tlsContents = new TlsContents();
                        Thread.SetData( tlsSlot, this.tlsContents );
                    }
                    else {
                        this.tlsContents.IncrementReferenceCount();
                    }

                    var newState = new TOKEN_PRIVILEGE {
                        PrivilegeCount = 1,
                        Privilege = {
                            Luid = this.luid, Attributes = enable ? NativeMethods.SE_PRIVILEGE_ENABLED : NativeMethods.SE_PRIVILEGE_DISABLED
                        }
                    };

                    var previousState = new TOKEN_PRIVILEGE();
                    UInt32 previousSize = 0;

                    //
                    // Place the new privilege on the thread token and remember the previous state.
                    //

                    if ( !NativeMethods.AdjustTokenPrivileges( this.tlsContents.ThreadHandle, false, ref newState, ( UInt32 )Marshal.SizeOf( previousState ),
                        ref previousState, ref previousSize ) ) {
                        error = Marshal.GetLastWin32Error();
                    }
                    else if ( NativeMethods.ERROR_NOT_ALL_ASSIGNED == Marshal.GetLastWin32Error() ) {
                        error = NativeMethods.ERROR_NOT_ALL_ASSIGNED;
                    }
                    else {

                        //
                        // This is the initial state that revert will have to go back to
                        //

                        this.initialState = ( previousState.Privilege.Attributes & NativeMethods.SE_PRIVILEGE_ENABLED ) != 0;

                        //
                        // Remember whether state has changed at all
                        //

                        this.stateWasChanged = this.initialState != enable;

                        //
                        // If we had to impersonate, or if the privilege state changed we'll need to revert
                        //

                        this.NeedToRevert = this.tlsContents.IsImpersonating || this.stateWasChanged;
                    }
                }
                finally {
                    if ( !this.NeedToRevert ) {
                        this.Reset();
                    }
                }
            }

            switch ( error ) {
                case NativeMethods.ERROR_NOT_ALL_ASSIGNED: throw new PrivilegeNotHeldException( privileges[ this.luid ] as String );
                case NativeMethods.ERROR_NOT_ENOUGH_MEMORY: throw new OutOfMemoryException();
                case NativeMethods.ERROR_ACCESS_DENIED:
                case NativeMethods.ERROR_CANT_OPEN_ANONYMOUS:
                    throw new UnauthorizedAccessException( "The caller does not have the right to change the privilege" );
            }

            if ( error != 0 ) {
                throw new Win32Exception( error );
            }
        }

        public override void DisposeManaged() {
            using ( this.tlsContents ) { }
        }

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        public void Enable() => this.ToggleState( true );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        public void Revert() {
            var error = 0;

            //
            // All privilege operations must take place on the same thread
            //

            if ( !this.currentThread.Equals( Thread.CurrentThread ) ) {
                throw new InvalidOperationException( "Operation must take place on the thread that created the object" );
            }

            if ( !this.NeedToRevert ) {
                return;
            }

            //
            // This code must be eagerly prepared and non-interruptible.
            //

            RuntimeHelpers.PrepareConstrainedRegions();

            try {

                //
                // The payload is entirely in the finally block
                // This is how we ensure that the code will not be
                // interrupted by catastrophic exceptions
                //
            }
            finally {
                var success = true;

                try {

                    //
                    // Only call AdjustTokenPrivileges if we're not going to be reverting to self,
                    // on this Revert, since doing the latter obliterates the thread token anyway
                    //

                    if ( this.stateWasChanged && ( this.tlsContents.ReferenceCountValue > 1 || !this.tlsContents.IsImpersonating ) ) {
                        var newState = new TOKEN_PRIVILEGE {
                            PrivilegeCount = 1,
                            Privilege = {
                                Luid = this.luid, Attributes = this.initialState ? NativeMethods.SE_PRIVILEGE_ENABLED : NativeMethods.SE_PRIVILEGE_DISABLED
                            }
                        };

                        var previousState = new TOKEN_PRIVILEGE();
                        UInt32 previousSize = 0;

                        if ( !NativeMethods.AdjustTokenPrivileges( this.tlsContents.ThreadHandle, false, ref newState, ( UInt32 )Marshal.SizeOf( previousState ),
                            ref previousState, ref previousSize ) ) {
                            error = Marshal.GetLastWin32Error();
                            success = false;
                        }
                    }
                }
                finally {
                    if ( success ) {
                        this.Reset();
                    }
                }
            }

            switch ( error ) {
                case NativeMethods.ERROR_NOT_ENOUGH_MEMORY: throw new OutOfMemoryException();
                case NativeMethods.ERROR_ACCESS_DENIED: throw new UnauthorizedAccessException( "Caller does not have the permission to change the privilege" );
            }

            if ( error != 0 ) {
                throw new Win32Exception( error );
            }
        }

        private sealed class TlsContents : IDisposable {

            [NotNull]
            private static readonly Object syncRoot = new Object();

            [NotNull]
            private static SafeTokenHandle processHandle = new SafeTokenHandle( IntPtr.Zero );

            [NotNull]
            private readonly SafeTokenHandle threadHandle = new SafeTokenHandle( IntPtr.Zero );

            private Boolean disposed;

            public Boolean IsImpersonating { get; }

            public Int32 ReferenceCountValue { get; private set; } = 1;

            [NotNull]
            public SafeTokenHandle ThreadHandle => this.threadHandle;

            public TlsContents() {
                var error = 0;
                var cachingError = 0;
                var success = true;

                if ( processHandle.IsInvalid ) {
                    lock ( syncRoot ) {
                        if ( processHandle.IsInvalid ) {
                            if ( !NativeMethods.OpenProcessToken( NativeMethods.GetCurrentProcess(), TokenAccessLevels.Duplicate, ref processHandle ) ) {
                                cachingError = Marshal.GetLastWin32Error();
                                success = false;
                            }
                        }
                    }
                }

                RuntimeHelpers.PrepareConstrainedRegions();

                try {

                    //
                    // Open the thread token; if there is no thread token,
                    // copy the process token onto the thread
                    //

                    if ( !NativeMethods.OpenThreadToken( NativeMethods.GetCurrentThread(), TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, true,
                        ref this.threadHandle ) ) {
                        if ( success ) {
                            error = Marshal.GetLastWin32Error();

                            if ( error != NativeMethods.ERROR_NO_TOKEN ) {
                                success = false;
                            }

                            if ( success ) {
                                error = 0;

                                if ( !NativeMethods.DuplicateTokenEx( processHandle,
                                    TokenAccessLevels.Impersonate | TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, IntPtr.Zero,
                                    NativeMethods.SecurityImpersonationLevel.Impersonation, NativeMethods.TokenType.Impersonation, ref this.threadHandle ) ) {
                                    error = Marshal.GetLastWin32Error();
                                    success = false;
                                }
                            }

                            if ( success ) {
                                if ( !NativeMethods.SetThreadToken( IntPtr.Zero, this.threadHandle ) ) {
                                    error = Marshal.GetLastWin32Error();
                                    success = false;
                                }
                            }

                            if ( success ) {

                                //
                                // This thread is now impersonating; it needs to be reverted to its original state
                                //

                                this.IsImpersonating = true;
                            }
                        }
                        else {
                            error = cachingError;
                        }
                    }
                    else {
                        success = true;
                    }
                }
                finally {
                    if ( !success ) {
                        this.Dispose();
                    }
                }

                switch ( error ) {
                    case NativeMethods.ERROR_NOT_ENOUGH_MEMORY: throw new OutOfMemoryException();
                    case NativeMethods.ERROR_ACCESS_DENIED:
                    case NativeMethods.ERROR_CANT_OPEN_ANONYMOUS:
                        throw new UnauthorizedAccessException( "The caller does not have the rights to perform the operation" );
                    default: {
                            if ( error != 0 ) {
                                throw new Win32Exception( error );
                            }
                            break;
                        }
                }
            }

            ~TlsContents() {
                this.Dispose();
            }

            public Int32 DecrementReferenceCount() {
                var result = --this.ReferenceCountValue;

                if ( result == 0 ) {
                    this.Dispose();
                }

                return result;
            }

            public void Dispose() {
                if ( this.disposed ) {
                    return;
                }

                using ( this.threadHandle ) { }

                if ( this.IsImpersonating ) {
                    NativeMethods.RevertToSelf();
                }

                this.disposed = true;

                GC.SuppressFinalize( this );
            }

            public void IncrementReferenceCount() => this.ReferenceCountValue++;
        }
    }
}