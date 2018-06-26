// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Next.cs" belongs to Rick@AIBrain.org and
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
// File "Next.cs" was last formatted by Protiguous on 2018/06/04 at 4:14 PM.

namespace Librainian.Measurement.Time.FluentTime {

	using System;

	public static class Next {

		private static DateTime GetNextOfDay( DayOfWeek dayOfWeek ) {
			var today = AdjustableCurrentTime.Today;
			var delta = dayOfWeek - today.DayOfWeek;

			var result = today.AddDays( delta <= 0 ? delta + 7 : delta );

			return result;
		}

		public static DateTime Friday() => GetNextOfDay( DayOfWeek.Friday );

		public static DateTime Monday() => GetNextOfDay( DayOfWeek.Monday );

		public static DateTime Saturday() => GetNextOfDay( DayOfWeek.Saturday );

		public static DateTime Sunday() => GetNextOfDay( DayOfWeek.Sunday );

		public static DateTime Thursday() => GetNextOfDay( DayOfWeek.Thursday );

		public static DateTime Tuesday() => GetNextOfDay( DayOfWeek.Tuesday );

		public static DateTime Wednesday() => GetNextOfDay( DayOfWeek.Wednesday );

	}

}