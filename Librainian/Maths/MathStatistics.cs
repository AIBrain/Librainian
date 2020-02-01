// Copyright © Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "MathStatistics.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
// 
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// 
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
// 
// Project: "Librainian", "MathStatistics.cs" was last formatted by Protiguous on 2020/01/31 at 12:26 AM.

namespace Librainian.Maths {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Measurement.Time;
    using Rationals;

    public static class MathStatistics {

        public static Decimal CalcAvg( [NotNull] this IEnumerable<Decimal> values ) => values.DefaultIfEmpty().Average( arg => arg );

        public static Decimal CalcEma( [NotNull] this IEnumerable<Decimal> values, Decimal alpha ) =>
            values.DefaultIfEmpty().Aggregate( ( ema, nextQuote ) => alpha * nextQuote + ( 1 - alpha ) * ema );

        /// <summary>
        ///     <para>
        ///     In mathematics, the geometric mean is a type of mean or average, which indicates the central tendency or typical value of a set of numbers by using the product of their
        ///     values (as opposed to the arithmetic mean which uses their sum).
        ///     </para>
        ///     <para>The geometric mean is defined as the nth root of the product of n numbers.</para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <see cref="http://wikipedia.org/wiki/Geometric_mean" />
        public static Double GeometricMean( [NotNull] this IEnumerable<Double> data, Int32 items ) {
            var aggregate = data.Aggregate( 1.0, ( current, d ) => current * d );

            return Math.Pow( aggregate, 1.0 / items );
        }

        /// <summary>
        ///     <para>
        ///     In mathematics, the geometric mean is a type of mean or average, which indicates the central tendency or typical value of a set of numbers by using the product of their
        ///     values (as opposed to the arithmetic mean which uses their sum).
        ///     </para>
        ///     <para>The geometric mean is defined as the nth root of the product of n numbers.</para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <see cref="http://wikipedia.org/wiki/Geometric_mean" />
        public static Decimal GeometricMean( [NotNull] this IEnumerable<Decimal> data, Int32 items ) {
            var aggregate = data.Aggregate( 1.0m, ( current, d ) => current * d );

            return ( Decimal ) Math.Pow( ( Double ) aggregate, ( Double ) ( 1.0m / items ) ); //BUG possible conversion errors here
        }

        /// <summary>
        ///     <para>
        ///     In mathematics, the geometric mean is a type of mean or average, which indicates the central tendency or typical value of a set of numbers by using the product of their
        ///     values (as opposed to the arithmetic mean which uses their sum).
        ///     </para>
        ///     <para>The geometric mean is defined as the nth root of the product of n numbers.</para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <see cref="http://wikipedia.org/wiki/Geometric_mean" />
        public static Rational GeometricMean( [NotNull] this IEnumerable<Rational> data, Int32 items ) {
            var aggregate = data.Aggregate( Rational.One, ( current, d ) => current * d );

            return Rational.Pow( aggregate, ( Int32 ) ( 1.0 / items ) ); //BUG possible conversion errors here
        }

        public static Double Intercept( [NotNull] this List<TimeProgression> data ) {
            if ( data is null ) {
                throw new ArgumentNullException( nameof( data ) );
            }

            var slope = data.Slope();

            return data.Average( d => d.Progress ) - slope * data.Average( d => d.MillisecondsPassed );
        }

        public static Double MeanGeometric( [NotNull] this IEnumerable<Double> numbers ) {
            var enumerable = numbers as IList<Double> ?? numbers.ToList();

            return Math.Pow( enumerable.Aggregate( ( s, i ) => s * i ), 1.0 / enumerable.Count );
        }

        public static Int32 MeanGeometric( [NotNull] this IEnumerable<Int32> numbers ) {
            var enumerable = numbers as IList<Int32> ?? numbers.ToList();

            return ( Int32 ) Math.Pow( enumerable.Aggregate( ( s, i ) => s * i ), 1.0 / enumerable.Count );
        }

        public static Single MeanGeometric( [NotNull] this IEnumerable<Single> numbers ) {
            var enumerable = numbers as IList<Single> ?? numbers.ToList();

            return ( Single ) Math.Pow( enumerable.Aggregate( ( s, i ) => s * i ), 1.0 / enumerable.Count );
        }

        public static Decimal MeanGeometric( [NotNull] this IEnumerable<Decimal> numbers ) {
            var enumerable = numbers as IList<Decimal> ?? numbers.ToList();
            Decimal result = 0;
            var first = true;

            foreach ( var @decimal in enumerable ) {
                if ( first ) {
                    first = false;
                    result = @decimal;

                    continue;
                }

                result *= @decimal;
            }

            return ( Decimal ) Math.Pow( ( Double ) result, 1.0 / enumerable.Count );
        }

        public static Double MeanHarmonic( [NotNull] this IEnumerable<Double> numbers ) {
            var enumerable = numbers as IList<Double> ?? numbers.ToList();

            return enumerable.Count / enumerable.Sum( i => 1 / i );
        }

        public static Int32 MeanHarmonic( [NotNull] this IEnumerable<Int32> numbers ) {
            var enumerable = numbers as IList<Int32> ?? numbers.ToList();

            return enumerable.Count / enumerable.Sum( i => 1 / i );
        }

        public static Single MeanHarmonic( [NotNull] this IEnumerable<Single> numbers ) {
            var enumerable = numbers as IList<Single> ?? numbers.ToList();

            return enumerable.Count / enumerable.Sum( i => 1 / i );
        }

        public static Decimal MeanHarmonic( [NotNull] this IEnumerable<Decimal> numbers ) {
            var enumerable = numbers as IList<Decimal> ?? numbers.ToList();

            return enumerable.Count / enumerable.Sum( i => 1 / i );
        }

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000f.OneIn();</example>
        public static Single OneIn( this Single possible ) => 1f / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this Double possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this UInt64 possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this Int64 possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this UInt32 possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this Int32 possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this UInt16 possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this Int16 possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this Byte possible ) => 1d / possible;

        /// <summary>One in <paramref name="possible" /></summary>
        /// <param name="possible"></param>
        /// <returns></returns>
        /// <example>var f = 7000.OneIn();</example>
        public static Double OneIn( this SByte possible ) => 1d / possible;

        public static Int32 Percent( this Int32 x, Single percent ) => ( Int32 ) ( x * percent / 100.0f );

        public static Single Percent( this Single x, Single percent ) => x * percent / 100.0f;

        public static Double Percent( this Double x, Double percent ) => x * percent / 100.0;

        public static Decimal Percent( this Decimal x, Decimal percent ) => x * percent / 100.0m;

        public static UInt64 Percent( this UInt64 x, Single percent ) => ( UInt64 ) ( x * percent / 100.0f );

        /// <summary>Returns true if this probability happens.</summary>
        /// <param name="probability"></param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this UInt64 probability, UInt64 maxvalue ) {
            var chance = Randem.Next( 0, maxvalue );

            return probability >= chance;
        }

        /// <summary>Returns true if this probability happens.</summary>
        /// <param name="probability"></param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this UInt32 probability, UInt32 maxvalue ) {
            var chance = Randem.Next( 0, maxvalue );

            return probability >= chance;
        }

        /// <summary>Returns true if this probability happens.</summary>
        /// <param name="probability"></param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this UInt16 probability, UInt16 maxvalue ) {
            var chance = 0.Next( maxvalue );

            return probability >= chance;
        }

        /// <summary>Returns true if this probability happens.</summary>
        /// <param name="probability"></param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this Int32 probability, Int32 maxvalue ) {
            var chance = 0.Next( maxValue: maxvalue );

            return probability >= chance;
        }

        /// <summary>Returns true <b>if</b> this probability happens.</summary>
        /// <param name="probability"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this Double probability ) {
            var chance = Randem.NextDouble( min: 0.0d, max: 1.0d );

            return probability >= chance;
        }

        /// <summary>Returns true <b>if</b> this probability happens.</summary>
        /// <param name="probability"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this Single probability ) {
            var chance = Randem.NextSingle( min: 0.0f, max: 1.0f );

            return probability >= chance;

            // if P is -0.1 then a chance of 0.01 will return default; a chance of 0.90 will return false

            // if P is 0.1 then a chance of 0.01 will return true a chance of 0.05 will return true
            // a chance of 0.09 will return true a chance of 0.10 will return false a chance of 0.50
            // will return false a chance of 0.90 will return false

            // if P is 0.89 then a chance of 0.01 will return true a chance of 0.05 will return true
            // a chance of 0.09 will return true a chance of 0.10 will return true a chance of 0.50
            // will return true a chance of 0.85 will return true a chance of 0.89 will return true
            // a chance of 0.90 will return false
        }

        public static Double StandardDeviation( [NotNull] this IEnumerable<Double> values ) {
            if ( values is null ) {
                throw new ArgumentNullException( nameof( values ) );
            }

            var doubles = values as Double[] ?? values.ToArray();
            var avg = doubles.Average();

            return Math.Sqrt( doubles.Average( v => Math.Pow( v - avg, 2 ) ) );
        }

        public static Decimal StandardDeviation( [NotNull] this IEnumerable<Decimal> values ) {
            if ( values is null ) {
                throw new ArgumentNullException( nameof( values ) );
            }

            var decimals = values as Decimal[] ?? values.ToArray();
            var avg = decimals.Average();

            return ( Decimal ) Math.Sqrt( decimals.Average( v => Math.Pow( ( Double ) ( v - avg ), 2 ) ) );
        }

    }

}