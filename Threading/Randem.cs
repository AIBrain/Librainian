
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// "Librainian/Randem.cs" was last cleaned by Rick on 2014/08/11 at 2:14 PM

namespace Librainian.Threading {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Collections;
    using Extensions;
    using FluentAssertions;
    using IO;
    using JetBrains.Annotations;
    using Linguistics;
    using Maths;
    using Measurement.Time;
    using NUnit.Framework;
    using Parsing;

    public static class Randem {
        /// <summary>
        /// Provide to each thread its own <see cref="Random" /> with a random seed.
        /// </summary>
        public static readonly ThreadLocal<Random> ThreadSafeRandom = new ThreadLocal<Random>( valueFactory: () => {
            var hash = ThreadingExtensions.ThreadLocalSHA256Managed.Value.ComputeHash( Guid.NewGuid().ToByteArray() );
            var seed = BitConverter.ToInt32( value: hash, startIndex: 0 );
            return new Random( seed.GetHashMerge( Thread.CurrentThread.ManagedThreadId ) );
        }, trackAllValues: false );

        private static readonly ThreadLocal<Byte[]> LocalByteBuffer = new ThreadLocal<byte[]>( () => new byte[ sizeof(Double) ], false );

        /// <summary>
        /// </summary>
        [NotNull]
        public static ConcurrentDictionary<Type, string[]> EnumDictionary { get; }
        = new ConcurrentDictionary<Type, String[]>();

        /// <summary>
        /// A thread-local (threadsafe) <see cref="Random" />.
        /// </summary>
        [NotNull]
        public static Random Instance { get; }
        = ThreadSafeRandom.Value;

        [NotNull]
        public static ThreadLocal<byte[]> LocalUInt64Buffers { get; }
        = new ThreadLocal<byte[]>( valueFactory: () => new byte[ sizeof(UInt64) ], trackAllValues: false );
        /// <summary>
        /// <para>More cryptographically strong than <see cref="Random"/>.</para>
        /// </summary>
        [NotNull]
        public static ThreadLocal<RandomNumberGenerator> RNG { get; }
        = new ThreadLocal<RandomNumberGenerator>( () => new RNGCryptoServiceProvider() );

        [NotNull]
        private static ThreadLocal<byte[]> LocalInt64Buffers { get; }
        = new ThreadLocal<byte[]>( valueFactory: () => new byte[ sizeof(Int64) ], trackAllValues: false );

        internal static void AddToList( [NotNull] ConcurrentBag<int> list ) {
            if ( list == null ) {
                throw new ArgumentNullException( "list" );
            }
            Parallel.ForEach( 1.To( 128 ), ThreadingExtensions.Parallelism, i => list.Add( Next( minValue: Int32.MinValue, maxValue: Int32.MaxValue ) ) );
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Double DoBusyWork( this UInt64 iterations ) {
            Double work = 0;
            for ( var i = 0ul ; i < iterations ; i++ ) {
                work += 1001.671;
            }
            return work;
        }

        [System.Diagnostics.Contracts.Pure]
        public static Char GetChar( this RandomNumberGenerator rng ) {
            if ( rng == null ) {
                throw new ArgumentNullException( "rng" );
            }
            var data = new byte[ sizeof(Char) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToChar( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static Double GetDouble( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(Double) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToDouble( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static Int16 GetInt16( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(Int16) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToInt16( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static Int32 GetInt32( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(Int32) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToInt32( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static Int64 GetInt64( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(Int64) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToInt64( data, 0 );
        }


        /// <summary>
        /// memoize?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static String[] GetNames<T>() {
            var key = typeof(T);
            String[] values;
            if ( EnumDictionary.TryGetValue( key, out values ) ) {
                return values;
            }
            values = Enum.GetNames( key );
            EnumDictionary.TryAdd( key, values );
            return values;
        }

        public static Percentage GetRandomness( this Action<byte[]> randomFunc, UInt16 bytesToTest ) {

            var buffer = new byte[ bytesToTest ];
            randomFunc( buffer );

            var compressed = buffer.Compress();

            var result = new Percentage( ( BigInteger )compressed.LongLength, buffer.LongLength );

            return result;
        }

        [System.Diagnostics.Contracts.Pure]
        public static Single GetSingle( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(Single) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToSingle( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static UInt16 GetUInt16( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(UInt16) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToUInt16( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static UInt32 GetUInt32( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(UInt32) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToUInt32( data, 0 );
        }

        [System.Diagnostics.Contracts.Pure]
        public static UInt64 GetUInt64( this RandomNumberGenerator rng ) {
            var data = new byte[ sizeof(UInt64) ];
            rng.GetNonZeroBytes( data );
            return BitConverter.ToUInt64( data, 0 );
        }
        /// <summary>
        /// Generate a random number between <paramref name="minValue" /> and <paramref
        /// name="maxValue" />.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Next( int minValue, int maxValue ) => Instance.Next( minValue: minValue, maxValue: maxValue );

        /// <summary>
        /// <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Next( int maxValue ) => Instance.Next( maxValue );

        /// <summary>
        /// <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static UInt16 Next( UInt16 maxValue ) => ( UInt16 )( Instance.Next( maxValue: maxValue ) );

        /// <summary>
        /// Generate a random number between <paramref name="range.Min" /> and <paramref
        /// name="range.Max" />.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int Next( this Int32Range range ) => Instance.Next( minValue: range.Min, maxValue: range.Max );

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns></returns>
        public static UInt32 Next() => ( UInt32 )( Instance.NextDouble() * UInt32.MaxValue );

        /// <summary>
        /// Generate a random number between <paramref name="minValue" /> and <paramref
        /// name="maxValue" />.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public static UInt64 Next( UInt64 minValue, UInt64 maxValue ) {
            var min = Math.Min( minValue, maxValue );
            var max = Math.Max( minValue, maxValue );
            return min + ( UInt64 )( Instance.NextDouble() * ( max - min ) );
        }

        /// <summary>
        /// Generate a random number between <paramref name="minValue" /> and <paramref
        /// name="maxValue" />.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
        /// <returns></returns>
        public static Int64 Next( Int64 minValue, Int64 maxValue ) {
            var min = Math.Min( minValue, maxValue );
            var max = Math.Max( minValue, maxValue );
            return min + ( Int64 )( Instance.NextDouble() * ( max - min ) );
        }

        /// <summary>
        /// </summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static BigInteger NextBigInteger( UInt16 numberOfDigits ) {
            numberOfDigits.Should().BeGreaterThan( 0 );
            if ( numberOfDigits <= 0 ) {
                throw new ArgumentOutOfRangeException( "numberOfDigits" );
            }

            var buffer = new Byte[ numberOfDigits ];
            Instance.NextBytes( buffer );
            return new BigInteger( buffer );
        }

        /// <summary>
        /// </summary>
        /// <param name="numberOfDigits"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static BigInteger NextBigIntegerPositive( UInt16 numberOfDigits ) {
            numberOfDigits.Should().BeGreaterThan( 0 );
            if ( numberOfDigits <= 0 ) {
                throw new ArgumentOutOfRangeException( "numberOfDigits" );
            }

            var buffer = new Byte[ numberOfDigits ];
            Instance.NextBytes( buffer );
            buffer[ buffer.Length - 1 ] &= 127; //force sign bit to positive according to http://stackoverflow.com/a/17367241/956364
            return new BigInteger( buffer );
        }

        public static BigInteger NextBigIntegerSecure( UInt16 numberOfDigits ) {
            numberOfDigits.Should().BeGreaterThan( 0 );
            if ( numberOfDigits <= 0 ) {
                throw new ArgumentOutOfRangeException( "numberOfDigits" );
            }

            var buffer = new Byte[ numberOfDigits ];
            RNG.Value.GetBytes( buffer ); //BUG is this correct? I think it is, but http://stackoverflow.com/questions/2965707/c-sharp-a-random-bigint-generator suggests a "numberOfDigits/8" here.
            return new BigInteger( buffer );
        }

        /// <summary>
        /// <para>Generate a random <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Boolean NextBoolean() => Instance.NextDouble() > 0.5D;

        /// <summary>
        /// <para>Generate a random <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Boolean NextBooleanFast() => Instance.Next( 2 ) == 0;

        /// <summary>
        /// <para>Returns a random <see cref="Byte" />.</para>
        /// </summary>
        /// <returns></returns>
        public static Byte NextByte() {
            unchecked {
                Byte[] buffer = { 0 };
                Instance.NextBytes( buffer );
                return buffer[ 0 ];
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="lowEnd"></param>
        /// <param name="highEnd"></param>
        /// <returns></returns>
        public static Color NextColor( Byte alpha = 255, Byte lowEnd = 0, Byte highEnd = 255 ) => Color.FromArgb( alpha: alpha, red: Next( lowEnd, highEnd ), green: Next( lowEnd, highEnd ), blue: Next( lowEnd, highEnd ) );

        /// <summary>
        /// Between 0 and 1.
        /// </summary>
        /// <returns></returns>
        public static Decimal NextDecimal() {
            do {
                try {
                    return ( Decimal )Instance.NextDouble();    //TODO meh. fake it for now.
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.More();
                }
            } while ( true );
        }

        /// <summary>
        /// <para>Returns a random Double between <paramref name="minValue" /> and <paramref name="maxValue" />.</para>
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static Decimal NextDecimal( Decimal minValue, Decimal maxValue ) {
            var min = Math.Min( minValue, maxValue );
            var max = Math.Max( minValue, maxValue );
            var range = max - min;
            return min + ( NextDecimal() * range );
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static Decimal NextDecimalFullRange() {
            do {
                try {
                    return new Decimal( NextInt32(), NextInt32(), NextInt32(), NextBoolean(), ( byte )Next( 0, 9 ) );
                }
                catch ( ArgumentOutOfRangeException exception ) {
                    exception.More();
                }
            } while ( true );
        }

        /// <summary>
        /// <para>Returns a random Double between <paramref name="range.Min" /> and <paramref name="range.Max" />.</para>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Double NextDouble( this DoubleRange range ) => range.Min + ( Instance.NextDouble() * range.Length );

        [UsedImplicitly]
        public static Double NextDouble( PairOfDoubles variance ) => NextDouble( min: variance.Low, max: variance.High );

        /// <summary>
        /// Returns a random Double between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Double NextDouble( Double min = 0.0, Double max = 1.0 ) {
            var range = max - min;
            if ( Double.IsNaN( range ) ) {
                throw new ArgumentOutOfRangeException();
            }
            Double result;

            if ( !Double.IsInfinity( range ) ) {
                result = min + ( Instance.NextDouble() * range );
                result.Should().BeInRange( min, max );
                return result;
            }

            do {
                Instance.NextBytes( LocalByteBuffer.Value );
                result = BitConverter.ToDouble( LocalByteBuffer.Value, 0 );
            } while ( Double.IsInfinity( result ) || Double.IsNaN( result ) );

            result.Should().BeInRange( min, max );

            return result;
        }

        /// <summary>
        /// Returns a random Double beetween 0 and 1
        /// </summary>
        /// <returns></returns>
        [Pure]
        public static Double NextDouble() => Instance.NextDouble();

        /// <summary>
        /// Returns a random <see cref="Single" /> between <paramref name="range.Min" /> and
        /// <paramref name="range.Max" />.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static Single NextFloat( this SingleRange range ) => ( Single )( range.Min + ( Instance.NextDouble() * range.Length ) );

        /// <summary>
        /// Returns a random float between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float NextFloat( float min = 0, float max = 1 ) => ( float )( min + ( Instance.NextDouble() * ( max - min ) ) );

        public static Guid NextGuid() => Guid.NewGuid();

        public static Guid NextGuid( Decimal minValue ) => Guid.NewGuid();
        /// <summary>
        /// Return a random number somewhere in the full range of <see cref="Int32"/>.
        /// </summary>
        /// <returns></returns>
        public static Int32 NextInt32() {
            unchecked {
                var firstBits = Instance.Next( 0, 1 << 4 ) << 28;
                var lastBits = Instance.Next( 0, 1 << 28 );
                return firstBits | lastBits;
            }
        }

        public static Int64 NextInt64() {
            Instance.NextBytes( LocalInt64Buffers.Value );
            return BitConverter.ToInt64( value: LocalInt64Buffers.Value, startIndex: 0 );
        }

        /// <summary>
        /// Returns a random Single between <paramref name="min" /> and <paramref name="max" />.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Single NextSingle( Single min = 0, Single max = 1 ) => ( Single )( min + ( Instance.NextDouble() * ( max - min ) ) );

        public static Decimal NextDecay( Decimal min = MathExtensions.EpsilonDecimal, Decimal max = 1 ) => min + ( NextDecimal() * ( max - min ) );

        /// <summary>
        /// Generate a random String.
        /// </summary>
        /// <param name="length">How many characters long.</param>
        /// <param name="lowers"><see cref="ParsingExtensions.Lowercase"/></param>
        /// <param name="uppers"><see cref="ParsingExtensions.Uppercase"/></param>
        /// <param name="numbers"><see cref="ParsingExtensions.Numbers"/></param>
        /// <param name="symbols"><see cref="ParsingExtensions.Symbols"/></param>
        /// <returns></returns>
        public static String NextString( int length = 11, Boolean lowers = false, Boolean uppers = false, Boolean numbers = false, Boolean symbols = false ) {

            var sb = new StringBuilder();
            if ( lowers ) {
                sb.Append( ParsingExtensions.Lowercase );
            }
            if ( uppers ) {
                sb.Append( ParsingExtensions.Uppercase );
            }
            if ( numbers ) {
                sb.Append( ParsingExtensions.Numbers );
            }
            if ( symbols ) {
                sb.Append( ParsingExtensions.Symbols );
            }

            var charPool = sb.ToString();
            return new String( Enumerable.Range( 0, length ).Select( i => charPool[ Next( 0, charPool.Length ) ] ).ToArray() );
        }

        /// <summary>
        /// Returns a random TimeSpan between <paramref name="minValue" /> and <paramref
        /// name="maxValue" />.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static TimeSpan NextTimeSpan( TimeSpan minValue, TimeSpan maxValue ) {
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
            var range = max.Subtract( min ).Ticks;
            Assert.That( min <= max );
            Assert.That( range >= 0 );

            var next = range * Instance.NextDouble();
            try {
                var span = TimeSpan.FromTicks( ( long )next );

                //TODO check for min and max int values. again. /sigh
                return span;
            }
            catch ( ArgumentOutOfRangeException exception ) {
                exception.More();
                return min;
            } //return TimeSpan.FromTicks( value: min.Ticks + Instance.Next( minValue: minTicks, maxValue: maxTicks ) );
        }

        /// <summary>
        /// Returns a random Double between <paramref name="minMilliseconds" /> and <paramref
        /// name="maxMilliseconds" />.
        /// </summary>
        /// <param name="minMilliseconds"></param>
        /// <param name="maxMilliseconds"></param>
        /// <returns></returns>
        public static TimeSpan NextTimeSpan( int minMilliseconds, int maxMilliseconds ) => TimeSpan.FromMilliseconds( minMilliseconds > maxMilliseconds ? Instance.Next( maxMilliseconds, minMilliseconds ) : Instance.Next( minMilliseconds, maxMilliseconds ) );

        public static UInt64 NextUInt64() {
            Instance.NextBytes( LocalUInt64Buffers.Value );
            return BitConverter.ToUInt64( value: LocalUInt64Buffers.Value, startIndex: 0 );
        }

        public static DateTime RandomDateTime( this DateTime value, TimeSpan timeSpan ) => value + new Milliseconds( timeSpan.TotalMilliseconds * Instance.NextDouble() );

        public static DateTime RandomDateTime( this DateTime earlier, DateTime later ) {
            if ( earlier > later ) {
                Utility.Swap( ref earlier, ref later );
            }
            var range = later - earlier;
            return earlier + new Milliseconds( range.TotalMilliseconds );
        }

        public static DateTimeOffset RandomDateTimeOffset( this DateTimeOffset value, TimeSpan timeSpan ) => value + new Milliseconds( timeSpan.TotalMilliseconds * Instance.NextDouble() );

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomEnum<T>() where T : struct {
            if ( !typeof(T).IsEnum ) {
                return default(T);
            }
            var vals = GetNames<T>();
            var rand = Instance.Next( 0, vals.Length );
            var picked = vals[ rand ];
            return ( T )Enum.Parse( typeof(T), picked );
        }

        /// <summary>
        /// Given the String <paramref name="charPool" />, return the letters in a random fashion.
        /// </summary>
        /// <param name="charPool"></param>
        /// <returns></returns>
        public static String Randomize( [CanBeNull] this String charPool ) => null == charPool ? String.Empty : charPool.OrderBy( r => Next() ).Aggregate( String.Empty, ( current, c ) => current + c );

        /// <summary>
        /// <para>A list containing <see cref="Boolean.True" /> or <see cref="Boolean.False" />.</para>
        /// </summary>
        public static IEnumerable<Boolean> Randomly() {
            do {
                yield return NextBoolean();
            } while ( true );

            // ReSharper disable once FunctionNeverReturns
        }

        public static Sentence RandomSentence( int avgWords = 8 ) {
            var list = new List<Word>();

            if ( NextBoolean() ) {
                ++avgWords;
            }
            else if ( NextBoolean() ) {
                --avgWords;
            }

            for ( var i = 0 ; i < avgWords ; i++ ) {
                list.Add( RandomWord() );
            }
            return new Sentence( list );
        }

        /// <summary>
        /// Generate a random String using a limited set of defaults. Default <paramref
        /// name="length" /> is 10. Default <paramref name="lowerCase" /> is true. Default <paramref
        /// name="upperCase" /> is false. Default <paramref name="numbers" /> is false. Default
        /// <paramref name="symbols" /> is false.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="lowerCase"></param>
        /// <param name="upperCase"></param>
        /// <param name="numbers"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static String RandomString( int length = 10, Boolean lowerCase = true, Boolean upperCase = false, Boolean numbers = false, Boolean symbols = false ) {
            var charPool = String.Concat( lowerCase ? ParsingExtensions.AllLowercaseLetters : String.Empty, upperCase ? ParsingExtensions.AllUppercaseLetters : String.Empty, numbers ? ParsingExtensions.Numbers : String.Empty, symbols ? ParsingExtensions.Symbols : String.Empty );
            return new String( Enumerable.Range( 0, length ).Select( i => charPool[ Next( 0, charPool.Length ) ] ).ToArray() );
        }

        public static Word RandomWord( int avglength = 5, Boolean lowerCase = true, Boolean upperCase = true, Boolean numbers = false, Boolean symbols = false ) {
            var word = RandomString( Next( avglength - 2, avglength + 2 ), lowerCase, upperCase, numbers, symbols );
            return new Word( word );
        }

        /// <summary>
        /// Generate two random numbers about halfway of <param name="goal"></param> .
        /// </summary>
        /// <remarks>Given one number, return two random numbers that add up to <param name="goal"></param></remarks>
        public static void Split( this int goal, out int lowResult, out int highResult ) {
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
            highResult.Should().BeGreaterThan( lowResult );
            ( lowResult + highResult ).Should().Be( goal );
        }

        /// <summary>
        /// <para>Generate two random numbers about halfway of <paramref name="goal"/>.</para>
        /// <para>Also, return a random number between <paramref name="lowResult"/> and <paramref name="highResult"/></para>
        /// </summary>
        /// <remarks>Given one number, return two random numbers that add up to <param name="goal"></param></remarks>
        public static Decimal Split( this Decimal goal, out Decimal lowResult, out Decimal highResult ) {
            var half = goal.Half();
            var quarter = half.Half();
            var firstNum = NextDecimal( minValue: half - quarter, maxValue: half + quarter );
            var secondNum = goal - firstNum;
            if ( firstNum > secondNum ) {
                lowResult = secondNum;
                highResult = firstNum;
            }
            else {
                lowResult = firstNum;
                highResult = secondNum;
            }
            highResult.Should().BeGreaterThan( lowResult );
            ( lowResult + highResult ).Should().Be( goal );
            return NextDecimal( lowResult, highResult );
        }

        [StructLayout(LayoutKind.Explicit), UsedImplicitly]
        internal class UInt64VsDouble {
            [FieldOffset(0)]
            public readonly Double value;
            [FieldOffset(0)]
            public readonly ulong valueulong;
        }
        /*
                public static Date RandomDate( this Date oldest, Date youngest ) {
                    if ( youngest > oldest ) {
                        Utility.Swap( ref youngest, ref oldest ); //make 'youngest' the more recent value
                    }
                    var range = later - earlier;
                    return earlier + new Milliseconds( range.TotalMilliseconds );
                }
        */
    }
}