// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved. This ENTIRE copyright notice and file header MUST BE KEPT VISIBLE in any source code derived from or used from our libraries and projects.
//
// ========================================================= This section of source code, "Releaser.cs", belongs to Rick@AIBrain.org and Protiguous@Protiguous.com unless otherwise specified OR the original license has
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
// "Librainian/Releaser.cs" was last cleaned by Protiguous on 2018/05/15 at 4:23 AM.

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
            if ( this._toRelease is null ) { return; }

            if ( this._writer ) { this._toRelease.WriterRelease(); }
            else { this._toRelease.ReaderRelease(); }
        }
    }
}