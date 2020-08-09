// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//     No warranties are expressed, implied, or given.
//     We are NOT responsible for Anything You Do With Our Code.
//     We are NOT responsible for Anything You Do With Our Executables.
//     We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Maths {

	using System;
	using System.Threading;
	using JetBrains.Annotations;

	/// <summary>Based from Microsoft Reference random.cs sources. This is purely for learning purposes.</summary>
	public class MicrosoftsRandom {

		private const Int32 Middle = 0b1001101001001110110010000110;

		/// <summary>55.. why?</summary>
		private const Byte Special = 0b110111;

		/// <summary>56.. why?</summary>
		private const Byte SpecialLength = 0b111000;

		[ThreadStatic]
		private static Int32 inext;

		[ThreadStatic]
		private static Int32 inextp;

		[ThreadStatic]
		private static Int32[] SeedArray;

		private MicrosoftsRandom() : this( Middle ) { }

		public MicrosoftsRandom( Int32 seed ) => Seed( seed );

		private static Int32 InternalSample() {
			unchecked {
				var locINext = inext;
				var locINextp = inextp;

				if ( ++locINext >= SpecialLength ) {
					locINext = 1;
				}

				if ( ++locINextp >= SpecialLength ) {
					locINextp = 1;
				}

				var retVal = SeedArray[ locINext ] - SeedArray[ locINextp ];

				SeedArray[ locINext ] = retVal;

				inext = locINext;
				inextp = locINextp;

				return retVal;
			}
		}

		protected virtual Double Sample() => InternalSample() * ( 1.0 / Int32.MaxValue );

		public static void Seed( Int32 seed ) {
			unchecked {
				if ( SeedArray is null || SeedArray.Length != SpecialLength ) {
					SeedArray = new Int32[ SpecialLength ];
				}

				var mj = Middle - ( seed ^ Thread.CurrentThread.ManagedThreadId.GetHashCode() );

				SeedArray[ Special ] = mj;

				var mk = 1;

				for ( var i = 0; i < Special; i++ ) {
					var ii = ( 21 * i ) % Special;
					SeedArray[ ii ] = mk;
					mk = mj - mk;
					mj = SeedArray[ ii ];
				}

				for ( var k = 1; k < 5; k++ ) {
					for ( var i = 1; i < SpecialLength; i++ ) {
						SeedArray[ i ] -= SeedArray[ 1 + ( ( i + 30 ) % Special ) ];
					}
				}

				inext = 0;
				inextp = 21;
			}
		}

		public virtual Int32 Next() => InternalSample();

		/*
		public virtual Int32 Next( Int32 minValue, Int32 maxValue ) {
			if ( minValue > maxValue ) {
				minValue.Swap( maxValue );
			}

			var range = ( Int64 )maxValue - minValue;
			if ( range <= Int32.MaxValue ) {
				return ( Int32 )( this.Sample() * range ) + minValue;
			}
			else {
				return ( Int32 )( ( Int64 )( this.GetSampleForLargeRange() * range ) + minValue );
			}
		}
		*/

		public virtual Int32 Next( Int32 maxValue ) {
			if ( maxValue < 0 ) {
				throw new ArgumentOutOfRangeException( nameof( maxValue ) );
			}

			return ( Int32 )( this.Sample() * maxValue );
		}

		/*=====================================Next=====================================
	**Returns: A double [0..1)
	**Arguments: None
	**Exceptions: None
	==============================================================================*/

		public virtual void NextBytes( [NotNull] Byte[] buffer ) {
			if ( buffer is null ) {
				throw new ArgumentNullException( nameof( buffer ) );
			}

			for ( var i = 0; i < buffer.Length; i++ ) {
				buffer[ i ] = ( Byte )( InternalSample() % ( Byte.MaxValue + 1 ) );
			}
		}

		public virtual Double NextDouble() => this.Sample();

		/*==================================NextBytes===================================
	**Action:  Fills the byte array with random bytes [0..0x7f].  The entire array is filled.
	**Returns:Void
	**Arugments:  buffer -- the array to be filled.
	**Exceptions: None
	==============================================================================*/
	}
}