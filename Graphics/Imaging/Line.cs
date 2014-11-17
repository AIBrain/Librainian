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
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Line.cs" was last cleaned by Rick on 2014/11/16 at 3:45 PM

#endregion License & Information

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using Annotations;

    /// <summary>
    ///     A horizontal line of <see cref="Pixel" />.
    /// </summary>
    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public class Line {

        /// <summary>
        ///     Checksum of the pixels (to guard against corruption).
        /// </summary>
        /// <remarks>Should include the <see cref="Count" /> to prevent buffer overflows.</remarks>
        [DataMember]
        [FieldOffset( 0 )]
        public UInt64 Checksum;

        /// <summary>
        ///     How many pixels should be in this line?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) )]
        public UInt64 Count;

        /// <summary>
        ///     An array of pixels
        /// </summary>
        /// <remarks>I'd prefer a list instead of an array.</remarks>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 2 )]
        [CanBeNull]
        public Pixel[] Pixels;

        /// <summary>
        /// Returns the zero-based <see cref="Pixel"/> or null if not found.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel? this[ UInt64 index ] {
            get {
                var pixels = this.Pixels;
                if ( index <= this.Count && pixels != null ) {
                    return pixels[ index ];
                }
                return null;
            }
            set {
                var pixels = this.Pixels;
                if ( value.HasValue && pixels != null && index <= Count ) {
                    pixels[ index ] = value.Value;
                }
            }
        }

    }
}