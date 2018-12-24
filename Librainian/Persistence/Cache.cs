// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Cache.cs" belongs to Protiguous@Protiguous.com and
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
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// Feel free to browse any source code we *might* make available.
// 
// Project: "Librainian", "Cache.cs" was last formatted by Protiguous on 2018/10/23 at 11:28 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;
    using Collections;
    using JetBrains.Annotations;
    using Logging;
    using Parsing;

    public static class Cache {

        /// <summary>
        ///     Gets a reference to the default <see cref="T:System.Runtime.Caching.MemoryCache" /> instance.
        /// </summary>
        [NotNull]
        public static MemoryCache Memory { get; } = MemoryCache.Default;

        /// <summary>
        ///     Build a unique string given the <paramref name="parts" />. (<see cref="ParsingExtensions.TriplePipes" /> between
        ///     each item)
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [DebuggerStepThrough]
        public static TrimmedString BuildKey<T>( [NotNull] params T[] parts ) {
            if ( parts == null ) {
                throw new ArgumentNullException( nameof( parts ) ).Log();
            }

            var list = new List<T>( parts.Length );
            list.AddRange( parts.Where( arg => arg != null ) );

            if ( !list.Any() ) {
                throw new InvalidOperationException( $"Error: No key-parts given in {nameof( Cache )}.{nameof( BuildKey )}." );
            }

            return new TrimmedString( list.ToStrings( ParsingExtensions.TriplePipes ) );
        }

        /// <summary>
        ///     These expire at a given time from Now.
        /// </summary>
        public static class Absolute {

            /// <summary>
            ///     50 seconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy AlmostAMinute =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 50 )
                };

            /// <summary>
            ///     Until 6pm local time if working hours (6am-6pm), otherwise 1 minute from now.
            /// </summary>
            /// <value></value>
            [NotNull]
            public static CacheItemPolicy BusinessHours {
                get {
                    var now = DateTime.Now;

                    if ( now.Hour >= 6 && now.Hour <= 18 ) {
                        return new CacheItemPolicy {
                            AbsoluteExpiration = new DateTime( now.Year, now.Month, now.Day, 18, 0, 0 )
                        };
                    }

                    return new CacheItemPolicy {
                        AbsoluteExpiration = DateTime.Now.AddMinutes( 1 )
                    };
                }
            }

            /// <summary>
            ///     5 seconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy FiveSeconds =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 5 )
                };

            /// <summary>
            ///     60 minutes
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Hour =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 60 )
                };

            /// <summary>
            ///     60 seconds
            /// </summary>
            /// <value></value>
            [NotNull]
            public static CacheItemPolicy Minute =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 60 )
                };

            /// <summary>
            ///     5 minutes
            /// </summary>
            /// <value></value>
            [NotNull]
            public static CacheItemPolicy Minutes =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 5 )
                };

            /// <summary>
            ///     100 milliseconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy OneHundredMilliseconds =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds( 100 )
                };

            /// <summary>
            ///     1 second
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy OneSecond =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 1 )
                };

            /// <summary>
            ///     15 minutes
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy QuarterHour =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 15 )
                };

            /// <summary>
            ///     2 seconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy TwoSeconds =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 2 )
                };

        }

        /// <summary>
        ///     A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.
        /// </summary>
        public static class Sliding {

            /// <summary>
            ///     60 minutes.
            /// </summary>
            public static CacheItemPolicy Hour { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromMinutes( 60 )
            };

            /// <summary>
            ///     4 hours.
            /// </summary>
            public static CacheItemPolicy LongLifeTime { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromHours( 4 )
            };

            /// <summary>
            ///     15 minutes
            /// </summary>
            public static CacheItemPolicy QuarterHour { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromMinutes( 15 )
            };

            /// <summary>
            ///     1 minute
            /// </summary>
            public static CacheItemPolicy ShortLifeTime { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromMinutes( 1 )
            };

            /// <summary>
            ///     30 seconds
            /// </summary>
            public static CacheItemPolicy VeryShortLifeTime { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromSeconds( 5 )
            };

        }

    }

}