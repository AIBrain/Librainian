﻿// Copyright © Protiguous. All Rights Reserved.
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
// File "IntegerGenerator.cs" last formatted on 2021-11-30 at 7:18 PM by Protiguous.

namespace Librainian.Internet.RandomOrg;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Exceptions;
using static Common;

public static class RandomDotOrg {

	private static readonly Lazy<Double> HighEnd = new( () => Math.Pow( 10, 8 ) );

	private static readonly Lazy<Double> LowEnd = new( () => -Math.Pow( 10, 8 ) );

	private static readonly Lazy<Double> ValidRange = new( () => Math.Pow( 10, 3 ) );

	static RandomDotOrg() => CancellationTokenSource = new();

	internal static Lazy<IntegerGenerator> Generator { get; } = new( () => new( 1, CancellationTokenSource.Token ) );

	public static CancellationTokenSource CancellationTokenSource { get; }

	public static async ValueTask<IEnumerable<Int32>> SequenceGenerator( this Int32 minValue, Int32 maxValue, CancellationToken cancellationToken ) {
		if ( maxValue < minValue ) {
			Swap( ref minValue, ref maxValue );
		}

		if ( maxValue - minValue + 1 > ValidRange.Value ) {
			throw new NullException( $"Range requested cannot be larger than {ValidRange.Value}." );
		}

		if ( minValue < LowEnd.Value || minValue > HighEnd.Value ) {
			throw new OutOfRangeException( $"Value of {nameof( minValue )} must be between {LowEnd.Value:R} and {HighEnd.Value}." );
		}

		if ( maxValue < LowEnd.Value || maxValue > HighEnd.Value ) {
			throw new NullException( $"Value of {nameof( maxValue )} must be between {LowEnd.Value:R} and {HighEnd.Value}." );
		}

		var url = new Uri( $"https://random.org/sequences/?min={minValue}&max={maxValue}&col=1&base=10&format=plain&rnd=new", UriKind.Absolute );

		var responseFromServer = await url.GetWebPageAsync( cancellationToken ).ConfigureAwait( false );

		if ( responseFromServer is null ) {
			return Enumerable.Empty<Int32>();
		}

		return responseFromServer.Split( '\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries ).Where( s => s.Any() ).Select( Int32.Parse );
	}

	/// <see cref="http://github.com/OrigamiTech/Random.org/blob/master/Random.org/IntegerGenerator.cs" />
	public class IntegerGenerator {

		private const Int32 BaseDefault = 10;

		private const Int32 ColDefault = 1;

		private const Int32 ColMax = 1000000000;

		private const Int32 Max = 1000000000;

		private const Int32 Min = -1000000000;

		private const Int32 NumMax = 10000;

		private const Int32 NumMin = 1;

		private Int32 _index;

		public IntegerGenerator( CancellationToken cancellationToken ) => this.Init( NumMax, Min, Max, ColDefault, BaseDefault, cancellationToken ).Wait( cancellationToken );

		public IntegerGenerator( Int32 num, CancellationToken cancellationToken ) =>
			this.Init( num, Min, Max, ColDefault, BaseDefault, cancellationToken ).Wait( cancellationToken );

		public IntegerGenerator( Int32 num, Int32 min, CancellationToken cancellationToken ) =>
			this.Init( num, min, Max, ColDefault, BaseDefault, cancellationToken ).Wait( cancellationToken );

		public IntegerGenerator( Int32 num, Int32 min, Int32 max, CancellationToken cancellationToken ) =>
			this.Init( num, min, max, ColDefault, BaseDefault, cancellationToken ).Wait( cancellationToken );

		public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col, CancellationToken cancellationToken ) =>
			this.Init( num, min, max, col, BaseDefault, cancellationToken ).Wait( cancellationToken );

		public IntegerGenerator( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase, CancellationToken cancellationToken ) =>
			this.Init( num, min, max, col, inbase, cancellationToken ).Wait( cancellationToken );

		private List<Int32> Ints { get; } = new();

		private async Task Init( Int32 num, Int32 min, Int32 max, Int32 col, Int32 inbase, CancellationToken cancellationToken ) {
			if ( num is < NumMin or > NumMax ) {
				throw new ArgumentOutOfRangeException( nameof( num ), "The number of random numbers to generate must be between 1 and 10000." );
			}

			if ( min < Min ) {
				throw new ArgumentOutOfRangeException( nameof( min ), "The random number lower bound must be between -1000000000 and 1000000000." );
			}

			if ( max > Max ) {
				throw new ArgumentOutOfRangeException( nameof( max ), "The random number upper bound must be between -1000000000 and 1000000000." );
			}

			if ( max <= min ) {
				throw new ArgumentOutOfRangeException( nameof( min ), "The random number upper bound must be greater than the lower bound." );
			}

			if ( col is <= 0 or > ColMax ) {
				throw new ArgumentOutOfRangeException( nameof( col ), "The column count must be between 1 and 1000000000." );
			}

			if ( inbase is not 2 and not 8 and not 10 and not 16 ) {
				throw new ArgumentOutOfRangeException( nameof( inbase ), "The base must be 2, 8, 10, or 16." );
			}

			Uri address = new( $"http://www.random.org/integers/?num={num}&min={min}&max={max}&col={col}&base={inbase}&format=plain&rnd=new" );

			//if ( job is null ) {
			//	throw new InvalidOperationException( "Unable to pull random numbers from Random.Org." );
			//}

			var toParse = await address.GetWebPageAsync( cancellationToken ).ConfigureAwait( false );

			if ( toParse is null ) {
				return;
			}

			foreach ( var s in Regex.Split( toParse, @"\D" ) ) {
				try {
					if ( !String.IsNullOrWhiteSpace( s ) ) {
						this.Ints.Add( Convert.ToInt32( s, inbase ) );
					}
				}
				catch { }
			}
		}

		public Int32 Get() {
			this._index++;
			this._index %= this.Ints.Count;

			return this.Ints[ this._index ];
		}
	}
}