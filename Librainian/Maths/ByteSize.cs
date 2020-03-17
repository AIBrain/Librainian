// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ByteSize.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original license and our thanks goes to those Authors.
// If you find your code in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// =========================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
//    No warranties are expressed, implied, or given.
//    We are NOT responsible for Anything You Do With Our Code.
//    We are NOT responsible for Anything You Do With Our Executables.
//    We are NOT responsible for Anything You Do With Your Computer.
// =========================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we make available.
//
// Project: "Librainian", File: "ByteSize.cs" was last formatted by Protiguous on 2020/03/16 at 2:56 PM.

namespace Librainian.Maths {

    using System;
    using System.Globalization;
    using JetBrains.Annotations;

    /// <summary>Represents a byte size value.</summary>
    /// <remarks>Source: https://github.com/omar/ByteSize/blob/master/src/ByteSizeLib/ByteSize.cs </remarks>
    public struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize> {

        public const Int64 BitsInByte = 8;

        public const String BitSymbol = "b";

        public const Int64 BytesInGigaByte = BytesInMegaByte * BytesInKiloByte;

        public const Int64 BytesInKiloByte = 1024;

        public const Int64 BytesInMegaByte = BytesInKiloByte * BytesInKiloByte;

        public const Int64 BytesInPetaByte = BytesInTeraByte * BytesInKiloByte;

        public const Int64 BytesInTeraByte = BytesInGigaByte * BytesInKiloByte;

        public const String ByteSymbol = "B";

        public const String GigaByteSymbol = "GB";

        public const String KiloByteSymbol = "KB";

        public const String MegaByteSymbol = "MB";

        public const String PetaByteSymbol = "PB";

        public const String TeraByteSymbol = "TB";

        public static readonly ByteSize MaxValue = FromBits( value: Int64.MaxValue );

        public static readonly ByteSize MinValue = FromBits( value: 0 );

        public Int64 Bits { get; }

        public Double Bytes { get; }

        public Double GigaBytes => this.Bytes / BytesInGigaByte;

        public Double KiloBytes => this.Bytes / BytesInKiloByte;

        [NotNull]
        public String LargestWholeNumberSymbol {
            get {

                // Absolute value is used to deal with negative values
                if ( Math.Abs( value: this.PetaBytes ) >= 1 ) {
                    return PetaByteSymbol;
                }

                if ( Math.Abs( value: this.TeraBytes ) >= 1 ) {
                    return TeraByteSymbol;
                }

                if ( Math.Abs( value: this.GigaBytes ) >= 1 ) {
                    return GigaByteSymbol;
                }

                if ( Math.Abs( value: this.MegaBytes ) >= 1 ) {
                    return MegaByteSymbol;
                }

                if ( Math.Abs( value: this.KiloBytes ) >= 1 ) {
                    return KiloByteSymbol;
                }

                if ( Math.Abs( value: this.Bytes ) >= 1 ) {
                    return ByteSymbol;
                }

                return BitSymbol;
            }
        }

        public Double LargestWholeNumberValue {
            get {

                // Absolute value is used to deal with negative values
                if ( Math.Abs( value: this.PetaBytes ) >= 1 ) {
                    return this.PetaBytes;
                }

                if ( Math.Abs( value: this.TeraBytes ) >= 1 ) {
                    return this.TeraBytes;
                }

                if ( Math.Abs( value: this.GigaBytes ) >= 1 ) {
                    return this.GigaBytes;
                }

                if ( Math.Abs( value: this.MegaBytes ) >= 1 ) {
                    return this.MegaBytes;
                }

                if ( Math.Abs( value: this.KiloBytes ) >= 1 ) {
                    return this.KiloBytes;
                }

                if ( Math.Abs( value: this.Bytes ) >= 1 ) {
                    return this.Bytes;
                }

                return this.Bits;
            }
        }

        public Double MegaBytes => this.Bytes / BytesInMegaByte;

        public Double PetaBytes => this.Bytes / BytesInPetaByte;

        public Double TeraBytes => this.Bytes / BytesInTeraByte;

        public ByteSize( Double byteSize ) : this() {

            // Get ceiling because bits are whole units
            this.Bits = ( Int64 )Math.Ceiling( a: byteSize * BitsInByte );

            this.Bytes = byteSize;
        }

        public static Boolean Equals( ByteSize left, ByteSize right ) => left.Bits == right.Bits;

        public static ByteSize FromBits( Int64 value ) => new ByteSize( byteSize: value / ( Double )BitsInByte );

        public static ByteSize FromBytes( Double value ) => new ByteSize( byteSize: value );

        public static ByteSize FromGigaBytes( Double value ) => new ByteSize( byteSize: value * BytesInGigaByte );

        public static ByteSize FromKiloBytes( Double value ) => new ByteSize( byteSize: value * BytesInKiloByte );

        public static ByteSize FromMegaBytes( Double value ) => new ByteSize( byteSize: value * BytesInMegaByte );

        public static ByteSize FromPetaBytes( Double value ) => new ByteSize( byteSize: value * BytesInPetaByte );

        public static ByteSize FromTeraBytes( Double value ) => new ByteSize( byteSize: value * BytesInTeraByte );

        public static ByteSize operator -( ByteSize b ) => new ByteSize( byteSize: -b.Bytes );

        public static ByteSize operator -( ByteSize b1, ByteSize b2 ) => new ByteSize( byteSize: b1.Bytes - b2.Bytes );

        public static ByteSize operator --( ByteSize b ) => new ByteSize( byteSize: b.Bytes - 1 );

        public static Boolean operator !=( ByteSize b1, ByteSize b2 ) => b1.Bits != b2.Bits;

        public static ByteSize operator +( ByteSize b1, ByteSize b2 ) => new ByteSize( byteSize: b1.Bytes + b2.Bytes );

        public static ByteSize operator ++( ByteSize b ) => new ByteSize( byteSize: b.Bytes + 1 );

        public static Boolean operator <( ByteSize b1, ByteSize b2 ) => b1.Bits < b2.Bits;

        public static Boolean operator <=( ByteSize b1, ByteSize b2 ) => b1.Bits <= b2.Bits;

        public static Boolean operator ==( ByteSize b1, ByteSize b2 ) => b1.Bits == b2.Bits;

        public static Boolean operator >( ByteSize b1, ByteSize b2 ) => b1.Bits > b2.Bits;

        public static Boolean operator >=( ByteSize b1, ByteSize b2 ) => b1.Bits >= b2.Bits;

        public static ByteSize Parse( String s ) {

            // Arg checking
            if ( String.IsNullOrWhiteSpace( value: s ) ) {
                throw new ArgumentNullException( paramName: nameof( s ), message: "String is null or whitespace" );
            }

            // Get the index of the first non-digit character
            s = s.TrimStart(); // Protect against leading spaces

            Int32 num;
            var found = false;

            var decimalSeparator = Convert.ToChar( value: NumberFormatInfo.CurrentInfo.NumberDecimalSeparator );
            var groupSeparator = Convert.ToChar( value: NumberFormatInfo.CurrentInfo.NumberGroupSeparator );

            // Pick first non-digit number
            for ( num = 0; num < s.Length; num++ ) {
                if ( !( Char.IsDigit( c: s[ index: num ] ) || s[ index: num ] == decimalSeparator || s[ index: num ] == groupSeparator ) ) {
                    found = true;

                    break;
                }
            }

            if ( found == false ) {
                throw new FormatException( message: $"No byte indicator found in value '{s}'." );
            }

            var lastNumber = num;

            // Cut the input string in half
            var numberPart = s.Substring( startIndex: 0, length: lastNumber ).Trim();
            var sizePart = s.Substring( startIndex: lastNumber, length: s.Length - lastNumber ).Trim();

            // Get the numeric part
            if ( !Double.TryParse( s: numberPart, style: NumberStyles.Float | NumberStyles.AllowThousands, provider: NumberFormatInfo.CurrentInfo, result: out var number ) ) {
                throw new FormatException( message: $"No number found in value '{s}'." );
            }

            // Get the magnitude part
            switch ( sizePart ) {
                case "b":

                    // Can't have partial bits
                    if ( number % 1 != 0 ) {
                        throw new FormatException( message: $"Can't have partial bits for value '{s}'." );
                    }

                    return FromBits( value: ( Int64 )number );

                case "B": return FromBytes( value: number );

                case "KB":
                case "kB":
                case "kb":
                    return FromKiloBytes( value: number );

                case "MB":
                case "mB":
                case "mb":
                    return FromMegaBytes( value: number );

                case "GB":
                case "gB":
                case "gb":
                    return FromGigaBytes( value: number );

                case "TB":
                case "tB":
                case "tb":
                    return FromTeraBytes( value: number );

                case "PB":
                case "pB":
                case "pb":
                    return FromPetaBytes( value: number );

                default: throw new FormatException( message: $"Bytes of magnitude '{sizePart}' is not supported." );
            }
        }

        public static Boolean TryParse( [CanBeNull] String? s, out ByteSize result ) {
            try {
                result = Parse( s: s );

                return true;
            }
            catch {
                result = new ByteSize();

                return default;
            }
        }

        public ByteSize Add( ByteSize bs ) => new ByteSize( byteSize: this.Bytes + bs.Bytes );

        public ByteSize AddBits( Int64 value ) => this + FromBits( value: value );

        public ByteSize AddBytes( Double value ) => this + FromBytes( value: value );

        public ByteSize AddGigaBytes( Double value ) => this + FromGigaBytes( value: value );

        public ByteSize AddKiloBytes( Double value ) => this + FromKiloBytes( value: value );

        public ByteSize AddMegaBytes( Double value ) => this + FromMegaBytes( value: value );

        public ByteSize AddPetaBytes( Double value ) => this + FromPetaBytes( value: value );

        public ByteSize AddTeraBytes( Double value ) => this + FromTeraBytes( value: value );

        public Int32 CompareTo( ByteSize other ) => this.Bits.CompareTo( value: other.Bits );

        public override Boolean Equals( Object value ) => Equals( left: this, right: value is ByteSize size ? size : default );

        public Boolean Equals( ByteSize value ) => Equals( left: this, right: value );

        public override Int32 GetHashCode() => this.Bits.GetHashCode();

        public ByteSize Subtract( ByteSize bs ) => new ByteSize( byteSize: this.Bytes - bs.Bytes );

        /// <summary>
        /// Converts the value of the current ByteSize object to a string. The metric prefix symbol (bit, byte, kilo, mega, giga, tera) used is the largest metric prefix such that
        /// the corresponding value is greater than or equal to one.
        /// </summary>
        public override String ToString() => this.ToString( format: "0.##", provider: CultureInfo.CurrentCulture );

        [NotNull]
        public String ToString( [CanBeNull] String? format ) => this.ToString( format: format, provider: CultureInfo.CurrentCulture );

        [NotNull]
        public String ToString( String format, IFormatProvider provider ) {
            if ( !format.Contains( value: "#" ) && !format.Contains( value: "0" ) ) {
                format = "0.## " + format;
            }

            if ( provider is null ) {
                provider = CultureInfo.CurrentCulture;
            }

            if ( Has( s: "PB" ) ) {
                return Output( n: this.PetaBytes );
            }

            if ( Has( s: "TB" ) ) {
                return Output( n: this.TeraBytes );
            }

            if ( Has( s: "GB" ) ) {
                return Output( n: this.GigaBytes );
            }

            if ( Has( s: "MB" ) ) {
                return Output( n: this.MegaBytes );
            }

            if ( Has( s: "KB" ) ) {
                return Output( n: this.KiloBytes );
            }

            // Byte and Bit symbol must be case-sensitive
            if ( format.IndexOf( value: ByteSymbol, comparisonType: StringComparison.Ordinal ) != -1 ) {
                return Output( n: this.Bytes );
            }

            if ( format.IndexOf( value: BitSymbol, comparisonType: StringComparison.Ordinal ) != -1 ) {
                return Output( n: this.Bits );
            }

            return $"{this.LargestWholeNumberValue.ToString( format: format, provider: provider )} {this.LargestWholeNumberSymbol}";

            Boolean Has( String s ) => format.IndexOf( value: s, comparisonType: StringComparison.CurrentCultureIgnoreCase ) != -1;

            String Output( Double n ) => n.ToString( format: format, provider: provider );
        }
    }
}