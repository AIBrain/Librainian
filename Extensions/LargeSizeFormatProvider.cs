// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "LargeSizeFormatProvider.cs" belongs to Rick@AIBrain.org and
// Protiguous@Protiguous.com unless otherwise specified or the original license has
// been overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
// 
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
// 
// Donations, royalties from any software that uses any of our code, or license fees can be paid
// to us via bitcoin at the address 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2.
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
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we might have available.
// 
// ***  Project "Librainian"  ***
// File "LargeSizeFormatProvider.cs" was last formatted by Protiguous on 2018/06/04 at 3:52 PM.

namespace Librainian.Extensions {

	using System;
	using JetBrains.Annotations;
	using Maths;

	public class LargeSizeFormatProvider : IFormatProvider, ICustomFormatter {

		public String Format( String format, Object arg, IFormatProvider formatProvider ) {
			if ( format?.StartsWith( FileSizeFormat ) != true ) { return DefaultFormat( format, arg, formatProvider ); }

			if ( arg is String ) { return DefaultFormat( format, arg, formatProvider ); }

			Single size;

			try { size = Convert.ToUInt64( arg ); }
			catch ( InvalidCastException ) { return DefaultFormat( format, arg, formatProvider ); }

			var suffix = "n/a";

			if ( size.Between( Constants.Sizes.OneTeraByte, UInt64.MaxValue ) ) {
				size /= Constants.Sizes.OneTeraByte;
				suffix = "trillion";
			}
			else if ( size.Between( Constants.Sizes.OneGigaByte, Constants.Sizes.OneTeraByte ) ) {
				size /= Constants.Sizes.OneGigaByte;
				suffix = "billion";
			}
			else if ( size.Between( Constants.Sizes.OneMegaByte, Constants.Sizes.OneGigaByte ) ) {
				size /= Constants.Sizes.OneMegaByte;
				suffix = "million";
			}
			else if ( size.Between( Constants.Sizes.OneKiloByte, Constants.Sizes.OneMegaByte ) ) {
				size /= Constants.Sizes.OneKiloByte;
				suffix = "thousand";
			}
			else if ( size.Between( UInt64.MinValue, Constants.Sizes.OneKiloByte ) ) { suffix = ""; }

			return $"{size:N3} {suffix}";
		}

		public Object GetFormat( [NotNull] Type formatType ) {
			if ( formatType is null ) { throw new ArgumentNullException( nameof( formatType ) ); }

			return formatType == typeof( ICustomFormatter ) ? this : null;
		}

		private static String DefaultFormat( String format, Object arg, IFormatProvider formatProvider ) {
			var formattableArg = arg as IFormattable;

			return formattableArg?.ToString( format, formatProvider ) ?? arg.ToString();
		}

		private const String FileSizeFormat = "fs";

	}

}