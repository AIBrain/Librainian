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
            ( ( Decimal )test ).Should().Be( -9M );

            test += BigDecimal.Parse( "11.1234567890" );
            ( ( Decimal )test ).Should().Be( 2.123456789M );

            test += BigDecimal.Parse( "-0.12342345" );
            ( ( Decimal )test ).Should().Be( 2.000033339M );

            test += BigDecimal.Parse( "1.1234567890" );
            ( ( Decimal )test ).Should().Be( 3.123490128M );

            test += BigDecimal.Parse( "-001.1234567890" );
            ( ( Decimal )test ).Should().Be( 2.000033339M );

            var test2 = BigDecimal.Parse( "111111.111111" );
            ( ( Decimal )test2 ).Should().Be( 111111.111111M );

            test2 += test2 * test2;
            ( ( Decimal )test ).Should().Be( 12345679012.320987654321M );
            

            Console.WriteLine( test2.ToString() );

        }

    }
}
