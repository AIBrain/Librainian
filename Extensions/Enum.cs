#region License & Information
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
// 
// "Librainian2/Enum.cs" was last cleaned by Rick on 2014/08/08 at 2:26 PM
#endregion

namespace Librainian.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Strongly typed version of Enum with Parsing and performance improvements.
    /// </summary>
    /// <typeparam name="T">Type of Enum</typeparam>
    /// <remarks>
    ///     Copyright (c) Damien Guard.  All rights reserved.
    ///     Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with
    ///     the License.
    ///     You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
    ///     Originally published at
    ///     http://damieng.com/blog/2010/10/17/enums-better-syntax-improved-performance-and-tryparse-in-net-3-5
    /// </remarks>
    public static class Enum< T > where T : struct {
        private static readonly IEnumerable< T > all = Enum.GetValues( typeof ( T ) ).Cast< T >();

        private static readonly Dictionary< string, T > insensitiveNames = all.ToDictionary( k => Enum.GetName( typeof ( T ), k ).ToUpperInvariant() );

        private static readonly Dictionary< string, T > sensitiveNames = all.ToDictionary( k => Enum.GetName( typeof ( T ), k ) );
        private static readonly Dictionary< int, T > values = all.ToDictionary( k => Convert.ToInt32( k ) );
        private static readonly Dictionary< T, string > names = all.ToDictionary( k => k, v => v.ToString() );

        public static Boolean IsDefined( T value ) {
            return names.Keys.Contains( value );
        }

        public static Boolean IsDefined( string value ) {
            return sensitiveNames.Keys.Contains( value );
        }

        public static Boolean IsDefined( int value ) {
            return values.Keys.Contains( value );
        }

        public static IEnumerable< T > GetValues() {
            return all;
        }

        public static string[] GetNames() {
            return names.Values.ToArray();
        }

        public static string GetName( T value ) {
            string name;
            names.TryGetValue( value, out name );
            return name;
        }

        public static T Parse( string value ) {
            T parsed;
            if ( !sensitiveNames.TryGetValue( value, out parsed ) ) {
                throw new ArgumentException( "Value is not one of the named constants defined for the enumeration", "value" );
            }
            return parsed;
        }

        public static T Parse( string value, Boolean ignoreCase ) {
            if ( !ignoreCase ) {
                return Parse( value );
            }

            T parsed;
            if ( !insensitiveNames.TryGetValue( value.ToUpperInvariant(), out parsed ) ) {
                throw new ArgumentException( "Value is not one of the named constants defined for the enumeration", "value" );
            }
            return parsed;
        }

        public static Boolean TryParse( string value, out T returnValue ) {
            return sensitiveNames.TryGetValue( value, out returnValue );
        }

        public static Boolean TryParse( string value, Boolean ignoreCase, out T returnValue ) {
            return ignoreCase ? insensitiveNames.TryGetValue( value.ToUpperInvariant(), out returnValue ) : TryParse( value, out returnValue );
        }

        public static T? ParseOrNull( string value ) {
            if ( String.IsNullOrEmpty( value ) ) {
                return null;
            }

            T foundValue;
            if ( sensitiveNames.TryGetValue( value, out foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static T? ParseOrNull( string value, Boolean ignoreCase ) {
            if ( !ignoreCase ) {
                return ParseOrNull( value );
            }

            if ( String.IsNullOrEmpty( value ) ) {
                return null;
            }

            T foundValue;
            if ( insensitiveNames.TryGetValue( value.ToUpperInvariant(), out foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static T? CastOrNull( int value ) {
            T foundValue;
            if ( values.TryGetValue( value, out foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static IEnumerable< T > GetFlags( T flagEnum ) {
            var flagInt = Convert.ToInt32( flagEnum );
            return all.Where( e => ( Convert.ToInt32( e ) & flagInt ) != 0 );
        }

        public static T SetFlags( IEnumerable< T > flags ) {
            var combined = flags.Aggregate( default( int ), ( current, flag ) => current | Convert.ToInt32( flag ) );

            T result;
            return values.TryGetValue( combined, out result ) ? result : default( T );
        }
    }
}
