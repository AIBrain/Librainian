#nullable enable

namespace Librainian.Collections {

	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using JetBrains.Annotations;

	/// <summary>
	///     This class provide service for both the singularization and pluralization, it takes the word pairs
	///     in the ctor following the rules that the first one is singular and the second one is plural.
	/// </summary>
	public class BidirectionalDictionary<TSingle, TPlural> where TSingle : class where TPlural : class {

		public BidirectionalDictionary( IDictionary<TSingle, TPlural> firstToSecondDictionary ) {
			foreach ( var pair in firstToSecondDictionary ) {
				this.AddValue( pair );
			}
		}

		public BidirectionalDictionary() { }

		[NotNull] private ConcurrentDictionary<TSingle, TPlural?> SingleToPlural { get; } = new ConcurrentDictionary<TSingle, TPlural?>();

		[NotNull] private ConcurrentDictionary<TPlural, TSingle?> PluralToSingle { get; } = new ConcurrentDictionary<TPlural, TSingle?>();

		public virtual Boolean ExistsInSingle( TSingle value ) => this.SingleToPlural.ContainsKey( value );

		public virtual Boolean ExistsInPlural( TPlural value ) => this.PluralToSingle.ContainsKey( value );

		public virtual TPlural? GetPlural( [NotNull]
		                                   TSingle value ) =>
			this.SingleToPlural.TryGetValue( value, out var second ) ? second : default;

		public virtual TSingle? GetSingle( [NotNull]
		                                   TPlural value ) =>
			this.PluralToSingle.TryGetValue( value, out var first ) ? first : default;

		public void AddValue( [NotNull]
		                      TSingle firstValue, [NotNull]
		                      TPlural secondValue ) {
			this.SingleToPlural[firstValue] = secondValue;

			this.PluralToSingle[secondValue] = firstValue;

		}

		public void AddValue( (TSingle firstValue, TPlural secondValue) values ) => this.AddValue( values.firstValue, values.secondValue );

		public void AddValue( KeyValuePair<TSingle, TPlural> pair ) => this.AddValue( pair.Key!, pair.Value! );

	}

}