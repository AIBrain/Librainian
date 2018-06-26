// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "ByteSize.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com .
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "ByteSize.cs" was last formatted by Protiguous on 2018/06/26 at 1:17 AM.

namespace Librainian.Maths {

	using System;
	using System.Globalization;
	using JetBrains.Annotations;

	/// <summary>
	///     Represents a byte size value.
	/// </summary>
	/// <remarks>Found at https://raw.githubusercontent.com/omar/ByteSize/master/src/ByteSizeLib/cs </remarks>
	public struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize> {

		public const Int64 BitsInByte = 8;

		public const String BitSymbol = "b";

		public const Int64 BytesInGigaByte = BytesInMegaByte * BytesInKiloByte;

		public const Int64 BytesInKiloByte = 1024;

		public const Int64 BytesInMegaByte = BytesInKiloByte * BytesInKiloByte;

		public const Int64 BytesInPetaByte = BytesInTeraByte * BytesInKiloByte;

		public const Int64 BytesInTeraByte = BytesInGigaByte * BytesInKiloByte;

		//1125899906842624;
		public const String ByteSymbol = "B";

		public const String GigaByteSymbol = "GB";

		//1099511627776;
		public const String KiloByteSymbol = "KB";

		//1073741824;
		public const String MegaByteSymbol = "MB";

		public const String PetaByteSymbol = "PB";

		//1048576;
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
			this.Bits = ( Int64 ) Math.Ceiling( byteSize * BitsInByte );

			this.Bytes = byteSize;
		}

		public static ByteSize FromBits( Int64 value ) => new ByteSize( value / ( Double ) BitsInByte );

		public static ByteSize FromBytes( Double value ) => new ByteSize( value );

		public static ByteSize FromGigaBytes( Double value ) => new ByteSize( value * BytesInGigaByte );

		public static ByteSize FromKiloBytes( Double value ) => new ByteSize( value * BytesInKiloByte );

		public static ByteSize FromMegaBytes( Double value ) => new ByteSize( value * BytesInMegaByte );

		public static ByteSize FromPetaBytes( Double value ) => new ByteSize( value * BytesInPetaByte );

		public static ByteSize FromTeraBytes( Double value ) => new ByteSize( value * BytesInTeraByte );

		public static ByteSize operator -( ByteSize b ) => new ByteSize( -b.Bytes );

		public static ByteSize operator -( ByteSize b1, ByteSize b2 ) => new ByteSize( b1.Bytes - b2.Bytes );

		public static ByteSize operator --( ByteSize b ) => new ByteSize( b.Bytes - 1 );

		public static Boolean operator !=( ByteSize b1, ByteSize b2 ) => b1.Bits != b2.Bits;

		public static ByteSize operator +( ByteSize b1, ByteSize b2 ) => new ByteSize( b1.Bytes + b2.Bytes );

		public static ByteSize operator ++( ByteSize b ) => new ByteSize( b.Bytes + 1 );

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
				if ( !( Char.IsDigit( s[ num ] ) || s[ num ] == decimalSeparator || s[ num ] == groupSeparator ) ) {
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
			var sizePart = s.Substring( lastNumber, s.Length - lastNumber ).Trim();

			// Get the numeric part
			if ( !Double.TryParse( numberPart, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.CurrentInfo, out var number ) ) {
				throw new FormatException( $"No number found in value '{s}'." );
			}

			// Get the magnitude part
			switch ( sizePart ) {
				case "b":

					if ( number % 1 != 0 ) // Can't have partial bits
					{
						throw new FormatException( $"Can't have partial bits for value '{s}'." );
					}

					return FromBits( ( Int64 ) number );

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

		public static Boolean TryParse( String s, out ByteSize result ) {
			try {
				result = Parse( s );

				return true;
			}
			catch {
				result = new ByteSize();

				return false;
			}
		}

		public ByteSize Add( ByteSize bs ) => new ByteSize( this.Bytes + bs.Bytes );

		public ByteSize AddBits( Int64 value ) => this + FromBits( value );

		public ByteSize AddBytes( Double value ) => this + FromBytes( value );

		public ByteSize AddGigaBytes( Double value ) => this + FromGigaBytes( value );

		public ByteSize AddKiloBytes( Double value ) => this + FromKiloBytes( value );

		public ByteSize AddMegaBytes( Double value ) => this + FromMegaBytes( value );

		public ByteSize AddPetaBytes( Double value ) => this + FromPetaBytes( value );

		public ByteSize AddTeraBytes( Double value ) => this + FromTeraBytes( value );

		public Int32 CompareTo( ByteSize other ) => this.Bits.CompareTo( other.Bits );

		public override Boolean Equals( Object value ) {
			if ( value is null ) {
				return false;
			}

			ByteSize other;

			if ( value is ByteSize size ) {
				other = size;
			}
			else {
				return false;
			}

			return this.Equals( other );
		}

		public Boolean Equals( ByteSize value ) => this.Bits == value.Bits;

		public override Int32 GetHashCode() => this.Bits.GetHashCode();

		public ByteSize Subtract( ByteSize bs ) => new ByteSize( this.Bytes - bs.Bytes );

		//  than or equal to one.
		/// <summary>
		///     Converts the value of the current ByteSize object to a string.
		///     The metric prefix symbol (bit, byte, kilo, mega, giga, tera) used is
		///     the largest metric prefix such that the corresponding value is greater
		/// </summary>
		public override String ToString() => this.ToString( "0.##", CultureInfo.CurrentCulture );

		public String ToString( String format ) => this.ToString( format, CultureInfo.CurrentCulture );

		public String ToString( String format, IFormatProvider provider ) {
			if ( !format.Contains( "#" ) && !format.Contains( "0" ) ) {
				format = "0.## " + format;
			}

			if ( provider is null ) {
				provider = CultureInfo.CurrentCulture;
			}

			Boolean Has( String s ) => format.IndexOf( s, StringComparison.CurrentCultureIgnoreCase ) != -1;

			String Output( Double n ) => n.ToString( format, provider );

			if ( Has( "PB" ) ) {
				return Output( this.PetaBytes );
			}

			if ( Has( "TB" ) ) {
				return Output( this.TeraBytes );
			}

			if ( Has( "GB" ) ) {
				return Output( this.GigaBytes );
			}

			if ( Has( "MB" ) ) {
				return Output( this.MegaBytes );
			}

			if ( Has( "KB" ) ) {
				return Output( this.KiloBytes );
			}

			// Byte and Bit symbol must be case-sensitive
			if ( format.IndexOf( ByteSymbol ) != -1 ) {
				return Output( this.Bytes );
			}

			if ( format.IndexOf( BitSymbol ) != -1 ) {
				return Output( this.Bits );
			}

			return $"{this.LargestWholeNumberValue.ToString( format, provider )} {this.LargestWholeNumberSymbol}";
		}
	}
}