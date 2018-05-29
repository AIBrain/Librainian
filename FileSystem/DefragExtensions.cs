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
// "Librainian/DefragExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:51 PM

namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;
    using System.IO;
    using OperatingSystem;

    public class DefragExtensions {

        /// <summary>
        ///     The function starts the Defrag.Exe and waits for it to finish. It ensures the process is
        ///     run with lower priority and the spawned process DfrgNtfs is given 'Idle' priority
        /// </summary>
        /// <param name="drive">Drive to defrag - format is "c:" for example</param>
        private static String Defrag( Drive drive ) {
            var path = Path.Combine( Windows.WindowsSystem32Folder.Value.FullName, "defrag.exe" );
            var info = new ProcessStartInfo {
                FileName = path,
                Arguments = String.Format( "{{{0}}} /O /V /M " + Environment.ProcessorCount, drive ),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            var defrag = Process.Start( info );

            if ( defrag == null ) {
                return String.Empty;
            }

            defrag.PriorityClass = ProcessPriorityClass.Idle;
            defrag.WaitForExit();

            return defrag.StandardOutput.ReadToEnd();
        }
    }
}