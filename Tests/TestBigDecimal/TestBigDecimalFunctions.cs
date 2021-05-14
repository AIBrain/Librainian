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
// File "TestBigDecimalFunctions.cs" last touched on 2021-05-14 at 7:57 AM by Protiguous.

namespace TestBigDecimal {

	using System;
	using System.Diagnostics;
	using System.Numerics;
	using Librainian.Maths.Bigger;
	using NUnit.Framework;

	[TestFixture]
	public class TestBigDecimalFunctions {

		[Test]
		public void TestGCD() {
			var expectedResult = BigDecimal.Parse( "10" );

			BigDecimal result = BigIntegerHelper.GCD( new BigInteger[] {
				20, 30, 210, 310, 360, 5040, 720720
			} );

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestGetFractionalPart() {
			var expectedResult = new BigDecimal( BigInteger.Parse( "9150201282920942551781108927727789384397020382853" ), -49 );
			var value = new BigDecimal( BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ), -49 );

			var result = value.GetFractionalPart();

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestGetLength() {
			var expectedResult = BigDecimal.Parse( "2268507702394854741827137539360680923314" );
			var value = new BigDecimal( BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ), -49 );

			BigDecimal result = value.WholeValue;

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestGetSign() {
			BigDecimal zero1 = 0;
			var zero2 = new BigDecimal( 0 );
			var zero3 = new BigDecimal( 0 );
			var zero4 = new BigDecimal( BigInteger.Zero );
			var zero5 = new BigDecimal( 0, -1 );
			BigDecimal zero6 = BigInteger.Subtract( BigInteger.Add( BigInteger.Divide( 2, 3 ), BigInteger.Multiply( -1, BigInteger.Divide( 1, 3 ) ) ), BigInteger.Divide( 1, 3 ) );

			var oneTenth = BigDecimal.Divide( BigDecimal.One, new BigDecimal( 10 ) );
			BigDecimal pointZeroOne = 0.1d;

			var zero7 = BigDecimal.Subtract( oneTenth, pointZeroOne );
			var zero8 = BigDecimal.Add( new BigDecimal( 1, -1 ), -1.0 / 10 );
			var zero9 = new BigDecimal( 15274, -7 ) * 0;

			BigDecimal positive1 = 1;
			BigDecimal positive2 = -1 * -1;

			var negative1 = BigDecimal.Multiply( BigDecimal.One, BigDecimal.MinusOne );
			var negative2 = BigDecimal.Subtract( BigDecimal.Zero, 3 );
			BigDecimal negative3 = BigInteger.Subtract( 0, 3 );
			BigDecimal negative4 = 10 * -1;

			Assert.AreEqual( 0, zero1.Sign );
			Assert.AreEqual( 0, zero2.Sign );
			Assert.AreEqual( 0, zero3.Sign );
			Assert.AreEqual( 0, zero4.Sign );
			Assert.AreEqual( 0, zero5.Sign );
			Assert.AreEqual( 0, zero6.Sign );
			Assert.AreEqual( 0, zero7.Sign );
			Assert.AreEqual( 0, zero8.Sign );
			Assert.AreEqual( 0, zero9.Sign );

			Assert.AreEqual( 1, positive1.Sign );
			Assert.AreEqual( 1, positive2.Sign );

			Assert.AreEqual( BigInteger.MinusOne, negative1.Sign );
			Assert.AreEqual( BigInteger.MinusOne, negative2.Sign );
			Assert.AreEqual( BigInteger.MinusOne, negative3.Sign );
			Assert.AreEqual( BigInteger.MinusOne, negative4.Sign );
		}

		[Test]
		public void TestGetWholeValue() {
			var expectedResult = BigDecimal.Parse( "2268507702394854741827137539360680923314" );
			var value = new BigDecimal( BigInteger.Parse( "22685077023948547418271375393606809233149150201282920942551781108927727789384397020382853" ), -49 );

			BigDecimal result = value.WholeValue;

			Assert.AreEqual( expectedResult.ToString(), result.ToString() );
		}

		[Test]
		public void TestIrrational001() {
			var goldenRatio = BigDecimal.Parse(
				"1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072072041893911374847540880753868917521266338622235369317931800607667263544333890865959395829056383226613199282902678806752087668925017116962070322210432162695486262963136144381497587012203408058879544547492461856953648644492" );

			Debug.WriteLine( "" );
			Debug.WriteLine( goldenRatio.ToString() );
			Debug.WriteLine( "" );
			Debug.WriteLine( "" );
		}

		[Test]
		public void TestIrrational002() {
			var goldenRatio = BigDecimal.Parse(
				"1.6180339887498948482045868343656381177203091798057628621354486227052604628189024497072072041893911374847540880753868917521266338622235369317931800607667263544333890865959395829056383226613199282902678806752087668925017116962070322210432162695486262963136144381497587012203408058879544547492461856953648644492" );

			Debug.WriteLine( "" );
			Debug.WriteLine( goldenRatio.ToString() );
			Debug.WriteLine( "" );
			Debug.WriteLine( "" );
		}

		[Test]
		public void TestLCD() {
			var expectedResult = BigDecimal.Parse( "45319990731015" );

			BigDecimal result = BigIntegerHelper.LCM( new BigInteger[] {
				3, 5, 7, 11, 13, 101, 307, 311, 313
			} );

			// 15015,
			// lcm(3, 5, 7, 11, 13, 101, 307, 311, 313) = 45319990731015
			// lcm(4973, 4292, 4978, 4968, 4297, 4287)  = 2822891742340306560

			Assert.AreEqual( expectedResult, result );
		}

		[Test]
		public void TestRounding() {
			var _ = BigDecimal.Parse( "10000000000000000000000000000000000000000000000000001" );

			var up = BigDecimal.Parse( 0.50001 );
			var down = BigDecimal.Parse( 0.49 );
			var oneAndAhalf = BigDecimal.Parse( "1.5" );

			var negEightPointFive = BigDecimal.Parse( -8.5 );
			BigDecimal negNinePointFive = -9.5d;

			var threePointFourNine = BigDecimal.Parse( 3.49 );
			var threePointFiveOne = BigDecimal.Parse( 3.51 );
			var sixPointFive = BigDecimal.Parse( 6.5 );

			var one = BigDecimal.Round( up );
			var zero = BigDecimal.Round( down );
			var two = BigDecimal.Round( oneAndAhalf );
			var three = BigDecimal.Round( threePointFourNine );
			var four = BigDecimal.Round( threePointFiveOne );
			var six = BigDecimal.Round( sixPointFive, MidpointRounding.ToEven );
			var negEight = BigDecimal.Round( negEightPointFive, MidpointRounding.ToEven );
			var negNine = BigDecimal.Round( negEightPointFive, MidpointRounding.AwayFromZero );
			var negTen = BigDecimal.Round( negNinePointFive, MidpointRounding.ToEven );

			Assert.AreEqual( BigInteger.One, one );
			Assert.AreEqual( BigInteger.Zero, zero );
			Assert.AreEqual( 2, two );
			Assert.AreEqual( 3, three );
			Assert.AreEqual( 4, four );
			Assert.AreEqual( 6, six );
			Assert.AreEqual( -8, negEight );
			Assert.AreEqual( -9, negNine );
			Assert.AreEqual( -10, negTen );
		}

		[Test]
		public void TestSignifigantDigits() {
			const Int32 expectedResult1 = 19;
			const Int32 expectedResult2 = 9;

			var number1 = new BigDecimal( 12345678901234567890, -10 );
			var number2 = new BigDecimal( 123456789, 1 );

			var result1 = number1.SignifigantDigits;
			var result2 = number2.SignifigantDigits;

			Assert.AreEqual( expectedResult1, result1 );
			Assert.AreEqual( expectedResult2, result2 );
		}

	}

}