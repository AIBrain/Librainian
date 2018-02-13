// Copyright 2016 Rick@AIBrain.org.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MultiKeyDictionary.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Multi-Key Dictionary Class</summary>
    /// <typeparam name="TK">Primary Key Type</typeparam>
    /// <typeparam name="TL">Sub Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    public class MultiKeyDictionary<TK, TL, TV> : ConcurrentDictionary<TK, TV> {
        internal readonly ConcurrentDictionary<TK, TL> PrimaryToSubkeyMapping = new ConcurrentDictionary<TK, TL>();
        internal readonly ConcurrentDictionary<TL, TK> SubDictionary = new ConcurrentDictionary<TL, TK>();

        public TV this[ TL subKey ] {
            get {
				if ( this.TryGetValue( subKey, out var item ) ) {
					return item;
				}

				throw new KeyNotFoundException( $"sub key not found: {subKey}" );
            }
        }

        public new TV this[ TK primaryKey ] {
            get {
				if ( this.TryGetValue( primaryKey, out var item ) ) {
					return item;
				}

				throw new KeyNotFoundException( $"primary key not found: {primaryKey}" );
            }
        }

        public void Add( TK primaryKey, TV val ) => this.TryAdd( primaryKey, val );

        public void Add( TK primaryKey, TL subKey, TV val ) {
            this.TryAdd( primaryKey, val );

            this.Associate( subKey, primaryKey );
        }

        public void Associate( TL subKey, TK primaryKey ) {
            if ( !base.ContainsKey( primaryKey ) ) {
                throw new KeyNotFoundException( $"The primary dictionary does not contain the key '{primaryKey}'" );
            }

            if ( this.SubDictionary.ContainsKey( subKey ) ) {
                this.SubDictionary[ subKey ] = primaryKey;
                this.PrimaryToSubkeyMapping[ primaryKey ] = subKey;
            }
            else {
                this.SubDictionary.TryAdd( subKey, primaryKey );
                this.PrimaryToSubkeyMapping.TryAdd( primaryKey, subKey );
            }
        }

        public TK[] ClonePrimaryKeys() => this.Keys.ToArray();

        public TL[] CloneSubKeys() => this.SubDictionary.Keys.ToArray();

        public TV[] CloneValues() => this.Values.ToArray();

        public Boolean ContainsKey( TL subKey ) => this.TryGetValue( subKey, out var val );

	    public new Boolean ContainsKey( TK primaryKey ) => this.TryGetValue( primaryKey, out var val );

	    public void Remove( TK primaryKey ) {
			this.SubDictionary.TryRemove( key: this.PrimaryToSubkeyMapping[ primaryKey ], value: out var kvalue );

			this.PrimaryToSubkeyMapping.TryRemove( key: primaryKey, value: out var lvalue );

			this.TryRemove( primaryKey, out var value );
		}

        public void Remove( TL subKey ) {
			this.TryRemove( this.SubDictionary[ subKey ], out var value );
			this.PrimaryToSubkeyMapping.TryRemove( key: this.SubDictionary[ subKey ], value: out var lvalue );
			this.SubDictionary.TryRemove( key: subKey, value: out var kvalue );
		}

        public Boolean TryGetValue( TL subKey, out TV val ) {
            val = default;

			return this.SubDictionary.TryGetValue( subKey, out var ep ) && this.TryGetValue( ep, out val );
		}

        public new Boolean TryGetValue( TK primaryKey, out TV val ) => base.TryGetValue( primaryKey, out val );
    }
}