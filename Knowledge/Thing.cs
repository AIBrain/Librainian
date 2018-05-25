// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous.
// All Rights Reserved.
//
// This ENTIRE copyright notice and file header MUST BE KEPT
// VISIBLE in any source code derived from or used from our
// libraries and projects.
//
// =========================================================
// This section of source code, "Thing.cs",
// belongs to Rick@AIBrain.org and Protiguous@Protiguous.com
// unless otherwise specified OR the original license has been
// overwritten by the automatic formatting.
//
// (We try to avoid that from happening, but it does happen.)
//
// Any unmodified portions of source code gleaned from other
// projects still retain their original license and our thanks
// goes to those Authors.
// =========================================================
//
// Donations (more please!), royalties from any software that
// uses any of our code, and license fees can be paid to us via
// bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or compiled binaries is AS-IS.
// No warranties are expressed or implied.
// I am NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
//
// "Librainian/Librainian/Thing.cs" was last cleaned by Protiguous on 2018/05/15 at 10:43 PM.

namespace Librainian.Knowledge {

    using System;
    using System.Collections.Concurrent;
    using JetBrains.Annotations;
    using Maths.Numbers;

    /// <summary>new Thing("Morris", "casually talking with the user Rick")</summary>
    /// <example>an object exists, and it is called Morris.</example>
    public class Thing {

        public Domain Domain { get; }

        public ConcurrentDictionary<TypeOrClass, Percentage> HasTheseSubClasses { get; private set; }

        public String Label { get; }

        /// <summary>
        ///     This <see cref="Thing" /> is a subClass of what <see cref="TypeOrClass" /> with a
        ///     percentage of Trueness (determined so far, updated when we have new info)
        /// </summary>
        public ConcurrentDictionary<TypeOrClass, Percentage> SubClassesOf { get; }

        public Thing( [NotNull] String label, [NotNull] Domain domain ) {
            this.Label = label ?? throw new ArgumentNullException( nameof( label ) );
            this.Domain = domain ?? throw new ArgumentNullException( nameof( domain ) );
            this.SubClassesOf = new ConcurrentDictionary<TypeOrClass, Percentage>();
        }
    }
}