// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "TrimmedString.cs" belongs to Protiguous@Protiguous.com
// unless otherwise specified or the original license has been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code in a commercial project, you must contact
// Protiguous@Protiguous.com for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     PayPal: Protiguous@Protiguous.com
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
// Project: "Librainian", "TrimmedString.cs" was last formatted by Protiguous on 2020/01/31 at 12:28 AM.

namespace LibrainianCore.Parsing {

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
        public static readonly TrimmedString Empty;

        [JsonProperty]
        [NotNull]
        public String Value { [NotNull] [DebuggerStepThrough] get; }

        static TrimmedString() => Empty = new TrimmedString( String.Empty );

        [DebuggerStepThrough]
        public TrimmedString( [CanBeNull] String value, Boolean veryTrim = false ) {
            value ??= String.Empty;

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

        /// <summary>Calls <see cref="Object.ToString" /> on the <paramref name="value" /> and calls <see cref="String.Trim(Char[])" />.</summary>
        /// <param name="value"></param>
        [DebuggerStepThrough]
        public TrimmedString( [CanBeNull] Object? value ) => this.Value = ( value?.ToString() ?? String.Empty ).Trim();

        /// <summary>Static equality test. (Compares both values with <see cref="String.Equals(Object)" />)</summary>
        /// <param name="left"> </param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        [Pure]
        public static Boolean Equals( TrimmedString left, TrimmedString right ) => String.Equals( left.Value, right.Value, StringComparison.Ordinal );

        [NotNull]
        public static implicit operator String( TrimmedString value ) => value.Value;

        public static implicit operator TrimmedString( [CanBeNull] String value ) => new TrimmedString( value );

        public Int32 CompareTo( TrimmedString other ) => String.Compare( this.Value, other.Value, StringComparison.CurrentCulture );

        public Int32 CompareTo( [CanBeNull] String other ) => String.Compare( this.Value, other, StringComparison.CurrentCulture );

        public Boolean Equals( TrimmedString other ) => Equals( this, other );

        public override Boolean Equals( Object obj ) => obj is TrimmedString right && Equals( this, right );

        [DebuggerStepThrough]
        public override Int32 GetHashCode() => this.Value.GetHashCode();

        [DebuggerStepThrough]
        public TypeCode GetTypeCode() => this.Value.GetTypeCode();

        public Boolean IsEmpty() => !this.Value.Length.Any() || !this.Value.Any() || Equals( this, Empty );

        public Boolean IsNotEmpty() => !this.IsEmpty();

        /// <summary>Compares and ignores case. ( <see cref="StringComparison.CurrentCultureIgnoreCase" />)</summary>
        /// <param name="right"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Boolean Like( [CanBeNull] String right ) => this.Value.Like( right );

        /// <summary>Compares and ignores case. ( <see cref="StringComparison.CurrentCultureIgnoreCase" />)</summary>
        /// <param name="right"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public Boolean Like( TrimmedString right ) => this.Value.Like( right.Value );

        [DebuggerStepThrough]
        public void ThrowIfEmpty() {
            if ( this.Value is null || this.IsEmpty() ) {
                throw new ArgumentEmptyException( "Value was empty." );
            }
        }

        [DebuggerStepThrough]
        public Boolean ToBoolean( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToBoolean( provider );

        [DebuggerStepThrough]
        public Byte ToByte( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToByte( provider );

        [DebuggerStepThrough]
        public Char ToChar( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToChar( provider );

        [DebuggerStepThrough]
        public DateTime ToDateTime( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToDateTime( provider );

        [DebuggerStepThrough]
        public Decimal ToDecimal( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToDecimal( provider );

        [DebuggerStepThrough]
        public Double ToDouble( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToDouble( provider );

        [DebuggerStepThrough]
        public Int16 ToInt16( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToInt16( provider );

        [DebuggerStepThrough]
        public Int32 ToInt32( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToInt32( provider );

        [DebuggerStepThrough]
        public Int64 ToInt64( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToInt64( provider );

        public TrimmedString ToLower() => this.Value.ToLower( CultureInfo.CurrentCulture );

        [DebuggerStepThrough]
        public SByte ToSByte( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToSByte( provider );

        [DebuggerStepThrough]
        public Single ToSingle( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToSingle( provider );

        [DebuggerStepThrough]
        [NotNull]
        public override String ToString() => this.Value;

        [DebuggerStepThrough]
        public String ToString( [CanBeNull] IFormatProvider provider ) => this.Value.ToString( provider );

        [DebuggerStepThrough]
        public Object ToType( Type conversionType, [CanBeNull] IFormatProvider provider ) =>
            ( this.Value as IConvertible ).ToType( conversionType, provider );

        [DebuggerStepThrough]
        public UInt16 ToUInt16( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToUInt16( provider );

        [DebuggerStepThrough]
        public UInt32 ToUInt32( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToUInt32( provider );

        [DebuggerStepThrough]
        public UInt64 ToUInt64( [CanBeNull] IFormatProvider provider ) => ( this.Value as IConvertible ).ToUInt64( provider );

        [DebuggerStepThrough]
        public TrimmedString ToUpper() => this.Value.ToUpper( CultureInfo.CurrentCulture );

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