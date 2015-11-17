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
// "Librainian/SecurityExtensions.cs" was last cleaned by Rick on 2015/06/12 at 3:12 PM

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using JetBrains.Annotations;
    using Maths;

    public static class SecurityExtensions {
        public static readonly SHA1CryptoServiceProvider CryptoProvider = new SHA1CryptoServiceProvider();
        public static readonly ThreadLocal< MD5 > Md5S = new ThreadLocal< MD5 >( MD5.Create );

        /// <summary>Provide to each thread its own <see cref="SHA256Managed" />.</summary>
        public static readonly ThreadLocal< SHA256Managed > SHA256Local = new ThreadLocal< SHA256Managed >( valueFactory: () => new SHA256Managed(), trackAllValues: false );

        /// <summary>Provide to each thread its own <see cref="SHA256Managed" />.</summary>
        public static readonly ThreadLocal< SHA384Managed > SHA384Local = new ThreadLocal< SHA384Managed >( valueFactory: () => new SHA384Managed(), trackAllValues: false );

        /// <summary>Provide to each thread its own <see cref="SHA256Managed" />.</summary>
        public static readonly ThreadLocal< SHA512Managed > SHA512Local = new ThreadLocal< SHA512Managed >( valueFactory: () => new SHA512Managed(), trackAllValues: false );

        public static String DecryptStringUsingRegistryKey([NotNull] this String decryptValue, [NotNull] String privateKey) {

            // This is the variable that will be returned to the user
            if ( decryptValue == null ) {
                throw new ArgumentNullException( nameof( decryptValue ) );
            }
            if ( privateKey == null ) {
                throw new ArgumentNullException( nameof( privateKey ) );
            }
            var decryptedValue = String.Empty;

            // Create the CspParameters object which is used to create the RSA provider without it
            // generating a new private/public key. Parameter value of 1 indicates RSA provider type
            // - 13 would indicate DSA provider
            var csp = new CspParameters( 1 ) {
                KeyContainerName = privateKey,
                ProviderName = "Microsoft Strong Cryptographic Provider"
            };

            // Registry key name containing the RSA private/public key

            // Supply the provider name

            try {

                //Create new RSA object passing our key info
                var rsa = new RSACryptoServiceProvider( csp );

                // Before decryption we must convert this ugly String into a byte array
                var valueToDecrypt = Convert.FromBase64String( decryptValue );

                // Decrypt the passed in String value - Again the false value has to do with padding
                var plainTextValue = rsa.Decrypt( valueToDecrypt, false );

                // Extract our decrypted byte array into a String value to return to our user
                decryptedValue = Encoding.UTF8.GetString( plainTextValue );
            }
            catch ( CryptographicException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return decryptedValue;
        }

        public static String EncryptStringUsingRegistryKey([NotNull] this String stringToEncrypt, [NotNull] String publicKey) {

            // This is the variable that will be returned to the user
            if ( stringToEncrypt == null ) {
                throw new ArgumentNullException( nameof( stringToEncrypt ) );
            }
            if ( publicKey == null ) {
                throw new ArgumentNullException( nameof( publicKey ) );
            }
            var encryptedValue = String.Empty;

            // Create the CspParameters object which is used to create the RSA provider without it
            // generating a new private/public key. Parameter value of 1 indicates RSA provider type
            // - 13 would indicate DSA provider
            var csp = new CspParameters( 1 ) {
                KeyContainerName = publicKey,
                ProviderName = "Microsoft Strong Cryptographic Provider"
            };

            // Registry key name containing the RSA private/public key

            // Supply the provider name

            try {

                //Create new RSA object passing our key info
                var rsa = new RSACryptoServiceProvider( csp );

                // Before encrypting the value we must convert it over to byte array
                var bytesToEncrypt = Encoding.UTF8.GetBytes( stringToEncrypt );

                // Encrypt our byte array. The false parameter has to do with padding (not to clear
                // on this point but you can look it up and decide which is better for your use)
                var bytesEncrypted = rsa.Encrypt( rgb: bytesToEncrypt, fOAEP: false );

                // Extract our encrypted byte array into a String value to return to our user
                encryptedValue = Convert.ToBase64String( bytesEncrypted );
            }
            catch ( CryptographicException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return encryptedValue;
        }

        public static Byte[] Sha256(this Byte[] input) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            return SHA256Local.Value.ComputeHash( input, 0, input.Length );
        }

        /// <summary>
        /// <para>Compute the SHA-256 hash for the <paramref name="input" /></para>
        /// <para>Defaults to <see cref="Encoding.ASCII" /></para>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Byte[] Sha256(this String input, Encoding encoding = null) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            if ( null == encoding ) {
                encoding = Encoding.ASCII;
            }
            return encoding.GetBytes( input ).Sha256();
        }

        /// <summary>
        /// <para>Compute the SHA-384 hash for the <paramref name="input" /></para>
        /// <para>Defaults to <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Byte[] Sha384(this String input, Encoding encoding = null) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes( input ).Sha384();
        }

        public static Byte[] Sha384(this Byte[] input) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            return SHA384Local.Value.ComputeHash( input, 0, input.Length );
        }

        /// <summary>
        /// <para>Compute the SHA-384 hash for the <paramref name="input" /></para>
        /// <para>Defaults to <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Byte[] Sha512(this String input, Encoding encoding = null) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes( input ).Sha512();
        }

        public static Byte[] Sha512(this Byte[] input) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            return SHA512Local.Value.ComputeHash( input, 0, input.Length );
        }

        public static String GetHash( this String s ) {
            var bt = Encoding.ASCII.GetBytes( s );
            using ( MD5 sec = new MD5CryptoServiceProvider() ) {
                return sec.ComputeHash( bt ).ToHex();
            }
        }

        public static String GetHexString( this IReadOnlyList<Byte> bt ) {
            var s = String.Empty;
            for ( var i = 0; i < bt.Count; i++ ) {
                var b = bt[ i ];
                Int32 n = b;
                var n1 = n & 15;
                var n2 = ( n >> 4 ) & 15;
                if ( n2 > 9 ) {
                    s += ( ( Char )( n2 - 10 + 'A' ) ).ToString();
                }
                else {
                    s += n2.ToString();
                }
                if ( n1 > 9 ) {
                    s += ( ( Char )( n1 - 10 + 'A' ) ).ToString();
                }
                else {
                    s += n1.ToString();
                }
                if ( ( i + 1 != bt.Count ) && ( ( i + 1 ) % 2 == 0 ) ) {
                    s += "-";
                }
            }
            return s;
        }

    }
}