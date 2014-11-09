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
// "Librainian/Base58String.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Parsing {
    using System;
    using System.Linq;
    using System.Numerics;
    using System.Text;

    public static class Base58String {
        private const String Base58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public static Byte[] ToByteArray( String s ) {
            BigInteger bi = 0;
            // Decode base58
            foreach ( var charVal in s.Select( c => Base58Chars.IndexOf( c ) ).Where( charVal => charVal != -1 ) ) {
                bi *= 58;
                bi += charVal;
            }
            var b = bi.ToByteArray();
            // Remove 0x00 sign byte if present.
            if ( b[ b.Length - 1 ] == 0x00 ) {
                b = b.Take( b.Length - 1 ).ToArray();
            }
            // Add leading 0x00 bytes
            var num0S = s.IndexOf( s.First( c => c != '1' ) );
            return b.Concat( new Byte[num0S] ).Reverse().ToArray();
        }

        public static String FromByteArray( Byte[] b ) {
            var sb = new StringBuilder();
            var bi = new BigInteger( b.Reverse().Concat( new Byte[] { 0x00 } ).ToArray() ); // concat adds sign byte
            // Calc base58 representation
            while ( bi > 0 ) {
                var mod = ( int ) ( bi%58 );
                bi /= 58;
                sb.Insert( 0, Base58Chars[ mod ] );
            }
            // Add 1s for leading 0x00 bytes
            for ( var i = 0; i < b.Length && b[ i ] == 0x00; i++ ) {
                sb.Insert( 0, '1' );
            }
            return sb.ToString();
        }
    }
}
