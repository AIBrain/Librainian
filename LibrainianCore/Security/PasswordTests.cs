namespace LibrainianCore.Security {

    using System;
    using System.Globalization;
    using Maths;

    [TestFixture]
    public static class PasswordTests {

        [Test]
        public static void TestAFew() {
            foreach ( var variable in 1.To( 25 ) ) {
                Console.WriteLine( ( String ) PronounceablePasswordCreator.Generate( 3.Next( 15 ) ).ToPascalCase( CultureInfo.CurrentUICulture ) );
            }
        }

    }

}
