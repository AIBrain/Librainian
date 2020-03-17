// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "Enums.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "Enums.cs" was last formatted by Protiguous on 2020/03/16 at 2:55 PM.

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>Strongly typed version of Enum with Parsing and performance improvements.</summary>
    /// <typeparam name="T">Type of Enum</typeparam>
    /// <remarks>
    /// Copyright (c) Damien Guard. All rights reserved. Licensed under the Apache License, Version 2. 0 (the "License"); you may not use this file except in compliance with the
    /// License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 Originally published at http:
    /// //damieng.com/blog/2010/10/17/enums-better-syntax-improved-performance-and-tryparse-in-net-3-5
    /// </remarks>
    public static class Enums<T> where T : struct {

        private static IEnumerable<T> All { get; } = Enum.GetValues( typeof( T ) ).Cast<T>();

        private static Dictionary<String, T> InsensitiveNames { get; } = All.ToDictionary( k => Enum.GetName( typeof( T ), k )?.ToUpperInvariant() );

        private static Dictionary<T, String> Names { get; } = All.ToDictionary( k => k, v => v.ToString() );

        private static Dictionary<String, T> SensitiveNames { get; } = All.ToDictionary( k => Enum.GetName( typeof( T ), k ) );

        private static Dictionary<Int32, T> Values { get; } = All.ToDictionary( k => Convert.ToInt32( k ) );

        public static T? CastOrNull( Int32 value ) {
            if ( Values.TryGetValue( value, out var foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        /// <summary>Gets all items for an enum type.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [NotNull]
        public static IEnumerable<T> GetAllItems() => Enum.GetValues( typeof( T ) ).Cast<T>();

        [NotNull]
        public static IEnumerable<T> GetFlags( T flagEnum ) {
            var flagInt = Convert.ToInt32( flagEnum );

            return All.Where( value => ( Convert.ToInt32( value ) & flagInt ) != 0 );
        }

        [CanBeNull]
        public static String GetName( T value ) {
            Names.TryGetValue( value, out var name );

            return name;
        }

        [NotNull]
        public static String[] GetNames() => Names.Values.ToArray();

        [CanBeNull]
        public static IEnumerable<T> GetValues() => All;

        public static Boolean IsDefined( T value ) => Names.Keys.Contains( value );

        public static Boolean IsDefined( [CanBeNull] String? value ) => SensitiveNames.Keys.Contains( value );

        public static Boolean IsDefined( Int32 value ) => Values.Keys.Contains( value );

        public static T Parse( [NotNull] String value ) {
            if ( !SensitiveNames.TryGetValue( value, out var parsed ) ) {
                throw new ArgumentException( "Value is not one of the named constants defined for the enumeration", nameof( value ) );
            }

            return parsed;
        }

        public static T Parse( [NotNull] String value, Boolean ignoreCase ) {
            if ( !ignoreCase ) {
                return Parse( value );
            }

            if ( !InsensitiveNames.TryGetValue( value.ToUpperInvariant(), out var parsed ) ) {
                throw new ArgumentException( "Value is not one of the named constants defined for the enumeration", nameof( value ) );
            }

            return parsed;
        }

        public static T? ParseOrNull( [CanBeNull] String? value ) {
            if ( String.IsNullOrEmpty( value ) ) {
                return null;
            }

            if ( SensitiveNames.TryGetValue( value, out var foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static T? ParseOrNull( [CanBeNull] String? value, Boolean ignoreCase ) {
            if ( !ignoreCase ) {
                return ParseOrNull( value );
            }

            if ( String.IsNullOrEmpty( value ) ) {
                return null;
            }

            if ( InsensitiveNames.TryGetValue( value.ToUpperInvariant(), out var foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static T SetFlags( [NotNull] IEnumerable<T> flags ) {
            var combined = flags.Aggregate( default( Int32 ), ( current, flag ) => current | Convert.ToInt32( flag ) );

            return Values.TryGetValue( combined, out var result ) ? result : default;
        }

        public static Boolean TryParse( [NotNull] String value, out T returnValue ) => SensitiveNames.TryGetValue( value, out returnValue );

        public static Boolean TryParse( [NotNull] String value, Boolean ignoreCase, out T returnValue ) =>
            ignoreCase ? InsensitiveNames.TryGetValue( value.ToUpperInvariant(), out returnValue ) : TryParse( value, out returnValue );
    }
}