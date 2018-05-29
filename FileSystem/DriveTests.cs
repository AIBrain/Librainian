namespace Librainian.FileSystem {

    using System;
    using System.Diagnostics;
    using NUnit.Framework;
    using Parsing;

    [TestFixture]
    public static class DriveTests {

        [Test]
        public static void TestAllDrives() {
            var alphabet = ParsingExtensions.EnglishAlphabetUppercase;
            Debug.WriteLine( alphabet );

            foreach ( var letter in alphabet ) {
                var drive = new Drive( letter );
                if ( drive.FreeSpace() > 0 ) {
                    Console.WriteLine( drive + " " + drive.FreeSpace() + " " );
                }
            }
        }

    }

}