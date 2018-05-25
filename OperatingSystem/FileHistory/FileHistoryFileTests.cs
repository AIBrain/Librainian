// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "FileHistoryFileTests.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/FileHistoryFileTests.cs" was last formatted by Protiguous on 2018/05/24 at 7:28 PM.

namespace Librainian.OperatingSystem.FileHistory {

    using System;
    using System.Globalization;
    using ComputerSystems.FileSystem;
    using FluentAssertions;
    using Magic;
    using NUnit.Framework;

    [TestFixture]
    public static class FileHistoryFileTests {

        public const String Example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

        [Test]
        public static void RunTests() {
            var example = DateTime.Parse( "2015/09/04 16:15:01" );

            //if ( !DateTime.TryParseExact(example , "yyyy/MM/dd hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out result ) ) {
            if ( !DateTime.TryParse( example.ToString( CultureInfo.CurrentCulture ), out var result ) ) { throw new InvalidOperationException(); }

            if ( !FileHistoryFileExtensions.TryParseFileHistoryFile( new Document( Example ), out var folder, out var filename, out var when ) ) { throw new InvalidCastException(); }
        }

        public static void TestForNullNess() => Example.Should().ThrowIfNull();
    }
}