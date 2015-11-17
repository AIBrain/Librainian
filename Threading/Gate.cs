
// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Gate.cs" was last cleaned by Rick on 2015/06/12 at 3:14 PM

namespace Librainian.Threading {
    using System;
    using System.Threading;

    /// <summary>
    ///     A simple atomic gate
    /// </summary>
    // TODO 2013, unfinished
    // TODO 2014, what was this class for? lol.
    // TODO 2015, still no idea.
    public sealed class Gate {
        private Int32 _value;

        /// <summary>
        ///     Returns true if the gate is closed
        /// </summary>
        public Boolean IsClosed => 0 == Interlocked.Add( ref this._value, 0 );

        /// <summary>
        ///     Returns true if the gate is open
        /// </summary>
        public Boolean IsOpened => OpenOrClosed.Opened == ( OpenOrClosed )Interlocked.Add( ref this._value, 0 );

        /// <summary>
        ///     Initializes a new instance of the Gate class.
        /// </summary>
        /// <param name="openOrClosed">Defaults to <see cref="OpenOrClosed.Closed" />.</param>
        public Gate( OpenOrClosed openOrClosed = OpenOrClosed.Closed ) {
            this._value = ( Int32 ) openOrClosed;
        }

        /// <summary>
        ///     Initializes a new instance of the Gate class in the closed state.
        /// </summary>
        public Gate() {
            //
        }
        /// <summary>
        ///     Closes the gate. The gate must be in the open state.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the gate is already closed
        /// </exception>
        public void Close() {
            if ( !this.TryClose() ) {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     Opens the gate. The gate must be in the closed state.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the gate is already open
        /// </exception>
        public void Open() {
            if ( !this.TryOpen() ) {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     Attempts to close the gate
        /// </summary>
        /// <returns>true if the operation was successful</returns>
        public Boolean TryClose() => 1 == Interlocked.CompareExchange( ref this._value, 0, 1 );

        /// <summary>
        ///     Attempts to open the gate
        /// </summary>
        /// <returns>true if the operation was successful</returns>
        public Boolean TryOpen() => 0 == Interlocked.CompareExchange( ref this._value, 1, 0 );
    }
}
