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
// File "ValidatedString.cs" last formatted on 2020-08-14 at 8:41 PM.

#nullable enable
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
	[JsonConverter( typeof( ValidatedStringJsonNetConverter ) )]
	public class ValidatedString : IValidatedString {

		public ValidatedString( [NotNull] String value, [NotNull] Func<String, Boolean> validationFunc ) {
			this.Value = value ?? throw new ArgumentNullException( nameof( value ) );
			this.ValidateFunc = validationFunc ?? throw new ArgumentNullException( nameof( validationFunc ) );
			this.Validated = this.ValidateFunc( this.Value );
		}

		public Int32 Length => this.Value.Length;

		public Char this[ Int32 index ] => this.Value[index];

		/// <inheritdoc />
		public Boolean? Validated { get; }

		/// <inheritdoc />
		[NotNull]
		public Func<String, Boolean> ValidateFunc { get; set; }

		[NotNull]
		public String Value { get; }

		public Int32 CompareTo( [CanBeNull] String? other ) => String.Compare( this.Value, other, StringComparison.CurrentCulture );

		public Int32 CompareTo( [CanBeNull] IValidatedString? other ) => String.Compare( this.Value, other?.Value, StringComparison.CurrentCulture );

		public Boolean Equals( String? other ) => Equals( this, other );

		public Boolean Equals( IValidatedString? other ) => Equals( this.Value, other );

		public IEnumerator<Char> GetEnumerator() => ( ( IEnumerable<Char> )this.Value ).GetEnumerator();

		Int32 IComparable.CompareTo( [CanBeNull] Object? obj ) => String.Compare( this.Value, obj as String, StringComparison.CurrentCulture );

		IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Value ).GetEnumerator();

		[SecuritySafeCritical]
		public static Int32 Compare( [NotNull] ValidatedString left, [NotNull] IValidatedString right, StringComparison comparisonType = StringComparison.CurrentCulture ) =>
			comparisonType == StringComparison.CurrentCulture ? String.CompareOrdinal( left, right.Value ) : String.Compare( left.Value, right.Value, comparisonType );

		public static Int32 Compare( [NotNull] ValidatedString left, Int32 leftIndex, [NotNull] IValidatedString right, Int32 rightIndex, Int32 length ) =>
			String.Compare( left.Value, leftIndex, right.Value, rightIndex, length, StringComparison.CurrentCulture );

		[SecuritySafeCritical]
		public static Int32 Compare(
			[NotNull] ValidatedString left,
			Int32 leftIndex,
			[NotNull] IValidatedString right,
			Int32 rightIndex,
			Int32 length,
			StringComparison comparisonType
		) =>
			String.Compare( left.Value, leftIndex, right.Value, rightIndex, length, comparisonType );

		public static Int32 CompareOrdinal( [NotNull] ValidatedString strA, [NotNull] IValidatedString right ) => String.CompareOrdinal( strA.Value, right.Value );

		[SecuritySafeCritical]
		public static Int32 CompareOrdinal( [NotNull] ValidatedString strA, Int32 indexA, [NotNull] IValidatedString strB, Int32 indexB, Int32 length ) =>
			String.CompareOrdinal( strA.Value, indexA, strB.Value, indexB, length );

		/// <summary>Static comparison for two <see cref="IValidatedString" />.</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] IValidatedString? left, [CanBeNull] IValidatedString? right ) {
			if ( ReferenceEquals( left, right ) ) {
				return true;
			}

			if ( left is null || right is null ) {
				return default( Boolean );
			}

			if ( left.Value is null || right.Value is null ) {
				return default( Boolean );
			}

			if ( left.Value.Length != right.Value.Length ) {
				return default( Boolean );
			}

			return left.SequenceEqual( right.Value );
		}

		/// <summary>Static comparison for two <see cref="IValidatedString" />.</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] String? left, [CanBeNull] IValidatedString? right ) {
			if ( left is null && right is null ) {
				return true;
			}

			if ( left is null || right?.Value is null ) {
				return default( Boolean );
			}

			return left.SequenceEqual( right.Value );
		}

		/// <summary>Static comparison for <see cref="IValidatedString" /> and <see cref="String" />.</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] IValidatedString? left, [CanBeNull] String? right ) {
			if ( left?.Value is null || right is null ) {
				return default( Boolean );
			}

			return left.Value.SequenceEqual( right );
		}

		[CanBeNull]
		public static implicit operator String?( [CanBeNull] ValidatedString? str ) => str?.Value;

		public static Boolean operator !=( [CanBeNull] ValidatedString? left, [CanBeNull] IValidatedString? right ) => !Equals( left, right?.Value );

		public static Boolean operator ==( [CanBeNull] ValidatedString? left, [CanBeNull] IValidatedString? right ) => Equals( left, right?.Value );

		public override Boolean Equals( [CanBeNull] Object? obj ) => Equals( this.Value, obj as IValidatedString );

		public override Int32 GetHashCode() => this.Value.GetHashCode();

		[NotNull]
		public override String ToString() => this.Value;

	}

}