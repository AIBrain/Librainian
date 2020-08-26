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
// File "WalletExtensions.cs" last formatted on 2020-08-14 at 8:33 PM.

#pragma warning disable RCS1138 // Add summary to documentation comment.

namespace Librainian.Financial.Containers.Wallets {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using Currency;
	using Currency.BankNotes;
	using Currency.Coins;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Threading;

	public static class WalletExtensions {

		static WalletExtensions() {
			foreach ( var denomination in typeof( IBankNote ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<IDenomination>() ) {
				PossibleDenominations.Add( denomination );
			}

			foreach ( var denomination in typeof( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<IDenomination>() ) {
				PossibleDenominations.Add( denomination );
			}
		}

		[NotNull]
		public static HashSet<IDenomination> PossibleDenominations { get; } = new HashSet<IDenomination>();

		/// <summary></summary>
		/// <param name="wallet"></param>
		/// <param name="message"></param>
		/// <remarks>Performs locks on the internal tables.</remarks>
		/// <returns>Returns the new quantity.</returns>
		public static Boolean Deposit( [NotNull] this Wallet wallet, TransactionMessage message ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			switch ( message.Denomination ) {
				case IBankNote bankNote: return wallet.Deposit( bankNote, message.Quantity );

				case ICoin coin: return wallet.Deposit( coin, message.Quantity ) > Decimal.Zero;
			}

			throw new NotImplementedException( $"Unknown denomination {message.Denomination}" );
		}

		/// <summary>Deposit <paramref name="bankNotes" /> and <paramref name="coins" /> into this wallet.</summary>
		/// <param name="wallet"></param>
		/// <param name="bankNotes"></param>
		/// <param name="coins"></param>
		public static void Deposit(
			[NotNull] this Wallet wallet,
			[CanBeNull] IEnumerable<KeyValuePair<IBankNote, UInt64>> bankNotes = null,
			[CanBeNull] IEnumerable<KeyValuePair<ICoin, UInt64>> coins = null
		) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			foreach ( var pair in bankNotes ?? Enumerable.Empty<KeyValuePair<IBankNote, UInt64>>() ) {
				wallet.Deposit( pair.Key, pair.Value );
			}

			foreach ( var pair in coins ?? Enumerable.Empty<KeyValuePair<ICoin, UInt64>>() ) {
				wallet.Deposit( pair.Key, pair.Value );
			}
		}

		public static void Fund( [NotNull] this Wallet wallet, [CanBeNull] params KeyValuePair<IDenomination, UInt64>[] sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			wallet.Fund( sourceAmounts.AsEnumerable() );
		}

		public static void Fund( [NotNull] this Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IDenomination, UInt64>> sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			if ( null == sourceAmounts ) {
				return;
			}

			Parallel.ForEach( sourceAmounts, pair => wallet.Deposit( pair.Key, pair.Value ) );
		}

		/// <summary>
		///     Adds the optimal amount of <see cref="IBankNote" /> and <see cref="ICoin" />. Returns any unused portion of
		///     the money (fractions of the smallest <see cref="ICoin" />).
		/// </summary>
		/// <param name="wallet"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public static async Task<Decimal> Fund( [NotNull] this Wallet wallet, Decimal amount ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			var notesAndCoins = amount.ToOptimal( out var leftOverFund );
			await StartDeposit( wallet, notesAndCoins ).ConfigureAwait( false );

			return leftOverFund;
		}

		/// <summary>Create a TPL dataflow task for depositing large volumes of money.</summary>
		/// <param name="wallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		[CanBeNull]
		public static Task StartDeposit( [NotNull] Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IDenomination, UInt64>> sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			var actionBlock = new ActionBlock<KeyValuePair<IDenomination, UInt64>>( pair => wallet.Deposit( pair.Key, pair.Value ),
																					Blocks.ManyProducers.ConsumeSensible( default ) );

			Parallel.ForEach( Enumerable.Empty<KeyValuePair<IDenomination, UInt64>>(), pair => actionBlock.Post( pair ) );
			actionBlock.Complete();

			return actionBlock.Completion;
		}

		/// <summary>
		///     Transfer everything FROM the <paramref name="source" /><see cref="Wallet" /> into this
		///     <paramref name="target" /> <see cref="Wallet" />.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		[NotNull]
		public static Task<ConcurrentDictionary<IDenomination, UInt64>> StartTransfer( [CanBeNull] this Wallet source, [CanBeNull] Wallet target ) =>
			Task.Run( () => {
				if ( null == source || null == target ) {
					return new ConcurrentDictionary<IDenomination, UInt64>();
				}

				return new ConcurrentDictionary<IDenomination, UInt64>( Transfer( source, target ) );
			} );

		/// <summary>
		///     Given the <paramref name="amount" />, return the optimal amount of <see cref="IBankNote" /> and
		///     <see cref="ICoin" /> ( <see cref="Wallet.Total" />) it would take to
		///     <see cref="Wallet" /> the <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of Dollars/Pennies not accounted for. OfficeSpace, Superman III"...</param>
		/// <returns></returns>
		[NotNull]
		public static Dictionary<IDenomination, UInt64> ToOptimal( this Decimal amount, out Decimal leftOverAmount ) {
			var denominations = new List<IDenomination>( PossibleDenominations );
			var optimal = denominations.ToDictionary<IDenomination, IDenomination, UInt64>( denomination => denomination, denomination => 0 );

			leftOverAmount = amount;

			while ( leftOverAmount > Decimal.Zero && denominations.Any() ) {
				var highestBill = denominations.OrderByDescending( denomination => denomination.FaceValue ).First();
				denominations.Remove( highestBill );

				var count = ( UInt64 )( leftOverAmount / highestBill.FaceValue );

				if ( count.Any() ) {
					optimal[highestBill] += count;
					leftOverAmount -= count * highestBill.FaceValue;
				}
			}

			var empties = optimal.Where( pair => !pair.Value.Any() ).Select( pair => pair.Key );

			foreach ( var empty in empties ) {
				optimal.Remove( empty );
			}

			return optimal;
		}

		[NotNull]
		public static IEnumerable<KeyValuePair<IDenomination, UInt64>> Transfer( [NotNull] this Wallet source, [NotNull] Wallet target ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			var transferred = new ConcurrentDictionary<IDenomination, UInt64>();

			foreach ( var pair in source.GetNotesGrouped() ) {
				if ( !source.Transfer( target, pair ) ) {
					continue;
				}

				var denomination = pair.Key;
				var count = pair.Value;
				transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
			}

			foreach ( var pair in source.GetCoinsGrouped() ) {
				if ( !source.Transfer( target, pair ) ) {
					continue;
				}

				var denomination = pair.Key;
				var count = pair.Value;
				transferred.AddOrUpdate( denomination, count, ( denomination1, running ) => running + count );
			}

			return transferred;
		}

		public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair<IDenomination, UInt64> denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value );
		}

		public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair<IBankNote, UInt64> denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) && target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value );
		}

		public static Boolean Transfer( [NotNull] this Wallet source, [NotNull] Wallet target, KeyValuePair<ICoin, UInt64> denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentNullException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentNullException( nameof( target ) );
			}

			return source.TryWithdraw( denominationAndAmount.Key, denominationAndAmount.Value ) &&
				   target.Deposit( denominationAndAmount.Key, denominationAndAmount.Value ) > 0m;
		}

		/// <summary>Create a TPL dataflow task for depositing large volumes of money into this wallet.</summary>
		/// <param name="wallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		[CanBeNull]
		public static Task Transfer( [NotNull] Wallet wallet, [CanBeNull] IEnumerable<KeyValuePair<IDenomination, UInt64>> sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentNullException( nameof( wallet ) );
			}

			var block = new ActionBlock<KeyValuePair<IDenomination, UInt64>>( pair => wallet.Deposit( pair.Key, pair.Value ),
																			  Blocks.ManyProducers.ConsumeSensible( default ) );

			block.Complete();

			return block.Completion;
		}

	}

}