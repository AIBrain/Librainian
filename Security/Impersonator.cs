// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Impersonator.cs",
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
// "Librainian/Librainian/Impersonator.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

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
                if ( !NativeMethods.RevertToSelf() ) { throw new Win32Exception( error: Marshal.GetLastWin32Error() ); }
                else {
                    if ( NativeMethods.LogonUser( lpszUserName: userName, lpszDomain: domain, lpszPassword: password, dwLogonType: Logon32LogonInteractive, dwLogonProvider: Logon32ProviderDefault, phToken: ref token ) ==
                         0 ) { throw new Win32Exception( error: Marshal.GetLastWin32Error() ); }
                    else {
                        if ( NativeMethods.DuplicateToken( hToken: token, impersonationLevel: 2, hNewToken: ref tokenDuplicate ) == 0 ) { throw new Win32Exception( error: Marshal.GetLastWin32Error() ); }
                        else {
                            var tempWindowsIdentity = new WindowsIdentity( userToken: tokenDuplicate );
                            this._impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                    }
                }
            }
            finally {
                if ( token != IntPtr.Zero ) { NativeMethods.CloseHandle( handle: token ); }

                if ( tokenDuplicate != IntPtr.Zero ) { NativeMethods.CloseHandle( handle: tokenDuplicate ); }
            }
        }

        public override void DisposeManaged() => this._impersonationContext?.Undo();
    }
}