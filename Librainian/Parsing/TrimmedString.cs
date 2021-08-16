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
// File "TrimmedString.cs" last formatted on 2020-08-14 at 8:41 PM.

#nullable enable

namespace Librainian.Parsing {

	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using Exceptions;
	using Extensions;
	using JetBrains.Annotations;
	using Maths;
	using Newtonsoft.Json;

	/// <summary>This <see cref="String" /> will always be <see cref="Empty" /> or trimmed, but *never* null. I hope.</summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject]
	[Immutable]
	public struct TrimmedString : IConvertible, IEquatable<TrimmedString>, IComparable<TrimmedString>, IComparable<String> {

		/// <summary>An immutable empty <see cref="TrimmedString" /> ( our version of <see cref="String.Empty" />).</summary>
		public static TrimmedString Empty { get; } = new( String.Empty );

		[field: JsonProperty]
		public String Value { get; }

		[DebuggerStepThrough]
		public TrimmedString( String? value, Boolean veryTrim = false ) {
			if ( value == null ) {
				this.Value = String.Empty;

				return;
			}

			if ( veryTrim ) {
				TryAgain:
				var anychange = false;

				while ( value.Contains( Replacements.Space + Replacements.Return ) ) {
					value = value.Replace( Replacements.Space + Replacements.Return, Replacements.Return ).Trim();
					anychange = true;
				}

				while ( value.Contains( Replacements.Space + Replacements.Feed ) ) {
					value = value.Replace( Replacements.Space + Replacements.Feed, Replacements.Feed ).Trim();
					anychange = true;
				}

				while ( value.Contains( Patterns.Feeds ) ) {
					value = value.Replace( Patterns.Feeds, Replacements.Feed ).Trim();
					anychange = true;
				}

				while ( value.Contains( Patterns.Returns ) ) {
					value = value.Replace( Patterns.Returns, Replacements.Return ).Trim();
					anychange = true;
				}

				while ( value.Contains( Patterns.NewLines ) ) {
					value = value.Replace( Patterns.NewLines, Replacements.NewLine ).Trim();
					anychange = true;
				}

				while ( value.Contains( Patterns.Spaces ) ) {
					value = value.Replace( Patterns.Spaces, Replacements.Space ).Trim();
					anychange = true;
				}

				while ( value.Contains( Patterns.Tabs ) ) {
					value = value.Replace( Patterns.Tabs, Replacements.Tab ).Trim();
					anychange = true;
				}

				if ( anychange ) {
					goto TryAgain;
				}
			}

			this.Value = value.Trim();
		}

		/// <summary>
		///     Calls <see cref="Object.ToString" /> on the <paramref name="value" /> and calls
		///     <see cref="String.Trim(Char[])" />.
		/// </summary>
		/// <param name="value"></param>
		[DebuggerStepThrough]
		public TrimmedString( Object? value ) => this.Value = ( value?.ToString() ?? String.Empty ).Trim();

		/// <summary>Static equality test. (Compares both values with <see cref="String.Equals(Object)" />)</summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[Pure]
		public static Boolean Equals( TrimmedString left, TrimmedString right ) => String.Equals( left.Value, right.Value, StringComparison.Ordinal );

		public static implicit operator String( TrimmedString value ) => value.Value;

		public static implicit operator TrimmedString( String? value ) => new( value );

		public static Boolean operator !=( TrimmedString left, TrimmedString right ) => !Equals( left, right );

		public static Boolean operator ==( TrimmedString left, TrimmedString right ) => Equals( left, right );

		public Int32 CompareTo( TrimmedString other ) => String.Compare( this.Value, other.Value, StringComparison.Ordinal );

		public Int32 CompareTo( String? other ) => String.Compare( this.Value, other, StringComparison.Ordinal );

		public Boolean Equals( TrimmedString other ) => Equals( this, other );

		public override Boolean Equals( Object? obj ) => obj is TrimmedString right && Equals( this, right );

		[DebuggerStepThrough]
		public override Int32 GetHashCode() => this.Value.GetHashCode();

		[DebuggerStepThrough]
		public TypeCode GetTypeCode() => this.Value.GetTypeCode();

		public Boolean IsEmpty() => !this.Value.Length.Any() || !this.Value.Any() || Equals( this, Empty );

		public Boolean IsNotEmpty() => !this.IsEmpty();

		/// <summary>Compares and ignores case. ( <see cref="StringComparison.CurrentCultureIgnoreCase" />)</summary>
		/// <param name="right"></param>
		[DebuggerStepThrough]
		public Boolean Like( String? right ) => this.Value.Like( right );

		/// <summary>Compares and ignores case. ( <see cref="StringComparison.CurrentCultureIgnoreCase" />)</summary>
		/// <param name="right"></param>
		[DebuggerStepThrough]
		public Boolean Like( TrimmedString right ) => this.Value.Like( right.Value );

		[DebuggerStepThrough]
		public void ThrowIfEmpty() {
			if ( this.IsEmpty() ) {
				throw new ArgumentEmptyException( "Value was empty." );
			}
		}

		[DebuggerStepThrough]
		public Boolean ToBoolean( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToBoolean( provider );

		[DebuggerStepThrough]
		public Byte ToByte( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToByte( provider );

		[DebuggerStepThrough]
		public Char ToChar( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToChar( provider );

		[DebuggerStepThrough]
		public DateTime ToDateTime( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToDateTime( provider );

		[DebuggerStepThrough]
		public Decimal ToDecimal( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToDecimal( provider );

		[DebuggerStepThrough]
		public Double ToDouble( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToDouble( provider );

		[DebuggerStepThrough]
		public Int16 ToInt16( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToInt16( provider );

		[DebuggerStepThrough]
		public Int32 ToInt32( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToInt32( provider );

		[DebuggerStepThrough]
		public Int64 ToInt64( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToInt64( provider );

		public TrimmedString ToLower( CultureInfo? cultureInfo = null ) => this.Value.ToLower( cultureInfo ?? CultureInfo.CurrentCulture );

		[DebuggerStepThrough]
		public SByte ToSByte( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToSByte( provider );

		[DebuggerStepThrough]
		public Single ToSingle( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToSingle( provider );

		[DebuggerStepThrough]
		public override String ToString() => this.Value;

		[DebuggerStepThrough]
		public String ToString( IFormatProvider? provider ) => this.Value.ToString( provider );

		[DebuggerStepThrough]
		public Object ToType( Type conversionType, IFormatProvider? provider ) => ( this.Value as IConvertible ).ToType( conversionType, provider );

		[DebuggerStepThrough]
		public UInt16 ToUInt16( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToUInt16( provider );

		[DebuggerStepThrough]
		public UInt32 ToUInt32( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToUInt32( provider );

		[DebuggerStepThrough]
		public UInt64 ToUInt64( IFormatProvider? provider ) => ( this.Value as IConvertible ).ToUInt64( provider );

		[DebuggerStepThrough]
		public TrimmedString ToUpper( CultureInfo? cultureInfo = null ) => this.Value.ToUpper( cultureInfo ?? CultureInfo.CurrentCulture );

		/// <summary>Strings to be replaced with <see cref="Replacements" />,</summary>
		internal static class Patterns {

			public const String Feeds = Replacements.Feed + Replacements.Feed;

			public const String NewLines = Replacements.NewLine + Replacements.NewLine;

			public const String Returns = Replacements.Return + Replacements.Return;

			public const String Spaces = Replacements.Space + Replacements.Space;

			public const String Tabs = Replacements.Tab + Replacements.Tab;
		}

		internal static class Replacements {

			public const String Feed = "\n";

			public const String NewLine = Return + Feed;

			public const String Return = "\r";

			public const String Space = " ";

			public const String Tab = "\t";
		}
	}
}