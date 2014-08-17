namespace Librainian.Maths {
    using System;
    using FluentAssertions;
    using NUnit.Framework;

    public static class TestUnitsOfMath {

        [Test]
        public static void TestNumberConversions() {

            var test = BigDecimal.Parse( "0" );
            test.Should().Be( BigDecimal.Zero );

            test += BigDecimal.Parse( "1" );
            test.Should().Be( BigDecimal.One );

            test += BigDecimal.Parse( "0.000123" );
            ( ( Decimal )test ).Should().Be( 1.000123m );

            test += BigDecimal.Parse( "-10.000123" );
            ( ( Decimal )test ).Should().Be( -8.999877M );

            test += BigDecimal.Parse( "11.1234567890" );
            ( ( Decimal )test ).Should().Be( 2.123579789M );

            test += BigDecimal.Parse( "-0.12342345" );
            ( ( Decimal )test ).Should().Be( 2.000156339M );

            test += BigDecimal.Parse( "1.1234567890" );
            ( ( Decimal )test ).Should().Be( 3.123613128M );


            test += BigDecimal.Parse( "-001.1234567890" );

            var test2 = BigDecimal.Parse( "1111111111111111111111111111111111111111.11111111111111111111111111111111111111111111111111111111111111111111111" );

            test2 += test2 * test2;

            Console.WriteLine( test2.ToString() );

        }

    }
}
