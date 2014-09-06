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
// "Librainian/PersistedDatabase.cs" was last cleaned by Rick on 2014/08/11 at 12:38 AM
#endregion

namespace Librainian.Persistence {
/*
    [DataContract( IsReference = true )]
    public class PersistedDatabase<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
        /// <summary>
        /// </summary>
        [NotNull]
        [DataMember]
        [OptionalField]
        public readonly ConcurrentDictionary<TKey, TValue> Collection = new ConcurrentDictionary<TKey, TValue>();

        /// <summary>
        /// </summary>
        private readonly String _persistPath;

        /// <summary>
        /// </summary>
        /// <param name="persistFileName"></param>
        public PersistedDatabase( String persistFileName ) {
            if ( String.IsNullOrWhiteSpace( persistFileName ) ) {
                throw new ArgumentNullException( "persistFileName" );
            }
            this._persistPath = persistFileName;
        }

        [CanBeNull]
        public ProgressChangedEventHandler Feedback {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        [CanBeNull]
        public Action OnAfterDeserialized {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        public DateTime? WhenDeserialized {
            get;
            private set;
        }

        /// <summary>
        /// </summary>
        public DateTime? WhenSerialized {
            get;
            private set;
        }

        /// <summary>
        ///     indexer. return the value, or default().
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[ TKey key ] {
            get {
                TValue result;
                return this.Collection.TryGetValue( key, out result ) ? result : default( TValue );
            }

            set {
                this.Collection[ key ] = value;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return this.Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public Boolean Add( TKey key, TValue value ) {
            return this.Collection.TryAdd( key, value );
        }

        public void Clear() {
            this.Collection.Clear();
        }

        /// <summary>
        ///     Call when ready to load data back in.
        /// </summary>
        public void Deserialize() {
            if ( !this._persistPath.Loader<PersistedDatabase<TKey, TValue>>( database => database.DeepClone( destination: this ), this.Feedback ) ) {
                return;
            }

            this.WhenDeserialized = Date.Now;
            var onAfterDeserialized = this.OnAfterDeserialized;
            if ( onAfterDeserialized != null ) {
                onAfterDeserialized();
            }
        }

        public void Serialize() {
            if ( this.Saver( this._persistPath ) ) {
                this.WhenSerialized = DateTime.Now;
            }
        }
    }
*/
}
