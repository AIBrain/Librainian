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

namespace Librainian.Measurement.Currency.USD {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Annotations;
    using Librainian.Extensions;
    using NUnit.Framework;
    using Threading;

    public static class Extensions {
        [NotNull] public static readonly HashSet< IDenomination > PossibleDenominations = new HashSet< IDenomination >();

        static Extensions() {
            foreach ( var denomination in typeof ( IBankNote ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType< IDenomination >() ) {
                PossibleDenominations.Add( denomination );
            }
            foreach ( var denomination in typeof ( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType< IDenomination >() ) {
                PossibleDenominations.Add( denomination );
            }
        }

        [NotNull]
        public static Wallet CreateWallet() => Wallet.Create();

        /// <summary>
        ///     Transfer everything FROM the <paramref name="source" /> <see cref="Wallet" /> into this <paramref name="target" />
        ///     <see cref="Wallet" />.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [NotNull]
        public static Task< ConcurrentDictionary< IDenomination, ulong > > StartTransfer( [CanBeNull] this Wallet source, [CanBeNull] Wallet target ) {
            return Task.Run( () => {
                                 if ( null == source || null == target ) {
                                     return new ConcurrentDictionary< IDenomination, ulong >();
                                 }

                                 return new ConcurrentDictionary< IDenomination, ulong >( Transfer( source, target ) );
                             } );
        }

        public static IEnumerable< KeyValuePair< IDenomination, ulong > > Transfer( [NotNull] this Wallet source, [NotNull] Wallet target ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }

            var transferred = new ConcurrentDictionary< IDenomination, ulong >();

            foreach ( var pair in source.Groups ) {
                if ( !source.Transfer( target, pair ) ) {
                    continue;
                }
                var denomination = pair.Key;
                var count = pair.Value;
                transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
            }

            return transferred;
        }

        public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair< IDenomination, ulong > denominationAndAmount ) {
            if ( source == null ) {
                throw new ArgumentNullException( "source" );
            }
            if ( target == null ) {
                throw new ArgumentNullException( "target" );
            }
            return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value );
        }

        /// <summary>
        ///     Create a TPL dataflow task for depositing large volumes of money into this wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="sourceAmounts"></param>
        /// <returns></returns>
        public static Task Transfer( [NotNull] Wallet wallet, [CanBeNull] IEnumerable< KeyValuePair< IDenomination, ulong > > sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            var bsfasd = new ActionBlock< KeyValuePair< IDenomination, ulong > >( pair => wallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeParallel );
            bsfasd.Complete();
            return bsfasd.Completion;
        }

        public static void Fund( [NotNull] Wallet wallet, [CanBeNull] params KeyValuePair< IDenomination, ulong >[] sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            Fund( wallet, sourceAmounts.AsEnumerable() );
        }

        public static void Fund( [NotNull] Wallet wallet, [CanBeNull] IEnumerable< KeyValuePair< IDenomination, ulong > > sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            if ( null == sourceAmounts ) {
                return;
            }
            Parallel.ForEach( sourceAmounts, pair => wallet.Deposit( pair.Key, pair.Value ) );
        }

        /// <summary>
        ///     Adds the optimal amount of <see cref="IBankNote" /> and <see cref="ICoin" />.
        ///     Returns any unused portion of the money (fractions of the smallest <see cref="ICoin" />).
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static async Task<Decimal > Fund( [NotNull] Wallet wallet,Decimal amount ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            var leftOverFund =Decimal.Zero;
            var notesAndCoins = amount.Optimal( ref leftOverFund );
            await StartDeposit( wallet, notesAndCoins );
            return leftOverFund;
        }

        /// <summary>
        ///     Given the <paramref name="amount" />, return the optimal amount of <see cref="IBankNote" /> and
        ///     <see cref="ICoin" /> (<see cref="Wallet.Total" />) it would take to <see cref="Wallet" /> the
        ///     <paramref name="amount" />.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="leftOverAmount">Fractions of Pennies not accounted for.</param>
        /// <returns></returns>
        public static Dictionary< IDenomination, ulong > Optimal( this Decimal amount, ref  Decimal leftOverAmount ) {
            var demonsLeft = new List< IDenomination >( PossibleDenominations );
            var result = demonsLeft.ToDictionary< IDenomination, IDenomination, UInt64 >( denomination => denomination, denomination => 0 );

            leftOverAmount += amount;
            while ( leftOverAmount >Decimal.Zero && demonsLeft.Any() ) {
                var highestBill = demonsLeft.OrderByDescending( denomination => denomination.FaceValue ).First();

                var chunks = ( UInt64 ) ( leftOverAmount/highestBill.FaceValue );

                if ( chunks >Decimal.Zero ) {
                    result[ highestBill ] += chunks;
                    leftOverAmount -= chunks*highestBill.FaceValue;
                }
                demonsLeft.Remove( highestBill );
            }

            Assert.Less( leftOverAmount, PossibleDenominations.OrderBy( denomination => denomination.FaceValue ).First().FaceValue );

            return result;
        }

        /// <summary>
        ///     Create a TPL dataflow task for depositing large volumes of money.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="sourceAmounts"></param>
        /// <returns></returns>
        public static Task StartDeposit( [NotNull] Wallet wallet, [CanBeNull] IEnumerable< KeyValuePair< IDenomination, ulong > > sourceAmounts ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            sourceAmounts = sourceAmounts ?? Enumerable.Empty< KeyValuePair< IDenomination, ulong > >();
            var actionBlock = new ActionBlock< KeyValuePair< IDenomination, ulong > >( pair => wallet.Deposit( pair.Key, pair.Value ), Blocks.ManyProducers.ConsumeParallel );
            Parallel.ForEach( sourceAmounts, pair => actionBlock.Post( pair ) );
            actionBlock.Complete();
            return actionBlock.Completion;
        }

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
            var bankNote = message.Denomination as IBankNote;
            if ( null != bankNote ) {
                return wallet.Deposit( bankNote, message.Quantity );
            }

            var coin = message.Denomination as ICoin;
            if ( null != coin ) {
                return wallet.Deposit( coin, message.Quantity );
            }

            throw new NotImplementedException( String.Format( "Unknown denomination {0}", message.Denomination ) );
        }

        /// <summary>
        ///     Deposit <paramref name="bankNotes" /> and <paramref name="coins" /> into this wallet.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="bankNotes"></param>
        /// <param name="coins"></param>
        public static void Deposit( [NotNull] Wallet wallet, [CanBeNull] IEnumerable< KeyValuePair< IBankNote, ulong > > bankNotes = null, IEnumerable< KeyValuePair< ICoin, ulong > > coins = null ) {
            if ( wallet == null ) {
                throw new ArgumentNullException( "wallet" );
            }
            bankNotes = bankNotes ?? Enumerable.Empty< KeyValuePair< IBankNote, ulong > >();
            foreach ( var pair in bankNotes ) {
                wallet.Deposit( denomination: pair.Key, quantity: pair.Value );
            }

            coins = coins ?? Enumerable.Empty< KeyValuePair< ICoin, ulong > >();
            foreach ( var pair in coins ) {
                wallet.Deposit( denomination: pair.Key, quantity: pair.Value );
            }
        }
    }
}
