// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "FileHistoryFileTests.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/LibrainianTests/FileHistoryFileTests.cs" was last cleaned by Protiguous on 2018/05/15 at 10:51 PM.

namespace LibrainianTests {

    using System;
    using System.Globalization;
    using FluentAssertions;
    using Librainian.FileSystem;
    using Librainian.Magic;
    using Librainian.OperatingSystem.FileHistory;
    using NUnit.Framework;

    [TestFixture]
    public static class FileHistoryFileTests {

        public const String Example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

        [Test]
        public static void RunTests() {
            var example = DateTime.Parse( "2015/09/04 16:15:01" );

            //if ( !DateTime.TryParseExact(example , "yyyy/MM/dd hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out result ) ) {
            if ( !DateTime.TryParse( example.ToString( CultureInfo.InvariantCulture ), out _ ) ) { throw new InvalidOperationException(); }

            if ( !new Document( Example ).TryParse( out _, out _, out _ ) ) { throw new InvalidCastException(); }
        }

        public static void TestForNullNess() => Example.Should().ThrowIfNull();
    }
}