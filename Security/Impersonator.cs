// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Impersonator.cs" was last cleaned by Protiguous on 2018/05/09 at 1:15 PM

namespace Librainian.Security {

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using Magic;
    using OperatingSystem;

    /// <summary>
    /// </summary>
    public class Impersonator : ABetterClassDispose {
        private const Int32 Logon32LogonInteractive = 2;

        private const Int32 Logon32ProviderDefault = 0;

        private WindowsImpersonationContext _impersonationContext;

        public Impersonator( String userName, String domainName, String password ) => this.ImpersonateValidUser( userName: userName, domain: domainName, password: password );

        private void ImpersonateValidUser( String userName, String domain, String password ) {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            try {
                if ( !NativeMethods.RevertToSelf() ) {
                    throw new Win32Exception( error: Marshal.GetLastWin32Error() );
                }
                else {
                    if ( NativeMethods.LogonUser( lpszUserName: userName, lpszDomain: domain, lpszPassword: password, dwLogonType: Logon32LogonInteractive, dwLogonProvider: Logon32ProviderDefault, phToken: ref token ) ==
                         0 ) {
                        throw new Win32Exception( error: Marshal.GetLastWin32Error() );
                    }
                    else {
                        if ( NativeMethods.DuplicateToken( hToken: token, impersonationLevel: 2, hNewToken: ref tokenDuplicate ) == 0 ) {
                            throw new Win32Exception( error: Marshal.GetLastWin32Error() );
                        }
                        else {
                            var tempWindowsIdentity = new WindowsIdentity( userToken: tokenDuplicate );
                            this._impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                    }
                }
            }
            finally {
                if ( token != IntPtr.Zero ) {
                    NativeMethods.CloseHandle( handle: token );
                }

                if ( tokenDuplicate != IntPtr.Zero ) {
                    NativeMethods.CloseHandle( handle: tokenDuplicate );
                }
            }
        }

        public override void DisposeManaged() => this._impersonationContext?.Undo();
    }
}