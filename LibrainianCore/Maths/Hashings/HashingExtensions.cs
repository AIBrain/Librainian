﻿// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "HashingExtensions.cs" belongs to Protiguous@Protiguous.com and
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
// Project: "Librainian", "HashingExtensions.cs" was last formatted by Protiguous on 2019/08/08 at 8:19 AM.

namespace LibrainianCore.Maths.Hashings {

    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using Converters;
    using OperatingSystem.FileSystem;

    public static class HashingExtensions {

        [ThreadStatic]
        private static Random RandomInstance;

        /// <summary>
        ///     Returns argument increased to the nearest number divisible by 16
        /// </summary>
        public static Int32 Align16( this Int32 i ) {
            var r = i & 15; // 00001111

            return r == 0 ? i : i + ( 16 - r );
        }

        /// <summary>
        ///     Returns argument increased to the nearest number divisible by 16
        /// </summary>
        public static Int64 Align16( this Int64 i ) {
            var r = i & 15; // 00001111

            return r == 0 ? i : i + ( 16 - r );
        }

        /// <summary>
        ///     Returns argument increased to the nearest number divisible by 8
        /// </summary>
        public static Int32 Align8( this Int32 i ) {
            var r = i & 7; // 00000111

            return r == 0 ? i : i + ( 8 - r );
        }

        /// <summary>
        ///     Returns argument increased to the nearest number divisible by 8
        /// </summary>
        public static Int64 Align8( this Int64 i ) {
            var r = i & 7; // 00000111

            return r == 0 ? i : i + ( 8 - r );
        }

        /// <summary>
        ///     poor mans crc
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static Int32 CalcHashCode( [NotNull] this FileInfo fileInfo ) {
            if ( fileInfo is null ) {
                throw new ArgumentNullException( nameof( fileInfo ) );
            }

            return fileInfo.AsBytes().Aggregate( 0, ( current, b ) => current.GetHashMerge( b ) );
        }

        /// <summary>
        ///     poor mans crc
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Int32 CalcHashCode( [NotNull] this Document document ) {
            if ( document is null ) {
                throw new ArgumentNullException( nameof( document ) );
            }

            var fileInfo = new FileInfo( document.FullPath );

            if ( fileInfo is null ) {
                throw new NullReferenceException( "fileInfo" );
            }

            return fileInfo.AsBytes().Aggregate( 0, ( current, b ) => current.GetHashMerge( b ) );
        }

        /// <summary>
        ///     Takes one <see cref="UInt64" />, and returns another Deterministic <see cref="UInt64" />.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static UInt64 Deterministic( this UInt64 index ) {
            var translate64 = new Translate64 {
                UnsignedValue = index
            };

            var bufferA = new Byte[ sizeof( Int32 ) ];
            new Random( translate64.SignedLow ).NextBytes( bufferA );

            var bufferB = new Byte[ sizeof( Int32 ) ];
            new Random( translate64.SignedHigh ).NextBytes( bufferB );

            translate64.SignedLow = Convert.ToInt32( bufferA );
            translate64.SignedHigh = Convert.ToInt32( bufferB );

            return translate64.UnsignedValue;
        }

        /// <summary>
        ///     Takes one <see cref="Int32" />, and returns another Deterministic <see cref="Int32" /> by using the
        ///     <see cref="Random" /> generator.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>A faster Randem.Reseed would be nice here.</remarks>
        public static Int32 Deterministic( this Int32 index ) {
            if ( RandomInstance is null ) {
                RandomInstance = new Random( index );
            }

            return RandomInstance.Next();
        }

        /// <summary>
        ///     Takes one <see cref="Int64" />, and returns another Deterministic <see cref="Int64" />.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Int64 Deterministic( this Int64 index ) {
            var translate64 = new Translate64 {
                SignedValue = index
            };

            var bufferA = new Byte[ sizeof( Int32 ) ];
            new Random( translate64.SignedLow ).NextBytes( bufferA );

            var bufferB = new Byte[ sizeof( Int32 ) ];
            new Random( translate64.SignedHigh ).NextBytes( bufferB );

            translate64.SignedLow = Convert.ToInt32( bufferA );
            translate64.SignedHigh = Convert.ToInt32( bufferB );

            return translate64.SignedValue;
        }

        /*
        public static Byte GetHashCodeByte<TLeft>( [NotNull] this TLeft objectA, Byte maximum = Byte.MaxValue ) {
            if ( Equals( objectA, default ) ) {
                return 0;
            }

            unchecked {
                var hashA = ( Byte )objectA.GetHashCode();

                return ( Byte )( ( ( ( hashA << 5 ) + hashA ) ^ hashA ) % maximum );
            }
        }
        */

        /// <summary>
        ///     Returns the combined <see cref="Object.GetHashCode" /> of all <paramref name="objects" />.
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        [Pure]
        public static Int32 GetHashCodes<T>( [CanBeNull] params T[] objects ) {
            if ( objects is null ) {
                return 0;
            }

            var hash = new HashCode();

            foreach ( var o in objects ) {
                hash.Add( o );
            }

            return hash.ToHashCode();
        }

        /// <summary>
        ///     Returns the combined <see cref="Object.GetHashCode" /> of all <paramref name="objects" />.
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        [Pure]
        public static Int32 GetHashCodes( [CanBeNull] params Object[] objects ) {
            if ( objects is null ) {
                return 0;
            }

            var hash = new HashCode();

            foreach ( var o in objects ) {
                hash.Add( o );
            }

            return hash.ToHashCode();
        }

        /// <summary>
        ///     Returns a combined <see cref="Object.GetHashCode" /> based on
        ///     <paramref name="objectA" /> and <paramref name="objectB" />.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <returns></returns>
        public static Int32 GetHashCodes<TLeft, TRight>( [CanBeNull] this TLeft objectA, [CanBeNull] TRight objectB ) {
            var hash = new HashCode();
            hash.Add( objectA );
            hash.Add( objectB );

            return hash.ToHashCode();
        }

        public static UInt16 GetHashCodeUInt16<TLeft>( [NotNull] this TLeft objectA ) {
            var hash = new HashCode();
            hash.Add( objectA );

            return ( UInt16 ) hash.ToHashCode();
        }

        public static UInt32 GetHashCodeUInt32<TLeft>( [NotNull] this TLeft objectA ) {
            var hash = new HashCode();
            hash.Add( objectA );

            return ( UInt32 ) hash.ToHashCode();
        }

        public static UInt64 GetHashCodeUInt64<TLeft>( [NotNull] this TLeft objectA ) {
            var hash = new HashCode();
            hash.Add( objectA );

            return ( UInt64 ) hash.ToHashCode();

        }

        /// <summary>
        ///     Returns a combined <see cref="Object.GetHashCode" /> based on
        ///     <paramref name="objectA" /> and <paramref name="objectB" />.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <returns></returns>
        [Pure]
        public static Int32 GetHashMerge<TLeft, TRight>( [CanBeNull] this TLeft objectA, [CanBeNull] TRight objectB ) {
            var hash = new HashCode();
            hash.Add( objectA );
            hash.Add( objectB );

            return hash.ToHashCode();
        }

    }

}