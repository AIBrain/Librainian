// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Extensions.cs" was last cleaned by Protiguous on 2018/05/06 at 12:21 PM

namespace Librainian.OperatingSystem {

    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

    public static class OSExtensions {

        /// <summary>
        ///     Execute the <paramref name="command" /> in a CMD.EXE context.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String CmdExecute( this String command ) {
            using ( var process = Process.Start(
                startInfo: new ProcessStartInfo( fileName: "cmd", arguments: "/c " + command ) { RedirectStandardOutput = true, UseShellExecute = false, RedirectStandardError = true, CreateNoWindow = true } ) ) {
                using ( var standardOutput = process?.StandardOutput ) {
                    return standardOutput?.ReadToEnd()?.Trim();
                }
            }
        }
    }
}