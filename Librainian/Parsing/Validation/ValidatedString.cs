// Copyright © Rick@AIBrain.org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
//
// This source code contained in "ValidatedString.cs" belongs to Protiguous@Protiguous.com and
// Rick@AIBrain.org unless otherwise specified or the original license has
// been overwritten by formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//     bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//     paypal@AIBrain.Org
//     (We're still looking into other solutions! Any ideas?)
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
// For business inquiries, please contact me at Protiguous@Protiguous.com
//
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// Project: "Librainian", "ValidatedString.cs" was last formatted by Protiguous on 2018/12/01 at 4:46 PM.

namespace Librainian.Parsing.Validation {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security;
	using Exceptions;
	using JetBrains.Annotations;
	using Newtonsoft.Json;

	/// <summary>
	///     Based from https://codereview.stackexchange.com/questions/208291/enforcing-string-validity-with-the-c-type-system
	/// </summary>
	[Serializable]
	[JsonObject]
	[JsonConverter( typeof( ValidatedStringJsonNetConverter ) )]
	public abstract class ValidatedString : IValidatedString, IStringValidator {

		[NotNull]
		public String Value { get; }

		public Int32 Length => this.Value.Length;

		public Int32 CompareTo( [CanBeNull] String other ) => String.Compare( this.Value, other, StringComparison.Ordinal );

		public Int32 CompareTo( [NotNull] IValidatedString other ) => String.Compare( this.Value, other.Value, StringComparison.Ordinal );

		public Boolean Equals( String other ) => this.Value.Equals( other );

		public Boolean Equals( IValidatedString other ) => Equals( this, other );

		public IEnumerator<Char> GetEnumerator() => ( ( IEnumerable<Char> ) this.Value ).GetEnumerator();

		public String Validate( String value ) => this.IsValid( value ) ? value : throw this.InvalidFormat;

		Int32 IComparable.CompareTo( [CanBeNull] Object obj ) => this.Value.CompareTo( obj );

		IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable ) this.Value ).GetEnumerator();

		[NotNull]
		public Exception InvalidFormat => new InvalidFormatException( $"The value must meet the requirement \"{this.Requirements}\"." );

		/// <summary>
		///     Human-readable description of the requirements for this string.
		/// </summary>
		[NotNull]
		public abstract String Requirements { get; }

		public Char this[ Int32 index ] => this.Value[ index ];

		protected ValidatedString( String value, [NotNull] IStringValidator validator ) => this.Value = validator.Validate( value );

		protected ValidatedString( String value, [NotNull] Func<String, String> validator ) => this.Value = validator( value );

		protected ValidatedString( String value ) => this.Value = this.Validate( value );

		protected abstract Boolean IsValid( String value );

		[SecuritySafeCritical]
		public static Int32 Compare( [NotNull] ValidatedString left, [NotNull] ValidatedString right, StringComparison comparisonType = StringComparison.Ordinal ) =>
			comparisonType == StringComparison.Ordinal ? String.CompareOrdinal( left, right ) : String.Compare( left.Value, right.Value, comparisonType );

		public static Int32 Compare( [NotNull] ValidatedString left, Int32 leftIndex, [NotNull] IValidatedString right, Int32 rightIndex, Int32 length ) =>
			String.Compare( left.Value, leftIndex, right.Value, rightIndex, length );

		[SecuritySafeCritical]
		public static Int32 Compare( [NotNull] ValidatedString left, Int32 leftIndex, [NotNull] IValidatedString right, Int32 rightIndex, Int32 length, StringComparison comparisonType ) =>
			String.Compare( left.Value, leftIndex, right.Value, rightIndex, length, comparisonType );

		public static Int32 CompareOrdinal( [NotNull] ValidatedString strA, [NotNull] IValidatedString right ) => String.CompareOrdinal( strA.Value, right.Value );

		[SecuritySafeCritical]
		public static Int32 CompareOrdinal( [NotNull] ValidatedString strA, Int32 indexA, [NotNull] IValidatedString strB, Int32 indexB, Int32 length ) =>
			String.CompareOrdinal( strA.Value, indexA, strB.Value, indexB, length );

		/// <summary>
		///     Static comparison for two <see cref="IValidatedString" />.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] IValidatedString left, [CanBeNull] IValidatedString right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return false;
			}

			if ( left.Value.Length != right.Value.Length ) {
				return false;
			}

			return left.Value.SequenceEqual( right.Value );
		}

		/// <summary>
		///     Static comparison for <see cref="IValidatedString" /> and <see cref="String" />.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] IValidatedString left, [CanBeNull] String right ) {

			if ( left is null || right is null ) {
				return false;
			}

			if ( left.Value.Length != right.Length ) {
				return false;
			}

			return left.Value.SequenceEqual( right );
		}

		[CanBeNull]
		public static implicit operator String( [CanBeNull] ValidatedString str ) => str?.Value;

		public static Boolean operator !=( [NotNull] ValidatedString left, [NotNull] IValidatedString right ) => !Equals( left, right );

		public static Boolean operator ==( [NotNull] ValidatedString left, [NotNull] IValidatedString right ) => Equals( left, right );

		public override Boolean Equals( Object obj ) => this.Value.Equals( obj );

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		public override String ToString() => this.Value;
	}
}