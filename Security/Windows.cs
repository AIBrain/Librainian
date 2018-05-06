// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Windows.cs" was last cleaned by Rick on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.Security;
    using System.Security.Principal;

    public static class Windows {

        /// <summary>Determine if the current user is in the role of <see cref="WindowsBuiltInRole" />.</summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static Boolean IsUserInRole( this WindowsBuiltInRole role ) {
            try {
                using ( var windowsIdentity = WindowsIdentity.GetCurrent() ) {
                    var windowsPrincipal = new WindowsPrincipal( ntIdentity: windowsIdentity );
                    return windowsPrincipal.IsInRole( role: role );
                }
            }
            catch ( SecurityException ) { }
            catch ( ArgumentNullException ) { }
            catch ( ArgumentException ) { }

            return false;
        }
    }
}