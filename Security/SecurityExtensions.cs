#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SecurityExtensions.cs" was last cleaned by Rick on 2014/08/12 at 10:05 AM

#endregion License & Information

namespace Librainian.Security {

    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using Annotations;
    using Threading;

    public static class SecurityExtensions {
        public static readonly ThreadLocal<MD5> Md5S = new ThreadLocal<MD5>( MD5.Create );

        /// <summary>
        /// Provide to each thread its own <see cref="SHA256Managed" />.
        /// </summary>
        public static readonly ThreadLocal<SHA256Managed> SHA256Local = new ThreadLocal<SHA256Managed>( valueFactory: () => new SHA256Managed(), trackAllValues: false );

        /// <summary>
        /// Provide to each thread its own <see cref="SHA256Managed" />.
        /// </summary>
        public static readonly ThreadLocal<SHA384Managed> SHA384Local = new ThreadLocal<SHA384Managed>( valueFactory: () => new SHA384Managed(), trackAllValues: false );

        /// <summary>
        /// Provide to each thread its own <see cref="SHA256Managed" />.
        /// </summary>
        public static readonly ThreadLocal<SHA512Managed> SHA512Local = new ThreadLocal<SHA512Managed>( valueFactory: () => new SHA512Managed(), trackAllValues: false );

        public static string DecryptStringUsingRegistryKey( [NotNull] this string decryptValue, [NotNull] string privateKey ) {

            // This is the variable that will be returned to the user
            if ( decryptValue == null ) {
                throw new ArgumentNullException( "decryptValue" );
            }
            if ( privateKey == null ) {
                throw new ArgumentNullException( "privateKey" );
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

                // Before decryption we must convert this ugly string into a byte array
                var valueToDecrypt = Convert.FromBase64String( decryptValue );

                // Decrypt the passed in string value - Again the false value has to do with padding
                var plainTextValue = rsa.Decrypt( valueToDecrypt, false );

                // Extract our decrypted byte array into a string value to return to our user
                decryptedValue = Encoding.UTF8.GetString( plainTextValue );
            }
            catch ( CryptographicException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }

            return decryptedValue;
        }

        public static string EncryptStringUsingRegistryKey( [NotNull] this string stringToEncrypt, [NotNull] string publicKey ) {

            // This is the variable that will be returned to the user
            if ( stringToEncrypt == null ) {
                throw new ArgumentNullException( "stringToEncrypt" );
            }
            if ( publicKey == null ) {
                throw new ArgumentNullException( "publicKey" );
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

                // Extract our encrypted byte array into a string value to return to our user
                encryptedValue = Convert.ToBase64String( bytesEncrypted );
            }
            catch ( CryptographicException exception ) {
                exception.Error();
            }
            catch ( Exception exception ) {
                exception.Error();
            }

            return encryptedValue;
        }

        public static byte[] Sha256( this byte[] input ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            return SHA256Local.Value.ComputeHash( input, 0, input.Length );
        }

        /// <summary>
        /// <para>Compute the SHA-256 hash for the <paramref name="input" /></para><para>Defaults to
        /// <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Sha256( this String input, Encoding encoding = null ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes( input ).Sha256();
        }

        /// <summary>
        /// <para>Compute the SHA-384 hash for the <paramref name="input" /></para><para>Defaults to
        /// <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Sha384( this String input, Encoding encoding = null ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes( input ).Sha384();
        }

        public static byte[] Sha384( this byte[] input ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            return SHA384Local.Value.ComputeHash( input, 0, input.Length );
        }

        /// <summary>
        /// <para>Compute the SHA-384 hash for the <paramref name="input" /></para><para>Defaults to
        /// <see cref="Encoding.UTF8" /></para>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] Sha512( this String input, Encoding encoding = null ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes( input ).Sha512();
        }

        public static byte[] Sha512( this byte[] input ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            return SHA512Local.Value.ComputeHash( input, 0, input.Length );
        }
    }
}