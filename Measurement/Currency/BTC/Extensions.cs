#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian/Extensions.cs" was last cleaned by Rick on 2014/08/11 at 12:39 AM
#endregion

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
		[NotNull]
		public static readonly HashSet<ICoin> PossibleCoins = new HashSet<ICoin>();

		static Extensions() {
			foreach ( var coin in typeof(ICoin).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<ICoin>() ) {
				PossibleCoins.Add( coin );
			}
		}

		public static String SimplerBTC( [NotNull] this SimpleBitcoinWallet wallet ) {
			if ( wallet == null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}
			return wallet.Balance.SimplerBTC();
		}

		/// <summary>
		///     <para>0.00000001 -> 1 satoshi</para>
		///     <para>0.00000011 -> 11 satoshi</para>
		///     <para>0.00000110 -> 11 μBTC</para>
		/// </summary>
		/// <param name="btc"></param>
		/// <param name="coinSuffix">
		///     <para>BTC</para>
		///     <para>NMC</para>
		///     <para>etc...</para>
		/// </param>
		/// <returns></returns>
		public static String SimplerBTC( this Decimal btc, [NotNull] String coinSuffix = "BTC" ) {
			if ( coinSuffix == null ) {
				throw new ArgumentNullException( nameof( coinSuffix ) );
			}
			btc = btc.Sanitize();

			//var wallet = new SimpleBitcoinWallet( btc );
			//Console.WriteLine( wallet.Formatted );

			var list = new List<String> {
											  new SimpleBitcoinWallet( btc ).Formatted.TrimEnd( '0' ).TrimEnd( '.' ),
											  String.Format( "{0:N6}", btc.TomBTC() ).TrimEnd( '0' ).TrimEnd( '.' ) + " m" + coinSuffix,
											  String.Format( "{0:N4}", btc.ToμBTC() ).TrimEnd( '0' ).TrimEnd( '.' ) + " μ" + coinSuffix,
											  String.Format( "{0:N0}", btc.ToSatoshi() ) + " sat"
										  };

			//as btc    

			//as mbtc

			//as μbtc

			//as satoshi
			var chosen = list.OrderBy( s => s.Length ).FirstOrDefault() ?? String.Empty;
			return chosen;
		}

		/// <summary>
		///     Truncate anything lesser than 1 <see cref="Satoshi" />.
		/// </summary>
		/// <param name="btc"></param>
		/// <returns></returns>
		public static Decimal Sanitize( this Decimal btc ) {
			var sanitized = btc.ToSatoshi().ToBTC();
			//Assert.GreaterOrEqual( btc, sanitized );
			return sanitized;
		}

		public static long ToSatoshi( this Decimal btc ) => ( long )( btc * SimpleBitcoinWallet.SatoshiInOneBTC );

		public static Decimal ToBTC( this Int16 satoshi ) => satoshi / ( decimal )SimpleBitcoinWallet.SatoshiInOneBTC;

		public static Decimal ToBTC( this Int32 satoshi ) => satoshi / ( decimal )SimpleBitcoinWallet.SatoshiInOneBTC;

		public static Decimal ToBTC( this Int64 satoshi ) => satoshi / ( decimal )SimpleBitcoinWallet.SatoshiInOneBTC;

		public static Decimal TomBTC( this Decimal btc ) => btc * SimpleBitcoinWallet.mBTCInOneBTC;

		public static Decimal ToμBTC( this Decimal btc ) => btc * SimpleBitcoinWallet.μBTCInOneBTC;

		/// <summary>
		///     Return the <paramref name="wallet" /> in Satoshi.
		/// </summary>
		/// <param name="wallet"></param>
		/// <returns></returns>
		public static long ToSatoshi( this SimpleBitcoinWallet wallet ) => wallet.Balance.ToSatoshi();

		/// <summary>
		///     Transfer everything FROM the <paramref name="source" /> <see cref="CoinWallet" /> into this
		///     <paramref name="target" />
		///     <see cref="CoinWallet" />.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[NotNull]
		public static Task<ConcurrentDictionary<ICoin, ulong>> StartTransfer( [CanBeNull] this CoinWallet source, [CanBeNull] CoinWallet target ) => Task.Run( () => {
			if ( null == source || null == target ) {
				return new ConcurrentDictionary<ICoin, ulong>();
			}

			return new ConcurrentDictionary<ICoin, ulong>( Transfer( source, target ) );
		} );

		public static IEnumerable<KeyValuePair<ICoin, ulong>> Transfer( [NotNull] this CoinWallet source, [NotNull] CoinWallet target ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			if ( target == null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			var transferred = new ConcurrentDictionary<ICoin, ulong>();

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

		public static Boolean Transfer( [NotNull] this CoinWallet source, [NotNull] CoinWallet target, KeyValuePair<ICoin, ulong> denominationAndAmount ) {
			if ( source == null ) {
				throw new ArgumentNullException( nameof( source ) );
			}
			if ( target == null ) {
				throw new ArgumentNullException( nameof( target ) );
			}
			return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value ) > 0;
		}

		/// <summary>
		///     Create a TPL dataflow task for depositing large volumes of money into this wallet.
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		public static Task Transfer( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, ulong>> sourceAmounts ) {
			if ( coinWallet == null ) {
				throw new ArgumentNullException( nameof( coinWallet ) );
			}
			var bsfasd = new ActionBlock<KeyValuePair<ICoin, ulong>>( pair => coinWallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeParallel );
			bsfasd.Complete();
			return bsfasd.Completion;
		}

		public static void Fund( [NotNull] CoinWallet coinWallet, [CanBeNull] params KeyValuePair<ICoin, ulong>[] sourceAmounts ) {
			if ( coinWallet == null ) {
				throw new ArgumentNullException( nameof( coinWallet ) );
			}
			Fund( coinWallet, sourceAmounts.AsEnumerable() );
		}

		public static void Fund( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, ulong>> sourceAmounts ) {
			if ( coinWallet == null ) {
				throw new ArgumentNullException( nameof( coinWallet ) );
			}
			if ( null == sourceAmounts ) {
				return;
			}
			Parallel.ForEach( sourceAmounts, pair => coinWallet.Deposit( pair.Key, pair.Value ) );
		}

		/// <summary>
		///     Create a TPL dataflow task for depositing large volumes of money.
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		public static Task StartDeposit( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, ulong>> sourceAmounts ) {
			if ( coinWallet == null ) {
				throw new ArgumentNullException( nameof( coinWallet ) );
			}
			sourceAmounts = sourceAmounts ?? Enumerable.Empty<KeyValuePair<ICoin, ulong>>();
			var actionBlock = new ActionBlock<KeyValuePair<ICoin, ulong>>( pair => coinWallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeParallel );
			Parallel.ForEach( sourceAmounts, pair => actionBlock.Post( pair ) );
			actionBlock.Complete();
			return actionBlock.Completion;
		}

		/// <summary>
		///     Adds the optimal amount of <see cref="ICoin" />.
		///     Returns any unused portion of the money (fractions of the smallest <see cref="ICoin" />).
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="amount"></param>
		/// <param name="optimalAmountOfCoin"></param>
		/// <returns></returns>
		public static Decimal Fund( [NotNull] this CoinWallet coinWallet, Decimal amount, Boolean optimalAmountOfCoin = true ) {
			if ( coinWallet == null ) {
				throw new ArgumentNullException( nameof( coinWallet ) );
			}
			var leftOverFund = Decimal.Zero;
			coinWallet.Deposit( optimalAmountOfCoin ? amount.Optimal( ref leftOverFund ) : amount.UnOptimal( ref leftOverFund ) );

			return leftOverFund;
		}

		/// <summary>
		///     Deposit <paramref name="coins" /> into this wallet.
		/// </summary>
		/// <param name="coinWallet"></param>
		/// <param name="coins"></param>
		public static void Deposit( [NotNull] this CoinWallet coinWallet, IEnumerable<KeyValuePair<ICoin, ulong>> coins = null ) {
			if ( coinWallet == null ) {
				throw new ArgumentNullException( nameof( coinWallet ) );
			}

			coins = coins ?? Enumerable.Empty<KeyValuePair<ICoin, ulong>>();
			foreach ( var pair in coins ) {
				coinWallet.Deposit( coin: pair.Key, quantity: pair.Value );
			}
		}

		/// <summary>
		///     Given the <paramref name="amount" />, return the optimal amount of <see cref="ICoin" /> (
		///     <see cref="CoinWallet.Total" />) it would take to <see cref="CoinWallet.Total" /> the
		///     <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of Pennies not accounted for.</param>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<ICoin, ulong>> Optimal( this Decimal amount, ref Decimal leftOverAmount ) {
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

		/// <summary>
		///     Given the <paramref name="amount" />, return the unoptimal amount of <see cref="ICoin" /> (
		///     <see cref="CoinWallet.Total" />) it would take to <see cref="CoinWallet.Total" /> the
		///     <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of coin not accounted for.</param>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<ICoin, ulong>> UnOptimal( this Decimal amount, ref Decimal leftOverAmount ) {
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

		/*
                /// <summary>
                /// </summary>
                /// <param name="wallet"></param>
                /// <param name="message"></param>
                /// <remarks>Performs locks on the internal tables.</remarks>
                /// <returns>Returns the new quantity.</returns>
                public static Boolean Deposit( [NotNull] Wallet wallet, TransactionMessage message ) {
                    if ( wallet == null ) {
                        throw new ArgumentNullException( "wallet" );
                    }

                    return wallet.Deposit( message.Coin, message.Quantity );
                }
        */
	}
}
