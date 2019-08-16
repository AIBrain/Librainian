// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "AESThenHMAC.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "AESThenHMAC.cs" was last formatted by Protiguous on 2019/08/08 at 9:31 AM.

namespace Librainian.Security {

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using JetBrains.Annotations;
    using Maths;

    /// <summary>
    ///     This work (Modern Encryption of a String C#, by James Tuley), identified by James Tuley, is
    ///     free of known copyright restrictions.
    ///     https: //gist.github.com/4336842
    ///     http: //creativecommons.org/publicdomain/mark/1.0/
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
        /// <returns></returns>
        [NotNull]
        public static Byte[] NewKey() {
            var key = new Byte[ KeyBitSize / 8 ];
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
        [CanBeNull]
        public static Byte[] SimpleDecrypt( [NotNull] Byte[] encryptedMessage, [NotNull] Byte[] cryptKey, [NotNull] Byte[] authKey, Int32 nonSecretPayloadLength = 0 ) {

            //Basic Usage Error Checks
            if ( cryptKey == null || cryptKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( $"CryptKey needs to be {KeyBitSize} bit!", nameof( cryptKey ) );
            }

            if ( authKey == null || authKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( $"AuthKey needs to be {KeyBitSize} bit!", nameof( authKey ) );
            }

            if ( encryptedMessage == null || encryptedMessage.Length == 0 ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            using ( var hmac = new HMACSHA256( authKey ) ) {
                var sentTag = new Byte[ hmac.HashSize / 8 ];

                //Calculate Tag
                var calcTag = hmac.ComputeHash( buffer: encryptedMessage, offset: 0, count: encryptedMessage.Length - sentTag.Length );
                const Int32 ivLength = BlockBitSize / 8;

                //if message length is to small just return null
                if ( encryptedMessage.Length < sentTag.Length + nonSecretPayloadLength + ivLength ) {
                    return null;
                }

                //Grab Sent Tag
                Array.Copy( sourceArray: encryptedMessage, sourceIndex: encryptedMessage.Length - sentTag.Length, destinationArray: sentTag, destinationIndex: 0,
                    sentTag.Length );

                //Compare Tag with constant time comparison
                var compare = 0;

                for ( var i = 0; i < sentTag.Length; i++ ) {
                    compare |= sentTag[ i ] ^ calcTag[ i ];
                }

                //if message doesn't authenticate return null
                if ( compare != 0 ) {
                    return null;
                }

                using ( var aes = new AesManaged {
                    KeySize = KeyBitSize, BlockSize = BlockBitSize, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7
                } ) {

                    //Grab IV from message
                    var iv = new Byte[ ivLength ];
                    Array.Copy( sourceArray: encryptedMessage, sourceIndex: nonSecretPayloadLength, destinationArray: iv, destinationIndex: 0, iv.Length );

                    using ( var decrypter = aes.CreateDecryptor( cryptKey, iv: iv ) ) {
                        var plainTextStream = new MemoryStream();
                        var decrypterStream = new CryptoStream( stream: plainTextStream, transform: decrypter, mode: CryptoStreamMode.Write );

                        using ( var binaryWriter = new BinaryWriter( output: decrypterStream ) ) {

                            //Decrypt Cipher Text from Message
                            binaryWriter.Write( buffer: encryptedMessage, index: nonSecretPayloadLength + iv.Length,
                                count: encryptedMessage.Length - nonSecretPayloadLength - iv.Length - sentTag.Length );
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
        [CanBeNull]
        public static String SimpleDecrypt( [NotNull] String encryptedMessage, [NotNull] Byte[] cryptKey, [NotNull] Byte[] authKey, Int32 nonSecretPayloadLength = 0 ) {
            if ( String.IsNullOrWhiteSpace( encryptedMessage ) ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            var cipherText = Convert.FromBase64String( s: encryptedMessage );
            var plainText = SimpleDecrypt( encryptedMessage: cipherText, cryptKey: cryptKey, authKey: authKey, nonSecretPayloadLength: nonSecretPayloadLength );

            return plainText == null ? null : Encoding.UTF8.GetString( bytes: plainText );
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
        [CanBeNull]
        public static String SimpleDecryptWithPassword( [NotNull] String encryptedMessage, [NotNull] String password, Int32 nonSecretPayloadLength = 0 ) {
            if ( String.IsNullOrWhiteSpace( encryptedMessage ) ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            if ( String.IsNullOrEmpty( value: password ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( password ) );
            }

            if ( !nonSecretPayloadLength.Any() ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( nonSecretPayloadLength ) );
            }

            var cipherText = Convert.FromBase64String( s: encryptedMessage );
            var plainText = SimpleDecryptWithPassword( encryptedMessage: cipherText, password: password, nonSecretPayloadLength: nonSecretPayloadLength );

            return plainText == null ? null : Encoding.UTF8.GetString( bytes: plainText );
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
        [CanBeNull]
        public static Byte[] SimpleDecryptWithPassword( [NotNull] Byte[] encryptedMessage, [NotNull] String password, Int32 nonSecretPayloadLength = 0 ) {

            //User Error Checks
            if ( String.IsNullOrWhiteSpace( password ) || password.Length < MinPasswordLength ) {
                throw new ArgumentException( $"Must have a password of at least {MinPasswordLength} characters!", nameof( password ) );
            }

            if ( encryptedMessage == null || encryptedMessage.Length == 0 ) {
                throw new ArgumentException( "Encrypted Message Required!", nameof( encryptedMessage ) );
            }

            var cryptSalt = new Byte[ SaltBitSize / 8 ];
            var authSalt = new Byte[ SaltBitSize / 8 ];

            //Grab Salt from Non-Secret Payload
            Array.Copy( sourceArray: encryptedMessage, sourceIndex: nonSecretPayloadLength, destinationArray: cryptSalt, destinationIndex: 0, cryptSalt.Length );

            Array.Copy( sourceArray: encryptedMessage, sourceIndex: nonSecretPayloadLength + cryptSalt.Length, destinationArray: authSalt, destinationIndex: 0,
                authSalt.Length );

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

            return SimpleDecrypt( encryptedMessage: encryptedMessage, cryptKey: cryptKey, authKey: authKey,
                nonSecretPayloadLength: cryptSalt.Length + authSalt.Length + nonSecretPayloadLength );
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
        [NotNull]
        public static Byte[] SimpleEncrypt( [NotNull] this Byte[] secretMessage, [NotNull] Byte[] cryptKey, [NotNull] Byte[] authKey, Byte[] nonSecretPayload = null ) {

            //User Error Checks
            if ( cryptKey == null || cryptKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( $"Key needs to be {KeyBitSize} bit!", nameof( cryptKey ) );
            }

            if ( authKey == null || authKey.Length != KeyBitSize / 8 ) {
                throw new ArgumentException( $"Key needs to be {KeyBitSize} bit!", nameof( authKey ) );
            }

            if ( secretMessage == null || secretMessage.Length < 1 ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
            }

            //non-secret payload optional
            nonSecretPayload = nonSecretPayload ?? new Byte[] { };

            Byte[] cipherText;
            Byte[] iv;

            using ( var aes = new AesManaged {
                KeySize = KeyBitSize, BlockSize = BlockBitSize, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7
            } ) {

                //Use random IV
                aes.GenerateIV();
                iv = aes.IV;

                using ( var encrypter = aes.CreateEncryptor( cryptKey, iv: iv ) ) {
                    var cipherStream = new MemoryStream();

                    using ( var binaryWriter = new BinaryWriter( output: new CryptoStream( stream: cipherStream, transform: encrypter, mode: CryptoStreamMode.Write ) ) ) {
                        binaryWriter.Write( buffer: secretMessage );
                    }

                    cipherText = cipherStream.ToArray();
                }
            }

            //Assemble encrypted message and add authentication
            using ( var hmac = new HMACSHA256( authKey ) ) {
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
        [NotNull]
        public static String SimpleEncrypt( [NotNull] String secretMessage, [NotNull] Byte[] cryptKey, [NotNull] Byte[] authKey, [CanBeNull] Byte[] nonSecretPayload = null ) {
            if ( String.IsNullOrEmpty( secretMessage ) ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
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
        [NotNull]
        public static String SimpleEncryptWithPassword( [NotNull] String secretMessage, [NotNull] String password, [CanBeNull] Byte[] nonSecretPayload = null ) {
            if ( String.IsNullOrEmpty( secretMessage ) ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
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
        [NotNull]
        public static Byte[] SimpleEncryptWithPassword( [NotNull] Byte[] secretMessage, [NotNull] String password, Byte[] nonSecretPayload = null ) {
            nonSecretPayload = nonSecretPayload ?? new Byte[] { };

            //User Error Checks
            if ( String.IsNullOrWhiteSpace( password ) || password.Length < MinPasswordLength ) {
                throw new ArgumentException( $"Must have a password of at least {MinPasswordLength} characters!", nameof( password ) );
            }

            if ( secretMessage == null || secretMessage.Length == 0 ) {
                throw new ArgumentException( "Secret Message Required!", nameof( secretMessage ) );
            }

            var payload = new Byte[ (SaltBitSize / 8 * 2) + nonSecretPayload.Length ];

            Array.Copy( sourceArray: nonSecretPayload, destinationArray: payload, nonSecretPayload.Length );
            var payloadIndex = nonSecretPayload.Length;

            Byte[] cryptKey;
            Byte[] authKey;

            //Use Random Salt to prevent pre-generated weak password attacks.
            using ( var generator = new Rfc2898DeriveBytes( password: password, saltSize: SaltBitSize / 8, iterations: Iterations ) ) {
                var salt = generator.Salt;

                //Generate Keys
                cryptKey = generator.GetBytes( cb: KeyBitSize / 8 );

                //Create Non Secret Payload
                Array.Copy( sourceArray: salt, sourceIndex: 0, destinationArray: payload, destinationIndex: payloadIndex, salt.Length );
                payloadIndex += salt.Length;
            }

            //Deriving separate key, might be less efficient than using HKDF,
            //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
            using ( var generator = new Rfc2898DeriveBytes( password: password, saltSize: SaltBitSize / 8, iterations: Iterations ) ) {
                var salt = generator.Salt;

                //Generate Keys
                authKey = generator.GetBytes( cb: KeyBitSize / 8 );

                //Create Rest of Non Secret Payload
                Array.Copy( sourceArray: salt, sourceIndex: 0, destinationArray: payload, destinationIndex: payloadIndex, salt.Length );
            }

            return secretMessage.SimpleEncrypt( cryptKey, authKey, payload );
        }
    }
}