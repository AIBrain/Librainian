﻿#region License & Information
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
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/EFV.cs" was last cleaned by Rick on 2014/12/05 at 12:08 AM
#endregion

namespace Librainian.Graphics.Moving {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Annotations;

    /// <summary>
    ///     Experimental Full Video
    /// </summary>
    /// <remarks>
    ///     Just for fun & learning.
    ///     Prefer compression over [decoding/display] speed (assuming local cpu will be 'faster' than network transfer speed).
    ///     Compressions must be lossless.
    ///     Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.
    ///     Checksums are used on each <see cref="Pixelyx" /> to guard against (detect but not fix) corruption.
    /// </remarks>
    [DataContract]
    [Serializable]
    public class EFV {
        public static readonly String Extension = ".efv"; //TODO

        /// <summary>
        ///     Human readable file header.
        /// </summary>
        public static readonly String Header = "EFV1"; //TODO

        [DataMember]
        public ConcurrentDictionary<UInt64, Pixelyx> Pixels = new ConcurrentDictionary<UInt64, Pixelyx>();

        /// <summary>
        /// For each item here, draw them too.
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<UInt64, List<UInt64>> Dopples = new ConcurrentDictionary<UInt64, List<UInt64>>();

        public EFV() {
            this.Checksum = UInt64.MaxValue; //an unlikely hash
        }

        public Int32 CalculateChecksum() {
            var sum = 0;
            foreach ( var pixelyx in Pixels ) {
                unchecked {
                    sum += pixelyx.GetHashCode();
                }
            }
            return Pixels.Count() + sum;
        }


        /// <summary>
        ///     Checksum guard
        /// </summary>
        [DataMember]
        public UInt64 Checksum {
            get;
            set;
        }

        [DataMember]
        public UInt16 Height {
            get;
            set;
        }

        [DataMember]
        public UInt16 Width {
            get;
            set;
        }

        [CanBeNull]
        public Pixelyx Get( UInt64 index ) {
            Pixelyx pixelyx;
            return this.Pixels.TryGetValue( index, out pixelyx ) ? pixelyx : null;
        }

        [CanBeNull]
        public Pixelyx Get( UInt16 x, UInt16 y ) {
            if ( x ==0  ) {
                throw new ArgumentException("x");
            }
            if (  y ==0 ) {
                throw new ArgumentException("y");
            }
            var index = ( UInt64 )( ( this.Height * y ) + x );
            return this.Get( index );
        }

        public Boolean Add( Pixelyx pixelyx ) {

            var rgbMatchesJustNotTimestamp = this.Pixels.Where( pair => Pixelyx.Equal( pair.Value, pixelyx ) );
            foreach ( var pair in rgbMatchesJustNotTimestamp ) {
                if ( null == this.Dopples[ pixelyx.Timestamp ] ) {
                    this.Dopples[ pixelyx.Timestamp ] = new List<ulong>();
                }
                this.Dopples[ pixelyx.Timestamp ].Add( pair.Value.Timestamp );
            }


            this.Pixels[ pixelyx.Timestamp ] = pixelyx;
            return true;
        }
    }
}