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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Thing.cs" was last cleaned by Rick on 2014/10/21 at 5:01 AM

namespace Librainian.Knowledge {
    using System;
    using System.Collections.Concurrent;
    using JetBrains.Annotations;
    using Maths;

    /// <summary>
    /// new Thing("Morris", "casually talking with the user Rick")
    /// </summary>
    /// <example>an object exists, and it is called Morris.</example>
    public class Thing {

        public Thing( [NotNull] String label, [NotNull] Domain domain ) {
            if ( label == null ) {
                throw new ArgumentNullException( nameof( label ) );
            }
            if ( domain == null ) {
                throw new ArgumentNullException( nameof( domain ) );
            }
            this.Label = label;
            this.Domain = domain;
            this.SubClassesOf = new ConcurrentDictionary<TypeOrClass, Percentage>();
        }

        public Domain Domain {
            get;
            private set;
        }

        public ConcurrentDictionary<TypeOrClass, Percentage> HasTheseSubClasses {
            get;
            private set;
        }

        public String Label {
            get;
            private set;
        }

        /// <summary>
        /// This <see cref="Thing" /> is a subClass of what <see cref="TypeOrClass" /> with a
        /// percentage of Trueness (determined so far, updated when we have new info)
        /// </summary>
        public ConcurrentDictionary<TypeOrClass, Percentage> SubClassesOf {
            get;
            private set;
        }
    }
}