namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.Serialization;
    using Annotations;
    using Extensions;

    [DataContract(IsReference = true)]
    [Serializable]
    [DebuggerDisplay( "{DebuggerDisplay,nq}" )]
    public class Potpourri<TKey> : IPotpourri< TKey > {

        [DataMember]
        [NotNull]
        protected readonly ConcurrentDictionary<TKey, BigInteger> Container = new ConcurrentDictionary<TKey, BigInteger>();

        [NotNull]
        public string FriendlyName {
            get {
                return Types.GetPropertyName( () => this );
            }
        }

        [UsedImplicitly]
        private string DebuggerDisplay {
            get {
                return string.Format( "{0}({1}) ", this.FriendlyName, this.Container.Select( pair => pair.Key.ToString() ).ToStrings() );
            }
        }

        public Boolean Remove( TKey key, BigInteger count ) {
            var before = this.Count();
            if ( before > count ) {
                count = before; //only remove what there is.
            }
            var newValue = this.Container.AddOrUpdate( key: key, addValue: 0, updateValueFactory: ( particles, integer ) => integer - count );
            return before != newValue;
        }

        public void Add( TKey key, BigInteger count ) {
            if ( Equals( key, default( TKey ) ) ) {
                return;
            }
            //if ( count <= BigInteger.Zero ) {
            //    return;
            //}
            this.Container.AddOrUpdate( key: key, addValue: count, updateValueFactory: ( particles, integer ) => integer + count );
        }

        public void Clear() {
            this.Container.Clear();
        }

        public Boolean Contains( [CanBeNull] TKey key ) {
            BigInteger value;
            if ( !this.Container.TryGetValue( key, out value ) ) {
                return false;
            }
            return value > BigInteger.Zero;
        }

        public BigInteger Count() {
            return this.Container.Aggregate( BigInteger.Zero, ( current, kvp ) => current + kvp.Value );
        }

        public IEnumerable<KeyValuePair< TKey, BigInteger > > Get() {
            return this.Container;
        }
    }
}