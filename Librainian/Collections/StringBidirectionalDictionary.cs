#nullable enable

namespace Librainian.Collections {

	using System;
	using System.Collections.Generic;

	public class StringBidirectionalDictionary : BidirectionalDictionary<String, String> {

		public StringBidirectionalDictionary( IDictionary<String, String> firstToSecondDictionary ) : base( firstToSecondDictionary ) { }

		public override Boolean ExistsInSingle( String value ) => base.ExistsInSingle( value.ToLowerInvariant() );

		public override Boolean ExistsInPlural( String value ) => base.ExistsInPlural( value.ToLowerInvariant() );

		public override String GetSingle( String value ) => base.GetSingle( value.ToLowerInvariant() );

		public override String GetPlural( String value ) => base.GetPlural( value.ToLowerInvariant() );

	}

}