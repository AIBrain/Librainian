// Copyright © 2020 Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Cache.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "LibrainianCore", File: "Cache.cs" was last formatted by Protiguous on 2020/03/16 at 3:11 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using Collections.Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Parsing;

    public static class Cache {

        /// <summary>Build a key from combining 1 or more <see cref="T" /> (converted to Strings).</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="things"></param>
        [NotNull]
        [DebuggerStepThrough]
        public static String BuildKey<T>( [NotNull] params T[] things ) {
            if ( things is null ) {
                throw new ArgumentNullException( paramName: nameof( things ) );
            }

            if ( !things.Any() ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( things ) );
            }

            var parts = things.Select( selector: o => {
                if ( o is IEnumerable<SqlParameter> parameters ) {
                    var kvp = parameters.Where( predicate: parameter => parameter != default ).Select( selector: parameter => new {
                        parameter.ParameterName, parameter.Value
                    } );

                    return $"{kvp.ToStrings( separator: Symbols.TwoPipes )}".Trim();
                }

                var s = o.Trimmed().NullIfEmpty();

                if ( s != null ) {
                    return s;
                }

                return $"{Symbols.VerticalEllipsis}null{Symbols.VerticalEllipsis}";
            } );

            return parts.ToStrings( separator: Symbols.TwoPipes ).Trim();
        }

        /// <summary>Build a key from combining 1 or more Objects.</summary>
        /// <param name="things"></param>
        [NotNull]
        [DebuggerStepThrough]
        public static String BuildKey( [NotNull] params Object[] things ) {
            if ( things is null ) {
                throw new ArgumentNullException( paramName: nameof( things ) ).Log( breakinto: true );
            }

            if ( !things.Any() ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( things ) );
            }

            var parts = things.Where( predicate: o => o != null ).Select( selector: o => {
                if ( o is IEnumerable<SqlParameter> collection ) {
                    var kvp = collection.Select( selector: parameter => new {
                        parameter.ParameterName, parameter.Value, parameter
                    } );

                    return $"{kvp.ToStrings( separator: Symbols.TwoPipes )}".Trim();
                }

                return o.ToString();
            } );

            return parts.ToStrings( separator: Symbols.TwoPipes ).Trim();
        }

    }

}