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

namespace Librainian.Financial.Containers.Wallets {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Threading.Tasks.Dataflow;
	using Currency;
	using Currency.BankNotes;
	using Currency.Coins;
	using Exceptions;
	using Extensions;
	using Maths;
	using Threading;

	public static class WalletExtensions {

		public static HashSet<IDenomination> PossibleDenominations { get; } = new();

		static WalletExtensions() {
			foreach ( var denomination in typeof( IBankNote ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<IDenomination>() ) {
				PossibleDenominations.Add( denomination );
			}

			foreach ( var denomination in typeof( ICoin ).GetTypesDerivedFrom().Select( Activator.CreateInstance ).OfType<IDenomination>() ) {
				PossibleDenominations.Add( denomination );
			}
		}

		private static Task StartDeposit( Wallet wallet, Dictionary<IDenomination, UInt64> dictionary ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			var actionBlock = new ActionBlock<KeyValuePair<IDenomination, UInt64>>( pair => wallet.Deposit( pair.Key, pair.Value ),
				Blocks.ManyProducers.ConsumeSensible( default( CancellationToken? ) ) );

			Parallel.ForEach( dictionary, pair => actionBlock.Post( pair ) );
			actionBlock.Complete();

			return actionBlock.Completion;
		}

		/// <summary></summary>
		/// <param name="wallet"></param>
		/// <param name="message"></param>
		public static Boolean Deposit( this Wallet wallet, TransactionMessage message ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			return message.Denomination switch {
				IBankNote bankNote => wallet.Deposit( bankNote, message.Quantity ),
				ICoin coin => wallet.Deposit( coin, message.Quantity ) > Decimal.Zero,
				var _ => throw new InvalidOperationException( $"Unknown denomination {message.Denomination}" )
			};
		}

		/// <summary>Deposit <paramref name="bankNotes" /> and <paramref name="coins" /> into this wallet.</summary>
		/// <param name="wallet"></param>
		/// <param name="bankNotes"></param>
		/// <param name="coins"></param>
		public static void Deposit( this Wallet wallet, IEnumerable<(IBankNote, UInt64)>? bankNotes = null,
			IEnumerable<(ICoin, UInt64)>? coins = null ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			if ( bankNotes != null ) {
				foreach ( (var bankNote, var quantity) in bankNotes ) {
					wallet.Deposit( bankNote, quantity );
				}
			}

			if ( coins != null ) {
				foreach ( (var coin, var quantity) in coins ) {
					wallet.Deposit( coin, quantity );
				}
			}
		}

		public static void Fund( this Wallet wallet, params (IDenomination, UInt64)[]? sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			if ( sourceAmounts is null ) {
				return;
			}

			Parallel.ForEach( sourceAmounts, pair => wallet.Deposit( pair.Item1, pair.Item2 ) );
		}

		public static void Fund( this Wallet wallet, IEnumerable<(IDenomination, UInt64)>? sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			if ( sourceAmounts is null ) {
				return;
			}

			Parallel.ForEach( sourceAmounts, pair => wallet.Deposit( pair.Item1, pair.Item2 ) );
		}

		/// <summary>
		///     Adds the optimal amount of <see cref="IBankNote" /> and <see cref="ICoin" />. Returns any unused portion of
		///     the money (fractions of the smallest <see cref="ICoin" />).
		/// </summary>
		/// <param name="wallet"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public static async Task<Decimal> Fund( this Wallet wallet, Decimal amount ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			var optimal = amount.ToOptimal( out var leftOverFund );

			await StartDeposit( wallet, optimal ).ConfigureAwait( false );

			return leftOverFund;
		}

		/// <summary>Create a TPL dataflow task for depositing large volumes of money.</summary>
		/// <param name="wallet"></param>
		/// <param name="sourceAmounts"></param>
		/// <returns></returns>
		public static Task StartDeposit( Wallet wallet, IEnumerable<(IDenomination, UInt64)>? sourceAmounts ) {
			if ( wallet is null ) {
				throw new ArgumentEmptyException( nameof( wallet ) );
			}

			var actionBlock = new ActionBlock<(IDenomination, UInt64)>( pair => wallet.Deposit( pair.Item1, pair.Item2 ),
				Blocks.ManyProducers.ConsumeSensible( default( CancellationToken? ) ) );

			Parallel.ForEach( sourceAmounts, pair => actionBlock.Post( pair ) );
			actionBlock.Complete();

			return actionBlock.Completion;
		}

		/// <summary>
		///     Transfer everything FROM the <paramref name="source" /><see cref="Wallet" /> into <paramref name="target" />
		///     <see cref="Wallet" />.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static Task StartTransfer( this Wallet source, Wallet target, CancellationToken cancellationToken ) =>
			Task.Run( () => {
				foreach ( (var denomination, var quantity) in source ) {
					if ( cancellationToken.IsCancellationRequested ) {
						break;
					}

					if ( source.TryWithdraw( denomination, quantity ) ) {
						target.Deposit( denomination, quantity );
					}
				}
			}, cancellationToken );

		/// <summary>
		///     Given the <paramref name="amount" />, return the optimal amount of <see cref="IBankNote" /> and
		///     <see cref="ICoin" /> ( <see cref="Wallet.Total" />) it would take to
		///     <see cref="Wallet" /> the <paramref name="amount" />.
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="leftOverAmount">Fractions of Dollars/Pennies not accounted for. OfficeSpace, Superman III"...</param>
		/// <returns></returns>
		public static Dictionary<IDenomination, UInt64> ToOptimal( this Decimal amount, out Decimal leftOverAmount ) {
			var denominations = new List<IDenomination>( PossibleDenominations );
			var optimal = denominations.ToDictionary<IDenomination, IDenomination, UInt64>( denomination => denomination, denomination => 0 );

			leftOverAmount = amount;

			while ( leftOverAmount > Decimal.Zero && denominations.Any() ) {
				var highestBill = denominations.OrderByDescending( denomination => denomination.FaceValue ).First();
				denominations.Remove( highestBill );

				var count = ( UInt64 )( leftOverAmount / highestBill.FaceValue );

				if ( count.Any() ) {
					optimal[ highestBill ] += count;
					leftOverAmount -= count * highestBill.FaceValue;
				}
			}

			var empties = optimal.Where( pair => !pair.Value.Any() ).Select( pair => pair.Key );

			foreach ( var empty in empties ) {
				optimal.Remove( empty );
			}

			return optimal;
		}

		public static IEnumerable<(IDenomination, UInt64)> Transfer( this Wallet source, Wallet target ) {
			if ( source is null ) {
				throw new ArgumentEmptyException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentEmptyException( nameof( target ) );
			}

			var transferred = new ConcurrentDictionary<IDenomination, UInt64>();

			foreach ( var pair in source.GetNotesGrouped() ) {
				if ( source.Transfer( target, pair ) ) {
					var denomination = pair.Key;
					var count = pair.Value;
					transferred.AddOrUpdate( denomination, count, ( _, running ) => running + count );
				}
			}

			foreach ( var pair in source.GetCoinsGrouped() ) {
				if ( source.Transfer( target, pair ) ) {
					var denomination = pair.Key;
					var count = pair.Value;
					transferred.AddOrUpdate( denomination, count, ( _, running ) => running + count );
				}
			}

			return transferred.Select( pair => (pair.Key, pair.Value) );
		}

		public static Boolean Transfer( this Wallet source, Wallet target, (IDenomination, UInt64) denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentEmptyException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentEmptyException( nameof( target ) );
			}

			(var denomination, var quantity) = denominationAndAmount;

			return source.TryWithdraw( denomination, quantity ) && target.Deposit( denomination, quantity );
		}

		public static Boolean Transfer( this Wallet source, Wallet target, KeyValuePair<IBankNote, UInt64> denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentEmptyException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentEmptyException( nameof( target ) );
			}

			(var denomination, var quantity) = denominationAndAmount;

			return source.TryWithdraw( denomination, quantity ) && target.Deposit( denomination, quantity );
		}

		public static Boolean Transfer( this Wallet source, Wallet target, KeyValuePair<ICoin, UInt64> denominationAndAmount ) {
			if ( source is null ) {
				throw new ArgumentEmptyException( nameof( source ) );
			}

			if ( target is null ) {
				throw new ArgumentEmptyException( nameof( target ) );
			}

			(var denomination, var quantity) = denominationAndAmount;

			return source.TryWithdraw( denomination, quantity ) && target.Deposit( denomination, quantity ) > 0m;
		}
	}
}