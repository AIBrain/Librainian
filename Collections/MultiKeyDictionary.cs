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
// "Librainian/MultiKeyDictionary.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Multi-Key Dictionary Class
    /// </summary>
    /// <typeparam name="K">Primary Key Type</typeparam>
    /// <typeparam name="L">Sub Key Type</typeparam>
    /// <typeparam name="V">Value Type</typeparam>
    public class MultiKeyDictionary< K, L, V > : ConcurrentDictionary< K, V > {
        internal readonly ConcurrentDictionary< K, L > primaryToSubkeyMapping = new ConcurrentDictionary< K, L >();

        internal readonly ConcurrentDictionary< L, K > subDictionary = new ConcurrentDictionary< L, K >();

        public V this[ L subKey ] {
            get {
                V item;
                if ( this.TryGetValue( subKey, out item ) ) {
                    return item;
                }

                throw new KeyNotFoundException( String.Format( "sub key not found: {0}", subKey ) );
            }
        }

        public new V this[ K primaryKey ] {
            get {
                V item;
                if ( this.TryGetValue( primaryKey, out item ) ) {
                    return item;
                }

                throw new KeyNotFoundException( String.Format( "primary key not found: {0}", primaryKey ) );
            }
        }

        public void Add( K primaryKey, V val ) {
            this.TryAdd( primaryKey, val );
        }

        public void Add( K primaryKey, L subKey, V val ) {
            this.TryAdd( primaryKey, val );

            this.Associate( subKey, primaryKey );
        }

        public void Associate( L subKey, K primaryKey ) {
            if ( !base.ContainsKey( primaryKey ) ) {
                throw new KeyNotFoundException( String.Format( "The primary dictionary does not contain the key '{0}'", primaryKey ) );
            }

            if ( this.subDictionary.ContainsKey( subKey ) ) {
                this.subDictionary[ subKey ] = primaryKey;
                this.primaryToSubkeyMapping[ primaryKey ] = subKey;
            }
            else {
                this.subDictionary.TryAdd( subKey, primaryKey );
                this.primaryToSubkeyMapping.TryAdd( primaryKey, subKey );
            }
        }

        public K[] ClonePrimaryKeys() {
            return this.Keys.ToArray();
        }

        public L[] CloneSubKeys() {
            return this.subDictionary.Keys.ToArray();
        }

        public V[] CloneValues() {
            return this.Values.ToArray();
        }

        public Boolean ContainsKey( L subKey ) {
            V val;

            return this.TryGetValue( subKey, out val );
        }

        public new Boolean ContainsKey( K primaryKey ) {
            V val;

            return this.TryGetValue( primaryKey, out val );
        }

        public void Remove( K primaryKey ) {
            K kvalue;
            this.subDictionary.TryRemove( key: this.primaryToSubkeyMapping[ primaryKey ], value: out kvalue );

            L lvalue;
            this.primaryToSubkeyMapping.TryRemove( key: primaryKey, value: out lvalue );

            V value;
            this.TryRemove( primaryKey, out value );
        }

        public void Remove( L subKey ) {
            V value;
            this.TryRemove( this.subDictionary[ subKey ], out value );
            L lvalue;
            this.primaryToSubkeyMapping.TryRemove( key: this.subDictionary[ subKey ], value: out lvalue );
            K kvalue;
            this.subDictionary.TryRemove( key: subKey, value: out kvalue );
        }

        public Boolean TryGetValue( L subKey, out V val ) {
            val = default( V );

            K ep;
            return this.subDictionary.TryGetValue( subKey, out ep ) && this.TryGetValue( ep, out val );
        }

        public new Boolean TryGetValue( K primaryKey, out V val ) {
            return base.TryGetValue( primaryKey, out val );
        }
    }
}
