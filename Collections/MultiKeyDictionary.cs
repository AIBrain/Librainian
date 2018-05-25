// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "MultiKeyDictionary.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/MultiKeyDictionary.cs" was last formatted by Protiguous on 2018/05/24 at 6:59 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Multi-Key Dictionary Class
    /// </summary>
    /// <typeparam name="TK">Primary Key Type</typeparam>
    /// <typeparam name="TL">Sub Key Type</typeparam>
    /// <typeparam name="TV">Value Type</typeparam>
    public class MultiKeyDictionary<TK, TL, TV> : ConcurrentDictionary<TK, TV> {

        internal ConcurrentDictionary<TK, TL> PrimaryToSubkeyMapping { get; } = new ConcurrentDictionary<TK, TL>();

        internal ConcurrentDictionary<TL, TK> SubDictionary { get; } = new ConcurrentDictionary<TL, TK>();

        public TV this[TL subKey] {
            get {
                if ( this.TryGetValue( subKey: subKey, val: out var item ) ) { return item; }

                throw new KeyNotFoundException( $"sub key not found: {subKey}" );
            }
        }

        public new TV this[TK primaryKey] {
            get {
                if ( this.TryGetValue( primaryKey: primaryKey, val: out var item ) ) { return item; }

                throw new KeyNotFoundException( $"primary key not found: {primaryKey}" );
            }
        }

        public void Add( TK primaryKey, TV val ) => this.TryAdd( primaryKey, val );

        public void Add( TK primaryKey, TL subKey, TV val ) {
            this.TryAdd( primaryKey, val );

            this.Associate( subKey: subKey, primaryKey: primaryKey );
        }

        public void Associate( TL subKey, TK primaryKey ) {
            if ( !base.ContainsKey( primaryKey ) ) { throw new KeyNotFoundException( $"The primary dictionary does not contain the key '{primaryKey}'" ); }

            if ( this.SubDictionary.ContainsKey( subKey ) ) {
                this.SubDictionary[subKey] = primaryKey;
                this.PrimaryToSubkeyMapping[primaryKey] = subKey;
            }
            else {
                this.SubDictionary.TryAdd( subKey, primaryKey );
                this.PrimaryToSubkeyMapping.TryAdd( primaryKey, subKey );
            }
        }

        public TK[] ClonePrimaryKeys() => this.Keys.ToArray();

        public TL[] CloneSubKeys() => this.SubDictionary.Keys.ToArray();

        public TV[] CloneValues() => this.Values.ToArray();

        public Boolean ContainsKey( TL subKey ) => this.TryGetValue( subKey: subKey, val: out var val );

        public new Boolean ContainsKey( TK primaryKey ) => this.TryGetValue( primaryKey: primaryKey, val: out var val );

        public void Remove( TK primaryKey ) {
            this.SubDictionary.TryRemove( this.PrimaryToSubkeyMapping[primaryKey], out var kvalue );

            this.PrimaryToSubkeyMapping.TryRemove( primaryKey, out var lvalue );

            this.TryRemove( primaryKey, out var value );
        }

        public void Remove( TL subKey ) {
            this.TryRemove( this.SubDictionary[subKey], out var value );
            this.PrimaryToSubkeyMapping.TryRemove( this.SubDictionary[subKey], out var lvalue );
            this.SubDictionary.TryRemove( subKey, out var kvalue );
        }

        public Boolean TryGetValue( TL subKey, out TV val ) {
            val = default;

            return this.SubDictionary.TryGetValue( subKey, out var ep ) && this.TryGetValue( primaryKey: ep, val: out val );
        }

        public new Boolean TryGetValue( TK primaryKey, out TV val ) => base.TryGetValue( primaryKey, out val );
    }
}