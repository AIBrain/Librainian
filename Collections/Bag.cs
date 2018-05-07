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
// "Librainian/Bag.cs" was last cleaned by Protiguous on 2018/05/06 at 9:30 PM

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
        /// Removes the first <paramref name="item"/> found.
        /// </summary>
        /// <param name="item"></param>
        public void Remove( TValue item ) {
            var right = new ConcurrentBag<TValue>();

#if DEBUG
            var before = this.Collection.Count( value => Equals( value, item ) );
#endif

            foreach ( var source in this.Collection.TakeWhile( source => !Equals( source, item ) ) ) {
                right.Add( item: source );
            }

            while ( right.TryTake( result: out var result ) ) {
                this.Collection.Add( item: result );
            }
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