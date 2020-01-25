// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ConsoleWindow.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "ConsoleWindow.cs" was last formatted by Protiguous on 2019/08/08 at 6:43 AM.

namespace Librainian.Controls {

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;

    /// <summary>
    ///     Pulled from https://stackoverflow.com/a/24040827/956364
    /// </summary>
    public static class ConsoleWindow {

        private const Int32 ATTACH_PARENT_PROCESS = -1;

        private const Int32 MF_GRAYED = 1;

        private const Int32 SC_CLOSE = 0xF060;

        private const Int32 SW_HIDE = 0;

        private const Int32 SW_SHOW = 5;

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = false )]
        private static extern Boolean AllocConsole();

        [DllImport( "kernel32.dll", ExactSpelling = false )]
        private static extern Boolean AttachConsole( Int32 dwProcessId );

        [DllImport( "user32.dll", ExactSpelling = false )]
        private static extern Boolean EnableMenuItem( IntPtr hMenu, UInt32 uIDEnableItem, UInt32 uEnable );

        [DllImport( "kernel32.dll", ExactSpelling = false )]
        private static extern IntPtr GetConsoleWindow();

        [DllImport( "user32.dll", ExactSpelling = false )]
        private static extern IntPtr GetSystemMenu( IntPtr hWnd, Boolean bRevert );

        [DllImport( "kernel32.dll", SetLastError = true, ExactSpelling = false )]
        private static extern Boolean SetConsoleIcon( IntPtr hIcon );

        [DllImport( "user32.dll", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false )]
        private static extern Boolean SetWindowText( IntPtr hwnd, String lpString );

        [DllImport( "user32.dll", ExactSpelling = false )]
        private static extern Boolean ShowWindow( IntPtr hWnd, Int32 nCmdShow );

        /// <summary>
        ///     redirect console output to parent process;
        ///     must be called before any calls to Console.WriteLine()
        /// </summary>
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

        public static void SetIcon( [NotNull] this Icon icon ) {
            if ( icon is null ) {
                throw new ArgumentNullException(  nameof( icon ) );
            }

            SetConsoleIcon( icon.Handle );
        }

        public static void SetText( [CanBeNull] String text ) {
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
}