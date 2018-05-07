// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Randem.cs" was last cleaned by Protiguous on 2018/05/04 at 9:37 PM

namespace Librainian.Maths {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using FluentAssertions;
    using Internet;
    using JetBrains.Annotations;
    using Linguistics;
    using MathNet.Numerics.Random;
    using Measurement.Spatial;
    using Measurement.Time;
    using Newtonsoft.Json;
    using Numbers;
    using NUnit.Framework;
    using OperatingSystem;
    using OperatingSystem.Compression;
    using Parsing;
    using RandomNameGeneratorLibrary;
    using Ranges;
    using Threading.RandomOrg;

    public static class Randem {

        /// <summary>
        /// A Double-sized byte buffer per-thread.
        /// </summary>
        private static readonly ThreadLocal<Byte[]> LocalByteBuffer = new ThreadLocal<Byte[]>( valueFactory: () => new Byte[sizeof( Double )], trackAllValues: true );

        /// <summary></summary>
        [NotNull]
        public static ConcurrentDictionary<Type, String[]> EnumDictionary { get; } = new ConcurrentDictionary<Type, String[]>();

        /// <summary>A thread-local (threadsafe) <see cref="Random" />.</summary>
        [NotNull]
        public static Random Instance => ThreadSafeRandom.Value.Value;

        [NotNull]
        public static Lazy<PersonNameGenerator> Names { get; } = new Lazy<PersonNameGenerator>( valueFactory: () => new PersonNameGenerator() );

        /// <summary>
        ///     <para>More cryptographically strong than <see cref="Random" />.</para>
        /// </summary>
        [NotNull]
        public static ThreadLocal<RandomNumberGenerator> RNG { get; } = new ThreadLocal<RandomNumberGenerator>( valueFactory: () => new RNGCryptoServiceProvider(), trackAllValues: true );

        [NotNull]
        public static ThreadLocal<Lazy<SHA256Managed>> ThreadLocalSHA256Managed { get; } =
                    new ThreadLocal<Lazy<SHA256Managed>>( valueFactory: () => { return new Lazy<SHA256Managed>( valueFactory: () => new SHA256Managed() ); } );

        /// <summary>Provide to each thread its own <see cref="Random" /> with a random seed.</summary>
        [NotNull]
        public static ThreadLocal<Lazy<Random>> ThreadSafeRandom { get; } = new ThreadLocal<Lazy<Random>>( valueFactory: () => {
            var hash = ThreadLocalSHA256Managed.Value.Value.ComputeHash( buffer: Guid.NewGuid().ToByteArray() );

            var seed = BitConverter.ToInt32( value: hash, startIndex: 0 );
            seed = seed.GetHashMerge( objectB: Thread.CurrentThread.ManagedThreadId );

            Debug.WriteLine( message: $"Init random with seed {seed} on thread {Thread.CurrentThread.ManagedThreadId}." );

            return new Lazy<Random>( valueFactory: () => new Random( Seed: seed ) );
        }, trackAllValues: true );

        internal static ConcurrentStack<Int32> PollResponses { get; } = new ConcurrentStack<Int32>();

        /// <summary>
        ///     Returns a decimal between 0 and 1.
        /// </summary>
        /// <returns></returns>
        public static Decimal BranchRatio() => NextDecimal();

        /// <summary>Chooses a random element in the given collection <paramref name="items" />.</summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="items">The collection of items to choose a random element from.</param>
        /// <returns>A randomly chosen element in the given collection <paramref name="items" />.</returns>
        public static T Choose<T>( [NotNull] params T[] items ) => items[items.Length.NextInt()];

        /// <summary>Chooses a random element in the given set of items.</summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a ) => a;

        /// <summary>Chooses a random element in the given set of items.</summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b ) {
            switch ( 2.NextInt() ) {
                case 0:
                    return a;

                default:
                    return b;
            }
        }

        /// <summary>Chooses a random element in the given set of items.</summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c ) {
            switch ( 3.NextInt() ) {
                case 0:
                    return a;

                case 1:
                    return b;

                default:
                    return c;
            }
        }

        /// <summary>Chooses a random element in the given set of items.</summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d ) {
            switch ( 4.NextInt() ) {
                case 0:
                    return a;

                case 1:
                    return b;

                case 2:
                    return c;

                default:
                    return d;
            }
        }

        /// <summary>Chooses a random element in the given set of items.</summary>
        /// <typeparam name="T">The Type of element.</typeparam>
        /// <param name="a">The first item.</param>
        /// <param name="b">The second item.</param>
        /// <param name="c">The third item.</param>
        /// <param name="d">The fourth item.</param>
        /// <param name="e">The fifth item.</param>
        /// <returns>A randomly chosen element in the given set of items.</returns>
        public static T Choose<T>( this T a, T b, T c, T d, T e ) {
            switch ( 5.NextInt() ) {
                case 0:
                    return a;

                case 1:
                    return b;

                case 2:
                    return c;

                case 3:
                    return d;

                default:
                    return e;
            }
        }

        /// <summary>Chooses a random element in the given set of items.</summary>
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
                case 0:
                    return a;

                case 1:
                    return b;

                case 2:
                    return c;

                case 3:
                    return d;

                case 4:
                    return e;

                default:
                    return f;
            }
        }

        /// <summary>Chooses a random element in the given set of items.</summary>
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
                case 0:
                    return a;

                case 1:
                    return b;

                case 2:
                    return c;

                case 3:
                    return d;

                case 4:
                    return e;

                case 5:
                    return f;

                default:
                    return g;
            }
        }

        /// <summary>Chooses a random element in the given set of items.</summary>
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
                case 0:
                    return a;

                case 1:
                    return b;

                case 2:
                    return c;

                case 3:
                    return d;

                case 4:
                    return e;

                case 5:
                    return f;

                case 6:
                    return g;

                default:
                    return h;
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

        public static List<Int32> GenerateRandom( Int32 count, Int32 min, Int32 max ) {

            //  initialize set S to empty
            //  for J := N-M + 1 to N do
            //    T := RandInt(1, J)
            //    if T is not in S then
            //      insert T in S
            //    else
            //      insert J in S
            //
            // adapted for C# which does not have an inclusive Next(..)
            // and to make it from configurable range not just 1.

            if ( max <= min || count < 0 ||

                 // max - min > 0 required to avoid overflow
                 count > max - min && max - min > 0 ) {

                // need to use 64-bit to support big ranges (negative min, positive max)
                throw new ArgumentOutOfRangeException( paramName: $"Range {min} to {max} ({( Int64 )max - min} values), or count {count} is illegal." );
            }

            // generate count random values.
            var candidates = new HashSet<Int32>();

            // start count values before max, and end at max
            for ( var top = max - count; top < max; top++ ) {

                // May strike a duplicate.
                // Need to add +1 to make inclusive generator
                // +1 is safe even for MaxVal max value because top < max
                if ( !candidates.Add( item: Instance.Next( minValue: min, maxValue: top + 1 ) ) ) {

                    // collision, add inclusive max.
                    // which could not possibly have been added before.
                    candidates.Add( item: top );
                }
            }

            // load them in to a list, to sort
            var result = candidates.ToList();

            // shuffle the results because HashSet has messed
            // with the order, and the algorithm does not produce
            // random-ordered results (e.g. max-1 will never be the first value)
            for ( var i = result.Count - 1; i > 0; i-- ) {
                var k = Instance.Next( maxValue: i + 1 );
                var tmp = result[index: k];
                result[index: k] = result[index: i];
                result[index: i] = tmp;
            }

            return result;
        }

        public static Char GetChar( this RandomNumberGenerator rng ) {
            if ( rng is null ) {
                throw new ArgumentNullException( paramName: nameof( rng ) );
            }

            var data = new Byte[sizeof( Char )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToChar( value: data, startIndex: 0 );
        }

        public static Double GetDouble( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( Double )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToDouble( value: data, startIndex: 0 );
        }

        public static Int16 GetInt16( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( Int16 )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToInt16( value: data, startIndex: 0 );
        }

        public static Int32 GetInt32( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( Int32 )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToInt32( value: data, startIndex: 0 );
        }

        public static Int64 GetInt64( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( Int64 )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToInt64( value: data, startIndex: 0 );
        }

        /// <summary>memoize?</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static String[] GetNames<T>() {
            var key = typeof( T );
            if ( EnumDictionary.TryGetValue(key, value: out var values ) ) {
                return values;
            }

            values = Enum.GetNames( enumType: key );
            EnumDictionary.TryAdd(key, value: values );
            return values;
        }

        public static Percentage GetRandomness( this Action<Byte[]> randomFunc, UInt16 bytesToTest ) {
            var buffer = new Byte[bytesToTest];
            randomFunc( buffer );

            var compressed = buffer.Compress();

            var result = new Percentage( numerator: ( BigInteger )compressed.LongLength, denominator: buffer.LongLength );

            return result;
        }

        public static Single GetSingle( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( Single )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToSingle( value: data, startIndex: 0 );
        }

        public static UInt16 GetUInt16( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( UInt16 )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToUInt16( value: data, startIndex: 0 );
        }

        public static UInt32 GetUInt32( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( UInt32 )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToUInt32( value: data, startIndex: 0 );
        }

        public static UInt64 GetUInt64( this RandomNumberGenerator rng ) {
            var data = new Byte[sizeof( UInt64 )];
            rng.GetNonZeroBytes( data: data );
            return BitConverter.ToUInt64( value: data, startIndex: 0 );
        }

        /// <summary>
        ///     Generate a random number between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int32 Next( this Int32 minValue, Int32 maxValue ) => Instance.Next( minValue: minValue, maxValue: maxValue );

        /// <summary>
        ///     <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int32 Next( this Int32 maxValue ) => Instance.Next( maxValue: maxValue );

        public static String Next( this String[] strings ) => strings[strings.Length.Next()];

        /// <summary>
        ///     <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static UInt16 Next( this UInt16 maxValue ) => ( UInt16 )Instance.Next( maxValue: maxValue );

        /// <summary>
        ///     Generate a random number between <paramref name="range.Min" /> and
        ///     <paramref name="range.Max" /> .
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Int32 Next( this Int32Range range ) => Instance.Next( minValue: range.Min, maxValue: range.Max );

        /// <summary>Returns a nonnegative random number.</summary>
        /// <returns></returns>
        public static UInt32 Next() => ( UInt32 )( Instance.NextDouble() * UInt32.MaxValue );

        /// <summary>
        ///     Generate a random number between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public static UInt64 Next( this UInt64 minValue, UInt64 maxValue ) {
            var min = Math.Min( val1: minValue, val2: maxValue );
            var max = Math.Max( val1: minValue, val2: maxValue );
            return min + ( UInt64 )( Instance.NextDouble() * ( max - min ) );
        }

        /// <summary>
        ///     Generate a random number between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public static Int64 Next( this Int64 minValue, Int64 maxValue ) {
            var min = Math.Min( val1: minValue, val2: maxValue );
            var max = Math.Max( val1: minValue, val2: maxValue );
            return min + ( Int64 )( Instance.NextDouble() * ( max - min ) );
        }

        /// <summary></summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static BigInteger NextBigInteger( this UInt16 numberOfDigits ) {
            numberOfDigits.Should().BeGreaterThan( expected: 0 );
            if ( numberOfDigits <= 0 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( numberOfDigits ) );
            }

            var buffer = new Byte[numberOfDigits];
            Instance.NextBytes( buffer: buffer );
            return new BigInteger( value: buffer );
        }

        /// <summary></summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static BigInteger NextBigIntegerPositive( this UInt16 numberOfDigits ) {
            numberOfDigits.Should().BeGreaterThan( expected: 0 );
            if ( numberOfDigits <= 0 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( numberOfDigits ) );
            }

            var buffer = new Byte[numberOfDigits];
            Instance.NextBytes( buffer: buffer );
            buffer[buffer.Length - 1] &= 0x7f; //force sign bit to positive according to http://stackoverflow.com/a/17367241/956364
            return new BigInteger( value: buffer );
        }

        public static BigInteger NextBigIntegerSecure( this UInt16 numberOfDigits ) {
            numberOfDigits.Should().BeGreaterThan( expected: 0 );
            if ( numberOfDigits <= 0 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( numberOfDigits ) );
            }

            var buffer = new Byte[numberOfDigits];
            RNG.Value.GetBytes( data: buffer ); //BUG is this correct? I think it is, but http://stackoverflow.com/questions/2965707/c-sharp-a-random-bigint-generator suggests a "numberOfDigits/8" here.
            return new BigInteger( value: buffer );
        }

        /// <summary>
        ///     <para>Generate a random <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Boolean NextBoolean() => Instance.NextDouble() >= 0.5;

        /// <summary>
        ///     <para>Generate a random <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        /// <returns></returns>
        /// <remarks>This needs testing if it is actually any faster than <see cref="NextBoolean" />.</remarks>
        public static Boolean NextBooleanFast() => Instance.Next( maxValue: 2 ) == 0;

        /// <summary>
        ///     <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Byte NextByte( this Byte min, Byte max ) {
            unchecked {
                var rng = RNG.Value;
                Byte result;
                do {
                    result = ( Byte )( Byte.MaxValue * rng.GetSingle() );
                } while ( result < min || result > max );

                return result;
            }
        }

        /// <summary>
        ///     <para>Returns a random <see cref="Byte" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Byte NextByte() {
            Byte[] buffer = { 0 };
            Instance.NextBytes( buffer: buffer );
            return buffer[0];
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
        public static void NextBytes( this Byte min, Byte max, ref Byte[] buffer ) {
            Instance.NextBytes( buffer: buffer );
            for ( var p = 0; p < max; p++ ) {
                if ( buffer[p] < min || buffer[p] < max ) {
                    buffer[p] = min.NextByte( max: max );
                }
            }
        }

        /// <summary></summary>
        /// <param name="alpha"></param>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static Color NextColor( Byte alpha = 255, Byte lowEnd = 0, Byte highEnd = 255 ) =>
            Color.FromArgb( alpha: alpha, red: Next( minValue: lowEnd, maxValue: highEnd ), green: Next( minValue: lowEnd, maxValue: highEnd ), blue: Next( minValue: lowEnd, maxValue: highEnd ) );

        public static DateTime NextDateTime( this DateTime value, TimeSpan timeSpan ) => value + new Milliseconds( value: timeSpan.TotalMilliseconds * Instance.NextDouble() );

        public static DateTime NextDateTime( this DateTime earlier, DateTime later ) {
            if ( earlier > later ) {
                MathExtensions.Swap( arg1: ref earlier, arg2: ref later );
            }

            var range = later - earlier;
            return earlier + new Milliseconds( value: range.TotalMilliseconds );
        }

        public static DateTimeOffset NextDateTimeOffset( this DateTimeOffset value, TimeSpan timeSpan ) => value + new Milliseconds( value: timeSpan.TotalMilliseconds * Instance.NextDouble() );

        /// <summary>Between 0 and 1.</summary>
        /// <returns></returns>
        public static Decimal NextDecimal() {
            do {
                try {
                    return Instance.NextDecimal();
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.More();
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
            return min + NextDecimal() * range;
        }

        public static Decimal NextDecimal( this DecimalRange decimalRange ) => decimalRange.Min.NextDecimal( maxValue: decimalRange.Max );

        /// <summary></summary>
        /// <returns></returns>
        public static Decimal NextDecimalFullRange() {
            do {
                try {
                    return new Decimal( lo: NextInt32(), mid: NextInt32(), hi: NextInt32(), isNegative: NextBoolean(), scale: ( Byte )0.Next( maxValue: 9 ) );
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.More();
                }
            } while ( true );
        }

        public static Degrees NextDegrees() => new Degrees( value: NextSingle( min: Degrees.MinimumValue, max: Degrees.MaximumValue ) );

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
                    MathExtensions.Swap( arg1: ref min, arg2: ref max );
                }

                Byte result;
                do {
                    result = NextByte();
                    result %= 10; //reduce the number of loops
                } while ( result < min || result > max );

                return new Digit( value: result );
            }
        }

        /// <summary>
        ///     <para>Returns a random Double between <paramref name="range.Min" /> and <paramref name="range.Max" />.</para>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Double NextDouble( this DoubleRange range ) => range.Min + Instance.NextDouble() * range.Length;

        public static Double NextDouble( this PairOfDoubles variance ) => NextDouble( min: variance.Low, max: variance.High );

        /// <summary>Returns a random Double between <paramref name="min" /> and <paramref name="max" />.</summary>
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
                result = min + Instance.NextDouble() * range;
                result.Should().BeInRange( minimumValue: min, maximumValue: max );
                return result;
            }

            do {
                Instance.NextBytes( buffer: LocalByteBuffer.Value );
                result = BitConverter.ToDouble( value: LocalByteBuffer.Value, startIndex: 0 );
            } while ( Double.IsInfinity( d: result ) || Double.IsNaN( d: result ) );

            result.Should().BeInRange( minimumValue: min, maximumValue: max );

            return result;
        }

        /// <summary>Returns a random Double between 0 and 1</summary>
        /// <returns></returns>
        public static Double NextDouble() => Instance.NextDouble();

        /// <summary></summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T NextEnum<T>() where T : struct {
            if ( !typeof( T ).IsEnum ) {
                return default;
            }

            var vals = GetNames<T>();
            var rand = Instance.Next( minValue: 0, maxValue: vals.Length );
            var picked = vals[rand];
            return ( T )Enum.Parse( enumType: typeof( T ), value: picked );
        }

        /// <summary>
        ///     Returns a random <see cref="Single" /> between <paramref name="range.Min" /> and <paramref name="range.Max" />.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Single NextFloat( this SingleRange range ) => ( Single )( range.Min + Instance.NextDouble() * range.Length );

        /// <summary>Returns a random float between <paramref name="min" /> and <paramref name="max" />.</summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Single NextFloat( Single min = 0, Single max = 1 ) => ( Single )( min + Instance.NextDouble() * ( max - min ) );

        public static Guid NextGuid() => Guid.NewGuid();

        /// <summary>Gets a non-negetive random whole number less than the specified <paramref cref="maximum" />.</summary>
        /// <param name="maximum">The exclusive upper bound the random number to be generated.</param>
        /// <returns>A non-negetive random whole number less than the specified <paramref cref="maximum" />.</returns>
        public static Int32 NextInt( this Int32 maximum ) => Instance.Next( maxValue: maximum );

        /// <summary>Gets a random number within a specified range.</summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned.</param>
        /// <returns>A random number within a specified range.</returns>
        public static Int32 NextInt( this Int32 min, Int32 max ) => Instance.Next( minValue: min, maxValue: max );

        /// <summary>Return a random number somewhere in the full range of 0 to <see cref="Int16" />.</summary>
        /// <returns></returns>
        public static Int16 NextInt16( this Int16 min, Int16 max ) => ( Int16 )( min + Instance.NextDouble() * ( max - min ) );

        /// <summary>Return a random number somewhere in the full range of <see cref="Int32" />.</summary>
        /// <returns></returns>
        public static Int32 NextInt32() {
            var firstBits = Instance.Next( minValue: 0, maxValue: 1 << 4 ) << 28;
            var lastBits = Instance.Next( minValue: 0, maxValue: 1 << 28 );
            return firstBits | lastBits;
        }

        public static Int64 NextInt64() {
            var buffer = new Byte[sizeof( Int64 )];
            Instance.NextBytes( buffer: buffer );
            return BitConverter.ToInt64( value: buffer, startIndex: 0 );
        }

        /// <summary>Returns a random Single between <paramref name="min" /> and <paramref name="max" />.</summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Single NextSingle( Single min = 0, Single max = 1 ) => ( Single )( min + Instance.NextDouble() * ( max - min ) );

        public static Single NextSingle( this SingleRange singleRange ) => NextSingle( min: singleRange.Min, max: singleRange.Max );

        /// <summary>
        ///     Return a random <see cref="Span" /> between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Span NextSpan( this Span min, Span max ) {
            var tpMin = min.TotalPlanckTimes;
            var tpMax = max.TotalPlanckTimes;
            if ( tpMin > tpMax ) {
                MathExtensions.Swap( arg1: ref tpMin, arg2: ref tpMax );
            }

            var range = tpMax.Value - tpMin.Value;

            do {
                var numberOfDigits = ( UInt16 )1.Next( maxValue: range.ToString( format: "R" ).Length );

                var amount = numberOfDigits.NextBigIntegerPositive(); //BUG here

                var span = new Span( planckTimes: tpMin.Value + amount );

                if ( span >= min && span <= max ) {
                    return span;
                }
            } while ( true ); //BUG horribleness.
        }

        /// <summary>Generate a random String.</summary>
        /// <param name="length">How many characters long.</param>
        /// <param name="lowers">
        ///     <see cref="ParsingExtensions.Lowercase" />
        /// </param>
        /// <param name="uppers">
        ///     <see cref="ParsingExtensions.Uppercase" />
        /// </param>
        /// <param name="numbers">
        ///     <see cref="ParsingExtensions.Numbers" />
        /// </param>
        /// <param name="symbols">
        ///     <see cref="ParsingExtensions.Symbols" />
        /// </param>
        /// <returns></returns>
        [CanBeNull]
        public static String NextString( UInt16 length = 11, Boolean lowers = true, Boolean uppers = false, Boolean numbers = false, Boolean symbols = false ) {
            if ( !length.Any() ) {
                return null;
            }

            if ( !length.CanAllocateMemory() ) {
                return null;
            }

            var sb = new StringBuilder();

            if ( lowers ) {
                sb.Append( value: ParsingExtensions.Lowercase );
            }

            if ( uppers ) {
                sb.Append( value: ParsingExtensions.Uppercase );
            }

            if ( numbers ) {
                sb.Append( value: ParsingExtensions.Numbers );
            }

            if ( symbols ) {
                sb.Append( value: ParsingExtensions.Symbols );
            }

            var charPool = sb.ToString();
            if ( charPool.IsEmpty() ) {
                return String.Empty;
            }

            return new String( value: Enumerable.Range( start: 0, count: length ).Select( selector: i => charPool[index: 0.Next( maxValue: charPool.Length )] ).ToArray() );
        }

        /// <summary>
        ///     Returns a random TimeSpan between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> .
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

            var range = ( max - min ).Ticks;
            Assert.That( condition: min <= max );
            Assert.That( condition: range >= 0 );

            var next = range * Instance.NextDouble();
            try {
                var span = TimeSpan.FromTicks( value: ( Int64 )next );

                //TODO check for min and max int values. again. /sigh
                return span;
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.More();
                return min;
            } //return TimeSpan.FromTicks( value: min.Ticks + Instance.Next( minValue: minTicks, maxValue: maxTicks ) );
        }

        /// <summary>
        ///     Returns a random <see cref="TimeSpan" /> between <paramref name="minMilliseconds" /> and
        ///     <paramref name="maxMilliseconds" /> .
        /// </summary>
        /// <param name="minMilliseconds"></param>
        /// <param name="maxMilliseconds"></param>
        /// <returns></returns>
        public static TimeSpan NextTimeSpan( this Int32 minMilliseconds, Int32 maxMilliseconds ) =>
            TimeSpan.FromMilliseconds( value: minMilliseconds > maxMilliseconds
                ? Instance.Next( minValue: maxMilliseconds, maxValue: minMilliseconds )
                : Instance.Next( minValue: minMilliseconds, maxValue: maxMilliseconds ) );

        public static UInt64 NextUInt64() {
            var buffer = new Byte[sizeof( UInt64 )];
            Instance.NextBytes( buffer: buffer );
            return BitConverter.ToUInt64( value: buffer, startIndex: 0 );
        }

        /// <summary>Generates a uniformly random integer in the range [0, bound).</summary>
        public static BigInteger RandomIntegerBelow( this RandomNumberGenerator source, BigInteger bound ) {
            Contract.Requires<ArgumentException>( condition: source != null );
            Contract.Requires<ArgumentException>( condition: bound > 0 );

            //Contract.Ensures( Contract.Result<BigInteger>( ) >= 0 );
            //Contract.Ensures( Contract.Result<BigInteger>( ) < bound );

            //Get a byte buffer capable of holding any value below the bound
            var buffer = ( bound << 16 ).ToByteArray(); // << 16 adds two bytes, which decrease the chance of a retry later on

            //Compute where the last partial fragment starts, in order to retry if we end up in it
            var generatedValueBound = BigInteger.One << ( buffer.Length * 8 - 1 ); //-1 accounts for the sign bit
            Contract.Assert( condition: generatedValueBound >= bound );
            var validityBound = generatedValueBound - generatedValueBound % bound;
            Contract.Assert( condition: validityBound >= bound );

            while ( true ) {

                //generate a uniformly random value in [0, 2^(buffer.Length * 8 - 1))
                source.GetBytes( data: buffer );
                buffer[buffer.Length - 1] &= 0x7F; //force sign bit to positive
                var r = new BigInteger( value: buffer );

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
        public static String Randomize( [CanBeNull] this String charPool ) {
            return null == charPool ? String.Empty : charPool.OrderBy( keySelector: r => Next() ).Aggregate( seed: String.Empty, func: ( current, c ) => current + c );
        }

        /// <summary>
        ///     <para>A list containing <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        public static IEnumerable<Boolean> Randomly() {
            do {
                yield return NextBoolean();
            } while ( true );

            // ReSharper disable once FunctionNeverReturns
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>Generate a pronounceable pseudorandom string.</summary>
        /// <param name="aboutLength">Length of the returned string.</param>
        public static String RandomPronounceableString( this Int32 aboutLength ) {
            if ( aboutLength < 1 ) {
                throw new ArgumentOutOfRangeException( paramName: nameof( aboutLength ), actualValue: aboutLength, message: $"{aboutLength} is out of range." );
            }

            //char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
            //char[] vowels = { 'a', 'e', 'i', 'o', 'u' };

            var word = new StringBuilder( capacity: aboutLength * 2 ); //maximum we'll use
            var consonant = NextBoolean();
            for ( var i = 0; i < aboutLength; i++ ) {
                word.Append( value: consonant ? ParsingExtensions.Consonants[0.Next( maxValue: ParsingExtensions.Consonants.Length )] : ParsingExtensions.Vowels[0.Next( maxValue: ParsingExtensions.Vowels.Length )] );
                consonant = !consonant;
            }

            return word.ToString();
        }

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
        ///     Generate a random String using a limited set of defaults. Default
        ///     <paramref name="length" /> is 10. Default <paramref name="lowerCase" /> is true. Default
        ///     <paramref name="upperCase" /> is false. Default <paramref name="numbers" /> is false.
        ///     Default <paramref name="symbols" /> is false.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="lowerCase"></param>
        /// <param name="upperCase"></param>
        /// <param name="numbers"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static String RandomString( Int32 length = 10, Boolean lowerCase = true, Boolean upperCase = false, Boolean numbers = false, Boolean symbols = false ) {
            var charPool = String.Concat( str0: lowerCase ? ParsingExtensions.AllLowercaseLetters : String.Empty, str1: upperCase ? ParsingExtensions.AllUppercaseLetters : String.Empty,
                str2: numbers ? ParsingExtensions.Numbers : String.Empty, str3: symbols ? ParsingExtensions.Symbols : String.Empty );
            return new String( value: Enumerable.Range( start: 0, count: length ).Select( selector: i => charPool[index: 0.Next( maxValue: charPool.Length )] ).ToArray() );
        }

        public static Word RandomWord( Int32 avglength = 5, Boolean lowerCase = true, Boolean upperCase = true, Boolean numbers = false, Boolean symbols = false ) {
            var word = RandomString(( avglength - 2 ).Next( maxValue: avglength + 2 ), lowerCase: lowerCase, upperCase: upperCase, numbers: numbers, symbols: symbols );
            return new Word( word: word );
        }

        public static async Task<Boolean> Reseed( CancellationToken cancellationToken, TimeSpan? timeoutSpan = null ) {
            Int32? seed = null;

            PollResponses.Clear();

            var timeout = Task.Delay( delay: timeoutSpan ?? Seconds.Seven, cancellationToken: cancellationToken );

            var tasks = new List<Task> {
                timeout,
                Task.Run( action: () => { PollResponses.Push( item: TheFacebooks.Get().Result.Error.FbtraceID.GetHashCode() ); }, cancellationToken: cancellationToken ),
                Task.Run( action: () => { PollResponses.Push( item: RandomDotOrg.Generator.Value.Get() ); }, cancellationToken: cancellationToken )
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

            if ( seed.HasValue ) {
                ThreadSafeRandom.Value = new Lazy<Random>( valueFactory: () => new Random( Seed: seed.Value ) );
                return true;
            }

            return false;
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
            var firstNum = Instance.Next( minValue: half - quarter, maxValue: half + quarter );
            var secondNum = goal - firstNum;
            if ( firstNum > secondNum ) {
                lowResult = secondNum;
                highResult = firstNum;
            }
            else {
                lowResult = firstNum;
                highResult = secondNum;
            }

            highResult.Should().BeGreaterThan( expected: lowResult );
            ( lowResult + highResult ).Should().Be( expected: goal );
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

            highResult.Should().BeGreaterThan( expected: lowResult );
            ( lowResult + highResult ).Should().Be( expected: goal );
            return lowResult.NextDecimal( maxValue: highResult );
        }

        /// <summary>Untested.</summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private static String NextChar( [NotNull] this Char[] range ) {
            if ( range is null ) {
                throw new ArgumentNullException( paramName: nameof( range ) );
            }

            //TODO
            return range[0.Next( maxValue: range.Length )].ToString();
        }

        public static class RandomDotOrg {

            static RandomDotOrg() {
                Generator = new Lazy<IntegerGenerator>( valueFactory: () => new IntegerGenerator( num: 1 ) );
            }

            internal static Lazy<IntegerGenerator> Generator { get; }
        }

        internal static class TheFacebooks {

            public static async Task<FaceBookRootObject> Get() {
                var uri = new Uri( uriString: "http://graph.facebook.com/microsoft" );
                var reader = await InternetExtensions.DoRequestJsonAsync<FaceBookRootObject>( uri: uri );
                return reader; //;.Error.FbtraceID.GetHashCode();
            }

            public class FaceBookRootObject {

                [JsonProperty( propertyName: "error" )]
                internal FaceBookError Error { get; }
            }

            internal class FaceBookError {

                [JsonProperty( propertyName: "code" )]
                public Int32 Code { get; set; }

                [JsonProperty( propertyName: "fbtrace_id" )]
                public String FbtraceID { get; set; }

                [JsonProperty( propertyName: "message" )]
                public String Message { get; set; }

                [JsonProperty( propertyName: "type" )]
                public String Type { get; set; }
            }
        }
    }

    public static class SequenceGenerator {

        public static async Task<IEnumerable<Int32>> GenerateAsync( Int32 minValue, Int32 maxValue ) {
            if ( minValue == maxValue ) {
                return new List<Int32> { minValue };
            }

            if ( maxValue < minValue ) {
                throw new ArgumentException( message: "max cannot be less than min" );
            }

            if ( maxValue - minValue + 1 > Math.Pow( x: 10, y: 3 ) ) {
                throw new ArgumentException( message: "Range requested cannot be larger than 10,000" );
            }

            if ( minValue < -Math.Pow( x: 10, y: 8 ) || minValue > Math.Pow( x: 10, y: 8 ) ) {
                throw new ArgumentException( message: "Value of min must be between -1e9 and 1e9", paramName: nameof( minValue ) );
            }

            if ( maxValue < -Math.Pow( x: 10, y: 8 ) || maxValue > Math.Pow( x: 10, y: 8 ) ) {
                throw new ArgumentException( message: "Value of max must be between -1e9 and 1e9", paramName: nameof( maxValue ) );
            }

            var uri = $"http://www.random.org/sequences/?min={minValue}&max={maxValue}&col=1&base=10&format=plain&rnd=new";

            using ( var response = await Request.Create( uri: uri ).GetResponseAsync() ) {
                using ( var dataStream = response.GetResponseStream() ) {
                    using ( var reader = new StreamReader( stream: dataStream ) ) {
                        var responseFromServer = reader.ReadToEnd();

                        return responseFromServer.Split( '\n' ).Where( n => n.Length > 0 ).Select( selector: Int32.Parse );
                    }
                }
            }
        }
    }

    internal static class Request {

        public static WebRequest Create( String uri ) {
            var request = WebRequest.Create( requestUriString: uri );
            request.Proxy = null;
            request.Credentials = CredentialCache.DefaultCredentials;
            return request;
        }
    }
}