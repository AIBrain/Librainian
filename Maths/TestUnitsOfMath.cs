namespace Librainian.Maths {
    using System.Numerics;
    using NUnit.Framework;

    public static class TestUnitsOfMath {

        [Test]
        public static void TestNumberConversion() {

            var number1 = new Number( 1.1234, 2.345 );

            var number2 = new Number( ( BigInteger )11234, 2345 );

        }

    }
}
