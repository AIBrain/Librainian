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
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ArrayTraverse.cs" was last cleaned by Protiguous on 2016/06/18 at 10:57 PM

namespace Librainian.Threading {

    using System;

    internal class ArrayTraverse {
        public readonly Int32[] Position;
        private readonly Int32[] _maxLengths;

        public ArrayTraverse( Array array ) {
            this._maxLengths = new Int32[ array.Rank ];
            for ( var i = 0; i < array.Rank; ++i ) {
                this._maxLengths[ i ] = array.GetLength( i ) - 1;
            }
            this.Position = new Int32[ array.Rank ];
        }

        public Boolean Step() {
            for ( var i = 0; i < this.Position.Length; ++i ) {
                if ( this.Position[ i ] >= this._maxLengths[ i ] ) {
                    continue;
                }
                this.Position[ i ]++;
                for ( var j = 0; j < i; j++ ) {
                    this.Position[ j ] = 0;
                }
                return true;
            }
            return false;
        }
    }
}