// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "Bag.cs" belongs to Rick@AIBrain.org and
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
//
// "Librainian/Librainian/Bag.cs" was last formatted by Protiguous on 2018/05/21 at 10:49 PM.

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public class Bag<TValue> {

        /// <summary>
        /// </summary>
        /// <remarks>no guarantee on the add/remove order with a ConcurrentBag</remarks>
        [JsonProperty]
        [NotNull]
        private ConcurrentBag<TValue> Collection { get; } = new ConcurrentBag<TValue>();

        public void Add( TValue item ) => this.Collection.Add( item: item );

        /// <summary>
        ///     Removes the first <paramref name="item" /> found.
        /// </summary>
        /// <param name="item"></param>
        public void Remove( TValue item ) {
            var right = new ConcurrentBag<TValue>();

#if DEBUG
            var before = this.Collection.Count( value => Equals( value, item ) );
#endif

            foreach ( var source in this.Collection.TakeWhile( source => !Equals( source, item ) ) ) { right.Add( item: source ); }

            while ( right.TryTake( result: out var result ) ) { this.Collection.Add( item: result ); }
#if DEBUG
            var after = this.Collection.Count( value => Equals( value, item ) );
            before.Should().BeGreaterOrEqualTo( expected: after );
#endif
        }

        public Boolean TryAdd( TValue guid ) {
            this.Collection.Add( item: guid );

            return true;
        }
    }
}