﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "D.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal:Protiguous@Protiguous.com
//     (We're always looking into other solutions.. Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", "D.cs" was last formatted by Protiguous on 2019/08/08 at 9:28 AM.

namespace LibrainianCore.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualBasic;

    /// <summary>
    ///     <para>[D]ata([K]ey=[V]alue)</para>
    ///     <para>[K] is not mutable, and can be an empty string, and contain whitespace.</para>
    ///     <para>[V] is mutable, and can be a null string.</para>
    /// </summary>
    [DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
    [Serializable]
    [JsonObject( MemberSerialization.OptIn, IsReference = false, ItemIsReference = false, ItemNullValueHandling = NullValueHandling.Ignore,
        ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore )]
    public class D : IEqualityComparer<D> {

        /// <summary>
        ///     The key.
        /// </summary>
        [JsonProperty( IsReference = false, ItemIsReference = false )]
        [NotNull]
        public String K { get; }

        /// <summary>
        ///     The value.
        /// </summary>
        [JsonProperty( IsReference = false, ItemIsReference = false )]
        [CanBeNull]
        public String V { get; set; }

        public D() {
            this.K = String.Empty;
            this.V = null;
        }

        public D( [NotNull] String key ) => this.K = key ?? throw new ArgumentNullException( nameof( key ) );

        public D( [NotNull] String key, [CanBeNull] String value ) {
            this.K = key ?? throw new ArgumentNullException( paramName: nameof( key ) );
            this.V = value;
        }

        public override Int32 GetHashCode() => this.K.GetHashCode();

        public override String ToString() {
            var keypart = String.Empty;

            if ( this.K.Length > 42 ) {
                var left = Strings.Left( this.K, 20 );
                var right = Strings.Right( this.K, 20 );

                keypart = $"{left}..{right}";
            }

            if ( this.V is null ) {
                return $"{keypart}=";
            }

            var valuepart = String.Empty;

            if ( this.V.Length > 42 ) {
                var left = Strings.Left( this.V, 20 );
                var right = Strings.Right( this.V, 20 );

                valuepart = $"{left}..{right}";
            }

            return $"{keypart}={valuepart}";
        }

        /// <summary>
        ///     <para>Static equality test.</para>
        ///     <para>Return true if: K and K have the same value, and V and V have the same value.</para>
        ///     <para>Two nulls should be equal.</para>
        ///     <para>Comparison is by <see cref="StringComparison.Ordinal" />.</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( D left, D right ) {
            if ( ReferenceEquals( left, right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return false;
            }

            if ( !left.K.Equals( right.K, StringComparison.Ordinal ) ) {
                return false;
            }

            if ( ReferenceEquals( left.V, right.V ) ) {
                return true;
            }

            if ( left.V is null || right.V is null ) {
                return false;
            }

            return left.V.Equals( right.V, StringComparison.Ordinal );
        }

        public override Boolean Equals( Object obj ) => Equals( this, obj as D );

        public Int32 GetHashCode( D d ) => d.K.GetHashCode();

        Boolean IEqualityComparer<D>.Equals( D x, D y ) => Equals( x, y );
    }
}