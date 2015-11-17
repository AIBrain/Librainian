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
// "Librainian/Topic.cs" was last cleaned by Rick on 2015/06/12 at 2:59 PM

namespace Librainian.Knowledge {

    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Collections;

    [DataContract( IsReference = true )]
    public class Topic : IEqualityComparer<Topic> {

        [DataMember]
        public readonly ConcurrentList<Factoid> Facts = new ConcurrentList<Factoid>();

        [DataMember]
        public String Description {
            get; private set;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        public Boolean Equals(Topic x, Topic y) {
            throw new NotImplementedException();
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">
        /// The <see cref="T:System.Object" /> for which a hash code is to be returned.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.
        /// </exception>
        public Int32 GetHashCode(Topic obj) {
            throw new NotImplementedException();
        }
    }
}