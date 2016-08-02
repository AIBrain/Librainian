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
// "Librainian/Bag.cs" was last cleaned by Rick on 2016/06/18 at 10:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using FluentAssertions;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [JsonObject]
    public class Bag<TValue> {

        /// <summary></summary>
        /// <remarks>
        ///     no guarantee on the add/remove order with a ConcurrentBag
        /// </remarks>
        [JsonProperty]
        [NotNull]
        private ConcurrentBag<TValue> Collection { get; } = new ConcurrentBag<TValue>();

        public void Add( TValue item ) {
            this.Collection.Add( item );
        }

        /// <summary>
        ///     Removes the first <paramref name="item" /> found.
        /// </summary>
        /// <param name="item"></param>
        public void Remove( TValue item ) {
            var right = new ConcurrentBag<TValue>();

#if DEBUG
            var before = this.Collection.Count( guid1 => Equals( guid1, item ) );
#endif

            foreach ( var source in this.Collection.TakeWhile( source => !Equals( source, item ) ) ) {
                right.Add( source );
            }

            TValue result;
            while ( right.TryTake( out result ) ) {
                this.Collection.Add( result );
            }
#if DEBUG
            var after = this.Collection.Count( guid1 => Equals( guid1, item ) );
            before.Should().BeGreaterOrEqualTo( after );
#endif
        }

        public Boolean TryAdd( TValue guid ) {
            this.Collection.Add( guid );
            return true;
        }
    }
}