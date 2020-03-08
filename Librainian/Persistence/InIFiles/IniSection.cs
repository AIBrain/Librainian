// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "IniSection.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "IniSection.cs" was last formatted by Protiguous on 2020/01/31 at 12:29 AM.

namespace Librainian.Persistence.InIFiles {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;
    using Parsing;

    [JsonObject]
    public class IniSection : IReadOnlyList<IniLine> {

        [JsonProperty]
        [NotNull]
        [ItemNotNull]
        private List<IniLine> lines { get; } = new List<IniLine>();

        /// <summary>Gets the number of elements in the collection.</summary>
        /// <returns>The number of elements in the collection. </returns>
        public Int32 Count => this.lines.Select( pair => pair.Value ).Count();

        /// <summary>Gets the element at the specified index in the read-only list.</summary>
        /// <param name="index">The zero-based index of the element to get. </param>
        /// <returns>The element at the specified index in the read-only list.</returns>
        [NotNull]
        public IniLine this[ Int32 index ] {
            get {
                if ( index <= 0 || index > this.Count ) {
                    throw new ArgumentOutOfRangeException( nameof( index ) );
                }

                return this.lines[ index ];
            }
        }

        public Boolean Add( [NotNull] String key, [CanBeNull] String? value ) {
            if ( String.IsNullOrEmpty( key ) ) {
                throw new ArgumentException( "Value cannot be null or empty.", nameof( key ) );
            }

            this.lines.Add( new IniLine( key, value ) );

            return true;
        }

        public Boolean Exists( [NotNull] String key ) {
            if ( String.IsNullOrEmpty( key ) ) {
                return default;
            }

            return this.lines.Any( pair => pair.Key.Like( key ) );
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IniLine> GetEnumerator() => this.lines.GetEnumerator();

        public Boolean Remove( [NotNull] String key ) => this.lines.RemoveAll( pair => pair?.Key.Like( key ) == true ).Any();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}