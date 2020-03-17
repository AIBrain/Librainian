// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Privilege.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "Privilege.cs" was last formatted by Protiguous on 2020/03/16 at 2:58 PM.

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
                throw new ArgumentNullException( paramName: nameof( privilegeName ) );
            }

            this.luid = LuidFromPrivilege( privilege: privilegeName );
        }

        [ReliabilityContract( consistencyGuarantee: Consistency.WillNotCorruptState, cer: Cer.MayFail )]
        private static LUID LuidFromPrivilege( [NotNull] String privilege ) {
            LUID luid;
            luid.LowPart = 0;
            luid.HighPart = 0;

            //
            // Look up the privilege LUID inside the cache
            //

            RuntimeHelpers.PrepareConstrainedRegions();

            try {
                privilegeLock.AcquireReaderLock( millisecondsTimeout: Timeout.Infinite );

                if ( luids.Contains( key: privilege ) ) {
                    var o = luids[ key: privilege ];

                    if ( o != null ) {
                        luid = ( LUID )o;
                    }

                    privilegeLock.ReleaseReaderLock();
                }
                else {
                    privilegeLock.ReleaseReaderLock();

                    if ( !NativeMethods.LookupPrivilegeValue( lpSystemName: null, lpName: privilege, Luid: ref luid ) ) {
                        var error = Marshal.GetLastWin32Error();

                        switch ( error ) {
                            case NativeMethods.ERROR_NOT_ENOUGH_MEMORY: throw new OutOfMemoryException();
                            case NativeMethods.ERROR_ACCESS_DENIED:
                                throw new UnauthorizedAccessException( message: "Caller does not have the rights to look up privilege local unique identifier" );
                            case NativeMethods.ERROR_NO_SUCH_PRIVILEGE:
                                throw new ArgumentException( message: $"{privilege} is not a valid privilege name", paramName: nameof( privilege ) );
                            default: throw new Win32Exception( error: error );
                        }
                    }

                    privilegeLock.AcquireWriterLock( millisecondsTimeout: Timeout.Infinite );
                }
            }
            finally {
                if ( privilegeLock.IsReaderLockHeld ) {
                    privilegeLock.ReleaseReaderLock();
                }

                if ( privilegeLock.IsWriterLockHeld ) {
                    if ( !luids.Contains( key: privilege ) ) {
                        luids[ key: privilege ] = luid;
                        privileges[ key: luid ] = privilege;
                    }

                    privilegeLock.ReleaseWriterLock();
                }
            }

            return luid;
        }

        [ReliabilityContract( consistencyGuarantee: Consistency.WillNotCorruptState, cer: Cer.Success )]
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
                    Thread.SetData( slot: tlsSlot, data: null );
                }
            }
        }

        [ReliabilityContract( consistencyGuarantee: Consistency.WillNotCorruptState, cer: Cer.MayFail )]
        private void ToggleState( Boolean enable ) {
            var error = 0;

            //
            // All privilege operations must take place on the same thread
            //

            if ( !this.currentThread.Equals( obj: Thread.CurrentThread ) ) {
                throw new InvalidOperationException( message: "Operation must take place on the thread that created the object" );
            }

            //
            // This privilege was already altered and needs to be reverted before it can be altered again
            //

            if ( this.NeedToRevert ) {
                throw new InvalidOperationException( message: "Must revert the privilege prior to attempting this operation" );
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

                    this.tlsContents = Thread.GetData( slot: tlsSlot ) as TlsContents;

                    if ( this.tlsContents == null ) {
                        this.tlsContents = new TlsContents();
                        Thread.SetData( slot: tlsSlot, data: this.tlsContents );
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

                    if ( !NativeMethods.AdjustTokenPrivileges( TokenHandle: this.tlsContents.ThreadHandle, DisableAllPrivileges: false, NewState: ref newState,
                        BufferLength: ( UInt32 )Marshal.SizeOf( structure: previousState ), PreviousState: ref previousState, ReturnLength: ref previousSize ) ) {
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
                case NativeMethods.ERROR_NOT_ALL_ASSIGNED: throw new PrivilegeNotHeldException( privilege: privileges[ key: this.luid ] as String );
                case NativeMethods.ERROR_NOT_ENOUGH_MEMORY: throw new OutOfMemoryException();
                case NativeMethods.ERROR_ACCESS_DENIED:
                case NativeMethods.ERROR_CANT_OPEN_ANONYMOUS:
                    throw new UnauthorizedAccessException( message: "The caller does not have the right to change the privilege" );
            }

            if ( error != 0 ) {
                throw new Win32Exception( error: error );
            }
        }

        public override void DisposeManaged() {
            using ( this.tlsContents ) { }
        }

        [ReliabilityContract( consistencyGuarantee: Consistency.WillNotCorruptState, cer: Cer.MayFail )]
        public void Enable() => this.ToggleState( enable: true );

        [ReliabilityContract( consistencyGuarantee: Consistency.WillNotCorruptState, cer: Cer.MayFail )]
        public void Revert() {
            var error = 0;

            //
            // All privilege operations must take place on the same thread
            //

            if ( !this.currentThread.Equals( obj: Thread.CurrentThread ) ) {
                throw new InvalidOperationException( message: "Operation must take place on the thread that created the object" );
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

                        if ( !NativeMethods.AdjustTokenPrivileges( TokenHandle: this.tlsContents.ThreadHandle, DisableAllPrivileges: false, NewState: ref newState,
                            BufferLength: ( UInt32 )Marshal.SizeOf( structure: previousState ), PreviousState: ref previousState, ReturnLength: ref previousSize ) ) {
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
                case NativeMethods.ERROR_ACCESS_DENIED: throw new UnauthorizedAccessException( message: "Caller does not have the permission to change the privilege" );
            }

            if ( error != 0 ) {
                throw new Win32Exception( error: error );
            }
        }

        private sealed class TlsContents : IDisposable {

            [NotNull]
            private static readonly Object syncRoot = new Object();

            [NotNull]
            private static SafeTokenHandle processHandle = new SafeTokenHandle( handle: IntPtr.Zero );

            [NotNull]
            private readonly SafeTokenHandle threadHandle = new SafeTokenHandle( handle: IntPtr.Zero );

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
                            if ( !NativeMethods.OpenProcessToken( ProcessToken: NativeMethods.GetCurrentProcess(), DesiredAccess: TokenAccessLevels.Duplicate,
                                TokenHandle: ref processHandle ) ) {
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

                    if ( !NativeMethods.OpenThreadToken( ThreadToken: NativeMethods.GetCurrentThread(),
                        DesiredAccess: TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, OpenAsSelf: true, TokenHandle: ref this.threadHandle ) ) {
                        if ( success ) {
                            error = Marshal.GetLastWin32Error();

                            if ( error != NativeMethods.ERROR_NO_TOKEN ) {
                                success = false;
                            }

                            if ( success ) {
                                error = 0;

                                if ( !NativeMethods.DuplicateTokenEx( ExistingToken: processHandle,
                                    DesiredAccess: TokenAccessLevels.Impersonate | TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges, TokenAttributes: IntPtr.Zero,
                                    ImpersonationLevel: NativeMethods.SecurityImpersonationLevel.Impersonation, TokenType: NativeMethods.TokenType.Impersonation,
                                    NewToken: ref this.threadHandle ) ) {
                                    error = Marshal.GetLastWin32Error();
                                    success = false;
                                }
                            }

                            if ( success ) {
                                if ( !NativeMethods.SetThreadToken( Thread: IntPtr.Zero, Token: this.threadHandle ) ) {
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
                        throw new UnauthorizedAccessException( message: "The caller does not have the rights to perform the operation" );
                    default: {
                            if ( error != 0 ) {
                                throw new Win32Exception( error: error );
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

                GC.SuppressFinalize( obj: this );
            }

            public void IncrementReferenceCount() => this.ReferenceCountValue++;
        }
    }
}