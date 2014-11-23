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
// "Librainian/Pixel.cs" was last cleaned by Rick on 2014/11/16 at 3:43 PM

#endregion License & Information

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using Maths;

    /// <summary>
    ///     A simple pixel with <see cref="Red" />, <see cref="Green" />, and <see cref="Blue" /> values.
    /// </summary>
    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Explicit )]
    public struct Pixel : IEquatable<Pixel> {

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

        public static explicit operator Byte[]( Pixel pixel ) {
            return new[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue };
        }

        /// <summary>
        /// Static comparison type.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal( Pixel left, Pixel right ) {
            return left.Alpha == right.Alpha
                && left.Red == right.Red
                && left.Green == right.Green
                && left.Blue == right.Blue;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals( Pixel other ) {
            return Equal( this, other );
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode() {
            return this.Green.GetHashMerge( this.Blue.GetHashMerge( this.Red.GetHashMerge( this.Alpha ) ) );
        }
    }
}