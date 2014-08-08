#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Utility.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Measurement.Time;
    using Threading;

    public static class Utility {
        public static readonly DummyXMLResolver DummyXMLResolver = new DummyXMLResolver();

        private static readonly ReaderWriterLockSlim ConsoleOutputSynch = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

        static Utility() {
            Contract.ContractFailed += ( sender, e ) => {
                                           if ( Debugger.IsAttached ) {
                                               Debugger.Break();
                                           }
                                           var message = String.Format( "Caught Uncaught Contract Failure\r\n{0}\r\n{1}\r\n{2}\r\n{3}", e.Condition, e.FailureKind, e.Handled, e.Message );
                                           e.OriginalException.Log( message: message );
                                       };
        }

        /// <summary>
        ///     Example: WriteTextAsync( fullPath: fullPath, text: message ).Wait();
        ///     Example: await WriteTextAsync( fullPath: fullPath, text: message );
        /// </summary>
        /// <param name="fileInfo"> </param>
        /// <param name="text"> </param>
        /// <returns> </returns>
        public static async void AppendTextAsync( this FileInfo fileInfo, String text ) {
            if ( fileInfo == null ) {
                throw new ArgumentNullException( "fileInfo" );
            }
            if ( String.IsNullOrWhiteSpace( fileInfo.FullName ) || String.IsNullOrWhiteSpace( text ) ) {
                return;
            }
            try {
                //using ( var str = new StreamWriter( fileInfo.FullName, true, Encoding.Unicode ) ) { return str.WriteLineAsync( text ); }
                var encodedText = Encoding.Unicode.GetBytes( text );
                var length = encodedText.Length;

                //hack
                //using ( var bob = File.Create( fileInfo.FullName, length, FileOptions.Asynchronous | FileOptions.RandomAccess | FileOptions.WriteThrough  ) ) {
                //    bob.WriteAsync
                //}

                using ( var sourceStream = new FileStream( path: fileInfo.FullName, mode: FileMode.Append, access: FileAccess.Write, share: FileShare.Write, bufferSize: length, useAsync: true ) ) {
                    await sourceStream.WriteAsync( buffer: encodedText, offset: 0, count: length );
                    await sourceStream.FlushAsync();
                }
            }
            catch ( UnauthorizedAccessException exception ) {
                exception.Log();
            }
            catch ( ArgumentNullException exception ) {
                exception.Log();
            }
            catch ( DirectoryNotFoundException exception ) {
                exception.Log();
            }
            catch ( PathTooLongException exception ) {
                exception.Log();
            }
            catch ( SecurityException exception ) {
                exception.Log();
            }
            catch ( IOException exception ) {
                exception.Log();
            }
        }

        /// <summary>
        ///     Output the <paramref name="text" /> at the end of the current <seealso cref="Console" /> line.
        /// </summary>
        /// <param name="text"> </param>
        /// <param name="yOffset"> </param>
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

        public static void DebugAssert( this Boolean condition ) {
            if ( condition ) {
                return;
            }
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        //    Console.SetCursorPosition( left: Console.WindowWidth - ( text.Length + 1 ), top: 0 );
        //    Console.Write( text );
        //    Console.SetCursorPosition( left: oldLeft, top: oldTop );
        //}
        public static void DebugAssert( this Boolean condition, String message ) {
            if ( condition ) {
                return;
            }
            Debug.WriteLine( message );
            if ( Debugger.IsAttached ) {
                Debugger.Break();
            }
        }

        public static void OnSet< T >( this EventHandler< T > @event, object sender, T e ) where T : EventArgs {
            throw new NotImplementedException();
            //if ( @event != null ) { @event( sender, e ); }
        }

        //    return false;
        //}
        /// <summary>
        ///     untested. is this written correctly? would it read from a *slow* media but not block the calling function?
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bufferSize"></param>
        /// <param name="fileMissingRetries"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        public static async Task< string > ReadTextAsync( String filePath, int? bufferSize = 4096, int? fileMissingRetries = 10, TimeSpan? retryDelay = null ) {
            if ( String.IsNullOrWhiteSpace( filePath ) ) {
                throw new ArgumentNullException( "filePath" );
            }

            if ( !bufferSize.HasValue ) {
                bufferSize = 4096;
            }
            if ( !retryDelay.HasValue ) {
                retryDelay = Seconds.One;
            }

            while ( fileMissingRetries.HasValue && fileMissingRetries.Value > 0 ) {
                if ( File.Exists( filePath ) ) {
                    break;
                }
                await Task.Delay( retryDelay.Value );
                fileMissingRetries--;
            }

            if ( File.Exists( filePath ) ) {
                try {
                    using ( var sourceStream = new FileStream( path: filePath, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read, bufferSize: bufferSize.Value, useAsync: true ) ) {
                        var sb = new StringBuilder( bufferSize.Value );
                        var buffer = new byte[bufferSize.Value];
                        int numRead;
                        while ( ( numRead = await sourceStream.ReadAsync( buffer, 0, buffer.Length ) ) != 0 ) {
                            var text = Encoding.Unicode.GetString( buffer, 0, numRead );
                            sb.Append( text );
                        }

                        return sb.ToString();
                    }
                }
                catch ( FileNotFoundException exception ) {
                    exception.Log();
                }
            }
            return String.Empty;
        }

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
            lock ( ConsoleOutputSynch ) {
                if ( null == parms || !parms.Any() ) {
                    text.TimeDebug();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor; //TODO d.r.y.
                    Console.BackgroundColor = backColor; //TODO d.r.y.
                    Console.Write( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {
                    String.Format( text, parms ).TimeDebug();
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
            lock ( ConsoleOutputSynch ) {
                if ( Equals( parms, null ) || !parms.Any() ) {
                    text.TimeDebug();
                    var oldFore = Console.ForegroundColor;
                    var oldBack = Console.BackgroundColor;
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    Console.WriteLine( text );
                    Console.BackgroundColor = oldBack;
                    Console.ForegroundColor = oldFore;
                }
                else {
                    String.Format( text, parms ).TimeDebug();
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

        //    var memorySize = ( int )Math.Ceiling( bytes / 1048576M );
        //    if ( memorySize < 1 ) { memorySize = 1; }
        //    try {
        //        using ( new MemoryFailPoint( memorySize ) ) {
        //            return true;
        //        }
        //    }
        //    catch ( AccessViolationException exception ) {
        //        exception.Log();
        //    }
        //    catch ( InsufficientMemoryException exception ) {
        //        if ( Debugger.IsAttached ) { Debugger.Break(); }
        //        GC.Collect();
        //        var result = GC.WaitForFullGCComplete( millisecondsTimeout: -1 );
        //        if ( result == GCNotificationStatus.Succeeded || result == GCNotificationStatus.NotApplicable ) {
        //            var amount = GC.GetTotalMemory( forceFullCollection: true );
        //            try {
        //                using ( new MemoryFailPoint( memorySize ) ) {
        //                    return true;
        //                }
        //            }
        //            catch ( InsufficientMemoryException exception2 ) {
        //                exception2.Log();
        //            }
        //        }
        //        exception.Log();
        //    }
        //    catch ( ArgumentOutOfRangeException exception ) {
        //        exception.Log();
        //    }
        //    catch ( InsufficientExecutionStackException exception ) {
        //        exception.Log();
        //    }

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

        public static void Swap< T >( ref T arg1, ref T arg2 ) {
            var temp = arg1;
            arg1 = arg2;
            arg2 = temp;
        }

        public class HTML {
            public const String EmptyHTML5 = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"utf-8\" /><title id=\"title1\"></title></head><body><header id=\"header1\"></header><article id=\"article1\"></article><footer id=\"footer1\"></footer></body></html>";

            /// <summary>
            ///     an empty document.
            /// </summary>
            public static String EmptyHTMLDocument { get { return String.Format( "<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN'>{0}<html>{1}<head>{2}<title>AIBrain</title>{3}<style>{4}body {{ background-color: gainsboro; font-family: Arial; font-size: 10pt; }}{5}div {{ margin-bottom: 3pt; }}{6}div.Critical {{ color: crimson; font-weight: bolder; }}{7}div.Error {{ color: firebrick; }}{8}div.Warning {{ color: purple; }}{9}div.Information {{ color: green; }}{10}div.Write {{ color: green; }}{11}div.WriteLine {{ color: green; }}{12}div.Verbose {{ color: dimgray; }}{13}div span {{ margin-right: 2px; vertical-align: top; }}{14}div span.Dingbat {{ display: none; }}{15}div span.DateTime {{ display: inline; Single : left; width: 3em; height: auto }}{16}div span.Source {{ display: none; Single : left; width: 8em; height: auto; }}{17}div span.ThreadId {{ display: inline; Single: left; width: 2em; height: auto; text-align: right; }}{18}div span.MessageType {{ display: none; Single: left; width: 6em; height: auto; text-align: left; }}{19}div span.MessageText {{ display: inline; width: 100%; position:relative; }}{20}div.Critical span.MessageText {{ font-weight: bold; }}{21}div span.CallStack {{ display: none; margin-left: 1em; }}{22}</style>{23}</head>{24}<body>{25}</body>{26}</html>{27}", Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine ); } }
        }
    }
}
