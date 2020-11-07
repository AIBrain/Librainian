#nullable enable

namespace Librainian.Collections {

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	public class StringBidirectionalDictionary : BidirectionalDictionary<String, String> {

		public StringBidirectionalDictionary( IDictionary<String, String> firstToSecondDictionary ) : base( firstToSecondDictionary ) { }

		[SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase" )]
		public override Boolean ExistsInSingle( String value ) => base.ExistsInSingle( value.ToLowerInvariant() );

		[SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase" )]
		public override Boolean ExistsInPlural( String value ) => base.ExistsInPlural( value.ToLowerInvariant() );

		[SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase" )]
		public override String GetSingle( String value ) => base.GetSingle( value.ToLowerInvariant() );

		[SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase" )]
		public override String GetPlural( String value ) => base.GetPlural( value.ToLowerInvariant() );

	}

}