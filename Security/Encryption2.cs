// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Encryption2.cs",
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
// "Librainian/Librainian/Encryption2.cs" was last formatted by Protiguous on 2018/05/17 at 3:43 PM.

namespace Librainian.Security {

    using System;
    using System.Collections;
    using System.Security.Cryptography;
    using System.Text;
    using JetBrains.Annotations;

    public static class Encryption2 {

        public static String Decrypt( [NotNull] this String inputString, Int32 keySize, [NotNull] String xmlString ) {

            // TODO: Add Proper Exception Handlers
            if ( inputString is null ) { throw new ArgumentNullException( nameof( inputString ) ); }

            if ( xmlString is null ) { throw new ArgumentNullException( nameof( xmlString ) ); }

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize: keySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString: xmlString );
            var base64BlockSize = keySize / 8 % 3 != 0 ? keySize / 8 / 3 * 4 + 4 : keySize / 8 / 3 * 4;
            var iterations = inputString.Length / base64BlockSize;
            var arrayList = new ArrayList();

            for ( var i = 0; i < iterations; i++ ) {
                var encryptedBytes = Convert.FromBase64String( s: inputString.Substring( startIndex: base64BlockSize * i, base64BlockSize ) );

                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after
                // encryption and before decryption. If you do not require compatibility with
                // Microsoft Cryptographic API (CAPI) and/or other vendors. Comment out the next
                // line and the corresponding one in the EncryptString function.
                Array.Reverse( array: encryptedBytes );
                arrayList.AddRange( c: rsaCryptoServiceProvider.Decrypt( rgb: encryptedBytes, fOAEP: true ) );
            }

            return !( arrayList.ToArray( type: typeof( Byte ) ) is Byte[] ba ) ? String.Empty : Encoding.Unicode.GetString( bytes: ba );
        }

        public static String Encrypt( [NotNull] this String inputString, Int32 dwKeySize, [NotNull] String xmlString ) {

            // TODO: Add Proper Exception Handlers
            if ( inputString is null ) { throw new ArgumentNullException( nameof( inputString ) ); }

            if ( xmlString is null ) { throw new ArgumentNullException( nameof( xmlString ) ); }

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize: dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString: xmlString );
            var keySize = dwKeySize / 8;
            var bytes = Encoding.Unicode.GetBytes( s: inputString );

            // The hash function in use by the .NET RSACryptoServiceProvider here is SHA1 int
            // maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            var maxLength = keySize - 42;
            var dataLength = bytes.Length;
            var iterations = dataLength / maxLength;
            var stringBuilder = new StringBuilder();

            for ( var i = 0; i <= iterations; i++ ) {
                var tempBytes = new Byte[dataLength - maxLength * i > maxLength ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy( src: bytes, srcOffset: maxLength * i, dst: tempBytes, dstOffset: 0, count: tempBytes.Length );
                var encryptedBytes = rsaCryptoServiceProvider.Encrypt( rgb: tempBytes, fOAEP: true );

                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes. It
                // does this after encryption and before decryption. If you do not require
                // compatibility with Microsoft Cryptographic API (CAPI) and/or other vendors
                // Comment out the next line and the corresponding one in the DecryptString function.
                Array.Reverse( array: encryptedBytes );

                // Why convert to base 64? Because it is the largest power-of-two base printable
                // using only ASCII characters
                stringBuilder.Append( Convert.ToBase64String( inArray: encryptedBytes ) );
            }

            return stringBuilder.ToString();
        }
    }
}