// Copyright © Protiguous. All Rights Reserved.
//
// This entire copyright notice and license must be retained and must be kept visible in any binaries, libraries, repositories, or source code (directly or derived) from our binaries, libraries, projects, solutions, or applications.
//
// All source code belongs to Protiguous@Protiguous.com unless otherwise specified or the original license has been overwritten by formatting. (We try to avoid it from happening, but it does accidentally happen.)
//
// Any unmodified portions of source code gleaned from other sources still retain their original license and our thanks goes to those Authors.
// If you find your code unattributed in this source code, please let us know so we can properly attribute you and include the proper license and/or copyright(s).
// If you want to use any of our code in a commercial project, you must contact Protiguous@Protiguous.com for permission, license, and a quote.
//
// Donations, payments, and royalties are accepted via bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2 and PayPal: Protiguous@Protiguous.com
//
//
// Disclaimer:  Usage of the source code or binaries is AS-IS.
// No warranties are expressed, implied, or given.
// We are NOT responsible for Anything You Do With Our Code.
// We are NOT responsible for Anything You Do With Our Executables.
// We are NOT responsible for Anything You Do With Your Computer.
//
//
// Contact us by email if you have any questions, helpful criticism, or if you would like to use our code in your project(s).
// For business inquiries, please contact me at Protiguous@Protiguous.com.
// Our software can be found at "https://Protiguous.com/Software/"
// Our GitHub address is "https://github.com/Protiguous".
//
// File "AtomicDouble.cs" last formatted on 2022-12-22 at 4:22 AM by Protiguous.

namespace Librainian.Maths.Numbers;

using System;
using System.ComponentModel;
using System.Threading;
using Exceptions;
using Newtonsoft.Json;

/// <summary>A Double number thread-safe by <see cref="Interlocked" />.</summary>
[JsonObject]
[Description( "A Double number thread-safe by Interlocked." )]
public struct AtomicDouble {

	/// <summary>ONLY used in the getter and setter.</summary>
	[JsonProperty]
	private Double _value;

	public AtomicDouble( Double value ) : this() => this.Value = value;

	public Double Value {
		get => Interlocked.Exchange( ref this._value, this._value );

		set => Interlocked.Exchange( ref this._value, value );
	}

	public static implicit operator Double( AtomicDouble special ) => special.Value;

	public static AtomicDouble operator -( AtomicDouble a1, AtomicDouble a2 ) => new( a1.Value - a2.Value );

	public static AtomicDouble operator *( AtomicDouble a1, AtomicDouble a2 ) => new( a1.Value * a2.Value );

	public static AtomicDouble operator +( AtomicDouble a1, AtomicDouble a2 ) => new( a1.Value + a2.Value );

	public static AtomicDouble operator ++( AtomicDouble a1 ) {
		a1.Value++;

		return a1;
	}

	public static AtomicDouble Parse( String value ) {
		if ( value is null ) {
			throw new ArgumentEmptyException( nameof( value ) );
		}

		return new AtomicDouble( Double.Parse( value ) );
	}

	/// <summary>Resets the value to zero if less than zero;</summary>
	public void CheckReset() {
		if ( this.Value < 0 ) {
			this.Value = 0;
		}
	}

	public override String ToString() => $"{this.Value:R}";
}