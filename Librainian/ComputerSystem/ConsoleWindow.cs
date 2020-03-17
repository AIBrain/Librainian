// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConsoleWindow.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "ConsoleWindow.cs" was last formatted by Protiguous on 2020/03/16 at 2:53 PM.

namespace Librainian.ComputerSystem {

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    public static class ConsoleWindow {

        private const Int32 MY_CODE_PAGE = 437;

        private const Int32 STD_ERROR_HANDLE = -12;

        private const Int32 STD_OUTPUT_HANDLE = -11;

        private static readonly IntPtr InvalidHandleValue = new IntPtr( -1 );

        public static Boolean IsConsoleVisible { get; set; }

        [Flags]
        public enum DesiredAccess : UInt32 {

            GenericRead = 0x80000000,

            GenericWrite = 0x40000000,

            GenericExecute = 0x20000000,

            GenericAll = 0x10000000
        }

        public enum StdHandle {

            Input = -10,

            Output = -11,

            Error = -12
        }

        [DllImport( "kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        public static extern Int32 AllocConsole();

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern IntPtr CreateFile( String lpFileName, [MarshalAs( UnmanagedType.U4 )] DesiredAccess dwDesiredAccess,
            [MarshalAs( UnmanagedType.U4 )] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs( UnmanagedType.U4 )] FileMode dwCreationDisposition,
            [MarshalAs( UnmanagedType.U4 )] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern Boolean FreeConsole();

        public static IntPtr GetConsoleStandardError() {
            var handle = CreateFile( "CONERR$", DesiredAccess.GenericWrite | DesiredAccess.GenericWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open,
                FileAttributes.Normal, IntPtr.Zero );

            return handle == InvalidHandleValue ? InvalidHandleValue : handle;
        }

        public static IntPtr GetConsoleStandardInput() {
            var handle = CreateFile( "CONIN$", DesiredAccess.GenericRead | DesiredAccess.GenericWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal,
                IntPtr.Zero );

            return handle == InvalidHandleValue ? InvalidHandleValue : handle;
        }

        public static IntPtr GetConsoleStandardOutput() {
            var handle = CreateFile( "CONOUT$", DesiredAccess.GenericWrite | DesiredAccess.GenericWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open,
                FileAttributes.Normal, IntPtr.Zero );

            return handle == InvalidHandleValue ? InvalidHandleValue : handle;
        }

        [DllImport( "kernel32.dll" )]
        public static extern IntPtr GetConsoleWindow();

        [DllImport( "kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        public static extern IntPtr GetStdHandle( Int32 nStdHandle );

        public static void Hide() => FreeConsole();

        public static void Maximize() {
            var p = Process.GetCurrentProcess();
            ShowWindow( p.MainWindowHandle, 3 ); //SW_MAXIMIZE = 3
        }

        [DllImport( "user32.dll" )]
        public static extern Boolean MoveWindow( IntPtr hWnd, Int32 X, Int32 Y, Int32 nWidth, Int32 nHeight, Boolean bRepaint );

        [DllImport( "kernel32.dll" )]
        public static extern Boolean SetConsoleScreenBufferSize( IntPtr hConsoleOutput, Point size );

        [DllImport( "kernel32.dll" )]
        public static extern Boolean SetStdHandle( StdHandle nStdHandle, IntPtr hHandle );

        public static void Show( Int32 bufferWidth = -1, Boolean breakRedirection = true, Int32 bufferHeight = 1600, Int32 screenNum = -1 /*-1 = Any but primary*/ ) {
            AllocConsole();
            var stdOut = InvalidHandleValue;

            if ( breakRedirection ) {
                UnredirectConsole( out stdOut, out _, out _ );
            }

            var outStream = Console.OpenStandardOutput();
            var errStream = Console.OpenStandardError();
            var encoding = Encoding.GetEncoding( MY_CODE_PAGE );
            StreamWriter standardOutput = new StreamWriter( outStream, encoding ), standardError = new StreamWriter( errStream, encoding );
            Screen screen = null;

            try {
                if ( screenNum < 0 ) {
                    screen = Screen.AllScreens.FirstOrDefault( s => !s.Primary );
                }
                else {
                    screen = Screen.AllScreens[ Math.Min( screenNum, Screen.AllScreens.Length - 1 ) ];
                }
            }
            catch ( Exception ) { }

            if ( bufferWidth == -1 ) {
                if ( screen is null ) {
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
                    var coord = new Point {
                        X = bufferWidth,
                        Y = bufferHeight
                    };

                    SetConsoleScreenBufferSize( stdOut, coord );
                }
                else {
                    Console.SetBufferSize( bufferWidth, bufferHeight );
                }
            }
            catch ( Exception e ) // Could be redirected
            {
                Debug.WriteLine( e.ToString() );
            }

            try {
                if ( screen != null ) {
                    var workingArea = screen.WorkingArea;
                    var hConsole = GetConsoleWindow();
                    MoveWindow( hConsole, workingArea.Left, workingArea.Top, workingArea.Width / 2, workingArea.Height / 2, true );
                }
            }
            catch ( Exception e ) // Could be redirected
            {
                Debug.WriteLine( e.ToString() );
            }
        }

        [DllImport( "user32.dll" )]
        public static extern Boolean ShowWindow( IntPtr hWnd, Int32 cmdShow );

        public static void UnredirectConsole( out IntPtr stdOut, out IntPtr stdIn, out IntPtr stdErr ) {
            SetStdHandle( StdHandle.Output, stdOut = GetConsoleStandardOutput() );
            SetStdHandle( StdHandle.Input, stdIn = GetConsoleStandardInput() );
            SetStdHandle( StdHandle.Error, stdErr = GetConsoleStandardError() );
        }
    }
}