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
        public readonly Byte Checksum;

        [DataMember]
        [FieldOffset(sizeof(Byte) + ( 0 * sizeof(Byte) ))]
        public readonly Byte Alpha;

        [DataMember]
        [FieldOffset(sizeof(Byte) + ( 1 * sizeof(Byte) ))]
        public readonly Byte Red;

        [DataMember]
        [FieldOffset(sizeof(Byte) + ( 2 * sizeof(Byte) ))]
        public readonly Byte Green;

        [DataMember]
        [FieldOffset(sizeof(Byte) + ( 3 * sizeof(Byte) ))]
        public readonly Byte Blue;

        [DataMember]
        [FieldOffset(sizeof(Byte) + ( 4 * sizeof(Byte) ))]
        public readonly UInt32 X;

        [DataMember]
        [FieldOffset(sizeof(Byte) + ( 4 * sizeof(Byte) + sizeof(UInt32) ))]
        public readonly UInt32 Y;

        public Pixel( Byte alpha, Byte red, Byte green, Byte blue, UInt32 x, UInt32 y ) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.X = x;
            this.Y = y;
            this.Checksum = ( Byte )MathExtensions.GetHashCodes( this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );
        }

        public Pixel( Color color, UInt32 x, UInt32 y ) {
            this.Alpha = color.A;
            this.Red = color.R;
            this.Green = color.G;
            this.Blue = color.B;
            this.X = x;
            this.Y = y;
            this.Checksum = ( Byte )MathExtensions.GetHashCodes( this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );
        }

        //public static explicit operator Pixel( Color pixel ) => new Pixel( pixel.A, pixel.R, pixel.G, pixel.B );

        public static implicit operator Color( Pixel pixel ) => Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );

        public static explicit operator Byte[] ( Pixel pixel ) => new[ ] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue };

        /// <summary>
        /// Static comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal( Pixel left, Pixel right ) =>
            left.Alpha == right.Alpha && left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.X == right.X && left.Y == right.Y;

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
        [Pure]
        public override int GetHashCode() {
            unchecked {
                return this.Checksum;
            }
        }

        public override String ToString() => String.Format( "{0}({1},{2},{3},{4})@{5},{6}", this.Checksum, this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );

        public Task WriteToStreamAsync( [NotNull] StreamWriter streamWriter ) {
            if ( streamWriter == null ) {
                throw new ArgumentNullException( nameof( streamWriter ) );
            }
            return streamWriter.WriteLineAsync( this.ToString() );
        }

        [CanBeNull]
        public static async Task<Pixel?> ReadFromStreamAsync( [NotNull] StreamReader reader, [NotNull] StreamWriter errors ) {
            if ( reader == null ) {
                throw new ArgumentNullException( nameof( reader ) );
            }
            if ( errors == null ) {
                throw new ArgumentNullException( nameof( errors ) );
            }

            var line = await reader.ReadLineAsync() ?? String.Empty;
            line = line.Trim();

            if ( String.IsNullOrWhiteSpace( line ) ) {
                await errors.WriteLineAsync( "Blank input line" );
                return null;
            }

            var openParent = line.IndexOf( "(", StringComparison.OrdinalIgnoreCase );
            if ( openParent <= -1 ) {
                await errors.WriteLineAsync( String.Format( "Unable to find a '(' in {0}", line ) );
                return null;
            }

            Byte checksum;
            if ( !Byte.TryParse( line.Substring( 0, openParent ), out checksum ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Checksum from {0}", line ) );
                return null;
            }

            var closeParent = line.IndexOf( ")", StringComparison.OrdinalIgnoreCase );
            if ( closeParent == -1 ) {
                await errors.WriteLineAsync( String.Format( "Unable to find a ')' in {0}", line ) );
                return null;
            }

            var argb = line.Substring( openParent + 1, closeParent - openParent ).Split( new[ ] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            if ( argb.Length != 4 ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Color from {0}", line ) );
                return null;
            }

            Byte alpha;
            if ( !Byte.TryParse( argb[ 0 ], out alpha ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Alpha from {0}", line ) );
                return null;
            }

            Byte red;
            if ( !Byte.TryParse( argb[ 1 ], out red ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Red from {0}", line ) );
                return null;
            }

            Byte green;
            if ( !Byte.TryParse( argb[ 2 ], out green ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Green from {0}", line ) );
                return null;
            }

            Byte blue;
            if ( !Byte.TryParse( argb[ 3 ], out blue ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Blue from {0}", line ) );
                return null;
            }

            var at = line.IndexOf( "@", StringComparison.OrdinalIgnoreCase );
            if ( at == -1 ) {
                await errors.WriteLineAsync( String.Format( "Unable to find an '@' in {0}", line ) );
                return null;
            }

            var xandy = line.Substring( at + 1 ).Split( new[ ] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            if ( xandy.Length != 2 ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse X & Y from {0}", line ) );
                return null;
            }


            UInt32 x;
            if ( !UInt32.TryParse( xandy[ 0 ], out x ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse X from {0}", line ) );
                return null;
            }

            UInt32 y;
            if ( !UInt32.TryParse( xandy[ 0 ], out y ) ) {
                await errors.WriteLineAsync( String.Format( "Unable to parse Y from {0}", line ) );
                return null;
            }

            var pixel = new Pixel( alpha, red, green, blue, x, y );
            if ( pixel.Checksum != checksum ) {
                await errors.WriteLineAsync( String.Format( "Warning checksums do not match! Expected {0}, but got {1}", checksum, pixel.Checksum ) );
            }

            return pixel;
        }
    }
}