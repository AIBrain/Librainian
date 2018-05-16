// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "Gate.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has been
// overwritten by the automatic formatting. (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors. =========================================================
//
// Donations (more please!), royalties from any software that uses any of our code, and license fees can be paid to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// ========================================================= Usage of the source code or compiled binaries is AS-IS. No warranties are expressed or implied. I am NOT responsible for Anything You Do With Our Code. =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Gate.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

namespace Librainian.Threading {

    using System;
    using System.Threading;

    /// <summary>
    /// A simple atomic gate
    /// </summary>
    // TODO 2013, unfinished TODO 2014, what was this class for? lol. TODO 2015, still no idea.
    public sealed class Gate {

        private Int32 _value;

        /// <summary>
        /// Initializes a new instance of the Gate class.
        /// </summary>
        /// <param name="openOrClosed">Defaults to <see cref="OpenOrClosed.Closed"/>.</param>
        public Gate( OpenOrClosed openOrClosed = OpenOrClosed.Closed ) => this._value = ( Int32 )openOrClosed;

        /// <summary>
        /// Initializes a new instance of the Gate class in the closed state.
        /// </summary>
        public Gate() {

            //
        }

        /// <summary>
        /// Returns true if the gate is closed
        /// </summary>
        public Boolean IsClosed => 0 == Interlocked.Add( ref this._value, 0 );

        /// <summary>
        /// Returns true if the gate is open
        /// </summary>
        public Boolean IsOpened => OpenOrClosed.Opened == ( OpenOrClosed )Interlocked.Add( ref this._value, 0 );

        /// <summary>
        /// Closes the gate. The gate must be in the open state.
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown if the gate is already closed</exception>
        public void Close() {
            if ( !this.TryClose() ) { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Opens the gate. The gate must be in the closed state.
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown if the gate is already open</exception>
        public void Open() {
            if ( !this.TryOpen() ) { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Attempts to close the gate
        /// </summary>
        /// <returns>true if the operation was successful</returns>
        public Boolean TryClose() => 1 == Interlocked.CompareExchange( ref this._value, 0, 1 );

        /// <summary>
        /// Attempts to open the gate
        /// </summary>
        /// <returns>true if the operation was successful</returns>
        public Boolean TryOpen() => 0 == Interlocked.CompareExchange( ref this._value, 1, 0 );
    }
}