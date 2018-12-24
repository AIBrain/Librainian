// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Extensions.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "Extensions.cs" was last formatted by Protiguous on 2018/07/13 at 1:21 AM.

namespace Librainian.Measurement.Currency.BTC {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using Denominations;
	using JetBrains.Annotations;
	using Librainian.Extensions;
	using NUnit.Framework;
	using Threading;

	public static class Extensions {

		/// <summary>All possible bitcoin denominations.</summary>
		[NotNull]

		// ReSharper disable once CollectionNeverUpdated.Global ReSharper disable once ReturnTypeCanBeEnumerable.Global
		public static HashSet<ICoin> PossibleCoins { get; } = new HashSet<ICoin>( typeof( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<ICoin>() );

		/// <summary>Deposit <paramref name="coins" /> into this wallet.</summary>
		/// <param name="coinWallet"></param>
		/// <param name="coins"></param>
		public static void Deposit( [NotNull] this CoinWallet coinWallet, IEnumerable<KeyValuePair<ICoin, UInt64>> coins = null ) {
			if ( coinWallet == null ) { throw new ArgumentNullException( nameof( coinWallet ) ); }

			coins = coins ?? Enumerable.Empty<KeyValuePair<ICoin, UInt64>>();

			foreach ( var pair in coins ) { coinWallet.Deposit( coin: pair.Key, quantity: pair.Value ); }
		}

		public static void Fund( [NotNull] CoinWallet coinWallet, [CanBeNull] params KeyValuePair<ICoin, UInt64>[] sourceAmounts ) {
			if ( coinWallet == null ) { throw new ArgumentNullException( nameof( coinWallet ) ); }

			Fund( coinWallet, sourceAmounts.AsEnumerable() );
		}

		public static void Fund( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
			if ( coinWallet == null ) { throw new ArgumentNullException( nameof( coinWallet ) ); }

			if ( null == sourceAmounts ) { return; }

			Parallel.ForEach( sourceAmounts, pair => coinWallet.Deposit( pair.Key, pair.Value ) );
		}

		/// <summary>
		///     Adds the optimal amount of <see cref="ICoin" />. Returns any unused portion of the money
		///     (fractions of the smallest <see cref="ICoin" />).
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="amount"></param>
		/// <param name="optimalAmountOfCoin"></param>
		/// <returns></returns>
		public static Decimal Fund( [NotNull] this CoinWallet coinWallet, Decimal amount, Boolean optimalAmountOfCoin = true ) {
			if ( coinWallet == null ) { throw new ArgumentNullException( nameof( coinWallet ) ); }

			var leftOverFund = Decimal.Zero;
			coinWallet.Deposit( optimalAmountOfCoin ? amount.Optimal( ref leftOverFund ) : amount.UnOptimal( ref leftOverFund ) );

			return leftOverFund;
		}

		/// <summary>
		///     Given the <paramref name="amount" />, return the optimal amount of <see cref="ICoin" />
		///     ( <see cref="CoinWallet.Total" />) it would take to <see cref="CoinWallet.Total" /> the <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of Pennies not accounted for.</param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<ICoin, UInt64>> Optimal( this Decimal amount, ref Decimal leftOverAmount ) {
			var left = new List<ICoin>( PossibleCoins );
			var result = left.ToDictionary<ICoin, ICoin, UInt64>( denomination => denomination, denomination => 0 );

			leftOverAmount += amount;

			while ( leftOverAmount > Decimal.Zero && left.Any() ) {
				var coin = left.OrderByDescending( denomination => denomination.FaceValue ).First();

				var chunks = ( UInt64 )( leftOverAmount / coin.FaceValue );

				if ( chunks > Decimal.Zero ) {
					result[ coin ] += chunks;
					leftOverAmount -= chunks * coin.FaceValue;
				}

				left.Remove( coin );
			}

			Assert.Less( leftOverAmount, PossibleCoins.OrderBy( denomination => denomination.FaceValue ).First().FaceValue );

			return result;
		}

		/// <summary>Truncate anything lesser than 1 <see cref="Satoshi" />.</summary>
		/// <param name="btc"></param>
		/// <returns></returns>
		public static Decimal Sanitize( this Decimal btc ) {
			var sanitized = btc.ToSatoshi().ToBTC();

			//Assert.GreaterOrEqual( btc, sanitized );
			return sanitized;
		}

		[NotNull]
		public static String SimplerBTC( [NotNull] this SimpleBitcoinWallet wallet ) {
			if ( wallet == null ) { throw new ArgumentNullException( nameof( wallet ) ); }

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
		/// <returns></returns>
		[NotNull]
		public static String SimplerBTC( this Decimal btc, [NotNull] String coinSuffix = "BTC" ) {
			if ( coinSuffix == null ) { throw new ArgumentNullException( nameof( coinSuffix ) ); }

			btc = btc.Sanitize();

			var list = new List<String> {
				new SimpleBitcoinWallet( btc ).ToString().TrimEnd( '0' ).TrimEnd( '.' ),
				$"{btc.TomBTC():N6}".TrimEnd( '0' ).TrimEnd( '.' ) + " m" + coinSuffix,
				$"{btc.ToμBtc():N4}".TrimEnd( '0' ).TrimEnd( '.' ) + " μ" + coinSuffix,
				$"{btc.ToSatoshi():N0}" + " sat"
			};

			//as btc

			//as mbtc

			//as μbtc

			//as satoshi
			var chosen = list.OrderBy( s => s.Length ).FirstOrDefault() ?? String.Empty;

			return chosen;
		}

		/// <summary>Create a TPL dataflow task for depositing large volumes of money.</summary>
		/// <param name="coinWallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		public static Task StartDeposit( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
			if ( coinWallet == null ) { throw new ArgumentNullException( nameof( coinWallet ) ); }

			sourceAmounts = sourceAmounts ?? Enumerable.Empty<KeyValuePair<ICoin, UInt64>>();
			var actionBlock = new ActionBlock<KeyValuePair<ICoin, UInt64>>( pair => coinWallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeSensible );
			Parallel.ForEach( sourceAmounts, pair => actionBlock.Post( pair ) );
			actionBlock.Complete();

			return actionBlock.Completion;
		}

		/// <summary>
		///     Transfer everything FROM the <paramref name="source" /><see cref="CoinWallet" /> into
		///     this <paramref name="target" /><see cref="CoinWallet" />.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[NotNull]
		public static Task<ConcurrentDictionary<ICoin, UInt64>> StartTransfer( [CanBeNull] this CoinWallet source, [CanBeNull] CoinWallet target ) =>
			Task.Run( () => {
				if ( null == source || null == target ) { return new ConcurrentDictionary<ICoin, UInt64>(); }

				return new ConcurrentDictionary<ICoin, UInt64>( Transfer( source, target ) );
			} );

		public static Decimal ToBTC( this Int16 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

		public static Decimal ToBTC( this Int32 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

		public static Decimal ToBTC( this Int64 satoshi ) => satoshi / ( Decimal )SimpleBitcoinWallet.SatoshiInOneBtc;

		public static Decimal TomBTC( this Decimal btc ) => btc * SimpleBitcoinWallet.mBTCInOneBTC;

		public static Int64 ToSatoshi( this Decimal btc ) => ( Int64 )( btc * SimpleBitcoinWallet.SatoshiInOneBtc );

		/// <summary>Return the <paramref name="wallet" /> in Satoshi.</summary>
		/// <param name="wallet"></param>
		/// <returns></returns>
		public static Int64 ToSatoshi( [NotNull] this SimpleBitcoinWallet wallet ) => wallet.Balance.ToSatoshi();

		public static Decimal ToμBtc( this Decimal btc ) => btc * SimpleBitcoinWallet.ΜBtcInOneBtc;

		[NotNull]
		public static IEnumerable<KeyValuePair<ICoin, UInt64>> Transfer( [NotNull] this CoinWallet source, [NotNull] CoinWallet target ) {
			if ( source == null ) { throw new ArgumentNullException( nameof( source ) ); }

			if ( target == null ) { throw new ArgumentNullException( nameof( target ) ); }

			var transferred = new ConcurrentDictionary<ICoin, UInt64>();

			foreach ( var pair in source ) {
				if ( !source.Transfer( target, pair ) ) { continue; }

				var denomination = pair.Key;
				var count = pair.Value;
				transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
			}

			return transferred;
		}

		public static Boolean Transfer( [NotNull] this CoinWallet source, [NotNull] CoinWallet target, KeyValuePair<ICoin, UInt64> denominationAndAmount ) {
			if ( source == null ) { throw new ArgumentNullException( nameof( source ) ); }

			if ( target == null ) { throw new ArgumentNullException( nameof( target ) ); }

			return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value ) > 0;
		}

		/// <summary>
		///     Create a TPL dataflow task for depositing large volumes of money into this wallet.
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		public static Task Transfer( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
			if ( coinWallet == null ) { throw new ArgumentNullException( nameof( coinWallet ) ); }

			var bsfasd = new ActionBlock<KeyValuePair<ICoin, UInt64>>( pair => coinWallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeSensible );
			bsfasd.Complete();

			return bsfasd.Completion;
		}

		/// <summary>
		///     Given the <paramref name="amount" />, return the unoptimal amount of
		///     <see cref="ICoin" /> ( <see cref="CoinWallet.Total" />) it would take to
		///     <see cref="CoinWallet.Total" /> the <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of coin not accounted for.</param>
		/// <returns></returns>
		[NotNull]
		public static IEnumerable<KeyValuePair<ICoin, UInt64>> UnOptimal( this Decimal amount, ref Decimal leftOverAmount ) {
			var left = new List<ICoin>( PossibleCoins );
			var result = left.ToDictionary<ICoin, ICoin, UInt64>( denomination => denomination, denomination => 0 );

			leftOverAmount += amount;

			while ( leftOverAmount > Decimal.Zero && left.Any() ) {
				var coin = left.OrderBy( denomination => denomination.FaceValue ).First();

				var chunks = ( UInt64 )( leftOverAmount / coin.FaceValue );

				if ( chunks > Decimal.Zero ) {
					result[ coin ] += chunks;
					leftOverAmount -= chunks * coin.FaceValue;
				}

				left.Remove( coin );
			}

			Assert.Less( leftOverAmount, PossibleCoins.OrderBy( denomination => denomination.FaceValue ).First().FaceValue );

			return result;
		}
	}
}