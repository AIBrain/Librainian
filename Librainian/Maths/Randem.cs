// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Randem.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "Randem.cs" was last formatted by Protiguous on 2019/08/08 at 8:34 AM.

namespace Librainian.Maths {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections.Extensions;
    using ComputerSystem;
    using Extensions;
    using Internet.RandomOrg;
    using JetBrains.Annotations;
    using Linguistics;
    using Logging;
    using Measurement.Spatial;
    using Measurement.Time;
    using Numbers;
    using OperatingSystem.Compression;
    using Parsing;
    using RandomNameGeneratorLibrary;
    using Ranges;

    public static partial class Randem {

        /// <summary>
        ///     Provide to each thread its own <see cref="Random" /> with a random seed.
        /// </summary>
        [NotNull]
        private static ThreadLocal<Lazy<Random>> ThreadSafeRandom { get; } = new ThreadLocal<Lazy<Random>>( valueFactory: () =>
            new Lazy<Random>( valueFactory: () => new Random( DateTime.Now.Ticks.GetHashCode() ^ Thread.CurrentThread.ManagedThreadId.GetHashCode() ) ) );

        internal static ConcurrentStack<Int32> PollResponses { get; } = new ConcurrentStack<Int32>();

        [NotNull]
        public static ConcurrentDictionary<Type, String[]> EnumDictionary { get; } = new ConcurrentDictionary<Type, String[]>();

        [NotNull]
        public static Lazy<PersonNameGenerator> Names { get; } = new Lazy<PersonNameGenerator>( valueFactory: () => new PersonNameGenerator() );

        /// <summary>
        ///     <para>More cryptographically strong than <see cref="Random" />.</para>
        /// </summary>
        [NotNull]
        public static ThreadLocal<RandomNumberGenerator> RNG { get; } =
            new ThreadLocal<RandomNumberGenerator>( valueFactory: () => new RNGCryptoServiceProvider(), trackAllValues: true );

        /// <summary>
        ///     A Double-sized byte buffer per-thread.
        /// </summary>
        private static readonly ThreadLocal<Byte[]> LocalByteBuffer = new ThreadLocal<Byte[]>( valueFactory: () => new Byte[ sizeof( Double ) ], trackAllValues: true );

        /// <summary>
        ///     A thread-local (threadsafe) <see cref="Random" />.
        /// </summary>
        [NotNull]
        private static Random Instance() => ThreadSafeRandom.Value.Value;

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private static Char NextChar( [NotNull] this Char[] range ) {
            if ( range == null ) {
                throw new ArgumentNullException( nameof( range ) );
            }

            return range[ 0.Next( maxValue: range.Length ) ];
        }

        /// <summary>
        ///     Returns a decimal between 0 and 1.
        /// </summary>
        /// <returns></returns>
        public static Decimal BranchRatio() => NextDecimal();

        /// <summary>
        ///     Chooses a random element in the given collection <paramref name="items" />.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="items">The collection of items to choose a random element from.</param>
        /// <returns>A randomly chosen element in the given collection <paramref name="items" />.</returns>
        public static T Choose<T>( [NotNull] params T[] items ) => items[ items.Length.NextInt() ];

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a ) => a;

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b ) {
            switch ( 2.NextInt() ) {
                case 0: return a;

                default: return b;
            }
        }

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c ) {
            switch ( 3.NextInt() ) {
                case 0: return a;

                case 1: return b;

                default: return c;
            }
        }

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d ) {
            switch ( 4.NextInt() ) {
                case 0: return a;

                case 1: return b;

                case 2: return c;

                default: return d;
            }
        }

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <param name="e">The fifth item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d, T e ) {
            switch ( 5.NextInt() ) {
                case 0: return a;

                case 1: return b;

                case 2: return c;

                case 3: return d;

                default: return e;
            }
        }

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <param name="e">The fifth item.</param>
        /// <param name="f">The sixth item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d, T e, T f ) {
            var index = 6.NextInt();

            switch ( index ) {
                case 0: return a;

                case 1: return b;

                case 2: return c;

                case 3: return d;

                case 4: return e;

                default: return f;
            }
        }

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <param name="e">The fifth item.</param>
        /// <param name="f">The sixth item.</param>
        /// <param name="g">The seventh item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d, T e, T f, T g ) {
            var index = 7.NextInt();

            switch ( index ) {
                case 0: return a;

                case 1: return b;

                case 2: return c;

                case 3: return d;

                case 4: return e;

                case 5: return f;

                default: return g;
            }
        }

        /// <summary>
        ///     Chooses a random element in the given set of items.
        /// </summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <param name="e">The fifth item.</param>
        /// <param name="f">The sixth item.</param>
        /// <param name="g">The seventh item.</param>
        /// <param name="h">The eigth item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d, T e, T f, T g, T h ) {
            var index = 8.NextInt();

            switch ( index ) {
                case 0: return a;

                case 1: return b;

                case 2: return c;

                case 3: return d;

                case 4: return e;

                case 5: return f;

                case 6: return g;

                default: return h;
            }
        }

        [MethodImpl( methodImplOptions: MethodImplOptions.NoInlining )]
        public static Double DoBusyWork( this UInt64 iterations ) {
            Double work = 0;

            for ( var i = 0ul; i < iterations; i++ ) {
                work += 1001.671;
            }

            return work;
        }

        [NotNull]
        public static List<Int32> GenerateRandom( Int32 count, Int32 min, Int32 max ) {

            // initialize set S to empty for J := N-M + 1 to N do T := RandInt(1, J) if T is not in S then insert T in S else insert J in S
            //
            // adapted for C# which does not have an inclusive Next(..) and to make it from configurable range not just 1.

            if ( max <= min || count < 0 ||

                 // max - min > 0 required to avoid overflow
                 count > max - min && max - min > 0 ) {

                // need to use 64-bit to support big ranges (negative min, positive max)
                throw new ArgumentOutOfRangeException( $"Range {min} to {max} ({( Int64 ) max - min} values), or count {count} is illegal." );
            }

            // generate count random values.
            var candidates = new HashSet<Int32>();

            // start count values before max, and end at max
            for ( var top = max - count; top < max; top++ ) {

                // May strike a duplicate. Need to add +1 to make inclusive generator
                // +1 is safe even for MaxVal max value because top < max
                if ( !candidates.Add( item: Instance().Next( minValue: min, maxValue: top + 1 ) ) ) {

                    // collision, add inclusive max. which could not possibly have been added before.
                    candidates.Add( item: top );
                }
            }

            // load them in to a list, to sort
            var result = candidates.ToList();

            // shuffle the results because HashSet has messed with the order, and the algorithm does not produce random-ordered results (e.g. max-1 will never be the first value)
            for ( var i = result.Count - 1; i > 0; i-- ) {
                var k = Instance().Next( maxValue: i + 1 );
                var tmp = result[ index: k ];
                result[ index: k ] = result[ index: i ];
                result[ index: i ] = tmp;
            }

            return result;
        }

        public static Char GetChar( [NotNull] this RandomNumberGenerator rng ) {
            if ( rng == null ) {
                throw new ArgumentNullException( nameof( rng ) );
            }

            var data = new Byte[ sizeof( Char ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToChar( data, startIndex: 0 );
        }

        public static Double GetDouble( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( Double ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToDouble( data, startIndex: 0 );
        }

        public static Int16 GetInt16( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( Int16 ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToInt16( data, startIndex: 0 );
        }

        public static Int32 GetInt32( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( Int32 ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToInt32( data, startIndex: 0 );
        }

        public static Int64 GetInt64( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( Int64 ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToInt64( data, startIndex: 0 );
        }

        /// <summary>
        ///     memoize?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static String[] GetNames<T>() {
            var key = typeof( T );

            if ( EnumDictionary.TryGetValue( key, out var values ) ) {
                return values;
            }

            values = Enum.GetNames( enumType: key );
            EnumDictionary.TryAdd( key, values );

            return values;
        }

        [NotNull]
        public static Percentage GetRandomness( [NotNull] this Action<Byte[]> randomFunc, UInt16 bytesToTest ) {
            var buffer = new Byte[ bytesToTest ];
            randomFunc( buffer );

            var compressed = buffer.Compress();

            var result = new Percentage( compressed.LongLength, buffer.LongLength );

            return result;
        }

        public static Single GetSingle( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( Single ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToSingle( data, startIndex: 0 );
        }

        public static UInt16 GetUInt16( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( UInt16 ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToUInt16( data, startIndex: 0 );
        }

        public static UInt32 GetUInt32( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( UInt32 ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToUInt32( data, startIndex: 0 );
        }

        public static UInt64 GetUInt64( [NotNull] this RandomNumberGenerator rng ) {
            var data = new Byte[ sizeof( UInt64 ) ];
            rng.GetNonZeroBytes( data: data );

            return BitConverter.ToUInt64( data, startIndex: 0 );
        }

        /// <summary>
        ///     Generate a random number between <paramref name="minValue" /> and <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int32 Next( this Int32 minValue, Int32 maxValue ) => Instance().Next( minValue: minValue, maxValue: maxValue );

        /// <summary>
        ///     <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int32 Next( this Int32 maxValue ) => Instance().Next( maxValue: maxValue );

        public static String Next( [NotNull] this String[] strings ) => strings[ strings.Length.Next() ];

        /// <summary>
        ///     <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static UInt16 Next( this UInt16 maxValue ) => ( UInt16 ) Instance().Next( maxValue: maxValue );

        /// <summary>
        ///     Generate a random number between <paramref name="range.Min" /> and <paramref name="range.Max" /> .
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int32 Next( this Int32Range range ) => Instance().Next( minValue: range.Min, maxValue: range.Max );

        /// <summary>
        ///     Returns a nonnegative random number.
        /// </summary>
        /// <returns></returns>
        public static UInt32 Next() => ( UInt32 ) ( Instance().NextDouble() * UInt32.MaxValue );

        /// <summary>
        ///     Generate a random number between <paramref name="minValue" /> and <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public static UInt64 Next( this UInt64 minValue, UInt64 maxValue ) {
            var min = Math.Min( val1: minValue, val2: maxValue );
            var max = Math.Max( val1: minValue, val2: maxValue );

            return min + ( UInt64 ) ( Instance().NextDouble() * ( max - min ) );
        }

        /// <summary>
        ///     Generate a random number between <paramref name="minValue" /> and <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public static Int64 Next( this Int64 minValue, Int64 maxValue ) {
            var min = Math.Min( val1: minValue, val2: maxValue );
            var max = Math.Max( val1: minValue, val2: maxValue );

            return min + ( Int64 ) ( Instance().NextDouble() * ( max - min ) );
        }

        /// <summary>
        ///     Untested.
        /// </summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        public static BigInteger NextBigInteger( this UInt16 numberOfDigits ) {

            if ( numberOfDigits <= 0 ) {
                return BigInteger.Zero;
            }

            var buffer = new Byte[ numberOfDigits ];
            Instance().NextBytes( buffer: buffer );

            return new BigInteger( buffer );
        }

        /// <summary>
        /// </summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static BigInteger NextBigIntegerPositive( this UInt16 numberOfDigits ) {

            if ( numberOfDigits <= 0 ) {
                return BigInteger.Zero;
            }

            var buffer = new Byte[ numberOfDigits ];
            Instance().NextBytes( buffer: buffer );
            buffer[ buffer.Length - 1 ] &= 0x7f; //force sign bit to positive according to http://stackoverflow.com/a/17367241/956364

            return new BigInteger( buffer );
        }

        public static BigInteger NextBigIntegerSecure( this UInt16 numberOfDigits ) {

            if ( numberOfDigits <= 0 ) {
                return BigInteger.Zero;
            }

            var buffer = new Byte[ numberOfDigits ];

            RNG.Value.GetBytes(
                data: buffer ); //BUG is this correct? I think it is, but http://stackoverflow.com/questions/2965707/c-sharp-a-random-bigint-generator suggests a "numberOfDigits/8" here.

            return new BigInteger( buffer );
        }

        /// <summary>
        ///     <para>Generate a random <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Boolean NextBoolean() => Instance().NextDouble() >= 0.5;

        /// <summary>
        ///     <para>Generate a random <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>This needs testing if it is actually any faster than <see cref="NextBoolean" />.</remarks>
        public static Boolean NextBooleanFast() => Instance().Next( maxValue: 2 ).Any();

        /// <summary>
        ///     <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Byte NextByte( this Byte min, Byte max ) {
            unchecked {
                var rng = RNG.Value;
                Byte result;

                do {
                    result = ( Byte ) ( Byte.MaxValue * rng.GetSingle() );
                } while ( result < min || result > max ); //TODO ugh

                return result;
            }
        }

        /// <summary>
        ///     <para>Returns a random <see cref="Byte" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Byte NextByte() {
            var buffer = new Byte[ 1 ];
            Instance().NextBytes( buffer: buffer );

            return buffer[ 0 ];
        }

        /// <summary>
        ///     <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Byte> NextBytes( this Byte min, Byte max ) {
            yield return min.NextByte( max: max );
        }

        /// <summary>
        ///     <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
        /// </summary>
        /// <returns></returns>
        public static void NextBytes( this Byte min, Byte max, [NotNull] ref Byte[] buffer ) {
            Instance().NextBytes( buffer: buffer );

            for ( var p = 0; p < max; p++ ) {
                if ( buffer[ p ] < min || buffer[ p ] < max ) {
                    buffer[ p ] = min.NextByte( max: max );
                }
            }
        }

        public static void NextBytes( ref Byte[] buffer ) {
            Instance().NextBytes( buffer );
        }

        /// <summary>
        /// </summary>
        /// <param name="alpha">  </param>
        /// <param name="lowEnd"> </param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static Color NextColor( Byte alpha = 255, Byte lowEnd = 0, Byte highEnd = 255 ) =>
            Color.FromArgb( alpha: alpha, red: Next( minValue: lowEnd, maxValue: highEnd ), green: Next( minValue: lowEnd, maxValue: highEnd ),
                blue: Next( minValue: lowEnd, maxValue: highEnd ) );

        public static DateTime NextDateTime( this DateTime value, TimeSpan timeSpan ) => value + new Milliseconds( timeSpan.TotalMilliseconds * Instance().NextDouble() );

        public static DateTime NextDateTime( this DateTime earlier, DateTime later ) {
            if ( earlier > later ) {
                CommonExtensions.Swap( left: ref earlier, right: ref later );
            }

            var range = later - earlier;

            return earlier + new Milliseconds( range.TotalMilliseconds );
        }

        public static DateTimeOffset NextDateTimeOffset( this DateTimeOffset value, TimeSpan timeSpan ) =>
            value + new Milliseconds( timeSpan.TotalMilliseconds * Instance().NextDouble() );

        /// <summary>
        ///     Between <see cref="Decimal.Zero" /> and <see cref="Decimal.One" />.
        /// </summary>
        /// <returns></returns>
        public static Decimal NextDecimal() {
            do {
                try {
                    return NextDecimal( Decimal.Zero, Decimal.One );
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.Log();
                }
            } while ( true );
        }

        /// <summary>
        ///     <para>Returns a random Decimal between <paramref name="minValue" /> and <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static Decimal NextDecimal( this Decimal minValue, Decimal maxValue ) {
            var min = Math.Min( val1: minValue, val2: maxValue );
            var max = Math.Max( val1: minValue, val2: maxValue );
            var range = max - min;

            return min + (NextDecimal() * range);
        }

        public static Decimal NextDecimal( [NotNull] this DecimalRange decimalRange ) => decimalRange.Min.NextDecimal( maxValue: decimalRange.Max );

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static Decimal NextDecimalFullRange() {
            do {
                try {
                    return new Decimal( lo: NextInt32(), mid: NextInt32(), hi: NextInt32(), isNegative: NextBoolean(), scale: ( Byte ) 0.Next( maxValue: 9 ) );
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.Log();
                }
            } while ( true );
        }

        public static Degrees NextDegrees() => new Degrees( NextSingle( min: Degrees.MinimumValue, max: Degrees.MaximumValue ) );

        /// <summary>
        ///     <para>Returns a random digit between 0 and 9.</para>
        /// </summary>
        /// <returns></returns>
        public static Byte NextDigit() {
            var result = NextByte();
            result %= 10; //TODO bug check, is modulo inclusive?

            return result;
        }

        /// <summary>
        ///     Returns a random digit (0,1,2,3,4,5,6,7,8,9) between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Digit NextDigit( this Digit min, Digit max ) {
            unchecked {
                if ( min > max ) {
                    CommonExtensions.Swap( left: ref min, right: ref max );
                }

                Byte result;

                do {
                    result = NextByte();
                    result %= 10; //reduce the number of loops
                } while ( result < min || result > max );

                return new Digit( result );
            }
        }

        /// <summary>
        ///     <para>Returns a random Double between <paramref name="range.Min" /> and <paramref name="range.Max" />.</para>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Double NextDouble( this DoubleRange range ) => range.Min + (Instance().NextDouble() * range.Length);

        public static Double NextDouble( this PairOfDoubles variance ) => NextDouble( min: variance.Low, max: variance.High );

        /// <summary>
        ///     Returns a random Double between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public static Double NextDouble( Double min = 0.0, Double max = 1.0 ) {
            var range = max - min;

            if ( Double.IsNaN( d: range ) ) {
                throw new ArgumentOutOfRangeException();
            }

            Double result;

            if ( !Double.IsInfinity( d: range ) ) {
                result = min + (Instance().NextDouble() * range);

                //result.Should().BeInRange( minimumValue: min, maximumValue: max );

                return result;
            }

            do {
                Instance().NextBytes( buffer: LocalByteBuffer.Value );
                result = BitConverter.ToDouble( LocalByteBuffer.Value, startIndex: 0 );
            } while ( Double.IsInfinity( d: result ) || Double.IsNaN( d: result ) );

            //result.Should().BeInRange( minimumValue: min, maximumValue: max );

            return result;
        }

        /// <summary>
        ///     Returns a random Double between 0 and 1
        /// </summary>
        /// <returns></returns>
        public static Double NextDouble() => Instance().NextDouble();

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T NextEnum<T>() where T : struct {
            if ( !typeof( T ).IsEnum ) {
                return default;
            }

            var vals = GetNames<T>();
            var rand = Instance().Next( minValue: 0, maxValue: vals.Length );
            var picked = vals[ rand ];

            return ( T ) Enum.Parse( enumType: typeof( T ), picked );
        }

        /// <summary>
        ///     Returns a random <see cref="Single" /> between <paramref name="range.Min" /> and <paramref name="range.Max" />.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Single NextFloat( this SingleRange range ) => ( Single ) ( range.Min + (Instance().NextDouble() * range.Length) );

        /// <summary>
        ///     Returns a random float between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Single NextFloat( Single min = 0, Single max = 1 ) => ( Single ) ( min + (Instance().NextDouble() * ( max - min )) );

        public static Guid NextGuid() => Guid.NewGuid();

        /// <summary>
        ///     Gets a non-negetive random whole number less than the specified <paramref cref="maximum" />.
        /// </summary>
        /// <param name="maximum">The exclusive upper bound the random number to be generated.</param>
        /// <returns>A non-negetive random whole number less than the specified <paramref cref="maximum" />.</returns>
        public static Int32 NextInt( this Int32 maximum ) => Instance().Next( maxValue: maximum );

        /// <summary>
        ///     Gets a random number within a specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned.</param>
        /// <returns>A random number within a specified range.</returns>
        public static Int32 NextInt( this Int32 min, Int32 max ) => Instance().Next( minValue: min, maxValue: max );

        /// <summary>
        ///     Return a random number somewhere in the full range of 0 to <see cref="Int16" />.
        /// </summary>
        /// <returns></returns>
        public static Int16 NextInt16( this Int16 min, Int16 max ) => ( Int16 ) ( min + (Instance().NextDouble() * ( max - min )) );

        /// <summary>
        ///     Return a random number somewhere in the full range of <see cref="Int32" />.
        /// </summary>
        /// <returns></returns>
        public static Int32 NextInt32() {
            var firstBits = Instance().Next( minValue: 0, maxValue: 1 << 4 ) << 28;
            var lastBits = Instance().Next( minValue: 0, maxValue: 1 << 28 );

            return firstBits | lastBits;
        }

        public static Int64 NextInt64() {
            var buffer = new Byte[ sizeof( Int64 ) ];
            Instance().NextBytes( buffer: buffer );

            return BitConverter.ToInt64( buffer, startIndex: 0 );
        }

        /// <summary>
        ///     Returns a random Single between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Single NextSingle( Single min = 0, Single max = 1 ) => ( Single ) ( min + (Instance().NextDouble() * ( max - min )) );

        public static Single NextSingle( this SingleRange singleRange ) => NextSingle( min: singleRange.Min, max: singleRange.Max );

        /// <summary>
        ///     Return a random <see cref="SpanOfTime" /> between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static SpanOfTime NextSpan( this SpanOfTime min, SpanOfTime max ) {
            var tpMin = min.CalcTotalPlanckTimes();
            var tpMax = max.CalcTotalPlanckTimes();

            if ( tpMin > tpMax ) {
                CommonExtensions.Swap( left: ref tpMin, right: ref tpMax );
            }

            var range = tpMax.Value - tpMin.Value;

            do {
                var numberOfDigits = ( UInt16 ) 1.Next( maxValue: range.ToString( format: "R" ).Length );

                var amount = numberOfDigits.NextBigIntegerPositive(); //BUG here

                var span = new SpanOfTime( planckTimes: tpMin.Value + amount );

                if ( span >= min && span <= max ) {
                    return span;
                }
            } while ( true ); //BUG fix this horribleness.
        }

        /// <summary>
        ///     Generate a random String.
        /// </summary>
        /// <param name="length"> How many characters long.</param>
        /// <param name="lowers">
        ///     <see cref="ParsingConstants.Lowercase" />
        /// </param>
        /// <param name="uppers">
        ///     <see cref="ParsingConstants.Uppercase" />
        /// </param>
        /// <param name="numbers">
        ///     <see cref="ParsingConstants.Numbers" />
        /// </param>
        /// <param name="symbols">
        ///     <see cref="ParsingConstants.Symbols" />
        /// </param>
        /// <returns></returns>
        [CanBeNull]
        public static String NextString( Int32 length, Boolean lowers = true, Boolean uppers = false, Boolean numbers = false, Boolean symbols = false ) {
            if ( !length.Any() ) {
                return null;
            }

            if ( !length.CanAllocateMemory() ) {
                return null;
            }

            var sb = new StringBuilder();

            if ( lowers ) {
                sb.Append( ParsingConstants.Lowercase );
            }

            if ( uppers ) {
                sb.Append( ParsingConstants.Uppercase );
            }

            if ( numbers ) {
                sb.Append( ParsingConstants.Numbers );
            }

            if ( symbols ) {
                sb.Append( ParsingConstants.Symbols );
            }

            var charPool = sb.ToString();

            if ( charPool.IsEmpty() ) {
                return String.Empty;
            }

            return new String( Enumerable.Range( start: 0, count: length ).Select( selector: i => charPool[ index: 0.Next( maxValue: charPool.Length ) ] ).ToArray() );
        }

        /// <summary>
        ///     Returns a random TimeSpan between <paramref name="minValue" /> and <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static TimeSpan NextTimeSpan( this TimeSpan minValue, TimeSpan maxValue ) {
            TimeSpan min;
            TimeSpan max;

            if ( minValue <= maxValue ) {
                min = minValue;
                max = maxValue;
            }
            else {
                min = maxValue;
                max = minValue;
            }

            try {
                var range = ( max - min ).Ticks;

                var next = range * Instance().NextDouble();

                return min + TimeSpan.FromTicks( ( Int64 ) next );
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.Log();

                return min;
            } //return TimeSpan.FromTicks(min.Ticks + Instance.Next( minValue: minTicks, maxValue: maxTicks ) );
        }

        /// <summary>
        ///     Returns a random <see cref="TimeSpan" /> between <paramref name="minMilliseconds" /> and
        ///     <paramref name="maxMilliseconds" /> .
        /// </summary>
        /// <param name="minMilliseconds"></param>
        /// <param name="maxMilliseconds"></param>
        /// <returns></returns>
        public static TimeSpan NextTimeSpan( this Int32 minMilliseconds, Int32 maxMilliseconds ) =>
            TimeSpan.FromMilliseconds( minMilliseconds > maxMilliseconds ?
                Instance().Next( minValue: maxMilliseconds, maxValue: minMilliseconds ) :
                Instance().Next( minValue: minMilliseconds, maxValue: maxMilliseconds ) );

        public static UInt64 NextUInt64() {
            var buffer = new Byte[ sizeof( UInt64 ) ];
            Instance().NextBytes( buffer: buffer );

            return BitConverter.ToUInt64( buffer, startIndex: 0 );
        }

        /// <summary>
        ///     Generates a uniformly random integer in the range [0, bound).
        /// </summary>
        public static BigInteger RandomIntegerBelow( [NotNull] this RandomNumberGenerator source, BigInteger bound ) {
            Contract.Requires<ArgumentException>( condition: source != null );
            Contract.Requires<ArgumentException>( condition: bound > 0 );

            //Contract.Ensures( Contract.Result<BigInteger>( ) >= 0 );
            //Contract.Ensures( Contract.Result<BigInteger>( ) < bound );

            //Get a byte buffer capable of holding any value below the bound
            var buffer = ( bound << 16 ).ToByteArray(); // << 16 adds two bytes, which decrease the chance of a retry later on

            //Compute where the last partial fragment starts, in order to retry if we end up in it
            var generatedValueBound = BigInteger.One << ( (buffer.Length * 8) - 1 ); //-1 accounts for the sign bit
            Contract.Assert( condition: generatedValueBound >= bound );
            var validityBound = generatedValueBound - (generatedValueBound % bound);
            Contract.Assert( condition: validityBound >= bound );

            while ( true ) {

                //generate a uniformly random value in [0, 2^(buffer.Length * 8 - 1))
                source.GetBytes( data: buffer );
                buffer[ buffer.Length - 1 ] &= 0x7F; //force sign bit to positive
                var r = new BigInteger( buffer );

                //return unless in the partial fragment
                if ( r >= validityBound ) {
                    continue;
                }

                return r % bound;
            }
        }

        /// <summary>
        ///     Given the String <paramref name="charPool" />, return the letters in a random fashion.
        /// </summary>
        /// <param name="charPool"></param>
        /// <returns></returns>
        public static String Randomize( [CanBeNull] this String charPool ) =>
            null == charPool ? String.Empty : charPool.OrderBy( keySelector: r => Next() ).Aggregate( seed: String.Empty, func: ( current, c ) => current + c );

        /// <summary>
        ///     <para>A list containing <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        public static IEnumerable<Boolean> Randomly() {
            do {
                yield return NextBoolean();
            } while ( true );

            // ReSharper disable once FunctionNeverReturns ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        ///     Generate a pronounceable pseudorandom string.
        /// </summary>
        /// <param name="aboutLength">Length of the returned string.</param>
        [NotNull]
        public static String RandomPronounceableString( this Int32 aboutLength ) {
            if ( aboutLength < 1 ) {
                throw new ArgumentOutOfRangeException( nameof( aboutLength ), actualValue: aboutLength, $"{aboutLength} is out of range." );
            }

            //char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
            //char[] vowels = { 'a', 'e', 'i', 'o', 'u' };

            var word = new StringBuilder( capacity: aboutLength * 2 ); //maximum we'll use
            var consonant = NextBoolean();

            for ( var i = 0; i < aboutLength; i++ ) {
                word.Append( consonant ?
                    ParsingConstants.Consonants[ 0.Next( maxValue: ParsingConstants.Consonants.Length ) ] :
                    ParsingConstants.Vowels[ 0.Next( maxValue: ParsingConstants.Vowels.Length ) ] );

                consonant = !consonant;
            }

            return word.ToString();
        }

        [NotNull]
        public static Sentence RandomSentence( Int32 avgWords = 7 ) {
            var list = new List<Word>();

            if ( NextBoolean() ) {

                //-V3003
                avgWords += NextByte( min: 1, max: 3 );
            }
            else if ( NextBoolean() ) {
                --avgWords;
            }

            for ( var i = 0; i < avgWords; i++ ) {
                list.Add( item: RandomWord() );
            }

            return new Sentence( words: list );
        }

        /// <summary>
        ///     Generate a random String using a limited set of defaults. Default <paramref name="length" /> is 10. Default
        ///     <paramref name="lowerCase" /> is true. Default <paramref name="upperCase" /> is false. Default
        ///     <paramref
        ///         name="numbers" />
        ///     is false. Default <paramref name="symbols" /> is false.
        /// </summary>
        /// <param name="length">   </param>
        /// <param name="lowerCase"></param>
        /// <param name="upperCase"></param>
        /// <param name="numbers">  </param>
        /// <param name="symbols">  </param>
        /// <returns></returns>
        [NotNull]
        public static String RandomString( Int32 length = 10, Boolean lowerCase = true, Boolean upperCase = false, Boolean numbers = false, Boolean symbols = false ) {
            var charPool = String.Concat( str0: lowerCase ? ParsingConstants.EnglishAlphabetLowercase : String.Empty,
                str1: upperCase ? ParsingConstants.EnglishAlphabetUppercase : String.Empty, str2: numbers ? ParsingConstants.Numbers : String.Empty,
                str3: symbols ? ParsingConstants.Symbols : String.Empty );

            return new String( Enumerable.Range( start: 0, count: length ).Select( selector: i => charPool[ index: 0.Next( maxValue: charPool.Length ) ] ).ToArray() );
        }

        [NotNull]
        public static Word RandomWord( Int32 avglength = 5, Boolean lowerCase = true, Boolean upperCase = true, Boolean numbers = false, Boolean symbols = false ) {
            var word = RandomString( ( avglength - 2 ).Next( maxValue: avglength + 2 ), lowerCase: lowerCase, upperCase: upperCase, numbers: numbers, symbols: symbols );

            return new Word( word: word );
        }

        [Obsolete( "Huh?" )]
        public static async Task<Boolean> Reseed( CancellationToken cancellationToken, TimeSpan? timeoutSpan = null ) {
            Int32? seed = null;

            PollResponses.Clear();

            var timeout = Task.Delay( delay: timeoutSpan ?? Seconds.One, cancellationToken: cancellationToken );

            var tasks = new List<Task> {
                timeout,
                Task.Run( async () => PollResponses.Push( item: ( await FacebookErrorGrabber.GetError() ).Error.FbtraceID.GetHashCode() ),
                    cancellationToken: cancellationToken ),
                Task.Run( () => PollResponses.Push( item: RandomDotOrg.Generator.Value.Get() ), cancellationToken: cancellationToken )
            };

            var task = await Task.WhenAny( tasks: tasks );

            if ( task == timeout ) {
                seed = Guid.NewGuid().GetHashCode();
            }
            else {
                if ( PollResponses.TryPop( result: out var result ) ) {
                    seed = result;
                }
            }

            if ( !seed.HasValue ) {
                return false;
            }

            ThreadSafeRandom.Value = new Lazy<Random>( valueFactory: () => new Random( seed.Value ) );

            return true;
        }

        public static void Seed( Int32 newValue ) {

            //ThreadSafeRandom.Value.Value.reseed???
        }

        /// <summary>
        ///     Generate two random numbers about halfway of
        ///     <param name="goal"></param>
        ///     .
        /// </summary>
        /// <remarks>
        ///     Given one number, return two random numbers that add up to
        ///     <param name="goal"></param>
        /// </remarks>
        public static void Split( this Int32 goal, out Int32 lowResult, out Int32 highResult ) {
            var half = goal.Half();
            var quarter = half.Half();
            var firstNum = Instance().Next( minValue: half - quarter, maxValue: half + quarter );
            var secondNum = goal - firstNum;

            if ( firstNum > secondNum ) {
                lowResult = secondNum;
                highResult = firstNum;
            }
            else {
                lowResult = firstNum;
                highResult = secondNum;
            }

            //highResult.Should().BeGreaterThan( expected: lowResult );
            //( lowResult + highResult ).Should().Be( expected: goal );
        }

        /// <summary>
        ///     <para>Generate two random numbers about halfway of <paramref name="goal" />.</para>
        ///     <para>Also, return a random number between <paramref name="lowResult" /> and <paramref name="highResult" /></para>
        /// </summary>
        /// <remarks>
        ///     Given one number, return two random numbers that add up to
        ///     <param name="goal"></param>
        /// </remarks>
        public static Decimal Split( this Decimal goal, out Decimal lowResult, out Decimal highResult ) {
            var half = goal.Half();
            var quarter = half.Half();
            var firstNum = ( half - quarter ).NextDecimal( maxValue: half + quarter );
            var secondNum = goal - firstNum;

            if ( firstNum > secondNum ) {
                lowResult = secondNum;
                highResult = firstNum;
            }
            else {
                lowResult = firstNum;
                highResult = secondNum;
            }

            //highResult.Should().BeGreaterThan( expected: lowResult );
            //( lowResult + highResult ).Should().Be( expected: goal );

            return lowResult.NextDecimal( maxValue: highResult );
        }
    }
}