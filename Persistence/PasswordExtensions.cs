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
// "Librainian/PasswordExtensions.cs" was last cleaned by Rick on 2015/06/02 at 8:31 AM

namespace Librainian.Persistence {

    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using JetBrains.Annotations;
    using NUnit.Framework;

    public static class PasswordExtensions {
        private const String EntropyPhrase1 = "ZuZgBzuvvtn98vmmmt4vn4v9vwcaSjUtOmSkrA8Wo3ATOlMp3qXQmRQOdWyFFgJU";
        private const String EntropyPhrase2 = "KSOPFJyNMPgchzs7OH12MFHnGOMftm9RZwrwA1vwb66q3nqC9HtKuMzAY4fhtN8F";
        private const String EntropyPhrase3 = "XtXowrE3jz6UESvqb63bqw36nxtxTo0VYH5YJLbsxE4TR20c5nN9ocVxyabim2SX";
        private static readonly Byte[] Entropy = Encoding.Unicode.GetBytes( $"{EntropyPhrase1} {EntropyPhrase2} {EntropyPhrase3}".ToLower( CultureInfo.CurrentCulture ) );

        [NotNull]
        public static SecureString DecryptString(this String encryptedData) {
            try {
                var decryptedData = ProtectedData.Unprotect( encryptedData: Convert.FromBase64String( encryptedData ), optionalEntropy: Entropy, scope: DataProtectionScope.CurrentUser );
                return ToSecureString( Encoding.Unicode.GetString( decryptedData ) );
            }
            catch {
                return new SecureString();
            }
        }

        /// <summary>
        /// Converts the given string ( <paramref name="input" />) to an encrypted Base64 string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String EncryptString(this SecureString input) {
            var encryptedData = ProtectedData.Protect( userData: Encoding.Unicode.GetBytes( ToInsecureString( input ) ), optionalEntropy: Entropy, scope: DataProtectionScope.CurrentUser );
            return Convert.ToBase64String( encryptedData );
        }

        /// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean Settings(String key, String value) => Settings( Environment.SpecialFolder.LocalApplicationData, key, value );

        /// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
        /// <param name="specialFolder"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean Settings(Environment.SpecialFolder specialFolder, String key, String value) {
            try {
                var configFile = ConfigurationManager.OpenExeConfiguration( GetDestinationFile( specialFolder ) );
                var settings = configFile.AppSettings.Settings;
                if ( settings[ key ] == null ) {
                    settings.Add( key, value );
                }
                else {
                    settings[ key ].Value = value;
                }
                configFile.Save( ConfigurationSaveMode.Modified );
                ConfigurationManager.RefreshSection( configFile.AppSettings.SectionInformation.Name );

                return true;
            }
            catch ( ConfigurationErrorsException exception ) {
                exception.More();
            }
            return false;
        }

        /// <summary>Return the value of the given <paramref name="key" />.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String Settings(String key) => Settings( Environment.SpecialFolder.LocalApplicationData, key );

        /// <summary>Return the value of the given <paramref name="key" />.</summary>
        /// <param name="specialFolder"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String Settings(Environment.SpecialFolder specialFolder, String key) {
            try {
                var configFile = ConfigurationManager.OpenExeConfiguration( GetDestinationFile( specialFolder ) );
                var settings = configFile.AppSettings.Settings;
                return null != settings[ key ] ? settings[ key ].Value : null;
            }
            catch ( ConfigurationErrorsException exception ) {
                exception.More();
                return null;
            }
        }

        [Test]
        public static void TestEncryptionAndDecryption() {
            const String phraseToTest = "Hello world";

            var encrypted = EncryptString( phraseToTest.ToSecureString() );
            var decrypted = DecryptString( encrypted ).ToInsecureString();

            Assert.AreEqual( decrypted, phraseToTest );
        }

        [Test]
        public static void TestStaticStorage() {
            const String phraseToTest = "Hello world";

            Settings( nameof( phraseToTest ), phraseToTest );

            Assert.AreEqual( phraseToTest, Settings( nameof( phraseToTest ) ) );
        }

        public static String ToInsecureString(this SecureString input) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }

            String returnValue;
            var ptr = Marshal.SecureStringToBSTR( input );
            try {
                returnValue = Marshal.PtrToStringBSTR( ptr );
            }
            finally {
                Marshal.ZeroFreeBSTR( ptr );
            }
            return returnValue;
        }

        public static SecureString ToSecureString(this String input) {
            if ( input == null ) {
                throw new ArgumentNullException( nameof( input ) );
            }
            var secure = new SecureString();
            foreach ( var c in input ) {
                secure.AppendChar( c );
            }
            secure.MakeReadOnly();
            return secure;
        }

        private static String GetDestinationFile(Environment.SpecialFolder specialFolder) {
            var path = Path.Combine( Environment.GetFolderPath( specialFolder ), nameof( Settings ) );
            if ( !Directory.Exists( path ) ) {
                Directory.CreateDirectory( path );
            }
            var destinationFile = Path.Combine( path, "StaticGlobalSettings.exe" );

            // ReSharper disable once InvertIf
            if ( !File.Exists( destinationFile ) ) {
                using ( var file = File.Create( destinationFile ) ) {
                    file.Close();
                }
                return destinationFile;
            }
            return destinationFile;
        }
    }
}