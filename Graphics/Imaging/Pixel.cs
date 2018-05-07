// Copyright 2016 Protiguous.
//
// This notice must be kept visible in the source.
//
// This section of source code belongs to Protiguous@Protiguous.com unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
//
// Donations and royalties can be paid via
//  PayPal: paypal@Protiguous.com
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  
//
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/Pixel.cs" was last cleaned by Protiguous on 2016/06/18 at 10:51 PM

namespace Librainian.Graphics.Imaging {

    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Maths;
    using Newtonsoft.Json;

    /// <summary>
    ///     <para>
    ///         A simple pixel with <see cref="Checksum" />, <see cref="Alpha" />, <see cref="Red" />, <see cref="Green" />,
    ///         <see cref="Blue" />, and <see cref="X" /> & <see cref="Y" /> values.
    ///     </para>
    ///     <remarks>Thoroughly untested.</remarks>
    /// </summary>
    [Immutable]
    [JsonObject]
    [StructLayout( LayoutKind.Sequential )]
    public struct Pixel : IEquatable<Pixel> {

        [JsonProperty]

        //[FieldOffset( 0 )]
        public readonly Byte Checksum;

        [JsonProperty]

        //[FieldOffset( sizeof( Byte ) + 4 * sizeof( Byte ) )]
        public readonly UInt32 X;

        [JsonProperty]

        //[FieldOffset( sizeof( Byte ) + 4 * sizeof( Byte ) + sizeof( UInt32 ) )]
        public readonly UInt32 Y;

        [JsonProperty]

        //[FieldOffset( sizeof( Byte ) + 0 * sizeof( Byte ) )]
        public readonly Byte Alpha;

        [JsonProperty]

        //[FieldOffset( sizeof( Byte ) + 1 * sizeof( Byte ) )]
        public readonly Byte Red;

        [JsonProperty]

        //[FieldOffset( sizeof( Byte ) + 2 * sizeof( Byte ) )]
        public readonly Byte Green;

        [JsonProperty]

        //[FieldOffset( sizeof( Byte ) + 3 * sizeof( Byte ) )]
        public readonly Byte Blue;

        public static Byte Hash( Byte alpha, Byte red, Byte green, Byte blue, UInt32 x, UInt32 y ) => ( Byte )Hashing.GetHashCodes( alpha, red, green, blue, x, y );

        public static Byte Hash( UInt32 x, UInt32 y, Byte alpha, Byte red, Byte green, Byte blue ) => ( Byte )Hashing.GetHashCodes( x, y, alpha, red, green, blue );

        public Pixel( Byte alpha, Byte red, Byte green, Byte blue, UInt32 x, UInt32 y ) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.X = x;
            this.Y = y;
            this.Checksum = Hash( this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );
        }

        public Pixel( UInt32 x, UInt32 y, Byte alpha, Byte red, Byte green, Byte blue ) {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.X = x;
            this.Y = y;
            this.Checksum = Hash( this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );
        }

        public Pixel( Color color, UInt32 x, UInt32 y ) {
            this.Alpha = color.A;
            this.Red = color.R;
            this.Green = color.G;
            this.Blue = color.B;
            this.X = x;
            this.Y = y;
            this.Checksum = Hash( this.Alpha, this.Red, this.Green, this.Blue, this.X, this.Y );
        }

        //public static explicit operator Pixel( Color pixel ) => new Pixel( pixel.A, pixel.R, pixel.G, pixel.B );

        public static implicit operator Color( Pixel pixel ) => Color.FromArgb( pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue );

        public static explicit operator Byte[] ( Pixel pixel ) => new[] { pixel.Checksum, pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue };

        /// <summary>
        ///     Static comparison.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equal( Pixel left, Pixel right ) => left.Checksum == right.Checksum && left.Alpha == right.Alpha && left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.X == right.X && left.Y == right.Y;

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter;
        ///     otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public Boolean Equals( Pixel other ) => Equal( this, other );

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        [Pure]
        public override Int32 GetHashCode() => this.Checksum + this.Alpha + this.Red + this.Green + this.Blue;

        public override String ToString() => $"{this.Checksum}({this.Alpha},{this.Red},{this.Green},{this.Blue})@{this.X},{this.Y}";

        public Task WriteToStreamAsync( [NotNull] StreamWriter streamWriter ) {
            if ( streamWriter is null ) {
                throw new ArgumentNullException( nameof( streamWriter ) );
            }
            return streamWriter.WriteLineAsync( this.ToString() );
        }

        public static async Task<Pixel?> ReadFromStreamAsync( [NotNull] StreamReader reader, [NotNull] StreamWriter errors ) {
            if ( reader is null ) {
                throw new ArgumentNullException( nameof( reader ) );
            }
            if ( errors is null ) {
                throw new ArgumentNullException( nameof( errors ) );
            }

            var line = await reader.ReadLineAsync() ?? String.Empty;
            line = line.Trim();

            if ( String.IsNullOrWhiteSpace( line ) ) {
                await errors.WriteLineAsync( "Blank input line" ).ConfigureAwait( false );
                return null;
            }

            var openParent = line.IndexOf( "(", StringComparison.OrdinalIgnoreCase );
            if ( openParent <= -1 ) {
                await errors.WriteLineAsync( $"Unable to find a '(' in {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !Byte.TryParse( line.Substring( 0, openParent ), out var checksum ) ) {
                await errors.WriteLineAsync( $"Unable to parse Checksum from {line}" ).ConfigureAwait( false );
                return null;
            }

            var closeParent = line.IndexOf( ")", StringComparison.OrdinalIgnoreCase );
            if ( closeParent == -1 ) {
                await errors.WriteLineAsync( $"Unable to find a ')' in {line}" ).ConfigureAwait( false );
                return null;
            }

            var argb = line.Substring( openParent + 1, closeParent - openParent ).Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            if ( argb.Length != 4 ) {
                await errors.WriteLineAsync( $"Unable to parse Color from {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !Byte.TryParse( argb[0], out var alpha ) ) {
                await errors.WriteLineAsync( $"Unable to parse Alpha from {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !Byte.TryParse( argb[1], out var red ) ) {
                await errors.WriteLineAsync( $"Unable to parse Red from {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !Byte.TryParse( argb[2], out var green ) ) {
                await errors.WriteLineAsync( $"Unable to parse Green from {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !Byte.TryParse( argb[3], out var blue ) ) {
                await errors.WriteLineAsync( $"Unable to parse Blue from {line}" ).ConfigureAwait( false );
                return null;
            }

            var at = line.IndexOf( "@", StringComparison.OrdinalIgnoreCase );
            if ( at == -1 ) {
                await errors.WriteLineAsync( $"Unable to find an '@' in {line}" ).ConfigureAwait( false );
                return null;
            }

            var xandy = line.Substring( at + 1 ).Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            if ( xandy.Length != 2 ) {
                await errors.WriteLineAsync( $"Unable to parse X & Y from {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !UInt32.TryParse( xandy[0], out var x ) ) {
                await errors.WriteLineAsync( $"Unable to parse X from {line}" ).ConfigureAwait( false );
                return null;
            }

            if ( !UInt32.TryParse( xandy[0], out var y ) ) {
                await errors.WriteLineAsync( $"Unable to parse Y from {line}" ).ConfigureAwait( false );
                return null;
            }

            var pixel = new Pixel( alpha, red, green, blue, x, y );
            if ( pixel.Checksum != checksum ) {
                await errors.WriteLineAsync( $"Warning checksums do not match! Expected {checksum}, but got {pixel.Checksum}" ).ConfigureAwait( false );
            }

            return pixel;
        }
    }
}