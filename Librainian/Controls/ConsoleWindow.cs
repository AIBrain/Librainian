// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ConsoleWindow.cs" last formatted on 2020-08-14 at 8:32 PM.

namespace Librainian.Controls {

	using System;
	using System.Runtime.InteropServices;
	using JetBrains.Annotations;

	/// <summary>Pulled from https://stackoverflow.com/a/24040827/956364</summary>
	public static class ConsoleWindow {

		private const Int32 ATTACH_PARENT_PROCESS = -1;

		private const Int32 MF_GRAYED = 1;

		private const Int32 SC_CLOSE = 0xF060;

		private const Int32 SW_HIDE = 0;

		private const Int32 SW_SHOW = 5;

		[DllImport( WindowsDLL.Kernel32, SetLastError = true, ExactSpelling = false )]
		private static extern Boolean AllocConsole();

		[DllImport( WindowsDLL.Kernel32, ExactSpelling = false )]
		private static extern Boolean AttachConsole( Int32 dwProcessId );

		[DllImport( "user32.dll", ExactSpelling = false )]
		private static extern Boolean EnableMenuItem( IntPtr hMenu, UInt32 uIDEnableItem, UInt32 uEnable );

		[DllImport( WindowsDLL.Kernel32, ExactSpelling = false )]
		private static extern IntPtr GetConsoleWindow();

		[DllImport( "user32.dll", ExactSpelling = false )]
		private static extern IntPtr GetSystemMenu( IntPtr hWnd, Boolean bRevert );

		[DllImport( WindowsDLL.Kernel32, SetLastError = true, ExactSpelling = false )]
		private static extern Boolean SetConsoleIcon( IntPtr hIcon );

		[DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false )]
		private static extern Boolean SetWindowText( IntPtr hwnd, String lpString );

		[DllImport( "user32.dll", ExactSpelling = false )]
		private static extern Boolean ShowWindow( IntPtr hWnd, Int32 nCmdShow );

		/// <summary>redirect console output to parent process; must be called before any calls to Console.WriteLine()</summary>
		public static void AttachConsoleWindow() => AttachConsole( ATTACH_PARENT_PROCESS );

		public static void DisableCloseButton() {
			var handle = GetConsoleWindow();

			var hmenu = GetSystemMenu( handle, false );

			EnableMenuItem( hmenu, SC_CLOSE, MF_GRAYED );
		}

		public static void HideWindow() {
			var handle = GetConsoleWindow();

			ShowWindow( handle, SW_HIDE );
		}

		public static void SetText( [CanBeNull] String? text ) {
			var handle = GetConsoleWindow();

			SetWindowText( handle, text );
		}

		public static void ShowWindow() {
			var handle = GetConsoleWindow();

			if ( handle == IntPtr.Zero ) {
				AllocConsole();
			}
			else {
				ShowWindow( handle, SW_SHOW );
			}
		}

	}

	public static class WindowsDLL {

		public const String Kernel32 = "kernel32.dll";

	}

}