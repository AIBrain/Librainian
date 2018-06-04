// Copyright © 1995-2018 to Rick@AIBrain.org and Protiguous. All Rights Reserved.
// 
// This entire copyright notice and license must be retained and must be kept visible
// in any binaries, libraries, repositories, and source code (directly or derived) from
// our binaries, libraries, projects, or solutions.
// 
// This source code contained in "Extensions.cs" belongs to Rick@AIBrain.org and
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
// File "Extensions.cs" was last formatted by Protiguous on 2018/06/04 at 4:10 PM.

namespace Librainian.Measurement.Physics {

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using JetBrains.Annotations;

	public static class Extensions {

		public static String Simpler( this ElectronVolts volts ) {
			var list = new HashSet<String> {
				volts.ToTeraElectronVolts().ToString(),
				volts.ToGigaElectronVolts().ToString(),
				volts.ToMegaElectronVolts().ToString(),
				volts.ToElectronVolts().ToString(),
				volts.ToMilliElectronVolts().ToString()
			};

			return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
		}

		public static String Simpler( this KiloElectronVolts volts ) {
			var list = new HashSet<String> {
				volts.ToTeraElectronVolts().ToString(),
				volts.ToGigaElectronVolts().ToString(),
				volts.ToMegaElectronVolts().ToString(),
				volts.ToElectronVolts().ToString(),
				volts.ToMilliElectronVolts().ToString()
			};

			return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
		}

		public static String Simpler( this MegaElectronVolts volts ) {
			var list = new HashSet<String> {
				volts.ToTeraElectronVolts().ToString(),
				volts.ToGigaElectronVolts().ToString(),
				volts.ToMegaElectronVolts().ToString(),
				volts.ToElectronVolts().ToString(),
				volts.ToMilliElectronVolts().ToString()
			};

			return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
		}

		public static String Simpler( this GigaElectronVolts volts ) {
			var list = new HashSet<String> {
				volts.ToTeraElectronVolts().ToString(),
				volts.ToGigaElectronVolts().ToString(),
				volts.ToMegaElectronVolts().ToString(),
				volts.ToElectronVolts().ToString(),
				volts.ToMilliElectronVolts().ToString()
			};

			return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
		}

		public static String Simpler( this TeraElectronVolts volts ) {
			var list = new HashSet<String> {
				volts.ToTeraElectronVolts().ToString(),
				volts.ToGigaElectronVolts().ToString(),
				volts.ToMegaElectronVolts().ToString(),
				volts.ToElectronVolts().ToString(),
				volts.ToMilliElectronVolts().ToString()
			};

			return list.OrderBy( s => s.Length ).FirstOrDefault() ?? "n/a";
		}

		public static MegaElectronVolts Sum( [NotNull] this IEnumerable<ElectronVolts> volts ) {
			if ( volts is null ) { throw new ArgumentNullException( nameof( volts ) ); }

			var result = volts.Aggregate( MegaElectronVolts.Zero, ( current, electronVolts ) => current + electronVolts.ToMegaElectronVolts() );

			return result;
		}

		public static GigaElectronVolts Sum( [NotNull] this IEnumerable<MegaElectronVolts> volts ) {
			if ( volts is null ) { throw new ArgumentNullException( nameof( volts ) ); }

			return volts.Aggregate( GigaElectronVolts.Zero, ( current, megaElectronVolts ) => current + megaElectronVolts.ToGigaElectronVolts() );
		}

		public static TeraElectronVolts Sum( [NotNull] this IEnumerable<GigaElectronVolts> volts ) {
			if ( volts is null ) { throw new ArgumentNullException( nameof( volts ) ); }

			return volts.Aggregate( TeraElectronVolts.Zero, ( current, gigaElectronVolts ) => current + gigaElectronVolts.ToTeraElectronVolts() );
		}

		public static Expression<Func<Double, Double>> FahrenheitToCelsius = fahrenheit => ( fahrenheit - 32.0 ) * 5.0 / 9.0;

	}

}