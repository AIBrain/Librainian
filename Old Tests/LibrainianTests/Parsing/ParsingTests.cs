
namespace LibrainianTests.Parsing {

    using System;
    using FluentAssertions;
    using Librainian.Parsing;
    using Xunit;

    public class ParsingTests {

        private String left;

        private String right;

        [Fact]
        public void ConfirmStringNullLikeNull() {
            this.left = "".Trim().NullIfBlank();
            this.right = default;
            this.left?.Like( this.right ).Should().BeTrue();
        }

        [Fact]
        public void ConfirmStringEmptyStringNotLikeNull() {
            this.left = default;
            this.right = String.Empty;
            this.right.Like( this.left ).Should().BeFalse();
        }

        [Fact]
        public void ConfirmStringEmptyStringLikeEmptyString() {
            this.left = String.Empty;
            this.right = String.Empty;
            this.left.Like( this.right ).Should().BeTrue();
        }

        [Fact]
        public void ConfirmcAtLikeCaT() {
            this.left = "cAt";
            this.right = "CaT";
            this.left.Like( this.right ).Should().BeTrue();
        }

        [Fact]
        public void ConfirmcAtNotSameCaT() {
            this.left = "cAt";
            this.right = "CaT";
            this.left.Same( this.right ).Should().BeFalse();
        }

        [Fact]
        public void ConfirmStringLimitShorter() => ParsingConstants.EnglishAlphabetLowercase.Limit( 6 ).Should().Be( "abcdef" );

        [Fact]
        public void ConfirmStringLimitLonger() =>
            ParsingConstants.EnglishAlphabetLowercase.Limit( ParsingConstants.EnglishAlphabetLowercase.Length * 2 ).Should()
                .Be( ParsingConstants.EnglishAlphabetLowercase );

    }
}
