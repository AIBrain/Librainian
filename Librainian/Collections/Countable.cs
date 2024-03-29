// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
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
// File "Countable.cs" last touched on 2021-03-07 at 2:00 PM by Protiguous.

#nullable enable

namespace Librainian.Collections {

	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Numerics;
	using System.Threading;
	using System.Threading.Tasks;
	using JetBrains.Annotations;
	using Measurement.Time;
	using Newtonsoft.Json;
	using Threading;
	using Threadsafe;
	using Utilities;

	/// <summary>
	///     <para>Threadsafe dictionary for LARGE numbers ( <see cref="BigInteger" />).</para>
	///     <para>Some methods have a <see cref="ReadTimeout" /> or <see cref="WriteTimeout" />.</para>
	///     <para>Call <see cref="Complete" /> as soon as possible.</para>
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	[JsonObject]
	public class Countable<TKey> : ABetterClassDispose, IEnumerable<(TKey, BigInteger)>, ICountable<TKey> where TKey : notnull {

		/// <summary>Quick hashes of <see cref="TKey" /> for <see cref="ReaderWriterLockSlim" />.</summary>
		[NotNull]
		private ConcurrentDictionary<Byte, ReaderWriterLockSlim> Buckets { get; } = new( Environment.ProcessorCount, 1 );

		/// <summary>Count of each <see cref="TKey" />.</summary>
		[JsonProperty]
		[NotNull]
		private ConcurrentDictionary<TKey, BigInteger> Dictionary { get; } = new();

		[JsonProperty]
		public VolatileBoolean IsReadOnly { get; set; }

		[JsonProperty]
		public TimeSpan ReadTimeout { get; set; }

		[JsonProperty]
		public TimeSpan WriteTimeout { get; set; }

		[CanBeNull]
		[JsonProperty]
		public BigInteger? this[ [NotNull] TKey key ] {
			get {
				if ( key is null ) {
					throw new ArgumentNullException( nameof( key ) );
				}

				var bucket = this.Bucket( key );

				if ( bucket.TryEnterReadLock( this.ReadTimeout ) ) {
					try {
						return this.Dictionary.TryGetValue( key, out var result ) ? result : default( BigInteger );
					}
					finally {
						bucket.ExitReadLock();
					}
				}

				return default( BigInteger? );
			}

			set {
				if ( key is null ) {
					throw new ArgumentNullException( nameof( key ) );
				}

				if ( this.IsReadOnly ) {
					return;
				}

				if ( !value.HasValue ) {
					return;
				}

				var bucket = this.Bucket( key );

				if ( bucket.TryEnterWriteLock( this.WriteTimeout ) ) {
					try {
						this.Dictionary.TryAdd( key, value.Value );
					}
					finally {
						bucket.ExitWriteLock();
					}
				}
			}
		}

		public Countable() : this( Minutes.One, Minutes.One ) { }

		public Countable( TimeSpan readTimeout, TimeSpan writeTimeout ) {
			this.ReadTimeout = readTimeout;
			this.WriteTimeout = writeTimeout;
		}

		private static Byte Hash( [NotNull] TKey key ) => ( Byte )key.GetHashCode();

		[NotNull]
		private ReaderWriterLockSlim Bucket( [NotNull] TKey key ) {
			var hash = Hash( key );

			TryAgain:

			if ( this.Buckets.TryGetValue( hash, out var bucket ) ) {
				return bucket;
			}

			bucket = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );

			if ( this.Buckets.TryAdd( hash, bucket ) ) {
				return bucket;
			}

			goto TryAgain;
		}

		[NotNull]
		private IEnumerable<Byte> GetUsedBuckets() => this.Dictionary.Keys.Select( Hash );

		public Boolean Add( [NotNull] IEnumerable<TKey> keys ) {
			if ( keys is null ) {
				throw new ArgumentNullException( nameof( keys ) );
			}

			if ( this.IsReadOnly ) {
				return false;
			}

			var result = Parallel.ForEach( keys.AsParallel(), CPU.AllExceptOne, key => this.Add( key ) );

			return result.IsCompleted;
		}

		public Boolean Add( [NotNull] TKey key ) {
			if ( key is null ) {
				throw new ArgumentNullException( nameof( key ) );
			}

			return this.Add( key, BigInteger.One );
		}

		public Boolean Add( [NotNull] TKey key, BigInteger amount ) {
			if ( key is null ) {
				throw new ArgumentNullException( nameof( key ) );
			}

			if ( this.IsReadOnly ) {
				return false;
			}

			var bucket = this.Bucket( key );

			if ( bucket.TryEnterWriteLock( this.WriteTimeout ) ) {
				try {
					if ( !this.Dictionary.ContainsKey( key ) ) {
						this.Dictionary.TryAdd( key, BigInteger.Zero );
					}

					this.Dictionary[ key ] += amount;

					return true;
				}
				finally {
					bucket.ExitWriteLock();
				}
			}

			return false;
		}

		/// <summary>Mark that this container will now become ReadOnly/immutable. No more adds or subtracts.</summary>
		/// <returns></returns>
		public Boolean Complete() {
			this.IsReadOnly = true;
			this.Trim();

			return this.IsReadOnly;
		}

		public override void DisposeManaged() {
			this.Complete();

			foreach ( var pair in this.Buckets ) {
				using ( pair.Value ) { }
			}
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator() => this.Dictionary.GetEnumerator();

		public Boolean Subtract( [NotNull] TKey key, BigInteger amount ) {
			if ( key is null ) {
				throw new ArgumentNullException( nameof( key ) );
			}

			if ( this.IsReadOnly ) {
				return false;
			}

			var bucket = this.Bucket( key );

			if ( bucket.TryEnterWriteLock( this.WriteTimeout ) ) {
				try {
					if ( !this.Dictionary.ContainsKey( key ) ) {
						this.Dictionary.TryAdd( key, BigInteger.Zero );
					}

					this.Dictionary[ key ] -= amount;

					return true;
				}
				finally {
					bucket.ExitWriteLock();
				}
			}

			return false;
		}

		/// <summary>Return the sum of all values.</summary>
		/// <returns></returns>
		public BigInteger Sum() => this.Dictionary.Aggregate( BigInteger.Zero, ( current, pair ) => current + pair.Value );

		public void Trim() =>
					Parallel.ForEach( this.Dictionary.Where( pair => pair.Value == default( BigInteger ) || pair.Value == BigInteger.Zero ), CPU.AllExceptOne,
						pair => this.Dictionary.TryRemove( pair.Key, out var dummy ) );

		/// <summary>Mark that this container will now become UnReadOnly/mutable. Allow more adds and subtracts.</summary>
		/// <returns></returns>
		public Boolean EnableMutable() => !( this.IsReadOnly = false );

		/// <summary>Returns an enumerator that iterates through the collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator<(TKey, BigInteger)> IEnumerable<(TKey, BigInteger)>.GetEnumerator() => ( IEnumerator<(TKey, BigInteger)> )this.GetEnumerator();
	}
}