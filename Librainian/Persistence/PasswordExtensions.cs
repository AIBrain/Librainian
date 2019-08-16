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
//  PayPal: Protiguous@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/PasswordExtensions.cs" was last cleaned by Rick on 2016/06/18 at 10:56 PM

namespace Librainian.Persistence {

    using System;
    using System.Configuration;
    using System.IO;
    using FileSystem;
    using NUnit.Framework;

    public static class PasswordExtensions {

        /// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean Settings( String key, String value ) => Environment.SpecialFolder.LocalApplicationData.Settings( key, value );

        /// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
        /// <param name="specialFolder"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean Settings( this Environment.SpecialFolder specialFolder, String key, String value ) {
            try {
                var configFile = ConfigurationManager.OpenExeConfiguration( specialFolder.GetStaticFile().FullPathWithFileName );
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
        public static String Settings( String key ) => Environment.SpecialFolder.LocalApplicationData.Settings( key );

        /// <summary>Return the value of the given <paramref name="key" />.</summary>
        /// <param name="specialFolder"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String Settings( this Environment.SpecialFolder specialFolder, String key ) {
            try {
                var configFile = ConfigurationManager.OpenExeConfiguration( specialFolder.GetStaticFile().FullPathWithFileName );
                return configFile.AppSettings.Settings[ key ]?.Value;
            }
            catch ( ConfigurationErrorsException exception ) {
                exception.More();
                return null;
            }
        }

        [Test]
        public static void TestStaticStorage() {
            const String phraseToTest = "Hello world";

            Settings( key: nameof( phraseToTest ), value: phraseToTest );

            Assert.AreEqual( phraseToTest, Settings( nameof( phraseToTest ) ) );
        }

        private static Document GetStaticFile( this Environment.SpecialFolder specialFolder ) {
            var path = Path.Combine( Environment.GetFolderPath( specialFolder ), nameof( Settings ) );
            if ( !Directory.Exists( path ) ) {
                Directory.CreateDirectory( path );
            }
            var destinationFile = Path.Combine( path, "StaticSettings.exe" );

            if ( File.Exists( destinationFile ) ) {
                return new Document( destinationFile );
            }

            using ( File.Create( destinationFile ) ) { }
            return new Document( destinationFile );
        }
    }
}