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
// "Librainian/Gate.cs" was last cleaned by Rick on 2014/08/11 at 12:41 AM
#endregion

namespace Librainian.Threading {
    using System;
    using System.Threading;

    /// <summary>
    ///     A simple atomic gate
    /// </summary>
    // TODO 2013, unfinished
    // TODO 2014, what was this class for? lol.
    public sealed class Gate {
        private int _value;

        /// <summary>
        ///     Initializes a new instance of the Gate class.
        /// </summary>
        /// <param name="openOrClosed">Defaults to <see cref="OpenOrClosed.Closed" />.</param>
        public Gate( OpenOrClosed openOrClosed = OpenOrClosed.Closed ) {
            this._value = ( int ) openOrClosed;
        }

        /// <summary>
        ///     Initializes a new instance of the Gate class in the closed state.
        /// </summary>
        public Gate() {
            //
        }

        /// <summary>
        ///     Returns true if the gate is closed
        /// </summary>
        public Boolean IsClosed { get { return 0 == Interlocked.Add( ref this._value, 0 ); } }

        /// <summary>
        ///     Returns true if the gate is open
        /// </summary>
        public Boolean IsOpened { get { return OpenOrClosed.Opened == ( OpenOrClosed ) Interlocked.Add( ref this._value, 0 ); } }

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
        public Boolean TryClose() {
            return 1 == Interlocked.CompareExchange( ref this._value, 0, 1 );
        }

        /// <summary>
        ///     Attempts to open the gate
        /// </summary>
        /// <returns>true if the operation was successful</returns>
        public Boolean TryOpen() {
            return 0 == Interlocked.CompareExchange( ref this._value, 1, 0 );
        }
    }
}
