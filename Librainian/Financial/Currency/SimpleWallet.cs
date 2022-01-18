// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or
// derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to
// avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors. If you find
// your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT responsible for Anything You Do
// With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s). For business inquiries, please
// contact me at Protiguous@Protiguous.com.
//
// Our software can be found at "https://Protiguous.com/Software" Our GitHub address is "https://github.com/Protiguous".

namespace Librainian.Financial.Currency;

using System;
using System.Diagnostics;
using System.Threading;
using Exceptions;
using Measurement.Time;
using Newtonsoft.Json;
using Utilities.Disposables;

/// <summary>A very simple, thread-safe, Decimal-based wallet.</summary>
[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
[JsonObject]
public class SimpleWallet : ABetterClassDispose, ISimpleWallet, IEquatable<SimpleWallet> {

	private readonly ReaderWriterLockSlim _access = new( LockRecursionPolicy.SupportsRecursion );

	[JsonProperty]
	private Decimal _balance;

	public SimpleWallet() => this.Timeout = Minutes.One;

	/// <summary>Initialize the wallet with the specified <paramref name="balance" />.</summary>
	/// <param name="balance"></param>
	public SimpleWallet( Decimal balance ) : this() => this._balance = balance;

	public Decimal Balance {
		get {
			if ( this._access.TryEnterReadLock( this.Timeout ) ) {
				try {
					return this._balance;
				}
				finally {
					this._access.ExitReadLock();
				}
			}

			return Decimal.Zero;
		}
	}

	public Action<Decimal>? OnAfterDeposit { get; set; }

	public Action<Decimal>? OnAfterWithdraw { get; set; }

	public Action<Decimal>? OnAnyUpdate { get; set; }

	public Action<Decimal>? OnBeforeDeposit { get; set; }

	public Action<Decimal>? OnBeforeWithdraw { get; set; }

	//TODO add in support for automatic persisting?
	/// <summary>
	/// <para>Defaults to <see cref="Seconds.Thirty" /> in the ctor.</para>
	/// </summary>
	public TimeSpan Timeout { get; set; }

	/// <summary>
	/// <para>Static comparison.</para>
	/// <para>Returns true if the wallets ARE the same instance.</para>
	/// <para>Returns true if the wallets HAVE the same balance.</para>
	/// </summary>
	/// <param name="left"></param>
	/// <param name="right"></param>
	public static Boolean Equals( SimpleWallet? left, SimpleWallet? right ) {
		if ( ReferenceEquals( left, right ) ) {
			return true;
		}

		if ( left is null || right is null ) {
			return false;
		}

		return left.Balance == right.Balance;
	}

	/// <summary>Returns a value that indicates whether two <see cref="SimpleWallet" /> objects have different values.</summary>
	/// <param name="left">The first value to compare.</param>
	/// <param name="right">The second value to compare.</param>
	/// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
	public static Boolean operator !=( SimpleWallet? left, SimpleWallet? right ) => !Equals( left, right );

	/// <summary>Returns a value that indicates whether the values of two <see cref="SimpleWallet" /> objects are equal.</summary>
	/// <param name="left">The first value to compare.</param>
	/// <param name="right">The second value to compare.</param>
	/// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
	public static Boolean operator ==( SimpleWallet? left, SimpleWallet? right ) => Equals( left, right );

	/// <summary>Dispose any disposable members.</summary>
	public override void DisposeManaged() {
		using ( this._access ) { }
	}

	/// <summary>Indicates whether the current wallet has the same balance as the <paramref name="other" /> wallet.</summary>
	/// <param name="other">Annother to compare with this wallet.</param>
	public Boolean Equals( SimpleWallet other ) => Equals( this, other );

	/// <summary>Determines whether the specified object is equal to the current object.</summary>
	/// <param name="obj">The object to compare with the current object.</param>
	/// <returns><see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
	public override Boolean Equals( Object? obj ) => Equals( this, obj as SimpleWallet );

	/// <summary>Serves as the default hash function.</summary>
	/// <returns>A hash code for the current object.</returns>
	public override Int32 GetHashCode() => this._access.GetHashCode();

	public override String ToString() => $"{this.Balance:F8}";

	/// <summary>Add any (+-)amount directly to the balance.</summary>
	/// <param name="amount"></param>
	public Boolean TryAdd( Decimal amount ) {
		try {
			if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
				try {
					this._balance += amount;

					return true;
				}
				finally {
					this._access.ExitWriteLock();
				}
			}
			else {
				return false;
			}
		}
		finally {
			this.OnAnyUpdate?.Invoke( amount );
		}
	}

	public Boolean TryAdd( SimpleWallet wallet ) {
		if ( wallet is null ) {
			throw new ArgumentEmptyException( nameof( wallet ) );
		}

		return this.TryAdd( wallet.Balance );
	}

	/// <summary>Attempt to deposit amount (larger than zero) to the <see cref="Balance" />.</summary>
	/// <param name="amount"></param>
	public Boolean TryDeposit( Decimal amount ) {
		if ( amount > Decimal.Zero ) {
			this.OnBeforeDeposit?.Invoke( amount );

			if ( this.TryAdd( amount ) ) {
				this.OnAfterDeposit?.Invoke( amount );

				return true;
			}
		}

		return false;
	}

	public Boolean TryTransfer( Decimal amount, ref SimpleWallet intoWallet ) {
		if ( Equals( this, intoWallet ) ) {
			throw new InvalidOperationException( "Cannot transfer amount into self-wallet." );
		}

		if ( amount <= Decimal.Zero ) {
			return false;
		}

		try {
			if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
				try {
					if ( this._balance < amount ) {
						return false;
					}

					this._balance -= amount;

					intoWallet.TryDeposit( amount );

					return true;
				}
				finally {
					this._access.ExitWriteLock();
				}
			}
			else {
				return false;
			}
		}
		finally {
			this.OnAfterWithdraw?.Invoke( amount );
			this.OnAnyUpdate?.Invoke( amount );
		}
	}

	/// <summary>
	/// <para>Directly sets the <see cref="Balance" /> of this wallet.</para>
	/// </summary>
	/// <param name="amount"></param>
	public Boolean TryUpdateBalance( Decimal amount ) {
		try {
			if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
				try {
					this._balance = amount;

					return true;
				}
				finally {
					this._access.ExitWriteLock();
				}
			}
			else {
				return false;
			}
		}
		finally {
			this.OnAnyUpdate?.Invoke( amount );
		}
	}

	public void TryUpdateBalance( SimpleWallet simpleWallet ) => this.TryUpdateBalance( simpleWallet.Balance );

	/// <summary>
	/// <para>Attempt to withdraw an amount (larger than Zero) from the wallet.</para>
	/// <para>If the amount is not available, then nothing is withdrawn.</para>
	/// </summary>
	/// <param name="amount"></param>
	public Boolean TryWithdraw( Decimal amount ) {
		if ( amount <= Decimal.Zero ) {
			return false;
		}

		try {
			if ( this._access.TryEnterWriteLock( this.Timeout ) ) {
				try {
					if ( this._balance < amount ) {
						return false;
					}

					this._balance -= amount;

					return true;
				}
				finally {
					this._access.ExitWriteLock();
				}
			}
			else {
				return false;
			}
		}
		finally {
			this.OnAfterWithdraw?.Invoke( amount );
			this.OnAnyUpdate?.Invoke( amount );
		}
	}

	public Boolean TryWithdraw( SimpleWallet wallet ) {
		if ( wallet is null ) {
			throw new ArgumentEmptyException( nameof( wallet ) );
		}

		return this.TryWithdraw( wallet.Balance );
	}
}