// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/FileHistoryFileTests.cs" was last cleaned by Rick on 2015/11/13 at 11:31 PM

namespace Librainian.OperatingSystem.FileHistory {

    using System;
    using FileSystem;
    using FluentAssertions;
    using Magic;
    using NUnit.Framework;

    [TestFixture]
    public static class FileHistoryFileTests {

        public const String Example = @"S:\do not delete! FileHistory\Rick\ZEUS do not delete!\Data\C\Users\Rick\Desktop\autoruns (2015_09_04 16_15_01 UTC).exe";

        public static void TestForNullNess() {
            Example.Should()
                   .ThrowIfNull();
        }

        [Test]
        public static void RunTests() {
            var example = DateTime.Parse( "2015/09/04 16:15:01" );

            DateTime result;

            //if ( !DateTime.TryParseExact(example , "yyyy/MM/dd hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out result ) ) {
            if ( !DateTime.TryParse( example.ToString(), out result ) ) {
                throw new InvalidOperationException();
            }

            Folder folder;
            String filename;
            DateTime? when;
            if ( !new Document( Example ).TryParse( out folder, out filename, out when ) ) {
                throw new InvalidCastException();
            }
        }

    }

}
