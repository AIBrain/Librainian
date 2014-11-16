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
// "Librainian/Class2.cs" was last cleaned by Rick on 2014/11/16 at 2:43 PM
#endregion

namespace Librainian.Graphics {
    using System;
    using System.Collections.Concurrent;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public struct Pixel {

        [DataMember]
        [FieldOffset( 0 )]
        public readonly Byte Alpha;

        [DataMember]
        [FieldOffset( 1 )]
        public readonly Byte Red;

        [DataMember]
        [FieldOffset( 2 )]
        public readonly Byte Green;

        [DataMember]
        [FieldOffset( 3 )]
        public readonly Byte Blue;

        private Pixel( Byte alpha, Byte red, Byte green, Byte blue ) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public static explicit operator Pixel( Color pixel ) {
            return new Pixel( pixel.A, pixel.R, pixel.G, pixel.B );
        }

        public static implicit operator Color( Pixel pixel ) {
            return Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );
        }
    }

    /// <summary>
    /// A horizontal line of <see cref="Pixel"/>.
    /// </summary>
    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public class LineOfPixels {

        /// <summary>
        /// Checksum of the pixels (guard against corruption).
        /// </summary>
        [DataMember]
        [FieldOffset( 0 )]
        public UInt64 Checksum;

        /// <summary>
        /// How many pixels should be in this line?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) )]
        public UInt64 Count;

        /// <summary>
        /// List of pixels
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 2 )]
        public Pixel[] Pixels;
    }

    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public class PageOfLines {

        /// <summary>
        /// Checksum of the page (guard against corruption).
        /// </summary>
        [DataMember]
        [FieldOffset( 0 )]
        public UInt64 Checksum;

        /// <summary>
        /// How many lines should be in this page?
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) )]
        public UInt64 Count;

        /// <summary>
        /// List of pixels
        /// </summary>
        [DataMember]
        [FieldOffset( sizeof( UInt64 ) * 2 )]
        public LineOfPixels[] Lines;
    }

    /// <summary>
    ///     Experimental and Fun Graphic
    /// </summary>
    /// <remarks>
    /// Prefer compression over speed (assuming local cpu will be 'faster' than network transfer speed).
    /// Compressions must be lossless.
    /// Allow 'pages' of animation, each with their own delay. Default should be page 0 = 0 delay.
    /// Checksums are used on each line of each page to guard against (detect but not fix) corruption.
    /// </remarks>
    [DataContract]
    [Serializable]
    public class EFG {

        /// <summary>
        /// Human readable file header.
        /// </summary>
        public static readonly String Header = "EFG1";
        public static readonly String Extension = ".efg";

        /// <summary>
        /// only here for reference
        /// </summary>
        [Obsolete]
        private Bitmap Bitmap {
            get;
            set;
        }

        /// <summary>
        /// Checksum of all pages
        /// </summary>
        [DataMember]
        public UInt64 Checksum {
            get;
            set;
        }

        /// <summary>
        /// EXIF metadatas
        /// </summary>
        [DataMember( IsRequired = true )]
        public ConcurrentDictionary<String, String> Exifs {
            get;
            set;
        }

        public EFG() {
            this.Checksum = 0;
        }

    }
}
