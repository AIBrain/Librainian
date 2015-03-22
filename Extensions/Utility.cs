#region License & Information

// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// "Librainian/Utility.cs" was last cleaned by Rick on 2014/08/11 at 12:37 AM
#endregion License & Information

namespace Librainian.Extensions {
    using System;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using Threading;

    public static class Utility {
        public static readonly DummyXMLResolver DummyXMLResolver = new DummyXMLResolver();

        private static readonly ReaderWriterLockSlim ConsoleOutputSynch = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        /// <summary>
        /// Output the <paramref name="text"/> at the end of the current <seealso cref="Console"/> line.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="yOffset"></param>
        public static void AtEndOfLine( this String text, int yOffset = 0 ) {
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
                    exception.More();
                }
                catch ( IOException exception ) {
                    exception.More();
                }
                catch ( SecurityException exception ) {
                    exception.More();
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

        public static void OnSet<T>( this EventHandler<T> @event, object sender, T e ) where T : EventArgs {
            throw new NotImplementedException();

            //if ( @event != null ) { @event( sender, e ); }
        }

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

        public static void Write( this String text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params object[] parms ) {
            lock (ConsoleOutputSynch) {
                if ( null == parms || !parms.Any() ) {
                    Log.WriteLine( text );
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor; //TODO d.r.y.
                    Console.BackgroundColor = backColor; //TODO d.r.y.
                    Console.Write( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {
                    Log.WriteLine( String.Format( text, parms ) );
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

        public static void WriteLine( this String text, ConsoleColor foreColor = ConsoleColor.White, ConsoleColor backColor = ConsoleColor.Black, params object[] parms ) {
            lock (ConsoleOutputSynch) {
                if ( Equals( parms, null ) || !parms.Any() ) {
                    Log.WriteLine( text );
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    Console.WriteLine( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {
                    Log.WriteLine( String.Format( text, parms ) );
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

        //public static Boolean isMemoryOkay( int bytes ) {
        //    return true;

        // var memorySize = ( int )Math.Ceiling( bytes / 1048576M ); if ( memorySize < 1 ) {
        // memorySize = 1; } try { using ( new MemoryFailPoint( memorySize ) ) { return true; } }
        // catch ( AccessViolationException exception ) { exception.Error(); } catch (
        // InsufficientMemoryException exception ) { if ( Debugger.IsAttached ) { Debugger.Break();
        // } GC.Collect(); var result = GC.WaitForFullGCComplete( millisecondsTimeout: -1 ); if (
        // result == GCNotificationStatus.Succeeded || result == GCNotificationStatus.NotApplicable
        // ) { var amount = GC.GetTotalMemory( forceFullCollection: true ); try { using ( new
        // MemoryFailPoint( memorySize ) ) { return true; } } catch ( InsufficientMemoryException
        // exception2 ) { exception2.Log(); } } exception.Error(); } catch (
        // ArgumentOutOfRangeException exception ) { exception.Error(); } catch (
        // InsufficientExecutionStackException exception ) { exception.Error(); }

        //public static rrrr ssdgsdfgs< ttt, rrr > = Func<rrr> ();

        ///// <summary>
        ///// <para></para>
        ///// </summary>
        //public static IEnumerable<tttt> Infinitely<tttt,rrrr>( this Func<tttt,rrrrr> value ) {
        //    do {
        //        yield return value( );
        //    } while ( true );
        //    // ReSharper disable once FunctionNeverReturns
        //}
    }
}