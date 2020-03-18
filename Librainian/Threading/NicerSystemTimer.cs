// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "NicerSystemTimer.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "NicerSystemTimer.cs" was last formatted by Protiguous on 2020/03/16 at 3:02 PM.

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Timer = System.Timers.Timer;

    /// <summary>Updated the code.</summary>
    public class NicerSystemTimer : IDisposable {

        [CanBeNull]
        private ReaderWriterLockSlim access { get; }

        [CanBeNull]
        private Timer Timer { get; }

        /// <summary>Perform an <paramref name="action" /> after the given interval (in <paramref name="milliseconds" />).</summary>
        /// <param name="action"></param>
        /// <param name="repeat">Perform the <paramref name="action" /> again. (Restarts the <see cref="Timer" />.)</param>
        /// <param name="milliseconds"></param>
        public NicerSystemTimer( [CanBeNull] Action action, Boolean repeat, Double? milliseconds = null ) {
            if ( action is null ) {
                return;
            }

            this.access = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

            this.Timer = new Timer {
                AutoReset = false,
                Interval = milliseconds.GetValueOrDefault( 1 )
            };

            this.Timer.Elapsed += ( sender, args ) => {

                try {
                    if ( this.access.TryEnterReadLock( 0 ) ) {
                        this.Timer.Stop();
                        action.Invoke();
                    }

                    if ( repeat ) {
                        this.Timer.Start();
                    }
                }
                catch ( Exception exception ) {
                    Debug.WriteLine( exception );
                }
            };

            this.Timer.Start();
        }

        public void Dispose() {
            using ( this.Timer ) { }
        }
    }
}