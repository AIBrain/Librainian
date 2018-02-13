namespace Tests.Linguistics {
    using System;
    using System.Linq;
    using FluentAssertions;
    using Librainian.Linguistics;
    using NUnit.Framework;

    [TestFixture]
    public static class SentenceTests {

        public const String Sample = "the quick brown fox jumped over the lazy dog";

        public static Sentence SampleLengthTest() {
            var sentence = new Sentence( Sample );
            sentence.LongCount()
               .Should()
               .Be( 9 );
            return sentence;
        }

        [Test]
        public static void CombinationsTest() {
            var sentence = SampleLengthTest();

            var list = sentence.ToList();

            var bob = sentence.Possibles().ToList();
            bob.Count.Should()
               .Be( 511 );
        }

    }
}
