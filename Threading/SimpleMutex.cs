// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "SimpleMutex.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has
// been overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/SimpleMutex.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

namespace Librainian.Threading {

    using System;
    using System.Diagnostics;
    using System.Threading;
    using JetBrains.Annotations;
    using Magic;

    /// <summary>
    /// A simple, cross application mutex. Use <see cref="Acquire"/> to acquire it
    /// </summary>
    /// <remarks>Only one thread (and thus process) can have the mutex acquired at the same time.</remarks>
    public sealed class SimpleMutex : ABetterClassDispose {

        private readonly Mutex _mutex;

        private SimpleMutex( String mutexName ) => this._mutex = new Mutex( false, mutexName );

        /// <summary>
        /// Acquires the mutex with the specified name.
        /// </summary>
        /// <param name="mutexName">the mutex's name</param>
        /// <param name="timeout">  how long to try to acquire the mutex</param>
        /// <returns>Returns the mutex or <c>null</c>, if the mutex couldn't be acquired in time (i.e. the current mutex holder didn't release it in time).</returns>
        [CanBeNull]
        public static SimpleMutex Acquire( String mutexName, TimeSpan timeout ) {
            var mutex = new SimpleMutex( mutexName );

            try {
                if ( !mutex._mutex.WaitOne( timeout ) ) {

                    // We could not acquire the mutex in time.
                    mutex._mutex.Dispose();

                    return null;
                }
            }
            catch ( AbandonedMutexException ex ) {

                // We now own this mutex. The previous owner didn't release it properly, though.
                Debug.WriteLine( ex );
            }

            return mutex;
        }

        public override void DisposeManaged() {
            using ( this._mutex ) { this._mutex.ReleaseMutex(); }
        }
    }
}