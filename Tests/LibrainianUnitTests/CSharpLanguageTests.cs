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
// File "CSharpLanguageTests.cs" last touched on 2021-11-17 at 1:48 PM by Protiguous.

namespace LibrainianUnitTests;

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NUnit.Framework;

/// <summary>
///     Mostly just for fun.
///     <para>A few to assert assumptions still work even though the language is evolving.</para>
/// </summary>
[TestFixture]
public static class CSharpLanguageTests {

	private const Boolean False = false;

	private const Boolean True = true;

	[Test]
	[SuppressMessage( "ReSharper", "SuggestVarOrType_BuiltInTypes" )]
	[SuppressMessage( "ReSharper", "DoubleNegationOperator" )]
	[SuppressMessage( "ReSharper", "InlineTemporaryVariable" )]
	[SuppressMessage( "ReSharper", "ConvertToConstant.Local" )]
	[SuppressMessage( "Style", "IDE0007:Use implicit type", Justification = "Just for tests." )]
	public static void TestBooleanFalse() {
		Boolean b0 = False;
		var b1 = False;
		var b2 = new Boolean();
		Boolean b3 = new();
		var b4 = !!False;

		False.Should()!.BeFalse();
		b0.Should()!.BeFalse();
		b1.Should()!.BeFalse();
		b2.Should()!.BeFalse();
		b3.Should()!.BeFalse();
		b4.Should()!.BeFalse();
	}

	[Test]
	[SuppressMessage( "ReSharper", "SuggestVarOrType_BuiltInTypes" )]
	[SuppressMessage( "ReSharper", "DoubleNegationOperator" )]
	[SuppressMessage( "ReSharper", "InlineTemporaryVariable" )]
	[SuppressMessage( "ReSharper", "ConvertToConstant.Local" )]
	[SuppressMessage( "Style", "IDE0007:Use implicit type", Justification = "Just for tests." )]
	public static void TestBooleanTrue() {
		Boolean b0 = True;
		var b1 = True;
		var b2 = !new Boolean();
		var b3 = !!!false;

		True.Should()!.BeTrue();
		b0.Should()!.BeTrue();
		b1.Should()!.BeTrue();
		b2.Should()!.BeTrue();
		b3.Should()!.BeTrue();
	}

}