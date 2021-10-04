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
// Our software can be found at "https://Protiguous.com/Software"
// Our GitHub address is "https://github.com/Protiguous".

#nullable enable

namespace Librainian.Financial.Currency.BTC {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using Denominations;
	using Exceptions;
	using Librainian.Extensions;
	using Threading;

	public static class Extensions {

		/// <summary>All possible bitcoin denominations.</summary>
		// ReSharper disable once CollectionNeverUpdated.Global ReSharper disable once ReturnTypeCanBeEnumerable.Global
		public static HashSet<ICoin> PossibleCoins { get; } = new( typeof( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<ICoin>() );

		/// <summary>Deposit <paramref name="coins" /> into this wallet.</summary>
		/// <param name="coinWallet"></param>
		/// <param name="coins"></param>
		public static void Deposit( this CoinWallet coinWallet, IEnumerable<KeyValuePair<ICoin, UInt64>> coins ) {
			if ( coinWallet is null ) {
				throw new ArgumentEmptyException( nameof( coinWallet ) );
			}

			foreach ( var pair in coins ) {
				coinWallet.Deposit( pair.Key, pair.Value );
			}
		}

		public static void Fund( CoinWallet coinWallet, params KeyValuePair<ICoin, UInt64>[] sourceAmounts ) {
			if ( coinWallet is null ) {
				throw new ArgumentEmptyException( nameof( coinWallet ) );
			}

			Fund( coinWallet, sourceAmounts.AsEnumerable() );
		}

		public static void Fund( CoinWallet coinWallet, IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
			if ( coinWallet is null ) {
				throw new ArgumentEmptyException( nameof( coinWallet ) );
			}

			Parallel.ForEach( sourceAmounts, pair => coinWallet.Deposit( pair.Key, pair.Value ) );
		}

		/// <summary>
		///     Adds the optimal amount of <see cref="ICoin" />. Returns any unused portion of the money (fractions of the
		///     smallest <see cref="ICoin" />).
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="amount"></param>
		/// <param name="optimalAmountOfCoin"></param>
		public static Decimal Fund( this CoinWallet coinWallet, Decimal amount, Boolean optimalAmountOfCoin = true ) {
			if ( coinWallet is null ) {
				throw new ArgumentEmptyException( nameof( coinWallet ) );
			}

			var leftOverFund = Decimal.Zero;
			coinWallet.Deposit( optimalAmountOfCoin ? amount.Optimal( ref leftOverFund ) : amount.UnOptimal( ref leftOverFund ) );

			return leftOverFund;
		}

		/// <summary>
		///     Given the <paramref name="amount" />, return the optimal amount of <see cref="ICoin" /> (
		///     <see cref="CoinWallet.Total" />) it would take to
		///     <see cref="CoinWallet.Total" /> the <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of Pennies not accounted for.</param>
		public static IEnumerable<KeyValuePair<ICoin, UInt64>> Optimal( this Decimal amount, ref Decimal leftOverAmount ) {
			var left = new List<ICoin>( PossibleCoins );
			var result = left.ToDictionary<ICoin, ICoin, UInt64>( denomination => denomination, denomination => 0 );

			leftOverAmount += amount;

			while ( leftOverAmount > Decimal.Zero && left.Any() ) {
				var coin = left.OrderByDescending( denomination => denomination.FaceValue ).First();

				var chunks = ( UInt64 )( leftOverAmount / coin.FaceValue );

				if ( chunks > Decimal.Zero ) {
					result[coin] += chunks;
					leftOverAmount -= chunks * coin.FaceValue;
				}

				left.Remove( coin );
			}

			return result;
		}

		/// <summary>Truncate anything lesser than 1 <see cref="Satoshi" />.</summary>
		/// <param name="btc"></param>
		public static Decimal Sanitize( this Decimal btc ) {
			var sanitized = btc.ToSatoshi().ToBTC();

			//Assert.GreaterOrEqual( btc, sanitized );
			return sanitized;
		}

		public static String? SimplerBTC( this SimpleBitcoinWallet wallet ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			return wallet.Balance.SimplerBTC();
		}

		/// <summary>
		///     <para>0. 00000001 -&gt; 1 satoshi</para>
		///     <para>0. 00000011 -&gt; 11 satoshi</para>
		///     <para>0. 00000110 -&gt; 11 μBTC</para>
		/// </summary>
		/// <param name="btc"></param>
		/// <param name="coinSuffix">
		///     <para>BTC</para>
		///     <para>NMC</para>
		///     <para>etc...</para>
		/// </param>
		public static String? SimplerBTC( this Decimal btc, String coinSuffix = "BTC" ) {
			if ( coinSuffix is null ) {
				throw new ArgumentEmptyException( nameof( coinSuffix ) );
			}

			btc = btc.Sanitize();

			var list = new List<String> {
				new SimpleBitcoinWallet( btc ).ToString().TrimEnd( '0' ).TrimEnd( '.' ), $"{$"{btc.TomBTC():N6}".TrimEnd( '0' ).TrimEnd( '.' )} m{coinSuffix}",
				$"{$"{btc.ToμBtc():N4}".TrimEnd( '0' ).TrimEnd( '.' )} μ{coinSuffix}", $"{btc.ToSatoshi():N0} satoshi"
			};

			//as btc

			//as mbtc

			//as μbtc

			//as satoshi
			var chosen = list.OrderBy( s => s.Length ).FirstOrDefault();

			return chosen;
		}

		/// <summary>Create a TPL dataflow task for depositing large volumes of money.</summary>
		/// <param name="coinWallet"></param>
		/// <param name="sourceAmounts"></param>
		public static Task StartDeposit( CoinWallet coinWallet, IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
			if ( coinWallet is null ) {
				throw new ArgumentEmptyException( nameof( coinWallet ) );
			}

			var actionBlock = new ActionBlock<KeyValuePair<ICoin, UInt64>>( pair => coinWallet.Deposit( pair.Key, pair.Value ),
				Blocks.ManyProducers.ConsumeSensible( default( CancellationToken? ) ) );

			Parallel.ForEach( sourceAmounts, pair => actionBlock.Post( pair ) );
			actionBlock.Complete();

			return actionBlock.Completion;
		}

		/// <summary>
		///     Transfer everything FROM the <paramref name="source" /><see cref="CoinWallet" /> into this
		///     <paramref name="target" /><see cref="CoinWallet" />.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static Task<ConcurrentDictionary<ICoin, UInt64>> StartTransfer( this CoinWallet source, CoinWallet target ) =>
			Task.Run( () => new ConcurrentDictionary<ICoin, UInt64>( Transfer( source, target ) ) );

		public static Decimal ToBTC( this Int16 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

		public static Decimal ToBTC( this Int32 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

		public static Decimal ToBTC( this Int64 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

		public static Decimal TomBTC( this Decimal btc ) => btc * SimpleBitcoinWallet.mBTCInOneBTC;

		public static Int64 ToSatoshi( this Decimal btc ) => ( Int64 )( btc * SimpleBitcoinWallet.SatoshiInOneBtc );

		/// <summary>Return the <paramref name="wallet" /> in Satoshi.</summary>
		/// <param name="wallet"></param>
		public static Int64 ToSatoshi( this SimpleBitcoinWallet wallet ) => wallet.Balance.ToSatoshi();

		public static Decimal ToμBtc( this Decimal btc ) => btc * SimpleBitcoinWallet.ΜBtcInOneBtc;

		public static IEnumerable<KeyValuePair<ICoin, UInt64>> Transfer( this CoinWallet source, CoinWallet target ) {
			if ( source is null ) {
				throw new ArgumentEmptyException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentEmptyException( nameof( target ) );
			}

			var transferred = new ConcurrentDictionary<ICoin, UInt64>();

			foreach ( var pair in source ) {
				if ( !source.Transfer( target, pair ) ) {
					continue;
				}

				var denomination = pair.Key;
				var count = pair.Value;
				transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
			}

			return transferred;
		}

		public static Boolean Transfer( this CoinWallet source, CoinWallet target, KeyValuePair<ICoin, UInt64> denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentEmptyException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentEmptyException( nameof( target ) );
			}

			return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) &&
				   target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value ) > 0;
		}

		/// <summary>Create a TPL dataflow task for depositing large volumes of money into this wallet.</summary>
		/// <param name="coinWallet"></param>
		/// <param name="sourceAmounts"></param>
		public static Task Transfer( CoinWallet coinWallet, IEnumerable<KeyValuePair<ICoin, UInt64>>? sourceAmounts ) {
			if ( coinWallet is null ) {
				throw new ArgumentEmptyException( nameof( coinWallet ) );
			}

			var actionBlock = new ActionBlock<KeyValuePair<ICoin, UInt64>>( pair => coinWallet.Deposit( pair.Key, pair.Value ),
				Blocks.ManyProducers.ConsumeSensible( default( CancellationToken? ) ) );

			actionBlock.Complete();

			return actionBlock.Completion;
		}

		/// <summary>
		///     Given the <paramref name="amount" />, return the unoptimal amount of <see cref="ICoin" /> (
		///     <see cref="CoinWallet.Total" />) it would take to
		///     <see cref="CoinWallet.Total" /> the <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of coin not accounted for.</param>
		public static IEnumerable<KeyValuePair<ICoin, UInt64>> UnOptimal( this Decimal amount, ref Decimal leftOverAmount ) {
			var left = new List<ICoin>( PossibleCoins );
			var result = left.ToDictionary<ICoin, ICoin, UInt64>( denomination => denomination, denomination => 0 );

			leftOverAmount += amount;

			while ( leftOverAmount > Decimal.Zero && left.Any() ) {
				var coin = left.OrderBy( denomination => denomination.FaceValue ).First();

				var chunks = ( UInt64 )( leftOverAmount / coin.FaceValue );

				if ( chunks > Decimal.Zero ) {
					result[coin] += chunks;
					leftOverAmount -= chunks * coin.FaceValue;
				}

				left.Remove( coin );
			}

			return result;
		}
	}
}