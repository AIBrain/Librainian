// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "DefragExtensions.cs" last formatted on 2022-12-22 at 5:15 PM by Protiguous.

namespace Librainian.FileSystem;

using System;
using System.Diagnostics;
using System.IO;
using ComputerSystem.Devices;
using OperatingSystem;

public static class DefragExtensions {

	/// <summary>
	///     The function starts the Defrag.Exe and waits for it to finish. It ensures the process is run with lower
	///     priority and the spawned process DfrgNtfs is given 'Idle' priority
	/// </summary>
	/// <param name="disk">Drive to defrag - format is "c:" for example</param>
	private static String Defrag( Disk? disk ) {
		var path = Path.Combine( Windows.WindowsSystem32Folder.Value.FullPath, "defrag.exe" );

		var info = new ProcessStartInfo {
			FileName = path,
			Arguments = String.Format( "{{{0}}} /O /V /M " + Environment.ProcessorCount, disk ),
			UseShellExecute = false,
			CreateNoWindow = true,
			RedirectStandardOutput = true
		};

		var defrag = Process.Start( info );

		if ( defrag is null ) {
			return String.Empty;
		}

		defrag.PriorityClass = ProcessPriorityClass.Idle;
		defrag.WaitForExit();

		return defrag.StandardOutput.ReadToEnd();
	}
}