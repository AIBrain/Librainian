// Copyright � Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FileHistoryFileTests.cs" belongs to Protiguous@Protiguous.com and
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
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "LibrainianTests", "FileHistoryFileTests.cs" was last formatted by Protiguous on 2019/03/17 at 11:06 AM.

namespace LibrainianTests.OperatingSystem {

    using System;
    using System.Globalization;
    using FluentAssertions;
    using Librainian.Magic;
    using Librainian.OperatingSystem.FileSystem;
    using Librainian.OperatingSystem.FileSystem.FileHistory;
    using Xunit;

    public static class FileHistoryFileTests {

        public const String Example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

        public static void TestForNullNess() => Example.Should().ThrowIfNull();

        [Fact]
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
    }
}