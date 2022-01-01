// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// 
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ParsingTests.cs" last touched on 2021-12-29 at 6:01 AM by Protiguous.

namespace LibrainianUnitTests.Parsing;

using System;
using System.Linq;
using FluentAssertions;
using Librainian.Parsing;
using NUnit.Framework;

[TestFixture]
public class ParsingTests {

	[Test]
	public void ConfirmcAtLikeCaT() {
		const String? left = "cAt";
		const String? right = "CaT";
		left.Like( right ).Should().BeTrue();
	}

	[Test]
	public void ConfirmcAtNotSameCaT() {
		const String? left = "cAt";
		const String? right = "CaT";
		left.Same( right ).Should().BeFalse();
	}

	[Test]
	public void ConfirmStringEmptyStringLikeEmptyString() {
		var left = String.Empty;
		var right = String.Empty;
		left.Like( right ).Should().BeTrue();
	}

	[Test]
	public void ConfirmStringEmptyStringNotLikeNull() {
		const String? left = default( String? );
		var right = String.Empty;
		right.Like( left ).Should().BeFalse();
	}

	[Test]
	public void ConfirmStringLeft() {
		var chars = ParsingConstants.English.Alphabet.Lowercase.Left( 6 );
		var s = new String( chars );
		s.Should().Be( "abcdef" );
	}

	[Test]
	public void ConfirmStringLimitShorter() {
		var chars = ParsingConstants.English.Alphabet.Lowercase.Take( 6 ).ToArray();
		var s = new String( chars );
		s.Should().Be( "abcdef" );
	}

	[Test]
	public void ConfirmStringNullLikeNull() {
		var left = "".Trim().NullIfBlank();
		const String? right = default( String? );
		left?.Like( right ).Should().BeTrue();
	}

	[Test]
	public void ConfirmStringRight() {
		var chars = ParsingConstants.English.Alphabet.Lowercase.Right( 6 );
		var s = new String( chars );
		s.Should().Be( "uvwxyz" );
	}

	[Test]
	public void TestHasSomeWhiteSpaceFalse1() {
		const String test = "a1s2d3f4g5hj6j7";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeFalse();
	}

	[Test]
	public void TestHasSomeWhiteSpaceTrue1() {
		const String test = " a1s2d3f4g5hj6j7 ";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeTrue();
	}

	[Test]
	public void TestHasSomeWhiteSpaceTrue2a() {
		const String test = "";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeFalse();
	}

	[Test]
	public void TestHasSomeWhiteSpaceTrue2b() {
		const String test = " a1s2d3f4g5hj6j7 ";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeTrue();
	}

	[Test]
	public void TestHasSomeWhiteSpaceTrue3() {
		const String test = " 1 ";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeTrue();
	}

	[Test]
	public void TestHasSomeWhiteSpaceTrue4() {
		const String test = " 	";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeTrue();
	}

	[Test]
	public void TestHasSomeWhiteSpaceTrue5() {
		const String test = "		";
		var result = test.HasSomeWhiteSpace();
		result.Should().BeTrue();
	}

	[Test]
	public void TestUnDoubleWording() {
		var test = "My cat cat likes likes to to to eat food.";
		var result = test.RemoveDoubleWords();
		result.Should().Be( "My cat likes to eat food ." );
	}

}