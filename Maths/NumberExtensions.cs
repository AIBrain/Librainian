namespace Librainian.Maths {
    using System;
    using System.Numerics;
    using Numerics;
    using NUnit.Framework;
    using Threading;

    public static class NumberExtensions {

        public static BigDecimal ToBigDecimal( this String longDecimalString ) {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Number"/>
        [Test]
        public static Boolean TestNumberParsings() {

            //var bob = "18913489007071346701367013467767613401616136.136301590214084662236232265343672235925607263623468709823672366";
            var bob = String.Format( "{0}.{1}", Randem.NextString( length: 31, numbers: true ), Randem.NextString( length: 31, numbers: true ) );
            bob = "-18913489007071346701367013467767613401616136.136301590214084662236232265343672235925607263623468709823672366";


            BigInteger beforeDecimalPoint;
            BigInteger afterDecimalPoint;
            Number? sdgasdgd;
            var result = Number.TryParseNumber( bob, out sdgasdgd );
            return result;
        }
    }
}