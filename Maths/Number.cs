
namespace Librainian.Maths {
    using Extensions;
    using Numerics;

    [Immutable]
    public struct Number {

        public readonly BigDecimal Numerator;
        public readonly BigDecimal Denominator;
        public readonly BigDecimal Answer;

        private BigRational bigRational;

        public Number( BigDecimal numerator, BigDecimal denominator ) {

            Answer = numerator / denominator;

            this.Numerator = new BigDecimal();

            this.Denominator = new BigDecimal();

            this.bigRational = new BigRational();
        }
    }
}
