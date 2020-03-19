﻿// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "UniqueExtensions.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
// 
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", File: "UniqueExtensions.cs" was last formatted by Protiguous on 2020/03/18 at 10:26 AM.

namespace Librainian.OperatingSystem.FileSystem {

    using System;
    using JetBrains.Annotations;
    using Parsing;

    public static class UniqueExtensions {

        [NotNull]
        public static Unique ToUnique( [NotNull] this String location ) {
            if ( String.IsNullOrWhiteSpace( location ) ) {
                throw new ArgumentException( "Value cannot be null or whitespace.", nameof( location ) );
            }

            return Unique.TryCreate( location, out var unique ) ? unique : throw new InvalidOperationException( $"Invalid location '{location}' given." );
        }

        /// <summary>Convert a <see cref="TrimmedString" /> to a <see cref="Unique" /> location.</summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [NotNull]
        public static Unique ToUnique( this TrimmedString location ) =>
            Unique.TryCreate( location, out var unique ) ? unique : throw new InvalidOperationException( $"Invalid location '{location}' given." );

        [NotNull]
        public static Unique ToUnique( [NotNull] this Uri location ) {
            if ( location is null ) {
                throw new ArgumentNullException( nameof( location ) );
            }

            return Unique.TryCreate( location, out var unique ) ? unique : throw new InvalidOperationException( $"Invalid location '{location}' given." );
        }

    }

}