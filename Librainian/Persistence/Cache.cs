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
// Project: "Librainian", File: "Cache.cs" was last formatted by Protiguous on 2020/03/16 at 3:00 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;
    using Collections.Extensions;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Maths;
    using Microsoft.Data.SqlClient;
    using Parsing;

    /// <summary>
    ///     <para><see cref="Recall{T}" /> to retrieve and value from <see cref="Memory" />.</para>
    ///     <para><see cref="Remember{T}" /> to store any value into <see cref="Memory" />.</para>
    ///     <para><see cref="Forget{T}" /> to remove any value from <see cref="Memory" />.</para>
    /// </summary>
    public static class Cache {

        /// <summary>Gets a reference to the default <see cref="MemoryCache" /> instance.</summary>
        /// <remarks></remarks>
        [NotNull]
        private static MemoryCache Memory { get; } = MemoryCache.Default;

        static Cache() => $"{Memory.CacheMemoryLimit.SizeSuffix()} memory available for caching.".Log();

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

            var parts = things.Where( predicate: o => !( o is null ) ).Select( selector: o => {
                if ( o is IEnumerable<SqlParameter> parameters ) {
                    var kvp = parameters.Where( predicate: parameter => parameter != default ).Select( selector: parameter => new {
                        parameter.ParameterName,
                        parameter.Value
                    } );

                    return $"{kvp.ToStrings( separator: Symbols.TwoPipes )}".Trim();
                }

                var s = Convert.ToString( value: o ).Trim().NullIf( right: String.Empty );

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
                        parameter.ParameterName,
                        parameter.Value,
                        parameter
                    } );

                    return $"{kvp.ToStrings( separator: Symbols.TwoPipes )}".Trim();
                }

                return o.ToString();
            } );

            return parts.ToStrings( separator: Symbols.TwoPipes ).Trim();
        }

        /// <summary>Remove <paramref name="key" /> from the cache.</summary>
        /// <param name="key"></param>
        public static void Forget( [NotNull] String key ) {
            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( key ) );
            }

            Memory.Remove( key: key );
        }

        /// <summary>Remove <paramref name="keys" /> from the cache.</summary>
        /// <param name="keys"></param>
        public static void Forget( [NotNull] params Object[] keys ) {
            if ( keys is null ) {
                throw new ArgumentNullException( paramName: nameof( keys ) );
            }

            if ( !keys.Any() ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( keys ) );
            }

            var key = BuildKey( things: keys );

            Memory.Remove( key: key );
        }

        /// <summary>Remove <paramref name="keyBuilder" /> from the cache.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyBuilder"></param>
        public static void Forget<T>( [NotNull] params T[] keyBuilder ) {
            if ( keyBuilder is null ) {
                throw new ArgumentNullException( paramName: nameof( keyBuilder ) );
            }

            if ( !keyBuilder.Any() ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( keyBuilder ) );
            }

            Memory.Remove( key: BuildKey( things: keyBuilder ) );
        }

        /// <summary>Attempt to pull <paramref name="key" /> from cache.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public static Object Recall( [NotNull] String key ) {
            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( key ) );
            }

            return Memory[ key: key ];
        }

        [CanBeNull]
        public static Object Recall( [NotNull] params Object[] keys ) => Recall( key: BuildKey( things: keys ) );

        [CanBeNull]
        public static Object Recall<T>( [NotNull] params T[] keys ) => Recall( key: BuildKey( things: keys ) );

        /// <summary>
        ///     <para>If <paramref name="policy" /> is not given, it will default to <see cref="Sliding.Minutes" /> (1 minute).</para>
        ///     <para>If <paramref name="value" /> is null, the cache for <paramref name="key" /> is released.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">   A unique string, or built using <see cref="BuildKey{T}" />.</param>
        /// <param name="value"> </param>
        /// <param name="policy"></param>
        [CanBeNull]
        public static T Remember<T>( [NotNull] String key, [CanBeNull] T value, [CanBeNull] CacheItemPolicy policy = null ) {
            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( key ) );
            }

            if ( value is null ) {
                Forget( key: key );

                return default;
            }

            Memory.Set( key: key, value: value, policy: policy ?? Sliding.Minutes( minutes: 1 ) );

            return value;
        }

        /// <summary>These expire at a given time from Now.</summary>
        public static class Absolute {

            /// <summary><paramref name="hours" /> from now.</summary>
            [NotNull]
            public static CacheItemPolicy Hours( Double hours ) =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddHours( value: hours )
                };

            /// <summary><paramref name="milliseconds" /> from now.</summary>
            /// <param name="milliseconds"></param>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Milliseconds( Double milliseconds ) =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds( value: milliseconds )
                };

            /// <summary><paramref name="minutes" /> from now.</summary>
            /// <param name="minutes"></param>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Minutes( Double minutes ) =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( value: minutes )
                };

            /// <summary><paramref name="seconds" /> from now.</summary>
            /// <param name="seconds"></param>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Seconds( Double seconds ) =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( value: seconds )
                };
        }

        /// <summary>A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.</summary>
        public static class Sliding {

            /// <summary><paramref name="hours" /> from now.</summary>
            [NotNull]
            public static CacheItemPolicy Hours( Double hours ) =>
                new CacheItemPolicy {
                    SlidingExpiration = TimeSpan.FromHours( value: hours )
                };

            /// <summary><paramref name="milliseconds" /> from now.</summary>
            /// <param name="milliseconds"></param>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Milliseconds( Double milliseconds ) =>
                new CacheItemPolicy {
                    SlidingExpiration = TimeSpan.FromMilliseconds( value: milliseconds )
                };

            /// <summary><paramref name="minutes" /> from now.</summary>
            /// <param name="minutes"></param>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Minutes( Double minutes ) =>
                new CacheItemPolicy {
                    SlidingExpiration = TimeSpan.FromMinutes( value: minutes )
                };

            /// <summary><paramref name="seconds" /> from now.</summary>
            /// <param name="seconds"></param>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Seconds( Double seconds ) =>
                new CacheItemPolicy {
                    SlidingExpiration = TimeSpan.FromSeconds( value: seconds )
                };
        }
    }
}