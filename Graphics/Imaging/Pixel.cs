#region License & Information

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
// bitcoin: 1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "Librainian 2015/Pixel.cs" was last cleaned by Rick on 2015/01/08 at 9:29 PM
#endregion License & Information

namespace Librainian.Graphics.Imaging {
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;

    /// <summary> <para> A simple pixel with <see cref="Alpha" />, <see cref="Red" />, <see
    /// cref="Green" /> , <see cref="Blue" />, and <see cref="X" /> & <see cref="Y" /> values.
    /// </para> </summary>
    [Immutable]
    [DataContract]
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel : IEquatable<Pixel> {
        [DataMember]
        [FieldOffset(0)]
        public readonly Int64 Checksum;

        [DataMember]
        [FieldOffset(sizeof(Int64) + ( 0 * sizeof(Byte) ))]
        public readonly Byte Alpha;

        [DataMember]
        [FieldOffset(sizeof(Int64) + ( 1 * sizeof(Byte) ))]
        public readonly Byte Red;

        [DataMember]
        [FieldOffset(sizeof(Int64) + ( 2 * sizeof(Byte) ))]
        public readonly Byte Green;

        [DataMember]
        [FieldOffset(sizeof(Int64) + ( 3 * sizeof(Byte) ))]
        public readonly Byte Blue;

        [DataMember]
        [FieldOffset(sizeof(Int64) + ( 4 * sizeof(Byte) ))]
        public readonly UInt64 X;

        [DataMember]
        [FieldOffset(sizeof(Int64) + sizeof(UInt64) + ( 4 * sizeof(UInt64) ))]
        public readonly UInt64 Y;

        public Pixel( Byte alpha, Byte red, Byte green, Byte blue, UInt64 x, UInt64 y ) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.X = x;
            this.Y = y;
            this.Checksum = MathExtensions.GetBigHash( this.Alpha, Red, this.Green, this.Blue, this.X, this.Y );
        }

        //public static explicit operator Pixel( Color pixel ) => new Pixel( pixel.A, pixel.R, pixel.G, pixel.B );

        public static implicit operator Color( Pixel pixel ) => Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );

        public static explicit operator Byte[] ( Pixel pixel ) => new[ ] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue };

        /// <summary>
        /// Static comparison type.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal( Pixel left, Pixel right ) => left.Alpha == right.Alpha && left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.X == right.X && left.Y == right.Y;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals( Pixel other ) => Equal( this, other );

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode() {
            unchecked {
                return ( int )this.Checksum;
            }

        }

        public Task WriteToStreamAsync( [NotNull] StreamWriter streamWriter ) {
            if ( streamWriter == null ) {
                throw new ArgumentNullException( "streamWriter" );
            }
            String message = String.Format( "{0}({1},{2},{3},{4}),{5},{6}", this.Checksum, this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );
            return streamWriter.WriteLineAsync( message );
        }
    }
}