#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/MathExtensions.cs" was last cleaned by Rick on 2014/08/13 at 8:09 AM
#endregion

namespace Librainian.Maths {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Annotations;
    using Collections;
    using FluentAssertions;
    using Measurement.Time;
    using Numerics;
    using NUnit.Framework;
    using Parsing;
    using Threading;

    public static class MathExtensions {
        // ReSharper disable UnusedMember.Global

        public delegate int FibonacciCalculator( int n );

        public const Boolean No = !Yes;

        public const string NumberBaseChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const Boolean Off = !On;

        public const Boolean On = true;

        public const UInt64 OneGigaByte = OneMegaByte * OneKiloByte;

        public const UInt64 OneKiloByte = 1024;

        public const UInt64 OneMegaByte = OneKiloByte * OneKiloByte;

        public const UInt64 OneTeraByte = OneGigaByte * OneKiloByte;

        public const Boolean Yes = !Off;

        /// <summary>
        ///     <para>Return the smallest possible value above <see cref="Decimal.Zero" /> for a <see cref="Decimal" />.</para>
        /// </summary>
        [UsedImplicitly]
        public const Decimal EpsilonDecimal = 0.0000000000000000000000000001m;

        public static readonly Double[] Logfactorialtable = { 0.000000000000000, 0.000000000000000, 0.693147180559945, 1.791759469228055, 3.178053830347946, 4.787491742782046, 6.579251212010101, 8.525161361065415, 10.604602902745251, 12.801827480081469, 15.104412573075516, 17.502307845873887, 19.987214495661885, 22.552163853123421, 25.191221182738683, 27.899271383840894, 30.671860106080675, 33.505073450136891, 36.395445208033053, 39.339884187199495, 42.335616460753485, 45.380138898476908, 48.471181351835227, 51.606675567764377, 54.784729398112319, 58.003605222980518, 61.261701761002001, 64.557538627006323, 67.889743137181526, 71.257038967168000, 74.658236348830158, 78.092223553315307, 81.557959456115029, 85.054467017581516, 88.580827542197682, 92.136175603687079, 95.719694542143202, 99.330612454787428, 102.968198614513810, 106.631760260643450, 110.320639714757390, 114.034211781461690, 117.771881399745060, 121.533081515438640, 125.317271149356880, 129.123933639127240, 132.952575035616290, 136.802722637326350, 140.673923648234250, 144.565743946344900, 148.477766951773020, 152.409592584497350, 156.360836303078800, 160.331128216630930, 164.320112263195170, 168.327445448427650, 172.352797139162820, 176.395848406997370, 180.456291417543780, 184.533828861449510, 188.628173423671600, 192.739047287844900, 196.866181672889980, 201.009316399281570, 205.168199482641200, 209.342586752536820, 213.532241494563270, 217.736934113954250, 221.956441819130360, 226.190548323727570, 230.439043565776930, 234.701723442818260, 238.978389561834350, 243.268849002982730, 247.572914096186910, 251.890402209723190, 256.221135550009480, 260.564940971863220, 264.921649798552780, 269.291097651019810, 273.673124285693690, 278.067573440366120, 282.474292687630400, 286.893133295426990, 291.323950094270290, 295.766601350760600, 300.220948647014100, 304.686856765668720, 309.164193580146900, 313.652829949878990, 318.152639620209300, 322.663499126726210, 327.185287703775200, 331.717887196928470, 336.261181979198450, 340.815058870798960, 345.379407062266860, 349.954118040770250, 354.539085519440790, 359.134205369575340, 363.739375555563470, 368.354496072404690, 372.979468885689020, 377.614197873918670, 382.258588773060010, 386.912549123217560, 391.575988217329610, 396.248817051791490, 400.930948278915760, 405.622296161144900, 410.322776526937280, 415.032306728249580, 419.750805599544780, 424.478193418257090, 429.214391866651570, 433.959323995014870, 438.712914186121170, 443.475088120918940, 448.245772745384610, 453.024896238496130, 457.812387981278110, 462.608178526874890, 467.412199571608080, 472.224383926980520, 477.044665492585580, 481.872979229887900, 486.709261136839360, 491.553448223298010, 496.405478487217580, 501.265290891579240, 506.132825342034830, 511.008022665236070, 515.890824587822520, 520.781173716044240, 525.679013515995050, 530.584288294433580, 535.496943180169520, 540.416924105997740, 545.344177791154950, 550.278651724285620, 555.220294146894960, 560.169054037273100, 565.124881094874350, 570.087725725134190, 575.057539024710200, 580.034272767130800, 585.017879388839220, 590.008311975617860, 595.005524249382010, 600.009470555327430, 605.020105849423770, 610.037385686238740, 615.061266207084940, 620.091704128477430, 625.128656730891070, 630.172081847810200, 635.221937855059760, 640.278183660408100, 645.340778693435030, 650.409682895655240, 655.484856710889060, 660.566261075873510, 665.653857411105950, 670.747607611912710, 675.847474039736880, 680.953419513637530, 686.065407301994010, 691.183401114410800, 696.307365093814040, 701.437263808737160, 706.573062245787470, 711.714725802289990, 716.862220279103440, 722.015511873601330, 727.174567172815840, 732.339353146739310, 737.509837141777440, 742.685986874351220, 747.867770424643370, 753.055156230484160, 758.248113081374300, 763.446610112640200, 768.650616799717000, 773.860102952558460, 779.075038710167410, 784.295394535245690, 789.521141208958970, 794.752249825813460, 799.988691788643450, 805.230438803703120, 810.477462875863580, 815.729736303910160, 820.987231675937890, 826.249921864842800, 831.517780023906310, 836.790779582469900, 842.068894241700490, 847.352097970438420, 852.640365001133090, 857.933669825857460, 863.231987192405430, 868.535292100464630, 873.843559797865740, 879.156765776907600, 884.474885770751830, 889.797895749890240, 895.125771918679900, 900.458490711945270, 905.796028791646340, 911.138363043611210, 916.485470574328820, 921.837328707804890, 927.193914982476710, 932.555207148186240, 937.921183163208070, 943.291821191335660, 948.667099599019820, 954.046996952560450, 959.431492015349480, 964.820563745165940, 970.214191291518320, 975.612353993036210, 981.015031374908400, 986.422203146368590, 991.833849198223450, 997.249949600427840, 1002.670484599700300, 1008.095434617181700, 1013.524780246136200, 1018.958502249690200, 1024.396581558613400, 1029.838999269135500, 1035.285736640801600, 1040.736775094367400, 1046.192096209724900, 1051.651681723869200, 1057.115513528895000, 1062.583573670030100, 1068.055844343701400, 1073.532307895632800, 1079.012946818975000, 1084.497743752465600, 1089.986681478622400, 1095.479742921962700, 1100.976911147256000, 1106.478169357800900, 1111.983500893733000, 1117.492889230361000, 1123.006317976526100, 1128.523770872990800, 1134.045231790853000, 1139.570684729984800, 1145.100113817496100, 1150.633503306223700, 1156.170837573242400 };

        public static readonly BigRational ThreeOverTwo = new BigRational( 3, 2 );
        public static readonly BigRational OneOverTwo = new BigRational( 1, 2 );
        public static readonly BigRational MinusOneOverTwo = new BigRational( -1, 2 );

        /// <summary>
        ///     ConvertBigIntToBcd
        /// </summary>
        /// <param name="numberToConvert"></param>
        /// <param name="howManyBytes"></param>
        /// <returns></returns>
        /// <seealso cref="http://github.com/mkadlec/ConvertBigIntToBcd/blob/master/ConvertBigIntToBcd.cs" />
        public static byte[] ConvertBigIntToBcd( this Int64 numberToConvert, int howManyBytes ) {
            var convertedNumber = new byte[ howManyBytes ];
            var strNumber = numberToConvert.ToString();
            var currentNumber = string.Empty;

            for ( var i = 0 ; i < howManyBytes ; i++ ) {
                convertedNumber[ i ] = 0xff;
            }

            for ( var i = 0 ; i < strNumber.Length ; i++ ) {
                currentNumber += strNumber[ i ];

                if ( i == strNumber.Length - 1 && i % 2 == 0 ) {
                    convertedNumber[ i / 2 ] = 0xf;
                    convertedNumber[ i / 2 ] |= ( byte )( ( int.Parse( currentNumber ) % 10 ) << 4 );
                }

                if ( i % 2 == 0 ) {
                    continue;
                }
                var value = int.Parse( currentNumber );
                convertedNumber[ ( i - 1 ) / 2 ] = ( byte )( value % 10 );
                convertedNumber[ ( i - 1 ) / 2 ] |= ( byte )( ( value / 10 ) << 4 );
                currentNumber = string.Empty;
            }

            return convertedNumber;
        }

        /// <summary>
        ///     Return true if an <see cref="IComparable" /> value is <see cref="Between{T}" /> two inclusive values.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="target"> </param>
        /// <param name="startInclusive"> </param>
        /// <param name="endInclusive"> </param>
        /// <returns> </returns>
        /// <example>5.Between(1, 10) == true</example>
        /// <example>5.Between(10, 1) == true</example>
        /// <example>5.Between(10, 6) == false</example>
        /// <example>5.Between(5, 5)) == true</example>
        public static Boolean Between<T>( this T target, T startInclusive, T endInclusive ) where T : IComparable {
            if ( startInclusive.CompareTo( endInclusive ) == 1 ) {
                return target.CompareTo( startInclusive ) <= 0 && ( target.CompareTo( endInclusive ) >= 0 );
            }
            return target.CompareTo( startInclusive ) >= 0 && ( target.CompareTo( endInclusive ) <= 0 );
        }

        /// <summary>
        ///     Combine two <see cref="UInt32" /> values into one <see cref="UInt64" /> value. Use Split() for the reverse.
        /// </summary>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static UInt64 Combine( this UInt32 high, UInt32 low ) {
            return ( UInt64 )high << 32 | low;
        }

        public static Double Crop( this Double x ) {
            return Math.Truncate( x * 100.0D ) / 100.0D;
        }

        public static Single Crop( this Single x ) {
            return ( Single )( Math.Truncate( x * 100.0f ) / 100.0f );
        }

        public static Double Erf( this Double x ) {
            // constants
            const Double a1 = 0.254829592;
            const Double a2 = -0.284496736;
            const Double a3 = 1.421413741;
            const Double a4 = -1.453152027;
            const Double a5 = 1.061405429;
            const Double p = 0.3275911;

            // Save the sign of x
            var sign = x < 0 ? -1 : 1;
            x = Math.Abs( x );

            // A&S formula 7.1.26
            var t = 1.0 / ( 1.0 + p * x );
            var y = 1.0 - ( ( ( ( ( a5 * t + a4 ) * t ) + a3 ) * t + a2 ) * t + a1 ) * t * Math.Exp( -x * x );

            return sign * y;
        }

        /// <summary>
        ///     Compute fibonacci series up to Max (> 1).
        ///     Example: foreach (int i in Fib(10)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static IEnumerable<int> Fib( int max ) {
            var a = 0;
            var b = 1;
            yield return 1;

            for ( var i = 0 ; i < max - 1 ; i++ ) {
                var c = a + b;
                yield return c;

                a = b;
                b = c;
            }
        }

        public static Double FiftyPercent( this Double x ) {
            var result = x / 2.0;
            return result < 1.0 ? 1 : result;
        }

        public static int FiftyPercent( this int x ) {
            var result = x / 2.0;
            return result < 1.0 ? 1 : ( int )result;
        }

        public static int FractionOf( this int x, Double top, Double bottom ) {
            var result = ( top * x ) / bottom;
            return result < 1.0 ? 1 : ( int )result;
        }

        public static Double FractionOf( this Double x, Double top, Double bottom ) {
            return ( top * x ) / bottom;
        }

        public static Single FractionOf( this Single x, Single top, Single bottom ) {
            return ( top * x ) / bottom;
        }

        public static UInt64 FractionOf( this UInt64 x, UInt64 top, UInt64 bottom ) {
            return ( top * x ) / bottom;
        }

        /// <summary>
        ///     Greatest Common Divisor for int
        /// </summary>
        /// <remarks>
        ///     Uses recursion, passing a remainder each time.
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int gcd( int x, int y ) {
            return y == 0 ? x : gcd( y, x % y );
        }

        /// <summary>
        ///     Greatest Common Divisor for long
        /// </summary>
        /// <remarks>
        ///     Uses recursion, passing a remainder each time.
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static long gcd( long x, long y ) {
            return y == 0 ? x : gcd( y, x % y );
        }

        /// <summary>
        ///     Greatest Common Divisor for int
        /// </summary>
        /// <remarks>
        ///     Uses a while loop and remainder.
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GCD( int a, int b ) {
            while ( b != 0 ) {
                var remainder = a % b;
                a = b;
                b = remainder;
            }

            return a;
        }

        /// <summary>
        ///     Greatest Common Divisor for long
        /// </summary>
        /// <remarks>
        ///     Uses a while loop and remainder.
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long GCD( long a, long b ) {
            while ( b != 0 ) {
                var remainder = a % b;
                a = b;
                b = remainder;
            }

            return a;
        }

        /// <summary>
        ///     Greatest Common Divisor for int
        /// </summary>
        /// <remarks>
        ///     More like the ancient greek Euclid originally devised it
        ///     Uses a while loop with subtraction.
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int gcd2( int x, int y ) {
            while ( x != y ) {
                if ( x > y ) {
                    x = x - y;
                }
                else {
                    y = y - x;
                }
            }
            return x;
        }

        /// <summary>
        ///     Greatest Common Divisor for long
        /// </summary>
        /// <remarks>
        ///     More like the ancient greek Euclid originally devised it
        ///     Uses a while loop with subtraction.
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static long gcd2( long x, long y ) {
            while ( x != y ) {
                if ( x > y ) {
                    x = x - y;
                }
                else {
                    y = y - x;
                }
            }
            return x;
        }

        public static Byte GetHashCodeByte<TLhs>( this TLhs objectA, Byte maximum = Byte.MaxValue ) {
            if ( Equals( objectA, default( TLhs ) ) ) {
                return 0;
            }
            unchecked {
                var hashA = ( Byte )objectA.GetHashCode();
                return ( Byte )( ( ( ( hashA << 5 ) + hashA ) ^ hashA ) % maximum );
            }
        }

        public static UInt16 GetHashCodeUInt16<TLhs>( this TLhs objectA, UInt16 maximum = UInt16.MaxValue ) {
            if ( Equals( objectA, default( TLhs ) ) ) {
                return 0;
            }
            unchecked {
                var hashA = ( UInt16 )objectA.GetHashCode();
                return ( UInt16 )( ( ( ( hashA << 5 ) + hashA ) ^ hashA ) % maximum );
            }
        }

        public static UInt32 GetHashCodeUInt32<TLhs>( this TLhs objectA, UInt32 maximum = UInt32.MaxValue ) {
            if ( Equals( objectA, default( TLhs ) ) ) {
                return 0;
            }
            unchecked {
                var hashA = ( UInt32 )objectA.GetHashCode();
                return ( ( ( hashA << 5 ) + hashA ) ^ hashA ) % maximum;
            }
        }

        public static UInt64 GetHashCodeUInt64<TLhs>( this TLhs objectA, UInt64 maximum = UInt64.MaxValue ) {
            if ( Equals( objectA, default( TLhs ) ) ) {
                return 0;
            }
            unchecked {
                var hashA = ( UInt64 )objectA.GetHashCode();
                return ( ( ( hashA << 5 ) + hashA ) ^ hashA ) % maximum;
            }
        }

        /// <summary>
        ///     Returns a combined <see cref="object.GetHashCode" /> based on <paramref name="objectA" /> and
        ///     <paramref name="objectB" />.
        /// </summary>
        /// <typeparam name="TLhs"></typeparam>
        /// <typeparam name="TRhs"></typeparam>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <returns></returns>
        public static Int32 GetHashMerge<TLhs, TRhs>( this TLhs objectA, TRhs objectB ) {
            if ( Equals( objectA, default( TLhs ) ) ) {
                return 0;
            }
            if ( Equals( objectB, default( TRhs ) ) ) {
                return 0;
            }
            unchecked {
                var hashA = objectA.GetHashCode();
                var hashB = objectB.GetHashCode();
                var combined = ( ( hashA << 5 ) + hashA ) ^ hashB;
                return combined;
            }
        }

        public static Single Half( this Single number ) {
            return number / 2.0f;
        }


        public static Double Half( this Double number ) {
            return number / 2.0d;
        }

        public static Byte Half( this Byte number ) {
            return ( Byte )( number / 2 );
        }

        public static TimeSpan Half( this TimeSpan timeSpan ) {
            return TimeSpan.FromTicks( timeSpan.Ticks.Half() );
        }

        public static Int32 Half( this Int32 number ) {
            return ( Int32 )( number / 2.0f );
        }
        public static Int16 Half( this Int16 number ) {
            return ( Int16 )( number / 2.0f );
        }

        public static UInt16 Half( this UInt16 number ) {
            return ( UInt16 )( number / 2.0f );
        }

        public static UInt32 Half( this UInt32 number ) {
            return ( UInt32 )( number / 2.0f );
        }

        public static UInt64 Half( this UInt64 number ) {
            return ( UInt64 )( number / 2.0d );
        }

        public static Int64 Half( this Int64 number ) {
            return ( Int64 )( number / 2.0d );
        }

        /// <summary>
        ///     <see
        ///         cref="http://stackoverflow.com/questions/17575375/how-do-i-convert-an-int-to-a-string-in-c-sharp-without-using-tostring" />
        /// </summary>
        /// <param name="n"></param>
        /// <param name="b"></param>
        /// <param name="minDigits"></param>
        /// <returns></returns>
        public static string IntToStringWithBase( this int n, int b, int minDigits = 1 ) {
            if ( minDigits < 1 ) {
                minDigits = 1;
            }
            if ( n == 0 ) {
                return new string( '0', minDigits );
            }
            var s = "";
            if ( b < 2 || b > NumberBaseChars.Length ) {
                return s;
            }
            var neg = false;
            if ( b == 10 && n < 0 ) {
                neg = true;
                n = -n;
            }
            var N = ( uint )n;
            var B = ( uint )b;
            while ( ( N > 0 ) | ( minDigits-- > 0 ) ) {
                s = NumberBaseChars[ ( int )( N % B ) ] + s;
                N /= B;
            }
            if ( neg ) {
                s = "-" + s;
            }
            //return s;
            return s;
        }

        public static Boolean IsNegative( this Single value ) {
            return value < 0.0f;
        }

        public static Boolean IsPositive( this Single value ) {
            return value > 0.0f;
        }

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source"> Source value. </param>
        /// <param name="target"> Target value. </param>
        /// <param name="amount"> Value between 0 and 1 indicating the weight of value2. </param>
        public static Single Lerp( this Single source, Single target, Single amount ) {
            return source + ( target - source ) * amount;
        }

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source"> Source value. </param>
        /// <param name="target"> Target value. </param>
        /// <param name="amount"> Value between 0 and 1 indicating the weight of value2. </param>
        public static Double Lerp( this  Double source, Double target, Single amount ) {
            return source + ( target - source ) * amount;
        }

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source"> Source value. </param>
        /// <param name="target"> Target value. </param>
        /// <param name="amount"> Value between 0 and 1 indicating the weight of value2. </param>
        public static UInt64 Lerp( this  UInt64 source, UInt64 target, Single amount ) {
            return ( UInt64 )( source + ( ( target - source ) * amount ) );
        }

        /// <summary>
        ///     Linearly interpolates between two values.
        /// </summary>
        /// <param name="source"> Source value. </param>
        /// <param name="target"> Target value. </param>
        /// <param name="amount"> Value between 0 and 1 indicating the weight of value2. </param>
        public static UInt32 Lerp( this  UInt32 source, UInt32 target, Single amount ) {
            return ( UInt32 )( source + ( ( target - source ) * amount ) );
        }

        public static Double LogFactorial( this  int n ) {
            if ( n < 0 ) {
                throw new ArgumentOutOfRangeException();
            }
            if ( n <= 254 ) {
                return Logfactorialtable[ n ];
            }
            var x = n + 1d;
            return ( x - 0.5 ) * Math.Log( x ) - x + 0.5 * Math.Log( 2 * Math.PI ) + 1.0 / ( 12.0 * x );
        }

        /// <summary>
        ///     compute log(1+x) without losing precision for small values of x
        /// </summary>
        /// <param name="x"> </param>
        /// <returns> </returns>
        public static Double LogOnePlusX( this Double x ) {
            if ( x <= -1.0 ) {
                throw new ArgumentOutOfRangeException( "x", String.Format( "Invalid input argument: {0}", x ) );
            }

            if ( Math.Abs( x ) > 1e-4 ) {
                // x is large enough that the obvious evaluation is OK
                return Math.Log( 1.0 + x );
            }

            // Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3
            // Since |x| < 10^-4, |x|^3 < 10^-12, relative error less than 10^-8
            return ( -0.5 * x + 1.0 ) * x;
        }

        public static Boolean Near( this Double number, Double target ) {
            return Math.Abs( number - target ) <= Double.Epsilon;
        }

        public static Boolean Near( this Single number, Single target ) {
            return Math.Abs( number - target ) <= Single.Epsilon;
        }

        public static Boolean Near( this  Decimal number, Decimal target ) {
            return Math.Abs( number - target ) <= EpsilonDecimal;
        }

        public static Boolean Near( this BigRational number, BigRational target ) {
            var difference = number - target;
            if ( difference < BigRational.Zero ) {
                difference = -difference;
            }
            return difference <= EpsilonDecimal;
        }

        public static Boolean Near( this BigInteger number, BigInteger target ) {
            var difference = number - target;
            return BigInteger.Zero == difference;
        }

        /// <summary>
        ///     <para>Attempt to parse a fraction from a string.</para>
        /// </summary>
        /// <example>
        ///     " 1234 / 346 "
        /// </example>
        /// <param name="numberString"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Boolean TryParse( [CanBeNull] this String numberString, out BigRational result ) {
            result = BigRational.Zero;

            if ( null == numberString ) {
                return false;
            }

            numberString = numberString.Trim();
            if ( numberString.IsNullOrEmpty() ) {
                return false;
            }

            var parts = numberString.Split( '/' ).ToList();
            if ( parts.Count() != 2 ) {
                return false;
            }

            var top = parts.TakeFirst();
            if ( String.IsNullOrWhiteSpace( top ) ) {
                return false;
            }
            top = top.Trim();

            var bottom = parts.TakeLast();
            if ( String.IsNullOrWhiteSpace( bottom ) ) {
                return false;
            }

            parts.Should().BeEmpty();
            if ( parts.Count > 0 ) {
                return false;
            }

            BigInteger numerator;
            BigInteger.TryParse( top, out numerator );

            BigInteger denominator;
            BigInteger.TryParse( bottom, out denominator );

            result = new BigRational( numerator, denominator );

            return true;
        }

        public static Boolean Near( this UInt64 number, UInt64 target ) {
            return number - target <= UInt64.MinValue;
        }

        public static Boolean Near( this UBigInteger number, UBigInteger target ) {
            return number - target <= UBigInteger.Epsilon;
        }

        public static Double Nested( this Double x ) {
            return Math.Sqrt( x * 100.0 ) / 100.0d;
        }

        public static Single Nested( this Single x ) {
            return ( Single )( Math.Sqrt( x * 100.0 ) / 100.0f );
        }

        [Obsolete]
        public static int Nested( this int x ) {
            return ( int )Math.Sqrt( x );
        }

        public static UInt64 OneHundreth( this UInt64 x ) {
            return x / 100;
        }

        public static UInt64 OneQuarter( this UInt64 x ) {
            return x / 4;
        }

        public static Single OneQuarter( this Single x ) {
            return x / 4.0f;
        }

        public static UInt64 OneTenth( this UInt64 x ) {
            return x / 10;
        }

        public static Single OneThird( this Single x ) {
            return x / 3.0f;
        }

        [Test]
        public static Boolean PassProbabilityTest() {
            var lower = new List<Boolean>();
            var probability = -0.33f;
            for ( var i = 0 ; i < 1048576 * 10 ; i++ ) {
                lower.Add( probability.Probability() );
            }

            var higher = new List<Boolean>();
            probability = 0.123f;
            for ( var i = 0 ; i < 1048576 * 10 ; i++ ) {
                higher.Add( probability.Probability() );
            }

            lower.RemoveAll( b => !b );
            higher.RemoveAll( b => !b );

            return higher.Count > lower.Count;
        }

        public static int Percent( this int x, Single percent ) {
            return ( int )( ( x * percent ) / 100.0f );
        }

        public static Single Percent( this Single x, Single percent ) {
            return ( x * percent ) / 100.0f;
        }

        public static Double Percent( this Double x, Double percent ) {
            return ( x * percent ) / 100.0;
        }

        public static UInt64 Percent( this UInt64 x, Single percent ) {
            return ( UInt64 )( ( x * percent ) / 100.0f );
        }

        public static Double Phi( this Double x ) {
            // constants
            const Double a1 = 0.254829592;
            const Double a2 = -0.284496736;
            const Double a3 = 1.421413741;
            const Double a4 = -1.453152027;
            const Double a5 = 1.061405429;
            const Double p = 0.3275911;

            // Save the sign of x
            var sign = x < 0 ? -1 : 1;
            x = Math.Abs( x ) / Math.Sqrt( 2.0 );

            // A&S formula 7.1.26
            var t = 1.0 / ( 1.0 + p * x );
            var y = 1.0 - ( ( ( ( ( a5 * t + a4 ) * t ) + a3 ) * t + a2 ) * t + a1 ) * t * Math.Exp( -x * x );

            return 0.5 * ( 1.0 + sign * y );
        }

        /*
                public static string Pluralize( this string value, int count ) {
                    if ( count == 1 ) {
                        return value;
                    }
                    return Pluralizer.Instance.Pluralize(
                        .CreateService( new CultureInfo( "en-US" ) )
                        .Pluralize( value );
                }
        */

        public static IEnumerable<int> Primes( int max ) {
            yield return 2;
            var found = new List<int> {
                                            3
                                        };
            var candidate = 3;
            while ( candidate <= max ) {
                var candidate1 = candidate;
                var candidate2 = candidate;
                if ( found.TakeWhile( prime => prime * prime <= candidate1 ).All( prime => candidate2 % prime != 0 ) ) {
                    found.Add( candidate );
                    yield return candidate;
                }
                candidate += 2;
            }
        }

        /// <summary>
        ///     <para>Returns the sum of all <see cref="BigInteger" />.</para>
        /// </summary>
        /// <param name="bigIntegers"></param>
        /// <returns></returns>
        public static BigInteger Sum( [NotNull] this IEnumerable<BigInteger> bigIntegers ) {
            return bigIntegers.Aggregate( BigInteger.Zero, ( current, bigInteger ) => current + bigInteger );
        }

        /// <summary>
        ///     Returns true if this probability happens.
        /// </summary>
        /// <param name="probability"> </param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this UInt64 probability, UInt64 maxvalue ) {
            var chance = Randem.Next( 0, maxvalue );
            return probability >= chance;
        }

        /// <summary>
        ///     Returns true if this probability happens.
        /// </summary>
        /// <param name="probability"> </param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this UInt32 probability, UInt32 maxvalue ) {
            var chance = Randem.Next( 0, maxvalue );
            return probability >= chance;
        }

        /// <summary>
        ///     Returns true if this probability happens.
        /// </summary>
        /// <param name="probability"> </param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this UInt16 probability, UInt16 maxvalue ) {
            var chance = Randem.Next( 0, maxvalue );
            return probability >= chance;
        }

        /// <summary>
        ///     Returns true if this probability happens.
        /// </summary>
        /// <param name="probability"> </param>
        /// <param name="maxvalue"></param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this int probability, int maxvalue ) {
            var chance = Randem.Next( minValue: 0, maxValue: maxvalue );
            return probability >= chance;
        }

        /// <summary>
        ///     Returns true <b>if</b> this probability happens.
        /// </summary>
        /// <param name="probability"> </param>
        /// <remarks>the higher the value of P, the more often this function should return true.</remarks>
        public static Boolean Probability( this Single probability ) {
            var chance = Randem.NextSingle( min: 0.0f, max: 1.0f );
            return probability >= chance;

            // if P is -0.1 then
            // a chance of 0.01 will return false;
            // a chance of 0.90 will return false

            // if P is 0.1 then
            // a chance of 0.01 will return true
            // a chance of 0.05 will return true
            // a chance of 0.09 will return true
            // a chance of 0.10 will return false
            // a chance of 0.50 will return false
            // a chance of 0.90 will return false

            // if P is 0.89 then
            // a chance of 0.01 will return true
            // a chance of 0.05 will return true
            // a chance of 0.09 will return true
            // a chance of 0.10 will return true
            // a chance of 0.50 will return true
            // a chance of 0.85 will return true
            // a chance of 0.89 will return true
            // a chance of 0.90 will return false
        }

        /// <summary>
        ///     Smooths a value to between 0 and 1.
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        public static Double Sigmoid0To1( this Double value ) {
            return 1.0D / ( 1.0D + Math.Exp( -value ) );
        }

        /// <summary>
        ///     Smooths a value to between -1 and 1.
        /// </summary>
        /// <param name="value"> </param>
        /// <returns> </returns>
        /// <seealso cref="http://www.wolframalpha.com/input/?i=1+-+%28+2+%2F+%281+%2B+Exp%28+v+%29+%29+%29%2C+v+from+-10+to+10" />
        public static Double SigmoidNeg1To1( this Double value ) {
            return 1.0D - ( 2.0D / ( 1.0D + Math.Exp( value ) ) );
        }

        /// <summary>
        ///     Return the integer part and the fraction parts of a <see cref="Decimal" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tuple<Decimal, Decimal> Split( this  Decimal value ) {
            var parts = value.ToString( "R" ).Split( '.' );
            var result = new Tuple<Decimal, Decimal>( Decimal.Parse( parts[ 0 ] ), Decimal.Parse( "0." + parts[ 1 ] ) );
            return result;
        }

        /// <summary>
        ///     Return the integer part and the fraction parts of a <see cref="Double" />.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Tuple<double, double> Split( this Double value ) {
            var parts = value.ToString( "R" ).Split( '.' );
            return new Tuple<double, double>( Double.Parse( parts[ 0 ] ), Double.Parse( "0." + parts[ 1 ] ) );
        }

        /// <summary>
        ///     Split one <see cref="UInt64" /> value into two <see cref="UInt32" /> values.  Use <see cref="Combine" /> for the
        ///     reverse.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        public static void Split( this UInt64 value, out UInt32 high, out UInt32 low ) {
            high = ( UInt32 )( value >> 32 );
            low = ( UInt32 )( value & UInt32.MaxValue );
        }

        public static Double StandardDeviation( [NotNull] this IEnumerable<double> values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            var doubles = values as Double[] ?? values.ToArray();
            var avg = doubles.Average();
            return Math.Sqrt( doubles.Average( v => Math.Pow( v - avg, 2 ) ) );
        }

        public static Decimal StandardDeviation( [NotNull] this IEnumerable<Decimal> values ) {
            if ( values == null ) {
                throw new ArgumentNullException( "values" );
            }
            var decimals = values as Decimal[] ?? values.ToArray();
            var avg = decimals.Average();
            return ( Decimal )Math.Sqrt( decimals.Average( v => Math.Pow( ( Double )( v - avg ), 2 ) ) );
        }

        public static int ThreeFourths( this int x ) {
            var result = ( 3.0 * x ) / 4.0;
            return result < 1.0 ? 1 : ( int )result;
        }

        public static UInt64 ThreeQuarters( this UInt64 x ) {
            return ( 3 * x ) / 4;
        }

        public static Single ThreeQuarters( this Single x ) {
            return ( 3.0f * x ) / 4.0f;
        }

        public static Double ThreeQuarters( this Double x ) {
            return ( 3.0d * x ) / 4.0d;
        }

        public static IEnumerable<int> Through( this int startValue, int end ) {
            int offset;
            if ( startValue < end ) {
                offset = 1;
            }
            else {
                offset = -1;
            }

            for ( var i = startValue ; i != end + offset ; i += offset ) {
                yield return i;
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 102.To(204)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<byte> To( this Byte start, Byte end, Byte step = 1 ) {
            if ( step <= 1 ) {
                step = 1;
            }

            if ( start <= end ) {
                for ( var b = start ; b <= end ; b += step ) {
                    yield return b;
                    if ( b == Byte.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var b = start ; b >= end ; b -= step ) {
                    yield return b;
                    if ( b == Byte.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> To( this int start, UInt64 end, UInt64 step = 1 ) {
            if ( start < 0 ) {
                throw new ArgumentOutOfRangeException( "start", "'low' must be equal to or greater than zero." );
            }

            if ( step == 0UL ) {
                step = 1UL;
            }

            var reFrom = ( UInt64 )start; //bug here is the bug if from is less than zero

            if ( start <= ( Decimal )end ) {
                for ( var ul = reFrom ; ul <= end ; ul += step ) {
                    yield return ul;
                    if ( ul == UInt64.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = reFrom ; ul >= end ; ul -= step ) {
                    yield return ul;
                    if ( ul == UInt64.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<int> To( this int start, int end, int step = 1 ) {
            if ( start < 0 ) {
                throw new ArgumentOutOfRangeException( "start", "'low' must be equal to or greater than zero." );
            }

            if ( step == 0 ) {
                step = 1;
            }

            var reFrom = start; //bug here is the bug if from is less than zero

            if ( start <= end ) {
                for ( var ul = reFrom ; ul <= end ; ul += step ) {
                    yield return ul;
                    if ( ul == int.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = reFrom ; ul >= end ; ul -= step ) {
                    yield return ul;
                    if ( ul == int.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="end"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> To( this UInt64 from, UInt64 end, UInt64 step = 1 ) {
            if ( step == 0UL ) {
                step = 1UL;
            }

            if ( @from <= end ) {
                for ( var ul = @from ; ul <= end ; ul += step ) {
                    yield return ul;
                    if ( ul == UInt64.MaxValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
            else {
                for ( var ul = @from ; ul >= end ; ul -= step ) {
                    yield return ul;
                    if ( ul == UInt64.MinValue ) {
                        yield break;
                    } //special case to deal with overflow
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<BigInteger> To( this BigInteger from, BigInteger to, UInt64 step = 1 ) {
            if ( step == 0UL ) {
                step = 1UL;
            }

            if ( @from <= to ) {
                for ( var ul = @from ; ul <= to ; ul += step ) {
                    yield return ul;
                }
            }
            else {
                for ( var ul = @from ; ul >= to ; ul -= step ) {
                    yield return ul;
                }
            }
        }

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<BigInteger> To( this long from, BigInteger to, UInt64 step = 1 ) {
            if ( step == 0UL ) {
                step = 1UL;
            }

            BigInteger reFrom = @from;

            if ( reFrom <= to ) {
                for ( var ul = reFrom ; ul <= to ; ul += step ) {
                    yield return ul;
                }
            }
            else {
                for ( var ul = reFrom ; ul >= to ; ul -= step ) {
                    yield return ul;
                }
            }
        }

        ///// <summary>
        /////     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        ///// </summary>
        ///// <param name="start"></param>
        ///// <param name="to"></param>
        ///// <param name="step"></param>
        ///// <returns></returns>
        ///// <remarks>A !huge! number will act like an infinite loop.</remarks>
        //public static IEnumerable< BigInteger > To( this int start, BigInteger to, UInt64 step = 1 ) {
        //    if ( step == 0UL ) {
        //        step = 1UL;
        //    }

        //    BigInteger reFrom = start;

        //    if ( reFrom <= to ) {
        //        for ( var ul = reFrom; ul <= to; ul += step ) {
        //            yield return ul;
        //        }
        //    }
        //    else {
        //        for ( var ul = reFrom; ul >= to; ul -= step ) {
        //            yield return ul;
        //        }
        //    }
        //}

        /// <summary>
        ///     Example: foreach (var i in 10240.To(20448)) { Console.WriteLine(i); }
        /// </summary>
        /// <param name="start"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<BigDecimal> To( this int start, BigDecimal to, BigDecimal step ) {
            if ( step < 0 ) {
                step = 1;
            }

            BigDecimal reFrom = start;

            if ( reFrom <= to ) {
                for ( var ul = reFrom ; ul <= to ; ul = ul + step ) {
                    yield return ul;
                }
            }
            else {
                for ( var ul = reFrom ; ul >= to ; ul -= step ) {
                    yield return ul;
                }
            }
        }

        /// <summary>
        ///     Return each <see cref="DateTime" /> between <paramref name="from" /> and <paramref name="to" />, stepped by a
        ///     <see cref="TimeSpan" /> (<paramref name="step" />).
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <remarks>//TODO Untested code!</remarks>
        /// <example>
        ///     var now = DateTime.UtcNow;
        ///     var then = now.AddMinutes( 10 );
        ///     var minutes = now.To( then, TimeSpan.FromMinutes( 1 ) );
        ///     foreach ( var dateTime in minutes ) { Console.WriteLine( dateTime ); }
        /// </example>
        public static IEnumerable<DateTime> To( this DateTime from, DateTime to, TimeSpan? step = null ) {
            if ( !step.HasValue ) {
                try {
                    var diff = @from <= to ? to.Subtract( @from ) : @from.Subtract( @from );

                    if ( diff.TotalDays > 0 ) {
                        step = TimeSpan.FromDays( 1 );
                    }
                    else if ( diff.TotalHours > 0 ) {
                        step = TimeSpan.FromHours( 1 );
                    }
                    else if ( diff.TotalMinutes > 0 ) {
                        step = TimeSpan.FromMinutes( 1 );
                    }
                    else if ( diff.TotalSeconds > 0 ) {
                        step = TimeSpan.FromSeconds( 1 );
                    }
                    else {
                        step = TimeSpan.FromMilliseconds( 1 );
                    }
                }
                catch ( ArgumentOutOfRangeException ) {
                    step = TimeSpan.MaxValue;
                }
            }

            if ( @from <= to ) {
                for ( var dt = @from ; dt <= to ; dt += step.Value ) {
                    yield return dt;
                }
            }
            else {
                for ( var dt = @from ; dt >= to ; dt -= step.Value ) {
                    yield return dt;
                }
            }
        }

        public static IEnumerable<double> To( this Double start, Double end ) {
            var count = end - start + 1;
            for ( var i = 0 ; i < count ; i++ ) {
                yield return start + i;
            }
        }

        public static IEnumerable<Decimal> To( this  Decimal start, Decimal end ) {
            var count = end - start + 1;
            for ( var i = 0 ; i < count ; i++ ) {
                yield return start + i;
            }
        }

        public static string ToHex( this IEnumerable<byte> input ) {
            if ( input == null ) {
                throw new ArgumentNullException( "input" );
            }
            return input.Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );
        }

        public static string ToHex( this uint value ) {
            return BitConverter.GetBytes( value ).Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );
        }

        public static string ToHex( this ulong value ) {
            return BitConverter.GetBytes( value ).Aggregate( "", ( current, b ) => current + b.ToString( "x2" ) );
        }

        public static Single Twice( this Single x ) {
            return x * 2.0f;
        }

        public static Single Squared( this Single number ) {
            return number * number;
        }

        public static Single Cubed( this Single number ) {
            return number * number * number;
        }

        public static Double Twice( this Double number ) {
            return number * 2d;
        }

        public static Double Squared( this Double number ) {
            return number * number;
        }

        public static Double Cubed( this Double number ) {
            return number * number * number;
        }

        public static Decimal Twice( this  Decimal number ) {
            return number * 2m;
        }

        public static Decimal Squared( this  Decimal number ) {
            return number * number;
        }

        public static Decimal Cubed( this  Decimal number ) {
            return number * number * number;
        }

        public static Boolean IsPowerOfTwo( this int number ) {
            return ( number & -number ) == number;
        }

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="Decimal.Zero" />, then return
        ///         <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static Decimal IfLessThanZeroThenZero( this  Decimal number ) {
            return number < Decimal.Zero ? Decimal.Zero : number;
        }

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="BigInteger.Zero" />, then return
        ///         <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static BigInteger IfLessThanZeroThenZero( this BigInteger number ) {
            return number < BigInteger.Zero ? BigInteger.Zero : number;
        }

        /// <summary>
        ///     <para>
        ///         If the <paramref name="number" /> is less than <see cref="BigDecimal.Zero" />, then return
        ///         <see cref="Decimal.Zero" />.
        ///     </para>
        ///     <para>Otherwise return the <paramref name="number" />.</para>
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static BigDecimal IfLessThanZeroThenZero( this BigDecimal number ) {
            return number < BigDecimal.Zero ? BigDecimal.Zero : number;
        }

        /// <summary>
        ///     Add two <see cref="UInt64" /> without the chance of "throw new ArgumentOutOfRangeException( "amount",
        ///     String.Format( "Values {0} and {1} are loo large to handle.", amount, uBigInteger ) );"
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static UInt64 AddWithoutOverFlow( this UInt64 left, UInt64 right ) {
            var integer = new UBigInteger( left ) + new UBigInteger( right );
            if ( integer >= new UBigInteger( UInt64.MaxValue ) ) {
                return UInt64.MaxValue;
            }
            return ( UInt64 )integer;
        }

        /// <summary>
        ///     Allow <paramref name="left" /> to increase or decrease by a signed number;
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static UInt64 AddWithoutOverFlow( this UInt64 left, long right ) {
            var integer = new BigInteger( left ) + new BigInteger( right );
            if ( integer >= new UBigInteger( UInt64.MaxValue ) ) {
                return UInt64.MaxValue;
            }
            if ( integer < UInt64.MinValue ) {
                return UInt64.MinValue;
            }
            return ( UInt64 )integer;
        }

        /// <summary>
        ///     Subtract <paramref name="right" /> away from <paramref name="left" /> without the chance of "throw new
        ///     ArgumentOutOfRangeException( "amount", String.Format( "Values {0} and {1} are loo small to handle.", amount,
        ///     uBigInteger ) );"
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static UInt64 SubtractWithoutUnderFlow( this UInt64 left, UInt64 right ) {
            var integer = new UBigInteger( left ) - new UBigInteger( right );
            if ( integer < new UBigInteger( UInt64.MinValue ) ) {
                return UInt64.MinValue;
            }
            return ( UInt64 )integer;
        }

        /// <summary>
        ///     Add in the votes from another <see cref="Votally" />.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void Add( [NotNull] this Votally left, [NotNull] Votally right ) {
            if ( left == null ) {
                throw new ArgumentNullException( "left" );
            }
            if ( right == null ) {
                throw new ArgumentNullException( "right" );
            }
            left.VoteYes( right.Yes );
            left.VoteNo( right.No );
        }

        public static Double Slope( [NotNull] List<TimeProgression> data ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            var averageX = data.Average( d => d.MillisecondsPassed );
            var averageY = data.Average( d => d.Progress );

            return data.Sum( d => ( d.MillisecondsPassed - averageX ) * ( d.Progress - averageY ) ) / data.Sum( d => Math.Pow( d.MillisecondsPassed - averageX, 2 ) );
        }

        public static Double Intercept( [NotNull] List<TimeProgression> data ) {
            if ( data == null ) {
                throw new ArgumentNullException( "data" );
            }
            var slope = Slope( data );
            return data.Average( d => d.Progress ) - slope * data.Average( d => d.MillisecondsPassed );
        }

        public static Boolean TrySplitDecimal( this  Decimal value, out BigInteger beforeDecimalPoint, out BigInteger afterDecimalPoint ) {
            var theString = value.ToString();
            if ( !theString.Contains( "." ) ) {
                theString += ".0";
            }
            var split = theString.Split( '.' );
            split.Should().HaveCount( expected: 2, because: "otherwise invalid" );

            afterDecimalPoint = BigInteger.Zero;

            return BigInteger.TryParse( split[ 0 ], out beforeDecimalPoint ) && BigInteger.TryParse( split[ 1 ], out afterDecimalPoint );
        }

        [CanBeNull]
        public static String ToScientificString( BigDecimal value ) {
            var bob = new clojure.lang.BigDecimal( value.Significand.ToBigInteger(), value.Exponent );
            return bob.ToScientificString();
        }

        /// <summary>
        /// Convert from <see cref="clojure.lang.BigDecimal"/> into a <see cref="BigDecimal"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigDecimal ToBigDecimal( this clojure.lang.BigDecimal value ) {
            var result = new BigDecimal( significand: value.Coefficient.ToBigInteger(), exponent: value.Exponent );
            return result;
        }

        /// <summary>
        /// Convert from <see cref="clojure.lang.BigInteger"/> into a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger ToBigInteger( this clojure.lang.BigInteger value ) {
            var result = BigInteger.Parse( value.ToString() );
            return result;
        }

        /// <summary>
        /// Convert from <see cref="BigInteger"/> into a <see cref="clojure.lang.BigInteger"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static clojure.lang.BigInteger ToBigInteger( this BigInteger value ) {
            var result = clojure.lang.BigInteger.Parse( value.ToString() );
            return result;
        }

        /// <summary>
        ///     Create a BigDecimal from a string representation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigDecimal ToBigDecimal( this String value ) {
            var bigDecimal = clojure.lang.BigDecimal.Parse( value.ToCharArray(), 0, value.Length );
            return bigDecimal.ToBigDecimal();
        }

        /// <summary>
        ///     <para>Convert most of a <see cref="BigDecimal"/> into a <see cref="BigInteger"/></para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BigInteger ToBigInteger( this BigDecimal value ) {
            var scaleDivisor = BigInteger.Pow( 10, value.Exponent );
            var scaledValue = BigInteger.Divide( value.Mantissa, scaleDivisor );
            return scaledValue;
        }

        /// <summary>
        /// Combine two bytes into one <see cref="UInt16"/>.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static UInt16 CombineBytes( this Byte low, Byte high ) {
            return BitConverter.ToUInt16( BitConverter.IsLittleEndian ? new[] { high, low } : new[] { low, high }, 0 );
        }

        /// <summary>
        /// Combine two bytes into one <see cref="UInt16"/> with little endianess.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        /// <seealso cref="CombineTwoBytesHighEndianess"/>
        public static UInt16 CombineTwoBytesLittleEndianess( this Byte low, Byte high ) {
            return ( UInt16 )( low + ( high << 8 ) );    //BUG is this backwards?
        }

        /// <summary>
        /// Combine two bytes into one <see cref="UInt16"/> with little endianess.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        /// <seealso cref="CombineTwoBytesLittleEndianess"/>
        public static UInt16 CombineTwoBytesHighEndianess( this Byte low, Byte high ) {
            return ( UInt16 )( high + ( low << 8 ) );    //BUG is this backwards?
        }

        /// <summary>
        /// <para>In mathematics, the geometric mean is a type of mean or average, which indicates the central tendency or typical value of a set of numbers by using the product of their values (as opposed to the arithmetic mean which uses their sum).</para>
        ///  <para>The geometric mean is defined as the nth root of the product of n numbers.</para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <seealso cref="http://wikipedia.org/wiki/Geometric_mean"/>
        public static double GeometricMean( this IEnumerable<Double> data, int items ) {
            var aggregate = data.Aggregate( 1.0, ( current, d ) => current * d );
            return Math.Pow( aggregate, ( 1.0 / items ) );
        }

        /// <summary>
        /// <para>In mathematics, the geometric mean is a type of mean or average, which indicates the central tendency or typical value of a set of numbers by using the product of their values (as opposed to the arithmetic mean which uses their sum).</para>
        ///  <para>The geometric mean is defined as the nth root of the product of n numbers.</para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <seealso cref="http://wikipedia.org/wiki/Geometric_mean"/>
        public static Decimal GeometricMean( this IEnumerable<Decimal> data, int items ) {
            var aggregate = data.Aggregate( 1.0m, ( current, d ) => current * d );
            return ( Decimal )Math.Pow( ( Double )aggregate, ( Double )( 1.0m / items ) );   //BUG possible conversion errors here
        }

        /// <summary>
        /// <para>In mathematics, the geometric mean is a type of mean or average, which indicates the central tendency or typical value of a set of numbers by using the product of their values (as opposed to the arithmetic mean which uses their sum).</para>
        ///  <para>The geometric mean is defined as the nth root of the product of n numbers.</para>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <seealso cref="http://wikipedia.org/wiki/Geometric_mean"/>
        public static BigDecimal GeometricMean( this IEnumerable<BigDecimal> data, int items ) {
            var aggregate = data.Aggregate( BigDecimal.One, ( current, d ) => current * d );
            return BigDecimal.Pow( ( double )aggregate, 1.0 / items );   //BUG possible conversion errors here
        }

        public static Double SquareRootOfProducts( this IEnumerable<Double> data ) {
            var aggregate = data.Aggregate( 1.0, ( current, d ) => current * d );
            return Math.Sqrt( aggregate );
        }

        public static Double Root( this Double x, Double root ) {
            return Math.Pow( x, 1.0 / root );
        }

        public static Double Root( this Decimal x, Decimal root ) {
            return Math.Pow( ( double )x, ( Double )( 1.0m / root ) );//BUG possible conversion errors here
        }

        public static Decimal SquareRootOfProducts( this IEnumerable<Decimal> data ) {
            var aggregate = data.Aggregate( 1.0m, ( current, d ) => current * d );
            return ( Decimal )Math.Sqrt( ( Double )aggregate );
        }

        /// <summary>
        /// <see cref="Decimal"/> raised to the nth power.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <seealso cref="http://stackoverflow.com/questions/429165/raising-a-Decimal-to-a-power-of-Decimal"/>
        public static Decimal Pow( Decimal x, UInt32 n ) {
            var a = 1m;
            var e = new BitArray( BitConverter.GetBytes( n ) );

            for ( var i = e.Count - 1 ; i >= 0 ; --i ) {
                a *= a;
                if ( e[ i ] ) {
                    a *= x;
                }
            }
            return a;
        }
    }

    // ReSharper restore UnusedMember.Global
}
