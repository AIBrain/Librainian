#nullable enable

namespace Librainian.Maths.Hashings {

	using System;
	using System.Linq;
	using Converters;
	using JetBrains.Annotations;
	using OperatingSystem.FileSystem;
	using OperatingSystem.FileSystem.Pri.LongPath;

	public static class HashingExtensions {

		[ThreadStatic]
		private static Random? RandomInstance;

		/// <summary>Returns argument increased to the nearest number divisible by 16</summary>
		public static Int32 Align16( this Int32 i ) {
			var r = i & 15; // 00001111

			return r == 0 ? i : i + ( 16 - r );
		}

		/// <summary>Returns argument increased to the nearest number divisible by 16</summary>
		public static Int64 Align16( this Int64 i ) {
			var r = i & 15; // 00001111

			return r == 0 ? i : i + ( 16 - r );
		}

		/// <summary>Returns argument increased to the nearest number divisible by 8</summary>
		public static Int32 Align8( this Int32 i ) {
			var r = i & 7; // 00000111

			return r == 0 ? i : i + ( 8 - r );
		}

		/// <summary>Returns argument increased to the nearest number divisible by 8</summary>
		public static Int64 Align8( this Int64 i ) {
			var r = i & 7; // 00000111

			return r == 0 ? i : i + ( 8 - r );
		}

		/// <summary>poor mans crc</summary>
		/// <param name="fileInfo"></param>
		/// <returns></returns>
		public static Int32 CalcHashCode( [NotNull] this FileInfo fileInfo ) {
			if ( fileInfo is null ) {
				throw new ArgumentNullException( nameof( fileInfo ) );
			}

			return fileInfo.AsBytes().Aggregate( 0, ( current, b ) => ( current, b ).GetHashCode() );
		}

		/// <summary>poor mans crc</summary>
		/// <param name="document"></param>
		/// <returns></returns>
		public static Int32 CalcHashCode( [NotNull] this Document document ) {
			if ( document is null ) {
				throw new ArgumentNullException( nameof( document ) );
			}

			var fileInfo = new FileInfo( document.FullPath );

			if ( fileInfo is null ) {
				throw new NullReferenceException( "fileInfo" );
			}

			return fileInfo.AsBytes().Aggregate( 0, ( current, b ) => ( current, b ).GetHashCode() );
		}

		/// <summary>Takes one <see cref="UInt64" />, and returns another Deterministic <see cref="UInt64" />.</summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static UInt64 Deterministic( this UInt64 index ) {
			var translate64 = new Translate64 {
				UnsignedValue = index
			};

			var bufferA = new Byte[sizeof( Int32 )];
			new Random( translate64.SignedLow ).NextBytes( bufferA );

			var bufferB = new Byte[sizeof( Int32 )];
			new Random( translate64.SignedHigh ).NextBytes( bufferB );

			translate64.SignedLow = Convert.ToInt32( bufferA );
			translate64.SignedHigh = Convert.ToInt32( bufferB );

			return translate64.UnsignedValue;
		}

		/// <summary>
		///     Takes one <see cref="Int32" />, and returns another Deterministic <see cref="Int32" /> by using the
		///     <see cref="Random" /> generator.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <remarks>
		///     //TODO A faster Random.Reseed would be nice here.
		/// </remarks>
		public static Int32 Deterministic( this Int32 index ) {
			RandomInstance ??= new Random( index );

			return RandomInstance.Next();
		}

		/// <summary>Takes one <see cref="Int64" />, and returns another Deterministic <see cref="Int64" />.</summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Int64 Deterministic( this Int64 index ) {
			var translate64 = new Translate64 {
				SignedValue = index
			};

			var bufferA = new Byte[sizeof( Int32 )];
			new Random( translate64.SignedLow ).NextBytes( bufferA );

			var bufferB = new Byte[sizeof( Int32 )];
			new Random( translate64.SignedHigh ).NextBytes( bufferB );

			translate64.SignedLow = Convert.ToInt32( bufferA );
			translate64.SignedHigh = Convert.ToInt32( bufferB );

			return translate64.SignedValue;
		}

		public static UInt16 GetHashCodeUInt16<TLeft>( [NotNull] this TLeft objectA ) => ( UInt16 )objectA!.GetHashCode();

		public static UInt32 GetHashCodeUInt32<TLeft>( [NotNull] this TLeft objectA ) => ( UInt32 )objectA!.GetHashCode();

		public static UInt64 GetHashCodeUInt64<TLeft>( [NotNull] this TLeft objectA ) => ( UInt64 )objectA!.GetHashCode();

	}

}