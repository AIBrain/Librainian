namespace LibrainianTests {

    using System;
    using Librainian.Security;
    using NUnit.Framework;

    [TestFixture]
    public static class SecurityTests {

        [Test]
        public static void TestEncryptionAndDecryption() {
            const String phraseToTest = "Hello world";

            var encrypted = phraseToTest.ToSecureString().EncryptString();
            var decrypted = encrypted.DecryptString().ToInsecureString();

            Assert.AreEqual( expected: decrypted, actual: phraseToTest );
        }

        [Test]
        public static void TestGenerateGenerates() {
            var result = SecurityExtensions.GenerateKey( username: "Test" );
            Console.WriteLine( result );

            // 1-KbQP3bo4zph3ynO0flLTxzB8d25AY74E
        }
    }
}