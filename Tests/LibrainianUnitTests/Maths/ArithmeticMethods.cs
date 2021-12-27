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
// File "ArithmeticMethods.cs" last touched on 2021-05-13 at 1:16 PM by Protiguous.

namespace LibrainianUnitTests.Maths;

using Librainian.Maths;
using NUnit.Framework;

[TestFixture]
public class ArithmeticMethods {

	[Test]
	public void AddBitsMethod() {
		var size = ByteSize.FromBytes( 1 ).AddBits( 8 );

		Assert.AreEqual( 2, size.Bytes );
		Assert.AreEqual( 16, size.Bits );
	}

	[Test]
	public void AddBytesMethod() {
		var size = ByteSize.FromBytes( 1 ).AddBytes( 1 );

		Assert.AreEqual( 2, size.Bytes );
		Assert.AreEqual( 16, size.Bits );
	}

	[Test]
	public void AddGigaBytesMethod() {
		var size = ByteSize.FromGigaBytes( 2 ).AddGigaBytes( 2 );

		Assert.AreEqual( 4d * 1024 * 1024 * 1024 * 8, size.Bits );
		Assert.AreEqual( 4d * 1024 * 1024 * 1024, size.Bytes );
		Assert.AreEqual( 4d * 1024 * 1024, size.KiloBytes );
		Assert.AreEqual( 4d * 1024, size.MegaBytes );
		Assert.AreEqual( 4d, size.GigaBytes );
	}

	[Test]
	public void AddKiloBytesMethod() {
		var size = ByteSize.FromKiloBytes( 2 ).AddKiloBytes( 2 );

		Assert.AreEqual( 4 * 1024 * 8, size.Bits );
		Assert.AreEqual( 4 * 1024, size.Bytes );
		Assert.AreEqual( 4, size.KiloBytes );
	}

	[Test]
	public void AddMegaBytesMethod() {
		var size = ByteSize.FromMegaBytes( 2 ).AddMegaBytes( 2 );

		Assert.AreEqual( 4 * 1024 * 1024 * 8, size.Bits );
		Assert.AreEqual( 4 * 1024 * 1024, size.Bytes );
		Assert.AreEqual( 4 * 1024, size.KiloBytes );
		Assert.AreEqual( 4, size.MegaBytes );
	}

	[Test]
	public void AddMethod() {
		var size1 = ByteSize.FromBytes( 1 );
		var result = size1.Add( size1 );

		Assert.AreEqual( 2, result.Bytes );
		Assert.AreEqual( 16, result.Bits );
	}

	[Test]
	public void AddPetaBytesMethod() {
		var size = ByteSize.FromPetaBytes( 2 ).AddPetaBytes( 2 );

		Assert.AreEqual( 4d * 1024 * 1024 * 1024 * 1024 * 1024 * 8, size.Bits );
		Assert.AreEqual( 4d * 1024 * 1024 * 1024 * 1024 * 1024, size.Bytes );
		Assert.AreEqual( 4d * 1024 * 1024 * 1024 * 1024, size.KiloBytes );
		Assert.AreEqual( 4d * 1024 * 1024 * 1024, size.MegaBytes );
		Assert.AreEqual( 4d * 1024 * 1024, size.GigaBytes );
		Assert.AreEqual( 4d * 1024, size.TeraBytes );
		Assert.AreEqual( 4d, size.PetaBytes );
	}

	[Test]
	public void AddTeraBytesMethod() {
		var size = ByteSize.FromTeraBytes( 2 ).AddTeraBytes( 2 );

		Assert.AreEqual( 4d * 1024 * 1024 * 1024 * 1024 * 8, size.Bits );
		Assert.AreEqual( 4d * 1024 * 1024 * 1024 * 1024, size.Bytes );
		Assert.AreEqual( 4d * 1024 * 1024 * 1024, size.KiloBytes );
		Assert.AreEqual( 4d * 1024 * 1024, size.MegaBytes );
		Assert.AreEqual( 4d * 1024, size.GigaBytes );
		Assert.AreEqual( 4d, size.TeraBytes );
	}

	[Test]
	public void DecrementOperator() {
		var size = ByteSize.FromBytes( 2 );

		size--;

		Assert.AreEqual( 8, size.Bits );
		Assert.AreEqual( 1, size.Bytes );
	}

	[Test]
	public void IncrementOperator() {
		var size = ByteSize.FromBytes( 2 );

		size++;

		Assert.AreEqual( 24, size.Bits );
		Assert.AreEqual( 3, size.Bytes );
	}

	[Test]
	public void MinusOperatorBinary() {
		var size = ByteSize.FromBytes( 4 ) - ByteSize.FromBytes( 2 );

		Assert.AreEqual( 16, size.Bits );
		Assert.AreEqual( 2, size.Bytes );
	}

	[Test]
	public void MinusOperatorUnary() {
		var size = ByteSize.FromBytes( 2 );

		size = -size;

		Assert.AreEqual( -16, size.Bits );
		Assert.AreEqual( -2, size.Bytes );
	}

	[Test]
	public void SubtractMethod() {
		var size = ByteSize.FromBytes( 4 ).Subtract( ByteSize.FromBytes( 2 ) );

		Assert.AreEqual( 16, size.Bits );
		Assert.AreEqual( 2, size.Bytes );
	}

}