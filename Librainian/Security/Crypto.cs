// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "Crypto.cs" last formatted on 2020-08-14 at 8:44 PM.

namespace Librainian.Security {

	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;
	using Exceptions;

	public static class Crypto {

		private static readonly Byte[] Salt = Encoding.ASCII.GetBytes( "evatuewot8evtet8e8paaa40aqtab60w489uvmw" );

		/// <summary>
		///     Decrypt the given string. Assumes the string was encrypted using EncryptStringAES(), using an identical
		///     sharedSecret.
		/// </summary>
		/// <param name="cipherText">The text to decrypt.</param>
		/// <param name="sharedSecret">A password used to generate a key for decryption.</param>
		public static String DecryptStringAES( this String cipherText, String sharedSecret ) {
			if ( String.IsNullOrEmpty( cipherText ) ) {
				throw new ArgumentEmptyException( nameof( cipherText ) );
			}

			if ( String.IsNullOrEmpty( sharedSecret ) ) {
				throw new ArgumentEmptyException( nameof( sharedSecret ) );
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
				var msDecrypt = new MemoryStream( bytes );

				// Create a RijndaelManaged object with the specified key and IV.
				aesAlg = new RijndaelManaged();
				aesAlg.Key = key.GetBytes( aesAlg.KeySize / 8 );

				// Get the initialization vector from the encrypted stream
				aesAlg.IV = msDecrypt.ReadByteArray();

				// Create a decrytor to perform the stream transform.
				var decryptor = aesAlg.CreateDecryptor( aesAlg.Key, aesAlg.IV );

				using var srDecrypt = new StreamReader( new CryptoStream( msDecrypt, decryptor, CryptoStreamMode.Read ) );

				// Read the decrypted bytes from the decrypting stream and place them in
				// a string.
				plaintext = srDecrypt.ReadToEnd();
			}
			finally {

				// Clear the RijndaelManaged object.
				aesAlg?.Clear();
			}

			return plaintext;
		}

		/// <summary>
		///     Encrypt the given string using AES. The string can be decrypted using DecryptStringAES(). The sharedSecret
		///     parameters must match.
		/// </summary>
		/// <param name="plainText">The text to encrypt.</param>
		/// <param name="sharedSecret">A password used to generate a key for encryption.</param>
		public static String EncryptStringAES( this String plainText, String sharedSecret ) {
			if ( String.IsNullOrEmpty( plainText ) ) {
				throw new ArgumentEmptyException( nameof( plainText ) );
			}

			if ( String.IsNullOrEmpty( sharedSecret ) ) {
				throw new ArgumentEmptyException( nameof( sharedSecret ) );
			}

			String outStr;                 // Encrypted string to return
			//AES aesAlg = null;
			RijndaelManaged? aesAlg = null; // RijndaelManaged object used to encrypt the data.

			try {

				// generate the key from the shared secret and the salt
				var key = new Rfc2898DeriveBytes( sharedSecret, Salt );

				// Create a RijndaelManaged object
				aesAlg = new RijndaelManaged();
				aesAlg.Key = key.GetBytes( aesAlg.KeySize / 8 );

				// Create a decryptor to perform the stream transform.
				var encryptor = aesAlg.CreateEncryptor( aesAlg.Key, aesAlg.IV );

				// Create the streams used for encryption.
				var msEncrypt = new MemoryStream();

				// prepend the IV
				msEncrypt.Write( BitConverter.GetBytes( aesAlg.IV.Length ), 0, sizeof( Int32 ) );
				msEncrypt.Write( aesAlg.IV, 0, aesAlg.IV.Length );

				var csEncrypt = new CryptoStream( msEncrypt, encryptor, CryptoStreamMode.Write );

				using ( var swEncrypt = new StreamWriter( csEncrypt ) ) {

					//Write all data to the stream.
					swEncrypt.Write( plainText );
				}

				outStr = Convert.ToBase64String( msEncrypt.ToArray() );
			}
			finally {

				// Clear the RijndaelManaged object.
				aesAlg?.Clear();
			}

			// Return the encrypted bytes from the memory stream.
			return outStr;
		}

		public static Byte[] ReadByteArray( this Stream s ) {
			var rawLength = new Byte[ sizeof( Int32 ) ];

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