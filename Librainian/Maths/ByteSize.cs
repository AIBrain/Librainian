// Copyright © Protiguous. All Rights Reserved.
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
// 
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
// 
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
// ====================================================================
// 
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// 
// File "ByteSize.cs" last formatted on 2020-08-14 at 8:36 PM.

#nullable enable

namespace Librainian.Maths {

	using System;
	using System.Globalization;
	using JetBrains.Annotations;

	/// <summary>Represents a byte size value.</summary>
	/// <remarks>Source: https://github.com/omar/ByteSize/blob/master/src/ByteSizeLib/ByteSize.cs </remarks>
	public readonly struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize> {

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

		public static readonly ByteSize MaxValue = FromBits( Int64.MaxValue );

		public static readonly ByteSize MinValue = FromBits( 0 );

		public Int64 Bits { get; }

		public Double Bytes { get; }

		public Double GigaBytes => this.Bytes / BytesInGigaByte;

		public Double KiloBytes => this.Bytes / BytesInKiloByte;

		[NotNull]
		public String LargestWholeNumberSymbol {
			get {
				// Absolute value is used to deal with negative values
				if ( Math.Abs( this.PetaBytes ) >= 1 ) {
					return PetaByteSymbol;
				}

				if ( Math.Abs( this.TeraBytes ) >= 1 ) {
					return TeraByteSymbol;
				}

				if ( Math.Abs( this.GigaBytes ) >= 1 ) {
					return GigaByteSymbol;
				}

				if ( Math.Abs( this.MegaBytes ) >= 1 ) {
					return MegaByteSymbol;
				}

				if ( Math.Abs( this.KiloBytes ) >= 1 ) {
					return KiloByteSymbol;
				}

				if ( Math.Abs( this.Bytes ) >= 1 ) {
					return ByteSymbol;
				}

				return BitSymbol;
			}
		}

		public Double LargestWholeNumberValue {
			get {
				// Absolute value is used to deal with negative values
				if ( Math.Abs( this.PetaBytes ) >= 1 ) {
					return this.PetaBytes;
				}

				if ( Math.Abs( this.TeraBytes ) >= 1 ) {
					return this.TeraBytes;
				}

				if ( Math.Abs( this.GigaBytes ) >= 1 ) {
					return this.GigaBytes;
				}

				if ( Math.Abs( this.MegaBytes ) >= 1 ) {
					return this.MegaBytes;
				}

				if ( Math.Abs( this.KiloBytes ) >= 1 ) {
					return this.KiloBytes;
				}

				if ( Math.Abs( this.Bytes ) >= 1 ) {
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
			this.Bits = ( Int64 )Math.Ceiling( byteSize * BitsInByte );

			this.Bytes = byteSize;
		}

		public static Boolean Equals( ByteSize left, ByteSize right ) => left.Bits == right.Bits;

		public static ByteSize FromBits( Int64 value ) => new( value / ( Double )BitsInByte );

		public static ByteSize FromBytes( Double value ) => new( value );

		public static ByteSize FromGigaBytes( Double value ) => new( value * BytesInGigaByte );

		public static ByteSize FromKiloBytes( Double value ) => new( value * BytesInKiloByte );

		public static ByteSize FromMegaBytes( Double value ) => new( value * BytesInMegaByte );

		public static ByteSize FromPetaBytes( Double value ) => new( value * BytesInPetaByte );

		public static ByteSize FromTeraBytes( Double value ) => new( value * BytesInTeraByte );

		public static ByteSize operator -( ByteSize b ) => new( -b.Bytes );

		public static ByteSize operator -( ByteSize b1, ByteSize b2 ) => new( b1.Bytes - b2.Bytes );

		public static ByteSize operator --( ByteSize b ) => new( b.Bytes - 1 );

		public static Boolean operator !=( ByteSize b1, ByteSize b2 ) => b1.Bits != b2.Bits;

		public static ByteSize operator +( ByteSize b1, ByteSize b2 ) => new( b1.Bytes + b2.Bytes );

		public static ByteSize operator ++( ByteSize b ) => new( b.Bytes + 1 );

		public static Boolean operator <( ByteSize b1, ByteSize b2 ) => b1.Bits < b2.Bits;

		public static Boolean operator <=( ByteSize b1, ByteSize b2 ) => b1.Bits <= b2.Bits;

		public static Boolean operator ==( ByteSize b1, ByteSize b2 ) => b1.Bits == b2.Bits;

		public static Boolean operator >( ByteSize b1, ByteSize b2 ) => b1.Bits > b2.Bits;

		public static Boolean operator >=( ByteSize b1, ByteSize b2 ) => b1.Bits >= b2.Bits;

		public static ByteSize Parse( String s ) {
			// Arg checking
			if ( String.IsNullOrWhiteSpace( s ) ) {
				throw new ArgumentNullException( nameof( s ), "String is null or whitespace" );
			}

			// Get the index of the first non-digit character
			s = s.TrimStart(); // Protect against leading spaces

			Int32 num;
			var found = false;

			var decimalSeparator = Convert.ToChar( NumberFormatInfo.CurrentInfo.NumberDecimalSeparator );
			var groupSeparator = Convert.ToChar( NumberFormatInfo.CurrentInfo.NumberGroupSeparator );

			// Pick first non-digit number
			for ( num = 0; num < s.Length; num++ ) {
				if ( !( Char.IsDigit( s[num] ) || s[num] == decimalSeparator || s[num] == groupSeparator ) ) {
					found = true;

					break;
				}
			}

			if ( found == false ) {
				throw new FormatException( $"No byte indicator found in value '{s}'." );
			}

			var lastNumber = num;

			// Cut the input string in half
			var numberPart = s.Substring( 0, lastNumber ).Trim();
			var sizePart = s[ lastNumber.. ].Trim();

			// Get the numeric part
			if ( !Double.TryParse( numberPart, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.CurrentInfo, out var number ) ) {
				throw new FormatException( $"No number found in value '{s}'." );
			}

			// Get the magnitude part
			switch ( sizePart ) {
				case "b":

					// Can't have partial bits
					if ( number % 1 != 0 ) {
						throw new FormatException( $"Can't have partial bits for value '{s}'." );
					}

					return FromBits( ( Int64 )number );

				case "B": return FromBytes( number );

				case "KB":
				case "kB":
				case "kb":
					return FromKiloBytes( number );

				case "MB":
				case "mB":
				case "mb":
					return FromMegaBytes( number );

				case "GB":
				case "gB":
				case "gb":
					return FromGigaBytes( number );

				case "TB":
				case "tB":
				case "tb":
					return FromTeraBytes( number );

				case "PB":
				case "pB":
				case "pb":
					return FromPetaBytes( number );

				default: throw new FormatException( $"Bytes of magnitude '{sizePart}' is not supported." );
			}
		}

		public static Boolean TryParse( [CanBeNull] String? s, out ByteSize result ) {
			try {
				result = Parse( s );

				return true;
			}
			catch {
				result = new ByteSize();

				return false;
			}
		}

		public ByteSize Add( ByteSize bs ) => new( this.Bytes + bs.Bytes );

		public ByteSize AddBits( Int64 value ) => this + FromBits( value );

		public ByteSize AddBytes( Double value ) => this + FromBytes( value );

		public ByteSize AddGigaBytes( Double value ) => this + FromGigaBytes( value );

		public ByteSize AddKiloBytes( Double value ) => this + FromKiloBytes( value );

		public ByteSize AddMegaBytes( Double value ) => this + FromMegaBytes( value );

		public ByteSize AddPetaBytes( Double value ) => this + FromPetaBytes( value );

		public ByteSize AddTeraBytes( Double value ) => this + FromTeraBytes( value );

		public Int32 CompareTo( ByteSize other ) => this.Bits.CompareTo( other.Bits );

		public override Boolean Equals( Object? value ) => Equals( this, value is ByteSize size ? size : default( ByteSize ) );

		public Boolean Equals( ByteSize value ) => Equals( this, value );

		public override Int32 GetHashCode() => this.Bits.GetHashCode();

		public ByteSize Subtract( ByteSize bs ) => new( this.Bytes - bs.Bytes );

		/// <summary>
		///     Converts the value of the current ByteSize object to a string. The metric prefix symbol (bit, byte, kilo, mega,
		///     giga, tera) used is the largest metric prefix such that
		///     the corresponding value is greater than or equal to one.
		/// </summary>
		[NotNull]
		public override String ToString() => this.ToString( "0.##", CultureInfo.CurrentCulture );

		[NotNull]
		public String ToString( [CanBeNull] String? format ) => this.ToString( format, CultureInfo.CurrentCulture );

		[NotNull]
		public String ToString( String format, IFormatProvider provider ) {
			if ( !format.Contains( "#" ) && !format.Contains( "0" ) ) {
				format = "0.## " + format;
			}

			if ( Has( "PB" ) ) {
				return this.PetaBytes.ToString( format, provider );
			}

			if ( Has( "TB" ) ) {
				return this.TeraBytes.ToString( format, provider );
			}

			if ( Has( "GB" ) ) {
				return this.GigaBytes.ToString( format, provider );
			}

			if ( Has( "MB" ) ) {
				return this.MegaBytes.ToString( format, provider );
			}

			if ( Has( "KB" ) ) {
				return this.KiloBytes.ToString( format, provider );
			}

			// Byte and Bit symbol must be case-sensitive
			if ( format.IndexOf( ByteSymbol, StringComparison.Ordinal ) != -1 ) {
				return this.Bytes.ToString( format, provider );
			}

			if ( format.IndexOf( BitSymbol, StringComparison.Ordinal ) != -1 ) {
				return ( ( Double )this.Bits ).ToString( format, provider );
			}

			return $"{this.LargestWholeNumberValue.ToString( format, provider )} {this.LargestWholeNumberSymbol}";

			Boolean Has( String s ) => format.IndexOf( s, StringComparison.CurrentCultureIgnoreCase ) != -1;
		}

	}

}