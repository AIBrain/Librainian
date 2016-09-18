namespace Librainian.Security {

    using System;
    using System.Globalization;
    using Maths;
    using NUnit.Framework;
    using Parsing;

    [TestFixture]
    public static class PasswordTests {

        [Test]
        public static void TestAFew() {
            foreach ( var VARIABLE in 1.To( 25 ) ) {
                Console.WriteLine( PronounceablePasswordCreator.Generate( 3.Next( 15 ) ).ToPascalCase( CultureInfo.CurrentUICulture ) );
            }
        }

    }

}
