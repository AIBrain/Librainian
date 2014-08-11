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
// "Librainian/GCNotification.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;

    /// <summary>
    ///     From http://www.wintellect.com/cs/blogs/jeffreyr/default.aspx
    /// </summary>
    public static class GCNotification {
        private static Action< int > _sGCDone; // The event’s field

        public static event Action< Int32 > GCDone {
            add {
                // If there were no registered delegates before, start reporting notifications now
                if ( _sGCDone == null ) {
                    new GenObject( 0 );
                    new GenObject( 2 );
                }
                _sGCDone += value;
            }
            remove { _sGCDone -= value; }
        }

        private sealed class GenObject {
            private readonly Int32 _mGeneration;

            public GenObject( Int32 generation ) {
                this._mGeneration = generation;
            }

            ~GenObject() {
                // This is the Finalize method
                // If this object is in the generation we want (or higher), notify the delegates that a GC just completed
                if ( GC.GetGeneration( this ) >= this._mGeneration ) {
                    var temp = Interlocked.CompareExchange( ref _sGCDone, null, null );
                    if ( temp != null ) {
                        temp( this._mGeneration );
                    }
                }
                // Keep reporting notifications if there is at least one delegate registered, the AppDomain isn't unloading, and the process isn’t shutting down
                if ( ( _sGCDone == null ) || AppDomain.CurrentDomain.IsFinalizingForUnload() || Environment.HasShutdownStarted ) {
                    return;
                }

                // For Gen 0, create a new object; for Gen 2, resurrect the object & let the GC call Finalize again the next time Gen 2 is GC'd
                if ( this._mGeneration == 0 ) {
                    new GenObject( 0 );
                }
                else {
                    GC.ReRegisterForFinalize( this );
                }
            }
        }

        /*
         And here is some code to see it in action:

            public static void Main() {
               GCNotification.GCDone += g => Console.Beep(g == 0 ? 800 : 8000, 200);
               var l = new List<Object>();
               // Construct a lot of 100-byte objects.
               for (Int32 x = 0; x < 1000000; x++) {
                  Console.WriteLine(x);
                  Byte[] b = new Byte[100];
                  l.Add(b);
               }
            }

         */
    }
}
