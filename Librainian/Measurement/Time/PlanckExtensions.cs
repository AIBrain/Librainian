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
// File "PlanckExtensions.cs" last formatted on 2020-08-14 at 8:38 PM.

namespace Librainian.Measurement.Time {

	using System;
	using System.Numerics;
	using ExtendedNumerics;
	using Rationals;

	public static class PlanckExtensions {

		/// <summary>Given the <paramref name="constant" />, reduce <paramref name="planckTimes" />, and return the remainder.</summary>
		/// <param name="constant"></param>
		/// <param name="planckTimes"></param>
		/// <returns></returns>
		public static BigInteger PullPlancks( this BigInteger constant, ref BigInteger planckTimes ) {
			var integer = BigInteger.Divide( planckTimes, constant );
			planckTimes -= BigInteger.Multiply( integer, constant );

			return integer;
		}

		/// <summary>Given the <paramref name="constant" />, reduce <paramref name="planckTimes" />, and return the remainder.</summary>
		/// <param name="constant"></param>
		/// <param name="planckTimes"></param>
		/// <returns></returns>
		public static BigInteger PullPlancks( this Double constant, ref BigInteger planckTimes ) {
			var right = new Rational( new BigInteger( constant ) );
			var plancks = Rational.Divide( planckTimes, right );
			planckTimes -= ( plancks * right ).WholePart;

			return plancks.WholePart;
		}

		/// <summary>Given the <paramref name="constant" />, reduce <paramref name="planckTimes" />, and return the remainder.</summary>
		/// <param name="constant"></param>
		/// <param name="planckTimes"></param>
		/// <returns></returns>
		public static BigInteger PullPlancks( this BigDecimal constant, ref BigInteger planckTimes ) {
			var reduce = ( BigInteger )constant;

			planckTimes -= reduce;

			return reduce;
		}

		/*

        /// <summary>
        ///     Given the <paramref name="constant" />, reduce <paramref name="planckTimes" />, and return the remainder.
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="planckTimes"></param>
        /// <returns></returns>
        public static BigInteger PullPlancks( this Double constant, ref BigInteger planckTimes ) {
            var pullPlancks = Rational.Divide( planckTimes, constant );
            planckTimes -= ( BigInteger )Rational.Multiply( pullPlancks, constant );

            return ( BigInteger )pullPlancks;
        }
        */
	}
}