﻿// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories,
// or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten
// by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to
// those Authors. If you find your code unattributed in this source code, please let us know so we can properly attribute you
// and include the proper license and/or copyright(s). If you want to use any of our code in a commercial project, you must
// contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
// ====================================================================
// Disclaimer:  Usage of the source code or binaries is AS-IS. No warranties are expressed, implied, or given. We are NOT
// responsible for Anything You Do With Our Code. We are NOT responsible for Anything You Do With Our Executables. We are NOT
// responsible for Anything You Do With Your Computer. ====================================================================
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com. Our software can be found at
// "https://Protiguous.com/Software/" Our GitHub address is "https://github.com/Protiguous".
//
// File "PhysicsExtensions.cs" last formatted on 2021-11-30 at 7:19 PM by Protiguous.

#nullable enable

namespace Librainian.Measurement.Physics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Exceptions;

public static class PhysicsExtensions {

	public static Expression<Func<Decimal, Decimal>> FahrenheitToCelsius { get; } = fahrenheit => ( fahrenheit - 32.0m ) * 5.0m / 9.0m;

	public static String Simpler( this ElectronVolts volts ) {
		var list = new HashSet<String> {
			volts.ToTeraElectronVolts().ToString(),
			volts.ToGigaElectronVolts().ToString(),
			volts.ToMegaElectronVolts().ToString(),
			volts.ToElectronVolts().ToString(),
			volts.ToMilliElectronVolts().ToString()
		};

		return list.OrderBy( s => s.Length ).First();
	}

	public static String Simpler( this KiloElectronVolts volts ) {
		var list = new HashSet<String> {
			volts.ToTeraElectronVolts().ToString(),
			volts.ToGigaElectronVolts().ToString(),
			volts.ToMegaElectronVolts().ToString(),
			volts.ToElectronVolts().ToString(),
			volts.ToMilliElectronVolts().ToString()
		};

		return list.OrderBy( s => s.Length ).First();
	}

	public static String Simpler( this MegaElectronVolts volts ) {
		var list = new HashSet<String> {
			volts.ToTeraElectronVolts().ToString(),
			volts.ToGigaElectronVolts().ToString(),
			volts.ToMegaElectronVolts().ToString(),
			volts.ToElectronVolts().ToString(),
			volts.ToMilliElectronVolts().ToString()
		};

		return list.OrderBy( s => s.Length ).First();
	}

	public static String Simpler( this GigaElectronVolts volts ) {
		var list = new HashSet<String> {
			volts.ToTeraElectronVolts().ToString(),
			volts.ToGigaElectronVolts().ToString(),
			volts.ToMegaElectronVolts().ToString(),
			volts.ToElectronVolts().ToString(),
			volts.ToMilliElectronVolts().ToString()
		};

		return list.OrderBy( s => s.Length ).First();
	}

	public static String Simpler( this TeraElectronVolts volts ) {
		var list = new HashSet<String> {
			volts.ToTeraElectronVolts().ToString(),
			volts.ToGigaElectronVolts().ToString(),
			volts.ToMegaElectronVolts().ToString(),
			volts.ToElectronVolts().ToString(),
			volts.ToMilliElectronVolts().ToString()
		};

		return list.OrderBy( s => s.Length ).First();
	}

	public static MegaElectronVolts Sum( this IEnumerable<ElectronVolts> volts ) {
		if ( volts is null ) {
			throw new NullException( nameof( volts ) );
		}

		return volts.Aggregate( MegaElectronVolts.Zero, ( current, electronVolts ) => current + electronVolts.ToMegaElectronVolts() );
	}

	public static GigaElectronVolts Sum( this IEnumerable<MegaElectronVolts> volts ) {
		if ( volts is null ) {
			throw new NullException( nameof( volts ) );
		}

		return volts.Aggregate( GigaElectronVolts.Zero, ( current, megaElectronVolts ) => current + megaElectronVolts.ToGigaElectronVolts() );
	}

	public static TeraElectronVolts Sum( this IEnumerable<GigaElectronVolts> volts ) {
		if ( volts is null ) {
			throw new NullException( nameof( volts ) );
		}

		return volts.Aggregate( TeraElectronVolts.Zero, ( current, gigaElectronVolts ) => current + gigaElectronVolts.ToTeraElectronVolts() );
	}
}