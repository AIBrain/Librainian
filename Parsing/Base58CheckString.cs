// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Base58CheckString.cs",
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
// "Librainian/Librainian/Base58CheckString.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Parsing {

    using System;
    using System.Linq;
    using System.Security.Cryptography;

    public static class Base58CheckString {

        public static String FromByteArray( Byte[] b, Byte version ) {
            using ( SHA256 sha256 = new SHA256Managed() ) {
                b = new[] { version }.Concat( b ).ToArray();
                var hash = sha256.ComputeHash( sha256.ComputeHash( b ) ).Take( 4 ).ToArray();

                return b.Concat( hash ).ToArray().FromByteArray();
            }
        }

        public static Byte[] ToByteArray( String s, out Byte version ) {
            var b = s.ToByteArray();

            using ( SHA256 sha256 = new SHA256Managed() ) {
                var hash = sha256.ComputeHash( sha256.ComputeHash( b.Take( b.Length - 4 ).ToArray() ) );

                if ( !hash.Take( 4 ).SequenceEqual( b.Skip( b.Length - 4 ).Take( 4 ) ) ) { throw new ArgumentException( "Invalid Base58Check String" ); }

                version = b.First();

                return b.Skip( 1 ).Take( b.Length - 5 ).ToArray();
            }
        }

        public static Byte[] ToByteArray( String s ) => ToByteArray( s, out var b );
    }
}