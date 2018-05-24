// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Utility.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Utility.cs" was last formatted by Protiguous on 2018/05/23 at 9:06 PM.

namespace Librainian.Extensions {

    using System;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;

    public static class Utility {

        private static ReaderWriterLockSlim ConsoleOutputSynch { get; } = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        public static DummyXMLResolver DummyXMLResolver { get; } = new DummyXMLResolver();

        /// <summary>
        ///     Output the <paramref name="text" /> at the end of the current <seealso cref="Console" /> line.
        /// </summary>
        /// <param name="text">   </param>
        /// <param name="yOffset"></param>
        public static void AtEndOfLine( this String text, Int32 yOffset = 0 ) {
            if ( String.IsNullOrEmpty( text ) ) { return; }

            try {
                ConsoleOutputSynch.EnterUpgradeableReadLock();
                var oldTop = Console.CursorTop;
                var oldLeft = Console.CursorLeft;

                try {
                    ConsoleOutputSynch.EnterWriteLock();
                    Console.CursorVisible = false;
                    yOffset = oldTop + yOffset;

                    while ( yOffset < 0 ) { yOffset++; }

                    while ( yOffset >= Console.WindowHeight ) { yOffset--; }

                    Console.SetCursorPosition( left: Console.WindowWidth - ( text.Length + 2 ), top: yOffset );
                    Console.Write( text );
                    Console.SetCursorPosition( left: oldLeft, top: oldTop );
                    Console.CursorVisible = true;
                }
                catch ( ArgumentOutOfRangeException exception ) { exception.More(); }
                catch ( IOException exception ) { exception.More(); }
                catch ( SecurityException exception ) { exception.More(); }
                finally { ConsoleOutputSynch.ExitWriteLock(); }
            }
            finally { ConsoleOutputSynch.ExitUpgradeableReadLock(); }
        }

        //    Console.SetCursorPosition( left: Console.WindowWidth - ( text.Length + 1 ), top: 0 );
        //    Console.Write( text );
        //    Console.SetCursorPosition( left: oldLeft, top: oldTop );
        //}

        public static void OnSet<T>( this EventHandler<T> @event, Object sender, T e ) where T : EventArgs => throw new NotImplementedException(); //if ( @event != null ) { @event( sender, e ); }

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

        public static void TopRight( String text ) {
            if ( String.IsNullOrEmpty( text ) ) { return; }

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
                finally { ConsoleOutputSynch.ExitWriteLock(); }
            }
            finally { ConsoleOutputSynch.ExitUpgradeableReadLock(); }
        }

        public static void WriteColor( this String text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params Object[] parms ) {
            lock ( ConsoleOutputSynch ) {
                if ( parms?.Any() != true ) {

                    //text.WriteLine();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor; //TODO d.r.y.
                    Console.BackgroundColor = backColor; //TODO d.r.y.
                    Console.Write( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {

                    //String.Format( text, parms ).WriteLine();
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

        public static void WriteLineColor( this String text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params Object[] parms ) {
            lock ( ConsoleOutputSynch ) {
                if ( parms?.Any() != true ) {

                    //text.WriteLine();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    Console.WriteLine( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {

                    //String.Format( text, parms ).WriteLine();
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