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
// Project: "CommonLibrary", "Cache.cs" was last formatted by Protiguous on 2019/02/01 at 2:45 PM.

namespace Librainian.Persistence {

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Caching;
    using Collections.Extensions;
    using Collections.Sets;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Maths;

    /// <summary>
    ///     <para><see cref="Recall{T}" /> to retrieve and value from <see cref="Memory" />.</para>
    ///     <para><see cref="Remember{T}" /> to store any value into <see cref="Memory" />.</para>
    ///     <para><see cref="Forget{T}" /> to remove any value from <see cref="Memory" />.</para>
    /// </summary>
    public static class Cache {

        /// <summary>
        ///     The string to place between each key part.
        /// </summary>
        private const String Separator = "⦀";

        /// <summary>
        ///     Gets a reference to the default <see cref="T:System.Runtime.Caching.MemoryCache" /> instance.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        private static MemoryCache Memory { get; } = MemoryCache.Default;

        static Cache() => $"{Memory.CacheMemoryLimit.SizeSuffix()} memory available for caching.".Log();

        /// <summary>
        ///     Build a key from combining 1 or more <see cref="T" /> (converted to Strings).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="things"></param>
        /// <returns></returns>
        [NotNull]
        [DebuggerStepThrough]
        public static String BuildKey<T>( [NotNull] params T[] things ) {
            if ( things == null ) {
                throw new ArgumentNullException( paramName: nameof( things ) );
            }

            if ( !things.Any() ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( things ) );
            }

            var parts = things.Where( o => o != null ).Select( o => {
                if ( o is IEnumerable<SqlParameter> collection ) {
                    var kvp = collection.Select( parameter => new {
                        parameter.ParameterName,
                        parameter.Value
                    } );

                    return $"{kvp.ToStrings( Separator )}".Trim();
                }

                return Convert.ToString( o ).Trim().NullIf( String.Empty ) ?? $"{Separator}null{Separator}";
            } );

            return parts.ToStrings( Separator ).Trim();
        }

        /// <summary>
        ///     Build a key from combining 1 or more Objects.
        /// </summary>
        /// <param name="things"></param>
        /// <returns></returns>
        [NotNull]
        [DebuggerStepThrough]
        public static String BuildKey( [NotNull] params Object[] things ) {
            if ( things == null ) {
                throw new ArgumentNullException( paramName: nameof( things ) ).Log();
            }

            if ( !things.Any() ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( things ) );
            }

            var parts = things.Where( o => o != null ).Select( o => {
                if ( o is ConcurrentHashset<SqlParameter> collection ) {
                    var kvp = collection.Select( parameter => new {
                        parameter.ParameterName,
                        parameter.Value
                    } );

                    return $"{kvp.ToStrings( Separator )}".Trim();
                }

                return o.ToString();
            } );

            return parts.ToStrings( Separator ).Trim();
        }

        public static void Forget( [NotNull] String key ) {
            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( key ) );
            }

            Memory.Remove( key );
        }

        public static void Forget( [NotNull] params Object[] keys ) {
            if ( keys == null ) {
                throw new ArgumentNullException( paramName: nameof( keys ) );
            }

            if ( keys.Length == 0 ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( keys ) );
            }

            var key = BuildKey( keys );

            Memory.Remove( key );
        }

        public static void Forget<T>( [NotNull] params T[] keys ) {
            if ( keys == null ) {
                throw new ArgumentNullException( paramName: nameof( keys ) );
            }

            if ( keys.Length == 0 ) {
                throw new ArgumentException( message: "Value cannot be an empty collection.", paramName: nameof( keys ) );
            }

            var key = BuildKey( keys );

            Memory.Remove( key );
        }

        public static Object Recall( [NotNull] String key ) {
            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( key ) );
            }

            return Memory[ key ];
        }

        public static Object Recall( [NotNull] params Object[] keys ) {
            var key = BuildKey( keys );

            return Recall( key );
        }

        public static Object Recall<T>( [NotNull] params T[] keys ) {
            var key = BuildKey( keys );

            return Recall( key );
        }

        /// <summary>
        ///     <para>If <paramref name="policy" /> is not given, it will default to <see cref="Sliding.OneMinute" /> (1 minute).</para>
        ///     <para>If <paramref name="value" /> is null, the cache for <paramref name="key" /> is released.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">A unique string, or built using <see cref="BuildKey{T}" />.</param>
        /// <param name="value"> </param>
        /// <param name="policy"></param>
        public static void Remember<T>( [NotNull] String key, [CanBeNull] T value, [CanBeNull] CacheItemPolicy policy = null ) {
            if ( String.IsNullOrEmpty( value: key ) ) {
                throw new ArgumentException( message: "Value cannot be null or empty.", paramName: nameof( key ) );
            }

            if ( value == null ) {
                Forget( key: key );

                return;
            }

            Memory.Set( key, value, policy ?? Sliding.OneMinute );
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
            public static CacheItemPolicy AlmostAMinute() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 50 )
                };

            /// <summary>
            ///     6pm local time.
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy BusinessHours() {
                var now = DateTime.Now;

                return new CacheItemPolicy {
                    AbsoluteExpiration = new DateTime( now.Year, now.Month, now.Day, hour: 18, 0, 0 )
                };
            }

            /// <summary>
            ///     5 seconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy FiveSeconds() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 5 )
                };

            /// <summary>
            ///     30 minutes
            /// </summary>
            [NotNull]
            public static CacheItemPolicy HalfHour() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 30 )
                };

            /// <summary>
            ///     60 minutes
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Hour() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 60 )
                };

            /// <summary>
            ///     60 seconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy Minute() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 60 )
                };

            /// <summary>
            ///     5 minutes
            /// </summary>
            /// <value></value>
            [NotNull]
            public static CacheItemPolicy Minutes() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 5 )
                };

            /// <summary>
            ///     100 milliseconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy OneHundredMilliseconds() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMilliseconds( 100 )
                };

            /// <summary>
            ///     1 second
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy OneSecond() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 1 )
                };

            /// <summary>
            ///     15 minutes
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy QuarterHour() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddMinutes( 15 )
                };

            /// <summary>
            ///     2 seconds
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy TwoSeconds() =>
                new CacheItemPolicy {
                    AbsoluteExpiration = DateTime.Now.AddSeconds( 2 )
                };

            /// <summary>
            ///     10pm local time.
            /// </summary>
            /// <returns></returns>
            [NotNull]
            public static CacheItemPolicy WorkingLate() {
                var now = DateTime.Now;

                return new CacheItemPolicy {
                    AbsoluteExpiration = new DateTime( now.Year, now.Month, now.Day, hour: 22, 0, 0 )
                };
            }
        }

        /// <summary>
        ///     A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.
        /// </summary>
        public static class Sliding {

            /// <summary>
            ///     30 seconds
            /// </summary>
            public static CacheItemPolicy FiveSeconds { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromSeconds( 5 )
            };

            /// <summary>
            ///     4 hours.
            /// </summary>
            public static CacheItemPolicy FourHours { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromHours( 4 )
            };

            /// <summary>
            ///     30 minutes
            /// </summary>
            public static CacheItemPolicy HalfHour { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromHours( 0.5 )
            };

            /// <summary>
            ///     500 milliseconds
            /// </summary>
            public static CacheItemPolicy HalfSecond { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromSeconds( 0.5 )
            };

            /// <summary>
            ///     60 minutes.
            /// </summary>
            public static CacheItemPolicy Hour { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromHours( 1 )
            };

            /// <summary>
            ///     1 minute
            /// </summary>
            public static CacheItemPolicy OneMinute { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromMinutes( 1 )
            };

            /// <summary>
            ///     1 second
            /// </summary>
            public static CacheItemPolicy OneSecond { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromSeconds( 1 )
            };

            /// <summary>
            ///     15 minutes
            /// </summary>
            public static CacheItemPolicy QuarterHour { get; } = new CacheItemPolicy {
                SlidingExpiration = TimeSpan.FromMinutes( 15 )
            };
        }
    }
}