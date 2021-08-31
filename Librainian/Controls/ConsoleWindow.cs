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
// File "ConsoleWindow.cs" last touched on 2021-07-17 at 2:05 PM by Protiguous.

namespace Librainian.Controls {

	using System;
	using System.Runtime.InteropServices;
	using OperatingSystem;

	/// <summary>Pulled from https://stackoverflow.com/a/24040827/956364</summary>
	public static class ConsoleWindow {

		private const Int32 ATTACH_PARENT_PROCESS = -1;

		private const Int32 MF_BYCOMMAND = 0x00000000;

		private const Int32 MF_GRAYED = 1;

		private const UInt32 SC_ARRANGE = 0xF110;

		private const UInt32 SC_CLOSE = 0xF060;

		private const UInt32 SC_CONTEXTHELP = 0xF180;

		private const UInt32 SC_DEFAULT = 0xF160;

		private const UInt32 SC_HOTKEY = 0xF150;

		private const UInt32 SC_HSCROLL = 0xF080;

		private const UInt32 SC_KEYMENU = 0xF100;

		private const UInt32 SC_MAXIMIZE = 0xF030;

		private const UInt32 SC_MINIMIZE = 0xF020;

		private const UInt32 SC_MONITORPOWER = 0xF170;

		private const UInt32 SC_MOUSEMENU = 0xF090;

		private const UInt32 SC_MOVE = 0xF010;

		private const UInt32 SC_NEXTWINDOW = 0xF040;

		private const UInt32 SC_PREVWINDOW = 0xF050;

		private const UInt32 SC_RESTORE = 0xF120;

		private const UInt32 SC_SCREENSAVE = 0xF140;

		private const UInt32 SC_SEPARATOR = 0xF00F;

		private const UInt32 SC_SIZE = 0xF000;

		private const UInt32 SC_TASKLIST = 0xF130;

		private const UInt32 SC_VSCROLL = 0xF070;

		private const Int32 SW_HIDE = 0;

		private const Int32 SW_SHOW = 5;

		public static IntPtr WindowHandle { get; set; } = IntPtr.Zero;

		[DllImport( DLL.Kernel32, SetLastError = true, ExactSpelling = false )]
		private static extern Boolean AllocConsole();

		[DllImport( DLL.Kernel32, ExactSpelling = false )]
		private static extern Boolean AttachConsole( Int32 dwProcessId );

		/// <summary>
		///     <code>DeleteMenu(GetSystemMenu(GetConsoleWindow(), false),SC_CLOSE, MF_BYCOMMAND);</code>
		/// </summary>
		/// <param name="hMenu"></param>
		/// <param name="nPosition"></param>
		/// <param name="wFlags"></param>
		[DllImport( DLL.Kernel32 )]
		private static extern Int32 DeleteMenu( IntPtr hMenu, Int32 nPosition, Int32 wFlags );

		[DllImport( DLL.Kernel32, ExactSpelling = false )]
		private static extern Boolean EnableMenuItem( IntPtr hMenu, UInt32 uIDEnableItem, UInt32 uEnable );

		[DllImport( DLL.Kernel32, ExactSpelling = true )]
		private static extern IntPtr GetConsoleWindow();

		[DllImport( DLL.Kernel32, ExactSpelling = false )]
		private static extern IntPtr GetSystemMenu( IntPtr hWnd, Boolean bRevert );

		[DllImport( DLL.Kernel32, SetLastError = true, ExactSpelling = false )]
		private static extern Boolean SetConsoleIcon( IntPtr hIcon );

		[DllImport( DLL.User32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false )]
		private static extern Boolean SetWindowText( IntPtr hwnd, String? lpString );

		[DllImport( DLL.User32, ExactSpelling = false )]
		private static extern Boolean ShowWindow( IntPtr hWnd, Int32 nCmdShow );

		public static Boolean AllocateWindow() {
			try {
				return AllocConsole();
			}
			finally {
				GetWindow();
			}
		}

		/// <summary>redirect console output to parent process; must be called before any calls to Console.WriteLine()</summary>
		public static void AttachConsoleWindow() => AttachConsole( ATTACH_PARENT_PROCESS );

		public static void DisableCloseButton() {
			var systemMenu = GetSystemMenu( GetWindow(), false );

			//TODO How does one re-enable the close button?
			EnableMenuItem( systemMenu, SC_CLOSE, MF_GRAYED );
		}

		public static IntPtr GetWindow() => WindowHandle = GetConsoleWindow();

		public static void HideWindow() => ShowWindow( GetWindow(), SW_HIDE );

		public static void SetTitle( String? text ) => SetWindowText( GetWindow(), text );

		public static void ShowWindow() {
			GetWindow();

			if ( WindowHandle == IntPtr.Zero ) {
				AllocateWindow();
			}
			else {
				ShowWindow( WindowHandle, SW_SHOW );
			}
		}
	}
}