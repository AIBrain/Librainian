// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/GCNotification.cs" was last cleaned by Rick on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;
    using System.Threading;

    /// <summary>From http://www.wintellect.com/cs/blogs/jeffreyr/default.aspx</summary>
    public static class GcNotification {
        private static Action<Int32> _sGcDone;

        public static event Action<Int32> GcDone {
            add {

                // If there were no registered delegates before, start reporting notifications now
                if ( _sGcDone == null ) {

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
                    var temp = Interlocked.CompareExchange( location1: ref _sGcDone, value: null, comparand: null );
                    temp?.Invoke( this._mGeneration );
                }

                // Keep reporting notifications if there is at least one delegate registered, the
                // AppDomain isn't unloading, and the process isn’t shutting down
                if ( ( _sGcDone == null ) || AppDomain.CurrentDomain.IsFinalizingForUnload() || Environment.HasShutdownStarted ) {
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