// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Extensions.cs" was last cleaned by Rick on 2016/06/18 at 10:53 PM

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
            if ( coinWallet is null ) {
                throw new ArgumentNullException( nameof( coinWallet ) );
            }

            coins = coins ?? Enumerable.Empty<KeyValuePair<ICoin, UInt64>>();
            foreach ( var pair in coins ) {
                coinWallet.Deposit( coin: pair.Key, quantity: pair.Value );
            }
        }

        public static void Fund( [NotNull] CoinWallet coinWallet, [CanBeNull] params KeyValuePair<ICoin, UInt64>[] sourceAmounts ) {
            if ( coinWallet is null ) {
                throw new ArgumentNullException( nameof( coinWallet ) );
            }
            Fund( coinWallet, sourceAmounts.AsEnumerable() );
        }

        public static void Fund( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
            if ( coinWallet is null ) {
                throw new ArgumentNullException( nameof( coinWallet ) );
            }
            if ( null == sourceAmounts ) {
                return;
            }
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
            if ( coinWallet is null ) {
                throw new ArgumentNullException( nameof( coinWallet ) );
            }
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

        public static String SimplerBTC( [NotNull] this SimpleBitcoinWallet wallet ) {
            if ( wallet is null ) {
                throw new ArgumentNullException( nameof( wallet ) );
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
        /// <returns></returns>
        public static String SimplerBTC( this Decimal btc, [NotNull] String coinSuffix = "BTC" ) {
            if ( coinSuffix is null ) {
                throw new ArgumentNullException( nameof( coinSuffix ) );
            }
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
            if ( coinWallet is null ) {
                throw new ArgumentNullException( nameof( coinWallet ) );
            }
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
        public static Task<ConcurrentDictionary<ICoin, UInt64>> StartTransfer( [CanBeNull] this CoinWallet source, [CanBeNull] CoinWallet target ) => Task.Run( () => {
            if ( null == source || null == target ) {
                return new ConcurrentDictionary<ICoin, UInt64>();
            }

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
        public static Int64 ToSatoshi( this SimpleBitcoinWallet wallet ) => wallet.Balance.ToSatoshi();

        public static Decimal ToμBtc( this Decimal btc ) => btc * SimpleBitcoinWallet.ΜBtcInOneBtc;

        public static IEnumerable<KeyValuePair<ICoin, UInt64>> Transfer( [NotNull] this CoinWallet source, [NotNull] CoinWallet target ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target is null ) {
                throw new ArgumentNullException( nameof( target ) );
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

        public static Boolean Transfer( [NotNull] this CoinWallet source, [NotNull] CoinWallet target, KeyValuePair<ICoin, UInt64> denominationAndAmount ) {
            if ( source is null ) {
                throw new ArgumentNullException( nameof( source ) );
            }
            if ( target is null ) {
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
        public static Task Transfer( [NotNull] CoinWallet coinWallet, [CanBeNull] IEnumerable<KeyValuePair<ICoin, UInt64>> sourceAmounts ) {
            if ( coinWallet is null ) {
                throw new ArgumentNullException( nameof( coinWallet ) );
            }
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

            Assert.Less( leftOverAmount, PossibleCoins.OrderBy( denomination => denomination.FaceValue ).First().FaceValue );

            return result;
        }
    }
}