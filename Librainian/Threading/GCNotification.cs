// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "GCNotification.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "GCNotification.cs" was last formatted by Protiguous on 2020/01/31 at 12:31 AM.

namespace Librainian.Threading {

    using System;
    using System.Threading;

    /// <summary>From http://www.wintellect.com/cs/blogs/jeffreyr/default.aspx</summary>
    public static class GcNotification {

        private static Action<Int32> _sGcDone;

        public static event Action<Int32> GcDone {
            add {

                // If there were no registered delegates before, start reporting notifications now
                if ( _sGcDone is null ) {

                    // ReSharper disable once ObjectCreationAsStatement
                    new GenObject( 0 );

                    // ReSharper disable once ObjectCreationAsStatement
                    new GenObject( 2 );
                }

                _sGcDone += value;
            }
            remove => _sGcDone -= value;
        }

        private sealed class GenObject {

            private readonly Int32 _mGeneration;

            public GenObject( Int32 generation ) => this._mGeneration = generation;

            ~GenObject() {

                // This is the Finalize method If this object is in the generation we want (or
                // higher), notify the delegates that a GC just completed
                if ( GC.GetGeneration( this ) >= this._mGeneration ) {
                    var temp = Interlocked.CompareExchange( ref _sGcDone, null, null );
                    temp?.Invoke( this._mGeneration );
                }

                // Keep reporting notifications if there is at least one delegate registered, the
                // AppDomain isn't unloading, and the process isn’t shutting down
                if ( _sGcDone is null || AppDomain.CurrentDomain.IsFinalizingForUnload() || Environment.HasShutdownStarted ) {
                    return;
                }

                // For Gen 0, create a new object; for Gen 2, resurrect the object & let the GC call
                // Finalize again the next time Gen 2 is GC'd
                if ( this._mGeneration == 0 ) {

                    // ReSharper disable once ObjectCreationAsStatement
                    new GenObject( 0 );
                }
                else {
                    GC.ReRegisterForFinalize( this );
                }
            }
        }

        // The event’s field
    }
}