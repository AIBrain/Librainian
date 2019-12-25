// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Impersonator.cs" belongs to Protiguous@Protiguous.com and
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
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Impersonator.cs" was last formatted by Protiguous on 2019/08/08 at 9:34 AM.

namespace LibrainianCore.Security {

    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using OperatingSystem;
    using Utilities;

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
                    if ( NativeMethods.LogonUser( lpszUserName: userName, lpszDomain: domain, lpszPassword: password, dwLogonType: Logon32LogonInteractive,
                             dwLogonProvider: Logon32ProviderDefault, phToken: ref token ) == 0 ) {
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
                    token.CloseHandle();
                }

                if ( tokenDuplicate != IntPtr.Zero ) {
                    tokenDuplicate.CloseHandle();
                }
            }
        }

        public override void DisposeManaged() => this._impersonationContext?.Undo();
    }
}