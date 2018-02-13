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
    using OperatingSystem;

    /// <summary>
    /// </summary>
    public class Impersonator : ABetterClassDispose {
        private const Int32 Logon32LogonInteractive = 2;

        private const Int32 Logon32ProviderDefault = 0;

        private WindowsImpersonationContext _impersonationContext;

		public Impersonator( String userName, String domainName, String password ) => this.ImpersonateValidUser( userName, domainName, password );

		protected override void DisposeManaged() => this._impersonationContext?.Undo();

		private void ImpersonateValidUser( String userName, String domain, String password ) {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            try {
                if ( !NativeMethods.RevertToSelf() ) {
                    throw new Win32Exception( Marshal.GetLastWin32Error() );
                }
                else {
                    if ( NativeMethods.LogonUser( userName, domain, password, Logon32LogonInteractive, Logon32ProviderDefault, ref token ) == 0 ) {
                        throw new Win32Exception( Marshal.GetLastWin32Error() );
                    }
                    else {
                        if ( NativeMethods.DuplicateToken( token, 2, ref tokenDuplicate ) == 0 ) {
                            throw new Win32Exception( Marshal.GetLastWin32Error() );
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
                   NativeMethods.CloseHandle( token );
                }
                if ( tokenDuplicate != IntPtr.Zero ) {
                    NativeMethods.CloseHandle( tokenDuplicate );
                }
            }
        }
    }
}
