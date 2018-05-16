// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Countable.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Countable.cs" was last cleaned by Protiguous on 2018/05/15 at 10:45 PM.

namespace Librainian.Maths.Numbers {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Magic;
    using Measurement.Time;
    using Newtonsoft.Json;
    using Threading;

    /// <summary>
    ///     <para>Threadsafe dictionary for LARGE numbers ( <see cref="BigInteger" />).</para>
    ///     <para>Some methods have a <see cref="ReadTimeout" /> or <see cref="WriteTimeout" />.</para>
    ///     <para>Call <see cref="Complete" /> as soon as possible.</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    [JsonObject]
    public class Countable<TKey> : ABetterClassDispose, IEnumerable<Tuple<TKey, BigInteger>> {

        private volatile Boolean _isReadOnly;

        public Countable() : this( readTimeout: Minutes.One, writeTimeout: Minutes.One ) { }

        public Countable( TimeSpan readTimeout, TimeSpan writeTimeout ) {
            this.ReadTimeout = readTimeout;
            this.WriteTimeout = writeTimeout;
        }

        /// <summary>
        ///     Quick hashes of <see cref="TKey" /> for <see cref="ReaderWriterLockSlim" />.
        /// </summary>
        private ConcurrentDictionary<Byte, ReaderWriterLockSlim> Buckets { get; } = new ConcurrentDictionary<Byte, ReaderWriterLockSlim>( concurrencyLevel: Environment.ProcessorCount, capacity: 1 );

        /// <summary>
        ///     Count of each <see cref="TKey" />.
        /// </summary>
        [JsonProperty]
        [NotNull]
        private ConcurrentDictionary<TKey, BigInteger> Dictionary { get; } = new ConcurrentDictionary<TKey, BigInteger>();

        [JsonProperty]
        public Boolean IsReadOnly {
            get => this._isReadOnly;

            private set => this._isReadOnly = value;
        }

        [JsonProperty]
        public TimeSpan ReadTimeout { get; set; }

        [JsonProperty]
        public TimeSpan WriteTimeout { get; set; }

        [CanBeNull]
        [JsonProperty]
        public BigInteger? this[TKey key] {
            get {
                if ( key == null ) { throw new ArgumentNullException( nameof( key ) ); }

                var bucket = this.Bucket( key );

                try {
                    if ( bucket.TryEnterReadLock( timeout: this.ReadTimeout ) ) { return this.Dictionary.TryGetValue( key, out var result ) ? result : default; }
                }
                finally {
                    if ( bucket.IsReadLockHeld ) { bucket.ExitReadLock(); }
                }

                return default;
            }

            set {
                if ( key == null ) { throw new ArgumentNullException( nameof( key ) ); }

                if ( this.IsReadOnly ) { return; }

                if ( !value.HasValue ) { return; }

                var bucket = this.Bucket( key );

                try {
                    if ( bucket.TryEnterWriteLock( timeout: this.WriteTimeout ) ) { this.Dictionary.TryAdd( key, value.Value ); }
                }
                finally {
                    if ( bucket.IsWriteLockHeld ) { bucket.ExitWriteLock(); }
                }
            }
        }

        private static Byte Hash( TKey key ) {
            var hash = key.GetHashCodeByte();

            return hash;
        }

        [NotNull]
        private ReaderWriterLockSlim Bucket( TKey key ) {
            var hash = Hash( key );

            TryAgain:

            if ( this.Buckets.TryGetValue( hash, out var bucket ) ) { return bucket; }

            bucket = new ReaderWriterLockSlim( recursionPolicy: LockRecursionPolicy.SupportsRecursion );

            if ( this.Buckets.TryAdd( hash, bucket ) ) { return bucket; }

            goto TryAgain;
        }

        private IEnumerable<Byte> GetUsedBuckets() => this.Dictionary.Keys.Select( selector: Hash );

        public Boolean Add( IEnumerable<TKey> keys ) {
            if ( this.IsReadOnly ) { return false; }

            var result = Parallel.ForEach( source: keys.AsParallel(), parallelOptions: ThreadingExtensions.CPUIntensive, body: key => this.Add( key ) );

            return result.IsCompleted;
        }

        public Boolean Add( TKey key ) => this.Add( key, amount: BigInteger.One );

        public Boolean Add( TKey key, BigInteger amount ) {
            if ( this.IsReadOnly ) { return false; }

            var bucket = this.Bucket( key );

            try {
                if ( bucket.TryEnterWriteLock( timeout: this.WriteTimeout ) ) {
                    if ( !this.Dictionary.ContainsKey( key ) ) { this.Dictionary.TryAdd( key, BigInteger.Zero ); }

                    this.Dictionary[key] += amount;

                    return true;
                }
            }
            finally {
                if ( bucket.IsWriteLockHeld ) { bucket.ExitWriteLock(); }
            }

            return false;
        }

        /// <summary>
        ///     Mark that this container will now become ReadOnly/immutable. No more adds or subtracts.
        /// </summary>
        /// <returns></returns>
        public Boolean Complete() {
            this.IsReadOnly = true;
            this.Trim();

            return this.IsReadOnly;
        }

        public override void DisposeManaged() => this.Trim();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator() => this.Dictionary.GetEnumerator();

        public Boolean Subtract( TKey key, BigInteger amount ) {
            if ( this.IsReadOnly ) { return false; }

            var bucket = this.Bucket( key );

            try {
                if ( bucket.TryEnterWriteLock( timeout: this.WriteTimeout ) ) {
                    if ( !this.Dictionary.ContainsKey( key ) ) { this.Dictionary.TryAdd( key, BigInteger.Zero ); }

                    this.Dictionary[key] -= amount;

                    return true;
                }
            }
            finally {
                if ( bucket.IsWriteLockHeld ) { bucket.ExitWriteLock(); }
            }

            return false;
        }

        /// <summary>
        ///     Return the sum of all values.
        /// </summary>
        /// <returns></returns>
        public BigInteger Sum() => this.Dictionary.Aggregate( seed: BigInteger.Zero, func: ( current, pair ) => current + pair.Value );

        public void Trim() =>
                    Parallel.ForEach( source: this.Dictionary.Where( pair => pair.Value == default( BigInteger ) || pair.Value == BigInteger.Zero ), parallelOptions: ThreadingExtensions.CPUIntensive,
                        body: pair => { this.Dictionary.TryRemove( pair.Key, out var dummy ); } );

        /// <summary>
        ///     Mark that this container will now become UnReadOnly/immutable. Allow more adds and subtracts.
        /// </summary>
        /// <returns></returns>
        public Boolean UnComplete() {
            this.IsReadOnly = false;
            this.Trim();

            return !this.IsReadOnly;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<Tuple<TKey, BigInteger>> IEnumerable<Tuple<TKey, BigInteger>>.GetEnumerator() => ( IEnumerator<Tuple<TKey, BigInteger>> )this.GetEnumerator();
    }
}