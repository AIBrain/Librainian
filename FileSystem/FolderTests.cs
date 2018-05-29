namespace Librainian.FileSystem {
    using NUnit.Framework;
    using OperatingSystem;
    using Persistence;

    [TestFixture]
    public static class FolderTests {

        [Test]
        public static void TestSerialize() {
            var expected = Windows.WindowsSystem32Folder.Value;

            var json = expected.ToJSON();

            var actual = json.FromJSON< Folder >();

            Assert.AreEqual( expected, actual );
        }

    }
}