// Copyright 2018 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the original license has been overwritten by the automatic formatting of this code. Any unmodified sections of source code
// borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations, royalties, and licenses can be paid via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/MultiKeyDictionary.cs" was last cleaned by Protiguous on 2018/05/06 at 9:31 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Multi-Key Dictionary Class
    /// </summary>
    /// <typeparam name="TK">Primary Key Type</typeparam>
    /// <typeparam name="TL">Sub Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    public class MultiKeyDictionary<TK, TL, TV> : ConcurrentDictionary<TK, TV> {
        internal readonly ConcurrentDictionary<TK, TL> PrimaryToSubkeyMapping = new ConcurrentDictionary<TK, TL>();
        internal readonly ConcurrentDictionary<TL, TK> SubDictionary = new ConcurrentDictionary<TL, TK>();

        public TV this[TL subKey] {
            get {
                if ( this.TryGetValue( subKey: subKey, val: out var item ) ) {
                    return item;
                }

                throw new KeyNotFoundException( message: $"sub key not found: {subKey}" );
            }
        }

        public new TV this[TK primaryKey] {
            get {
                if ( this.TryGetValue( primaryKey: primaryKey, val: out var item ) ) {
                    return item;
                }

                throw new KeyNotFoundException( message: $"primary key not found: {primaryKey}" );
            }
        }

        public void Add( TK primaryKey, TV val ) => TryAdd(primaryKey, value: val );

        public void Add( TK primaryKey, TL subKey, TV val ) {
            TryAdd(primaryKey, value: val );

            this.Associate( subKey: subKey, primaryKey: primaryKey );
        }

        public void Associate( TL subKey, TK primaryKey ) {
            if ( !base.ContainsKey(primaryKey ) ) {
                throw new KeyNotFoundException( message: $"The primary dictionary does not contain the key '{primaryKey}'" );
            }

            if ( this.SubDictionary.ContainsKey(subKey ) ) {
                this.SubDictionary[  subKey] = primaryKey;
                this.PrimaryToSubkeyMapping[  primaryKey] = subKey;
            }
            else {
                this.SubDictionary.TryAdd(subKey, value: primaryKey );
                this.PrimaryToSubkeyMapping.TryAdd(primaryKey, value: subKey );
            }
        }

        public TK[] ClonePrimaryKeys() => Keys.ToArray();

        public TL[] CloneSubKeys() => this.SubDictionary.Keys.ToArray();

        public TV[] CloneValues() => Values.ToArray();

        public Boolean ContainsKey( TL subKey ) => this.TryGetValue( subKey: subKey, val: out var val );

        public new Boolean ContainsKey( TK primaryKey ) => this.TryGetValue( primaryKey: primaryKey, val: out var val );

        public void Remove( TK primaryKey ) {
            this.SubDictionary.TryRemove(this.PrimaryToSubkeyMapping[  primaryKey], value: out var kvalue );

            this.PrimaryToSubkeyMapping.TryRemove(primaryKey, value: out var lvalue );

            TryRemove(primaryKey, value: out var value );
        }

        public void Remove( TL subKey ) {
            TryRemove(this.SubDictionary[  subKey], value: out var value );
            this.PrimaryToSubkeyMapping.TryRemove(this.SubDictionary[  subKey], value: out var lvalue );
            this.SubDictionary.TryRemove(subKey, value: out var kvalue );
        }

        public Boolean TryGetValue( TL subKey, out TV val ) {
            val = default;

            return this.SubDictionary.TryGetValue(subKey, value: out var ep ) && this.TryGetValue( primaryKey: ep, val: out val );
        }

        public new Boolean TryGetValue( TK primaryKey, out TV val ) => base.TryGetValue(primaryKey, value: out val );
    }
}