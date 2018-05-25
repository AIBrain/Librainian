// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "EFV.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
//
// =========================================================
// Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// "Librainian/Librainian/EFV.cs" was last formatted by Protiguous on 2018/05/24 at 7:13 PM.

namespace Librainian.Graphics.Moving {

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;

    /// <summary> Experimental Full Video </summary>
    /// <remarks>
    ///     Just for fun & learning. Prefer
    ///     compression over [decoding/display] speed (assuming local cpu will be 'faster' than network
    ///     transfer speed). Compressions must be lossless. Allow 'pages' of animation, each with their
    ///     own delay. Default should be page 0 = 0 delay. Checksums are used on each
    ///     <see cref="Pixelyx" /> to guard against (detect but not fix) corruption.
    /// </remarks>
    [JsonObject]
    public class Efv {

        public static readonly String Extension = ".efv";

        /// <summary>Human readable file header.</summary>
        public static readonly String Header = "EFV0.1";

        /// <summary>For each item here, draw them too.</summary>
        /// <remarks>I need to stop coding while I'm asleep.</remarks>
        [JsonProperty]
        public ConcurrentDictionary<UInt64, List<UInt64>> Dopples = new ConcurrentDictionary<UInt64, List<UInt64>>();

        [JsonProperty]
        public ConcurrentDictionary<UInt64, Pixelyx> Pixels = new ConcurrentDictionary<UInt64, Pixelyx>();

        /// <summary>Checksum guard</summary>
        [JsonProperty]
        public UInt64 Checksum { get; set; }

        [JsonProperty]
        public UInt16 Height { get; set; }

        [JsonProperty]
        public UInt16 Width { get; set; }

        public Efv() => this.Checksum = UInt64.MaxValue;

        public Boolean Add( Pixelyx pixelyx ) {
            var rgbMatchesJustNotTimestamp = this.Pixels.Where( pair => Pixelyx.Equal( pair.Value, pixelyx ) );

            foreach ( var pair in rgbMatchesJustNotTimestamp ) {
                if ( null == this.Dopples[pixelyx.Timestamp] ) { this.Dopples[pixelyx.Timestamp] = new List<UInt64>(); }

                this.Dopples[pixelyx.Timestamp].Add( pair.Value.Timestamp );
            }

            this.Pixels[pixelyx.Timestamp] = pixelyx;

            return true;
        }

        public Int32 CalculateChecksum() {
            var sum = 0;

            foreach ( var pixelyx in this.Pixels ) {
                unchecked { sum += pixelyx.GetHashCode(); }
            }

            return this.Pixels.Count + sum;
        }

        public async Task<UInt64> CalculateChecksumAsync() =>
            await Task.Run( () => {
                unchecked { return ( UInt64 )Hashing.GetHashCodes( this.Pixels ); }
            } );

        [CanBeNull]
        public Pixelyx Get( UInt64 index ) => this.Pixels.TryGetValue( index, out var pixelyx ) ? pixelyx : null;

        [CanBeNull]
        public Pixelyx Get( UInt16 x, UInt16 y ) {
            if ( x == 0 ) { throw new ArgumentException( "x" ); }

            if ( y == 0 ) { throw new ArgumentException( "y" ); }

            var index = ( UInt64 )( this.Height * y + x );

            return this.Get( index );
        }
    }
}