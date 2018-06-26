// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "TrimmedString.cs" belongs to Protiguous@Protiguous.com
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
// File "TrimmedString.cs" was last formatted by Protiguous on 2018/06/26 at 1:37 AM.

namespace Librainian.Parsing {

	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using Exceptions;
	using Extensions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     This <see cref="T:System.String" /> will always be <see cref="F:System.String.Empty" /> or trimmed, but *never*
	///     null. I hope.
	/// </summary>
	[DebuggerDisplay( "{" + nameof( ToString ) + "(),nq}" )]
	[Serializable]
	[JsonObject]
	[Immutable]
	public struct TrimmedString : IConvertible, IEquatable<TrimmedString>, IComparable<TrimmedString>, IComparable<String> {

		/// <summary>
		///     An immutable empty <see cref="TrimmedString" /> ( our version of <see cref="String.Empty" />).
		/// </summary>
		public static readonly TrimmedString Empty = new TrimmedString( String.Empty );

		public Boolean IsEmpty => this.Equals( Empty ) || !this.Value.Any();

		[JsonProperty]
		[NotNull]
		public String Value { [NotNull] [DebuggerStepThrough] get; }

		[DebuggerStepThrough]
		public TrimmedString( [CanBeNull] String value, Boolean veryTrim = false ) {
			value = value ?? String.Empty;

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
		///     Calls <see cref="Object.ToString" /> on the <paramref name="value" /> and calls <see cref="String.Trim(char[])" />.
		/// </summary>
		/// <param name="value"></param>
		[DebuggerStepThrough]
		public TrimmedString( [CanBeNull] Object value ) => this.Value = ( value?.ToString() ?? String.Empty ).Trim();

		/// <summary>
		///     Static equality test. (Compares both values with <see cref="String.Equals(Object)" />)
		/// </summary>
		/// <param name="left"> </param>
		/// <param name="right"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		[Pure]
		public static Boolean Equals( TrimmedString left, TrimmedString right ) => String.Equals( left.Value, right.Value, StringComparison.Ordinal );

		[NotNull]
		public static implicit operator String( TrimmedString value ) => value.Value;

		public static implicit operator TrimmedString( String value ) => new TrimmedString( value );

		public Int32 CompareTo( TrimmedString other ) => String.Compare( this.Value, other.Value, StringComparison.CurrentCulture );

		public Int32 CompareTo( String other ) => String.Compare( this.Value, other, StringComparison.CurrentCulture );

		public Boolean Equals( TrimmedString other ) => Equals( this, other );

		public override Boolean Equals( Object obj ) => obj is TrimmedString right && Equals( this, right );

		[DebuggerStepThrough]
		public override Int32 GetHashCode() => this.Value.GetHashCode();

		[DebuggerStepThrough]
		public TypeCode GetTypeCode() => this.Value.GetTypeCode();

		/// <summary>
		///     Compares and ignores case. ( <see cref="StringComparison.CurrentCultureIgnoreCase" />)
		/// </summary>
		/// <param name="right"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public Boolean Like( String right ) => this.Value.Like( right );

		/// <summary>
		///     Compares and ignores case. ( <see cref="StringComparison.CurrentCultureIgnoreCase" />)
		/// </summary>
		/// <param name="right"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public Boolean Like( TrimmedString right ) => this.Value.Like( right.Value );

		[DebuggerStepThrough]
		public void ThrowIfEmpty() {
			if ( this.Value is null || this.IsEmpty ) {
				throw new ArgumentEmptyException( "Value was empty." );
			}
		}

		[DebuggerStepThrough]
		public Boolean ToBoolean( IFormatProvider provider ) => ( this.Value as IConvertible ).ToBoolean( provider: provider );

		[DebuggerStepThrough]
		public Byte ToByte( IFormatProvider provider ) => ( this.Value as IConvertible ).ToByte( provider: provider );

		[DebuggerStepThrough]
		public Char ToChar( IFormatProvider provider ) => ( this.Value as IConvertible ).ToChar( provider: provider );

		[DebuggerStepThrough]
		public DateTime ToDateTime( IFormatProvider provider ) => ( this.Value as IConvertible ).ToDateTime( provider: provider );

		[DebuggerStepThrough]
		public Decimal ToDecimal( IFormatProvider provider ) => ( this.Value as IConvertible ).ToDecimal( provider: provider );

		[DebuggerStepThrough]
		public Double ToDouble( IFormatProvider provider ) => ( this.Value as IConvertible ).ToDouble( provider: provider );

		[DebuggerStepThrough]
		public Int16 ToInt16( IFormatProvider provider ) => ( this.Value as IConvertible ).ToInt16( provider: provider );

		[DebuggerStepThrough]
		public Int32 ToInt32( IFormatProvider provider ) => ( this.Value as IConvertible ).ToInt32( provider: provider );

		[DebuggerStepThrough]
		public Int64 ToInt64( IFormatProvider provider ) => ( this.Value as IConvertible ).ToInt64( provider: provider );

		public TrimmedString ToLower() => this.Value.ToLower();

		[DebuggerStepThrough]
		public SByte ToSByte( IFormatProvider provider ) => ( this.Value as IConvertible ).ToSByte( provider: provider );

		[DebuggerStepThrough]
		public Single ToSingle( IFormatProvider provider ) => ( this.Value as IConvertible ).ToSingle( provider: provider );

		[DebuggerStepThrough]
		public override String ToString() => this.Value;

		[DebuggerStepThrough]
		public String ToString( IFormatProvider provider ) => this.Value.ToString( provider: provider );

		[DebuggerStepThrough]
		public Object ToType( Type conversionType, IFormatProvider provider ) => ( this.Value as IConvertible ).ToType( conversionType: conversionType, provider: provider );

		[DebuggerStepThrough]
		public UInt16 ToUInt16( IFormatProvider provider ) => ( this.Value as IConvertible ).ToUInt16( provider: provider );

		[DebuggerStepThrough]
		public UInt32 ToUInt32( IFormatProvider provider ) => ( this.Value as IConvertible ).ToUInt32( provider: provider );

		[DebuggerStepThrough]
		public UInt64 ToUInt64( IFormatProvider provider ) => ( this.Value as IConvertible ).ToUInt64( provider: provider );

		[DebuggerStepThrough]
		public TrimmedString ToUpper() => this.Value.ToUpper();

		/// <summary>
		///     Strings to be replaced with <see cref="Replacements" />,
		/// </summary>
		public static class Patterns {

			public const String Feeds = Replacements.Feed + Replacements.Feed;

			public const String NewLines = Replacements.NewLine + Replacements.NewLine;

			public const String Returns = Replacements.Return + Replacements.Return;

			public const String Spaces = Replacements.Space + Replacements.Space;

			public const String Tabs = Replacements.Tab + Replacements.Tab;
		}

		public static class Replacements {

			public const String Feed = "\n";

			public const String NewLine = Return + Feed;

			public const String Return = "\r";

			public const String Space = " ";

			public const String Tab = "\t";
		}
	}

	public static class TrimmedStringExtensions {

		public static TrimmedString ToTrimmedString( [NotNull] this Object obj ) {
			if ( obj == null ) {
				throw new ArgumentNullException( paramName: nameof( obj ) );
			}

			return obj.ToString();
		}
	}
}