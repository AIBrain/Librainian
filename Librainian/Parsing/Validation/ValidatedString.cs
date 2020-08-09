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

		public Int32 CompareTo( [CanBeNull] String? other ) => String.Compare( this.Value, other, StringComparison.Ordinal );

		public Int32 CompareTo( [NotNull] IValidatedString other ) => String.Compare( this.Value, other.Value, StringComparison.Ordinal );

		public Boolean Equals( String other ) => Equals( this, other );

		public Boolean Equals( IValidatedString other ) => Equals( this.Value, other );

		public IEnumerator<Char> GetEnumerator() => ( ( IEnumerable<Char> )this.Value ).GetEnumerator();

		Int32 IComparable.CompareTo( [CanBeNull] Object? obj ) => this.Value.CompareTo( obj );

		IEnumerator IEnumerable.GetEnumerator() => ( ( IEnumerable )this.Value ).GetEnumerator();

		[SecuritySafeCritical]
		public static Int32 Compare( [NotNull] ValidatedString left, [NotNull] IValidatedString right, StringComparison comparisonType = StringComparison.Ordinal ) =>
			comparisonType == StringComparison.Ordinal ? String.CompareOrdinal( left, right.Value ) : String.Compare( left.Value, right.Value, comparisonType );

		public static Int32 Compare( [NotNull] ValidatedString left, Int32 leftIndex, [NotNull] IValidatedString right, Int32 rightIndex, Int32 length ) =>
			String.Compare( left.Value, leftIndex, right.Value, rightIndex, length );

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
				return default;
			}

			if ( left.Value is null || right.Value is null ) {
				return default;
			}

			if ( left.Value.Length != right.Value.Length ) {
				return default;
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
				return default;
			}

			return left.SequenceEqual( right.Value );
		}

		/// <summary>Static comparison for <see cref="IValidatedString" /> and <see cref="String" />.</summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static Boolean Equals( [CanBeNull] IValidatedString? left, [CanBeNull] String? right ) {
			if ( left?.Value is null || right is null ) {
				return default;
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