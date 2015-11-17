// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
//  
// "Librainian/CountableBigIntegers.cs" was last cleaned by Rick on 2015/10/09 at 12:42 AM

namespace Librainian.Maths {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Magic;
    using Threading;

    /// <summary>
    ///     <para>Threadsafe dictionary for large numbers ( <see cref="BigInteger" />).</para>
    ///     <para>All methods have a <see cref="ReadTimeout" /> or <see cref="WriteTimeout" />.</para>
    ///     <para>Call <see cref="Complete" /> as soon as possible.</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class CountableBigIntegers<TKey> : BetterDisposableClass {

        public CountableBigIntegers( TimeSpan readTimeout, TimeSpan writeTimeout ) {
            this.ReadTimeout = readTimeout;
            this.WriteTimeout = writeTimeout;
        }

        public Boolean IsReadOnly {
            get; private set;
        }

        public TimeSpan ReadTimeout {
            get; set;
        }

        public TimeSpan WriteTimeout {
            get; set;
        }

        /// <summary>
        ///     Quick hashes of <see cref="TKey" /> for <see cref="ReaderWriterLockSlim" />.
        /// </summary>
        private ConcurrentDictionary<Byte, ReaderWriterLockSlim> Buckets {
            get;
        } = new ConcurrentDictionary<Byte, ReaderWriterLockSlim>();

        /// <summary>
        ///     Count of each <see cref="TKey" />.
        /// </summary>
        [DataMember, NotNull]
        private ConcurrentDictionary<TKey, BigInteger> Dictionary {
            get;
        } = new ConcurrentDictionary<TKey, BigInteger>();

        [CanBeNull]
        public BigInteger? this[ TKey key ] {
            get {
                if ( key == null ) {
                    throw new ArgumentNullException( nameof( key ) );
                }
                var bucket = Bucket( key );
                try {
                    if ( bucket.TryEnterReadLock( this.ReadTimeout ) ) {
                        BigInteger result;
                        return this.Dictionary.TryGetValue( key, out result ) ? result : default( BigInteger );
                    }
                }
                finally {
                    if ( bucket.IsReadLockHeld ) {
                        bucket.ExitReadLock();
                    }
                }
                return default( BigInteger );
            }

            set {
                if ( key == null ) {
                    throw new ArgumentNullException( nameof( key ) );
                }
                if ( IsReadOnly ) {
                    return;
                }
                if ( !value.HasValue ) {
                    return;
                }

                var bucket = Bucket( key );

                try {
                    if ( bucket.TryEnterWriteLock( this.WriteTimeout ) ) {
                        this.Dictionary[ key ] = value.Value;
                    }
                }
                finally {
                    if ( bucket.IsWriteLockHeld ) {
                        bucket.ExitWriteLock();
                    }
                }
            }
        }

        public Boolean Add( TKey key, BigInteger amount ) {
            if ( IsReadOnly ) {
                return false;
            }

            var bucket = Bucket( key );

            try {
                if ( bucket.TryEnterWriteLock( this.WriteTimeout ) ) {
                    if ( !this.Dictionary.ContainsKey( key ) ) {
                        this.Dictionary[ key ] = BigInteger.Zero;
                    }

                    this.Dictionary[ key ] += amount;
                    return true;
                }
            }
            finally {
                if ( bucket.IsWriteLockHeld ) {
                    bucket.ExitWriteLock();
                }
            }
            return false;
        }

        /// <summary>
        ///     Mark that this container will now become ReadOnly/immutable. No more adds/subtracts.
        /// </summary>
        /// <returns></returns>
        public Boolean Complete() {
            CleanUpManagedResources();
            return this.IsReadOnly;
        }

        public Boolean Subtract( TKey key, BigInteger amount ) {
            if ( IsReadOnly ) {
                return false;
            }
            var bucket = Bucket( key );
            try {
                if ( bucket.TryEnterWriteLock( this.WriteTimeout ) ) {
                    if ( !this.Dictionary.ContainsKey( key ) ) {
                        this.Dictionary[ key ] = BigInteger.Zero;
                    }

                    this.Dictionary[ key ] -= amount;
                    return true;
                }
            }
            finally {
                if ( bucket.IsWriteLockHeld ) {
                    bucket.ExitWriteLock();
                }
            }
            return false;
        }

        /// <summary>
        ///     Return the sum of all keys.
        /// </summary>
        /// <returns></returns>
        public BigInteger Sum() {
            return this.Dictionary.Aggregate( BigInteger.Zero, ( current, pair ) => current + pair.Value );
        }

        protected override void CleanUpManagedResources() {
            this.IsReadOnly = true;

            Parallel.ForEach( this.Dictionary.Where( pair => ( pair.Value == BigInteger.Zero ) || ( pair.Value == default( BigInteger ) ) ), ThreadingExtensions.ParallelOptions, pair => {
                BigInteger dummy;
                this.Dictionary.TryRemove( pair.Key, out dummy );
            } );

            //var usedBuckets = this.GetUsedBuckets()
            //                      .ToHashSet();

            //for ( var i = Byte.MinValue; i < Byte.MaxValue; i++ ) {
            //    if ( usedBuckets.Contains( i ) ) {
            //        continue;
            //    }
            //    using ( this.Buckets[ i ] ) {
            //    }
            //    this.Buckets[ i ] = null;
            //}
        }

        private static Byte Hash( TKey key ) {
            unchecked {
                var hash = ( Byte )key.GetHashCode();
                return hash;
            }
        }

        [NotNull]
        private ReaderWriterLockSlim Bucket( TKey key ) {
            var hash = Hash( key );

            TryAgain:

            ReaderWriterLockSlim bucket;
            if ( this.Buckets.TryGetValue( hash, out bucket ) ) {
                return bucket;
            }

            bucket = new ReaderWriterLockSlim( LockRecursionPolicy.SupportsRecursion );
            if ( this.Buckets.TryAdd( hash, bucket ) ) {
                return bucket;
            }

            goto TryAgain;
        }

        private IEnumerable<Byte> GetUsedBuckets() {
            return this.Dictionary.Keys.Select( Hash );
        }

    }

}
