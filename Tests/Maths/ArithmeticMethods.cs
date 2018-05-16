// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "ArithmeticMethods.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/LibrainianTests/ArithmeticMethods.cs" was last cleaned by Protiguous on 2018/05/15 at 10:51 PM.

namespace LibrainianTests.Maths {

    using Librainian.Maths;
    using Xunit;

    public class ArithmeticMethods {

        [Fact]
        public void AddBitsMethod() {
            var size = ByteSize.FromBytes( 1 ).AddBits( 8 );

            Assert.Equal( 2, size.Bytes );
            Assert.Equal( 16, size.Bits );
        }

        [Fact]
        public void AddBytesMethod() {
            var size = ByteSize.FromBytes( 1 ).AddBytes( 1 );

            Assert.Equal( 2, size.Bytes );
            Assert.Equal( 16, size.Bits );
        }

        [Fact]
        public void AddGigaBytesMethod() {
            var size = ByteSize.FromGigaBytes( 2 ).AddGigaBytes( 2 );

            Assert.Equal( 4d * 1024 * 1024 * 1024 * 8, size.Bits );
            Assert.Equal( 4d * 1024 * 1024 * 1024, size.Bytes );
            Assert.Equal( 4d * 1024 * 1024, size.KiloBytes );
            Assert.Equal( 4d * 1024, size.MegaBytes );
            Assert.Equal( 4d, size.GigaBytes );
        }

        [Fact]
        public void AddKiloBytesMethod() {
            var size = ByteSize.FromKiloBytes( 2 ).AddKiloBytes( 2 );

            Assert.Equal( 4 * 1024 * 8, size.Bits );
            Assert.Equal( 4 * 1024, size.Bytes );
            Assert.Equal( 4, size.KiloBytes );
        }

        [Fact]
        public void AddMegaBytesMethod() {
            var size = ByteSize.FromMegaBytes( 2 ).AddMegaBytes( 2 );

            Assert.Equal( 4 * 1024 * 1024 * 8, size.Bits );
            Assert.Equal( 4 * 1024 * 1024, size.Bytes );
            Assert.Equal( 4 * 1024, size.KiloBytes );
            Assert.Equal( 4, size.MegaBytes );
        }

        [Fact]
        public void AddMethod() {
            var size1 = ByteSize.FromBytes( 1 );
            var result = size1.Add( size1 );

            Assert.Equal( 2, result.Bytes );
            Assert.Equal( 16, result.Bits );
        }

        [Fact]
        public void AddPetaBytesMethod() {
            var size = ByteSize.FromPetaBytes( 2 ).AddPetaBytes( 2 );

            Assert.Equal( 4d * 1024 * 1024 * 1024 * 1024 * 1024 * 8, size.Bits );
            Assert.Equal( 4d * 1024 * 1024 * 1024 * 1024 * 1024, size.Bytes );
            Assert.Equal( 4d * 1024 * 1024 * 1024 * 1024, size.KiloBytes );
            Assert.Equal( 4d * 1024 * 1024 * 1024, size.MegaBytes );
            Assert.Equal( 4d * 1024 * 1024, size.GigaBytes );
            Assert.Equal( 4d * 1024, size.TeraBytes );
            Assert.Equal( 4d, size.PetaBytes );
        }

        [Fact]
        public void AddTeraBytesMethod() {
            var size = ByteSize.FromTeraBytes( 2 ).AddTeraBytes( 2 );

            Assert.Equal( 4d * 1024 * 1024 * 1024 * 1024 * 8, size.Bits );
            Assert.Equal( 4d * 1024 * 1024 * 1024 * 1024, size.Bytes );
            Assert.Equal( 4d * 1024 * 1024 * 1024, size.KiloBytes );
            Assert.Equal( 4d * 1024 * 1024, size.MegaBytes );
            Assert.Equal( 4d * 1024, size.GigaBytes );
            Assert.Equal( 4d, size.TeraBytes );
        }

        [Fact]
        public void DecrementOperator() {
            var size = ByteSize.FromBytes( 2 );
            size--;

            Assert.Equal( 8, size.Bits );
            Assert.Equal( 1, size.Bytes );
        }

        [Fact]
        public void IncrementOperator() {
            var size = ByteSize.FromBytes( 2 );
            size++;

            Assert.Equal( 24, size.Bits );
            Assert.Equal( 3, size.Bytes );
        }

        [Fact]
        public void MinusOperatorBinary() {
            var size = ByteSize.FromBytes( 4 ) - ByteSize.FromBytes( 2 );

            Assert.Equal( 16, size.Bits );
            Assert.Equal( 2, size.Bytes );
        }

        [Fact]
        public void MinusOperatorUnary() {
            var size = ByteSize.FromBytes( 2 );

            size = -size;

            Assert.Equal( -16, size.Bits );
            Assert.Equal( -2, size.Bytes );
        }

        [Fact]
        public void SubtractMethod() {
            var size = ByteSize.FromBytes( 4 ).Subtract( ByteSize.FromBytes( 2 ) );

            Assert.Equal( 16, size.Bits );
            Assert.Equal( 2, size.Bytes );
        }
    }
}