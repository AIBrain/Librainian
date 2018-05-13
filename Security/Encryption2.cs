// Copyright 2018 Protiguous
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Encryption2.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.Collections;
    using System.Security.Cryptography;
    using System.Text;
    using JetBrains.Annotations;

    public static class Encryption2 {

        public static String DecryptString( [NotNull] this String inputString, Int32 dwKeySize, [NotNull] String xmlString ) {

            // TODO: Add Proper Exception Handlers
            if ( inputString is null ) {
                throw new ArgumentNullException(nameof( inputString ) );
            }

            if ( xmlString is null ) {
                throw new ArgumentNullException(nameof( xmlString ) );
            }

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize: dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString: xmlString );
            var base64BlockSize = dwKeySize / 8 % 3 != 0 ? dwKeySize / 8 / 3 * 4 + 4 : dwKeySize / 8 / 3 * 4;
            var iterations = inputString.Length / base64BlockSize;
            var arrayList = new ArrayList();
            for ( var i = 0; i < iterations; i++ ) {
                var encryptedBytes = Convert.FromBase64String( s: inputString.Substring( startIndex: base64BlockSize * i,base64BlockSize ) );

                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes after
                // encryption and before decryption. If you do not require compatibility with
                // Microsoft Cryptographic API (CAPI) and/or other vendors. Comment out the next
                // line and the corresponding one in the EncryptString function.
                Array.Reverse( array: encryptedBytes );
                arrayList.AddRange( c: rsaCryptoServiceProvider.Decrypt( rgb: encryptedBytes, fOAEP: true ) );
            }

            return !( arrayList.ToArray( type: typeof( Byte ) ) is Byte[] ba ) ? String.Empty : Encoding.UTF32.GetString( bytes: ba );
        }

        public static String EncryptString( [NotNull] this String inputString, Int32 dwKeySize, [NotNull] String xmlString ) {

            // TODO: Add Proper Exception Handlers
            if ( inputString is null ) {
                throw new ArgumentNullException(nameof( inputString ) );
            }

            if ( xmlString is null ) {
                throw new ArgumentNullException(nameof( xmlString ) );
            }

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize: dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString: xmlString );
            var keySize = dwKeySize / 8;
            var bytes = Encoding.UTF32.GetBytes( s: inputString );

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
                stringBuilder.Append( value: Convert.ToBase64String( inArray: encryptedBytes ) );
            }

            return stringBuilder.ToString();
        }
    }
}