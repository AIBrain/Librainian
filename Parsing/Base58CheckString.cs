// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Base58CheckString.cs" was last cleaned by Rick on 2016/06/18 at 10:55 PM

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
                if ( !hash.Take( 4 ).SequenceEqual( b.Skip( b.Length - 4 ).Take( 4 ) ) ) {
                    throw new ArgumentException( "Invalid Base58Check String" );
                }
                version = b.First();
                return b.Skip( 1 ).Take( b.Length - 5 ).ToArray();
            }
        }

        public static Byte[] ToByteArray( String s ) => ToByteArray( s, out var b );

    }
}