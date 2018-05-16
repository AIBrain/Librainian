// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Base58String.cs",
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
// "Librainian/Librainian/Base58String.cs" was last cleaned by Protiguous on 2018/05/15 at 10:49 PM.

namespace Librainian.Parsing {

    using System;
    using System.Linq;
    using System.Numerics;
    using System.Text;

    public static class Base58String {

        public const String Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static String FromByteArray( this Byte[] b ) {
            var sb = new StringBuilder();
            var bi = new BigInteger( b.Reverse().Concat( new Byte[] { 0x00 } ).ToArray() ); // concat adds sign byte

            // Calc base58 representation
            while ( bi > 0 ) {
                var mod = ( Int32 )( bi % 58 );
                bi /= 58;
                sb.Insert( 0, Base58Chars[mod] );
            }

            // Add 1s for leading 0x00 bytes
            for ( var i = 0; i < b.Length && b[i] == 0x00; i++ ) { sb.Insert( 0, '1' ); }

            return sb.ToString();
        }

        public static Byte[] ToByteArray( this String s ) {
            BigInteger bi = 0;

            // Decode base58
            foreach ( var charVal in s.Select( c => Base58Chars.IndexOf( c ) ).Where( charVal => charVal != -1 ) ) {
                bi *= 58;
                bi += charVal;
            }

            var b = bi.ToByteArray();

            // Remove 0x00 sign byte if present.
            if ( b[b.Length - 1] == 0x00 ) { b = b.Take( b.Length - 1 ).ToArray(); }

            // Add leading 0x00 bytes
            var num0S = s.IndexOf( s.First( c => c != '1' ) );

            return b.Concat( new Byte[num0S] ).Reverse().ToArray();
        }
    }
}