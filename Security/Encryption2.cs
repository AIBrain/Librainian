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
// "Librainian2/Encryption2.cs" was last cleaned by Rick on 2014/08/08 at 2:31 PM
#endregion

namespace Librainian.Security {
    using System;
    using System.Collections;
    using System.Security.Cryptography;
    using System.Text;
    using Annotations;

    public static class Encryption2 {
        public static string EncryptString( [NotNull] this string inputString, int dwKeySize, [NotNull] string xmlString ) {
            // TODO: Add Proper Exception Handlers
            if ( inputString == null ) {
                throw new ArgumentNullException( "inputString" );
            }
            if ( xmlString == null ) {
                throw new ArgumentNullException( "xmlString" );
            }
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString );
            var keySize = dwKeySize/8;
            var bytes = Encoding.UTF32.GetBytes( inputString );
            // The hash function in use by the .NET RSACryptoServiceProvider here 
            // is SHA1
            // int maxLength = ( keySize ) - 2 - 
            //              ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            var maxLength = keySize - 42;
            var dataLength = bytes.Length;
            var iterations = dataLength/maxLength;
            var stringBuilder = new StringBuilder();
            for ( var i = 0; i <= iterations; i++ ) {
                var tempBytes = new byte[( dataLength - maxLength*i > maxLength ) ? maxLength : dataLength - maxLength*i];
                Buffer.BlockCopy( bytes, maxLength*i, tempBytes, 0, tempBytes.Length );
                var encryptedBytes = rsaCryptoServiceProvider.Encrypt( tempBytes, true );
                // Be aware the RSACryptoServiceProvider reverses the order of encrypted bytes.
                // It does this after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic API (CAPI)
                // and/or other vendors Comment out the next line and the corresponding one in the DecryptString function.
                Array.Reverse( encryptedBytes );
                // Why convert to base 64?
                // Because it is the largest power-of-two base printable using only 
                // ASCII characters
                stringBuilder.Append( Convert.ToBase64String( encryptedBytes ) );
            }
            return stringBuilder.ToString();
        }

        public static string DecryptString( [NotNull] this string inputString, int dwKeySize, [NotNull] string xmlString ) {
            // TODO: Add Proper Exception Handlers
            if ( inputString == null ) {
                throw new ArgumentNullException( "inputString" );
            }
            if ( xmlString == null ) {
                throw new ArgumentNullException( "xmlString" );
            }
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider( dwKeySize );
            rsaCryptoServiceProvider.FromXmlString( xmlString );
            var base64BlockSize = ( ( dwKeySize/8 )%3 != 0 ) ? ( ( ( dwKeySize/8 )/3 )*4 ) + 4 : ( ( dwKeySize/8 )/3 )*4;
            var iterations = inputString.Length/base64BlockSize;
            var arrayList = new ArrayList();
            for ( var i = 0; i < iterations; i++ ) {
                var encryptedBytes = Convert.FromBase64String( inputString.Substring( base64BlockSize*i, base64BlockSize ) );
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic 
                // API (CAPI) and/or other vendors.
                // Comment out the next line and the corresponding one in the 
                // EncryptString function.
                Array.Reverse( encryptedBytes );
                arrayList.AddRange( rsaCryptoServiceProvider.Decrypt( encryptedBytes, true ) );
            }
            return Encoding.UTF32.GetString( arrayList.ToArray( Type.GetType( "System.Byte" ) ) as byte[] );
        }
    }
}
