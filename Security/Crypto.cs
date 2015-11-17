// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Class1.cs" was last cleaned by Rick on 2015/06/15 at 3:36 PM

// resource:
// https: //github.com/Digiex/MCLauncher.NET/blob/master/MCLauncher.net/Crypto.cs

namespace Librainian.Security {

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Crypto {
        private static readonly Byte[] Salt = Encoding.ASCII.GetBytes( "evatuewot8evtet8e8paaa40aqta" );

        /// <summary>
        /// Decrypt the given string. Assumes the string was encrypted using EncryptStringAES(),
        /// using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static String DecryptStringAES(this String cipherText, String sharedSecret) {
            if ( String.IsNullOrEmpty( cipherText ) ) {
                throw new ArgumentNullException( nameof( cipherText ) );
            }
            if ( String.IsNullOrEmpty( sharedSecret ) ) {
                throw new ArgumentNullException( nameof( sharedSecret ) );
            }

            // Declare the RijndaelManaged object used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold the decrypted text.
            String plaintext;

            try {

                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes( sharedSecret, Salt );

                // Create the streams used for decryption.
                var bytes = Convert.FromBase64String( cipherText );
                using ( var msDecrypt = new MemoryStream( bytes ) ) {

                    // Create a RijndaelManaged object with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes( aesAlg.KeySize / 8 );

                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = msDecrypt.ReadByteArray();

                    // Create a decrytor to perform the stream transform.
                    var decryptor = aesAlg.CreateDecryptor( aesAlg.Key, aesAlg.IV );
                    using ( var csDecrypt = new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read ) ) {
                        using ( var srDecrypt = new StreamReader( csDecrypt ) ) {

                            // Read the decrypted bytes from the decrypting stream and place them in
                            // a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally {
                // Clear the RijndaelManaged object.
                aesAlg?.Clear();
            }

            return plaintext;
        }

        /// <summary>
        /// Encrypt the given string using AES. The string can be decrypted using
        /// DecryptStringAES(). The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static String EncryptStringAES(String plainText, String sharedSecret) {
            if ( String.IsNullOrEmpty( plainText ) ) {
                throw new ArgumentNullException( nameof( plainText ) );
            }
            if ( String.IsNullOrEmpty( sharedSecret ) ) {
                throw new ArgumentNullException( nameof( sharedSecret ) );
            }

            String outStr; // Encrypted string to return
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try {

                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes( sharedSecret, Salt );

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes( aesAlg.KeySize / 8 );

                // Create a decryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor( aesAlg.Key, aesAlg.IV );

                // Create the streams used for encryption.
                using ( var msEncrypt = new MemoryStream() ) {

                    // prepend the IV
                    msEncrypt.Write( BitConverter.GetBytes( aesAlg.IV.Length ), 0, sizeof(Int32) );
                    msEncrypt.Write( aesAlg.IV, 0, aesAlg.IV.Length );
                    using ( var csEncrypt = new CryptoStream( msEncrypt, encryptor, CryptoStreamMode.Write ) ) {
                        using ( var swEncrypt = new StreamWriter( csEncrypt ) ) {

                            //Write all data to the stream.
                            swEncrypt.Write( plainText );
                        }
                    }
                    outStr = Convert.ToBase64String( msEncrypt.ToArray() );
                }
            }
            finally {
                // Clear the RijndaelManaged object.
                aesAlg?.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        public static Byte[] ReadByteArray( this Stream s) {
            var rawLength = new Byte[ sizeof(Int32) ];
            if ( s.Read( rawLength, 0, rawLength.Length ) != rawLength.Length ) {
                throw new SystemException( "Stream did not contain properly formatted byte array" );
            }

            var buffer = new Byte[ BitConverter.ToInt32( rawLength, 0 ) ];
            if ( s.Read( buffer, 0, buffer.Length ) != buffer.Length ) {
                throw new SystemException( "Did not read byte array properly" );
            }

            return buffer;
        }
    }
}