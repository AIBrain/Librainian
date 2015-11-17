// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/ArrayTraverse.cs" was last cleaned by Rick on 2015/08/25 at 3:01 PM

namespace Librainian.Threading {

    using System;

    internal class ArrayTraverse {

        private readonly Int32[] _maxLengths;

        public readonly Int32[] Position;

        public ArrayTraverse( Array array ) {
            this._maxLengths = new Int32[array.Rank];
            for ( var i = 0; i < array.Rank; ++i ) {
                this._maxLengths[ i ] = array.GetLength( i ) - 1;
            }
            this.Position = new Int32[array.Rank];
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
