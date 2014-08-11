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
// "Librainian/Impersonator.cs" was last cleaned by Rick on 2014/08/11 at 12:40 AM
#endregion

namespace Librainian.Security {
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    /// <summary>
    /// </summary>
    /// <see cref="http://mypassivedollar.com/csharp-impersonator/" />
    public class Impersonator : IDisposable {
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private WindowsImpersonationContext _impersonationContext;

        public Impersonator( string userName, string domainName, string password ) {
            this.ImpersonateValidUser( userName, domainName, password );
        }

        public void Dispose() {
            this.UndoImpersonation();
        }

        [DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
        private static extern Boolean CloseHandle( IntPtr handle );

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern int DuplicateToken( IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken );

        [DllImport( "advapi32.dll", SetLastError = true )]
        private static extern int LogonUser( string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken );

        [DllImport( "advapi32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        private static extern Boolean RevertToSelf();

        private void ImpersonateValidUser( string userName, string domain, string password ) {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            try {
                if ( RevertToSelf() ) {
                    if ( LogonUser( userName, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token ) != 0 ) {
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

        private void UndoImpersonation() {
            if ( this._impersonationContext != null ) {
                this._impersonationContext.Undo();
            }
        }
    }
}
