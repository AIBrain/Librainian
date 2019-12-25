// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "OSExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "OSExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 9:20 AM.

namespace LibrainianCore.OperatingSystem {

    using System;
    using System.Diagnostics;
    using Logging;

    public static class OSExtensions {

        /// <summary>
        ///     Execute the <paramref name="command" /> in a CMD.EXE context.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String CmdExecute( this String command ) {
            using ( var process = Process.Start( startInfo: new ProcessStartInfo( fileName: "cmd.exe", arguments: $"/c {command}" ) {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            } ) ) {
                using ( var standardOutput = process?.StandardOutput ) {
                    return standardOutput?.ReadToEnd().Trim();
                }
            }
        }

        /// <summary>
        ///     Copy the ToString() of <paramref name="value" /> to the <see cref="Clipboard" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="format"></param>
        public static void CopyToClipboard<T>( [CanBeNull] this T value, TextDataFormat format = TextDataFormat.UnicodeText ) {
            try {
                if ( value is null ) {
                    return;
                }

                var text = value.ToString();

                if ( !String.IsNullOrEmpty( text ) ) {
                    Clipboard.SetText( text, format );
                }
            }
            catch ( Exception exception ) {
                exception.Log();
            }
        }
    }
}