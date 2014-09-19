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
// "Librainian/Base58CheckString.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

namespace Librainian.Measurement.Currency.BTC {
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using Parsing;

    public static class Base58CheckString {
        public static String FromByteArray( Byte[] b, Byte version ) {
            SHA256 sha256 = new SHA256Managed();
            b = ( new[] { version } ).Concat( b ).ToArray();
            var hash = sha256.ComputeHash( sha256.ComputeHash( b ) ).Take( 4 ).ToArray();
            return Base58String.FromByteArray( b.Concat( hash ).ToArray() );
        }

        public static Byte[] ToByteArray( String s, out Byte version ) {
            SHA256 sha256 = new SHA256Managed();
            var b = Base58String.ToByteArray( s );
            var hash = sha256.ComputeHash( sha256.ComputeHash( b.Take( b.Length - 4 ).ToArray() ) );
            if ( !hash.Take( 4 ).SequenceEqual( b.Skip( b.Length - 4 ).Take( 4 ) ) ) {
                throw new ArgumentException( "Invalid Base58Check String" );
            }
            version = b.First();
            return b.Skip( 1 ).Take( b.Length - 5 ).ToArray();
        }

        public static Byte[] ToByteArray( String s ) {
            Byte b;
            return ToByteArray( s, out b );
        }
    }
}
