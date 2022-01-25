// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "PasswordExtensions.cs" last formatted on 2022-12-22 at 5:20 PM by Protiguous.

namespace Librainian.Persistence;

//using System;
//using System.Configuration;
//using System.IO;
//using OperatingSystem.FileSystem;

//public static class PasswordExtensions {
//	/// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
//	/// <param name="key"></param>
//	/// <param name="value"></param>
//	/// <returns></returns>
//	public static Boolean Settings( String key, String value ) => Environment.SpecialFolder.LocalApplicationData.Settings( key, value );

//	/// <summary>Set a static <paramref name="key" /> to the <paramref name="value" />.</summary>
//	/// <param name="specialFolder"></param>
//	/// <param name="key"></param>
//	/// <param name="value"></param>
//	/// <returns></returns>
//	public static Boolean Settings( this Environment.SpecialFolder specialFolder, String key, String value ) {
//		try {
//			var configFile = ConfigurationManager.OpenExeConfiguration( specialFolder.GetStaticFile().FullPath );
//			var settings = configFile.AppSettings.Settings;
//			if ( settings[ key ] == null ) {
//				settings.Add( key, value );
//			}
//			else {
//				settings[ key ].Value = value;
//			}
//			configFile.Save( ConfigurationSaveMode.Modified );

//			ConfigurationManager.RefreshSection( configFile.AppSettings.SectionInformation.Name );

//			return true;
//		}
//		catch ( ConfigurationErrorsException exception ) {
//			exception.Log();
//		}
//		return false;
//	}

//	/// <summary>Return the value of the given <paramref name="key" />.</summary>
//	/// <param name="key"></param>
//	/// <returns></returns>
//	public static String Settings( String key ) => Environment.SpecialFolder.LocalApplicationData.Settings( key );

//	/// <summary>Return the value of the given <paramref name="key" />.</summary>
//	/// <param name="specialFolder"></param>
//	/// <param name="key"></param>
//	/// <returns></returns>
//	public static String Settings( this Environment.SpecialFolder specialFolder, String key ) {
//		try {
//			var configFile = ConfigurationManager.OpenExeConfiguration( specialFolder.GetStaticFile().FullPath );
//			return configFile.AppSettings.Settings[ key ]?.Value;
//		}
//		catch ( ConfigurationErrorsException exception ) {
//			exception.Log();
//			return default;
//		}
//	}

//	public static void TestStaticStorage() {
//		const String phraseToTest = "Hello world";

//		Settings( nameof( phraseToTest ), phraseToTest );

//		//Assert.AreEqual( phraseToTest, Settings( nameof( phraseToTest ) ) );
//	}

//	private static Document GetStaticFile( this Environment.SpecialFolder specialFolder ) {
//		var path = Path.Combine( Environment.GetFolderPath( specialFolder ), nameof( Settings ) );
//		if ( !Directory.Exists( path ) ) {
//			Directory.CreateDirectory( path );
//		}
//		var destinationFile = Path.Combine( path, "StaticSettings.exe" );

//		if ( File.Exists( destinationFile ) ) {
//			return new Document( destinationFile );
//		}

//		using ( File.Create( destinationFile ) ) { }
//		return new Document( destinationFile );
//	}
//}