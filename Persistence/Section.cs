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
// "Librainian/Section.cs" was last cleaned by Protiguous on 2018/05/06 at 10:23 PM

namespace Librainian.Persistence {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// <para>This just wraps a ConcurrentDictionary with a Guid ID so we can index the data without throwing exceptions on missing or null keys.</para>
    /// <para>Does not throw <see cref="ArgumentNullException"/> on null keys passed to the indexer.</para>
    /// </summary>
    [DebuggerDisplay( value: "{" + nameof( ToString ) + "(),nq}" )]
    [JsonObject]
    public class Section : IEquatable<Section> {

        /// <summary>
        /// Create a new <see cref="ID"/> for this <see cref="Section"/>.
        /// </summary>
        public Section() => this.ID = Guid.NewGuid();

        public Section( Guid id ) => this.ID = id;

        private ConcurrentDictionary<String, String> Data { get; } = new ConcurrentDictionary<String, String>();

        /// <summary>
        /// The identifier for this dictionary of <see cref="Data"/>.
        /// </summary>
        [JsonProperty]
        public Guid ID { get; }

        [NotNull]
        public IReadOnlyList<String> Keys => ( IReadOnlyList<String> )this.Data.Keys;

        [NotNull]
        public IReadOnlyList<String> Values => ( IReadOnlyList<String> )this.Data.Values;

        public String this[[CanBeNull] String key] {
            [CanBeNull]
            get {
                if ( key is null ) {
                    return null;
                }

                return this.Data.TryGetValue( key, value: out var value ) ? value : null;
            }

            set {
                if ( key is null ) {
                    return;
                }

                this.Data[key] = value;
            }
        }

        /// <summary>
        /// Static comparison. Checks pointers and then <see cref="ID"/>.
        /// </summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( Section left, Section right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            if ( ReferenceEquals( left.Data, right.Data ) ) {
                return true;
            }

            return left.ID.Equals( g: right.ID ) && left.Data.Equals( right.Data );
        }

        public static Boolean operator !=( Section left, Section right ) => !Equals( left: left, right: right );

        public static Boolean operator ==( Section left, Section right ) => Equals( left: left, right: right );

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public Boolean Equals( Section other ) => Equals( left: this, right: other );

        public override Boolean Equals( Object obj ) => Equals( left: this, right: obj as Section );

        public override Int32 GetHashCode() => this.Data.GetHashCode();

        public override String ToString() => $"{this.ID:D}";
    }
}