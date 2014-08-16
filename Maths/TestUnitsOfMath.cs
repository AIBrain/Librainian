namespace Librainian.Maths {
    using NUnit.Framework;

    public static class TestUnitsOfMath {

        [Test]
        public static void TestNumberConversions() {

            var number1 = new BigDecimal( "1.12342345" );
            var number2 = new BigDecimal( "12369238762396823626626790362397690346234690723896743672340965234906539036626366466.12342345" );
            var number3 = new BigDecimal( "1.123423451596195601509123590612345690815906901569813560915691569156901612691690166501961550" );
            var number4 = new BigDecimal( "0.12342345" );
            var number5 = new BigDecimal( "0.00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000012342345" );
            var number6 = number2 + number3;


        }

    }
}
