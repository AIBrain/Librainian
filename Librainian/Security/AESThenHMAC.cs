// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "AESThenHMAC.cs" last formatted on 2021-11-30 at 7:22 PM by Protiguous.

namespace Librainian.Security;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Exceptions;
using Maths;

/// <summary>
/// This work (Modern Encryption of a String C#, by James Tuley), identified by James Tuley, is free of known copyright
/// restrictions. <remarks>https://gist.github.com/4336842</remarks><remarks>http://creativecommons.org/publicdomain/mark/1.0/</remarks>
/// </summary>
public static class AESThenHmac {

	public const Int32 BlockBitSize = 128;

	//Preconfigured Password Key Derivation Parameters
	public const Int32 Iterations = 10000;

	//Preconfigured Encryption Parameters
	public const Int32 KeyBitSize = 256;

	//private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();
	public const Int32 MinPasswordLength = 12;

	public const Int32 SaltBitSize = 64;

	/// <summary>Helper that generates a random key on each call.</summary>
	public static Byte[] NewKey() {
		var key = new Byte[ KeyBitSize / 8 ];
		Randem.RNG.Value?.GetBytes( key );
		return key;
	}

	/// <summary>Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.</summary>
	/// <param name="encryptedMessage">The encrypted message.</param>
	/// <param name="cryptKey">The crypt key.</param>
	/// <param name="authKey">The auth key.</param>
	/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
	/// <returns>Decrypted Message</returns>
	public static Byte[]? SimpleDecrypt( Byte[] encryptedMessage, Byte[] cryptKey, Byte[] authKey, Int32 nonSecretPayloadLength = 0 ) {

		//Basic Usage Error Checks
		if ( cryptKey.Length != KeyBitSize / 8 ) {
			throw new ArgumentException( $"CryptKey needs to be {KeyBitSize} bits.", nameof( cryptKey ) );
		}

		if ( authKey.Length != KeyBitSize / 8 ) {
			throw new ArgumentException( $"AuthKey needs to be {KeyBitSize} bits.", nameof( authKey ) );
		}

		if ( encryptedMessage is null || encryptedMessage.Length == 0 ) {
			throw new ArgumentException( "Encrypted message Required.", nameof( encryptedMessage ) );
		}

		using var hmac = new HMACSHA256( authKey );

		var sentTag = new Byte[ hmac.HashSize / 8 ];

		//Calculate Tag
		var calcTag = hmac.ComputeHash( encryptedMessage, 0, encryptedMessage.Length - sentTag.Length );
		const Int32 ivLength = BlockBitSize / 8;

		//if message length is too small just return null
		if ( encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength ) {
			return default( Byte[]? );
		}

		//Grab Sent Tag
		Array.Copy( encryptedMessage, encryptedMessage.Length - sentTag.Length, sentTag, 0, sentTag.Length );

		//Compare Tag with constant time comparison
		var compare = 0;

		for ( var i = 0; i < sentTag.Length; i++ ) {
			compare |= sentTag[ i ] ^ calcTag[ i ];
		}

		//if message doesn't authenticate return null
		if ( compare != 0 ) {
			return default( Byte[]? );
		}

		using var aes = Aes.Create( "AesManaged" );
		if ( aes is null ) {
			throw new NullException( "AesManaged." );
		}

		aes.KeySize = KeyBitSize;
		aes.BlockSize = BlockBitSize;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;

		//Grab IV from message
		var iv = new Byte[ ivLength ];
		Array.Copy( encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length );

		using var decrypter = aes.CreateDecryptor( cryptKey, iv );

		using var plainTextStream = new MemoryStream();

		using var decrypterStream = new CryptoStream( plainTextStream, decrypter, CryptoStreamMode.Write );

		using var binaryWriter = new BinaryWriter( decrypterStream );

		//Decrypt Cipher Text from Message
		binaryWriter.Write( encryptedMessage, nonSecretPayloadLength + iv.Length, encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length );

		//Return Plain Text
		return plainTextStream.ToArray();
	}

	/// <summary>Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.</summary>
	/// <param name="encryptedMessage">The encrypted message.</param>
	/// <param name="cryptKey">The crypt key.</param>
	/// <param name="authKey">The auth key.</param>
	/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
	/// <returns>Decrypted Message</returns>
	/// <exception cref="NullException">Encrypted Message Required!;encryptedMessage</exception>
	public static String? SimpleDecrypt( String encryptedMessage, Byte[] cryptKey, Byte[] authKey, Int32 nonSecretPayloadLength = 0 ) {
		if ( String.IsNullOrWhiteSpace( encryptedMessage ) ) {
			throw new ArgumentException( "Encrypted message Required", nameof( encryptedMessage ) );
		}

		var cipherText = Convert.FromBase64String( encryptedMessage );
		var plainText = SimpleDecrypt( cipherText, cryptKey, authKey, nonSecretPayloadLength );

		return plainText is null ? null : Encoding.UTF8.GetString( plainText );
	}

	/// <summary>
	/// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message using keys derived from a password (PBKDF2).
	/// </summary>
	/// <param name="encryptedMessage">The encrypted message.</param>
	/// <param name="password">The password.</param>
	/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
	/// <returns>Decrypted Message</returns>
	/// <exception cref="NullException">Encrypted Message Required!;encryptedMessage</exception>
	/// <remarks>Significantly less secure than using random binary keys.</remarks>
	public static String? SimpleDecryptWithPassword( String encryptedMessage, String password, Int32 nonSecretPayloadLength = 0 ) {
		if ( String.IsNullOrWhiteSpace( encryptedMessage ) ) {
			throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
		}

		if ( String.IsNullOrEmpty( password ) ) {
			throw new NullException( nameof( password ) );
		}

		if ( !nonSecretPayloadLength.Any() ) {
			throw new ArgumentOutOfRangeException( nameof( nonSecretPayloadLength ) );
		}

		var cipherText = Convert.FromBase64String( encryptedMessage );
		var plainText = SimpleDecryptWithPassword( cipherText, password, nonSecretPayloadLength );

		return plainText is null ? null : Encoding.UTF8.GetString( plainText );
	}

	/// <summary>
	/// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message using keys derived from a password (PBKDF2).
	/// </summary>
	/// <param name="encryptedMessage">The encrypted message.</param>
	/// <param name="password">The password.</param>
	/// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
	/// <returns>Decrypted Message</returns>
	/// <exception cref="NullException">Must have a password of minimum length;password</exception>
	/// <remarks>Significantly less secure than using random binary keys.</remarks>
	public static Byte[]? SimpleDecryptWithPassword( Byte[] encryptedMessage, String password, Int32 nonSecretPayloadLength = 0 ) {

		//User Error Checks
		if ( String.IsNullOrWhiteSpace( password ) || password.Length < MinPasswordLength ) {
			throw new ArgumentException( $"Must have a password of at least {MinPasswordLength} characters.", nameof( password ) );
		}

		if ( encryptedMessage is null || encryptedMessage.Length == 0 ) {
			throw new ArgumentException( "Encrypted message required.", nameof( encryptedMessage ) );
		}

		var cryptSalt = new Byte[ SaltBitSize / 8 ];
		var authSalt = new Byte[ SaltBitSize / 8 ];

		//Grab Salt from Non-Secret Payload
		Array.Copy( encryptedMessage, nonSecretPayloadLength, cryptSalt, 0, cryptSalt.Length );

		Array.Copy( encryptedMessage, nonSecretPayloadLength + cryptSalt.Length, authSalt, 0, authSalt.Length );

		Byte[] cryptKey;
		Byte[] authKey;

		//Generate crypt key
		using ( var generator = new Rfc2898DeriveBytes( password, cryptSalt, Iterations ) ) {
			cryptKey = generator.GetBytes( KeyBitSize / 8 );
		}

		//Generate auth key
		using ( var generator = new Rfc2898DeriveBytes( password, authSalt, Iterations ) ) {
			authKey = generator.GetBytes( KeyBitSize / 8 );
		}

		return SimpleDecrypt( encryptedMessage, cryptKey, authKey, cryptSalt.Length + authSalt.Length + nonSecretPayloadLength );
	}

	/// <summary>Simple Encryption(AES) then Authentication (HMAC) for a UTF8 Message.</summary>
	/// <param name="secretMessage">The secret message.</param>
	/// <param name="cryptKey">The crypt key.</param>
	/// <param name="authKey">The auth key.</param>
	/// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
	/// <returns>Encrypted Message</returns>
	/// <remarks>
	/// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize + HMac-Tag(32)) * 1.33 Base64
	/// </remarks>
	public static Byte[] SimpleEncrypt( this Byte[] secretMessage, Byte[] cryptKey, Byte[] authKey, Byte[]? nonSecretPayload = null ) {

		//User Error Checks
		if ( cryptKey.Length != KeyBitSize / 8 ) {
			throw new ArgumentException( $"Key needs to be {KeyBitSize} bits.", nameof( cryptKey ) );
		}

		if ( authKey.Length != KeyBitSize / 8 ) {
			throw new ArgumentException( $"Key needs to be {KeyBitSize} bits.", nameof( authKey ) );
		}

		if ( secretMessage is null || secretMessage.Length < 1 ) {
			throw new ArgumentException( "Secret message required.", nameof( secretMessage ) );
		}

		//non-secret payload optional
		nonSecretPayload ??= Array.Empty<Byte>();

		Byte[] cipherText;

		using var aes = Aes.Create( "AesManaged" );
		if ( aes is null ) {
			throw new InvalidOperationException( "Error creating AesManaged encryptor." );
		}

		aes.KeySize = KeyBitSize;
		aes.BlockSize = BlockBitSize;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		aes.GenerateIV();

		using ( var encrypter = aes.CreateEncryptor( cryptKey, aes.IV ) ) {
			using var memoryStream = new MemoryStream();
			using var binaryWriter = new BinaryWriter( new CryptoStream( memoryStream, encrypter, CryptoStreamMode.Write ) );
			binaryWriter.Write( secretMessage );

			cipherText = memoryStream.ToArray();
		}

		//Assemble encrypted message and add authentication
		using ( var hmac = new HMACSHA256( authKey ) ) {
			using var memoryStream = new MemoryStream();
			using var binaryWriter = new BinaryWriter( memoryStream );

			//Prepend non-secret payload if any
			binaryWriter.Write( nonSecretPayload );

			//Prepend IV
			binaryWriter.Write( aes.IV );

			//Write Ciphertext
			binaryWriter.Write( cipherText );
			binaryWriter.Flush(); //why?

			//Authenticate all data
			var tag = hmac.ComputeHash( memoryStream.ToArray() );

			//Postpend tag
			binaryWriter.Write( tag );

			return memoryStream.ToArray();
		}
	}

	/// <summary>Simple Encryption (AES) then Authentication (HMAC) for a UTF8 Message.</summary>
	/// <param name="secretMessage">The secret message.</param>
	/// <param name="cryptKey">The crypt key.</param>
	/// <param name="authKey">The auth key.</param>
	/// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
	/// <returns>Encrypted Message</returns>
	/// <exception cref="NullException">Secret Message Required!;secretMessage</exception>
	/// <remarks>
	/// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize + HMac-Tag(32)) * 1.33 Base64
	/// </remarks>
	public static String SimpleEncrypt( String secretMessage, Byte[] cryptKey, Byte[] authKey, Byte[]? nonSecretPayload = null ) {
		if ( String.IsNullOrEmpty( secretMessage ) ) {
			throw new ArgumentException( "Secret message required.", nameof( secretMessage ) );
		}

		var plainText = Encoding.UTF8.GetBytes( secretMessage );
		var cipherText = plainText.SimpleEncrypt( cryptKey, authKey, nonSecretPayload );

		return Convert.ToBase64String( cipherText );
	}

	/// <summary>
	/// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message using Keys derived from a Password (PBKDF2).
	/// </summary>
	/// <param name="secretMessage">The secret message.</param>
	/// <param name="password">The password.</param>
	/// <param name="nonSecretPayload">The non secret payload.</param>
	/// <returns>Encrypted Message</returns>
	/// <exception cref="NullException">password</exception>
	/// <remarks>
	/// Significantly less secure than using random binary keys. Adds additional non secret payload for key generation parameters.
	/// </remarks>
	public static String SimpleEncryptWithPassword( String secretMessage, String password, Byte[]? nonSecretPayload = null ) {
		if ( String.IsNullOrEmpty( secretMessage ) ) {
			throw new ArgumentException( "Secret message required.", nameof( secretMessage ) );
		}

		var plainText = Encoding.UTF8.GetBytes( secretMessage );
		var cipherText = SimpleEncryptWithPassword( plainText, password, nonSecretPayload );

		return Convert.ToBase64String( cipherText );
	}

	/// <summary>
	/// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message using Keys derived from a Password (PBKDF2)
	/// </summary>
	/// <param name="secretMessage">The secret message.</param>
	/// <param name="password">The password.</param>
	/// <param name="nonSecretPayload">The non secret payload.</param>
	/// <returns>Encrypted Message</returns>
	/// <exception cref="NullException">Must have a password of minimum length;password</exception>
	/// <remarks>
	/// Significantly less secure than using random binary keys. Adds additional non secret payload for key generation parameters.
	/// </remarks>
	public static Byte[] SimpleEncryptWithPassword( Byte[] secretMessage, String password, Byte[]? nonSecretPayload = null ) {
		nonSecretPayload ??= Array.Empty<Byte>();

		//User Error Checks
		if ( String.IsNullOrWhiteSpace( password ) || password.Length < MinPasswordLength ) {
			throw new ArgumentException( $"Must have a password of at least {MinPasswordLength} characters.", nameof( password ) );
		}

		if ( secretMessage is null || secretMessage.Length == 0 ) {
			throw new ArgumentException( "Secret message required.", nameof( secretMessage ) );
		}

		const Int32 saltsize = SaltBitSize / 8;
		const Int32 saltier = saltsize * 2;

		var payload = new Byte[ saltier + nonSecretPayload.Length ];

		Buffer.BlockCopy( nonSecretPayload, 0, payload, 0, nonSecretPayload.Length );

		//Array.Copy( nonSecretPayload, payload, nonSecretPayload.Length );

		var payloadIndex = nonSecretPayload.Length;

		Byte[] cryptKey;
		Byte[] authKey;

		//Use Random Salt to prevent pre-generated weak password attacks.
		using ( var generator = new Rfc2898DeriveBytes( password, saltsize, Iterations ) ) {
			var salt = generator.Salt;

			//Generate Keys
			cryptKey = generator.GetBytes( KeyBitSize / 8 );

			//Create Non Secret Payload
			Buffer.BlockCopy( salt, 0, payload, payloadIndex, salt.Length );

			//Array.Copy( salt, 0, payload, payloadIndex, salt.Length );

			payloadIndex += salt.Length;
		}

		//Deriving separate key, might be less efficient than using HKDF,
		//but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
		using ( var generator = new Rfc2898DeriveBytes( password, saltsize, Iterations ) ) {
			var salt = generator.Salt;

			//Generate Keys
			authKey = generator.GetBytes( KeyBitSize / 8 );

			//Create Rest of Non Secret Payload
			Buffer.BlockCopy( salt, 0, payload, payloadIndex, salt.Length );

			//Array.Copy( salt, 0, payload, payloadIndex, salt.Length );
		}

		return secretMessage.SimpleEncrypt( cryptKey, authKey, payload );
	}
}