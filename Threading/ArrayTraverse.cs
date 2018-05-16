// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ArrayTraverse.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/ArrayTraverse.cs" was last cleaned by Protiguous on 2018/05/15 at 10:50 PM.

namespace Librainian.Threading {

    using System;

    internal class ArrayTraverse {

        private readonly Int32[] _maxLengths;

        public readonly Int32[] Position;

        public ArrayTraverse( Array array ) {
            this._maxLengths = new Int32[array.Rank];

            for ( var i = 0; i < array.Rank; ++i ) { this._maxLengths[i] = array.GetLength( i ) - 1; }

            this.Position = new Int32[array.Rank];
        }

        public Boolean Step() {
            for ( var i = 0; i < this.Position.Length; ++i ) {
                if ( this.Position[i] >= this._maxLengths[i] ) { continue; }

                this.Position[i]++;

                for ( var j = 0; j < i; j++ ) { this.Position[j] = 0; }

                return true;
            }

            return false;
        }
    }
}