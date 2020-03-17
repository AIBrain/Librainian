// Copyright © 2020 Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, and source code (directly or derived)
// from our binaries, libraries, projects, or solutions.
//
// This source code contained in "ValidatedString.cs" belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
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
// Project: "Librainian", File: "ValidatedString.cs" was last formatted by Protiguous on 2020/03/16 at 2:59 PM.

namespace Librainian.Parsing.Validation {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    [Serializable]
    [JsonObject]
    [JsonConverter( converterType: typeof( ValidatedStringJsonNetConverter ) )]
    public class ValidatedString : IValidatedString {

        /// <inheritdoc />
        public Boolean? Validated { get; }

        /// <inheritdoc />
        [NotNull]
        public Func<String, Boolean> ValidateFunc { get; set; }

        [NotNull]
        public String Value { get; }

        public Int32 Length => this.Value.Length;

        public Char this[ Int32 index ] => this.Value[ index: index ];

        public ValidatedString( [NotNull] String value, [NotNull] Func<String, Boolean> validationFunc ) {
            this.Value = value ?? throw new ArgumentNullException( paramName: nameof( value ) );
            this.ValidateFunc = validationFunc ?? throw new ArgumentNullException( paramName: nameof( validationFunc ) );
            this.Validated = this.ValidateFunc( arg: this.Value );
        }

        [SecuritySafeCritical]
        public static Int32 Compare( [NotNull] ValidatedString left, [NotNull] IValidatedString right, StringComparison comparisonType = StringComparison.Ordinal ) =>
            comparisonType == StringComparison.Ordinal ?
                String.CompareOrdinal( strA: left, strB: right.Value ) :
                String.Compare( strA: left.Value, strB: right.Value, comparisonType: comparisonType );

        public static Int32 Compare( [NotNull] ValidatedString left, Int32 leftIndex, [NotNull] IValidatedString right, Int32 rightIndex, Int32 length ) =>
            String.Compare( strA: left.Value, indexA: leftIndex, strB: right.Value, indexB: rightIndex, length: length );

        [SecuritySafeCritical]
        public static Int32 Compare( [NotNull] ValidatedString left, Int32 leftIndex, [NotNull] IValidatedString right, Int32 rightIndex, Int32 length,
            StringComparison comparisonType ) =>
            String.Compare( strA: left.Value, indexA: leftIndex, strB: right.Value, indexB: rightIndex, length: length, comparisonType: comparisonType );

        public static Int32 CompareOrdinal( [NotNull] ValidatedString strA, [NotNull] IValidatedString right ) => String.CompareOrdinal( strA: strA.Value, strB: right.Value );

        [SecuritySafeCritical]
        public static Int32 CompareOrdinal( [NotNull] ValidatedString strA, Int32 indexA, [NotNull] IValidatedString strB, Int32 indexB, Int32 length ) =>
            String.CompareOrdinal( strA: strA.Value, indexA: indexA, strB: strB.Value, indexB: indexB, length: length );

        /// <summary>Static comparison for two <see cref="IValidatedString" />.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] IValidatedString left, [CanBeNull] IValidatedString right ) {

            if ( ReferenceEquals( objA: left, objB: right ) ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            if ( left.Value is null || right.Value is null ) {
                return default;
            }

            if ( left.Value.Length != right.Value.Length ) {
                return default;
            }

            return left.SequenceEqual( second: right.Value );
        }

        /// <summary>Static comparison for two <see cref="IValidatedString" />.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] String? left, [CanBeNull] IValidatedString right ) {
            if ( left is null && right is null ) {
                return true;
            }

            if ( left is null || right is null ) {
                return default;
            }

            if ( right.Value is null ) {
                return default;
            }

            if ( left.Length != right.Value.Length ) {
                return default;
            }

            return left.SequenceEqual( second: right.Value );
        }

        /// <summary>Static comparison for <see cref="IValidatedString" /> and <see cref="String" />.</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean Equals( [CanBeNull] IValidatedString left, [CanBeNull] String? right ) {

            if ( left is null || right is null ) {
                return default;
            }

            if ( left.Value is null ) {
                return default;
            }

            if ( left.Value.Length != right.Length ) {
                return default;
            }

            return left.Value.SequenceEqual( second: right );
        }

        [CanBeNull]
        public static implicit operator String( [CanBeNull] ValidatedString str ) => str?.Value;

        public static Boolean operator !=( [CanBeNull] ValidatedString left, [CanBeNull] IValidatedString right ) => !Equals( left: left, right: right?.Value );

        public static Boolean operator ==( [CanBeNull] ValidatedString left, [CanBeNull] IValidatedString right ) => Equals( left: left, right: right?.Value );

        public Int32 CompareTo( [CanBeNull] String? other ) => String.Compare( strA: this.Value, strB: other, comparisonType: StringComparison.Ordinal );

        public Int32 CompareTo( [NotNull] IValidatedString other ) => String.Compare( strA: this.Value, strB: other.Value, comparisonType: StringComparison.Ordinal );

        public Boolean Equals( String other ) => Equals( left: this, right: other );

        public Boolean Equals( IValidatedString other ) => Equals( left: this.Value, right: other );

        public override Boolean Equals( [CanBeNull] Object obj ) => Equals( left: this.Value, right: obj as IValidatedString );

        public IEnumerator<Char> GetEnumerator() => ( ( IEnumerable<Char> )this.Value ).GetEnumerator();

        public override Int32 GetHashCode() => this.Value.GetHashCode();

        public override String ToString() => this.Value;

        Int32 IComparable.CompareTo( [CanBeNull] Object obj ) => this.Value.CompareTo( value: obj );

        IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Value ).GetEnumerator();
    }
}