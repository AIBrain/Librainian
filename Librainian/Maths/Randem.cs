// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "Randem.cs" last formatted on 2021-11-30 at 7:19 PM by Protiguous.

#nullable enable

namespace Librainian.Maths;

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
using Exceptions;
using Linguistics;
using Measurement.Spatial;
using Measurement.Time;
using Numbers;
using OperatingSystem.Compression;
using Parsing;
using Ranges;

public static class Randem {

	private const Byte MiddleByte = Byte.MaxValue / 2;

	/// <summary>A Double-sized byte buffer per-thread.</summary>
	private static readonly ThreadLocal<Byte[]> ThreadLocalByteBuffer = new( () => new Byte[ sizeof( Double ) ], true );

	public const Byte One = 1;

	public const Byte Zero = 0;

	internal static ConcurrentStack<Int32> PollResponses { get; } = new();

	/// <summary>Provide to each thread its own <see cref="Random" /> with a random seed.</summary>
	internal static ThreadLocal<Random> ThreadSafeRandom { get; } =
		new( () => new Random( HashCode.Combine( DateTime.UtcNow.GetHashCode(), Environment.CurrentManagedThreadId.GetHashCode() ) ) );

	//internal static AsyncLocal<Random> ThreadSafeRandomAsync { get; } = new(); //TOD O. If this is used in async, then it still needs initialized. So maybe not use it?

	public static ConcurrentDictionary<Type, String[]> EnumDictionary { get; } = new();

	/// <summary>
	/// <para>More cryptographically strong than <see cref="Random" />.</para>
	/// </summary>
	public static ThreadLocal<RandomNumberGenerator> RNG { get; } = new( RandomNumberGenerator.Create, true );

	/// <summary>A thread-local (threadsafe) <see cref="Random" />.</summary>
	private static Random Instance() => ThreadSafeRandom.Value!;

	public static String BinaryString( Int32 length ) {
		if ( length <= 0 ) {
			return String.Empty;
		}

		var bytes = new Byte[ length ];
		Instance().NextBytes( bytes );

		return String.Concat( bytes.Select( b => b <= MiddleByte ? Zero : One ) );
	}

	/// <summary>Chooses a random element in the given collection <paramref name="items" />.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="items">The collection of items to choose a random element from.</param>
	/// <returns>A randomly chosen element in the given collection <paramref name="items" />.</returns>
	public static T Choose<T>( params T[] items ) => items[ items.Length.NextInt() ];

	/// <summary>Chooses a random element in the given set of items.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="a">The first item.</param>
	/// <returns>A randomly chosen element in the given set of items.</returns>
	public static T? Choose<T>( this T? a ) => a;

	/// <summary>Chooses a random element in the given set of items.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="a">The first item.</param>
	/// <param name="b">The second item.</param>
	/// <returns>A randomly chosen element in the given set of items.</returns>
	public static T? Choose<T>( this T? a, T? b ) =>
		2.NextInt() switch {
			0 => a,
			var _ => b
		};

	/// <summary>Chooses a random element in the given set of items.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="a">The first item.</param>
	/// <param name="b">The second item.</param>
	/// <param name="c">The third item.</param>
	/// <returns>A randomly chosen element in the given set of items.</returns>
	public static T? Choose<T>( this T? a, T? b, T? c ) =>
		3.NextInt() switch {
			0 => a,
			1 => b,
			var _ => c
		};

	/// <summary>Chooses a random element in the given set of items.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="a">The first item.</param>
	/// <param name="b">The second item.</param>
	/// <param name="c">The third item.</param>
	/// <param name="d">The fourth item.</param>
	/// <returns>A randomly chosen element in the given set of items.</returns>
	public static T? Choose<T>( this T? a, T? b, T? c, T? d ) =>
		4.NextInt() switch {
			0 => a,
			1 => b,
			2 => c,
			var _ => d
		};

	/// <summary>Chooses a random element in the given set of items.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="a">The first item.</param>
	/// <param name="b">The second item.</param>
	/// <param name="c">The third item.</param>
	/// <param name="d">The fourth item.</param>
	/// <param name="e">The fifth item.</param>
	/// <returns>A randomly chosen element in the given set of items.</returns>
	public static T? Choose<T>( this T? a, T? b, T? c, T? d, T? e ) =>
		5.NextInt() switch {
			0 => a,
			1 => b,
			2 => c,
			3 => d,
			var _ => e
		};

	/// <summary>Chooses a random element in the given set of items.</summary>
	/// <typeparam name="T">The Type of element.</typeparam>
	/// <param name="a">The first item.</param>
	/// <param name="b">The second item.</param>
	/// <param name="c">The third item.</param>
	/// <param name="d">The fourth item.</param>
	/// <param name="e">The fifth item.</param>
	/// <param name="f">The sixth item.</param>
	/// <returns>A randomly chosen element in the given set of items.</returns>
	public static T? Choose<T>( this T? a, T? b, T? c, T? d, T? e, T? f ) {
		var index = 6.NextInt();

		return index switch {
			0 => a,
			1 => b,
			2 => c,
			3 => d,
			4 => e,
			var _ => f
		};
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
	public static T? Choose<T>( this T? a, T? b, T? c, T? d, T? e, T? f, T? g ) {
		var index = 7.NextInt();

		return index switch {
			0 => a,
			1 => b,
			2 => c,
			3 => d,
			4 => e,
			5 => f,
			var _ => g
		};
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
	public static T? Choose<T>( this T? a, T? b, T? c, T? d, T? e, T? f, T? g, T? h ) {
		var index = 8.NextInt();

		return index switch {
			0 => a,
			1 => b,
			2 => c,
			3 => d,
			4 => e,
			5 => f,
			6 => g,
			var _ => h
		};
	}

	[MethodImpl( MethodImplOptions.NoInlining )]
	public static Double DoBusyWork( this UInt64 iterations ) {
		Double work = 0;

		for ( var i = 0ul; i < iterations; i++ ) {
			work += 1001.671;
		}

		return work;
	}

	public static IEnumerable<Int32> GenerateRandom( Int32 count, Int32 min, Int32 max ) {
		if ( !count.Any() ) {
			throw new ArgumentOutOfRangeException( $"{nameof( count )} is out of range ({count.DoubleQuote()} is less than 1)." );
		}

		if ( max < min ) {
			Common.Swap( ref min, ref max );
		}

		if ( count > max - min && ( max - min ).Any() ) {
			throw new ArgumentOutOfRangeException( $"Count {count} is out of range." );
		}

		return Enumerable.Range( min, count ).Select( _ => min.Next( max ) );
	}

	public static Char GetChar( this RandomNumberGenerator rng ) {
		if ( rng is null ) {
			throw new NullException( nameof( rng ) );
		}

		var data = new Byte[ sizeof( Char ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToChar( data, 0 );
	}

	public static Double GetDouble( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( Double ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToDouble( data, 0 );
	}

	public static Int16 GetInt16( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( Int16 ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToInt16( data, 0 );
	}

	public static Int32 GetInt32( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( Int32 ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToInt32( data, 0 );
	}

	public static Int64 GetInt64( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( Int64 ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToInt64( data, 0 );
	}

	/// <summary>memoize?</summary>
	/// <typeparam name="T"></typeparam>
	public static String[] GetNames<T>() {
		var key = typeof( T );

		if ( EnumDictionary.TryGetValue( key, out var values ) ) {
			return values;
		}

		values = Enum.GetNames( key );
		EnumDictionary.TryAdd( key, values );

		return values;
	}

	public static Decimal GetRandomness( this Action<Byte[]> randomFunc, UInt16 bytesToTest ) {
		var buffer = new Byte[ bytesToTest ];
		randomFunc( buffer );

		var compressed = buffer.Compress();

		var result = compressed.LongLength / ( Decimal )buffer.LongLength;

		return result;
	}

	public static Single GetSingle( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( Single ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToSingle( data, 0 );
	}

	public static UInt16 GetUInt16( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( UInt16 ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToUInt16( data, 0 );
	}

	public static UInt32 GetUInt32( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( UInt32 ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToUInt32( data, 0 );
	}

	public static UInt64 GetUInt64( this RandomNumberGenerator rng ) {
		var data = new Byte[ sizeof( UInt64 ) ];
		rng.GetNonZeroBytes( data );

		return BitConverter.ToUInt64( data, 0 );
	}

	/// <summary>Generate a random number between <paramref name="minValue" /> and <paramref name="maxValue" /> .</summary>
	/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
	/// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Int32 Next( this Int32 minValue, Int32 maxValue ) => Instance().Next( minValue, maxValue );

	/// <summary>
	/// <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
	/// </summary>
	/// <param name="maxValue"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Int32 Next( this Int32 maxValue ) => Instance().Next( maxValue );

	public static String Next( this String[] strings ) => strings[ strings.Length.Next() ];

	/// <summary>
	/// <para>Returns a nonnegative random number less than <paramref name="maxValue" />.</para>
	/// </summary>
	/// <param name="maxValue"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static UInt16 Next( this UInt16 maxValue ) => ( UInt16 )Instance().Next( maxValue );

	/// <summary>Generate a random number between <paramref name="range.Min" /> and <paramref name="range.Max" /> .</summary>
	/// <param name="range"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Int32 Next( this Int32Range range ) => Instance().Next( range.Minimum, range.Maximum );

	/// <summary>Returns a nonnegative random number.</summary>
	public static UInt32 Next() => ( UInt32 )( Instance().NextDouble() * UInt32.MaxValue );

	/// <summary>Generate a random number between <paramref name="minValue" /> and <paramref name="maxValue" /> .</summary>
	/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
	/// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
	public static UInt64 Next( this UInt64 minValue, UInt64 maxValue ) {
		var min = Math.Min( minValue, maxValue );
		var max = Math.Max( minValue, maxValue );

		return min + ( UInt64 )( Instance().NextDouble() * ( max - min ) );
	}

	/// <summary>Generate a random number between <paramref name="minValue" /> and <paramref name="maxValue" /> .</summary>
	/// <param name="minValue">The inclusive lower bound of the random number returned.</param>
	/// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
	public static Int64 Next( this Int64 minValue, Int64 maxValue ) {
		var min = Math.Min( minValue, maxValue );
		var max = Math.Max( minValue, maxValue );

		return min + ( Int64 )( Instance().NextDouble() * ( max - min ) );
	}

	/// <summary>Needs unit tests.</summary>
	/// <param name="numberOfDigits"></param>
	public static BigInteger NextBigInteger( this UInt16 numberOfDigits ) {
		if ( numberOfDigits <= 0 ) {
			return BigInteger.Zero;
		}

		var buffer = new Byte[ numberOfDigits ];
		Instance().NextBytes( buffer );

		return new BigInteger( buffer );
	}

	/// <summary>Needs unit tests.</summary>
	/// <param name="numberOfDigits"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static BigInteger NextBigIntegerPositive( this UInt16 numberOfDigits ) {
		if ( numberOfDigits <= 0 ) {
			return BigInteger.Zero;
		}

		var buffer = new Byte[ numberOfDigits ];
		Instance().NextBytes( buffer );
		buffer[ ^1 ] &= 0x7f; //force sign bit to positive according to http://stackoverflow.com/a/17367241/956364

		return new BigInteger( buffer );
	}

	/// <summary>Needs unit tests.</summary>
	/// <param name="numberOfDigits"></param>
	public static BigInteger NextBigIntegerSecure( this UInt16 numberOfDigits ) {
		if ( numberOfDigits <= 0 ) {
			return BigInteger.Zero;
		}

		var buffer = new Byte[ numberOfDigits ];

		RNG.Value!.GetBytes( buffer ); //BUG is this correct? I think it is, but http://stackoverflow.com/questions/2965707/c-sharp-a-random-bigint-generator suggests a "numberOfDigits/8" here.

		return new BigInteger( buffer );
	}

	public static Boolean NextBoolean() => Instance().NextDouble() >= 0.5;

	/// <summary>
	/// <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
	/// </summary>
	public static Byte NextByte( this Byte min, Byte max ) => ( Byte )( min + Instance().NextDouble() * ( max - min ) );

	/// <summary>
	/// <para>Returns a random <see cref="Byte" />.</para>
	/// </summary>
	public static Byte NextByte() {
		var buffer = new Byte[ 1 ];
		Instance().NextBytes( buffer );

		return buffer[ 0 ];
	}

	/// <summary>
	/// <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
	/// </summary>
	public static IEnumerable<Byte> NextBytes( this Byte min, Byte max ) {
		yield return min.NextByte( max );
	}

	/// <summary>
	/// <para>Returns a random <see cref="Byte" /> between <paramref name="min" /> and <paramref name="max" />.</para>
	/// </summary>
	public static void NextBytes( this Byte min, Byte max, ref Byte[] buffer ) {
		if ( buffer == null ) {
			throw new NullException( nameof( buffer ) );
		}

		Instance().NextBytes( buffer );

		for ( var p = 0; p < max; p++ ) {
			while ( buffer[ p ] < min || buffer[ p ] > max ) {
				buffer[ p ] = min.NextByte( max );
			}
		}
	}

	public static void NextBytes( ref Byte[] buffer ) {
		if ( buffer is null ) {
			throw new NullException( nameof( buffer ) );
		}

		Instance().NextBytes( buffer );
	}

	/// <summary>Untested.</summary>
	/// <param name="range"></param>
	public static Char NextChar( this Char[] range ) {
		if ( range is null ) {
			throw new NullException( nameof( range ) );
		}

		return range[ 0.Next( range.Length ) ];
	}

	/// <summary></summary>
	/// <param name="alpha"></param>
	/// <param name="lowEnd"></param>
	/// <param name="highEnd"></param>
	public static Color NextColor( Byte alpha = 255, Byte lowEnd = 0, Byte highEnd = 255 ) =>
		Color.FromArgb( alpha, Next( lowEnd, highEnd ), Next( lowEnd, highEnd ), Next( lowEnd, highEnd ) );

	public static DateTime NextDateTime( this DateTime value, TimeSpan timeSpan ) =>
		value + new Milliseconds( ( Decimal )( timeSpan.TotalMilliseconds * Instance().NextDouble() ) );

	public static DateTime NextDateTime( this DateTime earlier, DateTime later ) {
		var range = earlier > later ? earlier - later : later - earlier;

		return earlier + new Milliseconds( ( Decimal )range.TotalMilliseconds );
	}

	public static DateTimeOffset NextDateTimeOffset( this DateTimeOffset value, TimeSpan timeSpan ) =>
		value + new Milliseconds( ( Decimal )( timeSpan.TotalMilliseconds * Instance().NextDouble() ) );

	/// <summary>Between <see cref="Decimal.Zero" /> and <see cref="Decimal.One" />.</summary>
	public static Decimal NextDecimal() {
		do {
			try {
				return NextDecimal( Decimal.Zero, Decimal.One );
			}
			catch ( ArgumentOutOfRangeException ) { }
		} while ( true );
	}

	/// <summary>
	/// <para>Returns a random Decimal between <paramref name="minimum" /> and <paramref name="maximum" />.</para>
	/// </summary>
	/// <param name="minimum"></param>
	/// <param name="maximum"></param>
	public static Decimal NextDecimal( this Decimal minimum, Decimal maximum ) =>
		minimum <= maximum ? minimum + NextDecimal() * ( maximum - minimum ) : maximum + NextDecimal() * ( minimum - maximum );

	public static Decimal NextDecimal( this DecimalRange decimalRange ) => decimalRange.Minimum.NextDecimal( decimalRange.Maximum );

	/// <summary></summary>
	public static Decimal NextDecimalFullRange() {
		do {
			try {
				return new Decimal( RandomInt32(), RandomInt32(), RandomInt32(), NextBoolean(), ( Byte )0.Next( 29 ) );
			}
			catch ( ArgumentOutOfRangeException ) { }
		} while ( true );
	}

	public static Degrees NextDegrees() => new( NextSingle( Degrees.MinimumValue, Degrees.MaximumValue ) );

	/// <summary>
	/// <para>Returns a random digit between 0 and 9.</para>
	/// </summary>
	/// <note>10 mod 10 is 0.</note>
	/// <note>255 mod 10 is 5.</note>
	public static Digit NextDigit() {
		var n = ( Byte )( NextByte() % 10 );
		return n;
	}

	/// <summary>Returns a random digit (0,1,2,3,4,5,6,7,8,9) between <paramref name="min" /> and <paramref name="max" />.</summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public static Digit NextDigit( this Digit min, Digit max ) {
		if ( min == null ) {
			throw new NullException( nameof( min ) );
		}

		if ( max == null ) {
			throw new NullException( nameof( max ) );
		}

		unchecked {
			if ( min > max ) {
				Common.Swap( ref min, ref max );
			}

			Byte result;

			do {
				result = ( Byte )NextDigit();
			} while ( min > result || result > max );

			return new Digit( result );
		}
	}

	/*

	/// <summary>
	/// <para>Returns a random Double between <paramref name="range.Min" /> and <paramref name="range.Max" />.</para>
	/// </summary>
	/// <param name="range"></param>
	/// <returns></returns>
	public static Double NextDouble( this DoubleRange range ) => range.Min + Instance().NextDouble() * range.Length;
	*/

	//public static Double NextDouble( this PairOfDoubles variance ) => NextDouble( variance.Low, variance.High );

	/// <summary>Returns a random Double between <paramref name="min" /> and <paramref name="max" />.</summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static Double NextDouble( Double min = 0.0d, Double max = 1d ) {
		if ( Double.IsNaN( min ) ) {
			throw new ArgumentOutOfRangeException( nameof( min ), $"{nameof( min )} is a NaN." );
		}

		if ( Double.IsInfinity( min ) ) {
			throw new ArgumentOutOfRangeException( nameof( min ), $"{nameof( min )} is an Infinity." );
		}

		if ( Double.IsNaN( max ) ) {
			throw new ArgumentOutOfRangeException( nameof( max ), $"{nameof( max )} is a NaN." );
		}

		if ( Double.IsInfinity( max ) ) {
			throw new ArgumentOutOfRangeException( nameof( max ), $"{nameof( max )} is an Infinity." );
		}

		if ( min > max ) {
			Common.Swap( ref min, ref max );
		}

		var range = max - min;

		if ( Double.IsNaN( range ) ) {
			throw new ArgumentOutOfRangeException( nameof( range ), $"{nameof( max )}-{nameof( min )} produced a NaN." );
		}

		Double result;

		if ( !Double.IsInfinity( range ) ) {
			result = min + Instance().NextDouble() * range;

			//result.Should().BeInRange( minimumValue: min, maximumValue: max );

			return result;
		}

		do {
			Instance().NextBytes( ThreadLocalByteBuffer.Value! );
			result = BitConverter.ToDouble( ThreadLocalByteBuffer.Value!, 0 );
		} while ( Double.IsInfinity( result ) || Double.IsNaN( result ) );

		//result.Should().BeInRange( minimumValue: min, maximumValue: max );

		return result;
	}

	/// <summary>Returns a random Double between 0 and 1</summary>
	public static Double NextDouble() => Instance().NextDouble();

	/// <summary></summary>
	/// <typeparam name="T"></typeparam>
	public static T? NextEnum<T>() where T : struct {
		if ( !typeof( T ).IsEnum ) {
			return default( T? );
		}

		var names = GetNames<T>();
		var picked = names[ Instance().Next( 0, names.Length ) ];

		return ( T )Enum.Parse( typeof( T ), picked );
	}

	/// <summary>
	/// Returns a random <see cref="Single" /> between <paramref name="range.Min" /> and <paramref name="range.Max" /> .
	/// </summary>
	/// <param name="range"></param>
	public static Single NextFloat( this SingleRange range ) => ( Single )( range.Min + Instance().NextDouble() * range.Length );

	/// <summary>Returns a random float between <paramref name="minimum" /> and <paramref name="maximum" />.</summary>
	/// <param name="minimum"></param>
	/// <param name="maximum"></param>
	public static Single NextFloat( Single minimum = 0, Single maximum = 1 ) => RandomSingle( minimum, maximum );

	public static Guid NextGuid() => Guid.NewGuid();

	/// <summary>Gets a non-negative random whole number less than the specified <paramref cref="maximum" />.</summary>
	/// <param name="maximum">The exclusive upper bound the random number to be generated.</param>
	/// <returns>A non-negative random integer less than the specified <paramref cref="maximum" />.</returns>
	public static Int32 NextInt( this Int32 maximum ) => Instance().Next( maximum );

	/// <summary>Gets a random number within a specified range.</summary>
	/// <param name="min">The inclusive lower bound of the random number returned.</param>
	/// <param name="max">The exclusive upper bound of the random number returned.</param>
	/// <returns>A random integer within a specified range.</returns>
	public static Int32 NextInt( this Int32 min, Int32 max ) => Instance().Next( min, max );

	/// <summary>Return a random number somewhere in the full range of 0 to <see cref="Int16" />.</summary>
	public static Int16 NextInt16( this Int16 min, Int16 max ) => ( Int16 )( min + Instance().NextDouble() * ( max - min ) );

	/// <summary>Returns a random Single between <paramref name="min" /> and <paramref name="max" />.</summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public static Single NextSingle( Single min = 0, Single max = 1 ) => RandomSingle( min, max );

	public static Single NextSingle( this SingleRange singleRange ) => NextSingle( singleRange.Min, singleRange.Max );

	/// <summary>Return a random <see cref="SpanOfTime" /> between <paramref name="min" /> and <paramref name="max" />.</summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public static SpanOfTime NextSpan( this SpanOfTime min, SpanOfTime max ) {
		var tpMin = min.CalcTotalPlanckTimes();
		var tpMax = max.CalcTotalPlanckTimes();

		if ( tpMin > tpMax ) {
			Common.Swap( ref tpMin, ref tpMax );
		}

		var range = tpMax.Value - tpMin.Value;

		do {
			var numberOfDigits = ( UInt16 )1.Next( $"{range}".Length );

			var amount = numberOfDigits.NextBigIntegerPositive(); //BUG here

			var span = new SpanOfTime( tpMin.Value + amount );

			if ( span >= min && span <= max ) {
				return span;
			}
		} while ( true ); //BUG fix this horribleness.
	}

	/// <summary>Generate a random String.</summary>
	/// <param name="length">How many characters long.</param>
	/// <param name="lowers"><see cref="ParsingConstants.English.Alphabet.Lowercase" /></param>
	/// <param name="uppers"><see cref="ParsingConstants.English.Alphabet.Uppercase" /></param>
	/// <param name="numbers"><see cref="ParsingConstants.English.Numbers" /></param>
	/// <param name="symbols"><see cref="ParsingConstants.Strings.Symbols" /></param>
	public static String NextString( Int32 length, Boolean lowers = true, Boolean uppers = false, Boolean numbers = false, Boolean symbols = false ) {
		if ( !length.Any() ) {
			return String.Empty;
		}

		var toParseLength = ( lowers ? ParsingConstants.English.Alphabet.Lowercase.Length : 0 ) + ( uppers ? ParsingConstants.English.Alphabet.Uppercase.Length : 0 ) +
							( numbers ? ParsingConstants.English.Numbers.Length : 0 ) + ( symbols ? ParsingConstants.Strings.Symbols.Length : 0 );

		var sb = new StringBuilder( toParseLength, toParseLength );

		if ( lowers ) {
			sb.Append( ParsingConstants.English.Alphabet.Lowercase );
		}

		if ( uppers ) {
			sb.Append( ParsingConstants.English.Alphabet.Uppercase );
		}

		if ( numbers ) {
			sb.Append( ParsingConstants.English.Numbers );
		}

		if ( symbols ) {
			sb.Append( ParsingConstants.Strings.Symbols );
		}

		var charPool = sb.ToString();

		if ( !charPool.Any() ) {
			return String.Empty;
		}

		return new String( ParallelEnumerable.Range( 0, length ).AsUnordered().Select( _ => charPool[ 0.Next( charPool.Length ) ] ).ToArray() );
	}

	/// <summary>Returns a random TimeSpan between <paramref name="minValue" /> and <paramref name="maxValue" /> .</summary>
	/// <param name="minValue"></param>
	/// <param name="maxValue"></param>
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

		do {
			try {
				var range = ( max - min ).Ticks;

				var next = range * Instance().NextDouble();

				return min + TimeSpan.FromTicks( ( Int64 )next );
			}
			catch ( ArgumentOutOfRangeException ) { }
		} while ( true );
	}

	/// <summary>
	/// Returns a random <see cref="TimeSpan" /> between <paramref name="minMilliseconds" /> and <paramref
	/// name="maxMilliseconds" /> .
	/// </summary>
	/// <param name="minMilliseconds"></param>
	/// <param name="maxMilliseconds"></param>
	public static TimeSpan NextTimeSpan( this Int32 minMilliseconds, Int32 maxMilliseconds ) =>
		TimeSpan.FromMilliseconds( minMilliseconds > maxMilliseconds ? Instance().Next( maxMilliseconds, minMilliseconds ) :
			Instance().Next( minMilliseconds, maxMilliseconds ) );

	public static UInt64 NextUInt64() {
		var buffer = new Byte[ sizeof( UInt64 ) ];
		Instance().NextBytes( buffer );

		return BitConverter.ToUInt64( buffer, 0 );
	}

	public static IEnumerable<Boolean> RandomBooleans() {
		do {
			yield return NextBoolean();
		} while ( true );

		// ReSharper disable once IteratorNeverReturns
	}

	public static IEnumerable<Boolean> RandomBooleans( Int32 iterations ) => Enumerable.Range( 0, iterations ).Select( _ => NextBoolean() );

	public static Double RandomDouble( Double min = Double.MinValue, Double max = Double.MaxValue ) => min + Instance().NextDouble() * ( max - min );

	public static Guid RandomGuid() => Guid.NewGuid();

	public static Int16 RandomInt16( Int16 min = Int16.MinValue, Int16 max = Int16.MaxValue ) => ( Int16 )( min + Instance().NextDouble() * ( max - min ) );

	/// <summary>
	/// Return a random number in the full range of <see cref="Int32" /> between <param name="min"></param> and <param
	/// name="max"></param> .
	/// </summary>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public static Int32 RandomInt32( Int32 min = Int32.MinValue, Int32 max = Int32.MaxValue ) {
		var firstBits = Instance().Next( 0, 1 << 4 ) << 28;
		var lastBits = Instance().Next( 0, 1 << 28 );
		var result = min + ( firstBits | lastBits ) * ( max - min );

		return result;
	}

	/// <summary>Return a random number somewhere in the full range of <see cref="Int32" />.</summary>
	public static Int32 RandomInt32() {
		var buffer = new Byte[ sizeof( Int32 ) ];
		Instance().NextBytes( buffer );
		return BitConverter.ToInt32( buffer, 0 );
	}

	/// <summary>Return a random number somewhere in the full range of <see cref="Int32" />.</summary>
	public static Int32 RandomInt32Alt( Int32 min = Int32.MinValue, Int32 max = Int32.MaxValue ) {
		var buffer = new Byte[ sizeof( Int32 ) ];
		Instance().NextBytes( buffer );

		//TODO This needs tested/confirmed.
		var sample = 1.0d / BitConverter.ToInt32( buffer, 0 );
		var range = ( Int64 )max - min;
		return ( Int32 )( sample * range + min );
	}

	public static Int64 RandomInt64( Int64 min = Int64.MinValue, Int64 max = Int64.MaxValue ) => ( Int64 )( min + Instance().NextDouble() * ( max - min ) );

	/// <summary>Generates a uniformly random integer in the range [0, bound).</summary>
	public static BigInteger RandomIntegerBelow( this RandomNumberGenerator source, BigInteger bound ) {

		//Contract.Requires<NullException>( source != null );
		//Contract.Requires<NullException>( bound > 0 );

		//Contract.Ensures( Contract.Result<BigInteger>( ) >= 0 );
		//Contract.Ensures( Contract.Result<BigInteger>( ) < bound );

		//Get a byte buffer capable of holding any value below the bound
		var buffer = ( bound << 16 ).ToByteArray(); // << 16 adds two bytes, which decrease the chance of a retry later on

		//Compute where the last partial fragment starts, in order to retry if we end up in it
		var generatedValueBound = BigInteger.One << ( buffer.Length * 8 - 1 ); //-1 accounts for the sign bit

		//Contract.Assert( generatedValueBound >= bound );
		var validityBound = generatedValueBound - generatedValueBound % bound;

		//Contract.Assert( validityBound >= bound );

		while ( true ) {

			//generate a uniformly random value in [0, 2^(buffer.Length * 8 - 1))
			source.GetBytes( buffer );
			buffer[ ^1 ] &= 0x7F; //force sign bit to positive
			var r = new BigInteger( buffer );

			//return unless in the partial fragment
			if ( r >= validityBound ) {
				continue;
			}

			return r % bound;
		}
	}

	/// <summary>Return the <paramref name="enumerable" /> in a random order (shuffle).</summary>
	/// <param name="enumerable"></param>
	public static IEnumerable<T> Randomize<T>( this IEnumerable<T> enumerable ) => enumerable.OrderBy( _ => Next() );

	/// <summary>Generate a pronounceable pseudorandom string.</summary>
	/// <param name="aboutLength">Length of the returned string.</param>
	public static String RandomPronounceableString( this Int32 aboutLength ) {
		if ( aboutLength < 1 ) {
			throw new ArgumentOutOfRangeException( nameof( aboutLength ), aboutLength, $"{aboutLength} is out of range." );
		}

		//char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
		//char[] vowels = { 'a', 'e', 'i', 'o', 'u' };

		var word = new StringBuilder( aboutLength * 2 ); //maximum we'll use
		var consonant = NextBoolean();

		for ( var i = 0; i < aboutLength; i++ ) {
			word.Append( consonant ? ParsingConstants.English.Consonants[ 0.Next( ParsingConstants.English.Consonants.Length ) ] :
				ParsingConstants.English.Vowels[ 0.Next( ParsingConstants.English.Vowels.Length ) ] );

			consonant = !consonant;
		}

		return word.ToString();
	}

	/// <summary></summary>
	/// <param name="wordCount"></param>
	/// <returns></returns>
	public static String RandomSentence( Byte wordCount = 7 ) {
		var list = new List<Word>();

		var low = wordCount.Half();
		wordCount += wordCount.Half();

		for ( var i = 0; i < Next( low, wordCount + 1 ); i++ ) {
			list.Add( RandomWord() );
		}

		return list.ToStrings( ParsingConstants.Chars.Space, ParsingConstants.Chars.Period ).Capitialize() ?? String.Empty;
	}

	public static Single RandomSingle( Single min = Single.MinValue, Single max = Single.MaxValue ) => ( Single )( min + Instance().NextDouble() * ( max - min ) );

	/// <summary>Given the string <paramref name="pool" />, return the letters in a random order.</summary>
	/// <param name="pool"></param>
	public static String RandomString( this String pool ) {
		var result = new StringBuilder( pool.Length );

		foreach ( var c in pool.OrderBy( r => Next() ) ) {
			result.Append( c );
		}

		return result.ToString();
	}

	/*
	public static Int64 NextInt64() {
		var buffer = new Byte[sizeof( Int64 )];
		Instance().NextBytes( buffer );

		return BitConverter.ToInt64( buffer, 0 );
	}
	*/

	/// <summary>
	/// Generate a random String using a limited set of defaults. Default <paramref name="length" /> is 10. Default <paramref
	/// name="lowerCase" /> is true. Default <paramref name="upperCase" /> is false. Default <paramref name="numbers" /> is
	/// false. Default <paramref name="symbols" /> is false.
	/// </summary>
	/// <param name="length"></param>
	/// <param name="lowerCase"></param>
	/// <param name="upperCase"></param>
	/// <param name="numbers"></param>
	/// <param name="symbols"></param>
	public static String RandomString( Int32 length = 16, Boolean lowerCase = true, Boolean upperCase = false, Boolean numbers = false, Boolean symbols = false ) {
		var charPool = String.Concat( lowerCase ? ParsingConstants.English.Alphabet.Lowercase : String.Empty,
			upperCase ? ParsingConstants.English.Alphabet.Uppercase : String.Empty, numbers ? ParsingConstants.English.Numbers : String.Empty,
			symbols ? ParsingConstants.Strings.Symbols : String.Empty );

		var poolLength = charPool.Length;

		return new String( Enumerable.Range( 0, length ).Select( i => charPool[ 0.Next( poolLength ) ] ).ToArray() );
	}

	[Pure]
	public static Word RandomWord( Int32 avglength, Boolean lowerCase, Boolean upperCase, Boolean numbers, Boolean symbols ) {
		var low = avglength.Half();
		var high = avglength + low;
		var word = RandomString( low.Next( high ), lowerCase, upperCase, numbers, symbols );

		return new Word( word );
	}

	[Pure]
	public static Word RandomWord( Int32 avglength = 7 ) {
		var low = avglength.Half();
		var high = avglength + low;

		return new( RandomPronounceableString( low.Next( high ) ) );
	}

	/// <summary>Generate two random numbers about halfway of <param name="goal"></param> .</summary>
	/// <remarks>Given one number, return two random numbers that add up to <param name="goal"></param></remarks>
	public static void Split( this Int32 goal, out Int32 lowResult, out Int32 highResult ) {
		var half = goal.Half();
		var quarter = half.Half();
		var firstNum = Instance().Next( half - quarter, half + quarter );
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
	/// <para>Generate two random numbers about halfway of <paramref name="goal" />.</para>
	/// <para>Also, return a random number between <paramref name="lowResult" /> and <paramref name="highResult" /></para>
	/// </summary>
	/// <remarks>Given one number, return two random numbers that add up to <param name="goal"></param></remarks>
	public static Decimal Split( this Decimal goal, out Decimal lowResult, out Decimal highResult ) {
		var half = goal.Half();
		var quarter = half.Half();
		var firstNum = ( half - quarter ).NextDecimal( half + quarter );
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

		return lowResult.NextDecimal( highResult );
	}
}