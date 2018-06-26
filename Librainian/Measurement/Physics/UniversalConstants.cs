// Copyright © Rick@AIBrain.Org and Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our source code, binaries, libraries, projects, or solutions.
//
// This source code contained in "UniversalConstants.cs" belongs to Protiguous@Protiguous.com
// and Rick@AIBrain.org and unless otherwise specified or the original license has been
// overwritten by automatic formatting.
// (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other projects still retain their original
// license and our Thanks goes to those Authors. If you find your code in this source code, please
// let us know so we can properly attribute you and include the proper license and/or copyright.
//
// If you want to use any of our code, you must contact Protiguous@Protiguous.com or
// Sales@AIBrain.org for permission and a quote.
//
// Donations are accepted (for now) via
//    bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//    paypal@AIBrain.Org
//    (We're still looking into other solutions! Any ideas?)
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
// Our website can be found at "https://Protiguous.com/"
// Our software can be found at "https://Protiguous.Software/"
// Our GitHub address is "https://github.com/Protiguous".
// Feel free to browse any source code we *might* make available.
//
// ***  Project "Librainian"  ***
// File "UniversalConstants.cs" was last formatted by Protiguous on 2018/06/26 at 1:25 AM.

namespace Librainian.Measurement.Physics {

	using System;
	using Maths;

	public static class UniversalConstants {

		/// <summary>
		/// </summary>
		/// <seealso cref="http://wikipedia.org/wiki/Elementary_charge" />
		public static ElectronVolts ElementaryCharge { get; } = new ElectronVolts( 0.0000000000000000001602176565m );

		public static Decimal HalfSpin { get; } = ( Decimal ) Constants.OneOverTwo;

		/// <summary>
		///     Precalculated -1/3 of <see cref="ElementaryCharge" />.
		/// </summary>
		public static ElectronVolts NegativeOneThirdElementaryCharge { get; } = -1m * ElementaryCharge / 3m;

		/// <summary>
		///     Precalculated -2/3 of <see cref="ElementaryCharge" />.
		/// </summary>
		public static ElectronVolts NegativeTwoThirdsElementaryCharge { get; } = -2m * ElementaryCharge / 3m;

		public static ElectronVolts PositiveOneElementaryCharge { get; } = 1m * ElementaryCharge;

		/// <summary>
		///     Precalculated 1/3 of <see cref="ElementaryCharge" />.
		/// </summary>
		public static ElectronVolts PositiveOneThirdElementaryCharge { get; } = 1m * ElementaryCharge / 3m;

		public static ElectronVolts PositiveTwoElementaryCharge { get; } = 2m * ElementaryCharge;

		/// <summary>
		///     Precalculated +2/3 of <see cref="ElementaryCharge" />.
		/// </summary>
		public static ElectronVolts PositiveTwoThirdsElementaryCharge { get; } = 2m * ElementaryCharge / 3m;

		/// <summary>
		///     Precalculated +0 of <see cref="ElementaryCharge" />.
		/// </summary>
		public static ElectronVolts ZeroElementaryCharge { get; } = 0.0M * ElementaryCharge;
	}
}