// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Utility.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "Utility.cs" was last formatted by Protiguous on 2019/12/11 at 5:40 AM.

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using JetBrains.Annotations;
    using Logging;

    public static class Utility {

        private static ReaderWriterLockSlim ConsoleOutputSynch { get; } = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        public static DummyXMLResolver DummyXMLResolver { get; } = new DummyXMLResolver();

        /// <summary>Output the <paramref name="text" /> at the end of the current <see cref="Console" /> line.</summary>
        /// <param name="text">   </param>
        /// <param name="yOffset"></param>
        public static void AtEndOfLine( [CanBeNull] this String text, Int32 yOffset = 0 ) {
            if ( String.IsNullOrEmpty( text ) ) {
                return;
            }

            try {
                ConsoleOutputSynch.EnterUpgradeableReadLock();
                var oldTop = Console.CursorTop;
                var oldLeft = Console.CursorLeft;

                try {
                    ConsoleOutputSynch.EnterWriteLock();
                    Console.CursorVisible = false;
                    yOffset = oldTop + yOffset;

                    while ( yOffset < 0 ) {
                        yOffset++;
                    }

                    while ( yOffset >= Console.WindowHeight ) {
                        yOffset--;
                    }

                    Console.SetCursorPosition( left: Console.WindowWidth - ( text.Length + 2 ), top: yOffset );
                    Console.Write( text );
                    Console.SetCursorPosition( left: oldLeft, top: oldTop );
                    Console.CursorVisible = true;
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.Log();
                }
                catch ( IOException exception ) {
                    exception.Log();
                }
                catch ( SecurityException exception ) {
                    exception.Log();
                }
                finally {
                    ConsoleOutputSynch.ExitWriteLock();
                }
            }
            finally {
                ConsoleOutputSynch.ExitUpgradeableReadLock();
            }
        }

        //    Console.SetCursorPosition( left: Console.WindowWidth - ( text.Length + 1 ), top: 0 );
        //    Console.Write( text );
        //    Console.SetCursorPosition( left: oldLeft, top: oldTop );
        //}

        public static void OnSet<T>( this EventHandler<T> @event, Object sender, T e ) where T : EventArgs =>
            throw new NotImplementedException(); //if ( @event != null ) { @event( sender, e ); }

        //    return false;
        //}

        public static void Spin( String text ) {
            var oldTop = Console.CursorTop;
            var oldLeft = Console.CursorLeft;
            Console.Write( text );
            Console.SetCursorPosition( left: oldLeft, top: oldTop );
        }

        //public static void TopRight( String text ) {
        //    var oldTop = Console.CursorTop;
        //    var oldLeft = Console.CursorLeft;
        //public static void Log( this Exception exception ) {
        //    if ( Debugger.IsAttached ) {
        //        Debugger.Break();
        //    }
        //    Debug.WriteLine( String.Format( "[Exception: {0}]", exception.Message ) );
        //    Debug.Indent();
        //    Debug.WriteLine( String.Format( "[In: {0}]", exception.Source ) );
        //    Debug.WriteLine( String.Format( "[Msg: {0}]", exception.Message ) );
        //    Debug.Unindent();
        //}

        //public static void LogError( String message ) {
        //    if ( Debugger.IsAttached ) { Debugger.Break(); }
        //    Debug.WriteLine( "[Error: " + message + "]" );
        //
        //}

        //public static void LogError( Exception error ) {
        //    if ( Debugger.IsAttached ) { Debugger.Break(); }
        //    Debug.WriteLine( "[Error: " );
        //    Debug.Indent();
        //    Debug.WriteLine( error );
        //    Debug.Unindent();
        //    Debug.WriteLine( "]" );
        //
        //}

        //public static void LogWarning( String message ) {
        //    Debug.WriteLine( "[Warning: " + message + "]" );
        //}

        //public static void LogWarning( Exception exception ) {
        //    Debug.WriteLine( "[Warning: " );
        //    Debug.Indent();
        //    Debug.WriteLine( exception );
        //    Debug.Unindent();
        //    Debug.WriteLine( "]" );
        //}

        public static void TopRight( [CanBeNull] String text ) {
            if ( String.IsNullOrEmpty( text ) ) {
                return;
            }

            try {
                ConsoleOutputSynch.EnterUpgradeableReadLock();
                var oldTop = Console.CursorTop;
                var oldLeft = Console.CursorLeft;

                try {
                    ConsoleOutputSynch.EnterWriteLock();
                    Console.CursorVisible = false;
                    Console.SetCursorPosition( left: Console.WindowWidth - ( text.Length + 2 ), top: 0 );
                    Console.Write( text );
                    Console.SetCursorPosition( left: oldLeft, top: oldTop );
                    Console.CursorVisible = true;
                }
                finally {
                    ConsoleOutputSynch.ExitWriteLock();
                }
            }
            finally {
                ConsoleOutputSynch.ExitUpgradeableReadLock();
            }
        }

        public static void WriteColor( this String text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black,
            [CanBeNull] params Object[] parms ) {
            lock ( ConsoleOutputSynch ) {
                if ( parms?.Any() != true ) {

                    //text.Info();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor; //TODO d.r.y.
                    Console.BackgroundColor = backColor; //TODO d.r.y.
                    Console.Write( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {

                    //String.Format( text, parms ).Info();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    Console.Write( text, parms );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
            }
        }

        public static void WriteLineColor( this String text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black,
            [CanBeNull] params Object[] parms ) {
            lock ( ConsoleOutputSynch ) {
                if ( parms?.Any() != true ) {

                    //text.Info();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    Console.WriteLine( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {

                    //String.Format( text, parms ).Info();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    Console.WriteLine( text, parms );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
            }
        }

    }

}