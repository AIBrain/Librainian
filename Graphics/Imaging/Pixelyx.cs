namespace Librainian.Graphics.Imaging {
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using Maths;

    /// <summary>
    /// <para>A pixel (12 bytes!) with <see cref="Red" />, <see cref="Green" />, and <see cref="Blue" /> values, and X/Y coordinates.</para>
    /// <para>At one screencap of 1920*1080, that's about ~24883200 (23MB) bytes of data for just one frame.</para>
    /// <para>At 60 fps, that is ~1492992000 bytes of data for motion (1423MB/s)</para>
    /// </summary>
    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct Pixelyx : IEquatable<Pixelyx> {

        [DataMember]
        public readonly UInt32 Checksum;

        [DataMember]
        public readonly UInt32 Timestamp;

        [DataMember]
        public readonly Byte Alpha;

        [DataMember]
        public readonly Byte Red;

        [DataMember]
        public readonly Byte Green;

        [DataMember]
        public readonly Byte Blue;

        [DataMember]
        public readonly UInt16 X;

        [DataMember]
        public readonly UInt16 Y;

        private Pixelyx( Byte alpha, Byte red, Byte green, Byte blue, UInt16 x, UInt16 y ) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.X = x;
            this.Y = y;
        }

        public static explicit operator Pixelyx( Color pixel ) {
            return new Pixelyx( pixel.A, pixel.R, pixel.G, pixel.B, 0, 0 );
        }

        public static implicit operator Color( Pixelyx pixel ) {
            return Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );
        }

        public static explicit operator UInt16[]( Pixelyx pixel ) {
            return new[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue, pixel.X, pixel.Y };
        }

        /// <summary>
        /// Static comparison type.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal( Pixelyx left, Pixelyx right ) {
            return left.Alpha == right.Alpha
                   && left.Red == right.Red
                   && left.Green == right.Green
                   && left.Blue == right.Blue
                   && left.X == right.X
                   && left.Y == right.Y
                   ;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals( Pixelyx other ) {
            return Equal( this, other );
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode() {
            return this.Green.GetHashMerge( this.Blue.GetHashMerge( this.Red.GetHashMerge( this.Alpha.GetHashMerge( this.GetHashMerge( this.X.GetHashMerge( this.Y ) ) ) ) ) );
        }
    }
}