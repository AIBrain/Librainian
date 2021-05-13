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
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "FileHistoryFileTests.cs" last touched on 2021-03-07 at 3:20 PM by Protiguous.

namespace LibrainianUnitTests.OperatingSystem {

	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using Librainian.FileSystem;
	using Librainian.FileSystem.FileHistory;
	using Xunit;

	public static class FileHistoryFileTests {

		public const String Example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

#pragma warning disable IDE0059 // Unnecessary assignment of a value

		[Fact]
		[SuppressMessage( "ReSharper", "UnusedVariable" )]
		public static void RunTests() {
			var example = DateTime.Parse( "2015/09/04 16:15:01" );

			//if ( !DateTime.TryParseExact(example , "yyyy/MM/dd hh:mm:ss", CultureInfo.Ordinal, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out result ) ) {
			if ( !DateTime.TryParse( example.ToString( CultureInfo.CurrentCulture ), out var result ) ) {
				throw new InvalidOperationException();
			}

			if ( !FileHistoryFile.TryParseFileHistoryFile( new Document( Example ), out var folder, out var filename, out var when ) ) {
				throw new InvalidCastException();
			}
		}

#pragma warning restore IDE0059 // Unnecessary assignment of a value

	}

}