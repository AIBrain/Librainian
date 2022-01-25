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
// File "ConsoleWindow2.cs" last formatted on 2022-12-22 at 5:14 PM by Protiguous.

namespace Librainian.Controls;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Logging;

public class ConsoleWindow2 {

	private const Int32 MY_CODE_PAGE = 437;

	private const Int32 STD_ERROR_HANDLE = -12;

	private const Int32 STD_OUTPUT_HANDLE = -11;

	private static readonly IntPtr InvalidHandleValue = new( -1 );

	private enum StdHandle {

		Input = -10,

		Output = -11,

		Error = -12
	}

	[Flags]
	public enum DesiredAccess : UInt32 {

		GenericRead = 0x80000000,

		GenericWrite = 0x40000000,

		GenericExecute = 0x20000000,

		GenericAll = 0x10000000
	}

	[DllImport( "kernel32.dll", EntryPoint = nameof( AllocConsole ), SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
	private static extern Int32 AllocConsole();

	[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
	private static extern IntPtr CreateFile(
		String lpFileName,
		[MarshalAs( UnmanagedType.U4 )] DesiredAccess dwDesiredAccess,
		[MarshalAs( UnmanagedType.U4 )] FileShare dwShareMode,
		IntPtr lpSecurityAttributes,
		[MarshalAs( UnmanagedType.U4 )] FileMode dwCreationDisposition,
		[MarshalAs( UnmanagedType.U4 )] FileAttributes dwFlagsAndAttributes,
		IntPtr hTemplateFile
	);

	private static IntPtr GetConsoleStandardError() {
		var handle = CreateFile( "CONERR$", DesiredAccess.GenericWrite | DesiredAccess.GenericWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal,
			IntPtr.Zero );
		if ( handle == InvalidHandleValue ) {
			return InvalidHandleValue;
		}

		return handle;
	}

	private static IntPtr GetConsoleStandardInput() {
		var handle = CreateFile( "CONIN$", DesiredAccess.GenericRead | DesiredAccess.GenericWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal,
			IntPtr.Zero );
		if ( handle == InvalidHandleValue ) {
			return InvalidHandleValue;
		}

		return handle;
	}

	private static IntPtr GetConsoleStandardOutput() {
		var handle = CreateFile( "CONOUT$", DesiredAccess.GenericWrite | DesiredAccess.GenericWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal,
			IntPtr.Zero );
		if ( handle == InvalidHandleValue ) {
			return InvalidHandleValue;
		}

		return handle;
	}

	[DllImport( "kernel32.dll", EntryPoint = nameof( GetStdHandle ), SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
	private static extern IntPtr GetStdHandle( Int32 nStdHandle );

	[DllImport( "kernel32.dll" )]
	private static extern Boolean SetStdHandle( StdHandle nStdHandle, IntPtr hHandle );

	[DllImport( "kernel32.dll", EntryPoint = nameof( FreeConsole ), CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	protected static extern Boolean FreeConsole();

	[DllImport( "kernel32.dll" )]
	public static extern IntPtr GetConsoleWindow();

	public static void Hide() => FreeConsole();

	public static void Maximize() {
		var p = Process.GetCurrentProcess();
		ShowWindow( p.MainWindowHandle, 3 ); //SW_MAXIMIZE = 3
	}

	[DllImport( "user32.dll" )]
	public static extern Boolean MoveWindow( IntPtr hWnd, Int32 X, Int32 Y, Int32 nWidth, Int32 nHeight, Boolean bRepaint );

	[DllImport( "kernel32.dll" )]
	public static extern Boolean SetConsoleScreenBufferSize( IntPtr hConsoleOutput, COORD size );

	public static void Show( Int32 bufferWidth = -1, Boolean breakRedirection = true, Int32 bufferHeight = 1600, Int32 screenNum = -1 /*-1 = Any but primary*/ ) {
		_ = AllocConsole();
		var stdOut = InvalidHandleValue;
		if ( breakRedirection ) {
			UnredirectConsole( out stdOut, out var stdIn, out var stdErr );
		}

		var outStream = Console.OpenStandardOutput();
		var errStream = Console.OpenStandardError();
		var encoding = Encoding.GetEncoding( MY_CODE_PAGE );
		StreamWriter standardOutput = new( outStream, encoding ), standardError = new( errStream, encoding );
		Screen? screen = null;
		try {
			screen = screenNum switch {
				< 0 => Screen.AllScreens.FirstOrDefault( s => !s.Primary ),
				var _ => Screen.AllScreens[ Math.Min( screenNum, Screen.AllScreens.Length - 1 ) ]
			};
		}
		catch ( Exception exception ) {
			exception.Log();
		}

		if ( bufferWidth == -1 ) {
			if ( screen == null ) {
				bufferWidth = 180;
			}
			else {
				bufferWidth = screen.WorkingArea.Width / 10;
				if ( bufferWidth > 15 ) {
					bufferWidth -= 5;
				}
				else {
					bufferWidth = 10;
				}
			}
		}

		try {
			standardOutput.AutoFlush = true;
			standardError.AutoFlush = true;
			Console.SetOut( standardOutput );
			Console.SetError( standardError );
			if ( breakRedirection ) {
				var coord = new COORD {
					X = ( Int16 )bufferWidth,
					Y = ( Int16 )bufferHeight
				};
				SetConsoleScreenBufferSize( stdOut, coord );
			}
			else {
				Console.SetBufferSize( bufferWidth, bufferHeight );
			}
		}
		catch ( Exception exception ) // Could be redirected
		{
			exception.Log();
		}

		try {
			if ( screen != null ) {
				var workingArea = screen.WorkingArea;
				var hConsole = GetConsoleWindow();
				MoveWindow( hConsole, workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height, true );
			}
		}
		catch ( Exception exception ) // Could be redirected
		{
			exception.Log();
		}
	}

	[DllImport( "user32.dll" )]
	public static extern Boolean ShowWindow( IntPtr hWnd, Int32 cmdShow );

	public static void UnredirectConsole( out IntPtr stdOut, out IntPtr stdIn, out IntPtr stdErr ) {
		SetStdHandle( StdHandle.Output, stdOut = GetConsoleStandardOutput() );
		SetStdHandle( StdHandle.Input, stdIn = GetConsoleStandardInput() );
		SetStdHandle( StdHandle.Error, stdErr = GetConsoleStandardError() );
	}

	public struct COORD {

		public Int16 X { get; set; }

		public Int16 Y { get; set; }
	}
}