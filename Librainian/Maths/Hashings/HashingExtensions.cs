// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "HashingExtensions.cs" last formatted on 2020-08-14 at 8:35 PM.

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