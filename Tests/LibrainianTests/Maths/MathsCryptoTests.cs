// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any
// binaries, libraries, repositories, or source code (directly or derived) from our binaries,
// libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original
// license has been overwritten by formatting. (We try to avoid it from happening, but it does
// accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original
// license and our thanks goes to those Authors. If you find your code unattributed in this source
// code, please let us know so we can properly attribute you and include the proper license and/or
// copyright(s). If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied,
// or given. We are NOT responsible for Anything You Do With Our Code. We are NOT responsible for
// Anything You Do With Our Executables. We are NOT responsible for Anything You Do With Your
// Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our
// code in your project(s). For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "MathsCryptoTests.cs" last touched on 2022-01-22 at 1:19 AM by Protiguous.

namespace TestBigDecimal.Maths;

using Librainian.Maths;
using NUnit.Framework;

[TestFixture]
public static class MathsCryptoTests {

	[Test]
	public static void TestSanitize() {
		var random = new Random();

		for ( var i = 0; i < 10000; i++ ) {
			var btc = ( Decimal )random.NextDouble() * 10000M;

			var sanitized = btc.Sanitize();

			//var actual = sanitized.ToString( "G17" );
			//TestContext.WriteLine( actual );

			var test = sanitized - Math.Truncate( sanitized );
			var l = test.ToString( "G17" );

			Assert.That( l.Length <= 10, Is.True );
		}
	}
}