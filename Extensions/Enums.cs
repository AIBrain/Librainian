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
// "Librainian/Enums.cs" was last cleaned by Rick on 2015/06/12 at 2:53 PM

namespace Librainian.Extensions {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Strongly typed version of Enum with Parsing and performance improvements.</summary>
    /// <typeparam name="T">Type of Enum</typeparam>
    /// <remarks>
    /// Copyright (c) Damien Guard. All rights reserved. Licensed under the Apache License, Version
    /// 2. 0 (the "License"); you may not use this file except in compliance with the License. You
    ///    may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 Originally
    ///    published at
    /// http: //damieng.com/blog/2010/10/17/enums-better-syntax-improved-performance-and-tryparse-in-net-3-5
    /// </remarks>
    public static class Enums<T> where T : struct {
        private static readonly IEnumerable<T> All = Enum.GetValues( typeof( T ) ).Cast<T>();
        private static readonly Dictionary<String, T> InsensitiveNames = All.ToDictionary( k => Enum.GetName( typeof( T ), k ).ToUpperInvariant() );
        private static readonly Dictionary<T, String> Names = All.ToDictionary( k => k, v => v.ToString() );
        private static readonly Dictionary<String, T> SensitiveNames = All.ToDictionary( k => Enum.GetName( typeof( T ), k ) );
        private static readonly Dictionary<Int32, T> Values = All.ToDictionary( k => Convert.ToInt32( k ) );

        public static T? CastOrNull(Int32 value) {
            T foundValue;
            if ( Values.TryGetValue( value, out foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        /// <summary>Gets all items for an enum type.</summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems() => Enum.GetValues( typeof( T ) ).Cast<T>();

        public static IEnumerable<T> GetFlags(T flagEnum) {
            var flagInt = Convert.ToInt32( flagEnum );
            return All.Where( value => ( Convert.ToInt32( value ) & flagInt ) != 0 );
        }

        public static String GetName(T value) {
            String name;
            Names.TryGetValue( value, out name );
            return name;
        }

        public static String[] GetNames() => Names.Values.ToArray();

        public static IEnumerable<T> GetValues() => All;

        public static Boolean IsDefined(T value) => Names.Keys.Contains( value );

        public static Boolean IsDefined(String value) => SensitiveNames.Keys.Contains( value );

        public static Boolean IsDefined(Int32 value) => Values.Keys.Contains( value );

        public static T Parse(String value) {
            T parsed;
            if ( !SensitiveNames.TryGetValue( value, out parsed ) ) {
                throw new ArgumentException( "Value is not one of the named constants defined for the enumeration", nameof( value ) );
            }
            return parsed;
        }

        public static T Parse(String value, Boolean ignoreCase) {
            if ( !ignoreCase ) {
                return Parse( value );
            }

            T parsed;
            if ( !InsensitiveNames.TryGetValue( value.ToUpperInvariant(), out parsed ) ) {
                throw new ArgumentException( "Value is not one of the named constants defined for the enumeration", nameof( value ) );
            }
            return parsed;
        }

        public static T? ParseOrNull(String value) {
            if ( String.IsNullOrEmpty( value ) ) {
                return null;
            }

            T foundValue;
            if ( SensitiveNames.TryGetValue( value, out foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static T? ParseOrNull(String value, Boolean ignoreCase) {
            if ( !ignoreCase ) {
                return ParseOrNull( value );
            }

            if ( String.IsNullOrEmpty( value ) ) {
                return null;
            }

            T foundValue;
            if ( InsensitiveNames.TryGetValue( value.ToUpperInvariant(), out foundValue ) ) {
                return foundValue;
            }

            return null;
        }

        public static T SetFlags(IEnumerable<T> flags) {
            var combined = flags.Aggregate( default(Int32), (current, flag) => current | Convert.ToInt32( flag ) );

            T result;
            return Values.TryGetValue( combined, out result ) ? result : default(T);
        }

        public static Boolean TryParse(String value, out T returnValue) => SensitiveNames.TryGetValue( value, out returnValue );

        public static Boolean TryParse(String value, Boolean ignoreCase, out T returnValue) => ignoreCase ? InsensitiveNames.TryGetValue( value.ToUpperInvariant(), out returnValue ) : TryParse( value, out returnValue );



    }
}