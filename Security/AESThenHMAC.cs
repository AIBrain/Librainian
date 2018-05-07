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
// "Librainian/AESThenHMAC.cs" was last cleaned by Protiguous on 2018/05/06 at 2:22 PM

namespace Librainian.Security {

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Maths;

    /// <summary>
    ///     This work (Modern Encryption of a String C#, by James Tuley), identified by James Tuley, is
    ///     free of known copyright restrictions.
    ///     https: //gist.github.com/4336842
    ///     http: //creativecommons.org/publicdomain/mark/1.0/
    /// </summary>
    public static class AESThenHmac {
        public const Int32 BlockBitSize = 128;
        public const Int32 Iterations = 10000;
        public const Int32 KeyBitSize = 256;
        public const Int32 MinPasswordLength = 12;
        public const Int32 SaltBitSize = 64;

        //private static readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

        //Preconfigured Encryption Parameters

        //Preconfigured Password Key Derivation Parameters

        /// <summary>Helper that generates a random key on each call.</summary>
        /// <returns></returns>
        public static Byte[] NewKey() {
            var key = new Byte[KeyBitSize / 8];
            Randem.RNG.Value.GetBytes( data: key );
            return key;
        }

        /// <summary>
        ///     Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <param name="authKey">The auth key.</param>
        /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
        /// <returns>Decrypted Message</returns>
        public static Byte[] SimpleDecrypt( Byte[] encryptedMessage, Byte[] cryptKey, Byte[] authKey, Int32 nonSecretPayloadLength = 0 ) {

            //Basic Usage Error Checks
            if ( cryptKey is null || cryptKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( message: $"CryptKey needs to be {KeyBitSize} bit!", paramName: nameof( cryptKey ) );
            }

            if ( authKey is null || authKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( message: $"AuthKey needs to be {KeyBitSize} bit!", paramName: nameof( authKey ) );
            }

            if ( encryptedMessage is null || encryptedMessage.Length == 0 ) {
                throw new ArgumentException( message: "Encrypted Message Required!", paramName: nameof( encryptedMessage ) );
            }

            using ( var hmac = new HMACSHA256(authKey ) ) {
                var sentTag = new Byte[hmac.HashSize / 8];

                //Calculate Tag
                var calcTag = hmac.ComputeHash( buffer: encryptedMessage, offset: 0, count: encryptedMessage.Length - sentTag.Length );
                var ivLength = BlockBitSize / 8;

                //if message length is to small just return null
                if ( encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength ) {
                    return null;
                }

                //Grab Sent Tag
                Array.Copy( sourceArray: encryptedMessage, sourceIndex: encryptedMessage.Length - sentTag.Length, destinationArray: sentTag, destinationIndex: 0,sentTag.Length );

                //Compare Tag with constant time comparison
                var compare = 0;
                for ( var i = 0; i < sentTag.Length; i++ ) {
                    compare |= sentTag[i] ^ calcTag[i];
                }

                //if message doesn't authenticate return null
                if ( compare != 0 ) {
                    return null;
                }

                using ( var aes = new AesManaged { KeySize = KeyBitSize, BlockSize = BlockBitSize, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 } ) {

                    //Grab IV from message
                    var iv = new Byte[ivLength];
                    Array.Copy( sourceArray: encryptedMessage, sourceIndex: nonSecretPayloadLength, destinationArray: iv, destinationIndex: 0,iv.Length );

                    using ( var decrypter = aes.CreateDecryptor(cryptKey, iv: iv ) ) {
                        var plainTextStream = new MemoryStream();
                        var decrypterStream = new CryptoStream( stream: plainTextStream, transform: decrypter, mode: CryptoStreamMode.Write );
                        using ( var binaryWriter = new BinaryWriter( output: decrypterStream ) ) {

                            //Decrypt Cipher Text from Message
                            binaryWriter.Write( buffer: encryptedMessage, index: nonSecretPayloadLength + iv.Length, count: encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length );
                        }

                        //Return Plain Text
                        return plainTextStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        ///     Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <param name="authKey">The auth key.</param>
        /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
        /// <returns>Decrypted Message</returns>
        /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
        public static String SimpleDecrypt( String encryptedMessage, Byte[] cryptKey, Byte[] authKey, Int32 nonSecretPayloadLength = 0 ) {
            if ( String.IsNullOrWhiteSpace( value: encryptedMessage ) ) {
                throw new ArgumentException( message: "Encrypted Message Required!", paramName: nameof( encryptedMessage ) );
            }

            var cipherText = Convert.FromBase64String( s: encryptedMessage );
            var plainText = SimpleDecrypt( encryptedMessage: cipherText, cryptKey: cryptKey, authKey: authKey, nonSecretPayloadLength: nonSecretPayloadLength );
            return Encoding.UTF8.GetString( bytes: plainText );
        }

        /// <summary>
        ///     Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message using keys
        ///     derived from a password (PBKDF2).
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <param name="password">The password.</param>
        /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
        /// <returns>Decrypted Message</returns>
        /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
        /// <remarks>Significantly less secure than using random binary keys.</remarks>
        public static String SimpleDecryptWithPassword( String encryptedMessage, String password, Int32 nonSecretPayloadLength = 0 ) {
            if ( String.IsNullOrWhiteSpace( value: encryptedMessage ) ) {
                throw new ArgumentException( message: "Encrypted Message Required!", paramName: nameof( encryptedMessage ) );
            }

            var cipherText = Convert.FromBase64String( s: encryptedMessage );
            var plainText = SimpleDecryptWithPassword( encryptedMessage: cipherText, password: password, nonSecretPayloadLength: nonSecretPayloadLength );
            return Encoding.UTF8.GetString( bytes: plainText );
        }

        /// <summary>
        ///     Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message using keys
        ///     derived from a password (PBKDF2).
        /// </summary>
        /// <param name="encryptedMessage">The encrypted message.</param>
        /// <param name="password">The password.</param>
        /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
        /// <returns>Decrypted Message</returns>
        /// <exception cref="System.ArgumentException">Must have a password of minimum length;password</exception>
        /// <remarks>Significantly less secure than using random binary keys.</remarks>
        public static Byte[] SimpleDecryptWithPassword( Byte[] encryptedMessage, String password, Int32 nonSecretPayloadLength = 0 ) {

            //User Error Checks
            if ( String.IsNullOrWhiteSpace( value: password ) || password.Length < MinPasswordLength ) {
                throw new ArgumentException( message: $"Must have a password of at least {MinPasswordLength} characters!", paramName: nameof( password ) );
            }

            if ( encryptedMessage is null || encryptedMessage.Length == 0 ) {
                throw new ArgumentException( message: "Encrypted Message Required!", paramName: nameof( encryptedMessage ) );
            }

            var cryptSalt = new Byte[SaltBitSize / 8];
            var authSalt = new Byte[SaltBitSize / 8];

            //Grab Salt from Non-Secret Payload
            Array.Copy( sourceArray: encryptedMessage, sourceIndex: nonSecretPayloadLength, destinationArray: cryptSalt, destinationIndex: 0,cryptSalt.Length );
            Array.Copy( sourceArray: encryptedMessage, sourceIndex: nonSecretPayloadLength + cryptSalt.Length, destinationArray: authSalt, destinationIndex: 0,authSalt.Length );

            Byte[] cryptKey;
            Byte[] authKey;

            //Generate crypt key
            using ( var generator = new Rfc2898DeriveBytes( password: password, salt: cryptSalt, iterations: Iterations ) ) {
                cryptKey = generator.GetBytes( cb: KeyBitSize / 8 );
            }

            //Generate auth key
            using ( var generator = new Rfc2898DeriveBytes( password: password, salt: authSalt, iterations: Iterations ) ) {
                authKey = generator.GetBytes( cb: KeyBitSize / 8 );
            }

            return SimpleDecrypt( encryptedMessage: encryptedMessage, cryptKey: cryptKey, authKey: authKey, nonSecretPayloadLength: cryptSalt.Length + authSalt.Length + nonSecretPayloadLength );
        }

        /// <summary>Simple Encryption(AES) then Authentication (HMAC) for a UTF8 Message.</summary>
        /// <param name="secretMessage">The secret message.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <param name="authKey">The auth key.</param>
        /// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
        /// <returns>Encrypted Message</returns>
        /// <remarks>
        ///     Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +
        ///     HMac-Tag(32)) * 1.33 Base64
        /// </remarks>
        public static Byte[] SimpleEncrypt( this Byte[] secretMessage, Byte[] cryptKey, Byte[] authKey, Byte[] nonSecretPayload = null ) {

            //User Error Checks
            if ( cryptKey is null || cryptKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( message: $"Key needs to be {KeyBitSize} bit!", paramName: nameof( cryptKey ) );
            }

            if ( authKey is null || authKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( message: $"Key needs to be {KeyBitSize} bit!", paramName: nameof( authKey ) );
            }

            if ( secretMessage is null || secretMessage.Length < 1 ) {
                throw new ArgumentException( message: "Secret Message Required!", paramName: nameof( secretMessage ) );
            }

            //non-secret payload optional
            nonSecretPayload = nonSecretPayload ?? new Byte[] { };

            Byte[] cipherText;
            Byte[] iv;

            using ( var aes = new AesManaged { KeySize = KeyBitSize, BlockSize = BlockBitSize, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 } ) {

                //Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                using ( var encrypter = aes.CreateEncryptor(cryptKey, iv: iv ) ) {
                    var cipherStream = new MemoryStream();
                    using ( var binaryWriter = new BinaryWriter( output: new CryptoStream( stream: cipherStream, transform: encrypter, mode: CryptoStreamMode.Write ) ) ) {
                        binaryWriter.Write( buffer: secretMessage );
                    }

                    cipherText = cipherStream.ToArray();
                }
            }

            //Assemble encrypted message and add authentication
            using ( var hmac = new HMACSHA256(authKey ) ) {
                var encryptedStream = new MemoryStream();
                using ( var binaryWriter = new BinaryWriter( output: encryptedStream ) ) {

                    //Prepend non-secret payload if any
                    binaryWriter.Write( buffer: nonSecretPayload );

                    //Prepend IV
                    binaryWriter.Write( buffer: iv );

                    //Write Ciphertext
                    binaryWriter.Write( buffer: cipherText );
                    binaryWriter.Flush(); //why?

                    //Authenticate all data
                    var tag = hmac.ComputeHash( buffer: encryptedStream.ToArray() );

                    //Postpend tag
                    binaryWriter.Write( buffer: tag );
                }

                return encryptedStream.ToArray();
            }
        }

        /// <summary>Simple Encryption (AES) then Authentication (HMAC) for a UTF8 Message.</summary>
        /// <param name="secretMessage">The secret message.</param>
        /// <param name="cryptKey">The crypt key.</param>
        /// <param name="authKey">The auth key.</param>
        /// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
        /// <returns>Encrypted Message</returns>
        /// <exception cref="System.ArgumentException">Secret Message Required!;secretMessage</exception>
        /// <remarks>
        ///     Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +
        ///     HMac-Tag(32)) * 1.33 Base64
        /// </remarks>
        public static String SimpleEncrypt( String secretMessage, Byte[] cryptKey, Byte[] authKey, Byte[] nonSecretPayload = null ) {
            if ( String.IsNullOrEmpty( value: secretMessage ) ) {
                throw new ArgumentException( message: "Secret Message Required!", paramName: nameof( secretMessage ) );
            }

            var plainText = Encoding.UTF8.GetBytes( s: secretMessage );
            var cipherText = plainText.SimpleEncrypt( cryptKey, authKey, nonSecretPayload );
            return Convert.ToBase64String( inArray: cipherText );
        }

        /// <summary>
        ///     Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message using Keys derived
        ///     from a Password (PBKDF2).
        /// </summary>
        /// <param name="secretMessage">The secret message.</param>
        /// <param name="password">The password.</param>
        /// <param name="nonSecretPayload">The non secret payload.</param>
        /// <returns>Encrypted Message</returns>
        /// <exception cref="System.ArgumentException">password</exception>
        /// <remarks>
        ///     Significantly less secure than using random binary keys. Adds additional non secret
        ///     payload for key generation parameters.
        /// </remarks>
        public static String SimpleEncryptWithPassword( String secretMessage, String password, Byte[] nonSecretPayload = null ) {
            if ( String.IsNullOrEmpty( value: secretMessage ) ) {
                throw new ArgumentException( message: "Secret Message Required!", paramName: nameof( secretMessage ) );
            }

            var plainText = Encoding.UTF8.GetBytes( s: secretMessage );
            var cipherText = SimpleEncryptWithPassword( secretMessage: plainText, password: password, nonSecretPayload: nonSecretPayload );
            return Convert.ToBase64String( inArray: cipherText );
        }

        /// <summary>
        ///     Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message using Keys derived
        ///     from a Password (PBKDF2)
        /// </summary>
        /// <param name="secretMessage">The secret message.</param>
        /// <param name="password">The password.</param>
        /// <param name="nonSecretPayload">The non secret payload.</param>
        /// <returns>Encrypted Message</returns>
        /// <exception cref="System.ArgumentException">Must have a password of minimum length;password</exception>
        /// <remarks>
        ///     Significantly less secure than using random binary keys. Adds additional non secret
        ///     payload for key generation parameters.
        /// </remarks>
        public static Byte[] SimpleEncryptWithPassword( Byte[] secretMessage, String password, Byte[] nonSecretPayload = null ) {
            nonSecretPayload = nonSecretPayload ?? new Byte[] { };

            //User Error Checks
            if ( String.IsNullOrWhiteSpace( value: password ) || password.Length < MinPasswordLength ) {
                throw new ArgumentException( message: $"Must have a password of at least {MinPasswordLength} characters!", paramName: nameof( password ) );
            }

            if ( secretMessage is null || secretMessage.Length == 0 ) {
                throw new ArgumentException( message: "Secret Message Required!", paramName: nameof( secretMessage ) );
            }

            var payload = new Byte[SaltBitSize / 8 * 2 + nonSecretPayload.Length];

            Array.Copy( sourceArray: nonSecretPayload, destinationArray: payload,nonSecretPayload.Length );
            var payloadIndex = nonSecretPayload.Length;

            Byte[] cryptKey;
            Byte[] authKey;

            //Use Random Salt to prevent pre-generated weak password attacks.
            using ( var generator = new Rfc2898DeriveBytes( password: password, saltSize: SaltBitSize / 8, iterations: Iterations ) ) {
                var salt = generator.Salt;

                //Generate Keys
                cryptKey = generator.GetBytes( cb: KeyBitSize / 8 );

                //Create Non Secret Payload
                Array.Copy( sourceArray: salt, sourceIndex: 0, destinationArray: payload, destinationIndex: payloadIndex,salt.Length );
                payloadIndex += salt.Length;
            }

            //Deriving separate key, might be less efficient than using HKDF,
            //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
            using ( var generator = new Rfc2898DeriveBytes( password: password, saltSize: SaltBitSize / 8, iterations: Iterations ) ) {
                var salt = generator.Salt;

                //Generate Keys
                authKey = generator.GetBytes( cb: KeyBitSize / 8 );

                //Create Rest of Non Secret Payload
                Array.Copy( sourceArray: salt, sourceIndex: 0, destinationArray: payload, destinationIndex: payloadIndex,salt.Length );
            }

            return secretMessage.SimpleEncrypt( cryptKey, authKey, payload );
        }
    }
}