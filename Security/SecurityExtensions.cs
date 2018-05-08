// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
//
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/SecurityExtensions.cs" was last cleaned by Protiguous on 2018/05/07 at 11:20 PM

namespace Librainian.Security {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FileSystem;
    using JetBrains.Annotations;
    using Maths;
    using Mono.Math;
    using NUnit.Framework;

    public static class SecurityExtensions {

        /// <summary>
        /// Not very secure. Oh well.
        /// </summary>
        public const String EntropyPhrase1 = "ZuZgBzuvvtn98vmmmt4vn4v9vwcaSjUtOmSkrA8Wo3ATOlMp3qXQmRQOdWyFFgJU";

        public const String EntropyPhrase2 = "KSOPFJyNMPgchzs7OH12MFHnGOMftm9RZwrwA1vwb66q3nqC9HtKuMzAY4fhtN8F";
        public const String EntropyPhrase3 = "XtXowrE3jz6UESvqb63bqw36nxtxTo0VYH5YJLbsxE4TR20c5nN9ocVxyabim2SX";

        /// <summary>
        /// </summary>
        public static SHA1CryptoServiceProvider CryptoProvider { get; } = new SHA1CryptoServiceProvider();

        public static Byte[] Entropy { get; } = Encoding.UTF32.GetBytes( s: $"{EntropyPhrase1} {EntropyPhrase2} {EntropyPhrase3}" );

        public static ThreadLocal<MD5> Md5S { get; } = new ThreadLocal<MD5>( valueFactory: MD5.Create );

        /// <summary>
        /// Provide to each thread its own <see cref="SHA256Managed"/>.
        /// </summary>
        public static ThreadLocal<SHA256Managed> SHA256Local { get; } = new ThreadLocal<SHA256Managed>( valueFactory: () => new SHA256Managed(), trackAllValues: false );

        /// <summary>
        /// Provide to each thread its own <see cref="SHA256Managed"/>.
        /// </summary>
        public static ThreadLocal<SHA384Managed> SHA384Local { get; } = new ThreadLocal<SHA384Managed>( valueFactory: () => new SHA384Managed(), trackAllValues: false );

        /// <summary>
        /// Provide to each thread its own <see cref="SHA256Managed"/>.
        /// </summary>
        public static ThreadLocal<SHA512Managed> SHA512Local { get; } = new ThreadLocal<SHA512Managed>( valueFactory: () => new SHA512Managed(), trackAllValues: false );

        private static Byte[] Uid( String s ) {
            var numArray = new Byte[s.Length];
            for ( var i = 0; i < s.Length; i++ ) {
                numArray[i] = ( Byte )( s[index: i] & '\u007F' );
            }

            return numArray;
        }

        public static Task<Byte[]> ComputeMD5Hash( String filename ) =>
            Task.Run( function: () => {
                var md5Hasher = Md5S.Value;

                using ( var fs = new FileStream( path: filename, mode: FileMode.Open, access: FileAccess.Read, share: FileShare.Read ) ) {
                    return md5Hasher.ComputeHash( inputStream: fs );
                }
            } );

        [NotNull]
        public static SecureString DecryptString( this String encryptedData ) {
            try {
                var decryptedData = ProtectedData.Unprotect( encryptedData: Convert.FromBase64String( s: encryptedData ), optionalEntropy: Entropy, scope: DataProtectionScope.CurrentUser );
                return ToSecureString( input: Encoding.Unicode.GetString( bytes: decryptedData ) );
            }
            catch {
                return new SecureString();
            }
        }

        public static String DecryptStringUsingRegistryKey( [NotNull] this String decryptValue, [NotNull] String privateKey ) {

            // This is the variable that will be returned to the user
            if ( decryptValue is null ) {
                throw new ArgumentNullException( paramName: nameof( decryptValue ) );
            }

            if ( privateKey is null ) {
                throw new ArgumentNullException( paramName: nameof( privateKey ) );
            }

            var decryptedValue = String.Empty;

            // Create the CspParameters object which is used to create the RSA provider without it generating a new private/public key. Parameter value of 1 indicates RSA provider type
            // - 13 would indicate DSA provider
            var csp = new CspParameters( dwTypeIn: 1 ) { KeyContainerName = privateKey, ProviderName = "Microsoft Strong Cryptographic Provider" };

            // Registry key name containing the RSA private/public key

            // Supply the provider name

            try {

                //Create new RSA object passing our key info
                var rsa = new RSACryptoServiceProvider( parameters: csp );

                // Before decryption we must convert this ugly String into a byte array
                var valueToDecrypt = Convert.FromBase64String( s: decryptValue );

                // Decrypt the passed in String value - Again the false value has to do with padding
                var plainTextValue = rsa.Decrypt( rgb: valueToDecrypt, fOAEP: false );

                // Extract our decrypted byte array into a String value to return to our user
                decryptedValue = Encoding.UTF8.GetString( bytes: plainTextValue );
            }
            catch ( CryptographicException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return decryptedValue;
        }

        /// <summary>
        /// Converts the given string ( <paramref name="input"/>) to an encrypted Base64 string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String EncryptString( this SecureString input ) {
            var encryptedData = ProtectedData.Protect( userData: Encoding.Unicode.GetBytes( s: ToInsecureString( input: input ) ), optionalEntropy: Entropy, scope: DataProtectionScope.CurrentUser );
            return Convert.ToBase64String( inArray: encryptedData );
        }

        public static String EncryptStringUsingRegistryKey( [NotNull] this String stringToEncrypt, [NotNull] String publicKey ) {

            // This is the variable that will be returned to the user
            if ( stringToEncrypt is null ) {
                throw new ArgumentNullException( paramName: nameof( stringToEncrypt ) );
            }

            if ( publicKey is null ) {
                throw new ArgumentNullException( paramName: nameof( publicKey ) );
            }

            var encryptedValue = String.Empty;

            // Create the CspParameters object which is used to create the RSA provider without it generating a new private/public key. Parameter value of 1 indicates RSA provider type
            // - 13 would indicate DSA provider
            var csp = new CspParameters( dwTypeIn: 1 ) { KeyContainerName = publicKey, ProviderName = "Microsoft Strong Cryptographic Provider" };

            // Registry key name containing the RSA private/public key

            // Supply the provider name

            try {

                //Create new RSA object passing our key info
                var rsa = new RSACryptoServiceProvider( parameters: csp );

                // Before encrypting the value we must convert it over to byte array
                var bytesToEncrypt = Encoding.UTF8.GetBytes( s: stringToEncrypt );

                // Encrypt our byte array. The false parameter has to do with padding (not to clear on this point but you can look it up and decide which is better for your use)
                var bytesEncrypted = rsa.Encrypt( rgb: bytesToEncrypt, fOAEP: false );

                // Extract our encrypted byte array into a String value to return to our user
                encryptedValue = Convert.ToBase64String( inArray: bytesEncrypted );
            }
            catch ( CryptographicException exception ) {
                exception.More();
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return encryptedValue;
        }

        public static String GenerateKey( String username, Decimal version = 9.2M ) {
            username = ( username ?? String.Empty ).Trim();
            if ( String.IsNullOrEmpty( value: username ) ) {
                return String.Empty;
            }

            var value = ( Int32 )version;
            var num = ( Int32 )( ( version - value ) * new Decimal( value: 10 ) );
            var num1 = username.Aggregate( seed: 0, func: ( current, t ) => ( ( current << 7 ) + t ) % 0xfff1 );

            var bigInteger = new BigInteger( inData: Uid( s: username ) );
            bigInteger.SetBit( bitNum: 0 );

            var integer1 = BigInteger.Parse( number: "5675452544727795816938431027316696995782983680" );
            integer1 += new BigInteger( bi: value * 0x3e8 + num ) << 72;
            integer1 += new BigInteger( bi: num1 );

            var integer2 = BigInteger.Parse( number: "3483968730802868401158985191529641621586542542912639916793" );
            var modulus = BigInteger.Parse( number: "3483968730802868401158985191409366916534934371594468194468" );
            integer1 = integer1.ModPow( exp: bigInteger.ModInverse( modulus: modulus ), n: integer2 );
            return $"{1}-{Convert.ToBase64String( inArray: integer1.GetBytes() )}";
        }

        public static String GetHash( this String s ) {
            var bt = Encoding.ASCII.GetBytes( s: s );
            using ( MD5 sec = new MD5CryptoServiceProvider() ) {
                return sec.ComputeHash( buffer: bt ).ToHex();
            }
        }

        public static String GetHexString( this IReadOnlyList<Byte> bt ) {
            var s = String.Empty;
            for ( var i = 0; i < bt.Count; i++ ) {
                var b = bt[index: i];
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

                if ( i + 1 != bt.Count && ( i + 1 ) % 2 == 0 ) {
                    s += "-";
                }
            }

            return s;
        }

        /// <summary>
        /// Uses the md5sum.exe to obtain the md5 string.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [CanBeNull]
        public static String Md5( this FileInfo file ) {
            if ( !file.Exists ) {
                return null;
            }

            var p = new Process { StartInfo = { FileName = "md5sum.exe", Arguments = file.FullName, UseShellExecute = false, RedirectStandardOutput = true } };
            p.Start();
            p.WaitForExit();
            var output = p.StandardOutput.ReadToEnd();
            var result = output.Split( ' ' )[0].Substring( startIndex: 1 ).ToUpper();
            return String.IsNullOrWhiteSpace( value: result ) ? null : result;
        }

        public static Byte[] Sha256( this Byte[] input ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            return SHA256Local.Value.ComputeHash( buffer: input, offset: 0, count: input.Length );
        }

        /// <summary>
        /// <para>Compute the SHA-256 hash for the <paramref name="input"/></para>
        /// <para>Defaults to <see cref="Encoding.UTF8"/></para>
        /// </summary>
        /// <param name="input">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Byte[] Sha256( this String input, Encoding encoding = null ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }

            return encoding.GetBytes( s: input ).Sha256();
        }

        /// <summary>
        /// <para>Compute the SHA-384 hash for the <paramref name="input"/></para>
        /// <para>Defaults to <see cref="Encoding.UTF8"/></para>
        /// </summary>
        /// <param name="input">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Byte[] Sha384( this String input, Encoding encoding = null ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }

            return encoding.GetBytes( s: input ).Sha384();
        }

        public static Byte[] Sha384( this Byte[] input ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            return SHA384Local.Value.ComputeHash( buffer: input, offset: 0, count: input.Length );
        }

        /// <summary>
        /// <para>Compute the SHA-384 hash for the <paramref name="input"/></para>
        /// <para>Defaults to <see cref="Encoding.UTF8"/></para>
        /// </summary>
        /// <param name="input">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Byte[] Sha512( this String input, Encoding encoding = null ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            if ( null == encoding ) {
                encoding = Encoding.UTF8;
            }

            return encoding.GetBytes( s: input ).Sha512();
        }

        public static Byte[] Sha512( this Byte[] input ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            return SHA512Local.Value.ComputeHash( buffer: input, offset: 0, count: input.Length );
        }

        public static String ToInsecureString( this SecureString input ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            String returnValue;
            var ptr = Marshal.SecureStringToBSTR( s: input );
            try {
                returnValue = Marshal.PtrToStringBSTR( ptr: ptr );
            }
            finally {
                Marshal.ZeroFreeBSTR( s: ptr );
            }

            return returnValue;
        }

        public static SecureString ToSecureString( this String input ) {
            if ( input is null ) {
                throw new ArgumentNullException( paramName: nameof( input ) );
            }

            var secure = new SecureString();
            foreach ( var c in input ) {
                secure.AppendChar( c: c );
            }

            secure.MakeReadOnly();
            return secure;
        }

        public static Boolean TryComputeMd5ForFile( [CanBeNull] this Document document, [CanBeNull] out String md5 ) {
            md5 = null;
            try {
                if ( document is null || !File.Exists( path: "md5sum.exe" ) || !document.Exists() ) {
                    return false;
                }

                var p = new Process { StartInfo = { FileName = "md5sum.exe", Arguments = $"\"{document.FullPathWithFileName}\"", UseShellExecute = false, RedirectStandardOutput = true } };
                p.Start();
                p.WaitForExit();
                var output = p.StandardOutput.ReadToEnd();
                md5 = output.Split( ' ' )[0].Substring( startIndex: 1 ).ToUpper();
                return !String.IsNullOrWhiteSpace( value: md5 ) && md5.Length == 32;
            }
            catch ( Exception exception ) {
                exception.More();
            }

            return false;
        }

        /// <summary>
        /// Attempt to decrypt an encrypted version of the file with the given key and salt.
        /// </summary>
        /// <param name="input">            </param>
        /// <param name="output">           </param>
        /// <param name="key">              Must be between 1 and 32767 bytes.</param>
        /// <param name="reportProgress">   </param>
        /// <param name="exceptions">       List of exceptions encountered.</param>
        /// <param name="salt">             </param>
        /// <param name="reportEveryXBytes"></param>
        /// <returns>Returns true if all is successful</returns>
        public static Boolean TryDecryptFile( [CanBeNull] this Document input, [CanBeNull] Document output, [CanBeNull] String key, Int32 salt, UInt64? reportEveryXBytes, Action<Single> reportProgress,
            [NotNull] out List<Exception> exceptions ) {
            exceptions = new List<Exception>( capacity: 1 );

            if ( input is null ) {
                exceptions.Add( item: new ArgumentNullException( paramName: nameof( input ) ) );
                return false;
            }

            if ( !input.Exists() ) {
                exceptions.Add( item: new FileNotFoundException( message: $"The input file {input.FullPathWithFileName} is not found." ) );
                return false;
            }

            var inputFileSize = ( Single? )input.Size();
            if ( !inputFileSize.HasValue || inputFileSize.Value <= 0 ) {
                exceptions.Add( item: new FileNotFoundException( message: $"The input file {input.FullPathWithFileName} is empty." ) );
                return false;
            }

            if ( output is null ) {
                exceptions.Add( item: new ArgumentNullException( paramName: nameof( output ) ) );
                return false;
            }

            if ( output.Exists() ) {
                exceptions.Add( item: new IOException( message: $"The output file {output.FullPathWithFileName} already exists." ) );
                return false;
            }

            if ( key is null ) {
                exceptions.Add( item: new ArgumentNullException( paramName: nameof( key ) ) );
                return false;
            }

            if ( !key.Length.Between( startInclusive: 1, endInclusive: Int16.MaxValue ) ) {
                exceptions.Add( item: new ArgumentOutOfRangeException( paramName: nameof( key ) ) );
                return false;
            }

            try {
                if ( !output.Folder.Create() ) {
                    exceptions.Add( item: new IOException( message: $"Unable to write to {output.FullPathWithFileName} because folder {output.Folder.FullName} does not exist." ) );
                    return false;
                }

                using ( var aes = new AesCryptoServiceProvider() ) {
                    DeriveBytes rgb = new Rfc2898DeriveBytes( password: key, salt: Encoding.Unicode.GetBytes( s: salt.ToString() ) );

                    aes.BlockSize = 128;
                    aes.KeySize = 256;
                    aes.Key = rgb.GetBytes( cb: aes.KeySize >> 3 );
                    aes.IV = rgb.GetBytes( cb: aes.BlockSize >> 3 );
                    aes.Mode = CipherMode.CBC;

                    using ( var outputStream = new FileStream( path: output.FullPathWithFileName, mode: FileMode.Create, access: FileAccess.Write ) ) {
                        using ( var decryptor = aes.CreateDecryptor() ) {
                            var inputStream = new FileStream( path: input.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read );
                            using ( var cs = new CryptoStream( stream: inputStream, transform: decryptor, mode: CryptoStreamMode.Read ) ) {
                                Int32 data;
                                while ( ( data = cs.ReadByte() ) != -1 ) {
                                    if ( null != reportEveryXBytes && null != reportProgress ) {
                                        var position = ( UInt64 )inputStream.Position;
                                        if ( position % reportEveryXBytes.Value == 0 ) {
                                            var progress = position / inputFileSize;
                                            reportProgress( progress.Value );
                                        }
                                    }

                                    outputStream.WriteByte( value: ( Byte )data );
                                }
                            }
                        }
                    }
                }

                return output.Exists();
            }
            catch ( AggregateException exceptionss ) {
                exceptions.AddRange( collection: exceptionss.InnerExceptions );
                return false;
            }
            catch ( Exception exception ) {
                exceptions.Add( item: exception );
                return false;
            }
        }

        /// <summary>
        /// Create an encrypted version of the given file with the given key and salt.
        /// </summary>
        /// <param name="input">            </param>
        /// <param name="output">           </param>
        /// <param name="key">              Must be between 1 and 32767 bytes.</param>
        /// <param name="salt">             </param>
        /// <param name="reportEveryXBytes"></param>
        /// <param name="reportProgress">   Reports progress every X bytes</param>
        /// <param name="exceptions">       List of exceptions encountered.</param>
        /// <returns>Returns true if all is successful</returns>
        public static Boolean TryEncryptFile( [CanBeNull] this Document input, [CanBeNull] Document output, [CanBeNull] String key, Int32 salt, UInt64? reportEveryXBytes, Action<Single> reportProgress,
            [NotNull] out List<Exception> exceptions ) {
            exceptions = new List<Exception>( capacity: 1 );

            if ( input is null ) {
                exceptions.Add( item: new ArgumentNullException( paramName: nameof( input ) ) );
                return false;
            }

            if ( !input.Exists() ) {
                exceptions.Add( item: new FileNotFoundException( message: $"The input file {input.FullPathWithFileName} is not found." ) );
                return false;
            }

            var inputFileSize = ( Single? )input.Size();
            if ( !inputFileSize.HasValue || inputFileSize.Value <= 0 ) {
                exceptions.Add( item: new FileNotFoundException( message: $"The input file {input.FullPathWithFileName} is empty." ) );
                return false;
            }

            if ( output is null ) {
                exceptions.Add( item: new ArgumentNullException( paramName: nameof( output ) ) );
                return false;
            }

            if ( output.Exists() ) {
                exceptions.Add( item: new IOException( message: $"The output file {output.FullPathWithFileName} already exists." ) );
                return false;
            }

            if ( key is null ) {
                exceptions.Add( item: new ArgumentNullException( paramName: nameof( key ) ) );
                return false;
            }

            if ( !key.Length.Between( startInclusive: 1, endInclusive: Int16.MaxValue ) ) {
                exceptions.Add( item: new ArgumentOutOfRangeException( paramName: nameof( key ) ) );
                return false;
            }

            try {
                var rgb = new Rfc2898DeriveBytes( password: key, salt: Encoding.Unicode.GetBytes( s: salt.ToString() ) );

                if ( !output.Folder.Create() ) {
                    exceptions.Add( item: new IOException( message: $"Unable to write to {output.FullPathWithFileName} because folder {output.Folder.FullName} does not exist." ) );
                    return false;
                }

                using ( var aes = new AesCryptoServiceProvider() ) {
                    aes.BlockSize = 128;
                    aes.KeySize = 256;
                    aes.Key = rgb.GetBytes( cb: aes.KeySize >> 3 );
                    aes.IV = rgb.GetBytes( cb: aes.BlockSize >> 3 );
                    aes.Mode = CipherMode.CBC;

                    var outputStream = new FileStream( path: output.FullPathWithFileName, mode: FileMode.Create, access: FileAccess.Write );
                    if ( !outputStream.CanWrite ) {
                        exceptions.Add( item: new IOException( message: $"Unable to write to {output.FullPathWithFileName}." ) );
                        return false;
                    }

                    using ( var encryptor = aes.CreateEncryptor() ) {
                        using ( var cryptoStream = new CryptoStream( stream: outputStream, transform: encryptor, mode: CryptoStreamMode.Write ) ) {
                            using ( var inputStream = new FileStream( path: input.FullPathWithFileName, mode: FileMode.Open, access: FileAccess.Read ) ) {
                                if ( !inputStream.CanRead || !inputStream.CanSeek ) {
                                    exceptions.Add( item: new IOException( message: $"Unable to read from {input.FullPathWithFileName}." ) );
                                    return false;
                                }

                                inputStream.Seek( offset: 0, origin: SeekOrigin.Begin );
                                Int32 data;

                                //TODO put a 64k buffer here instead of byte-by-byte
                                while ( ( data = inputStream.ReadByte() ) != -1 ) {
                                    if ( null != reportEveryXBytes && null != reportProgress ) {
                                        var position = ( UInt64 )inputStream.Position;
                                        if ( position % reportEveryXBytes.Value == 0 ) {
                                            var progress = position / inputFileSize;
                                            reportProgress( progress.Value );
                                        }
                                    }

                                    cryptoStream.WriteByte( value: ( Byte )data );
                                }
                            }
                        }
                    }
                }

                return output.Exists();
            }
            catch ( AggregateException exceptionss ) {
                exceptions.AddRange( collection: exceptionss.InnerExceptions );
                return false;
            }
            catch ( Exception exception ) {
                exceptions.Add( item: exception );
                return false;
            }
        }
    }

    [TestFixture]
    public static class SecurityTests {

        [Test]
        public static void TestEncryptionAndDecryption() {
            const String phraseToTest = "Hello world";

            var encrypted = phraseToTest.ToSecureString().EncryptString();
            var decrypted = encrypted.DecryptString().ToInsecureString();

            Assert.AreEqual( expected: decrypted, actual: phraseToTest );
        }

        [Test]
        public static void TestGenerateGenerates() {
            var result = SecurityExtensions.GenerateKey( username: "Test" );
            Console.WriteLine( value: result );

            // 1-KbQP3bo4zph3ynO0flLTxzB8d25AY74E
        }
    }
}