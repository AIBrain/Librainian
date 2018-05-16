// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Hash.cs",
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
// "Librainian/Librainian/Hash.cs" was last cleaned by Protiguous on 2018/05/15 at 10:46 PM.

namespace Librainian.Measurement.Currency.BTC {

    using System;
    using System.Linq;

    /// <summary></summary>
    /// <see cref="http://github.com/mb300sd/Bitcoin-Tool" />
    public class Hash {

        public readonly Byte[] HashBytes;

        public Hash( Byte[] b ) => this.HashBytes = b;

        public Byte this[Int32 i] {
            get => this.HashBytes[i];

            set => this.HashBytes[i] = value;
        }

        public static implicit operator Byte[] ( Hash hash ) => hash.HashBytes;

        public static implicit operator Hash( Byte[] bytes ) => new Hash( bytes );

        public override Boolean Equals( Object obj ) => obj is Hash hash1 && this.HashBytes.SequenceEqual( hash1.HashBytes );

        public override Int32 GetHashCode() {
            if ( this.HashBytes.Length >= 4 ) { return ( this.HashBytes[0] << 24 ) | ( this.HashBytes[1] << 16 ) | ( this.HashBytes[2] << 8 ) | ( this.HashBytes[3] << 0 ); }

            return this.HashBytes.GetHashCode();
        }
    }
}