// Copyright 2018 Rick@AIBrain.org.
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
// "Librainian/Extensions.cs" was last cleaned by Rick on 2018/03/08 at 9:07 PM

namespace Librainian.OperatingSystem {
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;

    public static class Extensions {
        [ CanBeNull ]
        public static String Execute( String command ) {
            var str = "";

            using ( var process = Process.Start( startInfo: new ProcessStartInfo( "cmd", arguments: "/c " + command ) { RedirectStandardOutput = true, UseShellExecute = false, RedirectStandardError = true, CreateNoWindow = true } ) ) {
                using ( var standardOutput = process?.StandardOutput ) {
                    str = standardOutput?.ReadToEnd();
                }

                process?.WaitForExit();
            }

            return str?.Trim();
        }
    }
}