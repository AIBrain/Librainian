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
// "Librainian/AESThenHMAC.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Security {

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Threading;

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
            var key = new Byte[ KeyBitSize / 8 ];
            Randem.RNG.Value.GetBytes( key );
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
            if ( ( cryptKey == null ) || ( cryptKey.Length != KeyBitSize / 8 ) ) {
                throw new ArgumentException( $"CryptKey needs to be {KeyBitSize} bit!", nameof( cryptKey ) );
            }

            if ( ( authKey == null ) || ( authKey.Length != KeyBitSize / 8 ) ) {
                throw new ArgumentException( $"AuthKey needs to be {KeyBitSize} bit!", nameof( authKey ) );
            }

            if ( ( encryptedMessage == null ) || ( encryptedMessage.Length == 0 ) ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            using ( var hmac = new HMACSHA256( authKey ) ) {
                var sentTag = new Byte[ hmac.HashSize / 8 ];

                //Calculate Tag
                var calcTag = hmac.ComputeHash( encryptedMessage, 0, encryptedMessage.Length - sentTag.Length );
                var ivLength = BlockBitSize / 8;

                //if message length is to small just return null
                if ( encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength ) {
                    return null;
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
                    return null;
                }

                using ( var aes = new AesManaged { KeySize = KeyBitSize, BlockSize = BlockBitSize, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 } ) {

                    //Grab IV from message
                    var iv = new Byte[ ivLength ];
                    Array.Copy( encryptedMessage, nonSecretPayloadLength, iv, 0, iv.Length );

                    using ( var decrypter = aes.CreateDecryptor( cryptKey, iv ) ) {
                        using ( var plainTextStream = new MemoryStream() ) {
                            using ( var decrypterStream = new CryptoStream( plainTextStream, decrypter, CryptoStreamMode.Write ) ) {
                                using ( var binaryWriter = new BinaryWriter( decrypterStream ) ) {

                                    //Decrypt Cipher Text from Message
                                    binaryWriter.Write( encryptedMessage, nonSecretPayloadLength + iv.Length, encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length );
                                }
                            }

                            //Return Plain Text
                            return plainTextStream.ToArray();
                        }
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
            if ( String.IsNullOrWhiteSpace( encryptedMessage ) ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            var cipherText = Convert.FromBase64String( encryptedMessage );
            var plainText = SimpleDecrypt( cipherText, cryptKey, authKey, nonSecretPayloadLength );
            return Encoding.UTF8.GetString( plainText );
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
            if ( String.IsNullOrWhiteSpace( encryptedMessage ) ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            var cipherText = Convert.FromBase64String( encryptedMessage );
            var plainText = SimpleDecryptWithPassword( cipherText, password, nonSecretPayloadLength );
            return Encoding.UTF8.GetString( plainText );
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
            if ( String.IsNullOrWhiteSpace( password ) || ( password.Length < MinPasswordLength ) ) {
                throw new ArgumentException( $"Must have a password of at least {MinPasswordLength} characters!", nameof( password ) );
            }

            if ( ( encryptedMessage == null ) || ( encryptedMessage.Length == 0 ) ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
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
        ///     Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +
        ///     HMac-Tag(32)) * 1.33 Base64
        /// </remarks>
        public static Byte[] SimpleEncrypt( Byte[] secretMessage, Byte[] cryptKey, Byte[] authKey, Byte[] nonSecretPayload = null ) {

            //User Error Checks
            if ( ( cryptKey == null ) || ( cryptKey.Length != KeyBitSize / 8 ) ) {
                throw new ArgumentException( $"Key needs to be {KeyBitSize} bit!", nameof( cryptKey ) );
            }

            if ( ( authKey == null ) || ( authKey.Length != KeyBitSize / 8 ) ) {
                throw new ArgumentException( $"Key needs to be {KeyBitSize} bit!", nameof( authKey ) );
            }

            if ( ( secretMessage == null ) || ( secretMessage.Length < 1 ) ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
            }

            //non-secret payload optional
            nonSecretPayload = nonSecretPayload ?? new Byte[] { };

            Byte[] cipherText;
            Byte[] iv;

            using ( var aes = new AesManaged { KeySize = KeyBitSize, BlockSize = BlockBitSize, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 } ) {

                //Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                using ( var encrypter = aes.CreateEncryptor( cryptKey, iv ) ) {
                    using ( var cipherStream = new MemoryStream() ) {
                        using ( var cryptoStream = new CryptoStream( cipherStream, encrypter, CryptoStreamMode.Write ) ) {
                            using ( var binaryWriter = new BinaryWriter( cryptoStream ) ) {

                                //Encrypt Data
                                binaryWriter.Write( secretMessage );
                            }
                        }

                        cipherText = cipherStream.ToArray();
                    }
                }
            }

            //Assemble encrypted message and add authentication
            using ( var hmac = new HMACSHA256( authKey ) ) {
                using ( var encryptedStream = new MemoryStream() ) {
                    using ( var binaryWriter = new BinaryWriter( encryptedStream ) ) {

                        //Prepend non-secret payload if any
                        binaryWriter.Write( nonSecretPayload );

                        //Prepend IV
                        binaryWriter.Write( iv );

                        //Write Ciphertext
                        binaryWriter.Write( cipherText );
                        binaryWriter.Flush();

                        //Authenticate all data
                        var tag = hmac.ComputeHash( encryptedStream.ToArray() );

                        //Postpend tag
                        binaryWriter.Write( tag );
                    }
                    return encryptedStream.ToArray();
                }
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
            if ( String.IsNullOrEmpty( secretMessage ) ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
            }

            var plainText = Encoding.UTF8.GetBytes( secretMessage );
            var cipherText = SimpleEncrypt( plainText, cryptKey, authKey, nonSecretPayload );
            return Convert.ToBase64String( cipherText );
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
            if ( String.IsNullOrEmpty( secretMessage ) ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
            }

            var plainText = Encoding.UTF8.GetBytes( secretMessage );
            var cipherText = SimpleEncryptWithPassword( plainText, password, nonSecretPayload );
            return Convert.ToBase64String( cipherText );
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
            if ( String.IsNullOrWhiteSpace( password ) || ( password.Length < MinPasswordLength ) ) {
                throw new ArgumentException( $"Must have a password of at least {MinPasswordLength} characters!", nameof( password ) );
            }

            if ( ( secretMessage == null ) || ( secretMessage.Length == 0 ) ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
            }

            var payload = new Byte[ SaltBitSize / 8 * 2 + nonSecretPayload.Length ];

            Array.Copy( nonSecretPayload, payload, nonSecretPayload.Length );
            var payloadIndex = nonSecretPayload.Length;

            Byte[] cryptKey;
            Byte[] authKey;

            //Use Random Salt to prevent pre-generated weak password attacks.
            using ( var generator = new Rfc2898DeriveBytes( password, SaltBitSize / 8, Iterations ) ) {
                var salt = generator.Salt;

                //Generate Keys
                cryptKey = generator.GetBytes( KeyBitSize / 8 );

                //Create Non Secret Payload
                Array.Copy( salt, 0, payload, payloadIndex, salt.Length );
                payloadIndex += salt.Length;
            }

            //Deriving separate key, might be less efficient than using HKDF,
            //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
            using ( var generator = new Rfc2898DeriveBytes( password, SaltBitSize / 8, Iterations ) ) {
                var salt = generator.Salt;

                //Generate Keys
                authKey = generator.GetBytes( KeyBitSize / 8 );

                //Create Rest of Non Secret Payload
                Array.Copy( salt, 0, payload, payloadIndex, salt.Length );
            }

            return SimpleEncrypt( secretMessage, cryptKey, authKey, payload );
        }
    }
}