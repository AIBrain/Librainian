// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Thing.cs" was last cleaned by Protiguous on 2016/06/18 at 10:52 PM

namespace Librainian.Knowledge {

    using System;
    using System.Collections.Concurrent;
    using JetBrains.Annotations;
    using Maths.Numbers;

    /// <summary>new Thing("Morris", "casually talking with the user Rick")</summary>
    /// <example>an object exists, and it is called Morris.</example>
    public class Thing {

        public Thing( [NotNull] String label, [NotNull] Domain domain ) {
	        this.Label = label ?? throw new ArgumentNullException( nameof( label ) );
            this.Domain = domain ?? throw new ArgumentNullException( nameof( domain ) );
            this.SubClassesOf = new ConcurrentDictionary<TypeOrClass, Percentage>();
        }

        public Domain Domain {
            get;
        }

        public ConcurrentDictionary<TypeOrClass, Percentage> HasTheseSubClasses {
            get; private set;
        }

        public String Label {
            get;
        }

        /// <summary>
        ///     This <see cref="Thing" /> is a subClass of what <see cref="TypeOrClass" /> with a
        ///     percentage of Trueness (determined so far, updated when we have new info)
        /// </summary>
        public ConcurrentDictionary<TypeOrClass, Percentage> SubClassesOf {
            get;
        }
    }
}