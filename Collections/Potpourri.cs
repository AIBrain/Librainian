namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;

    [DataContract( IsReference = true )]
    [Serializable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public  class Potpourri<TKey> : IPotpourri<TKey> where TKey : class {

        [DataMember]
        [NotNull]
        protected readonly ConcurrentDictionary<TKey, BigInteger> Container = new ConcurrentDictionary<TKey, BigInteger>();

        [NotNull]
        protected string FriendlyName {
            get {
                return Types.GetPropertyName( () => this );
            }
        }

        [UsedImplicitly]
        protected string DebuggerDisplay {
            get {
                return string.Format( "{0}({1}) ", this.FriendlyName, this.Container.Select( pair => pair.Key.ToString() ).ToStrings() );
            }
        }

        public void Add( TKey key ) {
            if ( Equals( key, default( TKey ) ) ) {
                return;
            }
            this.Container.AddOrUpdate( key: key, addValue: BigInteger.One, updateValueFactory: ( particles, integer ) => integer + BigInteger.One );
        }

        public void Add( TKey key, BigInteger count ) {
            if ( Equals( key, default( TKey ) ) ) {
                return;
            }
            this.Container.AddOrUpdate( key: key, addValue: count, updateValueFactory: ( particles, integer ) => integer + count );
        }

        public void Add( KeyValuePair< TKey, BigInteger > keyValuePair ) {
            this.Add( keyValuePair.Key, keyValuePair.Value );
        }

        //public void Add( KeyValuePair<TKey, BigInteger> keyValuePair ) {
        //    this.Add( keyValuePair.Key, keyValuePair.Value );
        //}

        public void Add( Tuple<TKey, BigInteger> keyValuePair ) {
            this.Add( keyValuePair.Item1, keyValuePair.Item2 );
        }


        public void Clear() {
            this.Container.Clear();
        }

        public Boolean Contains( TKey key ) {
            BigInteger value;
            if ( !this.Container.TryGetValue( key, out value ) ) {
                return false;
            }
            return value > BigInteger.Zero;
        }

        public BigInteger Count() {
            return this.Container.Aggregate( BigInteger.Zero, ( current, kvp ) => current + kvp.Value );
        }

        public BigInteger Count<TParticle>(  ) {
            return this.Container.Where( pair => pair.Key is TParticle  ).Aggregate( BigInteger.Zero, ( current, kvp ) => current + kvp.Value );   //BUG is this right?
        }

        /// <summary>
        /// Get all particles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, BigInteger>> Get() {
            return this.Container;
        }

        /// <summary>
        /// Get all particles of type(<see cref="TParticle"/>).
        /// </summary>
        /// <typeparam name="TParticle"></typeparam>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, BigInteger>> Get<TParticle>() {
            return this.Container.Where( pair => pair.Key is TParticle );
        }

        //public IEnumerable<KeyValuePair<TKey, BigInteger>> Get<TCertainType>() {
        //    var keys = this.Container.Keys.Cast<TCertainType>();
        //    foreach ( TKey certainType in keys ) {
        //        if ( certainType != null ) {
        //            yield return new KeyValuePair<TCertainType, BigInteger>( certainType, this.Container[ certainType ] );
        //        }
        //    }
        //    //yield return new KeyValuePair< TCertainType, BigInteger >( result.Key as TCertainType, result.Value );

        //}

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, BigInteger>> GetEnumerator() {
            return this.Container.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public Boolean Remove( TKey key, BigInteger count ) {
            var before = this.Count();
            if ( before > count ) {
                count = before; //only remove what is there at the moment.
            }
            var newValue = this.Container.AddOrUpdate( key: key, addValue: 0, updateValueFactory: ( particles, integer ) => integer - count );
            return before != newValue;
        }

        public bool RemoveAll( TKey key ) {
            BigInteger value;
            return this.Container.TryRemove( key, out value );
        }
    }
}