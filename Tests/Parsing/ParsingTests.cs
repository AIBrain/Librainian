
namespace LibrainianTests.Parsing {

    using System;
    using FluentAssertions;
    using Librainian.Parsing;
    using NUnit.Framework;

    [TestFixture]
    public class ParsingTests {

        private String left;

        private String right;

        [Test]
        public void ConfirmStringNullLikeNull() {
            this.left = "".Trim().NullIfBlank();
            this.right = default;
            this.left?.Like( this.right ).Should().BeTrue();
        }

        [Test]
        public void ConfirmStringEmptyStringNotLikeNull() {
            this.left = default;
            this.right = String.Empty;
            this.right.Like( this.left ).Should().BeFalse();
        }

        [Test]
        public void ConfirmStringEmptyStringLikeEmptyString() {
            this.left = String.Empty;
            this.right = String.Empty;
            this.left.Like( this.right ).Should().BeTrue();
        }

        [Test]
        public void ConfirmcAtLikeCaT() {
            this.left = "cAt";
            this.right = "CaT";
            this.left.Like( this.right ).Should().BeTrue();
        }

        [Test]
        public void ConfirmcAtNotSameCaT() {
            this.left = "cAt";
            this.right = "CaT";
            this.left.Same( this.right ).Should().BeFalse();
        }

        [Test]
        public void ConfirmStringLimitShorter() => ParsingExtensions.EnglishAlphabetLowercase.Limit( 6 ).Should().Be( "abcdef" );

        [Test]
        public void ConfirmStringLimitLonger() =>
            ParsingExtensions.EnglishAlphabetLowercase.Limit( ParsingExtensions.EnglishAlphabetLowercase.Length * 2 ).Should()
                .Be( ParsingExtensions.EnglishAlphabetLowercase );

    }
}
