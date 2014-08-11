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
// "Librainian/Guids.cs" was last cleaned by Rick on 2014/08/11 at 12:36 AM
#endregion

namespace Librainian.Collections {
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Runtime.Serialization;
    using Annotations;

    [DataContract( IsReference = true )]
    public class Guids {
        /// <summary>
        /// </summary>
        /// <remarks>
        ///     no guarantee on the add/remove order with a ConcurrentBag, is there? If there is (so
        ///     Add/Remove would cycle through all items) and if ConcurrentBag is faster.. then use ConcurrentBag.
        /// </remarks>
        [DataMember] [OptionalField] [NotNull] public readonly ConcurrentQueue< Guid > Collection = new ConcurrentQueue< Guid >();

        public void Add( Guid guid ) {
            if ( !this.Collection.Contains( guid ) ) {
                this.Collection.Enqueue( guid );
            }
        }

        public void Remove( Guid guid ) {
            while ( null != this.Collection && this.Collection.Contains( guid ) ) {
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

        public Boolean TryAdd( Guid guid ) {
            if ( !this.Collection.Contains( guid ) ) {
                this.Collection.Enqueue( guid );
            }
            return true;
        }
    }
}
