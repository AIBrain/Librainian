// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "DefragExtensions.cs",
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
// "Librainian/Librainian/DefragExtensions.cs" was last cleaned by Protiguous on 2018/05/15 at 10:41 PM.

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

            if ( defrag is null ) { return String.Empty; }

            defrag.PriorityClass = ProcessPriorityClass.Idle;
            defrag.WaitForExit();

            return defrag.StandardOutput.ReadToEnd();
        }
    }
}