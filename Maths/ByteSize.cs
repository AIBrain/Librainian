namespace Librainian.Maths {

	using System;
	using System.Globalization;

	/// <summary>
	/// Represents a byte size value.
	/// </summary>
	/// <remarks>Found at https://raw.githubusercontent.com/omar/ByteSize/master/src/ByteSizeLib/cs </remarks>
	public struct ByteSize : IComparable<ByteSize>, IEquatable<ByteSize> {
		public static readonly ByteSize MinValue = FromBits( 0 );
		public static readonly ByteSize MaxValue = FromBits( Int64.MaxValue );

		public const Int64 BitsInByte = 8;
		public const Int64 BytesInKiloByte = 1024;

		public const Int64 BytesInMegaByte = BytesInKiloByte * BytesInKiloByte; //1048576;
		public const Int64 BytesInGigaByte = BytesInMegaByte * BytesInKiloByte; //1073741824;
		public const Int64 BytesInTeraByte = BytesInGigaByte * BytesInKiloByte; //1099511627776;
		public const Int64 BytesInPetaByte = BytesInTeraByte * BytesInKiloByte; //1125899906842624;

		public const String BitSymbol = "b";
		public const String ByteSymbol = "B";
		public const String KiloByteSymbol = "KB";
		public const String MegaByteSymbol = "MB";
		public const String GigaByteSymbol = "GB";
		public const String TeraByteSymbol = "TB";
		public const String PetaByteSymbol = "PB";

		public Int64 Bits { get; }
		public Double Bytes { get; }

		public Double KiloBytes => this.Bytes / BytesInKiloByte;
		public Double MegaBytes => this.Bytes / BytesInMegaByte;
		public Double GigaBytes => this.Bytes / BytesInGigaByte;
		public Double TeraBytes => this.Bytes / BytesInTeraByte;
		public Double PetaBytes => this.Bytes / BytesInPetaByte;

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

		public ByteSize( Double byteSize )
			: this() {
			// Get ceiling because bits are whole units
			this.Bits = ( Int64 )Math.Ceiling( byteSize * BitsInByte );

			this.Bytes = byteSize;
		}

		public static ByteSize FromBits( Int64 value ) => new ByteSize( value / ( Double )BitsInByte );

		public static ByteSize FromBytes( Double value ) => new ByteSize( value );

		public static ByteSize FromKiloBytes( Double value ) => new ByteSize( value * BytesInKiloByte );

		public static ByteSize FromMegaBytes( Double value ) => new ByteSize( value * BytesInMegaByte );

		public static ByteSize FromGigaBytes( Double value ) => new ByteSize( value * BytesInGigaByte );

		public static ByteSize FromTeraBytes( Double value ) => new ByteSize( value * BytesInTeraByte );

		public static ByteSize FromPetaBytes( Double value ) => new ByteSize( value * BytesInPetaByte );

		/// <summary>
		/// Converts the value of the current ByteSize object to a string.
		/// The metric prefix symbol (bit, byte, kilo, mega, giga, tera) used is
		/// the largest metric prefix such that the corresponding value is greater
		//  than or equal to one.
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

		public Int32 CompareTo( ByteSize other ) => this.Bits.CompareTo( other.Bits );

		public ByteSize Add( ByteSize bs ) => new ByteSize( this.Bytes + bs.Bytes );

		public ByteSize AddBits( Int64 value ) => this + FromBits( value );

		public ByteSize AddBytes( Double value ) => this + FromBytes( value );

		public ByteSize AddKiloBytes( Double value ) => this + FromKiloBytes( value );

		public ByteSize AddMegaBytes( Double value ) => this + FromMegaBytes( value );

		public ByteSize AddGigaBytes( Double value ) => this + FromGigaBytes( value );

		public ByteSize AddTeraBytes( Double value ) => this + FromTeraBytes( value );

		public ByteSize AddPetaBytes( Double value ) => this + FromPetaBytes( value );

		public ByteSize Subtract( ByteSize bs ) => new ByteSize( this.Bytes - bs.Bytes );

		public static ByteSize operator +( ByteSize b1, ByteSize b2 ) => new ByteSize( b1.Bytes + b2.Bytes );

		public static ByteSize operator ++( ByteSize b ) => new ByteSize( b.Bytes + 1 );

		public static ByteSize operator -( ByteSize b ) => new ByteSize( -b.Bytes );

		public static ByteSize operator -( ByteSize b1, ByteSize b2 ) => new ByteSize( b1.Bytes - b2.Bytes );

		public static ByteSize operator --( ByteSize b ) => new ByteSize( b.Bytes - 1 );

		public static Boolean operator ==( ByteSize b1, ByteSize b2 ) => b1.Bits == b2.Bits;

		public static Boolean operator !=( ByteSize b1, ByteSize b2 ) => b1.Bits != b2.Bits;

		public static Boolean operator <( ByteSize b1, ByteSize b2 ) => b1.Bits < b2.Bits;

		public static Boolean operator <=( ByteSize b1, ByteSize b2 ) => b1.Bits <= b2.Bits;

		public static Boolean operator >( ByteSize b1, ByteSize b2 ) => b1.Bits > b2.Bits;

		public static Boolean operator >=( ByteSize b1, ByteSize b2 ) => b1.Bits >= b2.Bits;

		public static ByteSize Parse( String s ) {
			// Arg checking
			if ( String.IsNullOrWhiteSpace( s ) ) {
				throw new ArgumentNullException( nameof(s), "String is null or whitespace" );
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
				throw new FormatException( $"No number found in value '{ s }'." );
			}

			// Get the magnitude part
			switch ( sizePart ) {
				case "b":
					if ( number % 1 != 0 ) // Can't have partial bits
{
						throw new FormatException( $"Can't have partial bits for value '{ s }'." );
					}

					return FromBits( ( Int64 )number );

				case "B":
					return FromBytes( number );

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

				default:
					throw new FormatException( $"Bytes of magnitude '{ sizePart }' is not supported." );
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
	}
}