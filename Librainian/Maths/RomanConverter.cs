// Copyright � Protiguous. All Rights Reserved.
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
// File "RomanConverter.cs" last formatted on 2020-08-14 at 8:36 PM.

namespace Librainian.Maths {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using JetBrains.Annotations;
	using Numbers;

	/// <summary>Based on the idea from lavz24.</summary>
	/// <see cref="http://github.com/lavz24/DecimalToRoman/blob/master/DecimalToRoman/Converter.cs" />
	public static class RomanConverter {

		/// <summary></summary>
		public static readonly RomanNumber[] RomanValues = {
			RomanNumber.I, RomanNumber.IV, RomanNumber.V, RomanNumber.IX, RomanNumber.X, RomanNumber.XL, RomanNumber.L, RomanNumber.XC, RomanNumber.C, RomanNumber.CD,
			RomanNumber.D, RomanNumber.CM, RomanNumber.M
		};

		/// <summary>
		///     <para>Returns the roman numeral for a <paramref name="number" /> between 1 and 3999.</para>
		///     <para>Or String.Empty in case of failure.</para>
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		[CanBeNull]
		public static String ToRoman( this UInt32 number ) {
			var result = new Queue<RomanNumber>( number.ToString().Length * 2 );

			var currentRoman = RomanValues.Length - 1;

			for ( var i = number; i > 0; ) {
				if ( i < ( UInt32 )RomanValues[currentRoman] ) {
					--currentRoman;
				}
				else {
					result.Enqueue( RomanValues[currentRoman] );
					i -= ( UInt32 )RomanValues[currentRoman];
				}
			}

			return result.Aggregate( String.Empty, ( current, romanNumber ) => current + romanNumber );
		}

	}

}