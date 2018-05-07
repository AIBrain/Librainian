// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Releaser.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;

    public struct Releaser : IDisposable {
        private readonly AsyncReaderWriterLock _toRelease;
        private readonly Boolean _writer;

        internal Releaser( AsyncReaderWriterLock toRelease, Boolean writer ) {
            this._toRelease = toRelease;
            this._writer = writer;
        }

        public void Dispose() {
            if ( this._toRelease is null ) {
                return;
            }
            if ( this._writer ) {
                this._toRelease.WriterRelease();
            }
            else {
                this._toRelease.ReaderRelease();
            }
        }
    }
}