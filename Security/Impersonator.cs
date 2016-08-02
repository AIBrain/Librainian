// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Impersonator.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Security {

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using Magic;

    /// <summary>
    /// </summary>
    public class Impersonator : BetterDisposableClass {
        private const Int32 Logon32LogonInteractive = 2;

        private const Int32 Logon32ProviderDefault = 0;

        private WindowsImpersonationContext _impersonationContext;

        public Impersonator( String userName, String domainName, String password ) {
            this.ImpersonateValidUser( userName, domainName, password );
        }

        protected override void CleanUpManagedResources() {
            this._impersonationContext?.Undo();
            base.CleanUpManagedResources();
        }

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        private static extern Boolean CloseHandle( IntPtr handle );

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern Int32 DuplicateToken( IntPtr hToken, Int32 impersonationLevel, ref IntPtr hNewToken );

        [DllImport( "advapi32.dll", SetLastError = true )]
        private static extern Int32 LogonUser( String lpszUserName, String lpszDomain, String lpszPassword, Int32 dwLogonType, Int32 dwLogonProvider, ref IntPtr phToken );

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern Boolean RevertToSelf();

        private void ImpersonateValidUser( String userName, String domain, String password ) {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            try {
                if ( RevertToSelf() ) {
                    if ( LogonUser( userName, domain, password, Logon32LogonInteractive, Logon32ProviderDefault, ref token ) != 0 ) {
                        if ( DuplicateToken( token, 2, ref tokenDuplicate ) != 0 ) {
                            var tempWindowsIdentity = new WindowsIdentity( userToken: tokenDuplicate );
                            this._impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                        else {
                            throw new Win32Exception( Marshal.GetLastWin32Error() );
                        }
                    }
                    else {
                        throw new Win32Exception( Marshal.GetLastWin32Error() );
                    }
                }
                else {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
            }
            finally {
                if ( token != IntPtr.Zero ) {
                    CloseHandle( token );
                }
                if ( tokenDuplicate != IntPtr.Zero ) {
                    CloseHandle( tokenDuplicate );
                }
            }
        }
    }
}