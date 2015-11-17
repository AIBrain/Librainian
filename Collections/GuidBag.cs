// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Guids.cs" was last cleaned by Rick on 2015/06/12 at 2:50 PM

namespace Librainian.Collections {

    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;

    [DataContract( IsReference = true )]
    public class GuidBag {

        /// <summary></summary>
        /// <remarks>
        /// no guarantee on the add/remove order with a ConcurrentBag, is there? If there is (so
        /// Add/Remove would cycle through all items) and if ConcurrentBag is faster.. then use ConcurrentBag.
        /// </remarks>
        [DataMember]
        [NotNull]
        public ConcurrentQueue<Guid> Collection {
            get;
        }
        = new ConcurrentQueue<Guid>();

        public void Add(Guid guid) {
            if ( !this.Collection.Contains( guid ) ) {
                this.Collection.Enqueue( guid );
            }
        }

        public void Remove(Guid guid) {
            while ( ( null != this.Collection ) && this.Collection.Contains( guid ) ) {
                Guid dummy;
                if ( !this.Collection.TryDequeue( out dummy ) ) {
                    return;
                }
                if ( Equals( dummy, guid ) ) {
                    return;
                }
                this.Collection.Enqueue( dummy );
            }
        }

        public Boolean TryAdd(Guid guid) {
            if ( !this.Collection.Contains( guid ) ) {
                this.Collection.Enqueue( guid );
            }
            return true;
        }
    }
}