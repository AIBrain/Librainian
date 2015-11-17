// Copyright 2015 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian/Pixelyx.cs" was last cleaned by Rick on 2015/06/12 at 2:55 PM

namespace Librainian.Graphics.Moving {

    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using Maths;

    /// <summary>
    /// <para>
    /// A pixel (14 bytes?) with <see cref="Red" />, <see cref="Green" />, and <see cref="Blue" />
    /// values, and X/Y coordinates.
    /// </para>
    /// <para>
    /// At one screencap of 1920*1080, that's about ~24883200 (23MB) bytes of data for just one frame.
    /// </para>
    /// <para>At 60 fps, that is ~1492992000 bytes of data (1423MB/s)</para>
    /// </summary>
    [DataContract]
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public class Pixelyx : IEquatable<Pixelyx> {

        [DataMember]
        public readonly Int32 Checksum;

        [DataMember]
        public readonly UInt64 Timestamp;

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

        private Pixelyx(Byte alpha, Byte red, Byte green, Byte blue, UInt16 x, UInt16 y, UInt64 timestamp) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.X = x;
            this.Y = y;
            this.Timestamp = timestamp;
            this.Checksum = this.Green.GetHashMerge( this.Blue.GetHashMerge( this.Red.GetHashMerge( this.Alpha.GetHashMerge( this.Timestamp.GetHashMerge( this.X.GetHashMerge( this.Y ) ) ) ) ) );
        }

        //public static explicit operator Pixelyx( Color pixel ) {
        //    return new Pixelyx( pixel.A, pixel.R, pixel.G, pixel.B, 0, 0, 0 );
        //}

        //public static implicit operator Color( Pixelyx pixel ) {
        //    return Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );
        //}

        //public static explicit operator UInt16[]( Pixelyx pixel ) {
        //    return new[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue, pixel.X, pixel.Y };
        //}

        /// <summary>
        /// <para>Static comparison type.</para>
        /// <para>Compares: Alpha, Red, Green, Blue, X, and Y</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal(Pixelyx left, Pixelyx right) => ( left.Alpha == right.Alpha ) && ( left.Red == right.Red ) && ( left.Green == right.Green ) && ( left.Blue == right.Blue ) && ( left.X == right.X ) && ( left.Y == right.Y );

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals(Pixelyx other) => Equal( this, other );

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override Int32 GetHashCode() => this.Checksum;
    }
}